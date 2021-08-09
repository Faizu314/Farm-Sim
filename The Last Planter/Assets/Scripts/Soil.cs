using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Soil : MonoBehaviour, ICompoundChannel
{
    private float sunlightIntensity;
    private float temperature;
    public int soilHardness;

    private UnityEvent<float, float, int> environment = new UnityEvent<float, float, int>();
    [SerializeField] private float environmentTickPeriod;

    public GameObject toPlant;
    public List<float> compoundContents;
    private float environment_dt;

    private void Start()
    {
        GameObject plant = new GameObject("Tomato Plant");
        plant.transform.localPosition = Vector3.zero;
        Instantiate(toPlant).GetComponent<PlantElement>().Initialize(this, this, plant.transform);
        environment_dt = 0;
        Weather.Subscribe(GetWeatherConditions);
    }
    private void Update()
    {
        environment_dt += Time.deltaTime;
        if (environment_dt >= environmentTickPeriod)
        {
            environment.Invoke(sunlightIntensity, temperature, soilHardness);
            environment_dt = 0f;
        }
    }
    public void Subscribe(UnityAction<float, float, int> callback)
    {
        environment.AddListener(callback);
    }
    public void Unsubscribe(UnityAction<float, float, int> callback)
    {
        environment.RemoveListener(callback);
    }
    public void Pat()
    {
        soilHardness++;
        if (soilHardness > 4)
            soilHardness = 4;
    }
    public void Till()
    {
        soilHardness--;
        if (soilHardness < 0)
            soilHardness = 0;
    }

    private void GetWeatherConditions(float sunlightIntensity, float temperature)
    {
        this.sunlightIntensity = sunlightIntensity;
        this.temperature = temperature;
    }

    #region ICompoundChannel Implementation
    public List<float> GetContent()
    {
        return compoundContents;
    }
    public float GetContent(int index)
    {
        return compoundContents[index];
    }
    public void UpdateContent(List<float> deltaContent)
    {
        for (int i = 0; i < compoundContents.Count; i++)
        {
            compoundContents[i] += deltaContent[i];
        }
    }
    public void UpdateContent(int index, float deltaContent)
    {
        compoundContents[index] += deltaContent;
    }
    #endregion
}
