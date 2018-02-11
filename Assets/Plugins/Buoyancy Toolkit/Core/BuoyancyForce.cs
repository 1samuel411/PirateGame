// BuoyancyForce.cs
// Copyright 2011, Gustav Olsson
using System.Collections.Generic;
using UnityEngine;

public enum BuoyancyQuality : int
{
	Low, Medium, High, Custom
}

//[RequireComponent(typeof(Rigidbody))]
public class BuoyancyForce : MonoBehaviour
{
    private static float boundsExtentBias = 0.01f;
	private static float weightFactorBias = 0.0f;
	private static int[] samplesAtQuality = { 3, 8, 16 };
	
	[SerializeField]
	[HideInInspector]
	private Collider buoyancyCollider;
	
	[SerializeField]
	[HideInInspector]
	private BuoyancyQuality quality = BuoyancyQuality.Medium;
	
	[SerializeField]
	[HideInInspector]
	private int samples = 8;
	
	[SerializeField]
	[HideInInspector]
	private bool useWeighting = true;
	
	[SerializeField]
	[HideInInspector]
	private float weightFactor = 1.5f;
	
	[SerializeField]
	[HideInInspector]
	private float dragScalar = 1.0f;
	
	[SerializeField]
	[HideInInspector]
	private float angularDragScalar = 1.0f;
	
	[SerializeField]
	[HideInInspector]
	private int ignoreLayers = 0;
	
	private float nonfluidDrag;
	private float nonfluidAngularDrag;
	
	private float inverseVolume;
	private ICollection<FluidVolume> fluidVolumes = new LinkedList<FluidVolume>();

    public Rigidbody Rigidbody;

    protected void OnValidate()
    {
        if  (Rigidbody == null)
        {
            Rigidbody = GetComponent<Rigidbody>();

            if (Rigidbody == null)
            {
                Rigidbody = GetComponentInParent<Rigidbody>();
            }
        }

        if (buoyancyCollider == null)
        {
            buoyancyCollider = GetComponent<Collider>();
        }
    }

    /// <summary>
    /// Gets or sets the buoyancy collider, the collider that will be used to calculate the buoyancy properties of the rigidbody. The collider should be convex for stability reasons.
    /// </summary>
    /// <value>
    /// The buoyancy collider.
    /// </value>
    public Collider BuoyancyCollider
	{
		get { return buoyancyCollider; }
		
		set
		{
			if (buoyancyCollider != value)
			{
				buoyancyCollider = value;
				
				Recalculate();
			}
		}
	}
	
	/// <summary>
	/// Gets or sets the quality of the simulation.
	/// </summary>
	/// <value>
	/// The quality.
	/// </value>
	public BuoyancyQuality Quality
	{
		get { return quality; }
		
		set
		{
			BuoyancyQuality clampedValue = (BuoyancyQuality)Mathf.Clamp((int)value, 0, 3);
			
			if (quality != clampedValue)
			{
				quality = clampedValue;
				
				if (quality != BuoyancyQuality.Custom)
				{
					samples = samplesAtQuality[(int)quality];
				}
				
				Recalculate();
			}
		}
	}
	
	/// <summary>
	/// Gets or sets the number of sample points used per axis for the buoyancy simulation. More samples results in more realistic behaviour while using more resources.
	/// </summary>
	/// <value>
	/// The samples.
	/// </value>
	public int Samples
	{
		get { return samples; }
		
		set
		{
			quality = BuoyancyQuality.Custom;
			
			int clampedValue = Mathf.Clamp(value, 1, 100);
			
			if (samples != clampedValue)
			{
				samples = clampedValue;
				
				Recalculate();
			}
		}
	}
	
	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="BuoyancyForce"/> use weighting. Weighting enables easy tweaking of the buoyancy behaviour. If weighting is not used, realistic proportions, rigidbody masses and fluid densities are required for realistic behaviour.
	/// </summary>
	/// <value>
	/// <c>true</c> if use weighting; otherwise, <c>false</c>.
	/// </value>
	public bool UseWeighting
	{
		get { return useWeighting; }
		
		set
		{
			if (useWeighting != value)
			{
				useWeighting = value;
				
				if (useWeighting)
				{
					Recalculate();
				}
			}
		}
	}
	
	/// <summary>
	/// Gets or sets the weight factor. Only applied when weighting is used. A weight factor of 1 results in enough force to counteract gravity and the rigidbody will stay in equilibrium within the fluid. A weight factor of 2 results in a net force equal to gravity but in the opposite direction (making the rigidbody float in the fluid) and so on.
	/// </summary>
	/// <value>
	/// The weight factor.
	/// </value>
	public float WeightFactor
	{
		get { return weightFactor; }
		set { weightFactor = value; }
	}
	
	/// <summary>
	/// Gets or sets the drag scalar, a scalar that is multiplied by each fluid volume's drag value before being set as linear drag to the submerged rigidbody.
	/// </summary>
	/// <value>
	/// The drag scalar.
	/// </value>
	public float DragScalar
	{
		get { return dragScalar; }
		set { dragScalar = value; }
	}
	
	/// <summary>
	/// Gets or sets the angular drag scalar, a scalar that is multiplied by each fluid volume's angular drag value before being set as angular drag to the submerged rigidbody.
	/// </summary>
	/// <value>
	/// The angular drag scalar.
	/// </value>
	public float AngularDragScalar
	{
		get { return angularDragScalar; }
		set { angularDragScalar = value; }
	}
	
	/// <summary>
	/// Gets or sets a bitmask indicating which layers this rigidbody should ignore when performing buoyancy simulation.
	/// </summary>
	/// <value>
	/// A bitmask.
	/// </value>
	public int IgnoreLayers
	{
		get { return ignoreLayers; }
		set { ignoreLayers = value; }
	}
	
	/// <summary>
	/// Gets or sets the nonfluid drag. The base linear drag that should be applied to the rigidbody.
	/// </summary>
	/// <value>
	/// The nonfluid drag.
	/// </value>
	public float NonfluidDrag
	{
		get { return nonfluidDrag; }
		set { nonfluidDrag = value; }
	}
	
	/// <summary>
	/// Gets or sets the nonfluid angular drag. The base angular drag that should be applied to the rigidbody.
	/// </summary>
	/// <value>
	/// The nonfluid angular drag.
	/// </value>
	public float NonfluidAngularDrag
	{
		get { return nonfluidAngularDrag; }
		set { nonfluidAngularDrag = value; }
	}
	
	private void SimulateBuoyancy(FluidVolume fluidVolume)
	{
		Bounds bounds = buoyancyCollider.bounds;
		
		bounds.Expand(boundsExtentBias);
		
		float tangentOffset = bounds.size.x / (float)samples;
		float bitangentOffset = bounds.size.z / (float)samples;
		
		float impulse = -Physics.gravity.y * Time.fixedDeltaTime;
		
		if (useWeighting)
		{
			impulse *= (weightFactor + weightFactorBias) * Rigidbody.mass * inverseVolume;
		}
		else
		{
			impulse *= fluidVolume.density;
		}
		
		Vector3 min = new Vector3(bounds.min.x, fluidVolume.GetComponent<Collider>().bounds.min.y, bounds.min.z);
		
		for (int x = 0; x < samples; x++)
		{
			for (int z = 0; z < samples; z++)
			{
				Vector3 offset = new Vector3(tangentOffset * ((float)x + 0.5f), 0.0f, bitangentOffset * ((float)z + 0.5f));
				Vector3 start = min + offset;
				
				ApplySampleImpulse(start, tangentOffset, bitangentOffset, fluidVolume.RelativeHeightAtPoint(start), impulse);
			}
		}
	}
	
	private void ApplySampleImpulse(Vector3 start, float width, float depth, float height, float impulse)
	{
		RaycastHit lowerHit;
		
		if (buoyancyCollider.Raycast(new Ray(start, Vector3.up), out lowerHit, height))
		{
			float fluidBoxHeight = lowerHit.distance;
			
			RaycastHit upperHit;
			
			if (buoyancyCollider.Raycast(new Ray(start + Vector3.up * height, Vector3.down), out upperHit, height))
			{
				fluidBoxHeight += upperHit.distance;
			}
			
			float sampleBoxHeight = height - fluidBoxHeight;
			float sampleVolume = sampleBoxHeight * width * depth;
			
			Rigidbody.AddForceAtPosition(Vector3.up * (sampleVolume * impulse), lowerHit.point, ForceMode.Impulse);
		}
	}
	
	private float CalculateApproximateVolume()
	{
		Bounds bounds = buoyancyCollider.bounds;
		
		bounds.Expand(boundsExtentBias);
		
		float tangentOffset = bounds.size.x / (float)samples;
		float bitangentOffset = bounds.size.z / (float)samples;
		
		float volume = 0.0f;
		
		for (int x = 0; x < samples; x++)
		{
			for (int z = 0; z < samples; z++)
			{
				Vector3 offset = new Vector3(tangentOffset * ((float)x + 0.5f), 0.0f, bitangentOffset * ((float)z + 0.5f));
				
				RaycastHit lowerHit;
				
				if (buoyancyCollider.Raycast(new Ray(bounds.min + offset, Vector3.up), out lowerHit, bounds.size.y))
				{
					RaycastHit upperHit;
					
					if (buoyancyCollider.Raycast(new Ray(bounds.min + Vector3.up * bounds.size.y + offset, Vector3.down), out upperHit, bounds.size.y))
					{
						volume += (bounds.size.y - lowerHit.distance - upperHit.distance) * tangentOffset * bitangentOffset;
					}
				}
			}
		}
		
		return volume;
	}
	
	private void SetRigidbodyDrag()
	{
		float totalDrag = nonfluidDrag;
		float totalAngularDrag = nonfluidAngularDrag;
		
		BuoyancyForce[] buoyancyForces = GetComponents<BuoyancyForce>();
		
		foreach (BuoyancyForce force in buoyancyForces)
		{
			foreach (FluidVolume volume in force.fluidVolumes)
			{
				totalDrag += force.dragScalar * volume.rigidbodyDrag;
				totalAngularDrag += force.angularDragScalar * volume.rigidbodyAngularDrag;
			}
		}
		
		Rigidbody.drag = totalDrag;
		Rigidbody.angularDrag = totalAngularDrag;
	}
	
	/// <summary>
	/// Recalculate internal properties. This method should always be called after the scale of the game object or collider has been changed.
	/// </summary>
	public void Recalculate()
	{
		if (useWeighting && buoyancyCollider != null)
		{
			inverseVolume = 1.0f / CalculateApproximateVolume();
		}
	}
	
	public void OnFluidVolumeEnter(FluidVolumeMessage message)
	{
		if (message.collider == buoyancyCollider && (ignoreLayers & (1 << message.fluidVolume.gameObject.layer)) == 0)
		{
			if (!fluidVolumes.Contains(message.fluidVolume))
			{
				fluidVolumes.Add(message.fluidVolume);
			}
		}
	}
	
	public void OnFluidVolumeExit(FluidVolumeMessage message)
	{
		if (message.collider == buoyancyCollider)
		{
			fluidVolumes.Remove(message.fluidVolume);
		}
	}
	
	public void Awake()
	{
		// Set initial nonfluid drag
		nonfluidDrag = Rigidbody.drag;
		nonfluidAngularDrag = Rigidbody.angularDrag;
	}

	public void Start()
	{
		Recalculate();
	}
	
	public void FixedUpdate()
	{
		if (buoyancyCollider != null && !Rigidbody.IsSleeping())
		{
			// Simulate buoyancy
			foreach (FluidVolume fluidVolume in fluidVolumes)
			{
				SimulateBuoyancy(fluidVolume);
			}
			
			// Set drag
			SetRigidbodyDrag();
		}
	}
}