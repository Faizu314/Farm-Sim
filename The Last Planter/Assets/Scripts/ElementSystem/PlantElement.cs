using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

[RequireComponent(typeof(OnClickEvent))]
public abstract class PlantElement : MonoBehaviour, ICompoundChannel
{
    protected const int H2O = 0;
    protected const int N = 1;
    protected const int PO4 = 2;
    protected const int K = 3;
    protected const int Ca = 4;

    [Header("Plant Properties")]

    [SerializeField] private PlantStatus status;
    [SerializeField] private float initialFoodStore;
    [SerializeField] protected float growthFoodConsumption;

    private GameObject canvasReference;
    private CinemachineVirtualCamera cinemachineCamera;

    protected Image waterBar;
    protected Text waterValue;
    protected Image lightBar;
    protected Text lightValue;
    protected Image mineralsBar;
    protected Text mineralsValue;
    protected Image growthBar;
    protected Text growthValue;
    protected Image healthBar;
    protected Text healthValue;
    private Text elementName;

    private Button exitButton;

    protected float sunlightIntensity;
    protected float temperature;
    protected int soilHardness;

    protected float health;
    protected float growth;
    protected float foodStore;
    protected ICompoundChannel sustainer;
    protected Soil environment;
    protected Transform parentObject;

    private float function_dt;
    private float grow_dt;
    private bool updateInterface;
    private bool isHealthy;


    private void Update()
    {
        if (health <= 0f)
            return;
        FunctionStep();
        GrowStep();
        OnUpdate();
        if (updateInterface)
            UpdateUserInterface();
    }
    private void OnDisable()
    {
        status.Reset();
    }
    public void Initialize(ICompoundChannel sustainer, Soil environment, Transform parentObject)
    {
        this.sustainer = sustainer;
        this.environment = environment;
        this.parentObject = parentObject;

        function_dt = grow_dt = 0f;
        isHealthy = true;
        updateInterface = false;
        growth = 0f;
        health = 50f;
        foodStore = initialFoodStore;

        canvasReference = Resources.FindObjectsOfTypeAll<Canvas>()[0].gameObject;
        cinemachineCamera = GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>();

        var allImages = Resources.FindObjectsOfTypeAll<Image>();

        foreach (Image image in allImages)
        {
            string name = image.gameObject.name;
            if (name == "HealthBar")
                healthBar = image;
            if (name == "GrowthBar")
                growthBar = image;
            if (name == "WaterBar")
                waterBar = image;
            if (name == "MineralBar")
                mineralsBar = image;
            if (name == "LightBar")
                lightBar = image;
        }

        var allTexts = Resources.FindObjectsOfTypeAll<Text>();
       

        foreach (Text text in allTexts)
        {
            string name = text.gameObject.name;
            if (name == "HealthValue")
                healthValue = text;
            if (name == "GrowthValue")
                growthValue = text;
            if (name == "WaterValue")
                waterValue = text;
            if (name == "MineralValue")
                mineralsValue = text;
            if (name == "LightValue")
                lightValue = text;
            if (name == "ElementName")
                elementName = text;
        }

        exitButton = Resources.FindObjectsOfTypeAll<Button>()[0];

        environment.Subscribe(GetEnvironmentConditions);
        GetComponent<OnClickEvent>().RegisterClickEvent(OnClick);

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
        float function_period = DebugFloats.instance.functionTickFrequency / DebugFloats.instance.simulationSpeed;
        if (function_dt >= function_period)
        {
            Function(DebugFloats.instance.simulationStep);
            Live(DebugFloats.instance.simulationStep);
            function_dt = 0f;
        }
    }
    private void GrowStep()
    {
        grow_dt += Time.deltaTime;
        float grow_period = DebugFloats.instance.growTickFrequency / DebugFloats.instance.simulationSpeed;
        if (grow_dt >= grow_period)
        {
            if (foodStore >= growthFoodConsumption * grow_dt)
                Grow(DebugFloats.instance.simulationStep);
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
        if (CanRespire(deltaTime))
        {
            Respiration(deltaTime);
            CheckCompoundContents(deltaTime);
        }
        else
            health -= DebugFloats.instance.healthIncrement * deltaTime;
        //Evaporation(deltaTime);
        CheckSymptoms();

        if (foodStore < 1f && sustainer is PlantElement)
            (sustainer as PlantElement).GiveFood(this, 1f);
    }

    #region Helper Functions
    private void RecieveFood(float amount)
    {
        foodStore += amount;
    }
    private bool CanRespire(float deltaTime)
    {
        return (foodStore >= DebugFloats.instance.respireFoodConsumption * deltaTime &&
                GetContent(H2O) >= DebugFloats.instance.respireWaterConsumption * deltaTime);
    }
    private void Respiration(float deltaTime)
    {
        foodStore -= DebugFloats.instance.respireFoodConsumption * deltaTime;
        UpdateContent(H2O, -DebugFloats.instance.respireWaterConsumption * deltaTime);
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
        float wellBeing = status.GetWellBeing() * DebugFloats.instance.healthIncrement * deltaTime;
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
    private void UpdateUserInterface()
    {
        elementName.text = gameObject.name;
        healthBar.fillAmount = health / 100f;
        healthValue.text = Mathf.Round(health) + "%";
        growthBar.fillAmount = growth / 10f;
        growthValue.text = Mathf.Round(growth * 10f) * 0.1f + "/10";
        waterBar.fillAmount = GetContent(H2O) / 10f;
        waterValue.text = Mathf.Round(GetContent(H2O) * 10f) * 0.1f + "/10";
        mineralsBar.fillAmount = (GetContent(N) + GetContent(PO4) + GetContent(K)) / 30f;
        mineralsValue.text = Mathf.Round((GetContent(N) + GetContent(PO4) + GetContent(K)) * 10f / 3f) * 0.1f + "/10";
        lightBar.fillAmount = sunlightIntensity;
        lightValue.text = Mathf.Round(sunlightIntensity * 100f) * 0.1f + "/10";
    }
    private void OnClick()
    {
        exitButton.onClick.Invoke();
        canvasReference.SetActive(true);
        updateInterface = true;
        exitButton.onClick.AddListener(OnExitButton);
        cinemachineCamera.Follow = transform;
        cinemachineCamera.LookAt = transform;
        Camera.main.GetComponent<CinemachineBrain>().enabled = true;
    }
    public void OnExitButton()
    {
        canvasReference.SetActive(false);
        updateInterface = false;
        exitButton.onClick.RemoveAllListeners();
        Camera.main.GetComponent<CinemachineBrain>().enabled = false;
        Camera.main.transform.localPosition = Vector3.zero;
        cinemachineCamera.Follow = GameObject.Find("Player").transform;
    }
    #endregion



    #region Abstract Functions
    protected abstract void OnInitialize();
    protected abstract void OnUpdate();
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
    public void GiveFood(PlantElement other, float amount)
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
    protected bool IsContentAppropriate(int compoundIndex)
    {
        return status.HasSymptom(compoundIndex);
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