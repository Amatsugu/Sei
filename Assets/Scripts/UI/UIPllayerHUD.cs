using UnityEngine;

public class UIPllayerHUD : MonoBehaviour
{
	public RectTransform healthFill;
	public RectTransform waterStorageFill;
	public Material gunMaterial;

	private void Update()
	{
		try
		{
			var _EM = GameRegistry.EM;
			var hp = _EM.GetComponentData<Health>(GameRegistry.Player);
			healthFill.anchorMax = new Vector2(hp.Value / hp.maxHealth, 1);

			var water = _EM.GetComponentData<WaterStorage>(GameRegistry.Player);
			var waterLevel = water.Value / water.maxCapacity;
			waterStorageFill.anchorMax = new Vector2(waterLevel, 1);
			gunMaterial.SetFloat("Fill", waterLevel);
		}
		catch
		{
		}
	}
}