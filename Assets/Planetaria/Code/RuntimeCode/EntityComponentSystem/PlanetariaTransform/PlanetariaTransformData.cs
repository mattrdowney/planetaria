using System;
using UnityEngine;
using Unity.Entities;

namespace Planetaria
{
    [Serializable]
    public struct PlanetariaTransformPosition : IComponentData
    {
        public PlanetariaTransformPosition(Vector3 position_data)
        {
            position = position_data;
        }

        [SerializeField] public Vector3 position; // TODO: figure out how to safely populate this to Vector3.forward (or similar), because Vector3.zero is a problematic float for Planetaria's internal systems
        //[SerializeField] public bool position_dirty = true; // I was thinking about using a dirty tag (component), but really updating every frame makes more sense (but there are issues with not having a dirty tag for directions, so I shall include it for everything). // To be fair, static objects are more common (for things that aren't Asteroids (1979))
    }

    public struct PlanetariaTransformPositionDirty : IComponentData { } // Entity-Component System tag

    [Serializable]
    public struct PlanetariaTransformDirection : IComponentData
    {
        public PlanetariaTransformDirection(Vector3 direction_data)
        {
            direction = direction_data;
        }

        [SerializeField] public Vector3 direction; // TODO: figure out how to safely populate this to Vector3.up (or similar), because Vector3.zero is a problematic float for Planetaria's internal systems
    }

    public struct PlanetariaTransformDirectionDirty : IComponentData { } // Entity-Component System tag

    [Serializable]
    public struct PlanetariaTransformScale : IComponentData
    {
        public PlanetariaTransformScale(float scale_data)
        {
            scale = scale_data;
        }

        [SerializeField] public float scale; // TODO: figure out how to safely populate this to 1f (or similar), because you don't want everything to be zero-scaled (invisible, infinitessimal collider).
    }

    public struct PlanetariaTransformScaleDirty : IComponentData { } // Entity-Component System tag
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