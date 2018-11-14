﻿using System;
using UnityEngine;

namespace Planetaria
{
    /// <summary>
	/// A circular sector light (similar to UnityEngine's SpotLight).
    /// </summary>
    [Serializable]
	public sealed class SectorLight : PlanetariaLight
	{
		// Properties (Public)
		
        /// <summary>The angle of the circular sector (in radians) of the sector light. Range: [0, 2PI].</summary>
        public float sectorAngle
        {
            get
            {
                return sector_angle_variable;
            }
            set
            {
                sector_angle_variable = value;
            }
        }

		// Methods (Public)
		
		// Static Methods (Public)
		
		// Properties (non-Public)
		
		// Methods (non-Public)
		
		// Static Methods (non-Public)
		
		// Messages (non-Public)
				
		// Variables (Public)
		
		// Variables (non-Public)
        
        [SerializeField] private float sector_angle_variable;
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