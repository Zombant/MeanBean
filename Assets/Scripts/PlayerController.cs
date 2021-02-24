using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour {

    [Header("Movement")]
    [SerializeField] private float walkingSpeed = 5f;
    [SerializeField] private float runningSpeed = 8f;
    [SerializeField] private float airControlMultiplier = 0.05f;
    [SerializeField] private KeyCode runKey = KeyCode.LeftShift;
    [SerializeField] private float jumpForce = 5f;

    private float currentSpeed = 5;


    [Header("Ground Collision")]
    [SerializeField] private float sphereRadiusOffset;
    [SerializeField] private float groundDistance;
    public LayerMask whatIsGround;
    [SerializeField] private bool isGrounded;


    private PlayerMotor motor;
    private CapsuleCollider playerCollider;
    private Rigidbody playerRigidBody;
    private PlayerShoot playerShoot;

    


    private void Start() {
        motor = GetComponent<PlayerMotor>();
        playerCollider = GetComponent<CapsuleCollider>();
        playerRigidBody = GetComponent<Rigidbody>();
        playerShoot = GetComponent<PlayerShoot>();
    }

    private void Update() {

        UpdateGrounded();
        UpdateSpeed();
        
        // Calculate movement velocity as a 3D vector
        float xMov = Input.GetAxisRaw("Horizontal");
        float zMov = Input.GetAxisRaw("Vertical");
        Vector3 movHorizontal = transform.right * xMov;
        Vector3 movVertical = transform.forward * zMov;
        //Final movement vector
        Vector3 velocity = (movHorizontal + movVertical).normalized * currentSpeed; // TODO: Time.deltaTime
        //Apply movement
        motor.Move(velocity);

        if (velocity.magnitude == 0) motor.StopMovement();


        float yRot = Input.GetAxisRaw("Mouse X");
        float xRot = Input.GetAxisRaw("Mouse Y");
        motor.Rotate(yRot, xRot);

        if (Input.GetButtonDown("Jump") && isGrounded) {
            isGrounded = false;
            motor.Jump(jumpForce);
            
        }


    }

    private void UpdateGrounded() {
        RaycastHit hitInfo;
        //ground distance is best at 0.05
        if(Physics.SphereCast(transform.position, playerCollider.radius * (1.0f - sphereRadiusOffset), Vector3.down, out hitInfo, (playerCollider.height/2 - playerCollider.radius) + groundDistance, whatIsGround)) {
            isGrounded = true; 
        } else {
            isGrounded = false;
        }
    }

    private void UpdateSpeed() {
        if (Input.GetKey(runKey)) {
            currentSpeed = runningSpeed;
        }else {
            currentSpeed = walkingSpeed;
            
        }

        if (!isGrounded) {
            currentSpeed *= airControlMultiplier;
        }
    }

    public void ChangeWalkingSpeed(float _speed) {
        walkingSpeed = _speed;
    }
    public void ChangeRunningSpeed(float _speed) {
        runningSpeed = _speed;
    }


}
