using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlantElement : MonoBehaviour, ICompoundChannel
{
    private enum LifeCycle { Morph, offspring };

    [Header("Plant Properties")]

    [SerializeField] private LifeCycle lifeCycle;
    [SerializeField] protected PlantElement output;
    [SerializeField] protected PlantStatus status;
    protected ICompoundChannel sustainer;

    [SerializeField] private float functionPeriod;
    [SerializeField] private float growPeriod;

    [SerializeField] private float requiredOffspringGrowth;
    [SerializeField] private float requiredMorphingGrowth;
    [SerializeField] private float initialFoodStore;

    private float growth;
    private float health;
    private float foodStore;

    private float function_dt;
    private float grow_dt;
    private bool deactivated;

    private void Update()
    {
        if (deactivated)
            return;
        FunctionStep();
        GrowStep();
        //Respire(Time.deltaTime);
        AttemptOutput();
    }

    private void FunctionStep()
    {
        function_dt += Time.deltaTime;
        if (function_dt >= functionPeriod)
        {
            Function(function_dt);
            function_dt = 0f;
        }
    }
    private void GrowStep()
    {
        grow_dt += Time.deltaTime;
        if (grow_dt >= growPeriod)
        {
            Grow(grow_dt);
            grow_dt = 0f;
        }
    }
    private void Respire(float deltaTime) {
        foodStore -= deltaTime;
    }
    private void AttemptOutput()
    {
        if (lifeCycle == LifeCycle.Morph)
        {
            if (growth >= requiredMorphingGrowth)
            {
                Morph();
            }
        }
        else if (lifeCycle == LifeCycle.offspring)
        {
            if (growth >= requiredOffspringGrowth)
            {
                Offspring();
            }
        }
    }

    protected void ExchangeCompoundsWithSustainer(float deltaTime, float multiplier = 1f)
    {
        status.ExchangeCompound(sustainer, deltaTime, multiplier);
    }
    protected void ExchangeCompoundsWithOffspring(float deltaTime, float multiplier = 1f)
    {
        if (growth >= requiredOffspringGrowth)
            status.ExchangeCompound(output, deltaTime, multiplier);
    }
    protected void GiveFoodToSustainer()
    {
        if (sustainer is PlantElement)
        {
            GiveFood(sustainer as PlantElement);
        }
        else
        {
            Debug.LogError("Cannot transfer food (Starch) with a non plant sustainer (Sustainer is soil/pot)\n GameObject: " + gameObject.name);
        }
    }
    protected void GiveFoodToOffspring()
    {
        GiveFood(output);
    }
    protected void Photosynthesis()
    {

    }

    private void GiveFood(PlantElement other)
    {
        other.RecieveFood(foodStore / 10f);
    }
    private void RecieveFood(float amount)
    {
        foodStore += amount;
    }

    public void Initialize(ICompoundChannel sustainer)
    {
        this.sustainer = sustainer;
        deactivated = false;
        function_dt = grow_dt = 0f;
        growth = 0f;
        health = 50f;
        foodStore = initialFoodStore;
        status.Initialize();
    }
    public void Deactivate()
    {
        Initialize(null);
        status.Reset();
        deactivated = true;
    }

    public List<float> GetContent()
    {
        return status.GetContent();
    }
    public void UpdateContent(List<float> deltaContent)
    {
        status.UpdateContent(deltaContent);
    }

    protected abstract void Function(float deltaTime);
    protected abstract void Grow(float deltaTime);
    protected abstract void Offspring();
    protected abstract void Morph();
}
