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
            public void Execute(
                    [ReadOnly] ref PlanetariaScaleComponent scale)
            {
                // TODO: scale
            }
        }*/

        //[BurstCompile]
        struct PlanetariaTransformRedirect : IJobProcessComponentData<PlanetariaDirection, PlanetariaDirectionDirty, PlanetariaPosition, PlanetariaPreviousPosition>
        {
            public void Execute(
                    /*[ReadWrite]*/ ref PlanetariaDirection direction,
                    [ReadOnly] ref PlanetariaDirectionDirty direction_dirty,
                    [ReadOnly] ref PlanetariaPosition position,
                    [ReadOnly] ref PlanetariaPreviousPosition previous_position)
            {
                // FIXME: any time the user teleports the player, velocity is invalidated (need to distinguish between Rigidbody translations and user translations)
                if (direction_dirty.data == 0) // FIXME: TODO: verify: I think this will eventually lead to significant drift (since you aren't orthonormalizing occasionally)
                {
                    Quaternion delta_rotation = Quaternion.FromToRotation(previous_position.data, position.data);
                    Vector3 adjusted_direction = delta_rotation * direction.data;
                    direction = new PlanetariaDirection { data = adjusted_direction };
                }
            }
        }

        [BurstCompile]
        struct PlanetariaTransformSavePrevious : IJobProcessComponentData<PlanetariaPreviousPosition, Rotation>
        {
            public void Execute(
                    [WriteOnly] ref PlanetariaPreviousPosition previous_position,
                    [ReadOnly] ref Rotation rotation)
            {
                previous_position = new PlanetariaPreviousPosition { data = (Quaternion)rotation.Value * Vector3.forward };
            }
        }

        [BurstCompile]
        struct PlanetariaTransformMove : IJobProcessComponentData<LocalToParent, PlanetariaDirection, PlanetariaPosition>
        {
            public void Execute(
                    /*[ReadWrite]*/  ref LocalToParent internal_matrix,
                    [ReadOnly] ref PlanetariaDirection direction,
                    [ReadOnly] ref PlanetariaPosition position)
            {
                float3 internal_position = new float3(internal_matrix.Value.c0.w, internal_matrix.Value.c1.w, internal_matrix.Value.c2.w);
                quaternion next_rotation = quaternion.LookRotationSafe(position.data, direction.data);
                float4x4 next_matrix = Matrix4x4.TRS(internal_position, next_rotation, Vector3.one);
                internal_matrix = new LocalToParent { Value = next_matrix };
            }
        }

        [BurstCompile]
        struct PlanetariaTransformCleanDirtyBits : IJobProcessComponentData<PlanetariaDirectionDirty>
        {
            public void Execute(
                    [WriteOnly] ref PlanetariaDirectionDirty direction_dirty)
            {
                // VALIDATE: This should be optimized to a memset (since you are setting all bits to zero)
                direction_dirty = new PlanetariaDirectionDirty(); // struct default initialized to false
            }
        }

        [BurstCompile]
        struct PlanetariaTransformOrthonormalize : IJobProcessComponentData<PlanetariaDirection, Rotation>
        {
            public void Execute(
                    [WriteOnly] ref PlanetariaDirection direction,
                    [ReadOnly] ref Rotation rotation)
            {
                direction = new PlanetariaDirection { data = (Quaternion)rotation.Value * Vector3.up };
            }
        }

        protected override JobHandle OnUpdate(JobHandle input_dependencies)
        {
            var redirect = new PlanetariaTransformRedirect();
            JobHandle redirect_job = redirect.Schedule<PlanetariaTransformRedirect>(this, input_dependencies); // NOTE: these should be scheduled simultaneously
            var cache = new PlanetariaTransformSavePrevious();
            JobHandle cache_job = cache.Schedule<PlanetariaTransformSavePrevious>(this, redirect_job); // TODO: verify
            var move = new PlanetariaTransformMove();
            JobHandle move_job = move.Schedule<PlanetariaTransformMove>(this, cache_job);
            var clean = new PlanetariaTransformCleanDirtyBits();
            JobHandle clean_job = clean.Schedule<PlanetariaTransformCleanDirtyBits>(this, move_job); // this and next step are parallel
            var orthonormalize = new PlanetariaTransformOrthonormalize();
            JobHandle orthonormalize_job = orthonormalize.Schedule<PlanetariaTransformOrthonormalize>(this, move_job); // this and prior step are parallel
            return JobHandle.CombineDependencies(clean_job, orthonormalize_job);
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