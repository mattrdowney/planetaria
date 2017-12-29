namespace Planetaria
{
    public class PlanetariaPhysicMaterial // https://gamedevelopment.tutsplus.com/tutorials/how-to-create-a-custom-2d-physics-engine-friction-scene-and-jump-table--gamedev-7756
    {
        float elasticity;
        float static_friction;
        float dynamic_friction;
        float magnetism;
        PlanetariaPhysicMaterialCombine elasticity_combine; /*Suggested: PlanetariaPhysicMaterialCombine.Quadratic*/
        PlanetariaPhysicMaterialCombine friction_combine; /*Suggested: PlanetariaPhysicMaterialCombine.Geometric*/
        PlanetariaPhysicMaterialCombine magnetism_combine; /*Suggested: PlanetariaPhysicMaterialCombine.Multiply*/
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