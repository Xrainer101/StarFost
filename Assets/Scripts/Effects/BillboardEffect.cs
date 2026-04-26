using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardEffect : MonoBehaviour
{
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void LateUpdate()
    {
        // Aligns explosion with camera
        transform.forward = mainCam.transform.forward;
    }
}
