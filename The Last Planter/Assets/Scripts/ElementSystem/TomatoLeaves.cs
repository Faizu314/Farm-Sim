using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TomatoLeaves : PlantElement
{
    protected override void Function(float deltaTime)
    {
        Photosynthesis(deltaTime);
        ExchangeCompoundsWithSustainer(deltaTime);
        GiveFoodToSustainer(0.05f, deltaTime);
    }
    protected override void Grow(float deltaTime)
    {
        foodStore -= growthFoodConsumption * deltaTime;
        growth += growthIncrement * deltaTime;
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
        throw new System.NotImplementedException();
    }
    protected override void ShowSymptoms(int compoundIndex, bool isExcess)
    {
        throw new System.NotImplementedException();
    }
}
