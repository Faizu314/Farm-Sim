using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pot : MonoBehaviour
{
    public Light sun;
    public Plant plant;

    public float tickPeriod;
    public List<float> soilContent;
    public float waterContent;

    private float deltaTick = 0;

    private void Update()
    {
        if (deltaTick >= tickPeriod)
        {
            plant.Absorb(soilContent, waterContent);
            deltaTick = 0;
        }
        else
            deltaTick += Time.deltaTime;
    }

    public void UpdateSoilMineralContent(List<float> deltaMineralAmount)
    {
        for (int i = 0; i < soilContent.Count; i++)
        {
            soilContent[i] += deltaMineralAmount[i];
        }
    }
    public void UpdateSoilWaterContent(float waterAmount)
    {
        waterContent += waterAmount;
    }
}
