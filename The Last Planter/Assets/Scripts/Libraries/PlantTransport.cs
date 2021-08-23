using System.Collections.Generic;

namespace PlantSimulation
{
    public static class Transport
    {
        public static readonly float MIN_WATER_SOLUBILITY = 0.2f;
        public static float Diffuse(float otherConcentration, float myConcentration, float surfaceArea)
        {
            float amount = (otherConcentration - myConcentration) / (otherConcentration + myConcentration + 0.0001f) * surfaceArea;
            if (amount < 0f && -amount > myConcentration * 0.5f)
                return -myConcentration * 0.5f;
            return amount;
        }
        public static float Osmosis(float otherWaterSolubility, float myWaterSolubility, float maxOsmosis)
        {
            return Diffuse(otherWaterSolubility, myWaterSolubility, maxOsmosis);
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