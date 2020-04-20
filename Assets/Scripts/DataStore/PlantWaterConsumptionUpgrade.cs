using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Water Consumption")]
public class PlantWaterConsumptionUpgrade : UpgradeBase
{
	public float upgradeAmmount = 0.1f;

	public override bool ApplyUpgrade(int level)
	{
		if (base.ApplyUpgrade(level))
		{
			var rate  = GameRegistry.EM.GetComponentData<PlantWaterDrain>(GameRegistry.Plant);
			rate.Value += upgradeAmmount;
			GameRegistry.EM.SetComponentData(GameRegistry.Plant, rate);
			return true;
		}
		else
			return false;

	}
}
