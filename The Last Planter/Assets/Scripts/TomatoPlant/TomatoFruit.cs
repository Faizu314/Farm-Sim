using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TomatoFruit : PlantElement
{
    [Header("Rotting")]
    [SerializeField] private float rottingGrowth;

    protected override void OnInitialize() { }
    protected override void OnUpdate() 
    {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 1.204145f * (Mathf.Clamp(growth, 0f, 1f) + 0.01f), Time.deltaTime);
    }
    protected override void Function(float deltaTime)
    {
        ExchangeCompounds(sustainer, deltaTime);
        if (growth >= rottingGrowth)
            Rot(deltaTime);
    }
    protected override void Grow(float deltaTime)
    {
        if (foodStore >= growthFoodConsumption * deltaTime)
        {
            foodStore -= growthFoodConsumption * deltaTime;
            growth += DebugFloats.growthIncrement * deltaTime;
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
    protected override void Output()
    {
        throw new System.NotImplementedException();
    }
    protected override bool ShouldOutput()
    {
        return false;
    }

    private void Rot(float deltaTime)
    {
        health -= DebugFloats.healthIncrement * deltaTime;
    }
}
