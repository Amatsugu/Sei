using System.Collections;
using System.Collections.Generic;
using Unity.Physics;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics.Systems;
using Unity.Collections;
using Unity.Mathematics;

[UpdateBefore(typeof(PlantSystem))]
public class CollisionSystem : JobComponentSystem
{
    public BuildPhysicsWorld buildPhysicsWorld;
    public StepPhysicsWorld stepPhysicsWorld;

    protected override void OnCreate()
    {
        base.OnCreate();
        stepPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<StepPhysicsWorld>();
        buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuildPhysicsWorld>();
    }

    private struct PlantConnectJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<PlayerTag> playerTag;
        [ReadOnly] public ComponentDataFromEntity<ConnectorTag> connectorTag;
        public ComponentDataFromEntity<Health> health;
        public EntityCommandBuffer.Concurrent cmb;
        public float dt;

        public void Execute(TriggerEvent tEvent)
        {
            if(connectorTag.HasComponent(tEvent.Entities.EntityB))
                Connect(tEvent.Entities.EntityA, tEvent.Entities.EntityB);
            else if(connectorTag.HasComponent(tEvent.Entities.EntityA))
                Connect(tEvent.Entities.EntityB, tEvent.Entities.EntityA);
        }

        public void Connect(Entity player, Entity connector)
        {
            var plant = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<PlantSystem>();
            if (plant.isDead)
                return;
            var h = health[player];
            var healthToTake = 5f;
            healthToTake = math.min(PlantSystem.maxResourceLevel - plant.energyLevel, healthToTake);
            h.Value -= healthToTake * dt;
            h.Value = math.clamp(h.Value, 0, h.maxHealth);
            health[player] = h;
            plant.energyLevel += healthToTake * dt * 4;
        }
    }

    private struct WaterFountainDrainJob : ITriggerEventsJob
    {
        public ComponentDataFromEntity<WaterStorage> waterStorage;
        [ReadOnly] public ComponentDataFromEntity<WaterFountain> fountainData;
        public float dt;

        public void Execute(TriggerEvent triggerEvent)
        {
            if(fountainData.HasComponent(triggerEvent.Entities.EntityA))
                DrainFountain(triggerEvent.Entities.EntityB, triggerEvent.Entities.EntityA);
            else if (fountainData.HasComponent(triggerEvent.Entities.EntityB))
                DrainFountain(triggerEvent.Entities.EntityA, triggerEvent.Entities.EntityB);
        }

        public void DrainFountain(Entity player, Entity fountain)
        {
            var fountainStorage = waterStorage[fountain];
            var playerStorage = waterStorage[player];

            float waterToTake = math.min(playerStorage.maxCapacity - playerStorage.Value, 5);
            waterToTake = math.min(fountainStorage.Value, waterToTake);

            fountainStorage.Value -= waterToTake * dt;
            playerStorage.Value += waterToTake * dt;

            fountainStorage.Value = math.clamp(fountainStorage.Value, 0, fountainStorage.maxCapacity);

            waterStorage[fountain] = fountainStorage;
            waterStorage[player] = playerStorage;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var plantConnect = new PlantConnectJob
        {
            connectorTag = GetComponentDataFromEntity<ConnectorTag>(true),
            playerTag = GetComponentDataFromEntity<PlayerTag>(true),
            health = GetComponentDataFromEntity<Health>(false),
            cmb = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer().ToConcurrent(),
            dt = Time.DeltaTime
        };

        inputDeps = plantConnect.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);

        var fountainDrain = new WaterFountainDrainJob
        {
            waterStorage = GetComponentDataFromEntity<WaterStorage>(false),
            fountainData = GetComponentDataFromEntity<WaterFountain>(true),
            dt = Time.DeltaTime
        };

        inputDeps = fountainDrain.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);

        return inputDeps;
    }
}
