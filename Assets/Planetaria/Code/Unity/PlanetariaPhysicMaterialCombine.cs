using UnityEngine;

namespace Planetaria
{
    public enum PlanetariaPhysicMaterialCombine // CONSIDER: do I *really* want this?
    {
        Harmonic = 0, // Harmonic < Geometric < Average < Quadratic (definition)
        Geometric = 1,
        Average = 2,
        Quadratic = 3,
        Minumum = 4,
        Multiply = 5,
        Maximum = 6
    }

    public static class PlanetariaPhysic
    {
        public static float blend(float left, PlanetariaPhysicMaterialCombine left_type,
                float right, PlanetariaPhysicMaterialCombine right_type)
        {
            PlanetariaPhysicMaterialCombine type = (left_type >= right_type ? left_type : right_type);
            switch(type) // Overengineering, sib
            {
                // Note: all functions map from (0,0)->0 and (1,1)->1; (c,c)->c [for c >= 0, except Multiply]
                // values (positive and negative) outside this range can still be used
                case PlanetariaPhysicMaterialCombine.Harmonic: return left+right != 0 ? 2*left*right/(left + right) : 0; // avoid division by zero
                case PlanetariaPhysicMaterialCombine.Geometric: return Mathf.Sign(left*right) == +1 ? Mathf.Sqrt(left*right) : 0; // sqrt(negative) is undefined
                case PlanetariaPhysicMaterialCombine.Average: return (left + right)/2;
                case PlanetariaPhysicMaterialCombine.Quadratic: return Mathf.Sqrt((left*left + right*right)/2);
                case PlanetariaPhysicMaterialCombine.Minumum: return Mathf.Min(left, right);
                case PlanetariaPhysicMaterialCombine.Multiply: return left * right;
            }
            /*case PlanetariaPhysicMaterialCombine.Maximum:*/ return Mathf.Max(left, right);
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