using System;
using UnityEngine;

namespace Planetaria
{
    // There are many seemingly-contradictory design specifications needed for this file.
    // The most prominent is the angle condition for unmoving objects vs very fast (>~ 5*PI radians per second)
    // Once I wrote this out, I remembered that around this speed a full rotation can be done in 24- frames,
    // so Quaternion.Slerp may be in order
    public class SimpleTracker : PlanetariaTracker // Camera tracking isn't simple, but it looks simple
    {
        public override void setup()
        {
            positions[0] = Vector3.Slerp(-target.gameObject.internal_game_object.transform.up, target.gameObject.internal_game_object.transform.forward, 0.8f); // unused
            positions[1] = Vector3.Slerp(-target.gameObject.internal_game_object.transform.up, target.gameObject.internal_game_object.transform.forward, 0.9f); // "previous position"
            positions[2] = Vector3.Slerp(-target.gameObject.internal_game_object.transform.up, target.gameObject.internal_game_object.transform.forward, 1.0f); // current position

            // Ditto with normals
        }

        public override void step()
        {
            if (target.position.data != positions[2])
            {
                positions[0] = positions[1];
                positions[1] = positions[2];
                positions[2] = target.position.data;
                //float angle_shift_v1 = Vector3.SignedAngle(positions[0], positions[2], positions[1]); // would often give ~ -180 or ~ +180
                Vector3 segment_a = Vector3.ProjectOnPlane(-positions[0], positions[1]).normalized;
                Vector3 segment_b = Vector3.ProjectOnPlane(positions[2], positions[1]).normalized;
                float angle_shift = Vector3.SignedAngle(segment_a, segment_b, positions[1]);
                angle_shift *= Mathf.Deg2Rad;
                angle -= angle_shift;
                Debug.Log(angle_shift + " " + angle);
                self.position = (NormalizedCartesianCoordinates) target.position.data;
                self.direction = (NormalizedCartesianCoordinates) Bearing.attractor(target.position.data, target.gameObject.internal_game_object.transform.up);
                //self.direction = (NormalizedCartesianCoordinates)
                //    (Quaternion.LookRotation(positions[2], Vector3.Cross(Vector3.Cross(positions[1], positions[2]), positions[2])) * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0));
            }
        }

        public override void cleanup() { }
        public override void teleport() { }

        [SerializeField] [HideInInspector] private float angle = Mathf.PI/2;
        [SerializeField] [HideInInspector] private Vector3[] positions = new Vector3[3];
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