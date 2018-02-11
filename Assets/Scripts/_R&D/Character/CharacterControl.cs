using UnityEngine;

namespace PirateGame
{
    public class CharacterControl : MonoBehaviour
    {
        public enum CharacterState
        {
            Idle,
            Walking,
            Jogging,
            InAir
        }

        public CharacterState State;

        [Header("Idle")]
        public float IdleDamping = 0.1f;
        public float IdleTurnDamping = 0.2f;

        [Header("Walking")]
        public float WalkSpeed = 2;
        public float WalkDamping = 0.3f;
        public float WalkTurnDamping = 0.3f;

        [Header("Jogging")]
        public float JogSpeed = 6;
        public float JogDamping = 0.2f;
        public float JogTurnDamping = 0.2f;

        [Header("Air")]
        public float JumpHeight = 2;
        public float JumpInputTimer = 0.1f;
        //public float JumpGroundedTimer = 0.1f;
        public float InAirSpeed = 2;
        public float InAirDamping = 0.6f;
        public float InAirTurnDamping = 0.4f;
        public float Gravity = 15;

        [Header("References")]
        public CharacterController Controller;
        public Animator Animator;

        [Header("Other")]
        public LayerMask Mask;

        // EXPERIMENTAL
        public Transform Boat;

        private Vector3 lastPosition;
        private Vector3 localBoatPointA;
        private Vector3 localBoatPointB;
        //

        private bool jumpInput
        {
            set { jumpInputTimer = (value ? Time.time + jumpInputTimer : -1); }
            get { return Time.time < jumpInputTimer; }
        }

        private float jumpInputTimer;

        private float verticalInput;
        private float horizontalInput;
        private Vector3 input;

        private Vector3 velocity;
        private Vector3 smoothDampVelocity;
        private float lookAngle;
        private float desiredLookAngle;
        private float angularSmoothDampVelocity;

        private bool isGrounded;
        private bool wasGrounded;

#if UNITY_EDITOR
        protected void OnValidate()
        {
            if (Controller == null)
            {
                Controller = GetComponent<CharacterController>();

                if (Controller == null)
                {
                    Controller = gameObject.AddComponent<CharacterController>();
                    Controller.center = Vector3.up * .9f;
                    Controller.radius = 0.2f;
                    Controller.height = 1.8f;
                }
            }

            if (Animator == null)
                Animator = GetComponentInChildren<Animator>();
        }
#endif

        protected void Start()
        {
            velocity = smoothDampVelocity = Vector3.zero;
            lookAngle = desiredLookAngle = transform.localEulerAngles.y;
            angularSmoothDampVelocity = 0;

            lastPosition = transform.position;

            if (Boat != null)
            {
                localBoatPointA = Boat.InverseTransformPoint(transform.position);
                localBoatPointB = Boat.InverseTransformPoint(transform.position + transform.forward);
            }
        }

        private void GetInput()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                jumpInput = true;

            verticalInput = Input.GetAxisRaw("Vertical");
            horizontalInput = Input.GetAxisRaw("Horizontal");

            Vector3 cameraForward = Camera.main.transform.forward;
            cameraForward.y = 0;
            cameraForward.Normalize();

            Vector3 cameraRight = Camera.main.transform.right;
            cameraRight.y = 0;
            cameraRight.Normalize();

            input = Vector3.zero;
            input += cameraForward * verticalInput;
            input += cameraRight * horizontalInput;

            if (input.sqrMagnitude > 1)
                input.Normalize();
        }

        private void StateChange()
        {
            // Handle falling
            switch (State)
            {
                case CharacterState.Idle:
                case CharacterState.Walking:
                case CharacterState.Jogging:

                    if (!isGrounded)
                    {
                        State = CharacterState.InAir;
                        // FallStart ();
                        return;
                    }

                    break;
            }

            // Handle state changes
            switch (State)
            {
                case CharacterState.Idle:

                    if (input.magnitude > 0)
                    {
                        State = CharacterState.Jogging;
                        JoggingStart();
                        break;
                    }

                    /*if (jumpInput)
                    {
                        jumpInput = false;
                        State = CharacterState.InAir;
                        IdleJump();
                        break;
                    }*/

                    break;

                case CharacterState.Jogging:

                    if (input.magnitude > 0)
                    {
                        if (jumpInput)
                        {
                            jumpInput = false;
                            State = CharacterState.InAir;
                            JoggingJump();
                        }
                    }
                    else
                    {
                        State = CharacterState.Idle;
                        JoggingEnd();
                    }

                    break;

                case CharacterState.InAir:

                    if (isGrounded)
                    {
                        if (input.magnitude > 0)
                        {
                            // if walk input State -> Walking etc.

                            State = CharacterState.Jogging;
                            JoggingLand();
                        }
                        else
                        {
                            State = CharacterState.Idle;
                            IdleLand();
                        }
                    }

                    break;
            }
        }

        #region Idle

        private void IdleStart()
        {
            Animator.CrossFadeInFixedTime("Idle", 0.2f);
        }

        private void Idle()
        {
            float y = velocity.y;
            velocity.y = 0;
            velocity = Vector3.SmoothDamp(velocity, Vector3.zero, ref smoothDampVelocity, IdleDamping);
            velocity.y = (wasGrounded ? -10 : (y - Time.deltaTime * Gravity));

            /*float desiredAngle;

            //if (!isAiming)
            //{
                Quaternion lookRotation = Quaternion.LookRotation(velocity.magnitude > 0.2f ? velocity : transform.forward);
                desiredAngle = lookRotation.eulerAngles.y;
            //}
            //else
            //{
                //desiredAngle = Camera.main.transform.localEulerAngles.y;
            //}

            lookAngle = Mathf.SmoothDampAngle(lookAngle, desiredAngle, ref angularSmoothDampVelocity, IdleTurnDamping);
            transform.rotation = Quaternion.Euler(0, lookAngle, 0);*/
        }

        #endregion

        #region Jogging

        private void JoggingStart()
        {
            Animator.CrossFadeInFixedTime("JogStart", 0.2f);
        }

        private void Jogging()
        {
            float y = velocity.y;
            velocity.y = 0;
            velocity = Vector3.SmoothDamp(velocity, input * JogSpeed, ref smoothDampVelocity, JogDamping);
            velocity.y = (isGrounded ? -1f : (y - Time.deltaTime * Gravity));

            Quaternion lookRotation = Quaternion.LookRotation(velocity.magnitude > 0.2f ? velocity : transform.forward);
            float desiredAngle = lookRotation.eulerAngles.y;

            lookAngle = Mathf.SmoothDampAngle(lookAngle, desiredAngle, ref angularSmoothDampVelocity, JogTurnDamping);
            transform.rotation = Quaternion.Euler(0, lookAngle, 0);
        }

        private void JoggingEnd()
        {
            Animator.CrossFadeInFixedTime("JogEnd", 0.2f);
        }

        #endregion

        #region Air

        private void IdleJump()
        {
            Animator.CrossFadeInFixedTime("IdleJump", 0.2f);
            velocity.y = JumpForce(JumpHeight);
            isGrounded = false;
        }

        private void IdleLand()
        {
            Animator.CrossFadeInFixedTime("IdleLand", 0.2f);
        }

        private void JoggingJump()
        {
            Animator.CrossFadeInFixedTime("JogJump", 0.2f);
            velocity.y = JumpForce(JumpHeight);
            isGrounded = false;
        }

        private void JoggingLand()
        {
            Animator.CrossFadeInFixedTime("JogLand", 0.2f);
        }

        private void FallStart()
        {
            Animator.CrossFadeInFixedTime("Fall", 0.2f);
        }

        private void InAir()
        {
            float y = velocity.y;
            velocity.y = 0;
            velocity = Vector3.SmoothDamp(velocity, input * InAirSpeed, ref smoothDampVelocity, InAirDamping);
            velocity.y = y - Time.deltaTime * Gravity;

            Quaternion lookRotation = Quaternion.LookRotation(velocity);
            float desiredAngle = lookRotation.eulerAngles.y;

            lookAngle = Mathf.SmoothDampAngle(lookAngle, desiredAngle, ref angularSmoothDampVelocity, InAirTurnDamping);
            transform.rotation = Quaternion.Euler(0, lookAngle, 0);
        }

        #endregion

        protected void Update()
        {
            Raycasts();

            GetInput();

            StateChange();

            DoState();
        }

        private void DoState()
        {
            switch (State)
            {
                case CharacterState.Idle:
                    Idle();
                    break;

                case CharacterState.Jogging:
                    Jogging();
                    break;

                case CharacterState.InAir:
                    InAir();
                    break;
            }

            Vector3 move = Vector3.zero;

            // EXPERIMENTAL
            if (Boat != null)
            {
                Vector3 newWorldPoint = Boat.TransformPoint(localBoatPointA);
                
                move += newWorldPoint - lastPosition;
            }
            //

            move += velocity * Time.deltaTime;

            Controller.Move(move);

            lastPosition = transform.position;

            if (Boat != null)
            {
                localBoatPointA = Boat.InverseTransformPoint(transform.position);
            }
        }

        private void Raycasts()
        {
            wasGrounded = isGrounded;

            RaycastHit hit;
            Physics.SphereCast(transform.position + Vector3.up, 0.2f, Vector3.down, out hit, Mask);
            isGrounded = hit.distance < 1.01f;
        }

        private float JumpForce(float height)
        {
            return Mathf.Sqrt(2 * Gravity * height);
        }
    }
}