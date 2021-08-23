using UnityEngine;

public class TomatoRoots : PlantElement
{
    [Header("Output")]
    [SerializeField] private GameObject output;
    [SerializeField] private float requiredOutputGrowth;

    private PlantElement stemPE;
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
        {
            ExchangeCompounds(stemPE, deltaTime);
            if (foodStore < 0.01f)
                stemPE.GiveFood(this, 0.02f);
        }
    }
    protected override void Grow(float deltaTime)
    {
        if (foodStore >= growthFoodConsumption * deltaTime)
        {
            foodStore -= growthFoodConsumption * deltaTime;
            growth += DebugFloats.instance.growthIncrement * deltaTime;
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
        output.SetActive(true);
        stemPE = output.GetComponent<PlantElement>();
        stemPE.Initialize(this, environment, parentObject);
        outputSpawned = true;
    }
}