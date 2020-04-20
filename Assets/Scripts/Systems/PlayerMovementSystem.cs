using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[BurstCompile]
public class PlayerMovementSystem : ComponentSystem
{
	private Camera _camera;
	private Transform _camTransform;
	private Entity _playerGun;

	protected override void OnStartRunning()
	{
		base.OnStartRunning();
		_camera = Camera.main;
		_camTransform = _camera.transform;
		//_camTransform.rotation = quaternion.identity;

	}


	private void DestroyAllEntities()
	{
		var e = GameRegistry.EM.GetAllEntities();
		for (int i = 0; i < e.Length; i++)
		{
			GameRegistry.EM.DestroyEntity(e);
		}
	}

	protected override void OnUpdate()
	{
		if (SceneManager.GetActiveScene().buildIndex != 1)
			return;
		if (_camTransform == null)
			OnStartRunning();
		if(GameRegistry.IsDead)
		{
			if (Input.GetKeyDown(KeyCode.Return))
			{
				var scene = SceneManager.GetActiveScene();
				DestroyAllEntities();
				Debug.Log("Reload");
				SceneManager.LoadScene(2);
				OnStartRunning();
				//SceneManager.UnloadSceneAsync(scene.buildIndex);
			}
			return;
		}
		Entities.WithNone<Frozen>().WithNone<PhysicsMass>().WithAll<PlayerTag>().ForEach((Entity e) =>
		{
			GameRegistry.INST.Init();
			var prop = MassProperties.UnitSphere;
			var mass = PhysicsMass.CreateDynamic(prop, 1);
			mass.InverseInertia = 0;
			PostUpdateCommands.AddComponent(e, mass);
			PostUpdateCommands.AddComponent<PhysicsVelocity>(e);
		});

		//Existance Cost
		Entities.WithNone<Frozen>().WithAllReadOnly<PlayerTag, PlayerLifeCost>().ForEach((ref Health h, ref PlayerLifeCost cost) =>
		{
			h.Value -= Time.DeltaTime * cost.Value;
			h.Value = math.max(0, h.Value);
			if(h.Value <= 0)
			{
				GameRegistry.DeathMessage.SetActive(true);
				GameRegistry.IsDead = true;
			}
		});

		

		Entities.WithNone<Frozen>().WithAllReadOnly<PlayerTag, PhysicsMass, MoveSpeed, PlayerLifeCost>().ForEach((ref Translation t, ref Rotation r, ref MoveSpeed speed, ref PhysicsVelocity v, ref PlayerLifeCost cost, ref Health health) =>
		{
			var vel = new float3();

			if (!GameRegistry.IsDead)
			{
				//TODO: Switch to Input System
				if (Input.GetKey(KeyCode.A))
				{
					vel.x = -speed.Value;
				}
				else if (Input.GetKey(KeyCode.D))
				{
					vel.x = speed.Value;
				}

				if (Input.GetKey(KeyCode.S))
				{
					vel.z = -speed.Value;
				}
				else if (Input.GetKey(KeyCode.W))
				{
					vel.z = speed.Value;
				}

				if (!vel.Equals(0))
					health.Value -= cost.Value * Time.DeltaTime;

				if (Input.GetKey(KeyCode.LeftShift))
				{
					health.Value -= cost.Value * Time.DeltaTime * 0.5f;
					vel.z *= 2;
				}
			}
			health.Value = math.max(0, health.Value);

			v.Linear = math.rotate(r.Value, vel);
		});


		//Camera
		Entities.WithAllReadOnly<PlayerTag, LookSpeed>().ForEach((ref Rotation r, ref LookSpeed sensitivity) =>
		{
			if (GameRegistry.UpgradePanel.IsOpen)
				return;
			var mouseMove = new float2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
			var camRot = _camTransform.rotation.eulerAngles;

			camRot.y += mouseMove.x * sensitivity.Value.x;
			camRot.x -= mouseMove.y * sensitivity.Value.y;
			if(camRot.x > 90 && camRot.x < 180)
				camRot.x = 90;
			if (camRot.x < 270 && camRot.x > 180)
				camRot.x = 270;
			camRot.z = 0;

			camRot = math.radians(camRot);
			var camQ = quaternion.Euler(camRot);
			_camTransform.rotation = camQ;
			r.Value = quaternion.Euler(new float3(0, camRot.y, 0));

			
		});
	}
}
