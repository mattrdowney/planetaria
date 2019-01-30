using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Experimental.PlayerLoop;

namespace Planetaria
{
    public class PlanetariaRigidbodyInjectSystem : ComponentSystem
    {
        struct PlanetariaRigidbodyGroup
        {
            [ReadOnly]
            public ComponentArray<PlanetariaRigidbody> Component; // Get all MonoBehaviour components to synchronize them
            public EntityArray Entity;
            public int Length;
        }
 
        [Inject] PlanetariaRigidbodyGroup group;
        protected override void OnUpdate()
        {
            for (int rigidbody = 0; rigidbody < group.Length; ++rigidbody)
            {
                if (group.Component[rigidbody].dirty)
                {
                    if (group.Component[rigidbody].velocity_dirty)
                    {
                        if (EntityManager.HasComponent<PlanetariaRigidbodyUnrestrained>(group.Entity[rigidbody])) // Not traversing an Arc
                        {
                            float3 position = EntityManager.GetComponentData<PlanetariaPosition>(group.Entity[rigidbody]).data;
                            if (group.Component[rigidbody].velocity_type == PlanetariaRigidbody.VelocityMode.Relative) // Set relative velocity
                            {
                                float3 up = EntityManager.GetComponentData<PlanetariaDirection>(group.Entity[rigidbody]).data;
                                float3 right = math.cross(up, position);
                                float3 x = right * group.Component[rigidbody].relative_velocity.x;
                                float3 y = up * group.Component[rigidbody].relative_velocity.y;
                                EntityManager.SetComponentData<PlanetariaVelocity>(group.Entity[rigidbody],
                                        new PlanetariaVelocity { data = x + y });
                            }
                            else // Set absolute velocity
                            {
                                float3 x = Bearing.east(position) * group.Component[rigidbody].absolute_velocity.x;
                                float3 y = Bearing.north(position) * group.Component[rigidbody].absolute_velocity.y;
                                EntityManager.SetComponentData<PlanetariaVelocity>(group.Entity[rigidbody],
                                        new PlanetariaVelocity { data = x + y });
                            }
                        }
                        else // Attached to an Arc
                        {
                            if (group.Component[rigidbody].velocity_type == PlanetariaRigidbody.VelocityMode.Relative) // Set relative velocity
                            {
                                Vector3 velocity = group.Component[rigidbody].relative_velocity;
                                EntityManager.SetComponentData<PlanetariaVelocity>(group.Entity[rigidbody],
                                        new PlanetariaVelocity { data = velocity });
                            }
                            else // Set absolute velocity
                            {
                                float3 position = EntityManager.GetComponentData<PlanetariaPosition>(group.Entity[rigidbody]).data;
                                float3 x = Bearing.east(position) * group.Component[rigidbody].absolute_velocity.x;
                                float3 y = Bearing.north(position) * group.Component[rigidbody].absolute_velocity.y;
                                EntityManager.SetComponentData<PlanetariaVelocity>(group.Entity[rigidbody],
                                        new PlanetariaVelocity { data = x + y });
                            }
                        }
                    }
                    if (group.Component[rigidbody].gravity_dirty)
                    {
                        EntityManager.SetComponentData<PlanetariaGravity>(group.Entity[rigidbody],
                                        new PlanetariaGravity { data = group.Component[rigidbody].gravity });
                        if (EntityManager.HasComponent<PlanetariaAcceleration>(group.Entity[rigidbody])) // Accelerating
                        {
                            if (group.Component[rigidbody].gravity == Vector3.zero) // but you just told the system not to accelerate
                            {
                                PostUpdateCommands.RemoveComponent<PlanetariaAcceleration>(group.Entity[rigidbody]);
                            }
                        }
                        else // Not accelerating
                        {
                            if (group.Component[rigidbody].gravity != Vector3.zero) // but you just told the system to accelerate
                            {
                                PostUpdateCommands.AddComponent<PlanetariaAcceleration>(group.Entity[rigidbody], new PlanetariaAcceleration());
                            }
                        }
                    }
                    group.Component[rigidbody].clean();
                }
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