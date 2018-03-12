using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class CompilingSaver
{
	[MenuItem("Assets/Optimize Compiling Time")]
	static void optimizeCompilingTime()
	{
		bool process = EditorUtility.DisplayDialog("NOTE!", "This action will change your project hierarchy. Make sure your project backuped.", "O..kay", "No Stop");
		if(!process)
			return;

		List<string> selectFolders = new List<string>();
		foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
		{
			string selectPath = AssetDatabase.GetAssetPath(obj);
			if(Directory.Exists(selectPath))
				selectFolders.Add(selectPath);
		}

		if(selectFolders.Count == 0)
		{
			Debug.Log("CompilingSaver:You should select a folder for optimization.");
			return;
		}	

		foreach(string guid in AssetDatabase.FindAssets("t:MonoScript", selectFolders.ToArray()))
		{
			string scriptPath = AssetDatabase.GUIDToAssetPath(guid);

			string newPath = "";
			if(isUnderEditor(scriptPath))
				newPath = scriptPath.Insert("Assets/".Length, "Standard Assets/Editor/");
			else
				newPath = scriptPath.Insert("Assets/".Length, "Standard Assets/Runtime/");

			Debug.Log("Move " + scriptPath + " to " + newPath);
			createDirectoriesForPath(newPath);
			AssetDatabase.MoveAsset(scriptPath, newPath);
		}
	}

	static bool isUnderEditor(string path)
	{
		foreach(string dir in path.Split('/'))
			if(dir == "Editor")
				return true;

		return false;
	}

	static void createDirectoriesForPath(string path)
	{
		string[] dirs = path.Split('/');
		string curPath = "";
		foreach(string curDir in dirs)
		{
			if(curDir.Contains(".")) // It's a file, don't try to create
				return;

			if(curPath != "" && !Directory.Exists(curPath + "/" + curDir))
			{
				Debug.Log("create folder " + curPath + "/" + curDir);
				AssetDatabase.CreateFolder(curPath, curDir);
			}

			if(curPath == "")
				curPath = curDir;
			else
				curPath += "/" + curDir;
		}
	}
}
