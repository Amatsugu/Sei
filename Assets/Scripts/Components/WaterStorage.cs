using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct WaterStorage : IComponentData
{
	public float Value;
	public float maxCapacity;
}
