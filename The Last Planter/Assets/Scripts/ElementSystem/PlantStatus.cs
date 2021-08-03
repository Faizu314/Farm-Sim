using System.Collections.Generic;
using UnityEngine;
using PlantSimulation;

[System.Serializable]
public class PlantStatus
{
    [SerializeField] private List<CompoundAssociation> compoundAssociations;
    [SerializeField] [Range(0.01f, 1f)] private float surfaceArea;
    [SerializeField] [Range(0f, 1f)] private float absorptionStrength;
    [SerializeField] [Range(0.01f, 1f)] private float maxOsmosis;

    public List<float> compoundContents;
    private List<float> deltaContents;

    public void Initialize()
    {
        compoundContents = new List<float>();
        deltaContents = new List<float>();
        for (int i = 0; i < compoundAssociations.Count; i++)
        {
            compoundContents.Add(0f);
            deltaContents.Add(0f);
        }
    }
    public void Reset()
    {
        Initialize();
    }

    public void ExchangeCompounds(ICompoundChannel other, bool fromSustainer, float deltaTime, float areaMultiplier = 1f)
    {
        List<float> otherCompoundContents = other.GetContent();
        float otherWaterSolubility = Transport.GetWaterSolubility(otherCompoundContents);
        float myWaterSolubility = Transport.GetWaterSolubility(GetContent());

        if (otherWaterSolubility < Transport.MIN_WATER_SOLUBILITY && myWaterSolubility < Transport.MIN_WATER_SOLUBILITY)
            return;

        float totalArea = surfaceArea * areaMultiplier;
        float mineralAmount;

        for (int i = 1; i < compoundContents.Count; i++)
        {
            mineralAmount = Transport.Diffuse(otherCompoundContents[i], compoundContents[i], totalArea);
            mineralAmount = Transport.ConserveCompound(otherCompoundContents[i], compoundContents[i], mineralAmount);
            if (fromSustainer && mineralAmount < 0.01f && absorptionStrength > 0f)
                mineralAmount = Transport.ActiveTransport(otherCompoundContents[i], compoundContents[i], absorptionStrength, totalArea);
            deltaContents[i] = mineralAmount * deltaTime;
        }
        for (int i = 1; i < compoundContents.Count; i++)
        {
            compoundContents[i] += deltaContents[i];
            deltaContents[i] *= -1f;
        }

        float waterAmount = Transport.Osmosis(otherWaterSolubility, myWaterSolubility, maxOsmosis) * deltaTime;
        waterAmount = Transport.ConserveCompound(otherCompoundContents[0], compoundContents[0], waterAmount);
        compoundContents[0] += waterAmount;
        deltaContents[0] = waterAmount * -1f;
        other.UpdateContent(deltaContents);
    }
    public float GetWellBeing()
    {
        float wellBeing = 0f;
        for (int i = 0; i < compoundContents.Count; i++)
        {
            float content = compoundContents[i];
            float minLevel = compoundAssociations[i].deficiencyLevel;
            float maxLevel = compoundAssociations[i].excessLevel;
            if (InRange(content, minLevel, maxLevel))
                wellBeing += 0.4f;
            else
            {
                wellBeing = -1f;
                break;
            }
        }
        return wellBeing;
    }
    public (int, bool) GetSymptom()
    {
        for (int i = 0; i < compoundContents.Count; i++)
        {
            float content = compoundContents[i];
            float minLevel = compoundAssociations[i].deficiencyLevel;
            float maxLevel = compoundAssociations[i].excessLevel;
            if (!InRange(content, minLevel, maxLevel))
                return (i, content > maxLevel);
        }
        return (-1, false);
    }
    public bool HasSymptom(int compoundIndex)
    {
        float content = compoundContents[compoundIndex];
        float minLevel = compoundAssociations[compoundIndex].deficiencyLevel;
        float maxLevel = compoundAssociations[compoundIndex].excessLevel;
        return InRange(content, minLevel, maxLevel);
    }
    private bool InRange(float amount, float min, float max)
    {
        return amount <= max && amount >= min;
    }

    #region ICompoundChannel
    public List<float> GetContent()
    {
        return compoundContents;
    }
    public float GetContent(int index)
    {
        return compoundContents[index];
    }
    public void UpdateContent(List<float> deltaContent)
    {
        for (int i = 0; i < deltaContent.Count; i++)
        {
            compoundContents[i] += deltaContent[i];
        }
    }
    public void UpdateContent(int index, float deltaContent)
    {
        compoundContents[index] += deltaContent;
    }
    #endregion

    [System.Serializable]
    public struct CompoundAssociation
    {
        public string compoundName;
        [Range(0f, 10f)] public float excessLevel;
        [Range(0f, 10f)] public float deficiencyLevel;
    }
}