public struct optional<Type>
{
    public bool exists
    {
        get { return exists_; }
    }

    public Type data
    {
        get
        {
            if (!exists)
            {
                throw new System.NullReferenceException();
            }
            return data_;
        }
        set { exists_ = true; data_ = value; }
    }

    public optional()
    {
        exists_ = false;
    }

    public optional(Type original)
	{
        exists_ = true;
		data_ = original;
    }

    public static implicit operator optional<Type>(Type original)
    {
        return new optional<Type>(original);
    } 

    public static bool operator ==(optional<Type> left, optional<Type> right)
    {
        bool bInequalExistence = (left.exists_ != right.exists_);
        bool bInequalValue = (left.exists_ && !left.data_.Equals(right.data_));
        bool bInequal = bInequalExistence || bInequalValue;
        return !bInequal;
    }

    public static bool operator !=(optional<Type> left, optional<Type> right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        if (!exists_)
        {
            return "nonexistent " + typeof(Type).Name;
        }

        return data_.ToString();
    }

    bool exists_;
    Type data_;
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