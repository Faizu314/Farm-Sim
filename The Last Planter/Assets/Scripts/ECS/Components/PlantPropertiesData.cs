using Unity.Entities;

[GenerateAuthoringComponent]
public class PlantPropertiesData : IComponentData
{
    public float volume;
    public float maxDiffusion;
    public float maxOsmosis;
}
