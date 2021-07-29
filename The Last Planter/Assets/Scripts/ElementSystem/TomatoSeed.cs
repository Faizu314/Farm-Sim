using UnityEngine;

public class TomatoSeed : PlantElement
{
    protected override void Function(float deltaTime)
    {
        ExchangeCompoundsWithSustainer(deltaTime);
    }
    protected override void Grow(float deltaTime)
    {
        foodStore -= growthFoodConsumption * deltaTime;
        growth += growthIncrement * deltaTime;
    }
    protected override void ShowSymptoms(int compoundIndex, bool isExcess)
    {
        throw new System.NotImplementedException();
    }
    protected override void HideSymptoms()
    {
        throw new System.NotImplementedException();
    }
    protected override void Morph()
    {
        output.Initialize(sustainer);
        output.gameObject.SetActive(true);
        PassEverthingToOutput();
        gameObject.SetActive(false);
    }
    protected override void Offspring()
    {
        throw new System.NotImplementedException();
    }
}