namespace Planetaria
{
    [System.Serializable]
    public class dirtyable<Type>
    {
        public bool dirty
        {
            get
            {
                return dirty_variable;
            }
        }

        public Type data
        {
            get
            {
                return data_variable;
            }
            set
            {
                dirty_variable = true;
                data_variable = value;
            }
        }

        public void clear()
        {
            dirty_variable = false;
        }

        public dirtyable()
        {
            dirty_variable = true;
            data_variable = default(Type);
        }

        public dirtyable(Type original)
        {
            dirty_variable = true;
            data_variable = original;
        }

        public static implicit operator dirtyable<Type>(Type original)
        {
            return new dirtyable<Type>(original);
        }

        public static implicit operator Type(dirtyable<Type> original)
        {
            return original.data;
        }

        [UnityEngine.SerializeField] private bool dirty_variable;
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