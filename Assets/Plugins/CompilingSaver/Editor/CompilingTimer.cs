using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class CompilingTimer
{
	static CompilingTimer()
	{
		EditorApplication.update += CheckForCompile;
	}

	static void CheckForCompile()
	{
		bool isCompiling = EditorPrefs.GetBool("isCompiling");
		if(EditorApplication.isCompiling != isCompiling)
		{
			if(!isCompiling)
			{
				EditorPrefs.SetFloat("Compile start time", (float)EditorApplication.timeSinceStartup);
			}
			else
			{
				Debug.Log("Compile time " + (EditorApplication.timeSinceStartup - (double)EditorPrefs.GetFloat("Compile start time")));
			}

			EditorPrefs.SetBool("isCompiling", EditorApplication.isCompiling);
		}
	}
}
