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
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(Collider))]
    public class Entity : Base
	{

        #region Public Variables
	    [FoldoutGroup("Public Variables")]
        public bool hasMaxSpeed;
        [ShowIf("hasMaxSpeed")]
	    [FoldoutGroup("Public Variables")]
        public float maxSpeed;
        [FoldoutGroup("Public Variables")]
        public ForceMode forceMode;

        #region Grounded Collision
        [FoldoutGroup("Public Variables/Collision")]
	    public bool showDebugCollision = true;

	    [FoldoutGroup("Public Variables/Collision")]
	    [ShowIf("showDebugCollision", true)]
        public Color debugCollisionColor  = Color.white;

        [FoldoutGroup("Public Variables/Collision/Grounded")]
	    public EntityEnums.GroundedCollisionDetection gCollisionDetection;

        [FoldoutGroup("Public Variables/Collision/Grounded")]
        public LayerMask gCollisionLayerMask;

        [FoldoutGroup("Public Variables/Collision/Grounded")]
	    public bool gCustomOrigin;
        
        [FoldoutGroup("Public Variables/Collision/Grounded")]
	    [ShowIf("gCustomOrigin", true)]
	    public Transform gOrigin;

	    [FoldoutGroup("Public Variables/Collision/Grounded")]
	    [HideIf("gCollisionDetection", EntityEnums.GroundedCollisionDetection.Box)]
	    [HideIf("gCollisionDetection", EntityEnums.GroundedCollisionDetection.Sphere)]
	    [HideIf("gCollisionDetection", EntityEnums.GroundedCollisionDetection.Capsule)]
	    public Vector3 gDirectionVector;

        [FoldoutGroup("Public Variables/Collision/Grounded")]
	    public Vector3 gOffset;

        [FoldoutGroup("Public Variables/Collision/Grounded")]
	    [HideIf("gCollisionDetection", EntityEnums.GroundedCollisionDetection.Box)]
	    [HideIf("gCollisionDetection", EntityEnums.GroundedCollisionDetection.Rays)]
	    [HideIf("gCollisionDetection", EntityEnums.GroundedCollisionDetection.Rays)]
        public float gRadius;

	    [FoldoutGroup("Public Variables/Collision/Grounded")]
	    [HideIf("gCollisionDetection", EntityEnums.GroundedCollisionDetection.Box)]
	    [HideIf("gCollisionDetection", EntityEnums.GroundedCollisionDetection.Sphere)]
	    [HideIf("gCollisionDetection", EntityEnums.GroundedCollisionDetection.Ray)]
	    [HideIf("gCollisionDetection", EntityEnums.GroundedCollisionDetection.Rays)]
        public float gHeight;

	    [FoldoutGroup("Public Variables/Collision/Grounded")]
	    [HideIf("gCollisionDetection", EntityEnums.GroundedCollisionDetection.Capsule)]
	    [HideIf("gCollisionDetection", EntityEnums.GroundedCollisionDetection.Sphere)]
	    [HideIf("gCollisionDetection", EntityEnums.GroundedCollisionDetection.Ray)]
	    [HideIf("gCollisionDetection", EntityEnums.GroundedCollisionDetection.Rays)]
        public Vector3 gBoxSize;

        [FoldoutGroup("Public Variables/Collision/Grounded")]
        [HideIf("gCollisionDetection", EntityEnums.GroundedCollisionDetection.Capsule)]
        [HideIf("gCollisionDetection", EntityEnums.GroundedCollisionDetection.Sphere)]
        [HideIf("gCollisionDetection", EntityEnums.GroundedCollisionDetection.Box)]
        public float gMaxDistance;

        #endregion

        #endregion

        #region Automatic Variables

        #region Collision
        [FoldoutGroup("Automatic Variables/Collision")]
        public bool inCollision
		{
			get
			{
				return collisions.Count > 0;
			}
		}
        [FoldoutGroup("Automatic Variables/Collision")]
        public List<Collider> collisions = new List<Collider>();
		
		
		[FoldoutGroup("Automatic Variables/Collision")]
		public bool inTrigger
        {
			get
			{
				return triggers.Count > 0;
			}
		}
		[FoldoutGroup("Automatic Variables/Collision")]
		public List<Collider> triggers = new List<Collider>();
        #endregion

	    [FoldoutGroup("Automatic Variables")]
	    public float velocityMagnitude;

	    [FoldoutGroup("Automatic Variables")]
	    public Vector3 velocityVector;

	    [FoldoutGroup("Automatic Variables")]
	    public Vector3 velocityVectorDirection;

	    [FoldoutGroup("Automatic Variables")]
	    public Vector3 velocityVectorDirectionInverse;

        [FoldoutGroup("Automatic Variables")]
	    public bool grounded
	    {
	        get { return groundedColliders != null && groundedColliders.Length > 0; }
	    }

	    [FoldoutGroup("Automatic Variables")]
        Collider[] groundedColliders;

        #endregion

        #region Unity Methods

        public void Awake()
	    {

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
	        
	    }

        #endregion

        #region Collision Detection
        void OnCollisionEnter(Collision collision)
		{				
			collisions.Add(collision.collider);
		}
		
		void OnCollisionExit(Collision collision)
		{
			collisions.Remove(collision.collider);
		}
		
		void OnTriggerEnter(Collider collider)
		{
			triggers.Add(collider);
		}
		
		void OnTriggerExit(Collider collider)
		{
			triggers.Remove(collider);
		}
        #endregion

        #region Ground Collision Detection

	    void CheckGroundedCollision()
	    {
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
            rigidbody.AddForce(direction, forceMode);
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

        #endregion

        // Lock speed if the velocity exceeds the max speed
        void LockSpeed()
	    {
            if(!hasMaxSpeed)
                return;

	        if (velocityMagnitude > maxSpeed)
	        {
	            rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, maxSpeed);
	        }
	    }

        // Update the automatic variables
	    void UpdateSpeedVariables()
	    {
	        velocityMagnitude = rigidbody.velocity.magnitude;
            velocityVector = rigidbody.velocity;
            velocityVectorDirection = transform.TransformDirection(rigidbody.velocity);
            velocityVectorDirectionInverse = transform.InverseTransformDirection(rigidbody.velocity);
        }

        /*****************************************************
         * Debug
         *****************************************************/
        
        #region Debug Ground Collision Check
        public void OnDrawGizmos()
	    {
	        if (!showDebugCollision)
	            return;

            Gizmos.color = debugCollisionColor;

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