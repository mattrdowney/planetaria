using System.Collections.Generic;
using System.Linq;
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

        // NOTE:
        // I have the suspicion that rectTransform text that is converted will have non-uniform character spacing as you get closer to the peripherals.
        // Based on (rudimentary) manual inspection, this appears not to be the case, but I should still look out for it.
        // The rectangles are converted to skewed text on the screen for virtual reality (standard 2D looks "normal").
        // The basic logic for getting text should be:
        // Vector2 screen_point = RectTransformUtility.PixelAdjustRect(rectTransform, canvas).center + vertex.position;
        // Vector3 world_point = camera.ScreenPointToRay(screen_point).direction;
        // vertex.position = inverse_rotation * world_point;
        // vertex.normal = -vertex.position;
        // vertex.position *= undo_scale;
        // (Theory based on non-uniform character spacing...)
        // Keep y coordinate as-is.
        // Adjust the x coordinate on the screen by considering the "angular sweep" from the left to the right of the text.
        // 2nd Note:
        // it seems like the x "angular sweep" is fine.
        // the y size shrinks as the text gets closer to the periphery.
        // This has to do with how I am using squares, not trapezoids (I think this is a special case of a right trapezoid).
        // In this case, the approximation is fine because you can always generate the text at the center of the screen and rotate it.

        protected override void OnPopulateMesh(VertexHelper vertex_helper)
        {
            // I cannot change the canvas screen scaling, so store the inverse scale for later
            float parent_scale = rectTransform.parent.lossyScale.x; // x == y == z (in this case)
            float undo_scale = 1f/parent_scale; // parent should exist for everything but the canvas
            rectTransform.localScale = Vector3.one;
            // Get the text position for normal 2D text.
            base.OnPopulateMesh(vertex_helper);
            // Cache camera and inverse rotation
            Camera camera = canvas.worldCamera;
            Quaternion inverse_rotation = Quaternion.Inverse(camera.transform.rotation);
            // Re-use UVs generated from text atlassing while modifying the positions on screen for spherical 2D text.
            for (int triangle = 0; triangle < vertex_helper.currentVertCount; triangle += 1)
            {
                UIVertex vertex = new UIVertex();
                vertex_helper.PopulateUIVertex(ref vertex, triangle);
                Debug.Log(vertex.position + " and " + rectTransform.anchoredPosition);
                Vector2 screen_point = camera.WorldToScreenPoint(rectTransform.TransformPoint(Vector3.zero)) + vertex.position; // not sure how scale affects TransformPoint call (documentation aside), otherwise I'd pass vertex.position
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