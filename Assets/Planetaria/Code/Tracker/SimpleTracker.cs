using System;
using UnityEngine;

namespace Planetaria
{
    /// <summary>
    /// If you don't know the type of Tracker you want, you probably want this one.
    /// </summary>
    public class SimpleTracker : PlanetariaTracker
    {
        public override void setup()
        {
            last_position = target.position.data;
        }

        public override void step()
        {
            if (last_position != target.position.data)
            {
                Vector3 last_velocity = Bearing.attractor(last_position, transform.position.data);
                Vector3 velocity = Bearing.repeller(transform.position.data, last_position);
                Quaternion last_rotation = Quaternion.LookRotation(last_position, last_velocity);
                Quaternion rotation = Quaternion.LookRotation(transform.position.data, velocity);
                Vector3 old_direction = gameObject.internal_game_object.transform.up;
                Vector3 relative_direction = Quaternion.Inverse(last_rotation) * old_direction;
                Vector3 new_direction = rotation * relative_direction;
                transform.direction = (NormalizedCartesianCoordinates) new_direction;
                transform.position = target.position;
                last_position = target.position.data;
            }
        }

        public override void cleanup() { }
        public override void teleport() { }
        
        [SerializeField] [HideInInspector] private Vector3 last_position;
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