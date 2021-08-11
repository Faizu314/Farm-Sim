using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowGraphics : MonoBehaviour
{
    [Header("Shader")]
    [SerializeField] private Material growMaterial;
    [SerializeField] private Texture2D mainTex;
    [SerializeField] private float shaderStrength;
    [SerializeField] private AnimationCurve clipValue;
    [SerializeField] private float minGrowth_;
    [SerializeField] private float maxGrowth_;
    
    [Header("Plant Element")]
    [Tooltip("Assuming that the minimum growth will always be 0 for every Plant Element")]
    [SerializeField] private float maxGrowth;

    private Material growMatCopy;
    private float goalGrowth;
    private float currentGrowth;

    private void Start()
    {
        growMatCopy = new Material(growMaterial);
        growMatCopy.SetFloat("Strength_", shaderStrength);
        growMatCopy.SetTexture("mainTex_", mainTex);
        GetComponent<Renderer>().sharedMaterial = growMatCopy;
        goalGrowth = minGrowth_;
        currentGrowth = minGrowth_;
    }
    private void Update()
    {
        currentGrowth = Mathf.Lerp(currentGrowth, goalGrowth, Time.deltaTime);
        growMatCopy.SetFloat("Grow_", currentGrowth);
        growMatCopy.SetFloat("Clip_", clipValue.Evaluate(currentGrowth));
    }
    public void SetGrowth(float growth)
    {
        goalGrowth = minGrowth_ + (growth / maxGrowth) * (maxGrowth_ - minGrowth_);
    }
}
