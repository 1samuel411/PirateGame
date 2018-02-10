// FluidVolume.cs
// Copyright 2011, Gustav Olsson
using UnityEngine;

public struct FluidVolumeMessage
{
	public FluidVolume fluidVolume;
	public Collider collider;
	
	public FluidVolumeMessage(FluidVolume fluidVolume, Collider collider)
	{
		this.fluidVolume = fluidVolume;
		this.collider = collider;
	}
}

[RequireComponent(typeof(Collider))]
public class FluidVolume : MonoBehaviour
{
	/// <summary>
	/// The density of the fluid. Measured in kilograms per cubic metre.
	/// </summary>
	public float density = 1000.0f;
	
	/// <summary>
	/// Determines the maximum world-space offset along the y-axis of a wave on the surface of the fluid volume. May only be greater than or equal to zero.
	/// </summary>
	public float waveAmplitude = 0.0f;
	
	/// <summary>
	/// The linear drag that is applied to a rigidbody that is submerged in the fluid volume.
	/// </summary>
	public float rigidbodyDrag = 1.0f;
	
	/// <summary>
	/// The angular drag that is applied to a rigidbody that is submerged in the fluid volume.
	/// </summary>
	public float rigidbodyAngularDrag = 1.0f;
	
	private void SendFluidVolumeEnter(Component receiver, FluidVolumeMessage message)
	{
		receiver.SendMessageUpwards("OnFluidVolumeEnter", message, SendMessageOptions.DontRequireReceiver);
	}
	
	private void SendFluidVolumeStay(Component receiver, FluidVolumeMessage message)
	{
		receiver.SendMessageUpwards("OnFluidVolumeStay", message, SendMessageOptions.DontRequireReceiver);
	}
	
	private void SendFluidVolumeExit(Component receiver, FluidVolumeMessage message)
	{
		receiver.SendMessageUpwards("OnFluidVolumeExit", message, SendMessageOptions.DontRequireReceiver);
	}
	
	/// <summary>
	/// Projects a point onto the surface of the fluid volume.
	/// </summary>
	/// <returns>
	/// The projected point.
	/// </returns>
	/// <param name='worldPoint'>
	/// A point in world-space.
	/// </param>
	public Vector3 ProjectPointOntoSurface(Vector3 worldPoint)
	{
		return new Vector3(worldPoint.x, GetComponent<Collider>().bounds.max.y + waveAmplitude * (WaveFunction(worldPoint) - 1.0f), worldPoint.z);
	}
	
	/// <summary>
	/// Calculates the distance from the bottom to the surface of the fluid volume at a point.
	/// </summary>
	/// <returns>
	/// The height at a point.
	/// </returns>
	/// <param name='worldPoint'>
	/// A point in world-space.
	/// </param>
	public float RelativeHeightAtPoint(Vector3 worldPoint)
	{
		return GetComponent<Collider>().bounds.size.y + waveAmplitude * (WaveFunction(worldPoint) - 1.0f);
	}
	
	/// <summary>
	/// Describes a wave function that is used as a lookup for the fluid volume's surface. This method should not take the wave amplitude into account. Extend FluidVolume and override this method to create waves on the surface of the fluid.
	/// </summary>
	/// <returns>
	/// The wave height along the y-axis. A value between -1.0f and 1.0f.
	/// </returns>
	/// <param name='worldPoint'>
	/// A point in world-space.
	/// </param>
	public virtual float WaveFunction(Vector3 worldPoint)
	{
		return 0.0f;
	}
	
	public void OnTriggerEnter(Collider collider)
	{
		SendFluidVolumeEnter(collider, new FluidVolumeMessage(this, collider));
	}
	
	public void OnTriggerStay(Collider collider)
	{
		SendFluidVolumeStay(collider, new FluidVolumeMessage(this, collider));
	}
	
	public void OnTriggerExit(Collider collider)
	{
		SendFluidVolumeExit(collider, new FluidVolumeMessage(this, collider));
	}
}