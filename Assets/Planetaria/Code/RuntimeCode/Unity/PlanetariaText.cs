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

        // Notably, this function acts slightly differently than might be expected. The text does not have a uniform y height.
        // Future implementations could be different, but I would have to implement trapezoid colliders.
        
        /// <summary>
        /// Mutator - render the text at the frustum position, but make it curved (along a unit-sphere).
        /// </summary>
        /// <param name="vertex_helper">A handle to a mesh builder object (to which you can add vertices and triangles).</param>
        protected override void OnPopulateMesh(VertexHelper vertex_helper)
        {
            // I cannot change the canvas screen scaling, so store the inverse scale for later
            float parent_scale = rectTransform.parent.lossyScale.x; // x == y == z (in this case)
            float undo_scale = 1f/parent_scale; // parent should exist for everything but the canvas
            rectTransform.localScale = Vector3.one;
            // Get the text position for normal 2D text.
            base.OnPopulateMesh(vertex_helper);
            // Cache camera and inverse rotation and text_center (invariants)
            Camera camera = canvas.worldCamera;
            Quaternion inverse_rotation = Quaternion.Inverse(camera.transform.rotation);
            Vector3 text_center = camera.WorldToScreenPoint(rectTransform.TransformPoint(Vector3.zero));
            // Re-use UVs generated from text atlassing while modifying the positions on screen for spherical 2D text.
            for (int triangle = 0; triangle < vertex_helper.currentVertCount; triangle += 1)
            {
                UIVertex vertex = new UIVertex();
                vertex_helper.PopulateUIVertex(ref vertex, triangle);
                // Vector2 world_point = rectTransform.TransformPoint(vertex.position); // doesn't seem to work
                Vector2 screen_point = text_center + vertex.position; // not sure how scale affects TransformPoint call (documentation aside), otherwise I'd pass vertex.position
                Vector3 world_point = camera.ScreenPointToRay(screen_point).direction;
                vertex.position = inverse_rotation * world_point;
                vertex.normal = -vertex.position;
                vertex.position *= undo_scale;
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