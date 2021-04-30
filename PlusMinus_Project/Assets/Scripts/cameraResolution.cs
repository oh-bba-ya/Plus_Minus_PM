using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraResolution : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        setupMainCamera();
    }
    private void setupMainCamera()
    {
        float targetWidthAspect = 19.2f;
        float targetHeightAspect = 10.8f;

        Camera maincamera = Camera.main;

        maincamera.aspect = targetWidthAspect / targetHeightAspect;

        float widthRatio = (float)Screen.width / targetWidthAspect;
        float heightRatio = (float)Screen.height / targetHeightAspect;

        float heightadd = ((widthRatio)/(heightRatio/100)-100)/ 200;
        float widthadd = ((heightRatio) / (widthRatio / 100) - 100) / 200;

        if (heightRatio > widthRatio)
            widthadd = 0.0f;
        else
            heightadd = 0.0f;

        maincamera.rect = new Rect(
            maincamera.rect.x + Mathf.Abs(widthadd),
            maincamera.rect.y + Mathf.Abs(heightadd),
            maincamera.rect.width + (widthadd * 2),
            maincamera.rect.height + (heightadd * 2));
    }
}
