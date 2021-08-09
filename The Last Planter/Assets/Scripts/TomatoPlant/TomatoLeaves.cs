using UnityEngine;

public class TomatoLeaves : PlantElement
{
    [Header("Photosynthesis")]
    [SerializeField] private float inputMineralsAmount = 0.01f;
    [SerializeField] private float inputWaterAmount = 0.02f;
    [SerializeField] private float outputFoodAmount = 0.2f;

    [Header("Output")]
    [SerializeField] private GameObject outputFruitPrefab;
    [SerializeField] private float requiredOutputGrowth;

    private bool hasGivenFruit;
    private PlantElement fruit;

    protected override void OnInitialize()
    {
        hasGivenFruit = false;
    }
    protected override void OnUpdate() 
    {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * Mathf.Clamp(growth, 0f, 1f), Time.deltaTime);
    }
    protected override void Function(float deltaTime)
    {
        Photosynthesis(deltaTime);
        GiveFood((PlantElement)sustainer, deltaTime * 0.07f);
        if (hasGivenFruit)
            GiveFood(fruit, deltaTime * 0.08f);
    }
    protected override void Grow(float deltaTime)
    {
        if (foodStore >= growthFoodConsumption * deltaTime)
        {
            foodStore -= growthFoodConsumption * deltaTime;
            growth += DebugFloats.growthIncrement * deltaTime;
        }
    }
    protected override void HideSymptoms()
    {
        Debug.Log(gameObject.name + " mineral levels restored");
    }
    protected override void ShowSymptoms(int compoundIndex, bool isExcess)
    {
        Debug.Log(gameObject.name + " have inappropriate ammounts of: " + compoundIndex + ", " + isExcess);
    }
    protected override bool ShouldOutput()
    {
        return outputFruitPrefab != null && growth >= requiredOutputGrowth && !hasGivenFruit;
    }
    protected override void Output()
    {
        GameObject fruitObj = Instantiate(outputFruitPrefab);
        fruit = fruitObj.GetComponent<PlantElement>();
        fruit.Initialize(this, environment, parentObject);
        hasGivenFruit = true;
    }

    private void Photosynthesis(float deltaTime)
    {
        float mineralAmount = inputMineralsAmount * deltaTime;
        float waterAmount = inputWaterAmount * deltaTime;

        if (GetContent(H2O) >= waterAmount &&
            GetContent(N) >= mineralAmount &&
            GetContent(PO4) >= mineralAmount &&
            GetContent(K) >= mineralAmount)
        {
            foodStore += outputFoodAmount * sunlightIntensity * deltaTime;
            UpdateContent(H2O, -waterAmount);
            UpdateContent(N, -mineralAmount);
            UpdateContent(PO4, -mineralAmount);
            UpdateContent(K, -mineralAmount);
        }
    }
}