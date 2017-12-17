using UnityEngine;

namespace Planetaria
{
    public class PlanetariaStereoscopicCamera : PlanetariaCamera
    {
	    private void Awake()
	    {
            initialize();

		    OVRCameraRig camera = GameObject.FindObjectOfType<OVRCameraRig>();

		    if (camera)
            {
			    camera.UpdatedAnchors += lock_camera;
            }
	    }

	    private void lock_camera(OVRCameraRig camera)
	    {
		    camera.trackingSpace.FromOVRPose(camera.centerEyeAnchor.ToOVRPose(true).Inverse(), true); // undo all headtracking (by reverting the changes)

		    camera.leftEyeAnchor.FromOVRPose(OVRPose.identity); // reset all relative positions to origin
		    camera.rightEyeAnchor.FromOVRPose(OVRPose.identity);
		    camera.leftHandAnchor.FromOVRPose(OVRPose.identity);
		    camera.rightHandAnchor.FromOVRPose(OVRPose.identity);
		    camera.trackerAnchor.FromOVRPose(OVRPose.identity);
	    }
    }
}

/*
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/