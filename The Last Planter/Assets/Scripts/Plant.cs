using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    public Pot myPot;
    public Transform roots;
    public Transform leaves;
    public Material rootsMat;
    public Material leavesMat;

    [Header("Plant Statistics")]
    [Range(0.01f, 1f)] public float surfaceAreaOfRoots;
    [Range(0.01f, 1f)] public float surfaceAreaOfLeaves;
    public List<CompoundAssociation> mineralAssociations;
    public CompoundAssociation waterAssociation;
    public float maxOsmosis;

    [Header("Battle Behaviour")]
    public IAttack attackScript;

    [Header("Debug")]
    public float diffusionRate;
    public float minWaterSolubility;

    private enum Minerals { N, Po4, K, Ca, Ma, S };


    // Plant State
    private List<float> mineralConcentration = new List<float>();
    private float waterContent;
    private float leavesGrowth;
    private float rootsGrowth;
    private float flowerGrowth;
    private int growthStage;
    private float timeSinceAbsorb;
    private float growTick = 0f;

    // Temp
    private List<float> concGradients = new List<float>();

    private void Start()
    {
        InitializeState();
    }

    private void InitializeState()
    {
        leavesGrowth = rootsGrowth = flowerGrowth = 0.1f;
        waterContent = timeSinceAbsorb = growthStage = 0;
        for (int i = 0; i < mineralAssociations.Count; i++)
        {
            mineralConcentration.Add(0f);
            concGradients.Add(0f);
        }
    }

    public void Absorb(List<float> soilMineralContent, float soilWaterContent)
    {
        if (GetWaterSolubility(soilMineralContent, soilWaterContent) < minWaterSolubility)
            return;
        AbsorbWater(soilWaterContent);
        AbsorbMinerals(soilMineralContent, soilWaterContent);
        timeSinceAbsorb = 0f;
    }

    private void AbsorbWater(float soilWaterContent)
    {
        float deltaWater = Osmosis(soilWaterContent, waterContent);
        waterContent += deltaWater;
        myPot.UpdateSoilWaterContent(deltaWater * -1f);
    }
    private void AbsorbMinerals(List<float> soilMineralContent, float soilWaterContent)
    {
        for (int i = 0; i < soilMineralContent.Count; i++)
        {
            float soilConcentration = soilMineralContent[i] / soilWaterContent;
            concGradients[i] = Diffuse(soilConcentration, mineralConcentration[i]);
            if (CanActiveTransport(i, concGradients[i]))
                concGradients[i] = ActiveTransport(concGradients[i], mineralAssociations[i].absorbtionStrength);
        }
        for (int i = 0; i < soilMineralContent.Count; i++)
        {
            mineralConcentration[i] += concGradients[i];
            concGradients[i] *= -1f;
        }
        myPot.UpdateSoilMineralContent(concGradients);
    }

    private void Grow()
    {
        GrowLeaves();

        GrowRoots();
    }

    private void GrowLeaves()
    {
        if (IsMineralAvailable((int)Minerals.N) && IsWaterAvailable())
        {
            UseMineral((int)Minerals.N, leavesGrowth + 1);
            UseWater(leavesGrowth + 1);
            leavesGrowth += 0.04f;
        }
    }
    private void GrowRoots()
    {
        if (IsMineralAvailable((int)Minerals.Po4) && IsMineralAvailable((int)Minerals.K) && IsWaterAvailable())
        {
            UseMineral((int)Minerals.Po4, rootsGrowth + 1);
            UseMineral((int)Minerals.K, rootsGrowth + 1);
            UseWater(rootsGrowth + 1);
            rootsGrowth += 0.04f;
        }
    }


    private bool CanActiveTransport(int mineral, float concGradient)
    {
        return (concGradient > 0) &&
               (mineralAssociations[mineral].absorbtionStrength != 0) && 
               (IsMineralAvailable((int)Minerals.K)) &&
               (IsMineralAvailable((int)Minerals.Po4));
    }
    private float ActiveTransport(float concGradient, float Vmax)
    {
        UseMineral((int)Minerals.Po4, Vmax);
        UseMineral((int)Minerals.K, Vmax);
        return Vmax / (1 + (surfaceAreaOfRoots * rootsGrowth / concGradient));
    }

    #region LowLevelFunctions
    private float Diffuse(float soilConc, float rootConc)
    {
        return (soilConc - rootConc) * surfaceAreaOfRoots * rootsGrowth * diffusionRate * timeSinceAbsorb;
    }
    private float Osmosis(float soilWater, float rootWater)
    {
        float amount = Diffuse(soilWater, rootWater);
        if (amount > maxOsmosis)
            amount = maxOsmosis;
        return amount;
    }
    private float GetWaterSolubility(List<float> soilMineralContent, float soilWaterContent)
    {
        float totalMineral = 0f;
        foreach (float content in soilMineralContent)
        {
            totalMineral += content;
        }
        float waterSolubility = soilWaterContent / totalMineral;
        return waterSolubility;
    }

    private bool IsMineralAvailable(int mineralIndex)
    {
        return mineralConcentration[mineralIndex] >= mineralAssociations[mineralIndex].consumption;
    }
    private void UseMineral(int mineralIndex, float multiplier = 1f)
    {
        mineralConcentration[mineralIndex] -= mineralAssociations[mineralIndex].consumption * multiplier;
    }
    private bool IsWaterAvailable()
    {
        return waterContent > waterAssociation.consumption;
    }
    private void UseWater(float multiplier = 1f)
    {
        waterContent -= waterAssociation.consumption * multiplier;
    }
    #endregion

    public void Debug()
    {
        leaves.localScale = Vector3.one * leavesGrowth;
        roots.localScale = Vector3.one * rootsGrowth;
    }
    private void TimeStep()
    {
        timeSinceAbsorb += Time.deltaTime;
        growTick += Time.deltaTime;
        if (growTick > 4f)
        {
            Grow();
            growTick = 0f;
        }
    }

    private void Maintain()
    {
        //CheckMineralDificiency();
        //CheckMineralExcess();
        //CheckWaterDificiency();
        //CheckWaterExcess();
    }
    private void Update()
    {
        TimeStep();
        Maintain();
        Debug();
    }

    [System.Serializable]
    public struct CompoundAssociation
    {
        public string compoundName;
        [Range(0f, 10f)] public float absorbtionStrength;
        [Range(0f, 1f)] public float excessLevel;
        [Range(0f, 1f)] public float deficiencyLevel;
        public float consumption;
    }
}
