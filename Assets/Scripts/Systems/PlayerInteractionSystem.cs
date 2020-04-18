using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using UnityEngine;

[UpdateBefore(typeof(PlantSystem))]
public class PlayerInteractionSystem : ComponentSystem
{
	private Transform _camTransform;
	private BuildPhysicsWorld _buildPhysicsWorld;
	private PlantSystem _plant;

	protected override void OnStartRunning()
	{
		base.OnStartRunning();
		_camTransform = Camera.main.transform;
		_buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuildPhysicsWorld>();
		_plant = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<PlantSystem>();
	}

	protected override void OnUpdate()
	{
		EatPlant();
		WaterPlant();
		UpgradePlant();
	}

	private void UpgradePlant()
	{
		if (!Input.GetKeyUp(KeyCode.Tab))
			return;
		Entities.WithAllReadOnly<PlayerTag>().ForEach((Entity e, ref Health health) =>
		{
			var hit = _buildPhysicsWorld.PhysicsWorld.CastRay(new Unity.Physics.RaycastInput
			{
				Start = _camTransform.position,
				End = _camTransform.forward * 3,
				Filter = new Unity.Physics.CollisionFilter
				{
					BelongsTo = 1u << 0,
					CollidesWith = 1u << 0
				}
			}, out var hitInfo);
			if (hit)
			{
				if(health.Value - 1 >= 25)
				{
					health.Value -= 25;
					var drain = EntityManager.GetComponentData<PlantWaterDrain>(hitInfo.Entity);
					drain.Value -= 0.1f;
					PostUpdateCommands.SetComponent(hitInfo.Entity, drain);
					Debug.Log($"Reduced water consumtion {drain.Value + 0.1f} > {drain.Value}");
				}
			}
		});
	}

	private void WaterPlant()
	{
		if (!Input.GetKey(KeyCode.F))
			return;
		Entities.WithAllReadOnly<PlayerTag>().ForEach((Entity e, ref WaterStorage storage) =>
		{
			var waterToGive = math.min(25, storage.Value);
			var hit = _buildPhysicsWorld.PhysicsWorld.CastRay(new Unity.Physics.RaycastInput
			{
				Start = _camTransform.position,
				End = _camTransform.forward * 3,
				Filter = new Unity.Physics.CollisionFilter
				{
					BelongsTo = 1u << 0,
					CollidesWith = 1u << 0
				}
			});
			if(hit)
			{
				waterToGive = math.min(PlantSystem.maxResourceLevel - _plant.waterLevel, waterToGive);
				_plant.waterLevel += waterToGive * Time.DeltaTime;
				storage.Value -= waterToGive * Time.DeltaTime;
				storage.Value = math.max(0, storage.Value);
			}
		});
	}

	private void EatPlant()
	{
		if (!Input.GetKeyUp(KeyCode.E))
			return;
		Entities.WithAllReadOnly<PlayerTag>().ForEach((Entity e, ref Health health) =>
		{
			var healthToTake = math.min(25, health.maxHealth - health.Value);
			var hit = _buildPhysicsWorld.PhysicsWorld.CastRay(new Unity.Physics.RaycastInput
			{
				Start = _camTransform.position,
				End = _camTransform.forward * 3,
				Filter = new Unity.Physics.CollisionFilter
				{
					BelongsTo = 1u << 0,
					CollidesWith = 1u << 0
				}
			}, out var hitInfo);
			if (hit)
			{
				var plantHealth = EntityManager.GetComponentData<Health>(hitInfo.Entity);
				healthToTake = math.min(plantHealth.maxHealth / 2, healthToTake);
				if (plantHealth.Value <= healthToTake)
					return;
				plantHealth.Value -= healthToTake;
				health.Value += healthToTake;
				PostUpdateCommands.SetComponent(hitInfo.Entity, plantHealth);
			}
		});
	}
}
