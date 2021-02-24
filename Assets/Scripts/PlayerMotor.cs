using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour {

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;

    /*
    private float cameraRotationX;
    private float currentCameraRotationX;
    [SerializeField]
    private float cameraRotationLimit = 85f;*/

    private Rigidbody rb;

    [SerializeField]
    private Camera cam;


    [Serializable]
    public class MouseLook {
        public float XSensitivity = 2f;
        public float YSensitivity = 2f;
        public bool clampVerticalRotation = true;
        public float MinimumX = -90F;
        public float MaximumX = 90F;
        public bool smooth;
        public float smoothTime = 5f;
        public bool lockCursor = true;


        private Quaternion m_CharacterTargetRot;
        private Quaternion m_CameraTargetRot;
        private bool m_cursorIsLocked = true;

        public void Init(Transform character, Transform camera) {
            m_CharacterTargetRot = character.localRotation;
            m_CameraTargetRot = camera.localRotation;
        }


        public void LookRotation(Transform character, Transform camera, float _yRot, float _xRot) {
            float yRot = Input.GetAxis("Mouse X") * XSensitivity;
            float xRot = Input.GetAxis("Mouse Y") * YSensitivity;

            m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
            m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

            if (clampVerticalRotation)
                m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

            if (smooth) {
                character.localRotation = Quaternion.Slerp(character.localRotation, m_CharacterTargetRot,
                    smoothTime * Time.deltaTime);
                camera.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraTargetRot,
                    smoothTime * Time.deltaTime);
            } else {
                character.localRotation = m_CharacterTargetRot;
                camera.localRotation = m_CameraTargetRot;
            }

            UpdateCursorLock();
        }

        public void SetCursorLock(bool value) {
            lockCursor = value;
            if (!lockCursor) {//we force unlock the cursor if the user disable the cursor locking helper
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public void UpdateCursorLock() {
            //if the user set "lockCursor" we check & properly lock the cursos
            if (lockCursor)
                InternalLockUpdate();
        }

        private void InternalLockUpdate() {
            if (Input.GetKeyUp(KeyCode.Escape)) {
                m_cursorIsLocked = false;
            } else if (Input.GetMouseButtonUp(0)) {
                m_cursorIsLocked = true;
            }

            if (m_cursorIsLocked) {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            } else if (!m_cursorIsLocked) {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        Quaternion ClampRotationAroundXAxis(Quaternion q) {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

            angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }

    }
    public MouseLook mouseLook = new MouseLook();

    private void Start() {
        rb = GetComponent<Rigidbody>();
        mouseLook.Init(transform, cam.transform);
    }
    private void FixedUpdate() {
        PerformMovement();
    }

    //Gets a movement vector
    public void Move(Vector3 _velocity) {
        velocity = _velocity;
    }

    public void Rotate(float _yRot, float _xRot) {
        mouseLook.LookRotation(transform, cam.transform, _yRot, _xRot);
    }

    

    // Perform movement based on velocity variable
    private void PerformMovement() {
        if(velocity != Vector3.zero) {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
            //rb.AddForce(velocity * Time.deltaTime);
        }
    }

    public void StopMovement() {
        rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
    }

    public void Jump(float force) {
        //drag?
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(new Vector3(0f, force, 0f), ForceMode.Impulse);
    }


}
