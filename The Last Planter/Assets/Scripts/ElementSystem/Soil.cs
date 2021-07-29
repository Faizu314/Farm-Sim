using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soil : MonoBehaviour, ICompoundChannel
{
    public PlantElement toPlant;
    public List<float> compoundContents;
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

    public void Start()
    {
        toPlant.Initialize(this);
        toPlant.gameObject.SetActive(true);
    }
}
