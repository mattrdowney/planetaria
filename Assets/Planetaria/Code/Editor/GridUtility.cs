using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridUtility
{
    public static Vector3 grid_snap(Vector3 position, float rows, float columns) // FIXME: optimize
	{
        NormalizedCartesianCoordinates Cartesian_position = new NormalizedCartesianCoordinates(position);

		NormalizedSphericalCoordinates clamped_coordinates = new NormalizedSphericalCoordinates(0, 0);
        NormalizedSphericalCoordinates desired_coordinates = Cartesian_position;

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

        Debug.Log("phi: " + clamped_coordinates.data.x + " theta: " + clamped_coordinates.data.y + " dphi: " + desired_coordinates.data.x + " dtheta: " + desired_coordinates.data.y);

        Cartesian_position = clamped_coordinates;

        return Cartesian_position.data;
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