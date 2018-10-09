namespace Planetaria
{
    public struct ArcVisitor
    {
        /// <summary>
        /// Constructor - finds the ArcVisitor at a given position in a shape
        /// </summary>
        /// <param name="shape">The shape representing the list of Arc segments</param>
        /// <param name="index">The index of the referenced arc segment.</param>
        /// <returns>An ArcVisitor struct (for iterating across the arcs in a block).</returns>
        public static ArcVisitor arc_visitor(PlanetariaShape shape, int index)
        {
            return new ArcVisitor(shape, index);
        }

        /// <summary>
        /// Constructor - find the (new!) arc segment to the right of the current one.
        /// </summary>
        /// <returns>A (new!) ArcIndex struct referring to the arc segment to the right of current.</returns>
        public ArcVisitor right()
        {
            int right_index = (index_variable >= (shape_variable.Length - 1) ? 0 : (index_variable+1)); // cyclic behavior (wrap numbers from [0, size-1])
            return arc_visitor(shape_variable, right_index);
        }

        /// <summary>
        /// Constructor - find the (new!) arc segment to the left of the current one.
        /// </summary>
        /// <returns>A (new!) ArcIndex struct referring to the arc segment to the left of current.</returns>
        public ArcVisitor left()
        {
            int left_index = (index_variable <= 0 ? (shape_variable.Length - 1) : (index_variable-1)); // cyclic behavior (wrap numbers from [0, size-1])
            return arc_visitor(shape_variable, left_index);
        }

        public Arc arc
        {
            get
            {
                return shape_variable[index_variable];
            }
        }

        private ArcVisitor(PlanetariaShape shape, int index)
        {
            shape_variable = shape;
            index_variable = index;
        }

        public Arc this[int relative_index]
        {
            get
            {
                int absolute_index = index_variable + relative_index;
                absolute_index = absolute_index < 0 ? absolute_index + shape_variable.Length : absolute_index;
                absolute_index = absolute_index >= shape_variable.Length ? absolute_index - shape_variable.Length : absolute_index;
                return shape_variable[absolute_index];
            }
        }

        public static bool operator ==(ArcVisitor left, ArcVisitor right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ArcVisitor left, ArcVisitor right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(System.Object other)
        {
            bool same_type = other is ArcVisitor;
            if (!same_type)
            {
                return false;
            }
            ArcVisitor other_visitor = (ArcVisitor) other;
            bool same_data = this.index_variable == other_visitor.index_variable &&
                    this.shape_variable == other_visitor.shape_variable;

            return same_data;
        }

        public override int GetHashCode() 
        {
            return shape_variable.GetHashCode() ^ index_variable.GetHashCode();
        }

        private PlanetariaShape shape_variable;
        private int index_variable;
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