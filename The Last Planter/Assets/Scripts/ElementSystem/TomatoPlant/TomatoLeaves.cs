using UnityEngine;

public class TomatoLeaves : PlantElement
{
    protected override void Function(float deltaTime)
    {
        Photosynthesis(0.01f, 0.02f, 0.2f, deltaTime);
        GiveFoodToSustainer(deltaTime * 0.15f);
    }
    protected override void Grow(float deltaTime)
    {
        foodStore -= growthFoodConsumption * deltaTime;
        growth += growthIncrement * deltaTime;
    }
    protected override void HideSymptoms()
    {
        Debug.Log(gameObject.name + " mineral levels restored");
    }
    protected override void ShowSymptoms(int compoundIndex, bool isExcess)
    {
        Debug.Log(gameObject.name + " have inappropriate ammounts of: " + compoundIndex + ", " + isExcess);
    }
    protected override void Output()
    {
        throw new System.NotImplementedException();
    }
}
