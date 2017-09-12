using UnityEngine;

public class TemporaryArc : MonoBehaviour
{
    enum ConstructionState { SetBegin, SetRight, SetEnd }
    ConstructionState build_state;

    Vector3 begin_variable;
    Vector3 right_variable;
    Vector3 end_variable;

    public Vector3 begin
    {
        set
        {
            begin_variable = value;
        }
    }

    optional<Arc> arc;
}
