using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUpgradeButton : UIButtonHover
{
	public Image icon;
	public TMP_Text upgradeName;
	public TMP_Text upgradeCost;
	public Button upgradeButton;
	[HideInInspector]
	public UpgradeBase curUpgrade;

	public void Show(UpgradeBase upgrade)
	{
		curUpgrade = upgrade;
		icon.sprite = upgrade.sprite;
		upgradeName.SetText(upgrade.name);
		//.SetText(upgrade.Description);
		upgradeCost.SetText(upgrade.cost.ToString());
		gameObject.SetActive(true);
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}

}
