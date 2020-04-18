using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct LookSpeed : IComponentData
{
	public float2 Value;
}
