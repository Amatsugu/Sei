using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Growth Upgrade")]
public class PlantGrowthUpgrade : UpgradeBase
{
	public float upgradeAmmount = 0.1f;

	public override bool ApplyUpgrade(int level)
	{
		if (base.ApplyUpgrade(level))
		{
			var rate  = GameRegistry.EM.GetComponentData<PlantGrowthRate>(GameRegistry.Plant);
			rate.Value -= upgradeAmmount;
			GameRegistry.EM.SetComponentData(GameRegistry.Plant, rate);
			return true;
		}
		else
			return false;

	}
}
