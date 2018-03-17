using System.Configuration.Assemblies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace PirateGame.Entity
{
    /// <summary>
    /// Basic entity used on anything that has a rigidbody
    /// 
    /// Includes
    /// - Forces
    /// - Grounded Checker
    /// - Max speed
    /// - Collision and trigger detection
    /// - (Needs) Collision sounds
    /// </summary>

	[RequireComponent(typeof(Collider))]
    public class Entity : NetworkingBase
    {

        [Header("Entity")]
	    public float drag;

        public bool hasCustomGravity;
        [ShowIf("hasCustomGravity")]
        public float gravity;
        public bool hasMaxSpeed;
        [ShowIf("hasMaxSpeed")]
        public float maxSpeed;

        public ForceMode forceMode;

        [Header("Collision")]
	    public bool showDebugCollision = true;
	    [ShowIf("showDebugCollision", true)]
        public Color debugCollisionColor  = Color.white;
		public bool checkGrounded;
	    [ShowIf("checkGrounded", true)]
	    public EntityEnums.GroundedCollisionDetection gCollisionDetection;
	    [ShowIf("checkGrounded", true)]
        public LayerMask gCollisionLayerMask;
	    [ShowIf("checkGrounded", true)]
	    public bool gCustomOrigin;
	    [ShowIf("checkGrounded", true)]
	    [ShowIf("gCustomOrigin", true)]
	    public Transform gOrigin;
	    [ShowIf("checkGrounded", true)]
	    [HideIf("gCollisionDetection", EntityEnums.GroundedCollisionDetection.Box)]
	    [HideIf("gCollisionDetection", EntityEnums.GroundedCollisionDetection.Sphere)]
	    [HideIf("gCollisionDetection", EntityEnums.GroundedCollisionDetection.Capsule)]
	    public Vector3 gDirectionVector;
	    [ShowIf("checkGrounded", true)]
	    public Vector3 gOffset;
	    [ShowIf("checkGrounded", true)]
	    [HideIf("gCollisionDetection", EntityEnums.GroundedCollisionDetection.Box)]
	    [HideIf("gCollisionDetection", EntityEnums.GroundedCollisionDetection.Rays)]
	    [HideIf("gCollisionDetection", EntityEnums.GroundedCollisionDetection.Rays)]
        public float gRadius;
	    [ShowIf("checkGrounded", true)]
	    [HideIf("gCollisionDetection", EntityEnums.GroundedCollisionDetection.Box)]
	    [HideIf("gCollisionDetection", EntityEnums.GroundedCollisionDetection.Sphere)]
	    [HideIf("gCollisionDetection", EntityEnums.GroundedCollisionDetection.Ray)]
	    [HideIf("gCollisionDetection", EntityEnums.GroundedCollisionDetection.Rays)]
        public float gHeight;
	    [ShowIf("checkGrounded", true)]
	    [HideIf("gCollisionDetection", EntityEnums.GroundedCollisionDetection.Capsule)]
	    [HideIf("gCollisionDetection", EntityEnums.GroundedCollisionDetection.Sphere)]
	    [HideIf("gCollisionDetection", EntityEnums.GroundedCollisionDetection.Ray)]
	    [HideIf("gCollisionDetection", EntityEnums.GroundedCollisionDetection.Rays)]
        public Vector3 gBoxSize;
	    [ShowIf("checkGrounded", true)]
        [HideIf("gCollisionDetection", EntityEnums.GroundedCollisionDetection.Capsule)]
        [HideIf("gCollisionDetection", EntityEnums.GroundedCollisionDetection.Sphere)]
        [HideIf("gCollisionDetection", EntityEnums.GroundedCollisionDetection.Box)]
        public float gMaxDistance;

        [Header("Debug Entity Variables")]
        public List<Collider> collisions = new List<Collider>();
        public bool inCollision
		{
			get
			{
				return collisions.Count > 0;
			}
		}
		
		public List<Collider> triggers = new List<Collider>();
		public bool inTrigger
        {
			get
			{
				return triggers.Count > 0;
			}
		}

        public float verticalSpeed;

        public float velocityPlanarMagnitude;
        public float velocityMagnitude;
	    public Vector3 velocityVector;
	    public Vector3 velocityVectorDirection;
	    public Vector3 velocityVectorDirectionInverse;

	    [ShowIf("checkGrounded", true)]
	    public bool grounded
	    {
	        get { return groundedColliders != null && groundedColliders.Length > 0; }
	    }
	    [ShowIf("checkGrounded", true)]
		public Collider[] groundedColliders;

        #region Private Variables

	    private Vector3 _fakeAngularVelocity;

        private bool _characterController;


        #endregion

        #region Unity Methods

        public void Awake()
        {
            _characterController = characterController != null;
        }

        public void Start()
	    {
	        
	    }

	    public void Update()
	    {
	        CheckGroundedCollision();


            UpdateSpeedVariables();

	        LockSpeed();
	    }

	    public void LateUpdate()
	    {

        }

        public void FixedUpdate()
	    {
	        ApplyFakeAngularVelocity();

	        ApplyDrag();

		    ApplyGravity();
            
		    MoveWithGround();
	    }

        #endregion

        #region Collision Detection
        void OnCollisionEnter(Collision collision)
		{				
			collisions.Add(collision.collider);
		    OnCollisionEnter();

		}
        public virtual void OnCollisionEnter() { }
		
		void OnCollisionExit(Collision collision)
		{
			collisions.Remove(collision.collider);
		    OnCollisionExit();
        }
        public virtual void OnCollisionExit() { }

        void OnTriggerEnter(Collider collider)
		{
			triggers.Add(collider);
		    OnTriggerEnter();
        }
        public virtual void OnTriggerEnter() { }

        void OnTriggerExit(Collider collider)
		{
			triggers.Remove(collider);
		    OnTriggerExit();
        }
        public virtual void OnTriggerExit() { }
        #endregion

        #region Ground Collision Detection

        void CheckGroundedCollision()
	    {
	        groundedColliders = null;
            if (gCollisionDetection == EntityEnums.GroundedCollisionDetection.Capsule)
	        {
	            Vector3 originP1 = (gCustomOrigin ? gOrigin.position : transform.position);
	            originP1 += gOffset;
	            originP1.y -= (gHeight / 2);
	            originP1.y += (gRadius);
	            Vector3 originP2 = (gCustomOrigin ? gOrigin.position : transform.position);
	            originP2 += gOffset;
	            originP2.y += (gHeight / 2);
	            originP2.y -= (gRadius);

	            groundedColliders = Physics.OverlapCapsule(originP1, originP2, gRadius, gCollisionLayerMask);
	        }

	        if (gCollisionDetection == EntityEnums.GroundedCollisionDetection.Sphere)
	        {
	            Vector3 origin = (gCustomOrigin ? gOrigin.position : transform.position);
	            origin += gOffset;

	            groundedColliders = Physics.OverlapSphere(origin, gRadius, gCollisionLayerMask);
	        }

	        if (gCollisionDetection == EntityEnums.GroundedCollisionDetection.Box)
	        {
	            Vector3 origin = (gCustomOrigin ? gOrigin.position : transform.position);
	            origin += gOffset;

	            groundedColliders = Physics.OverlapBox(origin, gBoxSize, Quaternion.identity, gCollisionLayerMask);
	        }

	        if (gCollisionDetection == EntityEnums.GroundedCollisionDetection.Ray)
	        {
	            RaycastHit[] hit;
	            Vector3 origin = (gCustomOrigin ? gOrigin.position : transform.position);
	            origin += gOffset;
	            Debug.DrawRay(origin, gDirectionVector * gMaxDistance, debugCollisionColor);

	            hit = Physics.RaycastAll(origin, gDirectionVector, gMaxDistance, gCollisionLayerMask);
                if(hit.Length > 0)
	            {
	                groundedColliders = new Collider[hit.Length];
	                for (int i = 0; i < hit.Length; i++)
	                {
	                    groundedColliders[i] = hit[i].collider;
	                }
	            }
	        }
	    }

        #endregion

        #region Movement Methods

	    public void AddForce(Vector3 direction, float amount, ForceMode forceMode)
	    {
	        if (_characterController)
	        {
	            return;
	        }
            rigidbody.AddForce(direction * amount, forceMode);
	    }

	    public void AddForce(Vector3 direction, float amount)
	    {
	        AddForce(direction, amount, forceMode);
	    }

	    public void AddForwardForce(Vector3 direction, float amount)
	    {
	        AddForce(transform.TransformDirection(direction), amount);
	    }

	    public void AddForwardForce(Vector3 direction, float amount, ForceMode forceMode)
	    {
	        AddForce(transform.TransformDirection(direction), amount, forceMode);
	    }

	    public void RotateTowards(Vector3 rotation, float speed)
	    {
	        Vector3 dir = Vector3.RotateTowards(transform.forward, rotation, speed * Time.deltaTime, 0.0F);
	        transform.eulerAngles = dir;
	    }

	    public void SetVelocity(Vector3 velocity)
	    {
	        if (_characterController)
	        {
	            return;
	        }

            rigidbody.velocity = velocity;
	    }

	    public void SetVelocityForward(Vector3 velocity)
	    {
	        SetVelocity(transform.TransformDirection(velocity));
	    }

	    public void SetAngularVelocity(Vector3 angularVelocity)
	    {
            _fakeAngularVelocity = Vector3.zero;

	        if (_characterController || (rigidbody.constraints == RigidbodyConstraints.FreezeRotationY || rigidbody.constraints == RigidbodyConstraints.FreezeRotation || rigidbody.constraints == RigidbodyConstraints.FreezeAll))
	        {
                // use fake angular velocity
	            _fakeAngularVelocity = angularVelocity;

	            return;
	        }
	        rigidbody.angularVelocity = angularVelocity;
	    }
	    
	    private Vector3 lastPosition;
	    private GameObject lastObject;
	    void MoveWithGround()
	    {
	    	if(grounded)
	    	{
	    		if(groundedColliders[0].gameObject != lastObject)
	    		{
	    			lastObject = groundedColliders[0].gameObject;
	    			lastPosition = lastObject.transform.position;
	    			return;
	    		}
	    		
	    		Vector3 difference = Vector3.zero;
	    		if(lastPosition != Vector3.zero)
	    		{
	    			difference = groundedColliders[0].transform.position - lastPosition;
	    		}
	    		
	    		transform.position += difference;
	    		
	    		lastPosition = groundedColliders[0].transform.position;
	    	}
	    	else
	    	{
	    		lastPosition = Vector3.zero;
	    		lastObject = null;
	    	}
	    }

        #endregion

        // Lock speed if the velocity exceeds the max speed
        void LockSpeed()
	    {
            if(!hasMaxSpeed)
                return;

	        if (velocityMagnitude > maxSpeed)
	        {
	            if (characterController)
	            {
	                return;
	            }
	            rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, maxSpeed);
	        }
	    }

        // Update the automatic variables
	    void UpdateSpeedVariables()
	    {
	        velocityMagnitude = (_characterController ? characterController.velocity.magnitude: rigidbody.velocity.magnitude);
            velocityVector = (_characterController ? characterController.velocity : rigidbody.velocity);
	        velocityPlanarMagnitude = new Vector3(velocityVector.x, 0, velocityVector.z).magnitude;
            velocityVectorDirection = transform.TransformDirection(velocityVector);
            velocityVectorDirectionInverse = transform.InverseTransformDirection(velocityVector);
        }

        // Add drag
	    void ApplyDrag()
	    {
	        if (_characterController)
	            return;

            Vector3 velocity = velocityVector;
	        velocity = Vector3.Lerp(velocity, Vector3.zero, drag * Time.deltaTime);
	        velocity.y = velocityVector.y;
	        rigidbody.velocity = velocity;
	    }

        // Apply gravity
        public virtual void ApplyGravity()
        {
            if (hasCustomGravity)
            {
                if (_characterController)
                {
                    if (grounded)
                    {
                        verticalSpeed = -1;
                    }

                    verticalSpeed -= gravity * Time.deltaTime;
                }
                else
                {
                    rigidbody.useGravity = false;
                    rigidbody.velocity = new Vector3(rigidbody.velocity.x, -gravity, rigidbody.velocity.z);
                }
            }
        }

        // Add fake angular velocity
        void ApplyFakeAngularVelocity()
	    {
	        transform.eulerAngles += _fakeAngularVelocity * Time.deltaTime;
	    }

        /*****************************************************
         * Debug
         *****************************************************/

	    public virtual void DebugLog(string message)
	    {
	        Debug.Log("[" + this.GetType().Name + " (" + gameObject.name + ")] " + message, gameObject);
	    }

        #region Debug Ground Collision Check
        public void OnDrawGizmos()
	    {
	        if (!showDebugCollision)
	            return;

            Gizmos.color = debugCollisionColor;

			if(!checkGrounded)
				return;
				
			if (gCollisionDetection == EntityEnums.GroundedCollisionDetection.Capsule)
	        {
	            Vector3 originP1 = (gCustomOrigin ? gOrigin.position : transform.position);
	            originP1 += gOffset;
	            originP1.y -= (gHeight / 2);
	            originP1.y += (gRadius);
                Vector3 originP2 = (gCustomOrigin ? gOrigin.position : transform.position);
	            originP2 += gOffset;
	            originP2.y += (gHeight / 2);
	            originP2.y -= (gRadius);


                Gizmos.DrawWireSphere(originP1, gRadius);
	            Gizmos.DrawWireSphere(originP2, gRadius);
            }

	        if (gCollisionDetection == EntityEnums.GroundedCollisionDetection.Sphere)
	        {
	            Vector3 origin = (gCustomOrigin ? gOrigin.position : transform.position);
	            origin += gOffset;

	            Gizmos.DrawWireSphere(origin, gRadius);

            }

            if (gCollisionDetection == EntityEnums.GroundedCollisionDetection.Box)
	        {
	            Vector3 origin = (gCustomOrigin ? gOrigin.position : transform.position);
	            origin += gOffset;

	            Gizmos.DrawWireCube(origin, gBoxSize);
            }

            if (gCollisionDetection == EntityEnums.GroundedCollisionDetection.Ray)
	        {
	            Vector3 origin = (gCustomOrigin ? gOrigin.position : transform.position);
	            origin += gOffset;
                Debug.DrawRay(origin, gDirectionVector * gMaxDistance, debugCollisionColor);
	        }

        }
        #endregion

    }
}