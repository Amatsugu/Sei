using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(PlantSystem))]
public class PlayerInteractionSystem : ComponentSystem
{
	private Transform _camTransform;
	private BuildPhysicsWorld _buildPhysicsWorld;
	private PlantSystem _plant;
	private ParticleSystem _curEFX;
	private ParticleSystem.EmissionModule _curEmission;
	private float _baseEmissionRate;
	private ParticleSystem _normalEFX;
	private ParticleSystem _longEFX;
	protected override void OnStartRunning()
	{
		base.OnStartRunning();
		_camTransform = Camera.main.transform;
		_buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuildPhysicsWorld>();
		_plant = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<PlantSystem>();
		_curEFX = GameRegistry.NormalRangeGunEFX;
		_curEmission = _curEFX.emission;
		_baseEmissionRate = _curEmission.rateOverTimeMultiplier;
		_longEFX = GameRegistry.LongRangeGunEFX;
	}

	protected override void OnUpdate()
	{
		if (GameRegistry.UpgradePanel.IsOpen)
			return;
		EatPlant();
		WaterPlant();
		UpgradePlant();
		RangeUpgrade();
	}

	private void RangeUpgrade()
	{
		Entities.WithAllReadOnly<PlayerTag, RangeUpgradeTag>().ForEach(e =>
		{
			_curEFX = _longEFX;
			_curEmission = _longEFX.emission;
			_baseEmissionRate = _curEmission.rateOverTimeMultiplier;
			PostUpdateCommands.RemoveComponent<RangeUpgradeTag>(e);
		});

	}

	private void UpgradePlant()
	{
		Entities.WithNone<Frozen>().WithAllReadOnly<PlayerTag>().ForEach((Entity e, ref Health health) =>
		{
			if (!Input.GetKeyUp(KeyCode.Tab))
				return;
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
			if (hit)
			{
				GameRegistry.UpgradePanel.Show();
			}
		});
	}

	private void WaterPlant()
	{
		if (Input.GetKeyUp(KeyCode.Mouse0))
			_curEFX.Stop();
		Entities.WithAllReadOnly<PlayerTag>().ForEach((Entity e, ref WaterStorage storage) =>
		{
			if (!Input.GetKey(KeyCode.Mouse0))
				return;
			var waterToGive = math.min(25, storage.Value);
			if (storage.Value <= 0)
			{
				_curEFX.Stop();
				return;
			}
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
				_curEmission.rateOverTimeMultiplier = _baseEmissionRate * (waterToGive/25);
				if (!_curEFX.isEmitting)
				{
					_curEFX.Play();
				}
			}
			else
			{
				storage.Value -= 1 * Time.DeltaTime;
				_curEmission.rateOverTimeMultiplier = _baseEmissionRate;
				if (!_curEFX.isEmitting)
				{
					_curEFX.Play();
				}
			}
			storage.Value = math.max(0, storage.Value);
		});
	}

	private void EatPlant()
	{
		Entities.WithAllReadOnly<PlayerTag>().ForEach((Entity e, ref Health health) =>
		{
			if (!Input.GetKeyUp(KeyCode.E))
				return;
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
