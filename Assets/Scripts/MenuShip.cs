using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuShip : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] GameObject shipModel;
    public float rotationSpeed = 1f;
    public float maxAngle = 25f;

    void Update()
    {
        transform.Translate(Vector3.forward * speed);
        float zRotation = Mathf.Sin(Time.time * rotationSpeed) * maxAngle;
        shipModel.transform.rotation = Quaternion.Euler(0,0,zRotation);
    }
    void FixedUpdate()
    {
        if(transform.position.z >= 300)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }
    }
}
