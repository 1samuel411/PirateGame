using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PirateGame
{
	public class DestroyMe : Base 
	{

		public void DestroyThis()
		{
			Destroy(gameObject);	
		}
		
	}
}