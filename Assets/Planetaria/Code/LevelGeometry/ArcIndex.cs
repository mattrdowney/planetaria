using System.Collections.Generic;

public struct ArcIndex
{
    /// <summary>
    /// Constructor - finds the ArcIndex at a given position in a list
    /// </summary>
    /// <param name="arc_list">The list of Arc segments</param>
    /// <param name="index">The index of the referenced arc segment.</param>
    /// <returns>An ArcIndex struct (for cyclic indices wrapping in range [0, last_index] inclusive).</returns>
    public ArcIndex arc_index(List<optional<Arc>> arc_list, int index)
    {
        return arc_index(arc_list.Count-1, index);
    }

    private ArcIndex arc_index(int last_index, int index)
    {
        ArcIndex result = new ArcIndex();
        last_index_variable = last_index;
        index_variable = index;
        return result;
    }

    /// <summary>
    /// Constructor - find the (new!) arc segment to the right of the current one.
    /// </summary>
    /// <returns>A (new!) ArcIndex struct referring to the arc segment to the right of current.</returns>
    public ArcIndex right()
    {
        int right_index = (index_variable >= last_index_variable ? 0 : (index_variable+1)); // cyclic behavior (wrap numbers from [0, last_index])
        return arc_index(last_index_variable, right_index);
    }

    /// <summary>
    /// Constructor - find the (new!) arc segment to the left of the current one.
    /// </summary>
    /// <returns>A (new!) ArcIndex struct referring to the arc segment to the left of current.</returns>
    public ArcIndex left()
    {
        int left_index = (index_variable <= 0 ? last_index_variable : (index_variable-1)); // cyclic behavior (wrap numbers from [0, last_index])
        return arc_index(last_index_variable, left_index);
    }

    public int index
    {
        get
        {
            return index_variable;
        }
    }

    private int last_index_variable;
    private int index_variable;
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