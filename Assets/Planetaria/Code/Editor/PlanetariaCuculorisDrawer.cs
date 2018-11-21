using UnityEngine;
using UnityEditor;

namespace Planetaria
{
    /// <summary>
	///
    /// </summary>
    [CustomPropertyDrawer(typeof(PlanetariaCuculoris))]
    public class PlanetariaCuculorisDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            int previous_indentation = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            Rect texture_rectangle = new Rect(position.x, position.y, position.width, position.height);
            EditorGUI.PropertyField(texture_rectangle, property.FindPropertyRelative("one_dimensional_cuculoris"), GUIContent.none);
            EditorGUI.indentLevel = previous_indentation;

            EditorGUI.EndProperty();
        }
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