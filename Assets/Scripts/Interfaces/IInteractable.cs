using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PirateGame.Entity;

namespace PirateGame.Interactables
{
	public interface IInteractable
	{
		
		void Interact(EntityHumanoid humanoid, Action<IInteractable> callback);

		string GetInteractAnimation();
		string GetUnInteractAnimation();

		bool GetActive();

		bool GetAutomatic();

		Vector3 GetInteractPoint();

	}
}