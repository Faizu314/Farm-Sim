using UnityEngine;
using System.Collections.Generic;

public class TomatoStem : PlantElement
{
    [Header("Grow")]
    [SerializeField] [Range(0, 4)] private int maxSoilHardness;
    [SerializeField] private float translationRate;
    [SerializeField] private float maxTranslationGrowth;

    [Header("Output")]
    [SerializeField] private GameObject outputLeavesPrefab;
    [SerializeField] private int maxLeaves;
    [SerializeField] private float requiredHeightPerLeaf;
    [SerializeField] private float requiredFoodPerLeaf;

    private List<PlantElement> outputLeaves = new List<PlantElement>();
    private float diffMultiplier;
    private int outputLeavesSpawned;

    protected override void OnInitialize()
    {
        outputLeavesSpawned = 0;
        diffMultiplier = 1f;
    }
    protected override void Function(float deltaTime)
    {
        for (int i = 0; i < outputLeavesSpawned; i++)
            ExchangeCompounds(outputLeaves[i], deltaTime, diffMultiplier);

        if (outputLeavesSpawned >= maxLeaves || foodStore > requiredFoodPerLeaf + 0.1f)
            GiveFood((PlantElement)sustainer, deltaTime * 0.1f);
    }
    protected override void Grow(float deltaTime)
    {
        if (foodStore >= growthFoodConsumption * deltaTime && soilHardness < maxSoilHardness)
        {
            foodStore -= growthFoodConsumption * deltaTime;
            growth += DebugFloats.growthIncrement * deltaTime;
            if (foodStore >= requiredFoodPerLeaf && growth <= maxTranslationGrowth)
                GetComponent<Transform>().Translate(Vector3.up * translationRate * deltaTime);
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
        if (transform.position.y < 0.1f)
            return false;
        if (outputLeavesSpawned >= maxLeaves)
            return false;
        if (foodStore < requiredFoodPerLeaf)
            return false;
        return transform.position.y >= requiredHeightPerLeaf * (outputLeavesSpawned + 1);
    }
    protected override void Output()
    {
        outputLeaves.Add(Instantiate(outputLeavesPrefab).GetComponent<PlantElement>());
        Transform leaves = outputLeaves[outputLeavesSpawned].GetComponent<Transform>();

        leaves.position = transform.position;
        leaves.Translate(new Vector3(0.01f, 0.05f, 0));
        outputLeaves[outputLeavesSpawned].Initialize(this, environment);
        outputLeavesSpawned++;
        diffMultiplier = 1f / outputLeavesSpawned;
    }
}