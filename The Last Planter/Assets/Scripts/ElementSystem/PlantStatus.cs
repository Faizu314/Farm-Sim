using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlantTransport;
public class PlantStatus
{
    [SerializeField] private List<CompoundAssociation> compoundAssociations;
    [SerializeField] [Range(1f, 10f)] private float surfaceArea;
    [SerializeField] [Range(0.01f, 10f)] private float absorptionStrength;
    [SerializeField] private float maxOsmosis;

    private List<float> compoundContents;
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

    public void ExchangeCompound(ICompoundChannel other, float deltaTime, float areaMultiplier = 1f)
    {
        List<float> otherCompoundContents = other.GetContent();
        float totalArea = surfaceArea * areaMultiplier;
        float amount;

        for (int i = 1; i < compoundContents.Count; i++)
        {
            amount = CompoundTransport.Diffuse(otherCompoundContents[i], compoundContents[i], totalArea);
            if (amount < 0f && absorptionStrength > 0f)
            {
                //amount = CompoundTransport.ActiveTransport(otherCompoundContents[i], compoundContents[i], absorptionStrength, totalArea);
            }
            deltaContents[i] = amount * deltaTime;
        }
        for (int i = 1; i < compoundContents.Count; i++)
        {
            compoundContents[i] += deltaContents[i];
            deltaContents[i] = -1f;
        }

        amount = CompoundTransport.Osmosis(otherCompoundContents[0], compoundContents[0], totalArea, maxOsmosis) * deltaTime;
        compoundContents[0] += amount;
        deltaContents[0] = amount * -1f;

        other.UpdateContent(deltaContents);
    }
    public float GetWellBeing()
    {
        //Checks the excess or lack of minerals and returns a float that represents how good the plantElement is doing
        return 0f;
    }
    public void ShowSymptom()
    {
        //If health is low this will check what the plant is suffering from and trigger the right animation
    }

    public List<float> GetContent()
    {
        return compoundContents;
    }
    public void UpdateContent(List<float> deltaContents)
    {
        for (int i = 0; i < deltaContents.Count; i++)
        {
            compoundContents[i] += deltaContents[i];
        }
    }

    [System.Serializable]
    public struct CompoundAssociation
    {
        public string compoundName;
        [Range(0f, 10f)] public float excessLevel;
        [Range(0f, 10f)] public float deficiencyLevel;
    }
}
