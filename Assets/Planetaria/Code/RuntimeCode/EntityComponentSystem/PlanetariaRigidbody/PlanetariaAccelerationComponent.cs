using System;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace Planetaria
{
    [Serializable]
    public struct PlanetariaAcceleration : IComponentData // Implementation note: if PlanetariaRigidbodyGravity is Vector3.zero, the PlanetariaRigidbodyAcceleration tag/acceleration is removed
    {
        [SerializeField] public float3 data; // magnitude in [0, infinity]
        //[SerializeField] public float2 data; // implicitly shared - for grounded state
    }

    [DisallowMultipleComponent] public class PlanetariaAccelerationComponent : ComponentDataWrapper<PlanetariaAcceleration> { }
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