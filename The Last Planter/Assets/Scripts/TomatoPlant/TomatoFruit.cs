using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TomatoFruit : PlantElement
{
    [Header("Growth")]
    [SerializeField] private float goalScale;

    [Header("Rotting")]
    [SerializeField] private float rottingGrowth;

    protected override void OnInitialize() 
    {
        transform.localScale = Vector3.one * 0.01f;
    }
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
            growth += DebugFloats.instance.growthIncrement * deltaTime;
        }
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
        health -= DebugFloats.instance.healthIncrement * deltaTime;
    }
}
