using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

public class CameraAttachmentSystem : MonoBehaviour
{
	private Transform _transform;
	private Entity _player;

	private void Start()
	{
		_transform = transform;
		_player = GameRegistry.Player;
	}

	private void LateUpdate()
	{
		_transform.position = GameRegistry.EM.GetComponentData<Translation>(_player).Value + new float3(0, 1.1f, 0);
	}
}
