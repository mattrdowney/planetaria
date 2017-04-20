using System.Collections.Generic;
using UnityEngine;

public sealed class PlanetariaMonoBehaviour : MonoBehaviour
{
    PlanetariaActor actor;

    Dictionary<Block, BlockInteractor> collision_map = new Dictionary<Block, BlockInteractor>();
    Dictionary<Zone, ZoneInteractor> trigger_map = new Dictionary<Zone, ZoneInteractor>();

	public void Start()
    {
        collision_map = new Dictionary<Block, BlockInteractor>();
        trigger_map = new Dictionary<Zone, ZoneInteractor>();
        // add to collision_map and trigger_map for all objects currently intersecting (via Physics.OverlapBox())
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

        optional<Arc> arc = PlanetariaCache.arc_cache.Get(box_collider); // C++17 if statements are so pretty compared to this...
        optional<Zone> zone = PlanetariaCache.zone_cache.Get(box_collider);
        if (arc.exists) // block
        {
            optional<Block> block = PlanetariaCache.block_cache.Get(arc.data);
            if (!block.exists)
            {
                Debug.LogError("Critical Err0r.");
                return;
            }

            if (!collision_map.ContainsKey(block.data) && arc.data.contains(actor.transform.position, actor.transform.scale))
            {
                BlockInteractor collision = new BlockInteractor(block.data, block_index, interpolator_angle);
                collision_map.Add(block.data, collision);
                actor.OnBlockEnter(collision);
            }
            else if (collision_map.ContainsKey(block.data) && !block.data.contains(actor.transform.position, actor.transform.scale))
            {
                BlockInteractor collision = collision_map[block.data];
                actor.OnBlockExit(collision);
                collision_map.Remove(block.data);
            }

            if (collision_map.ContainsKey(block.data))
            {
                BlockInteractor collision = collision_map[block.data];
                actor.OnBlockStay(collision);
            }
        }
        else // zone
        {
            optional<Zone> zone = PlanetariaCache.zone_cache.Get(box_collider);
            if (!zone.exists)
            {
                Debug.LogError("This is likely an Err0r or setup issue.");
                return;
            }

            if (!trigger_map.ContainsKey(zone.data) && arc.data.contains(actor.transform.position, actor.transform.scale))
            {
                ZoneInteractor trigger = new ZoneInteractor(zone.data);
                trigger_map.Add(zone.data, trigger);
                actor.OnZoneEnter(trigger);
            }
            else if (trigger_map.ContainsKey(zone.data) && !zone.data.contains(actor.transform.position, actor.transform.scale))
            {
                ZoneInteractor collision = trigger_map[zone.data];
                actor.OnZoneExit(collision);
                trigger_map.Remove(zone.data);
            }

            if (trigger_map.ContainsKey(zone.data))
            {
                ZoneInteractor collision = trigger_map[zone.data];
                actor.OnZoneStay(collision);
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