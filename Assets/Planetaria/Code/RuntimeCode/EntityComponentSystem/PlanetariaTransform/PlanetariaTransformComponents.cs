using System;
using UnityEngine;
using Unity.Entities;

namespace Planetaria
{
    // position, direction, and scale (and tags for when they are dirty)

    [Serializable]
    public struct PlanetariaPositionComponent : IComponentData
    {
        public PlanetariaPositionComponent(Vector3 position_data)
        {
            position = position_data;
        }

        [SerializeField] public Vector3 position;
    }

    [Serializable]
    public struct PlanetariaPreviousPositionComponent : IComponentData
    {
        public PlanetariaPreviousPositionComponent(Vector3 previous_position_data)
        {
            previous_position = previous_position_data;
        }

        [SerializeField] public Vector3 previous_position;
    }

    [Serializable]
    public struct PlanetariaDirectionComponent : IComponentData
    {
        public PlanetariaDirectionComponent(Vector3 direction_data)
        {
            direction = direction_data;
        }

        [SerializeField] public Vector3 direction;
    }

    [Serializable]
    public struct PlanetariaPreviousDirectionComponent : IComponentData
    {
        public PlanetariaPreviousDirectionComponent(Vector3 previous_direction_data)
        {
            previous_direction = previous_direction_data;
        }

        [SerializeField] public Vector3 previous_direction;
    }

    [Serializable]
    public struct PlanetariaScaleComponent : IComponentData
    {
        public PlanetariaScaleComponent(float scale_data)
        {
            scale = scale_data;
        }

        [SerializeField] public float scale;
    }

    [Serializable]
    public struct PlanetariaPreviousScaleComponent : IComponentData
    {
        public PlanetariaPreviousScaleComponent(float previous_scale_data)
        {
            previous_scale = previous_scale_data;
        }

        [SerializeField] public float previous_scale;
    }
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