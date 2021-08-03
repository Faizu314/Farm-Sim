namespace PlantSimulation
{
    public static class Sunlight
    {
        public static float SunlightAtDepth(float surfaceSunlight, float depth)
        {
            return surfaceSunlight + (depth * 0.5f);
        }
        public static float SunlightAtHeight(float surfaceSunlight, float height)
        {
            return surfaceSunlight + (height * 0.1f);
        }
    }
}