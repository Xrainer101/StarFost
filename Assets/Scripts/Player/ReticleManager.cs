using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticleManager : MonoBehaviour
{   
    [Header("Close Reticle")]
    public RectTransform closeReticleUI; // Bigger reticle image
    public Transform closeAimPoint; // 3D empty ahead of the ship

    [Header("Far Reticle")]
    public RectTransform farReticleUI; // Smaller reticle image
    public Transform farAimPoint; // 3D empty farther ahead of the ship

    [Header("Reticle Drag Settings")]
    // To make the lasers more "accurate" to the reticle
    public float dragSpeed = 10f;

    // The "ghost point" that chases the real aimPoint
    private Vector3 farLaggedReticlePos;
    private Vector3 closeLaggedReticlePos;

    private Camera mainCam;

    // Start is called before the first frame update
    void Start()
    {
        // Get the main camera
        mainCam = Camera.main;

        // Start the ghost where the ship is looking at (straight ahead)
        closeLaggedReticlePos = closeAimPoint.position;
        farLaggedReticlePos = farAimPoint.position;
    }

    void Update()
    {
        // Slide ghost points toward the ship's actual aim points
        closeLaggedReticlePos = Vector3.Lerp(
            closeLaggedReticlePos,
            closeAimPoint.position,
            dragSpeed * Time.deltaTime
        );
        farLaggedReticlePos = Vector3.Lerp(
            farLaggedReticlePos,
            farAimPoint.position,
            dragSpeed * Time.deltaTime
        );

        // Convert the ghost points' position into 2D coordinates on the screen
        Vector3 closeScreenPos = mainCam.WorldToScreenPoint(closeLaggedReticlePos);
        Vector3 farScreenPos = mainCam.WorldToScreenPoint(farLaggedReticlePos);

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
