using UnityEngine;

public abstract class CameraStrategy : MonoBehaviour
{
    public abstract void Update();
    public abstract void OnShutterBlink();
}
