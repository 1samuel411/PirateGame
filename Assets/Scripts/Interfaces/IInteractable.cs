using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PirateGame.Interactables
{
	public interface IInteractable
	{
		
		void Interact();

		Vector3 GetInteractPoint();

	}
}