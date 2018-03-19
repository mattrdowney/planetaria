using UnityEngine;
using Planetaria;

public class InstantRotationTracker : PlanetariaTracker
{
    public override void setup()
    {
        target = GameObject.Find("Character").GetComponent<PlanetariaTransform>();
    }

    public override void step()
    {
        NormalizedSphericalCoordinates self_position = self.position;
        NormalizedSphericalCoordinates target_position = target.data.position;
        self.rotation = 
    }

    public override void cleanup() { }
    public override void teleport() { }
}
