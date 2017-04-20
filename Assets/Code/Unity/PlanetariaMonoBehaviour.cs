using System.Collections.Generic;
using UnityEngine;

public sealed class PlanetariaMonoBehaviour : MonoBehaviour
{
    PlanetariaActor actor;

    Dictionary<BoxCollider, BlockInteractor> collision_map;
    HashSet<BoxCollider> 

	public void Start()
    {
        collision_map = new Dictionary<BoxCollider, BlockInteractor>();
        // add to collision_map for all objects currently intersecting (via Physics.OverlapBox())
		actor.Start();
	}
	
	public void FixedUpdate()
    {
		actor.Update();
	}

    public void OnTriggerStay(Collider collider)
    {
        BoxCollider box_collider = collider as BoxCollider;
        if (!box_collider)
        {
            return;
        }
        optional<Arc> arc = PlanetariaCache.arc_cache.Get(box_collider);
        optional<Zone> zone = PlanetariaCache.zone_cache.Get(box_collider);
        if (arc.exists) // block
        {
            optional<Block> block = PlanetariaCache.block_cache.Get(arc.data);
            if (!block.exists)
            {
                Debug.LogError("Critical Err0r.");
                return;
            }

            if (!collision_map.ContainsValue()arc.data.contains(actor.transform.position))
            {
                BlockInteractor collision = new BlockInteractor(block.data, block_index, interpolator_angle);
                collision_map.Add(collision);
                actor.OnBlockEnter(collision);
            }
            else if (just_stopped_colliding)
            {
                actor.OnBlockExit(collision);
            }

            if (in_map)
            {
                actor.OnBlockStay(collision);
            }
        }
        else // zone
        {
            // FIXME: might not be a trigger either
            ZoneInteractor trigger = new ZoneInteractor(PlanetariaCache.GetZone(box_collider).data);

            if (just_started_colliding)
            {
                actor.OnZoneEnter(trigger);
            }
            else if (just_stopped_colliding)
            {
                actor.OnZoneExit(trigger);
            }

            if (in_map)
            {
                actor.OnZoneStay(trigger);
            }
        }
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