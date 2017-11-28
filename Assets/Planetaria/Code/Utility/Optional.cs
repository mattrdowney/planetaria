namespace Planetaria
{
    [System.Serializable]
    public struct optional<Type> // https://stackoverflow.com/questions/16199227/optional-return-in-c-net
    {
        public bool exists
        {
            get
            {
                return exists_variable;
            }
        }

        public Type data
        {
            get
            {
                if (!exists)
                {
                    throw new System.NullReferenceException();
                }
                return data_variable;
            }
            set
            {
                exists_variable = !(value == null);
                data_variable = value;
            }
        }

        public optional(Type original) // FIXME: (?) This is far more expensive than it used to be
        {
            exists_variable = !(original == null);
            data_variable = original;

            // Unity overrides the definition of nullity, so check for Unity-specific "nulls"
            if (original != null && original.GetType().IsSubclassOf(typeof(UnityEngine.Object))) // https://blogs.unity3d.com/2014/05/16/custom-operator-should-we-keep-it/
            {
                exists_variable = !((original as UnityEngine.Object) == null);
            }
        }

        public static implicit operator optional<Type>(Type original)
        {
            return new optional<Type>(original);
        } 

        public static bool operator ==(optional<Type> left, optional<Type> right)
        {
            bool inequal_existance = (left.exists != right.exists);
            bool inequal_value = (left.exists && right.exists && !left.data.Equals(right.data));
            bool inequal = inequal_existance || inequal_value;
            return !inequal;
        }

        public static bool operator !=(optional<Type> left, optional<Type> right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            if (!exists)
            {
                return "nonexistent " + typeof(Type).Name;
            }

            return data.ToString();
        }
    
        [UnityEngine.SerializeField] private bool exists_variable;
        [UnityEngine.SerializeField] private Type data_variable;
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