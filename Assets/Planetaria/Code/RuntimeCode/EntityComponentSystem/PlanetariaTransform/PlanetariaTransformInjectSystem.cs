using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Experimental.PlayerLoop;

namespace Planetaria
{
    public class PlanetariaTransformInjectSystem : ComponentSystem
    {
        struct PlanetariaTransformGroup
        {
            [ReadOnly]
            public ComponentArray<PlanetariaTransform> Component; // Get all MonoBehaviour components to synchronize them
            public EntityArray Entity;
            public int Length;
        }
 
        [Inject] PlanetariaTransformGroup group;
        protected override void OnUpdate()
        {
            for (int transform = 0; transform < group.Length; ++transform)
            {
                if (group.Component[transform].dirty)
                {
                    if (group.Component[transform].position_dirty)
                    {
                        EntityManager.SetComponentData<PlanetariaPosition>(group.Entity[transform],
                                new PlanetariaPosition { data = group.Component[transform].position });
                    }
                    if (group.Component[transform].direction_dirty)
                    {
                        EntityManager.SetComponentData<PlanetariaDirection>(group.Entity[transform],
                                new PlanetariaDirection { data = group.Component[transform].direction });
                        EntityManager.SetComponentData<PlanetariaDirectionDirty>(group.Entity[transform],
                                new PlanetariaDirectionDirty { data = 1 });
                    }
                    if (group.Component[transform].scale_dirty)
                    {
                        EntityManager.SetComponentData<PlanetariaScale>(group.Entity[transform],
                                new PlanetariaScale { data = group.Component[transform].scale });
                    }
                    group.Component[transform].clean();
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