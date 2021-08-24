using UnityEngine;

public class TomatoLeaves : PlantElement
{
    [Header("Photosynthesis")]
    [SerializeField] private float inputMineralsAmount = 0.01f;
    [SerializeField] private float inputWaterAmount = 0.02f;
    [SerializeField] private float outputFoodAmount = 0.2f;

    [Header("Growth")]
    [SerializeField] private float goalScale;

    [Header("Output")]
    [SerializeField] private GameObject output;
    [SerializeField] private float requiredOutputGrowth;

    private bool hasGivenFruit;
    private PlantElement fruit;

    protected override void OnInitialize()
    {
        hasGivenFruit = false;
        transform.localScale = Vector3.one * 0.01f;
    }
    protected override void OnUpdate() 
    {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * goalScale * Mathf.Clamp(growth, 0f, 1f), Time.deltaTime);
    }
    protected override void Function(float deltaTime)
    {
        Photosynthesis(deltaTime);
        if (foodStore > 0.5f)
            GiveFood((PlantElement)sustainer, deltaTime * 0.2f);
        if (hasGivenFruit)
            GiveFood(fruit, deltaTime * 0.08f);
    }
    protected override void Grow(float deltaTime)
    {
        if (foodStore >= growthFoodConsumption * deltaTime)
        {
            foodStore -= growthFoodConsumption * deltaTime;
            growth += DebugFloats.instance.growthIncrement * deltaTime;
        }
    }
    protected override bool ShouldOutput()
    {
        return output != null && growth >= requiredOutputGrowth && !hasGivenFruit;
    }
    protected override void Output()
    {
        output.SetActive(true);
        fruit = output.GetComponent<PlantElement>();
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
            foodStore += outputFoodAmount * sunlightIntensity * growth * deltaTime;
            UpdateContent(H2O, -waterAmount);
            UpdateContent(N, -mineralAmount);
            UpdateContent(PO4, -mineralAmount);
            UpdateContent(K, -mineralAmount);
        }
    }
}