using System;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace Planetaria
{
    // position, direction, and scale (and tags for when they are dirty)

    [Serializable]
    public struct PlanetariaPositionComponent : IComponentData
    {
        [SerializeField] public float3 data; // CONSIDER: double3, although without a double-precision quaternion this is less useful
    }
    
    [Serializable]
    public struct PlanetariaPreviousPositionComponent : IComponentData // CONSIDER: How do I remove this in the long run? Can I?
    {
        [SerializeField] public float3 data; // CONSIDER: double3
    }

    [Serializable]
    public struct PlanetariaDirectionComponent : IComponentData
    {
        [SerializeField] public float3 data; // CONSIDER: double3
    }

    [Serializable]
    public struct PlanetariaDirectionDirtyComponent : IComponentData // CONSIDER: How do I remove this in the long run? Can I?
    {
        [SerializeField] public byte data; // FIXME: blittable bool / Unity.Mathematics.bool1
    }

    [Serializable]
    public struct PlanetariaScaleComponent : IComponentData
    {
        [SerializeField] public float data;
    }

    
    public class PlanetariaTransform1 : ComponentDataWrapper<PlanetariaPositionComponent> { }
    public class PlanetariaTransform2 : ComponentDataWrapper<PlanetariaPreviousPositionComponent> { }
    public class PlanetariaTransform3 : ComponentDataWrapper<PlanetariaDirectionComponent> { }
    public class PlanetariaTransform4 : ComponentDataWrapper<PlanetariaDirectionDirtyComponent> { }
    public class PlanetariaTransform5 : ComponentDataWrapper<PlanetariaScaleComponent> { }
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