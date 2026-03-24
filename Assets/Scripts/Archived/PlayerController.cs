using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float horiInput, vertInput;

    public float moveSpeed;

    public float tilt;
    public float tiltSpeed;
    Vector3 tiltAngle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        horiInput = Input.GetAxis("Horizontal");
        vertInput = Input.GetAxis("Vertical");

        HandleTilting();
        ClampToScreen();
    }

    void FixedUpdate()
    {
        Movement();  
    }

    void Movement()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        Vector3 movement = new Vector3(horiInput, vertInput, 0);
        transform.localPosition += new Vector3(movement.x, movement.y, movement.z) * moveSpeed * Time.deltaTime;
    }

    void HandleTilting()
    {
        TiltZ(horiInput);
        TiltX(vertInput);
    }

    void TiltZ(float axis) // tilt L + R
    {
        Vector3 targetEuAng = transform.localEulerAngles;

        transform.localEulerAngles = new Vector3(targetEuAng.x,
            Mathf.LerpAngle(targetEuAng.y, axis * tilt, tiltSpeed),
            Mathf.LerpAngle(targetEuAng.z, -axis * tilt, tiltSpeed)
        );
    }

    void TiltX(float axis) // tilt U + D
    {
        Vector3 targetEuAng = transform.localEulerAngles;

        transform.localEulerAngles = new Vector3(Mathf.LerpAngle(targetEuAng.x, -axis * tilt, tiltSpeed),
            targetEuAng.y,
            targetEuAng.z
        );
    }

    void ClampToScreen()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp01(pos.x);
        pos.y = Mathf.Clamp01(pos.y);
        transform.position = Camera.main.ViewportToWorldPoint(pos);
    }
}
