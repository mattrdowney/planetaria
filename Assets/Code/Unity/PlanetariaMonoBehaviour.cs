using UnityEngine;

public sealed class PlanetariaMonoBehaviour : MonoBehaviour
{
    PlanetariaActor actor;

	public void Start()
    {
		actor.Start();
	}
	
	public void Update()
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

        if (PlanetariaCache.GetArc(box_collider).exists) // block
        {
            BlockInteractor collision = new BlockInteractor(PlanetariaCache.GetArc(box_collider).data);

            if (just_started_colliding)
            {
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