using System;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

namespace Planetaria
{
    [Serializable]
    public class PlanetariaRenderer : MonoBehaviour // FIXME: There is a bug similar to PlanetariaCollider with layers. If the parent object changes layers, the generated internal child object won't match layers, creating renderer layer bugs (I don't use layers often with rendering, but it is a feature). This one cannot be fixed in the same way, so it's harder.
    {
        private void Awake()
        {
            setup();
#if UNITY_EDITOR
            spawned_object.hideFlags = HideFlags.None;
#endif
        }

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
                if (World.Active != null && Application.isPlaying)
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
                spawned_object = this.add_child("Renderer");
                game_object_entity = spawned_object.AddComponent<GameObjectEntity>();
                game_object_entity.add_component_data<PositionComponent>();
                game_object_entity.add_component_data<RotationComponent>();
                game_object_entity.add_component_data<ScaleComponent>();
                game_object_entity.add_component_data<PlanetariaRendererTagComponent>();
                game_object_entity.add_component_data<RenderMeshComponent>();
                RenderMesh initialized_renderer = new RenderMesh();
                initialized_renderer.castShadows = UnityEngine.Rendering.ShadowCastingMode.Off; // Intentionally redundant
                initialized_renderer.receiveShadows = false; // Intentionally redundant
                initialized_renderer.layer = 0; // Intentionally redundant
                initialized_renderer.subMesh = 0; // Intentionally redundant
                initialized_renderer.mesh = GameObject.CreatePrimitive(PrimitiveType.Quad).GetComponent<MeshFilter>().sharedMesh;
                initialized_renderer.material = new Material(Shader.Find("Standard"));
                game_object_entity.set_shared_component_data<RenderMesh, RenderMeshComponent>(initialized_renderer);
                game_object_entity.add_component_data<AttachComponent>(); // TODO: verify attach is working (thanks to this tutorial: https://www.youtube.com/watch?v=cUrHcEA8azw )
                game_object_entity.set_component_data<Attach, AttachComponent>(new Attach { Parent = this.gameObject.GetComponent<GameObjectEntity>().Entity, Child = game_object_entity.Entity });
                angle = 0;
                offset = 1;
                scale = new float2(1,1);
            }
        }

        public float angle
        {
            get
            {
                return ((Quaternion)game_object_entity.get_component_data<Rotation, RotationComponent>().Value).eulerAngles.z * Mathf.Deg2Rad;
            }
            set
            {
                float3 intermediate_result = ((Quaternion)game_object_entity.get_component_data<Rotation, RotationComponent>().Value).eulerAngles;
                intermediate_result.z = value * Mathf.Rad2Deg;
                game_object_entity.set_component_data<Rotation, RotationComponent>(new Rotation { Value = quaternion.EulerXYZ(0, 0, value * Mathf.Rad2Deg) });
            }
        }

        public bool flip_horizontal
        {
            get
            {
                return game_object_entity.get_component_data<Scale, ScaleComponent>().Value.x < 0;
            }
            set
            {
                float3 intermediate_result = game_object_entity.get_component_data<Scale, ScaleComponent>().Value;
                intermediate_result.x = Mathf.Abs(intermediate_result.x) * (value ? -1 : +1);
                game_object_entity.set_component_data<Scale, ScaleComponent>(new Scale { Value = intermediate_result });
            }
        }

        public bool flip_vertical
        {
            get
            {
                return game_object_entity.get_component_data<Scale, ScaleComponent>().Value.y < 0;
            }
            set
            {
                float3 intermediate_result = game_object_entity.get_component_data<Scale, ScaleComponent>().Value;
                intermediate_result.y = Mathf.Abs(intermediate_result.y) * (value ? -1 : +1);
                game_object_entity.set_component_data<Scale, ScaleComponent>(new Scale { Value = intermediate_result });
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
                float3 intermediate_result = game_object_entity.get_component_data<Position, PositionComponent>().Value;
                intermediate_result.z = value; // This is for consistency. (I don't know if anyone would ever have a use for this, though.)
                game_object_entity.set_component_data<Position, PositionComponent>(new Position { Value = intermediate_result });
            }
        }

        public float2 scale
        {
            get
            {
                float3 intermediate_result = game_object_entity.get_component_data<Scale, ScaleComponent>().Value;
                return new float2(intermediate_result.x, intermediate_result.y);
            }
            set
            {
                float3 intermediate_result = game_object_entity.get_component_data<Scale, ScaleComponent>().Value;
                intermediate_result.x = value.x; // This is for consistency. I want the interface to ignore the z value (which means I have to get it so I don't overwrite it).
                intermediate_result.y = value.y; // While this may seem useless, there are many renderers which have thickness (scale.z = 1) and others that don't (scale.z = 0)
                game_object_entity.set_component_data<Scale, ScaleComponent>(new Scale { Value = intermediate_result });
            }
        }
        
        [SerializeField] [HideInInspector] GameObject spawned_object;
        [SerializeField] [HideInInspector] GameObjectEntity game_object_entity;
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