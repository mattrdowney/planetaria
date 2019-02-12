using System;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Planetaria
{
    [DisallowMultipleComponent]
    [Serializable]
    public abstract class PlanetariaRenderer : MonoBehaviour // FIXME: There is a bug similar to PlanetariaCollider with layers. If the parent object changes layers, the generated internal child object won't match layers, creating renderer layer bugs (I don't use layers often with rendering, but it is a feature). This one cannot be fixed in the same way, so it's harder.
    {
#if UNITY_EDITOR
        private void Awake()
        {
            setup();
        }
#endif

        private void OnDestroy()
        {
            cleanup();
        }

        private void Reset()
        {
            cleanup();
            setup();
        }

        private void cleanup()
        {
            if (spawned_object)
            {
                if (Application.isPlaying)
                {
                    World.Active.GetOrCreateManager<EntityManager>().DestroyEntity(game_object_entity.Entity);
                    Destroy(spawned_object);
                }
                else
                {
                    DestroyImmediate(spawned_object);
                }
            }
        }

        private void setup()
        {
            if (spawned_object == null) // FIXME: The problem with this method is Unity always breaks references if the code cannot compile, which means objects cannot be destroyed later
            {
                GameObject child = this.GetOrAddChild("Renderer");
                internal_transform = child.GetComponent<Transform>();
            }
        }

        public float angle
        {
            get
            {
                return ((Quaternion)game_object_entity.get_component_data<Rotation, RotationComponent>().Value).eulerAngles.z;
            }
            set
            {
                game_object_entity.set_component_data<Rotation, RotationComponent>(new Rotation { Value = quaternion.EulerXYZ(0, 0, angle * Mathf.Rad2Deg) });
            }
        }

        public bool flip_horizontal
        {
            get
            {
                return game_object_entity.get_component_data<PlanetariaRendererScale, PlanetariaRendererScaleComponent>().flip_horizontal == 1;
            }
            set
            {
                game_object_entity.set_component_data<PlanetariaRendererScale, PlanetariaRendererScaleComponent>(new PlanetariaRendererScale
                {
                    scale = game_object_entity.get_component_data<PlanetariaRendererScale, PlanetariaRendererScaleComponent>().scale,
                    flip_horizontal = value ? (byte)1 : (byte)0,
                    flip_vertical = game_object_entity.get_component_data<PlanetariaRendererScale, PlanetariaRendererScaleComponent>().flip_vertical,
                });
            }
        }

        public bool flip_vertical
        {
            get
            {
                return game_object_entity.get_component_data<PlanetariaRendererScale, PlanetariaRendererScaleComponent>().flip_vertical == 1;
            }
            set
            {
                game_object_entity.set_component_data<PlanetariaRendererScale, PlanetariaRendererScaleComponent>(new PlanetariaRendererScale
                {
                    scale = game_object_entity.get_component_data<PlanetariaRendererScale, PlanetariaRendererScaleComponent>().scale,
                    flip_horizontal = game_object_entity.get_component_data<PlanetariaRendererScale, PlanetariaRendererScaleComponent>().flip_horizontal,
                    flip_vertical = value ? (byte)1 : (byte)0,
                });
            }
        }

        public float offset // CONSIDER: FIXME: why make an PlanetariaRendererOffsetComponent if I'm not gonna use it?
        {
            get
            {
                return game_object_entity.get_component_data<Position, PositionComponent>().Value.z;
            }
            set
            {
                game_object_entity.set_component_data<Position, PositionComponent>(new Position { Value = new float3(0, 0, value) });
            }
        }

        public float2 scale
        {
            get
            {
                return game_object_entity.get_component_data<PlanetariaRendererScale, PlanetariaRendererScaleComponent>().scale;
            }
            set
            {
                game_object_entity.set_component_data<PlanetariaRendererScale, PlanetariaRendererScaleComponent>(new PlanetariaRendererScale
                {
                    scale = value,
                    flip_horizontal = game_object_entity.get_component_data<PlanetariaRendererScale, PlanetariaRendererScaleComponent>().flip_horizontal,
                    flip_vertical = game_object_entity.get_component_data<PlanetariaRendererScale, PlanetariaRendererScaleComponent>().flip_vertical,
                });
            }
        }
        
        [SerializeField] GameObject spawned_object;
        [SerializeField] GameObjectEntity game_object_entity;
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