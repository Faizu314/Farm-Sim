using System.Collections.Generic;
using UnityEngine;

public abstract class PlantElement : MonoBehaviour, ICompoundChannel
{
    protected const int H2O = 0;
    protected const int N = 1;
    protected const int PO4 = 2;
    protected const int K = 3;
    protected const int Ca = 4;

    [Header("Plant Properties")]

    [SerializeField] private PlantStatus status;
    [SerializeField] private float functionPeriod;
    [SerializeField] private float growPeriod;
    [SerializeField] private float initialFoodStore;
    [SerializeField] protected float growthFoodConsumption;


    protected float sunlightIntensity;
    protected float temperature;
    protected int soilHardness;

    protected float health;
    protected float growth;
    protected float foodStore;
    protected ICompoundChannel sustainer;
    protected Soil environment;

    private float function_dt;
    private float grow_dt;
    private bool isHealthy;


    private void Update()
    {
        if (health <= 0f)
            return;
        FunctionStep();
        GrowStep();
    }
    private void OnDisable()
    {
        status.Reset();
    }
    public void Initialize(ICompoundChannel sustainer, Soil environment)
    {
        this.sustainer = sustainer;
        this.environment = environment;
        environment.Subscribe(GetEnvironmentConditions);
        function_dt = grow_dt = 0f;
        isHealthy = true;
        growth = 0f;
        health = 50f;
        foodStore = initialFoodStore;

        status.Initialize();
        OnInitialize();
    }
    private void GetEnvironmentConditions(float sunlightIntensity, float temperature, int soilHardness)
    {
        this.sunlightIntensity = sunlightIntensity;
        this.temperature = temperature;
        this.soilHardness = soilHardness;
    }

    private void FunctionStep()
    {
        function_dt += Time.deltaTime;
        if (function_dt >= functionPeriod)
        {
            Function(function_dt * DebugFloats.simulationSpeed);
            Live(function_dt * DebugFloats.simulationSpeed);
            function_dt = 0f;
        }
    }
    private void GrowStep()
    {
        grow_dt += Time.deltaTime;
        if (grow_dt >= growPeriod)
        {
            if (foodStore >= growthFoodConsumption * grow_dt)
                Grow(grow_dt * DebugFloats.simulationSpeed);
            AttemptOutput();
            grow_dt = 0f;
        }
    }
    private void AttemptOutput()
    {
        if (ShouldOutput())
            Output();
    }
    private void Live(float deltaTime)
    {
        Respiration(deltaTime);
        //Evaporation(deltaTime);
        CheckCompoundContents(deltaTime);
        CheckSymptoms();

        if (foodStore < 1f && sustainer is PlantElement)
            (sustainer as PlantElement).GiveFood(this, 1f);
    }

    #region Helper Functions
    private void RecieveFood(float amount)
    {
        foodStore += amount;
    }
    private void Respiration(float deltaTime)
    {
        if (foodStore >= DebugFloats.respireFoodConsumption * deltaTime && 
            GetContent(H2O) >= DebugFloats.respireWaterConsumption * deltaTime)
        {
            foodStore -= DebugFloats.respireFoodConsumption * deltaTime;
            UpdateContent(H2O, -DebugFloats.respireWaterConsumption * deltaTime);
        }
        else
            health -= DebugFloats.healthIncrement * deltaTime;
    }
    private void Evaporation(float deltaTime)
    {
        //if (GetContent(H2O) >= evaporationRate * deltaTime)
        //    UpdateContent(H2O, evaporationRate * deltaTime * -1f);
        //else
        //    UpdateContent(H2O, GetContent(H2O) * -1f);
    }
    private void CheckCompoundContents(float deltaTime)
    {
        float wellBeing = status.GetWellBeing() * DebugFloats.healthIncrement * deltaTime;
        if (!(health <= 10f && wellBeing < 0f))
            health += wellBeing;
        if (health <= 0f)
            Debug.Log(gameObject.name + " is dead");
        else if (health >= 100f)
            health = 100f;
    }
    private void CheckSymptoms()
    {
        if (health < 10 && isHealthy)
        {
            Debug.Log(gameObject.name + ": is unhealthy");
            isHealthy = false;
            (int index, bool isExcess) = status.GetSymptom();
            if (index != -1)
                ShowSymptoms(index, isExcess);
        }
        else if (health > 20 && !isHealthy)
        {
            Debug.Log(gameObject.name + ": is healthy again");
            isHealthy = true;
            HideSymptoms();
        }
    }
    #endregion



    #region Abstract Functions
    protected abstract void OnInitialize();
    protected abstract void Function(float deltaTime);
    protected abstract void Grow(float deltaTime);
    protected abstract void ShowSymptoms(int compoundIndex, bool isExcess);
    protected abstract void HideSymptoms();
    protected abstract bool ShouldOutput();
    protected abstract void Output();
    #endregion

    #region User Interface
    protected void ExchangeCompounds(ICompoundChannel other, float deltaTime, float rateMultiplier = 1f)
    {
        status.ExchangeCompounds(other, other == sustainer, deltaTime, rateMultiplier);
    }
    protected void GiveFood(PlantElement other, float amount)
    {
        if (foodStore > amount)
        {
            other.RecieveFood(amount);
            foodStore -= amount;
        }
    }
    protected void PassEverthing(PlantElement other)
    {
        List<float> deltaContent = GetContent();
        other.RecieveFood(foodStore);
        foodStore = 0f;
        other.UpdateContent(deltaContent);
        for (int i = 0; i < deltaContent.Count; i++)
            deltaContent[i] *= -1f;
        UpdateContent(deltaContent);
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