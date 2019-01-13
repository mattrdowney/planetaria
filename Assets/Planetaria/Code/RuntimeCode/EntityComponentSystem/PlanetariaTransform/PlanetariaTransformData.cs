using System;
using UnityEngine;

namespace Planetaria
{
    [Serializable]
    public class PlanetariaTransformData : MonoBehaviour // TODO: breakout into PlanetariaTransformPosition, PlanetariaTransformDirection, PlanetariaTransformScale
    {
        //[SerializeField] private Quaternion rotation; // implicitly held in UnityEngine.Transform and represents both PlanetariaTransform.position and PlanetariaTransform.direction but you can't efficiently change the Transform like that
        
        [SerializeField] public bool position_dirty;
        [SerializeField] public bool direction_dirty;
        [SerializeField] public bool scale_dirty;

        [SerializeField] public Vector3 position;
        [SerializeField] public Vector3 direction;
        [SerializeField] public float scale;
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