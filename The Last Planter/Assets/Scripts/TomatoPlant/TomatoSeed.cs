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
    [SerializeField] private GameObject output;
    [SerializeField] private float requiredOutputGrowth;

    protected override void OnInitialize() { }
    protected override void OnUpdate() { }

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
        output.SetActive(true);
        PlantElement rootsPE = output.GetComponent<PlantElement>();
        rootsPE.Initialize(sustainer, environment, parentObject);
        PassEverthing(rootsPE);
        Destroy(gameObject);
    }

    private bool CanGerminate()
    {
        if (!IsContentAppropriate(H2O))
            return false;
        float currSunlight = Sunlight.SunlightAtDepth(sunlightIntensity, transform.position.y);
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