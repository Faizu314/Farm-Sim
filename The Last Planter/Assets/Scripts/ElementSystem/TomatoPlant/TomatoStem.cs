using UnityEngine;

public class TomatoStem : PlantElement
{
    protected override void Function(float deltaTime)
    {
        ExchangeCompoundsWithOutput(deltaTime);
        GiveFoodToSustainer(deltaTime * 0.1f);
    }
    protected override void Grow(float deltaTime)
    {
        foodStore -= growthFoodConsumption * deltaTime;
        growth += growthIncrement * deltaTime;
    }
    protected override void ShowSymptoms(int compoundIndex, bool isExcess)
    {
        Debug.Log(gameObject.name + " have inappropriate ammounts of: " + compoundIndex + ", " + isExcess);
    }
    protected override void HideSymptoms()
    {
        Debug.Log(gameObject.name + " mineral levels restored");
    }
    protected override void Output()
    {
        output.Initialize(this);
        output.gameObject.SetActive(true);
    }
}
