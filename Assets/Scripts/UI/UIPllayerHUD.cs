using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class UIPllayerHUD : MonoBehaviour
{
	public RectTransform healthFill;
	public RectTransform waterStorageFill;

	private Entity _player;
	private EntityManager _EM;

	private void Start()
	{
		_EM = GameRegistry.EM;
		_player = GameRegistry.Player;
	}

	private void Update()
	{
		var hp = _EM.GetComponentData<Health>(_player);
		healthFill.anchorMax = new Vector2(hp.Value /hp.maxHealth, 1);

		var water = _EM.GetComponentData<WaterStorage>(_player);
		waterStorageFill.anchorMax = new Vector2(water.Value / water.maxCapacity, 1);
	}
}