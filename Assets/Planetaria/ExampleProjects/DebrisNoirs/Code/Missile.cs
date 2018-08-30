using UnityEngine;
using Planetaria;

public class Missile : PlanetariaMonoBehaviour
{
    protected override void OnConstruction()
    {
        rigidbody = GetComponent<PlanetariaRigidbody>();
        rigidbody.relative_velocity = Vector2.up * Mathf.PI;
        PlanetariaGameObject.Destroy(this.gameObject, 2f);
        transform.localScale = +0.01f;
        this.GetComponent<PlanetariaCollider>().is_field = true;
    }

    protected override void OnDestruction()
    {

    }

    [SerializeField] private new PlanetariaRigidbody rigidbody; // TODO: check how storing/caching rigidbodies works in MonoBehaviours (this needs the new keyword)
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