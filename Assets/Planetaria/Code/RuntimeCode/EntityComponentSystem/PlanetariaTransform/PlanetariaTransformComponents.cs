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
        [SerializeField] public float3 data; // CONSIDER: double3
    }
    
    [Serializable]
    public struct PlanetariaPreviousPositionComponent : IComponentData // CONSIDER: How do I remove this in the long run? Can I?
    {
        [SerializeField] public float3 data; // CONSIDER: double3
    }

    // TODO: I have the sneaking suspicion that the barriers required for the dirty flags will cost more than they will save (also, I think I implicitly need a boolean just to add these dirty flags at the barrier, which basically defeats the point)
    public struct PlanetariaPositionDirtyComponent : IComponentData { } // tag for when position needs to be updated.

    [Serializable]
    public struct PlanetariaDirectionComponent : IComponentData
    {
        [SerializeField] public float3 data; // CONSIDER: double3
    }
        
    public struct PlanetariaDirectionDirtyComponent : IComponentData { } // tag for when direction needs to be updated.

    [Serializable]
    public struct PlanetariaScaleComponent : IComponentData
    {
        [SerializeField] public float data;
    }
    
    public struct PlanetariaScaleDirtyComponent : IComponentData { } // tag for when scale needs to be updated.
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