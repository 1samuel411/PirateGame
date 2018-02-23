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
		string GetUnInteractAnimation();

		bool GetActive();

		Vector3 GetInteractPoint();

	}
}