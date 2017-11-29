using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Planetaria
{
    // Definitely needs the observer pattern for all attached PlanetariaMonoBehaviours
    public class PlanetariaCollider : MonoBehaviour
    {
        private /*JANK super-temp*/ PlanetariaActor SUPER_TEMP; // FIXME:

        private void Awake()
        {
            GameObject child_for_collision = new GameObject("PlanetariaCollider");
            child_for_collision.transform.localPosition = Vector3.forward;
            child_for_collision.transform.parent = this.gameObject.transform;
            internal_collider = child_for_collision.transform.GetOrAddComponent<SphereCollider>();
            SUPER_TEMP = this.GetComponent<PlanetariaCharacter>();
            transform = this.GetOrAddComponent<PlanetariaTransform>();
            // add to collision_map and trigger_map for all objects currently intersecting (via Physics.OverlapBox()) // CONSIDER: I think Unity Fixed this, right?
            StartCoroutine(post_fixed_update());
        }

        private IEnumerator post_fixed_update()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();
                optional<BlockCollision> next_collision = collision_candidates.Min((collision) => collision);
                if (next_collision.exists)
                {
                    if (current_collision.exists)
                    {
                        if (SUPER_TEMP.on_block_exit.exists)
                        {
                            SUPER_TEMP.on_block_exit.data(current_collision.data);
                        }
                    }
                    if (SUPER_TEMP.on_block_enter.exists)
                    {
                        SUPER_TEMP.on_block_enter.data(next_collision.data);
                    }
                    current_collision = next_collision;
                }
                // I feel like this doesn't account for a forced on_block_exit inside on_block_enter call
                if (current_collision.exists)
                {
                    if (SUPER_TEMP.on_block_stay.exists)
                    {
                        SUPER_TEMP.on_block_stay.data(current_collision.data);
                    }
                }
                transform.move();
            }
        }

        private void OnTriggerStay(Collider collider)
        {
            optional<BoxCollider> box_collider = collider as BoxCollider;
            if (!box_collider.exists)
            {
                return;
            }
            NormalizedCartesianCoordinates position = transform.position;
            optional<Arc> arc = PlanetariaCache.arc_cache.get(box_collider.data); // C++17 if statements are so pretty compared to this...
            if (arc.exists)
            {
                optional<Block> block = PlanetariaCache.block_cache.get(box_collider.data);
                if (!block.exists)
                {
                    Debug.LogError("Critical Err0r.");
                    return;
                }
                collision_enter(arc.data, block.data, position);
                // collision_exit(arc.data, block.data); // collisions are handled at stage: post_fixed_update()
                // collision_stay(arc.data, block.data); // enter vs. exit behaviour handled by last_collision_set vs. collision_set continuity
            }
            else // field
            {
                optional<Field> field = PlanetariaCache.field_cache.get(box_collider.data);
                if (!field.exists)
                {
                    Debug.LogError("This is likely an Err0r or setup issue.");
                    return;
                }
                field_enter(field.data, position);
                field_exit(field.data, position);
                field_stay(field.data);
            }
        }

        private void collision_enter(Arc arc, Block block, NormalizedCartesianCoordinates position)
        {
            if (!current_collision.exists || current_collision.data.block != block)
            {
                if (block.active && arc.contains(position.data, transform.scale/2))
                {
                    float half_height = transform.scale / 2;
                    optional<BlockCollision> collision = BlockCollision.block_collision(arc, block, transform.previous_position.data, transform.position.data, half_height);
                    if (collision.exists)
                    {
                        collision_candidates.Add(collision.data);
                    }
                }
            }
        }

        private void field_enter(Field field, NormalizedCartesianCoordinates position)
        {
            if (!trigger_set.Contains(field) && field.contains(position.data, transform.scale))
            {
                trigger_set.Add(field);
                if (SUPER_TEMP.on_field_enter.exists)
                {
                    SUPER_TEMP.on_field_enter.data(field);
                }
            }
        }

        private void field_stay(Field field)
        {
            if (trigger_set.Contains(field))
            {
                if (SUPER_TEMP.on_field_stay.exists)
                {
                    SUPER_TEMP.on_field_stay.data(field);
                }
            }
        }

        private void field_exit(Field field, NormalizedCartesianCoordinates position)
        {
            if (trigger_set.Contains(field) && !field.contains(position.data, transform.scale))
            {
                trigger_set.Remove(field);
                if (SUPER_TEMP.on_field_exit.exists)
                {
                    SUPER_TEMP.on_field_exit.data(field);
                }
            }
        }

        public float scale
        {
            get
            {
                return scale_variable;
            }
            set
            {
                scale_variable = value;
                internal_collider.radius = scale_variable;
            }
        }

        private new PlanetariaTransform transform;
            
        public optional<BlockCollision> current_collision = new optional<BlockCollision>(); // FIXME: JANK
        private List<BlockCollision> collision_candidates = new List<BlockCollision>();
        private List<Field> trigger_set = new List<Field>();
        private SphereCollider internal_collider; // FIXME: collider list
        float scale_variable;
    }
}

/*
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/