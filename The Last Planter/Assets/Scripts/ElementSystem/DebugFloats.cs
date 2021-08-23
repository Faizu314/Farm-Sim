using UnityEngine;

public class DebugFloats : MonoBehaviour
{
    public static DebugFloats instance;

    [Range(0.0001f, 100f)] public float simulationSpeed = 1f;
    public float functionTickFrequency = 1f;
    public float growTickFrequency = 2f;
    public float simulationStep = 0.5f;
    public float respireWaterConsumption = 0.001f;
    public float respireFoodConsumption = 0.001f;
    public float growthIncrement = 0.01f;
    public float healthIncrement = 1f;

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        else
            instance = this;
    }
}
