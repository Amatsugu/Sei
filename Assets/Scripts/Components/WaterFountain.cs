using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct WaterFountain : IComponentData
{
	public float level;
	public float baseGenerationRate;
}
