using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticleManager : MonoBehaviour
{
    private Camera mainCam;
    
    [Header("Close Reticle")]
    public RectTransform closeReticleUI; // Bigger reticle image
    public Transform closeAimPoint; // 3D empty ahead of the ship

    [Header("Far Reticle")]
    public RectTransform farReticleUI; // Smaller reticle image
    public Transform farAimPoint; // 3D empty farther ahead of the ship

    // Start is called before the first frame update
    void Start()
    {
        // Get the main camera
        mainCam = Camera.main;
    }

    void Update()
    {
        // Convert the empties' position into 2D coordinates on the screen
        Vector3 closeScreenPos = mainCam.WorldToScreenPoint(closeAimPoint.position);
        Vector3 farScreenPos = mainCam.WorldToScreenPoint(farAimPoint.position);

        if (closeScreenPos.z > 0)
        {
            // Move the crosshairs to the converted screen points
            closeReticleUI.position = closeScreenPos;
            farReticleUI.position = farScreenPos;

            // If the reticles are not visible
            if (!closeReticleUI.gameObject.activeSelf)
            {
                // Make them visible
                closeReticleUI.gameObject.SetActive(true);
                farReticleUI.gameObject.SetActive(true);
            }
        }
        else // If the empties are behind the camera
        {
            // If the reticles are visible
            if (closeReticleUI.gameObject.activeSelf)
            {
                // Make them not visible
                closeReticleUI.gameObject.SetActive(false);
                farReticleUI.gameObject.SetActive(false);
            }
        }
    }
}
