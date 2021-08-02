using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Weather : MonoBehaviour
{
    public float sunlightIntensity;
    public float temperature;

    private static UnityEvent<float, float> weather = new UnityEvent<float, float>();

    [SerializeField] private float weatherTickPeriod;
    private float weather_dt;

    private void Update()
    {
        weather_dt += Time.deltaTime;
        if (weather_dt >= weatherTickPeriod)
        {
            weather.Invoke(sunlightIntensity, temperature);
            weather_dt = 0f;
        }
    }
    public static void Subscribe(UnityAction<float, float> callback)
    {
        weather.AddListener(callback);
    }
    public static void Unsubscribe(UnityAction<float, float> callback)
    {
        weather.RemoveListener(callback);
    }
}
