using System;
using UnityEngine;
using Unity.Entities;

namespace Planetaria
{
    [Serializable]
    public struct PlanetariaVelocityComponent : IComponentData
    {
        public PlanetariaVelocityComponent(Vector3 velocity_data)
        {
            velocity = velocity_data;
        }

        [SerializeField] public Vector3 velocity; // magnitude in [0, infinity]
        //[SerializeField] public float horizontal_velocity; // implicitly - through grounded state
        //[SerializeField] public float vertical_velocity;
    }

    [Serializable]
    public struct PlanetariaAccelerationComponent : IComponentData // Implementation note: if PlanetariaRigidbodyGravity is Vector3.zero, the PlanetariaRigidbodyAcceleration tag/acceleration is removed
    {
        public PlanetariaAccelerationComponent(Vector3 acceleration_data)
        {
            acceleration = acceleration_data;
        }
        
        [SerializeField] public Vector3 acceleration; // magnitude in [0, infinity]        
        //[SerializeField] public float horizontal_acceleration; // implicitly - through grounded state
        //[SerializeField] public float vertical_acceleration;
    }

    [Serializable]
    public struct PlanetariaGravityComponent : IComponentData
    {
        public PlanetariaGravityComponent(Vector3 gravity_data)
        {
            gravity = gravity_data;
        }

        [SerializeField] public Vector3 gravity; // magnitude in [0, infinity]
    }

    public struct PlanetariaGravityDirtyComponent : IComponentData { } // tag
}

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.