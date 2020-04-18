using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class WaterFountainSystem : ComponentSystem
{
	protected override void OnUpdate()
	{
		Entities.WithAllReadOnly<WaterFountain>().ForEach((Entity e, ref WaterFountain fountain, ref WaterStorage storage) =>
		{
			if(storage.Value < storage.maxCapacity / 2)
				storage.Value += fountain.baseGenerationRate + (1 + fountain.level) * Time.DeltaTime;
		});
	}
}
