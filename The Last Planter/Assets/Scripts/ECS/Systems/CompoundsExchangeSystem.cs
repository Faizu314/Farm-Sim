using Unity.Entities;
using PlantSimulation;
public class CompoundsExchangeSystem : SystemBase
{
    private static float DELTA_TIME_PER_STEP = 0.001f;
    private static float EXCHANGE_TICK_PERIOD = 2f;

    private float deltaTime;

    protected override void OnUpdate()
    {
        deltaTime += Time.DeltaTime;
        if (deltaTime <= EXCHANGE_TICK_PERIOD)
            return;
        else
            deltaTime = 0f;

        Entities.
        WithoutBurst().
        WithNone<DisabledTag>().
        ForEach((CompoundContentsData myContents, in SustainerData sustainer, in PlantPropertiesData properties) =>
        {
            if (sustainer.value == Entity.Null)
                return;

            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            CompoundContentsData otherContents = entityManager.GetComponentData<CompoundContentsData>(sustainer.value);
            float[] deltaContents = new float[4];
            deltaContents[0] = Transport.Diffuse(otherContents.nitrogenContent, myContents.nitrogenContent, properties.maxDiffusion) * DELTA_TIME_PER_STEP;
            deltaContents[1] = Transport.Diffuse(otherContents.phosphorousContent, myContents.phosphorousContent, properties.maxDiffusion) * DELTA_TIME_PER_STEP;
            deltaContents[2] = Transport.Diffuse(otherContents.potassiumContent, myContents.potassiumContent, properties.maxDiffusion) * DELTA_TIME_PER_STEP;
            deltaContents[3] = Transport.Diffuse(otherContents.calciumContent, myContents.calciumContent, properties.maxDiffusion) * DELTA_TIME_PER_STEP;

            //Make sure to Conserve the transport amount here

            myContents.nitrogenContent += deltaContents[0];
            myContents.phosphorousContent += deltaContents[1];
            myContents.potassiumContent += deltaContents[2];
            myContents.calciumContent += deltaContents[3];

            otherContents.nitrogenContent -= deltaContents[0];
            otherContents.phosphorousContent -= deltaContents[1];
            otherContents.potassiumContent -= deltaContents[2];
            otherContents.calciumContent -= deltaContents[3];

        }).Run();
    }
}
