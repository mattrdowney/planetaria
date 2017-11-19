using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ArcBuilder : MonoBehaviour
{
    public static ArcBuilder arc_builder(Vector3 original_point)
    {
        GameObject game_object = new GameObject("Arc Builder");
        ArcBuilder result = game_object.AddComponent<ArcBuilder>();
        result.point = result.original_point = original_point;
        result.arcs.Add(Arc.line(original_point, original_point));
        return result;
    }

    public void receive_vector(Vector3 vector)
    {
        if (state == CreationState.SetSlope)
        {
            slope = vector;
            arcs[arcs.Count-1] = Arc.line(point, slope);
        }
        else // CreationState.SetPoint
        {
            if (arcs.Count != 0)
            {
                arcs[arcs.Count-1] = Arc.curve(arcs[arcs.Count-1].position(0), slope, point);
            }
            point = vector;
        }
    }

    public void next()
    {
        if (state == CreationState.SetSlope)
        {
            finalize();
        }
        state = (state == CreationState.SetSlope ? CreationState.SetPoint : CreationState.SetSlope); // state = !state;
    }

    public void finalize()
    {
        curves.Add(GeospatialCurve.curve(point, slope));
        arcs.Add(arcs[arcs.Count-1]);
        UnityEditor.EditorUtility.SetDirty(this.gameObject);
        point = slope; // point should always be defined
    }

    public void close_shape()
    {
        if (state == CreationState.SetSlope)
        {
            curves.Add(GeospatialCurve.curve(point, original_point));
        }

        GameObject shape = Block.block(curves);
        Block block = shape.GetComponent<Block>();
        optional<TextAsset> svg_file = BlockRenderer.render(block);
        OctahedronMesh mesh = shape.AddComponent<OctahedronMesh>();
        Renderer renderer = shape.GetComponent<Renderer>();
        if (svg_file.exists)
        {
            //renderer.material = RenderVectorGraphics.render(svg_file.data);
        }
        DestroyImmediate(this.gameObject);
    }
    
    public List<Arc> arcs = new List<Arc>();
    private List<GeospatialCurve> curves = new List<GeospatialCurve>();
    private enum CreationState { SetSlope = 0, SetPoint = 1 }
    private CreationState state = CreationState.SetSlope;
    private Vector3 point { get; set; }
    private Vector3 slope { get; set; }
    private Vector3 original_point;
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