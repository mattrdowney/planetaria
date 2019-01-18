using System;
using UnityEngine;

namespace Planetaria
{
    //[RequireComponent(typeof(PlanetariaTransform))] // TODO: CHECK: How does this work with PlanetariaTransform? Infinite loop? // CONFIRMED: you can never remove PlanetariaTransforms
    public abstract class PlanetariaComponent : MonoBehaviour
    {
        protected virtual void Awake() { }

        protected virtual void OnDestroy() { } // TODO: OnDestroy() no longer needs to be declared as empty in other files.

        protected virtual void OnEnable() { }

        protected virtual void OnDisable() { }

        protected virtual void Reset() { }

        public new PlanetariaTransform transform
        {
            get { return gameObject.transform; }
        }

        public new PlanetariaGameObject gameObject
        {
            get
            {
                if (!initialized)
                {
                    game_object_variable = (PlanetariaGameObject) this;
                }
                return game_object_variable;
            }
        }

        public Subtype AddComponent<Subtype>() where Subtype : PlanetariaComponent // mostly boilerplate code
        {
            return gameObject.internal_game_object.AddComponent<Subtype>();
        }

        public new Subtype GetComponent<Subtype>() where Subtype : PlanetariaComponent
        {
            return gameObject.internal_game_object.GetComponent<Subtype>();
        }

        public new Subtype[] GetComponents<Subtype>() where Subtype : PlanetariaComponent
        {
            return gameObject.internal_game_object.GetComponents<Subtype>();
        }

        public Subtype GetOrAddComponent<Subtype>() where Subtype : PlanetariaComponent
        {
            return Miscellaneous.GetOrAddComponent<Subtype>(gameObject.internal_game_object);
        }

        public new Subtype GetComponentInChildren<Subtype>(bool include_inactive = false) where Subtype : PlanetariaComponent
        {
            return gameObject.internal_game_object.GetComponentInChildren<Subtype>(include_inactive);
        }

        public new Subtype GetComponentInParent<Subtype>() where Subtype : PlanetariaComponent
        {
            return gameObject.internal_game_object.GetComponentInParent<Subtype>();
        }

        public new Subtype[] GetComponentsInChildren<Subtype>(bool include_inactive = false) where Subtype : PlanetariaComponent
        {
            return gameObject.internal_game_object.GetComponentsInChildren<Subtype>(include_inactive);
        }

        public new Subtype[] GetComponentsInParent<Subtype>() where Subtype : PlanetariaComponent
        {
            return gameObject.internal_game_object.GetComponentsInParent<Subtype>();
        }

        [SerializeField] [HideInInspector] private bool initialized = false; // optional<PlanetariaGameObject> didn't work
        [SerializeField] [HideInInspector] private PlanetariaGameObject game_object_variable; // FIXME: bug when switching Component from GameObject1 to GameObject2 (reference improperly held).
    }
}

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.