using UnityEngine;
using PlantSimulation;

public class TomatoSeed : PlantElement
{
    [Header("Germination")]
    [SerializeField] [Range(0, 4)] private int minSoilHardness;
    [SerializeField] [Range(0f, 1f)] private float minSunlightIntensity;
    [SerializeField] [Range(0f, 1f)] private float maxSunlightIntensity;
    [SerializeField] private float germinationWaterConsumption;

    [Header("Output")]
    [SerializeField] private GameObject rootsPrefab;
    [SerializeField] private float requiredOutputGrowth;

    private Transform seed;

    protected override void OnInitialize()
    {
        seed = GetComponent<Transform>();
    }
    protected override void Function(float deltaTime)
    {
        ExchangeCompounds(sustainer, deltaTime);
    }
    protected override void Grow(float deltaTime)
    {
        if (CanGerminate())
            Germinate(deltaTime);
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
        return growth >= requiredOutputGrowth;
    }
    protected override void Output()
    {
        PlantElement rootsPE = Instantiate(rootsPrefab).GetComponent<PlantElement>();
        rootsPE.Initialize(sustainer, environment);
        PassEverthing(rootsPE);
        Destroy(gameObject);
    }

    private bool CanGerminate()
    {
        float currSunlight = Sunlight.SunlightAtDepth(sunlightIntensity, seed.position.y);
        if (soilHardness >= minSoilHardness)
            if (currSunlight >= minSunlightIntensity && currSunlight <= maxSunlightIntensity)
                return true;
        return false;
    }
    private void Germinate(float deltaTime)
    {
        if (foodStore >= growthFoodConsumption * deltaTime &&
            GetContent(H2O) > germinationWaterConsumption * deltaTime)
        {
            foodStore -= growthFoodConsumption * deltaTime;
            UpdateContent(H2O, germinationWaterConsumption * deltaTime * -1f);
            growth += DebugFloats.growthIncrement * deltaTime;
        }
    }
}