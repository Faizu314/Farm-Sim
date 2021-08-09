using UnityEngine;

public class TomatoRoots : PlantElement
{
    [Header("Output")]
    [SerializeField] private GameObject outputPrefab;
    [SerializeField] private float requiredOutputGrowth;

    private PlantElement output;
    private bool outputSpawned;

    protected override void OnInitialize()
    {
        outputSpawned = false;
    }
    protected override void OnUpdate() { }

    protected override void Function(float deltaTime)
    {
        ExchangeCompounds(sustainer, deltaTime);
        if (outputSpawned)
            ExchangeCompounds(output, deltaTime);
    }
    protected override void Grow(float deltaTime)
    {
        if (foodStore >= growthFoodConsumption * deltaTime)
        {
            foodStore -= growthFoodConsumption * deltaTime;
            growth += DebugFloats.growthIncrement * deltaTime;
        }
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
        return growth >= requiredOutputGrowth && !outputSpawned;
    }
    protected override void Output()
    {
        output = Instantiate(outputPrefab).GetComponent<PlantElement>();
        output.Initialize(this, environment, parentObject);
        outputSpawned = true;
    }
}