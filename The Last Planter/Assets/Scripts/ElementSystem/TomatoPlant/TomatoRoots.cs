using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TomatoRoots : PlantElement
{
    protected override void Function(float deltaTime)
    {
        ExchangeCompoundsWithSustainer(deltaTime);
        ExchangeCompoundsWithOutput(deltaTime);
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