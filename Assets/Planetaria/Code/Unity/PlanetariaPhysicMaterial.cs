namespace Planetaria
{
    public class PlanetariaPhysicMaterial // https://gamedevelopment.tutsplus.com/tutorials/how-to-create-a-custom-2d-physics-engine-friction-scene-and-jump-table--gamedev-7756
    {
        public float elasticity; /*Suggested: [0,1); 0 for no bounce, 1 for perfect bounce*/
        public float static_friction; /*Suggested: [0,1); 0 for no friction, 1 for heavy friction*/
        public float dynamic_friction; /*Suggested: [0,1); 0 for no friction, 1 for heavy friction*/
        public float magnetism; /*Suggested: (-inf, +inf)*/
        public float induced_magnetism_multiplier; /*Suggested: 0 for plastic, .01 for metal*/
        public PlanetariaPhysicMaterialCombine elasticity_combine; /*Suggested: PlanetariaPhysicMaterialCombine.Quadratic*/
        public PlanetariaPhysicMaterialCombine friction_combine; /*Suggested: PlanetariaPhysicMaterialCombine.Geometric*/
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