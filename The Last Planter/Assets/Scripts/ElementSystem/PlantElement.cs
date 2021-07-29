using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlantElement : MonoBehaviour, ICompoundChannel
{
    private enum LifeCycle { None, Morph, offspring };

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

    protected float growth;
    protected float foodStore;

    private float health;
    private float function_dt;
    private float grow_dt;
    private bool isHealthy;
    private bool outputSpawned;

    [Header("Debug")]
    public float respireFoodConsumption;
    public float respireWaterConsumption;
    public float growthFoodConsumption;
    public float growthIncrement;
    public float healthIncrement;

    private void Update()
    {
        FunctionStep();
        GrowStep();
        Live(Time.deltaTime);
        AttemptOutput();
    }
    private void OnDisable()
    {
        status.Reset();
        //Initialize(null);
    }
    public void Initialize(ICompoundChannel sustainer)
    {
        this.sustainer = sustainer;
        function_dt = grow_dt = 0f;
        isHealthy = true;
        outputSpawned = false;
        growth = 0f;
        health = 50f;
        foodStore = initialFoodStore;
        status.Initialize();
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
            if (foodStore >= growthFoodConsumption * grow_dt)
                Grow(grow_dt);
            else
                health -= healthIncrement * grow_dt;
            grow_dt = 0f;
        }
    }
    private void Live(float deltaTime)
    {
        if (foodStore >= respireFoodConsumption * deltaTime && GetContent(0) >= respireWaterConsumption * deltaTime)
        {
            foodStore -= respireFoodConsumption * deltaTime;
            UpdateContent(0, respireWaterConsumption * deltaTime);
        }
        else
            health -= healthIncrement * deltaTime;

        health += status.GetWellBeing() * deltaTime;
        if (health < 10 && isHealthy)
        {
            Debug.Log(gameObject.name + ": is unhealthy");
            isHealthy = false;
            (int index, bool isExcess) = status.GetSymptom();
            ShowSymptoms(index, isExcess);
        }
        else if (health > 20 && !isHealthy)
        {
            Debug.Log(gameObject.name + ": is healthy again");
            isHealthy = true;
            HideSymptoms();
        }
    }
    private void AttemptOutput()
    {
        if (outputSpawned)
            return;
        switch (lifeCycle)
        {
            case LifeCycle.None:
                return;
            case LifeCycle.Morph:
                if (growth >= requiredMorphingGrowth)
                {
                    Morph();
                    outputSpawned = true;
                }
                break;
            case LifeCycle.offspring:
                if (growth >= requiredOffspringGrowth)
                {
                    Offspring();
                    outputSpawned = true;
                }
                break;
        }
    }
    private void GiveFood(PlantElement other, float percentage, float deltaTime)
    {
        other.RecieveFood(foodStore * deltaTime * percentage);
        foodStore -= foodStore * deltaTime * percentage;
    }
    private void RecieveFood(float amount)
    {
        foodStore += amount;
    }


    #region Abstract Functions
    protected abstract void Function(float deltaTime);
    protected abstract void Grow(float deltaTime);
    protected abstract void ShowSymptoms(int compoundIndex, bool isExcess);
    protected abstract void HideSymptoms();
    protected abstract void Offspring();
    protected abstract void Morph();
    #endregion

    #region UserInterface
    protected void ExchangeCompoundsWithSustainer(float deltaTime, float multiplier = 1f)
    {
        status.ExchangeCompounds(sustainer, deltaTime, multiplier);
    }
    protected void ExchangeCompoundsWithOffspring(float deltaTime, float multiplier = 1f)
    {
        if (lifeCycle == LifeCycle.offspring && growth >= requiredOffspringGrowth)
            status.ExchangeCompounds(output, deltaTime, multiplier);
    }
    protected void GiveFoodToSustainer(float percentage, float deltaTime)
    {
        if (sustainer is PlantElement)
        {
            GiveFood(sustainer as PlantElement, percentage, deltaTime);
        }
        else
        {
            Debug.LogError("Cannot transfer food (Starch) with a non plant sustainer (Soil/Pot are non plant sustainers)\n GameObject: " + gameObject.name);
        }
    }
    protected void GiveFoodToOffspring(float percentage, float deltaTime)
    {
        GiveFood(output, percentage, deltaTime);
    }
    protected void PassEverthingToOutput()
    {
        List<float> deltaContent = GetContent();
        output.UpdateContent(deltaContent);
        output.RecieveFood(foodStore);
    }
    protected void Photosynthesis(float deltaTime)
    {
        if (GetContent(0) >= 0.05f * deltaTime &&
            GetContent(1) >= 0.05f * deltaTime &&
            GetContent(3) >= 0.05f * deltaTime) 
        {
            foodStore += 0.04f * (1f + growth) * deltaTime;
            UpdateContent(0, -0.05f * deltaTime);
            UpdateContent(1, -0.05f * deltaTime);
            UpdateContent(3, -0.05f * deltaTime);
        }
    }   
    #endregion

    #region ICompoundChannel Implementation
    public List<float> GetContent()
    {
        return status.GetContent();
    }
    public float GetContent(int index)
    {
        return status.GetContent(index);
    }
    public void UpdateContent(List<float> deltaContent)
    {
        status.UpdateContent(deltaContent);
    }
    public void UpdateContent(int index, float deltaContent)
    {
        status.UpdateContent(index, deltaContent);
    }
    #endregion
}