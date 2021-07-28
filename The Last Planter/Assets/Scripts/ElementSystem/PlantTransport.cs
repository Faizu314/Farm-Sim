using System.Collections.Generic;

namespace PlantTransport
{
    public static class CompoundTransport
    {
        public static float Diffuse(float otherContent, float myContent, float surfaceArea)
        {
            return (otherContent - myContent) * surfaceArea;
        }
        public static float Osmosis(float otherWaterContent, float myWaterContent, float surfaceArea, float maxOsmosis)
        {
            float amount = Diffuse(otherWaterContent, myWaterContent, surfaceArea);
            if (amount > maxOsmosis)
                amount = maxOsmosis;
            return amount;
        }
        public static float GetWaterSolubility(List<float> soilMineralContent, float soilWaterContent)
        {
            float totalMineral = 0f;
            foreach (float content in soilMineralContent)
            {
                totalMineral += content;
            }
            float waterSolubility = soilWaterContent / totalMineral;
            return waterSolubility;
        }
        public static float ActiveTransport(float otherContent, float myContent, float absorptionStrength, float surfaceArea)
        {
            return absorptionStrength * surfaceArea * otherContent / myContent;
        }
    }
}
