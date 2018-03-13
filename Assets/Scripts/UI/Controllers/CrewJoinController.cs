using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PirateGame.UI.Views;
using PirateGame.Managers;

namespace PirateGame.UI.Controllers
{
	public class CrewJoinController : Controller 
	{

		public CrewJoinView crewJoinView;
		public int crewId;

		void Start () 
		{
			Refresh();
		}
		
		void Update () 
		{
			Refresh();
		}

		void Refresh()
		{
			Crew crew = ServerManager.instance.crews[crewId];
			crewJoinView.crewCountText.text = crew.members.Count.ToString() + "/4";
			crewJoinView.crewNameText.text = crew.crewName;
		}

		public void Join()
		{
			ServerManager.instance.ChangeCrew(crewId);
		}
	}
}
