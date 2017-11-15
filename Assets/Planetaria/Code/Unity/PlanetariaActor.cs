using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlanetariaMonoBehaviour))]
public abstract class PlanetariaActor : PlanetariaMonoBehaviour
{
    protected sealed override void Awake()
    {
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

    private IEnumerator post_fixed_update()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            // FIXME: go through each BlockCollision and cull redundant / irrelevant collisions
            transform.move();
            List<Block>[] block_set = new List<Block>[2];
            foreach (BlockCollision block_collision in last_collision_set)
            {
                block_set[0].Add(block_collision.block);
            }
            foreach (BlockCollision block_collision in collision_set)
            {
                block_set[1].Add(block_collision.block);
            }
            List<Block
            last_collision_set = collision_set;
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
            float half_height = transform.scale / 2;
            optional<BlockCollision> collision = BlockCollision.block_collision(arc, block, transform.previous_position.data, transform.position.data, half_height);
            if (collision.exists)
            {
                collision_set.Add(collision.data);
            }
        }
    }

    private void field_enter(Field field, NormalizedCartesianCoordinates position)
    {
        if (!trigger_set.Contains(field) && field.contains(position.data, transform.scale))
        {
            trigger_set.Add(field);
            on_field_enter(field);
        }
    }

    private void field_stay(Field field)
    {
        if (trigger_set.Contains(field))
        {
            on_field_stay(field);
        }
    }

    private void field_exit(Field field, NormalizedCartesianCoordinates position)
    {
        if (trigger_set.Contains(field) && !field.contains(position.data, transform.scale))
        {
            trigger_set.Remove(field);
            on_field_exit(field);
        }
    }

    protected sealed override void Update() { }
    protected sealed override void LateUpdate() { }

    protected sealed override void OnTriggerEnter(Collider collider) { }
    protected sealed override void OnTriggerExit(Collider collider) { } // To account for object deletion, this (or other code) must be defined.

    protected sealed override void OnCollisionEnter(Collision collision) { }
    protected sealed override void OnCollisionStay(Collision collision) { }
    protected sealed override void OnCollisionExit(Collision collision) { }

    private List<BlockCollision> last_collision_set = new List<BlockCollision>();
    private List<BlockCollision> collision_set = new List<BlockCollision>();
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