using UnityEngine;

public class CameraHACK : MonoBehaviour
{
    RegularPolygonCameraShutter shutter;

    float blink_duration = 0.3f;
    float blink_interval = 5f;

    void Start()
    {
        shutter = gameObject.AddComponent<RegularPolygonCameraShutter>() as RegularPolygonCameraShutter;
        shutter = gameObject.GetComponent<RegularPolygonCameraShutter>() as RegularPolygonCameraShutter;
        shutter.initialize(2, -1f);
        shutter.set(0f);
    }

    void LateUpdate()
    {
        float interpolation_factor = Time.time % blink_interval;

        if (interpolation_factor > blink_duration)
        {
            interpolation_factor = 0f;
        }
        else if (interpolation_factor > blink_duration/2)
        {
            interpolation_factor = 1 - (interpolation_factor-blink_duration/2)/(blink_duration/2);
        }
        else
        {
            interpolation_factor /= (blink_duration/2);
        }

        shutter.set(interpolation_factor);
    }
}