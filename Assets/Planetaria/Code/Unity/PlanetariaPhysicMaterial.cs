using UnityEngine;

namespace Planetaria
{
    // TODO: PlanetariaPhysicMaterialEditor (CreateAsset)
    [CreateAssetMenu(fileName = "PlanetariaPhysicMaterial", menuName = "PlanetariaPhysicMaterial", order = 1)]
    public class PlanetariaPhysicMaterial : ScriptableObject // https://gamedevelopment.tutsplus.com/tutorials/how-to-create-a-custom-2d-physics-engine-friction-scene-and-jump-table--gamedev-7756
    {
        [SerializeField]
        [Range(0,1)]
        [Tooltip("The bounciness of a surface: 0 means no bounce; 1 means perfect bounce")]
        public float elasticity;

        [SerializeField]
        [Range(0,Mathf.Infinity)]
        [Tooltip("The force of friction past which an object will start moving: 0 means no friction, 1 is a typical max friction")]
        public float friction;

        [SerializeField]
        [Range(Mathf.NegativeInfinity,Mathf.Infinity)]
        [Tooltip("The magnetic charge of a surface: opposites attract, likes repel, magnitudes are unbounded and multiplied")]
        public float magnetism;

        [SerializeField]
        [Range(0,1)]
        [Tooltip("The fractional (and opposing) charge induced in a material when exposed to a magnetic field: 0 means no ferromagnetism (e.g. plastic), 0.01 would be a typical coefficient for iron (which is attracted to magnets)")]
        public float induced_magnetism_multiplier;

        [SerializeField]
        [Tooltip("The mathematics function used to get a elasticity coefficient from two materials with separate coefficients; Quadratic recommended")]
        public PlanetariaPhysicMaterialCombine elasticity_combine = PlanetariaPhysicMaterialCombine.Quadratic;

        [SerializeField]
        [Tooltip("The mathematics function used to get a friction coefficient from two materials with separate coefficients; Geometric recommended")]
        public PlanetariaPhysicMaterialCombine friction_combine = PlanetariaPhysicMaterialCombine.Geometric;
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