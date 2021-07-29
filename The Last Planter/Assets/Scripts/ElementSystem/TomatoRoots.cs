using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TomatoRoots : PlantElement
{
    protected override void Function(float deltaTime)
    {
        ExchangeCompoundsWithSustainer(deltaTime);
        ExchangeCompoundsWithOffspring(deltaTime);
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
        throw new System.NotImplementedException();
    }
    protected override void Offspring()
    {
        output.Initialize(this);
        output.gameObject.SetActive(true);
    }
}