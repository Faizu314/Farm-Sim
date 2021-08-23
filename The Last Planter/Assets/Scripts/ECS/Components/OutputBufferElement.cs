using Unity.Entities;

[InternalBufferCapacity(10)]
public struct OutputBufferElement : IBufferElementData
{
    public Entity outputPrefab;
    public float requiredGrowth;
}
