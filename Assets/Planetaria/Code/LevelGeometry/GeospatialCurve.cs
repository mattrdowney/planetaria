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
    public Vector3 from { get; private set; }

    /// <summary>
    /// Property - get the rightward slope starting at point "from".
    /// </summary>
    public Vector3 slope { get; private set; }

    /// <summary>
    /// Property - the end of the geospatial curve.
    /// </summary>
    public Vector3 to { get; private set; }

    private GeospatialCurve(Vector3 from, Vector3 slope, Vector3 to)
    {
        this.from = from;
        this.slope = slope;
        this.to = to;
    }
}
