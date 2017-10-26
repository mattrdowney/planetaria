using UnityEngine;

[ExecuteInEditMode]
public class ArcBuilder : MonoBehaviour
{
    private enum ConstructionState { SetFrom = 0, SetTangentLine = 1, SetTo = 2 }

    public static ArcBuilder arc_builder()
    {
        GameObject block = Block.block();
        return block.AddComponent<ArcBuilder>();
    }

    public void advance()
    {
        ++build_state;
    }

    public void finalize_edge()
    {
        Block block = this.gameObject.GetComponent<Block>();
        block.add(curve);
        UnityEditor.EditorUtility.SetDirty(block.gameObject);

        build_state = ConstructionState.SetTangentLine;
        arc_variable = new optional<Arc>();
        from_variable = to_variable;
    }

    public void close_shape()
    {
        GameObject shape = this.gameObject;
        Block block = this.gameObject.GetComponent<Block>();

        to = original_point;

        if (arc.exists)
        {
            block.add(curve);
            UnityEditor.EditorUtility.SetDirty(block.gameObject);
        }

        if (block.empty())
        {
            GameObject.DestroyImmediate(shape);
        }
        else
        {
            BlockRenderer.render(block);
            OctahedronMesh mesh = shape.AddComponent<OctahedronMesh>();
            Renderer renderer = shape.GetComponent<Renderer>();
            TextAsset svg_file = VectorGraphicsWriter.get_svg();
            renderer.material = RenderVectorGraphics.render(svg_file);  
            DestroyImmediate(this);
        }
    }

    public Vector3 from
    {
        set
        {
            from_variable = value.normalized;

            if (build_state == ConstructionState.SetFrom)
            {
                original_point = from_variable;
            }
        }

        get
        {
            return from_variable;
        }
    }

    public Vector3 from_tangent
    {
        set
        {
            to_variable = value.normalized;
            from_tangent_variable = (to_variable - from_variable).normalized;
            arc_variable = Arc.arc(curve);
        }
    }

    public Vector3 to
    {
        set
        {
            if (build_state != ConstructionState.SetFrom)
            {
                to_variable = value.normalized;
                arc_variable = Arc.arc(curve);
            }
        }
    }

    public GeospatialCurve curve
    {
        get
        {
            if (from_tangent_variable != Vector3.zero && build_state == ConstructionState.SetTo) // for defined slopes, use standard rendering
            {
                return GeospatialCurve.curve(from_variable, from_tangent_variable, to_variable);
            }
            else // draw the shortest path if no slope was defined
            {
                return GeospatialCurve.line(from_variable, to_variable);
            }
            
        }
    }

    public optional<Arc> arc
    {
        get
        {
            return arc_variable;
        }
    }

    private ConstructionState build_state;
    private optional<Arc> arc_variable;
    private Vector3 original_point;

    private Vector3 from_variable;
    private Vector3 from_tangent_variable;
    private Vector3 to_variable;
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