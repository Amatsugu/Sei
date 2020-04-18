using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public class PlantSystem : ComponentSystem
{
	public bool isDead = false;
	public float waterLevel;
	public float energyLevel;
	public const int maxResourceLevel = 100;

	protected override void OnCreate()
	{
		base.OnCreate();
		waterLevel = energyLevel = maxResourceLevel;
	}

	protected override void OnUpdate()
	{
		if (isDead)
			return;
		Entities.WithAnyReadOnly<PlantLightTag, Plant>().WithNone<NonUniformScale>().ForEach((Entity e) =>
		{
			PostUpdateCommands.AddComponent(e, new NonUniformScale { Value = 1 });
		});


		//Drain Resources
		Entities.WithAllReadOnly<Plant, PlantGrowthRate>().ForEach((ref PlantEnergyDrain eDrain, ref PlantWaterDrain wDrain, ref Health health, ref PlantGrowthRate growthRate) =>
		{
			var plantHealth = health.Value;
			energyLevel = math.min(energyLevel, maxResourceLevel);
			waterLevel = math.min(waterLevel, maxResourceLevel);

			if (plantHealth == 0)
			{
				isDead = true;
				return;
			}
			var eUsed = eDrain.Value * Time.DeltaTime;
			var wUsed = wDrain.Value * Time.DeltaTime;

			if (energyLevel > 0 && waterLevel > 0 && plantHealth < 100)
			{
				plantHealth += (eUsed + wUsed) * growthRate.Value;
				plantHealth = math.min(plantHealth, 100);
				energyLevel -= eUsed;
			}
			if (energyLevel >= maxResourceLevel && waterLevel > 0)
			{
				plantHealth += eUsed * growthRate.Value;
				plantHealth = math.min(plantHealth, 200);
				energyLevel -= eUsed;
			}

			if(waterLevel <= 0)
				plantHealth -= wUsed;
			waterLevel -= wUsed;
			energyLevel = math.max(energyLevel, 0);
			waterLevel = math.max(waterLevel, 0);
			plantHealth = math.max(plantHealth, 0);
			health.Value = plantHealth;
		});

		//Update Light
		Entities.WithAllReadOnly<PlantLightTag>().ForEach((ref NonUniformScale scale) =>
		{
			scale.Value = energyLevel / 100;
		});

		//Update Health
		Entities.WithAllReadOnly<Plant>().ForEach((ref NonUniformScale scale, ref Plant plant, ref Health plantHealth) =>
		{
			scale.Value = plant.InitialSize * (plantHealth.Value / plantHealth.maxHealth);
		});

		//Update Water Level
		Entities.WithAllReadOnly<PlantWaterLevelFill>().ForEach((ref Translation t, ref PlantWaterLevelFill waterFill, ref NonUniformScale scale) =>
		{
			var fill = waterFill.InitialSize;

			fill.y *= waterLevel/100;
			var pos = t.Value;
			pos.y = (1-(-fill.y/2))-1.5f;

			t.Value = pos;

			scale.Value = fill;
		});

	}
}
