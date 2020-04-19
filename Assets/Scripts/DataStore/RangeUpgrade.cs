using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Range Upgrade")]
public class RangeUpgrade : UpgradeBase
{
	public override bool ApplyUpgrade(int level)
	{
		if (base.ApplyUpgrade(level))
		{
			GameRegistry.EM.AddComponent<RangeUpgradeTag>(GameRegistry.Player);
			return true;
		}
		return false;
	}
}
