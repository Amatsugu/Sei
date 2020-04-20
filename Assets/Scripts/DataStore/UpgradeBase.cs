using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeBase : ScriptableObject
{
	public string Description;
	public Sprite sprite;
	public int cost;
	[Range(1,5)]
	public int maxLevel;
	public int costPerLevel;

	public virtual bool ApplyUpgrade(int level)
	{
		bool upgraded = false;
		var playeHealth = GameRegistry.GetPlayerHealth();
		var upgradeCost = GetUpgradeCost(level);
		if (playeHealth.Value >= upgradeCost)
		{
			upgraded = true;
			playeHealth.Value -= upgradeCost;
			GameRegistry.SetPlayerHealth(playeHealth);
		}
		return upgraded;
	}

	public virtual int GetUpgradeCost(int level) => cost + (costPerLevel * level);
}
