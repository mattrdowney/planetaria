using System;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace Planetaria
{
    [Serializable]
    public struct PlanetariaVelocityComponent : IComponentData
    {
        [SerializeField] public float3 data; // magnitude in [0, infinity]
        //[SerializeField] public float2 data; // implicitly shared - for grounded state
    }

    [Serializable]
    public struct PlanetariaAccelerationComponent : IComponentData // Implementation note: if PlanetariaRigidbodyGravity is Vector3.zero, the PlanetariaRigidbodyAcceleration tag/acceleration is removed
    {
        [SerializeField] public float3 data; // magnitude in [0, infinity]
        //[SerializeField] public float2 data; // implicitly shared - for grounded state
    }

    [Serializable]
    public struct PlanetariaGravityComponent : IComponentData
    {
        [SerializeField] public float3 data; // magnitude in [0, infinity]
    }

    public struct PlanetariaRigidbodyAerialComponent : IComponentData { } // tag for whether Rigidbody is grounded or in midair.

    public class PlanetariaRigidbody1 : ComponentDataWrapper<PlanetariaVelocityComponent> { }
    public class PlanetariaRigidbody2 : ComponentDataWrapper<PlanetariaAccelerationComponent> { }
    public class PlanetariaRigidbody3 : ComponentDataWrapper<PlanetariaGravityComponent> { }
    public class PlanetariaRigidbody4 : ComponentDataWrapper<PlanetariaRigidbodyAerialComponent> { }
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