using Unity.Entities;

[GenerateAuthoringComponent]
public class PlantStatusData : IComponentData
{
    public float health;
    public float growth;
    public float foodStore;
}
