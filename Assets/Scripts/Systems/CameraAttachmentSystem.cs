using Unity.Mathematics;
using Unity.Transforms;

using UnityEngine;

public class CameraAttachmentSystem : MonoBehaviour
{
	private Transform _transform;

	private void Start()
	{
		_transform = transform;
	}

	private void LateUpdate()
	{
		try
		{
			_transform.position = GameRegistry.EM.GetComponentData<Translation>(GameRegistry.Player).Value + new float3(0, 1.1f, 0);
		}
		catch
		{
		}
	}
}