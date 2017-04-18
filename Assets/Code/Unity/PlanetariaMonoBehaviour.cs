using UnityEngine;

public class PlanetariaMonoBehaviour : MonoBehaviour
{
    PlanetariaActor actor;

	void Start()
    {
		actor.Start();
	}
	
	void Update()
    {
		actor.Update();
	}

    void OnTriggerStay(Collider collider)
    {
        BoxCollider box_collider = collider as BoxCollider;
        if (!box_collider)
        {
            return;
        }

        if (PlanetariaCache.GetArc(box_collider).exists) // block
        {
            if (just_started_colliding)
            {
                actor.OnBlockEnter();
            }
            else if (just_stopped_colliding)
            {
                actor.OnBlockExit();
            }

            if (in_map)
            {
                actor.OnBlockStay();
            }
        }
        else // zone
        {
            if (just_started_colliding)
            {
                actor.OnZoneEnter();
            }
            else if (just_stopped_colliding)
            {
                actor.OnZoneExit();
            }

            if (in_map)
            {
                actor.OnZoneStay();
            }
        }
    }
}
