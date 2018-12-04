using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Planetaria
{
    /// <summary>
	///
    /// </summary>
    /// <seealso cref="https://www.gamedev.net/articles/programming/engines-and-middleware/how-to-implement-custom-ui-meshes-in-unity-r5017/"/>
    /// <seealso cref="https://stackoverflow.com/questions/40529025/unity-c-sharp-get-text-width-font-character-width"/>
	public class PlanetariaText : Text 
	{
		// Properties (Public)
		
		// Methods (Public)
		
		// Static Methods (Public)
		
		// Properties (non-Public)
		
		// Methods (non-Public)

        protected override void OnPopulateMesh(VertexHelper vertex_helper)
        {
            // I'm pretty certain I need to 1) if possible re-use UVs (otherwise I don't think this works), and 2) override vertices.
            // This will be interesting, because I'm blindly testing the API of UnityEngine.UI.Text (more or less).
            // Note: text rendering uses atlassing (not un-expected, but since font-letter pairs are generated on the fly and hot reloaded it means extra consideration must be taken).

            base.OnPopulateMesh(vertex_helper);
            for (int triangle = 0; triangle < vertex_helper.currentVertCount; triangle += 1)
            {
                UIVertex vertex = new UIVertex();
                vertex_helper.PopulateUIVertex(ref vertex, triangle);
                vertex.position = ((NormalizedCartesianCoordinates)new NormalizedSphericalCoordinates(vertex.position.y/100, vertex.position.x/100)).data;
                vertex.normal = -vertex.position;
                vertex_helper.SetUIVertex(vertex, triangle);
            }
        }
		
		// Static Methods (non-Public)
		
		// Messages (non-Public)
				
		// Variables (Public)
		
		// Variables (non-Public)
		
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