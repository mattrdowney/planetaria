using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Planetaria
{
    /// <summary>
    /// Mutable Struct! - A wrapper class that encapsulates a reference to a GameObject (WARNING: this class invalidates typical immutable struct rules)
    /// </summary>
    [Serializable]
    public struct PlanetariaGameObject // TODO: test this... a lot
    {
        // Properties
        public bool activeInHierarchy // mostly boilerplate code
        {
            get { return internal_game_object.activeInHierarchy; }
        }

        public bool activeSelf
        {
            get { return internal_game_object.activeSelf; }
        }

        public PlanetariaGameObject gameObject
        {
            get { return this; }
        }

        public HideFlags hideFlags
        {
            get { return internal_game_object.hideFlags; }
            set { internal_game_object.hideFlags = value; }
        }

        public GameObject internal_game_object
        {
            get
            {
                if (initialized)
                {
                    return game_object_variable;
                }
                game_object_variable = new GameObject();
                Debug.LogError("Happening");
                initialized = true;
                return game_object_variable;
            }
        }

        public int layer
        {
            get { return internal_game_object.layer; }
            set { internal_game_object.layer = value; }
        }

        public string name
        {
            get { return internal_game_object.name; }
            set { internal_game_object.name = value; }
        }

        public Scene scene
        {
            get { return internal_game_object.scene; }
        }

        public string tag
        {
            get { return internal_game_object.tag; }
            set { internal_game_object.tag = value; }
        }

        public PlanetariaTransform transform
        {
            get { return GetOrAddComponent<PlanetariaTransform>(); }
        }

        // Constructors
        public PlanetariaGameObject(string name) // TODO: test for empty (I think it's gonna set GameObject to null, which would be useless)
        {
            game_object_variable = new GameObject(name);
            initialized = true;
        }

        public PlanetariaGameObject(GameObject game_object)
        {
            game_object_variable = game_object;
            initialized = true;
        }

        public static implicit operator PlanetariaGameObject(GameObject game_object)
        {
            return new PlanetariaGameObject(game_object);
        }

        // Public Methods
        public Subtype AddComponent<Subtype>() where Subtype : PlanetariaComponent
        {
            return internal_game_object.AddComponent<Subtype>();
        }

        public void BroadcastMessage(string message_name, object parameter = null, SendMessageOptions options = SendMessageOptions.RequireReceiver)
        {
            internal_game_object.BroadcastMessage(message_name, parameter, options);
        }

        public Subtype GetComponent<Subtype>() where Subtype : PlanetariaComponent
        {
            return internal_game_object.GetComponent<Subtype>();
        }

        public Subtype GetComponentInChildren<Subtype>(bool include_inactive = false) where Subtype : PlanetariaComponent
        {
            return internal_game_object.GetComponentInChildren<Subtype>(include_inactive);
        }

        public Subtype GetComponentInParent<Subtype>() where Subtype : PlanetariaComponent
        {
            return internal_game_object.GetComponentInParent<Subtype>();
        }

        public static explicit operator PlanetariaGameObject(PlanetariaComponent component)
        {
            MonoBehaviour unity_base = component;
            return new PlanetariaGameObject(unity_base.gameObject);
        }

        /// <summary>
        /// Mutator - Gets the attached PlanetariaComponent if it exists; otherwise it adds and returns it.
        /// </summary>
        /// <typeparam name="Subtype">The type of the PlanetariaComponent to be fetched.</typeparam>
        /// <returns>The found or newly added PlanetariaComponent.</returns>
        public Subtype GetOrAddComponent<Subtype>() where Subtype : PlanetariaComponent
        {
            optional<Subtype> result = GetComponent<Subtype>();
            if (!result.exists)
            {
                result = AddComponent<Subtype>();
            }
            return result.data;
        }

        public Subtype[] GetComponents<Subtype>() where Subtype : PlanetariaComponent
        {
            return internal_game_object.GetComponents<Subtype>();
        }

        public Subtype[] GetComponentsInChildren<Subtype>(bool include_inactive = false) where Subtype : PlanetariaComponent
        {
            return internal_game_object.GetComponentsInChildren<Subtype>(include_inactive);
        }

        public Subtype[] GetComponentsInParent<Subtype>() where Subtype : PlanetariaComponent
        {
            return internal_game_object.GetComponentsInParent<Subtype>();
        }

        public int GetInstanceID()
        {
            return internal_game_object.GetInstanceID();
        }

        public void SetActive(bool is_active)
        {
            internal_game_object.SetActive(is_active);
        }

        public void SendMessage(string message_name, object parameter = null, SendMessageOptions options = SendMessageOptions.RequireReceiver)
        {
            internal_game_object.SendMessage(message_name, parameter, options);
        }

        public void SendMessageUpwards(string message_name, object parameter = null, SendMessageOptions options = SendMessageOptions.RequireReceiver)
        {
            internal_game_object.SendMessageUpwards(message_name, parameter, options);
        }

        public override string ToString()
        {
            return internal_game_object.ToString();
        }

        // Static Methods
        public static void Destroy(PlanetariaGameObject game_object, float time_delay = 0)
        {
            GameObject.Destroy(game_object.internal_game_object, time_delay);
        }

        public static void DestroyImmediate(PlanetariaGameObject game_object, bool allow_destroying_assets = false)
        {
            GameObject.DestroyImmediate(game_object.internal_game_object, allow_destroying_assets);
        }

        public static void DontDestroyOnLoad(UnityEngine.Object saved_object)
        {
            GameObject.DontDestroyOnLoad(saved_object);
        }

        public static void DontDestroyOnLoad(PlanetariaGameObject game_object)
        {
            GameObject.DontDestroyOnLoad(game_object.internal_game_object);
        }

        public static PlanetariaGameObject Find(string name)
        {
            return new PlanetariaGameObject(GameObject.Find(name));
        }

        public static PlanetariaGameObject[] FindGameObjectsWithTag(string tag)
        {
            GameObject[] game_objects = GameObject.FindGameObjectsWithTag(tag);
            PlanetariaGameObject[] result = new PlanetariaGameObject[game_objects.Length];
            for (int index = 0; index < game_objects.Length; ++index)
            {
                result[index] = new PlanetariaGameObject(game_objects[index]);
            }
            return result;
        }

        public static UnityEngine.Object FindObjectOfType(Type type)
        {
            return GameObject.FindObjectOfType(type);
        }

        public static UnityEngine.Object[] FindObjectsOfType(Type type)
        {
            return GameObject.FindObjectsOfType(type);
        }

        public static PlanetariaGameObject FindWithTag(string tag)
        {
            return new PlanetariaGameObject(GameObject.FindWithTag(tag));
        }

        public static PlanetariaGameObject Instantiate(GameObject template, Transform parent = null)
        {
            GameObject game_object = GameObject.Instantiate(template, parent);
            return new PlanetariaGameObject(game_object);
        }

        public static PlanetariaGameObject Instantiate(GameObject template, Vector3 position)
        {
            GameObject game_object = GameObject.Instantiate(template, Vector3.zero, Quaternion.LookRotation(position, Vector3.up));
            return new PlanetariaGameObject(game_object);
        }

        public static PlanetariaGameObject Instantiate(GameObject template, Vector3 position, Vector3 up, Transform parent = null)
        {
            GameObject game_object = GameObject.Instantiate(template, Vector3.zero, Quaternion.LookRotation(position, up), parent);
            return new PlanetariaGameObject(game_object);
        }

        // Operators
        public static implicit operator bool(PlanetariaGameObject game_object)
        {
            return game_object.initialized && game_object.internal_game_object; // TODO: make sure this works with nulls and Unity nulls (destroyed objects)
        }

        [SerializeField] [HideInInspector] private bool initialized;
        [SerializeField] private GameObject game_object_variable; // TODO: re-evaluate [HideInInspector] and [SerializeField] and public/private // TODO: set initialized when this is set in inspector
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