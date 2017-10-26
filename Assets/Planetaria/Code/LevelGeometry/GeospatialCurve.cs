using UnityEngine;

[System.Serializable]
public struct GeospatialCurve
{
    public static GeospatialCurve curve(Vector3 from, Vector3 slope, Vector3 to)
    {
        from.Normalize();
        slope = Vector3.ProjectOnPlane(slope, from);
        slope.Normalize();
        to.Normalize();

        Debug.Assert(from != slope);
        Debug.Assert(from != to);
        return new GeospatialCurve(from, slope, to);
    }

    public static GeospatialCurve line(Vector3 from, Vector3 to)
    {
        return curve(from, to, to);
    }

    /// <summary>
    /// Property - the start of the geospatial curve.
    /// </summary>
    public Vector3 from
    {
        get
        {
            return from_variable;
        }
    }

    /// <summary>
    /// Property - get the rightward slope starting at point "from".
    /// </summary>
    public Vector3 slope
    {
        get
        {
            return slope_variable;
        }
    }

    /// <summary>
    /// Property - the end of the geospatial curve.
    /// </summary>
    public Vector3 to
    {
        get
        {
            return to_variable;
        }
    }

    private GeospatialCurve(Vector3 from, Vector3 slope, Vector3 to)
    {
        from_variable = from;
        slope_variable = slope;
        to_variable = to;
    } 

    [SerializeField] private Vector3 from_variable;
    [SerializeField] private Vector3 slope_variable;
    [SerializeField] private Vector3 to_variable;
}
