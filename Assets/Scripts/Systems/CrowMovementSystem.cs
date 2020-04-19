using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class CrowMovementSystem : ComponentSystem
{
	public float minAltitude = 1;
	public float maxAltitude = 8;

	protected override void OnUpdate()
	{
		Entities.WithNone<Frozen>().WithNone<PhysicsMass>().WithAll<CrowTag>().ForEach((Entity e) =>
		{
			var prop = MassProperties.UnitSphere;
			var mass = PhysicsMass.CreateDynamic(prop, 1);
			//mass.InverseInertia = 0;
			PostUpdateCommands.AddComponent(e, mass);
			PostUpdateCommands.AddComponent<PhysicsVelocity>(e);
			PostUpdateCommands.AddComponent(e, new PhysicsGravityFactor
			{
				Value = 0
			});
		});


		Entities.WithAllReadOnly<CrowTag>().ForEach((ref Translation t, ref Rotation r, ref PhysicsVelocity v, ref MoveSpeed speed) =>
		{
			var pos = t.Value;
			var vel = v.Linear;
			var aVel = v.Angular;


			//var plantPos = EntityManager.GetComponentData<Translation>(GameRegistry.Plant);

			vel = new float3(0,0,speed.Value);
			var plantDist = math.lengthsq(pos);
			if(plantDist > 10 * 10)
			{
				aVel.y = speed.Value;
			}

			r.Value = Quaternion.LookRotation(vel, math.up());


			v.Linear = math.rotate(r.Value, vel);
			v.Angular = aVel;
			t.Value = pos;
		});
	}
}
