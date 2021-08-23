using Unity.Entities;
using UnityEngine;
using Unity.Collections;

public class OutputBufferElementAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public OutputData[] outputData = new OutputData[0];
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        DynamicBuffer<OutputBufferElement> dynamicBuffer = dstManager.AddBuffer<OutputBufferElement>(entity);

        for (int i = 0; i < outputData.Length; i++)
        {
            World defaultWorld = World.DefaultGameObjectInjectionWorld;
            GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(defaultWorld, null);
            Entity currentOutput = GameObjectConversionUtility.ConvertGameObjectHierarchy(outputData[i].output, settings);



            OutputBufferElement element = new OutputBufferElement { 
                outputPrefab = currentOutput, 
                requiredGrowth = outputData[i].requiredGrowth 
            };

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            ecb.AppendToBuffer(entity, element);
            ecb.Playback(dstManager);
            ecb.Dispose();
        }
    }
}
[System.Serializable] public struct OutputData
{
    public GameObject output;
    public float requiredGrowth;
}