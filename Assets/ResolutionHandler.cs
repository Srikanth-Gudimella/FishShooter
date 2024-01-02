using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionHandler : MonoBehaviour
{
    public float targetAspectRatio = 192f / 108f;  // Set your target aspect ratio here

    void Start()
    {
        AdjustScale();
    }

    void AdjustScale()
    {
        float currentAspectRatio = (float)Screen.width / Screen.height;

        // Calculate the scaling factor
        float scaleFactor = currentAspectRatio / targetAspectRatio;

        // Apply the scaling factor to the GameObject's scale
        transform.localScale = new Vector3(transform.localScale.x * scaleFactor, transform.localScale.y, transform.localScale.z);
    }


}
