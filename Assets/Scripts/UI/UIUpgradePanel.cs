﻿using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class UIUpgradePanel : UIPanel
{
	public UIUpgradeButton upgradeButtonPrefab;
	public UpgradeBase[] upgrades;
	public RectTransform buttonsRoot;

	private int[] _upgradeLevels;
	private UIUpgradeButton[] _buttons;
	private bool _hasInit = false;

	protected override void Start()
	{
		base.Start();
		GameRegistry.SetUpgradePanel(this);
	}

	protected void Init()
	{
		_hasInit = true;
		_upgradeLevels = new int[upgrades.Length];
		CreateButtons();
	}

	private void CreateButtons()
	{
		_buttons = new UIUpgradeButton[upgrades.Length];
		for (int i = 0; i < upgrades.Length; i++)
		{
			var upgradeIndex = i;
			_buttons[i] = Instantiate(upgradeButtonPrefab, buttonsRoot, false);
			_buttons[i].Show(upgrades[i]);
			_buttons[i].upgradeButton.onClick.AddListener(() => OnUpgrade(upgradeIndex));
		}
		UpdateButtonStates();
	}

	private void OnUpgrade(int upgradeIndex)
	{
		var upgrade = upgrades[upgradeIndex];
		if (_upgradeLevels[upgradeIndex] >= upgrade.maxLevel)
			return;
		if(upgrade.ApplyUpgrade(_upgradeLevels[upgradeIndex] + 1))
		{
			_upgradeLevels[upgradeIndex]++;
		}
		UpdateButtonStates();
	}

	protected override void Update()
	{
		base.Update();
		if (!_hasInit || !GameRegistry.EM.Exists(GameRegistry.Player))
			Init();
		if (Input.GetKeyDown(KeyCode.Tab))
			Hide();
		UpdateButtonStates();
	}

	public override void Show()
	{
		base.Show();
		Debug.Log("Show");
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		if (!GameRegistry.EM.HasComponent<Frozen>(GameRegistry.Player))
			GameRegistry.EM.AddComponent<Frozen>(GameRegistry.Player);
	}


	private void UpdateButtonStates()
	{
		Health playerHealth = GameRegistry.GetPlayerHealth();
		for (int i = 0; i < _buttons.Length; i++)
		{
			var upgradeCost = upgrades[i].GetUpgradeCost(_upgradeLevels[i]);
			if (upgradeCost >= playerHealth.Value || _upgradeLevels[i] >= upgrades[i].maxLevel)
				_buttons[i].upgradeButton.interactable = false;
			else
				_buttons[i].upgradeButton.interactable = true;
			_buttons[i].upgradeCost.SetText(upgradeCost.ToString());
		}
	}

	public override void Hide()
	{
		base.Hide();
		Debug.Log("Hide");
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		if (GameRegistry.EM.HasComponent<Frozen>(GameRegistry.Player))
			GameRegistry.EM.RemoveComponent<Frozen>(GameRegistry.Player);
	}
}