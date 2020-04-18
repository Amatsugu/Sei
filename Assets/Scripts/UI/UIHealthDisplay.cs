using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class UIHealthDisplay : MonoBehaviour
{
	public TMP_Text healthText;

	private Entity _player;
	private EntityManager _EM;
	private PlantSystem _plant;
	private Entity _plantEntity;

	void Start()
	{
		_EM = World.DefaultGameObjectInjectionWorld.EntityManager;
		var entities = _EM.GetAllEntities();

		for (int i = 0; i < entities.Length; i++)
		{
			if (_EM.HasComponent<PlayerTag>(entities[i]))
				_player = entities[i];
			if (_EM.HasComponent<Plant>(entities[i]))
				_plantEntity = entities[i];
		}
		entities.Dispose();
		_plant = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<PlantSystem>();
	}

	void Update()
	{
		var sb = new StringBuilder();
		sb.AppendLine("Player");
		sb.AppendLine($"Health {math.round(_EM.GetComponentData<Health>(_player).Value)}");
		sb.AppendLine($"Water {math.round(_EM.GetComponentData<WaterStorage>(_player).Value)}");
		sb.AppendLine("Plant");
		sb.AppendLine($"Health: {math.round(_EM.GetComponentData<Health>(_plantEntity).Value)}");
		sb.AppendLine($"Power: {math.round(_plant.energyLevel)}");
		sb.AppendLine($"Water: {math.round(_plant.waterLevel)}");
		healthText.SetText(sb);
	}
}
