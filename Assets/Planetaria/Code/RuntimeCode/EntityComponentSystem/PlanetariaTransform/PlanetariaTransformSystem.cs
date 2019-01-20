using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Planetaria
{
    [UpdateAfter(typeof(PlanetariaTransform))]
    public class PlanetariaTransformSystem : JobComponentSystem
    {
        // order of operations are usually scale, then rotate, then transform
        [BurstCompile]
        struct PlanetariaScale : IJobProcessComponentData<PlanetariaScaleComponent, PlanetariaScaleDirtyComponent>
        {
            public void Execute([ReadOnly] ref PlanetariaScaleComponent scale, ref PlanetariaScaleDirtyComponent dirty)
            {
                // TODO: scale
            }
        }

        [BurstCompile]
        [RequireComponentTag(typeof(PlanetariaPositionDirtyComponent))]
        [RequireSubtractiveComponent(typeof(PlanetariaDirectionDirtyComponent))]
        struct PlanetariaTransformMoveOnly : IJobProcessComponentData<Rotation, PlanetariaPreviousPositionComponent, PlanetariaDirectionComponent, PlanetariaPositionComponent>
        {
            public void Execute(ref Rotation rotation,
                    ref PlanetariaPreviousPositionComponent previous_position,
                    ref PlanetariaDirectionComponent direction,
                    [ReadOnly] ref PlanetariaPositionComponent position)
            {
                 // TODO: OPTIMIZE: and then optimize again
                float3 last_velocity = -Vector3.ProjectOnPlane(previous_position.data, position.data);
                float3 velocity = Vector3.ProjectOnPlane(position.data, previous_position.data);
                quaternion last_rotation = quaternion.LookRotationSafe(previous_position.data, last_velocity);
                quaternion current_rotation = quaternion.LookRotationSafe(position.data, velocity);
                float3 relative_direction = Quaternion.Inverse(last_rotation) * direction.data;
                float3 next_direction = (Quaternion)current_rotation * relative_direction;
                direction = new PlanetariaDirectionComponent { data = next_direction };
                previous_position = new PlanetariaPreviousPositionComponent { data = (Quaternion)rotation.Value * Vector3.forward };
                rotation = new Rotation { Value = quaternion.LookRotationSafe(position.data, direction.data) };
            }
        }

        [BurstCompile]
        [RequireComponentTag(typeof(PlanetariaPositionDirtyComponent), typeof(PlanetariaDirectionDirtyComponent))]
        struct PlanetariaTransformRotateAndMove : IJobProcessComponentData<Rotation, PlanetariaPreviousPositionComponent, PlanetariaDirectionComponent, PlanetariaPositionComponent>
        {
            public void Execute(ref Rotation rotation,
                    ref PlanetariaPreviousPositionComponent previous_position,
                    [ReadOnly] ref PlanetariaDirectionComponent direction,
                    [ReadOnly] ref PlanetariaPositionComponent position)
            {
                previous_position = new PlanetariaPreviousPositionComponent { data = (Quaternion)rotation.Value * Vector3.forward };
                rotation = new Rotation { Value = quaternion.LookRotationSafe(position.data, direction.data) };
            }
        }

        protected override JobHandle OnUpdate(JobHandle input_dependencies)
        {
            var complex_case = new PlanetariaTransformMoveOnly();
            var simple_case = new PlanetariaTransformRotateAndMove();
            var handle = complex_case.Schedule<PlanetariaTransformMoveOnly>(this, input_dependencies); // NOTE: these should be scheduled simultaneously
            handle = simple_case.Schedule<PlanetariaTransformRotateAndMove>(this, input_dependencies); // TODO: verify
            // TODO: implement scale here
            return handle;
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