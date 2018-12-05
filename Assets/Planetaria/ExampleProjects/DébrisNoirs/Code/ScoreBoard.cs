namespace Planetaria
{
    /// <summary>
	///
    /// </summary>
	public static class ScoreBoard
	{
		// Properties (Public)
		
		// Methods (Public)
		
		// Static Methods (Public)
		
		// Properties (non-Public)
		
		// Methods (non-Public)
		
		// Static Methods (non-Public)
		
		// Messages (non-Public)
				
		// Variables (Public)
		
		// Variables (non-Public)

        public static void increment_score(PlanetariaGameObject asteroid)
        {
            // find delta_mass
            // delta_mass = size*size - 2*(next_size*next_size)
            // e.g. delta_mass = 3*3 - 2*(2*2) = 9 - 2*4 = 9 - 8 = 1 // largest astroid
            // e.g. delta_mass = 2*2 - 2*(1*1) = 4 - 2*1 = 4 - 2 = 2 // medium asteroid
            // e.g. delta_mass = 1*1 - 2*(0*0) = 1 - 2*0 = 1 - 0 = 1 // small asteroid
            // but use PlanetariaMath.area_of_circle_on_sphere() // assume constant density
            // delta_mass might be negative if bad values are used (e.g. 1.1*1.1 - 2*(1*1)) so use a max(n,0) function just in case

            // find velocity (i.e. planetaria_rigidbody.internal_velocity.magnitude)
            // calculate m*v^2
        }

        private static float score_decimal = 0f;
        private static long score = 0; 
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