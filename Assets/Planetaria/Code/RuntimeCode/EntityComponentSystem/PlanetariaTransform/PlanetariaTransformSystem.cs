using UnityEngine;
using Unity.Entities;

namespace Planetaria
{
    [UpdateBefore(typeof(Transform))] // TODO: verify this compiles as intended
    [UpdateAfter(typeof(PlanetariaTransform))]
    public class PlanetariaTransformSystem : ComponentSystem
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