using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BuoyancyForce))]
public class BuoyancyForceEditor : Editor
{
	public override void OnInspectorGUI()
	{
		BuoyancyForce source = (BuoyancyForce)target;
		
		EditorGUILayout.BeginVertical();
		
		EditorGUILayout.BeginHorizontal();
		source.BuoyancyCollider = (Collider)EditorGUILayout.ObjectField(new GUIContent("Collider", "The collider that will be used to calculate the buoyancy properties of the rigidbody. The collider should be convex for stability reasons."), source.BuoyancyCollider, typeof(Collider));
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		source.Quality = (BuoyancyQuality)EditorGUILayout.EnumPopup(new GUIContent("Quality"), source.Quality);
		EditorGUILayout.EndHorizontal();
		
		if (source.Quality == BuoyancyQuality.Custom)
		{
			EditorGUILayout.BeginHorizontal();
			source.Samples = EditorGUILayout.IntField(new GUIContent("Samples", "The number of sample points used per axis for the buoyancy simulation. More samples results in more realistic behaviour while using more resources."), source.Samples);
			EditorGUILayout.EndHorizontal();
		}
		
		EditorGUILayout.BeginHorizontal();
		source.UseWeighting = EditorGUILayout.Toggle(new GUIContent("Use Weighting", "Weighting enables easy tweaking of the buoyancy behaviour. If no weighting is used, realistic proportions, rigidbody masses and fluid densities are required for realistic behaviour."), source.UseWeighting);
		EditorGUILayout.EndHorizontal();
		
		if (source.UseWeighting)
		{
			EditorGUILayout.BeginHorizontal();
			source.WeightFactor = EditorGUILayout.Slider(new GUIContent("Weight Factor", "A weight factor of 1 results in enough force to counteract gravity and the rigidbody will stay in equilibrium within the fluid. A weight factor of 2 results in a net force equal to gravity but in the opposite direction (making the rigidbody float in the fluid) and so on."), source.WeightFactor, 0.0f, 20.0f);
			EditorGUILayout.EndHorizontal();
		}
		
		EditorGUILayout.BeginHorizontal();
		source.DragScalar = EditorGUILayout.FloatField(new GUIContent("Drag Scalar", "A scalar that is multiplied by each fluid volume's drag value before being set as linear drag to the submerged rigidbody."), source.DragScalar);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		source.AngularDragScalar = EditorGUILayout.FloatField(new GUIContent("Ang. Drag Scalar", "A scalar that is multiplied by each fluid volume's angular drag value before being set as angular drag to the submerged rigidbody."), source.AngularDragScalar);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.EndVertical();
	}
}