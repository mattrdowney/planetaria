using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PlanetariaMonoBehaviour))]
public abstract class PlanetariaActor : PlanetariaMonoBehaviour
{
    protected sealed override void Awake()
    {
        set_delegates();
        transform = new PlanetariaTransform(this.GetComponent<Transform>());
        if (on_first_exists.exists)
        {
            on_first_exists.data();
        }
        StartCoroutine(post_fixed_update());
    }

    protected sealed override void Start()
    {
        // add to collision_map and trigger_map for all objects currently intersecting (via Physics.OverlapBox())
        if (on_time_zero.exists)
        {
            on_time_zero.data();
        }
    }
    
    protected sealed override void FixedUpdate() // always calling FixedUpdate is less than ideal
    {
        if (on_every_frame.exists)
        {
            on_every_frame.data(); // if undefined, this will error out
        }
    }

    protected abstract void set_delegates();

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
                    if (on_block_exit.exists)
                    {
                        on_block_exit.data(current_collision.data);
                    }
                }
                if (on_block_enter.exists)
                {
                    on_block_enter.data(next_collision.data);
                }
                current_collision = next_collision;
            }
            // I feel like this doesn't account for a forced on_block_exit inside on_block_enter call
            if (current_collision.exists)
            {
                if (on_block_stay.exists)
                {
                    on_block_stay.data(current_collision.data);
                }
            }
            transform.move();
        }
    }

    protected sealed override void OnTriggerStay(Collider collider)
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
        if (block.active && arc.contains(position.data, transform.scale))
        {
            if (!current_collision.exists || current_collision.data.block != block)
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
            if (on_field_enter.exists)
            {
                on_field_enter.data(field);
            }
        }
    }

    private void field_stay(Field field)
    {
        if (trigger_set.Contains(field))
        {
            if (on_field_stay.exists)
            {
                on_field_stay.data(field);
            }
        }
    }

    private void field_exit(Field field, NormalizedCartesianCoordinates position)
    {
        if (trigger_set.Contains(field) && !field.contains(position.data, transform.scale))
        {
            trigger_set.Remove(field);
            if (on_field_exit.exists)
            {
                on_field_exit.data(field);
            }
        }
    }

    protected sealed override void Update() { }
    protected sealed override void LateUpdate() { }

    protected sealed override void OnTriggerEnter(Collider collider) { }
    protected sealed override void OnTriggerExit(Collider collider) { } // To account for object deletion, this (or other code) must be defined.

    protected sealed override void OnCollisionEnter(Collision collision) { }
    protected sealed override void OnCollisionStay(Collision collision) { }
    protected sealed override void OnCollisionExit(Collision collision) { }
    
    protected optional<BlockCollision> current_collision = new optional<BlockCollision>();
    private List<BlockCollision> collision_candidates = new List<BlockCollision>();
    private List<Field> trigger_set = new List<Field>();
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