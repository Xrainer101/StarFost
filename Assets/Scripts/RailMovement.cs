using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailMovement : MonoBehaviour
{
    // Transform tf;
    public float smoothSpeed = 0.1f;
    [SerializeField] Transform player;

    [SerializeField] float minPosX, maxPosX;
    [SerializeField] float minPosY, maxPosY;

    // Start is called before the first frame update
    void Start()
    {
        // tf = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 desiredPos = player.position;
        Vector3 smoothPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);

        transform.position = smoothPos;

        transform.position = new Vector3(
            Mathf.Clamp(smoothPos.x, minPosX, maxPosX),
            Mathf.Clamp(smoothPos.y, minPosY, maxPosY),
            transform.position.z
        );
    }

    private void FixedUpdate()
    {
        
    }
}
