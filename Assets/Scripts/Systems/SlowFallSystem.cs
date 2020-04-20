using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public class SlowFallSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity e, ref Translation t, ref SlowFall f) =>
        {
            var pos = t.Value;
            if (pos.y < -100)
                PostUpdateCommands.RemoveComponent<SlowFall>(e);
            pos.y -= f.Value * Time.DeltaTime;
            t.Value = pos;
        });
    }
}