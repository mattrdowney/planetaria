using System.Collections.Generic;

public struct ArcVisitor
{
    /// <summary>
    /// Constructor - finds the ArcVisitor at a given position in a list
    /// </summary>
    /// <param name="arc_list">The list of Arc segments</param>
    /// <param name="index">The index of the referenced arc segment.</param>
    /// <returns>An ArcVisitor struct (for iterating across the arcs in a block).</returns>
    public static ArcVisitor arc_visitor(List<optional<Arc>> arc_list, int index)
    {
        return new ArcVisitor(arc_list, index);
    }

    /// <summary>
    /// Constructor - find the (new!) arc segment to the right of the current one.
    /// </summary>
    /// <returns>A (new!) ArcIndex struct referring to the arc segment to the right of current.</returns>
    public ArcVisitor right()
    {
        int right_index = (index_variable >= (arc_list_variable.Count-1) ? 0 : (index_variable+1)); // cyclic behavior (wrap numbers from [0, size-1])
        UnityEngine.Debug.Log(right_index);
        return arc_visitor(arc_list_variable, right_index);
    }

    /// <summary>
    /// Constructor - find the (new!) arc segment to the left of the current one.
    /// </summary>
    /// <returns>A (new!) ArcIndex struct referring to the arc segment to the left of current.</returns>
    public ArcVisitor left()
    {
        int left_index = (index_variable <= 0 ? (arc_list_variable.Count-1) : (index_variable-1)); // cyclic behavior (wrap numbers from [0, size-1])
        UnityEngine.Debug.Log(left_index);
        return arc_visitor(arc_list_variable, left_index);
    }

    public optional<Arc> arc
    {
        get
        {
            return arc_list_variable[index_variable];
        }
    }

    private ArcVisitor(List<optional<Arc>> arc_list, int index)
    {
        arc_list_variable = arc_list;
        index_variable = index;
    }

    public static bool operator ==(ArcVisitor left_hand_side, ArcVisitor right_hand_side)
    {
        return left_hand_side.index_variable == right_hand_side.index_variable &&
                left_hand_side.arc_list_variable == right_hand_side.arc_list_variable;
    }

    public static bool operator !=(ArcVisitor left_hand_side, ArcVisitor right_hand_side)
    {
        return !(left_hand_side == right_hand_side);
    }

    private List<optional<Arc>> arc_list_variable;
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