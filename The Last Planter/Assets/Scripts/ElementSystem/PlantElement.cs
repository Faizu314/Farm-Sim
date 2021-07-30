using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlantElement : MonoBehaviour, ICompoundChannel
{
    private enum LifeCycle { None, Output };

    [Header("Plant Properties")]

    [SerializeField] private LifeCycle lifeCycle;
    [SerializeField] protected PlantElement output;
    [SerializeField] protected PlantStatus status;
    protected ICompoundChannel sustainer;

    [SerializeField] private float functionPeriod;
    [SerializeField] private float growPeriod;

    [SerializeField] private float requiredOutputGrowth;
    [SerializeField] private float initialFoodStore;

    protected float growth;
    protected float foodStore;

    private float health;
    private float function_dt;
    private float grow_dt;
    private bool outputsSpawned;
    private bool isHealthy;

    [Header("Debug")]
    public float respireFoodConsumption;
    public float respireWaterConsumption;
    public float growthFoodConsumption;
    public float growthIncrement;
    public float healthIncrement;

    private void Update()
    {
        if (health <= 0f)
            return;
        Live(Time.deltaTime);
        FunctionStep();
        GrowStep();
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
        outputsSpawned = false;
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

        float wellBeing = status.GetWellBeing() * deltaTime;
        if (!(health <= 10f && wellBeing < 0f))
            health += wellBeing;
        if (health <= 0f)
        {
            Debug.Log(gameObject.name + " is dead");
        }
        else if (health >= 100f)
            health = 100f;
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
        if (foodStore < 1f)
        {
            if (sustainer is PlantElement element)
            {
                element.GiveFood(this, 1f);
            }
        }
    }
    private void AttemptOutput()
    {
        if (outputsSpawned)
            return;
        switch (lifeCycle)
        {
            case LifeCycle.None:
                return;
            case LifeCycle.Output:
                if (growth >= requiredOutputGrowth)
                {
                    Output();
                    outputsSpawned = true;
                }
                break;
        }
    }
    private void GiveFood(PlantElement other, float amount)
    {
        if (foodStore > amount)
        {
            other.RecieveFood(amount);
            foodStore -= amount;
        }
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
    protected abstract void Output();
    #endregion

    #region User Interface
    protected void ExchangeCompoundsWithSustainer(float deltaTime, float multiplier = 1f)
    {
        status.ExchangeCompounds(sustainer, true, deltaTime, multiplier);
    }
    protected void ExchangeCompoundsWithOutput(float deltaTime, float multiplier = 1f)
    {
        if (lifeCycle == LifeCycle.Output && growth >= requiredOutputGrowth)
            status.ExchangeCompounds(output, false, deltaTime, multiplier);
    }
    protected void GiveFoodToSustainer(float amount)
    {
        if (sustainer is PlantElement element)
        {
            GiveFood(element, amount);
        }
        else
        {
            Debug.LogError("Cannot transfer food (Starch) with a non plant sustainer (Soil/Pot are non plant sustainers)\n GameObject: " + gameObject.name);
        }
    }
    protected void GiveFoodToOffspring(float amount)
    {
        GiveFood(output, amount);
    }
    protected void PassEverthingToOutput()
    {
        List<float> deltaContent = GetContent();
        output.RecieveFood(foodStore);
        foodStore = 0f;
        output.UpdateContent(deltaContent);
        for (int i = 0; i < deltaContent.Count; i++)
            deltaContent[i] *= -1f;
        UpdateContent(deltaContent);
    }
    protected void Photosynthesis(float inputMineralsAmount, float inputWaterAmount, float outputFoodAmount, float deltaTime)
    {
        float mineralAmount = inputMineralsAmount * deltaTime;
        float waterAmount = inputWaterAmount * deltaTime;

        if (GetContent(0) >= waterAmount &&
            GetContent(1) >= mineralAmount &&
            GetContent(2) >= mineralAmount &&
            GetContent(3) >= mineralAmount) 
        {
            foodStore += outputFoodAmount * (1f + growth) * deltaTime;
            UpdateContent(0, -waterAmount);
            UpdateContent(1, -mineralAmount);
            UpdateContent(2, -mineralAmount);
            UpdateContent(3, -mineralAmount);
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