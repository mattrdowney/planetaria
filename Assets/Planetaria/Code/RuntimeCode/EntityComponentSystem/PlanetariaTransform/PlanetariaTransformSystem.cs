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
        struct PlanetariaScale : IJobProcessComponentData<PlanetariaScaleComponent, PlanetariaPreviousScaleComponent>
        {
            public void Execute(ref PlanetariaScaleComponent scale, [ReadOnly] ref PlanetariaPreviousScaleComponent previous_scale)
            {
                if (scale.scale != previous_scale.previous_scale)
                {
                    // TODO: scale
                }
            }
        }

        [BurstCompile]
        struct PlanetariaRotate : IJobProcessComponentData<PlanetariaPositionComponent, PlanetariaDirectionComponent, PlanetariaPreviousDirectionComponent>
        {
            public void Execute(ref PlanetariaPositionComponent position, ref PlanetariaDirectionComponent direction, [ReadOnly] ref PlanetariaPreviousDirectionComponent previous_direction)
            {
                if (direction.direction != previous_direction.previous_direction)
                {
                    var rotat quaternion.LookRotationSafe(heading.Value, math.up());
                    Vector3 last_velocity = -Vector3.ProjectOnPlane(current_position, next_position);
                    Vector3 velocity = Vector3.ProjectOnPlane(next_position, current_position);
                    Quaternion last_rotation = Quaternion.LookRotation(current_position, last_velocity); // TODO: optimize
                    Quaternion rotation = Quaternion.LookRotation(next_position, velocity);
                    Vector3 old_direction = component.transform.up;
                    Vector3 relative_direction = Quaternion.Inverse(last_rotation) * old_direction;
                    Vector3 next_direction = rotation * relative_direction;
                    component.transform.localRotation = Quaternion.LookRotation(next_position, next_direction);
                    var rotationFromHeading = quaternion.LookRotationSafe(heading.Value, math.up());
                    rotation = new Rotation { Value = rotationFromHeading };
                }
            }
        }

        [BurstCompile]
        struct PlanetariaMove : IJobProcessComponentData<PlanetariaPositionComponent, PlanetariaDirectionComponent, PlanetariaPreviousPositionComponent>
        {
            public void Execute(ref PlanetariaPositionComponent position, ref PlanetariaDirectionComponent direction, [ReadOnly] ref PlanetariaPreviousPositionComponent previous_position)
            {
                if (position.position != previous_position.previous_position)
                {
                    Vector3 last_velocity = -Vector3.ProjectOnPlane(current_position, next_position);
                    Vector3 velocity = Vector3.ProjectOnPlane(next_position, current_position);
                    Quaternion last_rotation = Quaternion.LookRotation(current_position, last_velocity); // TODO: optimize
                    Quaternion rotation = Quaternion.LookRotation(next_position, velocity);
                    Vector3 old_direction = component.transform.up;
                    Vector3 relative_direction = Quaternion.Inverse(last_rotation) * old_direction;
                    Vector3 next_direction = rotation * relative_direction;
                    component.transform.localRotation = Quaternion.LookRotation(next_position, next_direction);
                    var rotationFromHeading = quaternion.LookRotationSafe(heading.Value, math.up());
                    rotation = new Rotation { Value = rotationFromHeading };
                }
            }
        }

        protected override JobHandle OnUpdate(JobHandle input_dependencies)
        {
            var rotationFromHeadingJob = new PlanetariaRotate();
            var rotationFromHeadingJobHandle = rotationFromHeadingJob.Schedule(this, input_dependencies);

            return rotationFromHeadingJobHandle;
        }

        protected override void OnUpdate()
        {
            foreach (PlanetariaTransformComponent component in GetEntities<PlanetariaPositionComponent>())
            {
                /*if (component.position.position_dirty || component.position.direction_dirty)
                {
                    Vector3 current_position = component.transform.forward;
                    Vector3 next_position = component.position.position;
                    if (current_position != next_position && !component.position.direction_dirty)
                    {
                        Vector3 last_velocity = -Vector3.ProjectOnPlane(current_position, next_position);
                        Vector3 velocity = Vector3.ProjectOnPlane(next_position, current_position);
                        Quaternion last_rotation = Quaternion.LookRotation(current_position, last_velocity); // TODO: optimize
                        Quaternion rotation = Quaternion.LookRotation(next_position, velocity);
                        Vector3 old_direction = component.transform.up;
                        Vector3 relative_direction = Quaternion.Inverse(last_rotation) * old_direction;
                        Vector3 next_direction = rotation * relative_direction;
                        component.transform.localRotation = Quaternion.LookRotation(next_position, next_direction);
                    }
                    else
                    {
                        Vector3 next_direction = component.position.direction;
                        component.transform.localRotation = Quaternion.LookRotation(next_position, next_direction);
                    }
                    component.position.position_dirty = false;
                    component.position.direction_dirty = false;
                    component.transform.position = Vector3.zero;
                }
                // TODO: scale*/
            }
        }
    }

    [UpdateAfter(typeof(PlanetariaTransformRotationSystem))]
    public class PlanetariaTransformRotationSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            foreach (PlanetariaTransformComponent component in GetEntities<PlanetariaTransformComponent>())
            {
                /*if (component.position.position_dirty || component.position.direction_dirty)
                {
                    Vector3 current_position = component.transform.forward;
                    Vector3 next_position = component.position.position;
                    if (current_position != next_position && !component.position.direction_dirty)
                    {
                        Vector3 last_velocity = -Vector3.ProjectOnPlane(current_position, next_position);
                        Vector3 velocity = Vector3.ProjectOnPlane(next_position, current_position);
                        Quaternion last_rotation = Quaternion.LookRotation(current_position, last_velocity); // TODO: optimize
                        Quaternion rotation = Quaternion.LookRotation(next_position, velocity);
                        Vector3 old_direction = component.transform.up;
                        Vector3 relative_direction = Quaternion.Inverse(last_rotation) * old_direction;
                        Vector3 next_direction = rotation * relative_direction;
                        component.transform.localRotation = Quaternion.LookRotation(next_position, next_direction);
                    }
                    else
                    {
                        Vector3 next_direction = component.position.direction;
                        component.transform.localRotation = Quaternion.LookRotation(next_position, next_direction);
                    }
                    component.position.position_dirty = false;
                    component.position.direction_dirty = false;
                    component.transform.position = Vector3.zero;
                }
                // TODO: scale*/
            }
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