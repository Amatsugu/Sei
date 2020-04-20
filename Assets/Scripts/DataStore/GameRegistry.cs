using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

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
	public static GameObject UpgradePrompt => _INST.upgradePrompt;
	public static GameObject EatPrompt => _INST.eatPlantPrompt;
	public static GameObject ActivatePrompt => _INST.activatePrompt;
	public static AudioSource PlayerSource => _INST.playerSrc;
	public static AudioSource WaterSound => _INST.waterSound;
	public static AudioClip EatSound => _INST.eatSound;
	public static AudioClip DrainSound => _INST.drainSound;
	public static AudioClip CollectSound => _INST.collectSound;

	public static bool IsDead
	{
		get => _INST._isDead;
		set => _INST._isDead = value;
	}


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

	public static GameObject DeathMessage => _INST.deathMessage;

	public GameObject upgradePrompt;
	public GameObject eatPlantPrompt;
	public GameObject activatePrompt;
	public GameObject deathMessage;
	public AudioSource waterSound;
	public AudioSource playerSrc;
	public AudioClip eatSound;
	public AudioClip drainSound;
	public AudioClip collectSound;

	private Entity _player;
	private Entity _playerGun;
	private Entity _plant;
	private ParticleSystem _normalRangeGunEFX;
	private ParticleSystem _longRangeGunEFX;
	private PlantSystem _plantSystem;
	private UIUpgradePanel _upgradePanel;
	private EntityManager _em;
	private bool _isDead;

	private void Start()
	{
		Init();
	}

	public GameRegistry Init()
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
		try
		{

		_normalRangeGunEFX = GameObject.Find("NormalRange").GetComponent<ParticleSystem>();
		_longRangeGunEFX = GameObject.Find("LongRange").GetComponent<ParticleSystem>();
		}catch
		{

		}
		return this;
	}

	public static void SetUpgradePanel(UIUpgradePanel upgradePanel)
	{
		_INST = null;
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
