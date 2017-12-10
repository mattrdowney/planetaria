using System.Collections.Generic;
using UnityEngine;

public class CollisionObserver
{
    public CollisionObserver(PlanetariaMonoBehaviour[] observers)
    {
        this.observers = observers;
        StartCoroutine(notify());
    }

    private IEnumerator notify()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            field_notifications();
            block_notifications();
        }
    }

    private void block_notifications()
    {
        optional<BlockCollision> next_collision = new optional<BlockCollision>();

        if (collision_candidates.Count > 0)
        {
            next_collision = collision_candidates.Aggregate(
                    (closest, next_candidate) =>
                    closest.distance < next_candidate.distance ? closest : next_candidate);
        }

        if (next_collision.exists)
        {
            block_notification(next_collision);
        }
    }

    private void field_notifications()
    {
        foreach (PlanetariaMonoBehaviour listener in listeners)
        {
            foreach (PlanetariaCollider field in fields_entered)
            {
                listener.enter_field(field);
            }
        }
        fields_entered.Clear();
        foreach (PlanetariaMonoBehaviour listener in listeners)
        {
            foreach (PlanetariaCollider field in fields_exited)
            {
                listener.exit_field(field);
            }
        }
        fields_exited.Clear();
    }

    public void register(PlanetariaMonoBehaviour observer)
    {
        observers.Add(observer);
        foreach (PlanetariaCollider field in field_set)
        {
            observer.enter_field(field);
        }
        foreach (BlockCollision collision in current_collisions)
        {
            observer.enter_block(collision);
        }
    }

    public void unregister(PlanetariaMonoBehaviour observer)
    {
        observers.Remove(observer);
        foreach (PlanetariaCollider field in field_set)
        {
            observer.exit_field(field);
        }
        foreach (BlockCollision collision in current_collisions)
        {
            observer.exit_block(collision);
        }
    }

    public void enter_field(PlanetariaCollider field)
    {
        if (!field_set.Contains(other_collider.data)) // field triggering is handled in notify() stage
        {
            field_set.Add(other_collider.data);
            fields_entered.Add(other_collider.data);
        }
    }

    public void exit_field(PlanetariaCollider field)
    {
        if (field_set.Contains(other_collider.data))
        {
            field_set.Remove(other_collider.data);
            fields_exited.Add(other_collider.data);
        }
    }

    private List<PlanetariaMonoBehaviour> observers = new List<PlanetariaMonoBehaviour>();
    private List<BlockCollision> current_collisions = new List<BlockCollision>();
    private List<BlockCollision> collision_candidates = new List<BlockCollision>();
    private List<PlanetariaCollider> field_set = new List<PlanetariaCollider>();
    private List<PlanetariaCollider> fields_entered = new List<PlanetariaCollider>();
    private List<PlanetariaCollider> fields_exited = new List<PlanetariaCollider>();
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