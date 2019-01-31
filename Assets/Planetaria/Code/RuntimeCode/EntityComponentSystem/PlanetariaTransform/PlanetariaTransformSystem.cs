using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Experimental.PlayerLoop;

namespace Planetaria
{
    [UpdateBefore(typeof(Update))]
    [UpdateAfter(typeof(PlanetariaTransform))]
    public class PlanetariaTransformSystem : JobComponentSystem
    {
        // order of operations are usually scale, then rotate, then transform
        /*[BurstCompile]
        struct PlanetariaScale : IJobProcessComponentData<PlanetariaScaleComponent>
        {
            public void Execute([ReadOnly] ref PlanetariaScaleComponent scale)
            {
                // TODO: scale
            }
        }*/

        [BurstCompile]
        struct PlanetariaTransformRedirect : IJobProcessComponentData<PlanetariaDirection, PlanetariaDirectionDirty, PlanetariaPosition, PlanetariaPreviousPosition>
        {
            public void Execute(ref PlanetariaDirection direction,
                    ref PlanetariaDirectionDirty direction_dirty,
                    [ReadOnly] ref PlanetariaPosition position,
                    [ReadOnly] ref PlanetariaPreviousPosition previous_position)
            {
                // FIXME: any time the user teleports the player, velocity is invalidated (need to distinguish between Rigidbody translations and user translations)
                if (direction_dirty.data == 0) // FIXME: TODO: verify: I think this will eventually lead to significant drift (since you aren't orthonormalizing occasionally)
                {
                    Quaternion delta_rotation = Quaternion.FromToRotation(previous_position.data, position.data);
                    Vector3 adjusted_direction = delta_rotation * direction.data;
                    direction = new PlanetariaDirection { data = adjusted_direction };
                    direction_dirty = new PlanetariaDirectionDirty { data = 0 }; // struct default initialized to false
                }
            }
        }

        [BurstCompile]
        struct PlanetariaTransformSavePrevious : IJobProcessComponentData<PlanetariaPreviousPosition, Rotation>
        {
            public void Execute(ref PlanetariaPreviousPosition previous_position,
                    [ReadOnly] ref Rotation rotation)
            {
                previous_position = new PlanetariaPreviousPosition { data = (Quaternion)rotation.Value * Vector3.forward };
            }
        }

        [BurstCompile]
        struct PlanetariaTransformMove : IJobProcessComponentData<LocalToWorld, PlanetariaDirection, PlanetariaPosition>
        {
            public void Execute(ref LocalToWorld matrix,
                    [ReadOnly] ref PlanetariaDirection direction,
                    [ReadOnly] ref PlanetariaPosition position)
            {
                matrix = new LocalToWorld { Value = float4x4.LookAt(float3.zero, position.data, direction.data) };
            }
        }

        protected override JobHandle OnUpdate(JobHandle input_dependencies)
        {
            var modify_direction = new PlanetariaTransformRedirect();
            JobHandle transformation = modify_direction.Schedule<PlanetariaTransformRedirect>(this, input_dependencies); // NOTE: these should be scheduled simultaneously
            var cache = new PlanetariaTransformSavePrevious();
            transformation = cache.Schedule<PlanetariaTransformSavePrevious>(this, transformation); // TODO: verify
            var translate = new PlanetariaTransformMove();
            transformation = translate.Schedule<PlanetariaTransformMove>(this, transformation);
            return transformation;
        }
    }
}

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.