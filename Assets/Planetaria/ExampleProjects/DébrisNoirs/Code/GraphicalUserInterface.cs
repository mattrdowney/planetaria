using UnityEngine;
using UnityEngine.XR;

namespace Planetaria
{
    /// <summary>
	///
    /// </summary>
	public class GraphicalUserInterface : PlanetariaMonoBehaviour
	{
        // Methods (non-Public)
        
        protected override void OnConstruction() { }

        protected override void OnDestruction() { }

		// Messages (non-Public)

        private void Start()
        {
            graphical_user_interface = PlanetariaGameObject.Find("Head_Adjustor").gameObject.internal_game_object.GetComponent<RectTransform>();
            graphical_user_interface.sizeDelta = graphical_user_interface.parent.GetComponent<RectTransform>().sizeDelta;
#if UNITY_EDITOR
            inverse_start_rotation = Quaternion.identity;
#else
            inverse_start_rotation = Quaternion.Inverse(InputTracking.GetLocalRotation(XRNode.Head));
#endif
        }

        private void LateUpdate()
        {
             // TODO: implement color, transparency, head_tracking, etc
#if UNITY_EDITOR
#else
            graphical_user_interface.rotation = Quaternion.Inverse(InputTracking.GetLocalRotation(XRNode.Head) * inverse_start_rotation);
#endif
        }

        // Variables (non-Public)

        private Quaternion inverse_start_rotation;
        private RectTransform graphical_user_interface;
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