using UnityEngine;

namespace Planetaria
{
    public static class GridUtility
    {
        public static void draw_grid(int rows, int columns)
        {
		    UnityEditor.Handles.color = Color.white;

		    for(float row = 1; row <= rows; ++row)
		    {
			    UnityEditor.Handles.DrawWireDisc(Vector3.down*Mathf.Cos(Mathf.PI*row/(rows+1)),
			                                     Vector3.up,
			                                     Mathf.Sin(Mathf.PI*row/(rows+1)));
		    }
		
		    for(float column = 0; column < columns; ++column)
		    {
			    UnityEditor.Handles.DrawWireDisc(Vector3.zero,
			                                     Vector3.forward*Mathf.Cos(Mathf.PI*column/columns) +
			                                     Vector3.right  *Mathf.Sin(Mathf.PI*column/columns),
			                                     1);
		    }
        }

        public static Vector3 grid_snap(Vector3 position, float rows, float columns) // FIXME: optimize
	    {
            if(!Event.current.shift) // only snap when shift key is held
            {
                return position;
            }

            NormalizedCartesianCoordinates cartesian_position = new NormalizedCartesianCoordinates(position);

		    NormalizedSphericalCoordinates clamped_coordinates = new NormalizedSphericalCoordinates(0, 0);
            NormalizedSphericalCoordinates desired_coordinates = cartesian_position;

		    for (float row = 0; row <= rows + 1; ++row) //going over with off by one errors won't ruin the program...
		    {
                float angle = Mathf.PI*row/(rows+1);
			    float x = Mathf.Sin(angle);
                float y = Mathf.Cos(angle);
                NormalizedSphericalCoordinates test_coordinates = new NormalizedCartesianCoordinates(new Vector3(x, y, 0));

                float error = Mathf.Abs(Mathf.DeltaAngle(desired_coordinates.data.x * Mathf.Rad2Deg, test_coordinates.data.x * Mathf.Rad2Deg));
			    float old_error = Mathf.Abs(Mathf.DeltaAngle(desired_coordinates.data.x * Mathf.Rad2Deg, clamped_coordinates.data.x * Mathf.Rad2Deg));

			    if(error < old_error)
			    {
				    clamped_coordinates = new NormalizedSphericalCoordinates(test_coordinates.data.x, clamped_coordinates.data.y);
			    }
		    }

		    for (float column = 0; column < columns*2; ++column) //... but going under is bad
		    {
			    float angle = column/columns*Mathf.PI;
                float x = Mathf.Sin(angle);
                float z = Mathf.Cos(angle);
                NormalizedSphericalCoordinates test_coordinates = new NormalizedCartesianCoordinates(new Vector3(x, 0, z));

                float error = Mathf.Abs(Mathf.DeltaAngle(desired_coordinates.data.y * Mathf.Rad2Deg, test_coordinates.data.y * Mathf.Rad2Deg));
			    float old_error = Mathf.Abs(Mathf.DeltaAngle(desired_coordinates.data.y * Mathf.Rad2Deg, clamped_coordinates.data.y * Mathf.Rad2Deg));

			    if(error < old_error)
			    {
				    clamped_coordinates = new NormalizedSphericalCoordinates(clamped_coordinates.data.x, test_coordinates.data.y);
			    }
		    }

            cartesian_position = clamped_coordinates;

            return cartesian_position.data;
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