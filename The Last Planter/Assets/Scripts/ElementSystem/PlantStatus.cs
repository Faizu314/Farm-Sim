using System.Collections.Generic;
using UnityEngine;
using PlantTransport;

[System.Serializable]
public class PlantStatus
{
    [SerializeField] private List<CompoundAssociation> compoundAssociations;
    [SerializeField] [Range(0.01f, 1f)] private float surfaceArea;
    [SerializeField] [Range(0f, 1f)] private float absorptionStrength;
    [SerializeField] private float maxOsmosis;

    public List<float> compoundContents;
    private List<float> deltaContents;

    private enum Minerals { H2o, N, Po4, K, Ca };


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
        if (CompoundTransport.GetWaterSolubility(otherCompoundContents) < CompoundTransport.MIN_WATER_SOLUBILITY)
            return;
        float totalArea = surfaceArea * areaMultiplier;
        float amount;

        for (int i = 1; i < compoundContents.Count; i++)
        {
            amount = CompoundTransport.Diffuse(otherCompoundContents[i], compoundContents[i], totalArea);
            if (fromSustainer && amount < 0.01f && absorptionStrength > 0f)
            {
                amount = CompoundTransport.ActiveTransport(otherCompoundContents[i], compoundContents[i], absorptionStrength, totalArea);
            }
            deltaContents[i] = amount * deltaTime;
        }
        for (int i = 1; i < compoundContents.Count; i++)
        {
            compoundContents[i] += deltaContents[i];
            deltaContents[i] *= -1f;
        }

        amount = CompoundTransport.Osmosis(otherCompoundContents[0], compoundContents[0], totalArea, maxOsmosis) * deltaTime;
        compoundContents[0] += amount;
        deltaContents[0] = amount * -1f;

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
                wellBeing += 0.1f;
            else
                wellBeing -= 0.2f;
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
    private bool InRange(float amount, float min, float max)
    {
        return amount <= max && amount > min;
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

    [System.Serializable] public struct CompoundAssociation
    {
        public string compoundName;
        [Range(0f, 10f)] public float excessLevel;
        [Range(0f, 10f)] public float deficiencyLevel;
    }
}