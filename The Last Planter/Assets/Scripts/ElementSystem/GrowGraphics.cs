using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowGraphics : MonoBehaviour
{
    [Header("Shader")]
    [SerializeField] private Material growMaterial;
    [SerializeField] private AnimationCurve clipValue;
    [SerializeField] private float minGrowth_;
    [SerializeField] private float maxGrowth_;
    
    [Header("Plant Element")]
    [Tooltip("Assuming that the minimum growth will always be 0 for every Plant Element")]
    [SerializeField] private float maxGrowth;

    private float goalGrowth;
    private float currentGrowth;

    private void Start()
    {
        goalGrowth = minGrowth_;
        currentGrowth = minGrowth_;
    }
    private void Update()
    {
        currentGrowth = Mathf.Lerp(currentGrowth, goalGrowth, Time.deltaTime);
        growMaterial.SetFloat("Grow_", currentGrowth);
        growMaterial.SetFloat("Clip_", clipValue.Evaluate(currentGrowth));
    }
    public void SetGrowth(float growth)
    {
        goalGrowth = minGrowth_ + (growth / maxGrowth) * (maxGrowth_ - minGrowth_);
    }
}
