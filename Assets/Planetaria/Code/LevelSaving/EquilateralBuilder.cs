#if UNITY_EDITOR 

using UnityEngine;
using System.Collections.Generic;

namespace Planetaria
{
    [ExecuteInEditMode]
    public class EquilateralBuilder : MonoBehaviour
    {
        public static EquilateralBuilder equilateral_builder(Vector3 center, int edges)
        {
            GameObject game_object = new GameObject("Equilateral Builder");
            EquilateralBuilder result = game_object.AddComponent<EquilateralBuilder>();
            result.center = result.first_vertex = center;
            result.arcs.Add(Arc.line(center, center));
            return result;
        }

        public void set_edge(Vector3 first_corner)
        {
            this.first_vertex = first_corner;
        }

        public void close_shape()
        {
            GameObject shape = Block.block(curves);
            Block block = shape.GetComponent<Block>();
            optional<TextAsset> svg_file = BlockRenderer.render(block);
            PlanetariaRenderer renderer = shape.AddComponent<PlanetRenderer>();
            if (svg_file.exists)
            {
                //renderer.material = RenderVectorGraphics.render(svg_file.data);
            }
            DestroyImmediate(this.gameObject);
        }
    
        public List<Arc> arcs = new List<Arc>();
        private List<GeospatialCurve> curves = new List<GeospatialCurve>();
        private Vector3 center { get; set; }
        private Vector3 first_vertex { get; set; }
    }
}

#endif

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
