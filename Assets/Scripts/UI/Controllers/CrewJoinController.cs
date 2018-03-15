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
            crewJoinView.joinButton.gameObject.SetActive(true);

		    Color backgroundColor = CrewManager.GetCrewColor(crewId);
            backgroundColor.a = 0.4f;
            crewJoinView.backgroundImage.color = backgroundColor;

            if(crew.members.Count >= 4)
                crewJoinView.joinButton.gameObject.SetActive(false);
        }

        public void Join()
		{
			ServerManager.instance.ChangeCrew(crewId);
		}
	}
}
