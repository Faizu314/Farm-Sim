using System.Collections.Generic;
using UnityEngine;

namespace PlantSimulation
{
    public static class Transport
    {
        public static readonly float MIN_WATER_SOLUBILITY = 0.2f;
        public static float Diffuse(float otherContent, float myContent, float surfaceArea)
        {
            float amount = (otherContent - myContent) / (otherContent + myContent + 0.01f) * surfaceArea;
            return amount;
        }
        public static float Osmosis(float otherWaterSolubility, float myWaterSolubility, float surfaceArea, float maxOsmosis)
        {
            float amount = Diffuse(otherWaterSolubility, myWaterSolubility, surfaceArea);
            if (Mathf.Abs(amount) > maxOsmosis)
                amount = Mathf.Sign(amount) * maxOsmosis;
            return amount;
        }
        public static float GetWaterSolubility(List<float> mineralContent)
        {
            if (mineralContent[0] == 0)
                return 0f;
            float totalMineral = 0f;
            for (int i = 1; i < mineralContent.Count; i++)
            {
                totalMineral += mineralContent[i];
            }
            float waterSolubility = mineralContent[0] / totalMineral;
            return waterSolubility;
        }
        public static float ActiveTransport(float otherContent, float myContent, float absorptionStrength, float surfaceArea)
        {
            return absorptionStrength * surfaceArea * otherContent / (myContent + 1f);
        }
        public static float ConserveCompound(float otherContent, float myContent, float transferAmount)
        {
            if (transferAmount > 0f && transferAmount > otherContent)
                transferAmount = otherContent;
            else if (transferAmount < 0f && -transferAmount > myContent)
                transferAmount = -myContent;
            return transferAmount;
        }
    }

}