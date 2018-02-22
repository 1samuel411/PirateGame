using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PirateGame.Interactables
{
	public interface IInteractable
	{
		
		void Interact(Action<IInteractable> callback);

		string GetInteractAnimation();

		Vector3 GetInteractPoint();

	}
}