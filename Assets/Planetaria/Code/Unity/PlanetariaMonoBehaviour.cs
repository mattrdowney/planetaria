using UnityEngine;

public abstract class PlanetariaMonoBehaviour : MonoBehaviour
{
    public new PlanetariaTransform transform;

    protected delegate void ActionDelegate();
    protected delegate void CollisionDelegate(BlockCollision block_information);
    protected delegate void TriggerDelegate(Field field_information);

    protected optional<ActionDelegate> on_first_exists = null;
    protected optional<ActionDelegate> on_time_zero = null; // XXX: use at your own risk
    protected optional<ActionDelegate> on_every_frame = null;

    protected optional<CollisionDelegate> on_block_enter = null;
    protected optional<CollisionDelegate> on_block_exit = null;
    protected optional<CollisionDelegate> on_block_stay = null;
    
    protected optional<TriggerDelegate> on_field_enter = null;
    protected optional<TriggerDelegate> on_field_exit = null;
    protected optional<TriggerDelegate> on_field_stay = null;

    protected abstract void Awake();
    protected abstract void Start();
    protected abstract void Update();
    protected abstract void LateUpdate();
    protected abstract void FixedUpdate();

    protected abstract void OnTriggerEnter(Collider collider);
    protected abstract void OnTriggerStay(Collider collider);
    protected abstract void OnTriggerExit(Collider collider);

    protected abstract void OnCollisionEnter(Collision collision);
    protected abstract void OnCollisionStay(Collision collision);
    protected abstract void OnCollisionExit(Collision collision);
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