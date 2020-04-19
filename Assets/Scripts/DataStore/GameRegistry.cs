using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class GameRegistry : MonoBehaviour
{
	public static EntityManager EM => INST._em;
	public static Entity Player => INST._player;
	public static Entity PlayerGun => INST._playerGun;
	public static Entity Plant => INST._plant;
	public static PlantSystem PlantSystem => INST._plantSystem;
	public static UIUpgradePanel UpgradePanel => INST._upgradePanel;

	public static ParticleSystem NormalRangeGunEFX => _INST._normalRangeGunEFX;
	public static ParticleSystem LongRangeGunEFX => _INST._longRangeGunEFX;

	private static GameRegistry _INST;

	public static GameRegistry INST
	{
		get
		{
			if(_INST == null)
				_INST = FindObjectOfType<GameRegistry>().Init();
			return _INST;
		}
	}

	private Entity _player;
	private Entity _playerGun;
	private Entity _plant;
	private ParticleSystem _normalRangeGunEFX;
	private ParticleSystem _longRangeGunEFX;
	private PlantSystem _plantSystem;
	private UIUpgradePanel _upgradePanel;
	private EntityManager _em;

	private void Start()
	{
		Init();
	}

	GameRegistry Init()
	{
#if UNITY_EDITOR
		Debug.Log("Game Regisitry Init");
#endif
		_em = World.DefaultGameObjectInjectionWorld.EntityManager;
		var entities = _em.GetAllEntities();
		for (int i = 0; i < entities.Length; i++)
		{
			if (_em.HasComponent<PlayerTag>(entities[i]))
				_player = entities[i];
			if (_em.HasComponent<PlayerWaterGun>(entities[i]))
				_playerGun = entities[i];
			if (_em.HasComponent<Plant>(entities[i]))
				_plant = entities[i];
		}
		_plantSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<PlantSystem>();
		entities.Dispose();
		_normalRangeGunEFX = GameObject.Find("NormalRange").GetComponent<ParticleSystem>();
		_longRangeGunEFX = GameObject.Find("LongRange").GetComponent<ParticleSystem>();
		return this;
	}

	public static void SetUpgradePanel(UIUpgradePanel upgradePanel)
	{
		INST._upgradePanel = upgradePanel;
	}

	internal static Health GetPlayerHealth()
	{
		return EM.GetComponentData<Health>(Player);
	}

	public static void SetPlayerHealth(Health health)
	{
		EM.SetComponentData(Player, health);
	}
}
