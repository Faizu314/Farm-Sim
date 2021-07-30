using System.Collections.Generic;
using UnityEngine;

namespace PlantTransport
{
    public static class CompoundTransport
    {
        //If S.A is 1 then only DiffusionRate% of content can be transferred in one iteration
        private const float DIFFUSION_RATE = 0.1f;
        public static readonly float MIN_WATER_SOLUBILITY = 0.2f;
        public static float Diffuse(float otherContent, float myContent, float surfaceArea)
        {
            float amount = (otherContent - myContent) / (otherContent + myContent + 0.01f) * surfaceArea * DIFFUSION_RATE;
            if (amount > 0f && amount > otherContent)
                amount = otherContent;
            else if (amount < 0f && amount > myContent)
                amount = myContent;
            return amount;
        }
        public static float Osmosis(float otherWaterContent, float myWaterContent, float surfaceArea, float maxOsmosis)
        {
            float amount = Diffuse(otherWaterContent, myWaterContent, surfaceArea);
            if (Mathf.Abs(amount) > maxOsmosis)
                amount = Mathf.Sign(amount) * maxOsmosis;
            return amount;
        }
        public static float GetWaterSolubility(List<float> mineralContent)
        {
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
    }
}
