using UnityEngine;

public class Tuk : MonoBehaviour
{
    [Header("Speed & Power")]
    public float maxSpeed = 10f;
    public float acceleration = 10f;
    public float reverseSpeed = 5f;

    [Header("Electric Feel")]
    public AnimationCurve accelerationCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    public float torque = 15f;

    [Header("Steering")]
    public float turnSpeed = 70f;
    public float driftFactor = 0.92f;

    [Header("Input Smoothing")]
    public float inputSmoothTime = 0.1f;
    public float deadzone = 0.1f;

    [Header("Battery")]
    public float battery = 100f;
    public float drainRate = 2f;

    [Header("Mobile Input (optional)")]
    public float mobileThrottle;
    public float mobileSteering;

    float moveInput;
    float turnInput;

    float smoothMove;
    float smoothTurn;
    float moveVelocity;
    float turnVelocity;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        GetInput();
        SmoothInput();
    }

    void FixedUpdate()
    {
        Move();
        Steer();
        ApplyDrift();
        DrainBattery();
    }

    void GetInput()
    {
        // Keyboard + arrows + controller
        moveInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");

        // Mobile override
        moveInput += mobileThrottle;
        turnInput += mobileSteering;

        // Deadzone
        if (Mathf.Abs(moveInput) < deadzone) moveInput = 0;
        if (Mathf.Abs(turnInput) < deadzone) turnInput = 0;
    }

    void SmoothInput()
    {
        smoothMove = Mathf.SmoothDamp(smoothMove, moveInput, ref moveVelocity, inputSmoothTime);
        smoothTurn = Mathf.SmoothDamp(smoothTurn, turnInput, ref turnVelocity, inputSmoothTime);
    }

    void Move()
    {
        if (battery <= 0) return;

        float speed = rb.linearVelocity.magnitude;
        float speedPercent = speed / maxSpeed;
        float curve = accelerationCurve.Evaluate(speedPercent);

        Vector3 forward = transform.forward;

        // 🚗 FORWARD
        if (smoothMove > 0.1f)
        {
            if (speed < maxSpeed)
            {
                rb.AddForce(forward * smoothMove * acceleration * torque * curve, ForceMode.Acceleration);
            }
        }

        // 🔙 REVERSE (FIXED)
        else if (smoothMove < -0.1f)
        {
            rb.AddForce(-forward * Mathf.Abs(smoothMove) * reverseSpeed * 10f, ForceMode.Acceleration);

            // Helps overcome forward momentum
            rb.linearVelocity *= 0.98f;
        }

        // 🛑 BRAKE
        else
        {
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, 3f * Time.fixedDeltaTime);
        }
    }

    void Steer()
    {
        float speedFactor = rb.linearVelocity.magnitude / maxSpeed;

        // 🛺 3-wheel unstable feel
        float stability = Mathf.Lerp(1f, 0.6f, speedFactor);

        float turn = smoothTurn * turnSpeed * stability * Time.fixedDeltaTime;

        Quaternion rot = Quaternion.Euler(0, turn, 0);
        rb.MoveRotation(rb.rotation * rot);
    }

    void ApplyDrift()
    {
        Vector3 vel = rb.linearVelocity;

        Vector3 forward = transform.forward * Vector3.Dot(vel, transform.forward);
        Vector3 sideways = transform.right * Vector3.Dot(vel, transform.right);

        rb.linearVelocity = forward + sideways * driftFactor;
    }

    void DrainBattery()
    {
        if (rb.linearVelocity.magnitude > 0.5f)
        {
            battery -= drainRate * Time.fixedDeltaTime;
            battery = Mathf.Clamp(battery, 0, 100);
        }
    }

    // 📱 Mobile UI hooks
    public void SetThrottle(float value)
    {
        mobileThrottle = value;
    }

    public void SetSteering(float value)
    {
        mobileSteering = value;
    }
}