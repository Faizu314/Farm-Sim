using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(GrowGraphics))]
public class TomatoStem : PlantElement
{
    [Header("Grow")]
    [SerializeField] private GrowGraphics growGraphics;
    [SerializeField] [Range(0, 4)] private int maxSoilHardness;

    [SerializeField] private List<LeavesData> outputData;

    private List<PlantElement> outputPEs = new List<PlantElement>();
    private float diffMultiplier;
    private int currentOutput;

    protected override void OnInitialize()
    {
        currentOutput = 0;
        diffMultiplier = 1f;
    }
    protected override void OnUpdate() { }

    protected override void Function(float deltaTime)
    {
        for (int i = 0; i < currentOutput; i++)
            ExchangeCompounds(outputPEs[i], deltaTime, diffMultiplier);

        if (currentOutput >= outputData.Count ||
            foodStore > outputData[currentOutput].requiredFood + 1f)
            GiveFood((PlantElement)sustainer, deltaTime * 0.1f);
    }
    protected override void Grow(float deltaTime)
    {
        if (currentOutput < outputData.Count)
        {
            if (growth <= outputData[currentOutput].requiredGrowth && foodStore < outputData[currentOutput].requiredFood)
                return;
        }
        if (foodStore >= growthFoodConsumption * deltaTime && soilHardness <= maxSoilHardness)
        {
            foodStore -= growthFoodConsumption * deltaTime;
            growth += DebugFloats.instance.growthIncrement * deltaTime;
            growGraphics.SetGrowth(growth);
        }
    }
    protected override void ShowSymptoms(int compoundIndex, bool isExcess)
    {
        Debug.Log(gameObject.name + " have inappropriate ammounts of: " + compoundIndex + ", " + isExcess);
    }
    protected override void HideSymptoms()
    {
        Debug.Log(gameObject.name + " mineral levels restored");
    }
    protected override bool ShouldOutput()
    {
        if (currentOutput >= outputData.Count)
            return false;
        if (foodStore < outputData[currentOutput].requiredFood)
            return false;
        if (growth < outputData[currentOutput].requiredGrowth)
            return false;
        return true;
    }
    protected override void Output()
    {
        outputData[currentOutput].prefab.SetActive(true);
        outputPEs.Add(outputData[currentOutput].prefab.GetComponent<PlantElement>());
        outputPEs[currentOutput].Initialize(this, environment, parentObject);
        foodStore -= outputData[currentOutput].requiredFood;

        currentOutput++;
        diffMultiplier = 1f / currentOutput;
    }
    [System.Serializable] private struct LeavesData
    {
        public GameObject prefab;
        public float requiredGrowth;
        public float requiredFood;
    }
}