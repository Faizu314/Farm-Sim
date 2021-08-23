using Unity.Entities;

[GenerateAuthoringComponent]
public class CompoundContentsData : IComponentData
{
    public float waterContent;
    public float nitrogenContent;
    public float phosphorousContent;
    public float potassiumContent;
    public float calciumContent;
}
