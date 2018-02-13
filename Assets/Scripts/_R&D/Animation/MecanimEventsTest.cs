using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MecanimEventsTest : MonoBehaviour
{
	public void DebugEvent (string functionName)
	{
		Debug.Log (gameObject.name + "." + "DebugEvent." + functionName + " at time " + Time.time);
	}
}