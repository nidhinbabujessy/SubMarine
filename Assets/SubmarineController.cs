using TMPro;
using UnityEngine;

public class SubmarineController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float forwardSpeed = 10f;    // Forward/Backward Speed
    [SerializeField] private float turnSpeed = 30f;       // Turn Speed (Yaw)
    [SerializeField] private float verticalSpeed = 5f;    // Ascend/Descend Speed (Ballast Control)
    [SerializeField] private float strafeSpeed = 5f;      // Sideways Movement (Lateral)

    [Header("Buoyancy Settings")]
    [SerializeField] private float buoyancyForce = 9.8f;  // Force that makes submarine float
    [SerializeField] private float waterDrag = 0.99f;     // Slow down movement in water
    [SerializeField] private float waterLevel = 0f;       // Adjust based on ocean height

    [Header("Depth Sensor")]
    [SerializeField] private float seaSurfaceY = 61.51125f;
    [SerializeField] private TMP_Text depthText;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearDamping = 1.5f;
        rb.angularDamping = 2f;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }


    void FixedUpdate()
    {
        HandleMovement();
        ApplyBuoyancy();
        ApplyWaterResistance();
    }

    private void Update()
    {
        depthM();
    }
    void OnCollisionEnter(Collision collision)
    {
        rb.angularVelocity = Vector3.zero;
      

    }

   

 

    void HandleMovement()
    {
        // Move Forward/Backward (W/S or Up/Down Arrows)
        float move = Input.GetAxis("Vertical") * forwardSpeed;
        rb.AddForce(transform.forward * move, ForceMode.Acceleration);

        // Turn Left/Right (A/D or Left/Right Arrows)
        float turn = Input.GetAxis("Horizontal") * turnSpeed;
        rb.AddTorque(Vector3.up * turn, ForceMode.Acceleration);

        // Ascend/Descend (Q/E)
        if (Input.GetKey(KeyCode.Q))
        {
            // Only apply upward force if below max Y
            if (rb.position.y < seaSurfaceY)
                rb.AddForce(Vector3.up * verticalSpeed, ForceMode.Acceleration);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            rb.AddForce(Vector3.down * verticalSpeed, ForceMode.Acceleration);
        }

        // Strafe Left/Right (Shift + A/D)
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.A))
            rb.AddForce(-transform.right * strafeSpeed, ForceMode.Acceleration);
        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.D))
            rb.AddForce(transform.right * strafeSpeed, ForceMode.Acceleration);

        // Clamp the Y position if it exceeds the limit
        if (rb.position.y > seaSurfaceY)
        {
            Vector3 clampedPos = rb.position;
            clampedPos.y = seaSurfaceY;
            rb.position = clampedPos;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // Stop upward movement
        }
    }

    void ApplyBuoyancy()
    {
        if (transform.position.y < waterLevel)
        {
            rb.AddForce(Vector3.up * buoyancyForce, ForceMode.Force);
        }
    }

    void ApplyWaterResistance()
    {
        rb.linearVelocity *= waterDrag;
        rb.angularVelocity *= waterDrag;
    }

    #region depth

    void depthM()
    {
        float depth = Mathf.Max(0, seaSurfaceY - transform.position.y);
        depthText.text = "Depth: " + depth.ToString("F1") + " m";

        // Fog toggle based on depth
        RenderSettings.fog = depth > 1;

        // Set depth text color based on thresholds
        if (depth > 35)
        {
            depthText.color = Color.red;
        }
        else if (depth > 25)
        {
            depthText.color = Color.yellow;
        }
        else
        {
            depthText.color = Color.white; // default safe color
        }
    }

    #endregion
}
