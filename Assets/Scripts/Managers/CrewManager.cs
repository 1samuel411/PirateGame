using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PirateGame.Managers
{
    public class CrewManager : MonoBehaviour
    {

        public static CrewManager instance;

        void Awake()
        {
            if (instance != null)
            {
                return;
            }

            instance = this;
        }

        public static bool IsLeader(int userId)
        {
            for (int i = 0; i < ServerManager.instance.crews.Count; i++)
            {
                if (ServerManager.instance.crews[i].leader == userId)
                {
                    return true;
                }
            }

            return false;
        }

        public static Color GetCrewColor(int color)
        {
            return IconManager.instance.crewColors[color];
        }

        public static Crew GetCrew(int crew)
        {
            if(crew < 0)
                return null;

            if (crew >= ServerManager.instance.crews.Count)
                return null;

            return ServerManager.instance.crews[crew];
        }

        public static bool HasCrew(int crew)
        {
            if (crew < 0)
                return false;

            if (crew >= ServerManager.instance.crews.Count)
                return false;

            return true;
        }

        public static int GetNextCrew(int curCrew)
        {
            if (curCrew < 0)
                return 0;

            curCrew++;
            if (curCrew >= ServerManager.instance.crews.Count)
            {
                curCrew = 0;
            }

            return curCrew;
        }
    }

    [System.Serializable]
    public class Crew
    {

        public string crewName;
        public List<int> members;
        public int leader = -1;
        public int crewColor = 0;

        public string crewPassword;

    }
}
