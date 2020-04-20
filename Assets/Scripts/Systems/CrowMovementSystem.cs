using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

public class CrowMovementSystem : JobComponentSystem
{
	public float minAltitude = 1;
	public float maxAltitude = 8;

	public struct BoidsMovementJob : IJobChunk
	{
		[ReadOnly] public ArchetypeChunkComponentType<Translation> translationType;
		public ArchetypeChunkComponentType<PhysicsVelocity> velocityType;
		public ArchetypeChunkComponentType<Rotation> rotationType;
		[ReadOnly] internal ArchetypeChunkComponentType<MoveSpeed> moveSpeedType;
		public float dt;
		public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
		{
			var translations = chunk.GetNativeArray(translationType);
			var velocities = chunk.GetNativeArray(velocityType);
			var rotations = chunk.GetNativeArray(rotationType);
			var speeds = chunk.GetNativeArray(moveSpeedType);

			for (int i = 0; i < chunk.Count; i++)
			{
				var v = velocities[i];

				var totalVelocity = v.Linear;
					
				totalVelocity += Flocking(i, ref translations, chunk.Count) * dt;

				totalVelocity += CollisionAvoidance(i, ref translations, chunk.Count) * dt;

				totalVelocity += MatchSpeed(i, ref velocities, chunk.Count) * dt;

				totalVelocity += TendToCenter(translations[i], 30, 50) * dt;

				totalVelocity += TendToAltitude(translations[i], 10) * dt;

				totalVelocity = LimitVelocity(totalVelocity, speeds[i].Value);

				v.Linear = totalVelocity;

				velocities[i] = v;

				rotations[i] = new Rotation
				{
					Value = quaternion.LookRotation(totalVelocity, math.up())
				};
			}
		}


		private float3 TendToAltitude(Translation t, float altitude)
		{
			var tAlt = new float3(t.Value.x, altitude, t.Value.z);
			if (t.Value.y != altitude)
				return (tAlt - t.Value) / 100;
			else
				return 0;
		}

		private float3 LimitVelocity(float3 velocity, float limit)
		{
			if (math.lengthsq(velocity) > limit* limit)
				return (math.normalizesafe(velocity) * limit);
			else
				return velocity;
		}


		private float3 TendToCenter(Translation translation, float minDist, float maxDist)
		{
			var c = new float3(0, 10, 0);
			var dist = math.lengthsq(c - translation.Value);
			minDist *= minDist;
			maxDist *= maxDist;
			if (dist > minDist)
				return (c - translation.Value) / 100;
			else
				return 0;
		}

		private float3 MatchSpeed(int j, ref NativeArray<PhysicsVelocity> velocities, int count)
		{
			float3 avgVel = float3.zero;

			for (int i = 0; i < count; i++)
			{
				if (i != j)
					avgVel += velocities[i].Linear;
			}

			avgVel /= count - 1;
			return (avgVel - velocities[j].Linear) / 8;
		}

		private float3 CollisionAvoidance(int j, ref NativeArray<Translation> translations, int count)
		{
			var c = float3.zero;
			for (int i = 0; i < count; i++)
			{
				if (i != j)
				{
					if(math.lengthsq(translations[i].Value - translations[j].Value) < 9)
						c -= (translations[i].Value - translations[j].Value);
				}
			}
			return c;
		}

		private float3 Flocking(int j, ref NativeArray<Translation> translations, int count)
		{
			float3 avgPos = float3.zero;

			for (int i = 0; i < count; i++)
			{
				if (i != j)
					avgPos += translations[i].Value;
			}

			avgPos /= count - 1;
			return (avgPos - translations[j].Value) / 100;
		}
	}

	private EntityQuery _entityQuery;

	protected override void OnCreate()
	{
		base.OnCreate();
		_entityQuery = GetEntityQuery(ComponentType.ReadOnly<Translation>(), typeof(PhysicsVelocity), typeof(Rotation), ComponentType.ReadOnly<CrowTag>(), ComponentType.ReadOnly<MoveSpeed>());
	}

	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		var boidsJob = new BoidsMovementJob
		{
			rotationType = GetArchetypeChunkComponentType<Rotation>(false),
			translationType = GetArchetypeChunkComponentType<Translation>(true),
			velocityType = GetArchetypeChunkComponentType<PhysicsVelocity>(false),
			moveSpeedType = GetArchetypeChunkComponentType<MoveSpeed>(true),
			dt = Time.DeltaTime
		};
		inputDeps = boidsJob.Schedule(_entityQuery, inputDeps);

		return inputDeps;
	}
}

public class CrowInitSystem : ComponentSystem
{
	protected override void OnUpdate()
	{
		Entities.WithNone<Frozen, PhysicsMass>().WithAllReadOnly<CrowTag, MoveSpeed>().ForEach((Entity e, ref MoveSpeed speed) =>
		{
			var prop = MassProperties.UnitSphere;
			var mass = PhysicsMass.CreateDynamic(prop, 1);
			mass.InverseInertia = 0;
			PostUpdateCommands.AddComponent(e, mass);
			var vel = UnityEngine.Random.onUnitSphere * speed.Value;
			vel.y = 0;
			PostUpdateCommands.AddComponent(e, new PhysicsVelocity
			{
				Linear = vel
			});
			PostUpdateCommands.AddComponent(e, new PhysicsGravityFactor
			{
				Value = 0
			});
		});
	}
}