# Buoyancy Toolkit
# Copyright 2011, Gustav Olsson

# Example scenes:

Check out the example scenes to see how the buoyancy toolkit is used in practice. Turn off "Maximize on Play" in the Game window and move around the objects using the Scene window while playing the game to get a feel for how the simulation works.

# Basic use:

1. Add the FluidVolume script to a game object that has a trigger collider.
2. Add the BuoyancyForce script to a game object that has a rigidbody and a convex collider. Drag the convex collider into the Collider slot of the BuoyancyForce script.
3. Tweak the Weight Factor value of the BuoyancyForce script for different behaviors.

See the Basic example scene.

# Custom fluid surface:

1. Create a script that extends FluidVolume and overrides the WaveFunction(Vector3) method.
2. See Basic use, but use the new script instead of FluidVolume.
3. Change the Wave Amplitude of the new script in the inspector to a value greater than zero.

See the Ocean example scene.

# Good to know

1. Always call BuoyancyForce.Recalculate() when the scale of the connected game object or collider changes.

2. Try changing the center of mass of a rigidbody if it doesn't behave the intended way in the fluid. For example, a boat might work better when the center of mass lies below the geometric center.

3. If you want to change a rigidbody's drag/angularDrag during gameplay, set the attached BuoyancyForce.nonfluidDrag/nonfluidAngularDrag instead. This is needed because BuoyancyForce often change the drag values of the rigidbody.

4. You may add several BuoyancyForce scripts to one game object (as long as it has a rigidbody) if you want multiple buoyancy colliders (the colliders are usually attached to the game object's children). If you want to change the rigidbody's drag/angularDrag during gameplay when in this setup, make sure you set BuoyancyForce.nonfluidDrag/nonfluidAngularDrag on all BuoyancyForce script instances.