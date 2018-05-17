using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SNetwork.Server
{
    public class MatchMaking
    {

        public class MatchMakingGroup
        {
            public string roomId;
            public DateTime joinTime;
            public int elo;
            public int minRange;
            public int maxRange;
            public DateTime lastExpandTime;
        }

        public List<MatchMakingGroup> matchMakingSockets = new List<MatchMakingGroup>();

        public void AddToMatchMaking(Socket socketToAdd)
        {
            // Confirm the user is the room leader
            Room roomFound = ServerManager.instance.server.rooms.FirstOrDefault(x => x.usersInRoomIds.Contains(ServerManager.instance.server.clientSockets[socketToAdd].id));

            if(roomFound == null)
            {
                return;
            }

            if(roomFound.usersInRoomIds[0] != ServerManager.instance.server.clientSockets[socketToAdd].id)
            {
                return;
            }

            roomFound.Refresh();

            MatchMakingGroup group = new MatchMakingGroup();
            group.roomId = roomFound.roomId;
            int avgElo = 0;
            for(int i = 0; i < roomFound.usersInRoom.Count; i++)
            {
                group.elo += roomFound.usersInRoom[i].elo;
            }
            group.elo = avgElo / roomFound.usersInRoom.Count;
            group.joinTime = DateTime.UtcNow;
            group.lastExpandTime = DateTime.UtcNow;
            group.minRange = ServerManager.instance.server.clientSockets[socketToAdd].elo - 100;
            group.maxRange = ServerManager.instance.server.clientSockets[socketToAdd].elo + 100;
            matchMakingSockets.Add(group);

            roomFound.matchmaking = true;
            roomFound.matchmakingBegin = DateTime.UtcNow;
        }

        public void RemoveFromMatchMaking(Socket socketToRemove)
        {
            // Confirm the user is the room leader
            Room roomFound = ServerManager.instance.server.rooms.FirstOrDefault(x => x.usersInRoomIds.Contains(ServerManager.instance.server.clientSockets[socketToRemove].id));

            if (roomFound == null)
            {
                return;
            }

            if (roomFound.matchmaking == false)
            {
                return;
            }

            if (roomFound.usersInRoomIds[0] != ServerManager.instance.server.clientSockets[socketToRemove].id)
            {
                return;
            }

            matchMakingSockets.RemoveAll(x => x.roomId == roomFound.roomId);
            Messaging.instance.SendInfoMessage(socketToRemove, "Removed from matchmaking", 0);

            roomFound.matchmaking = false;
        }

        private Thread _userSyncThread;
        public MatchMaking()
        {
            _userSyncThread = new Thread(Begin);
            _userSyncThread.Start();
        }

        void Begin()
        {
            while(true)
            {
                Thread.Sleep(1000);
                MatchMake();    
            }
        }

        void MatchMake()
        {
            for(int i = 0; i < matchMakingSockets.Count; i++)
            {
                // Expand
                if((DateTime.Now - matchMakingSockets[i].lastExpandTime).TotalSeconds >= 15)
                {
                    matchMakingSockets[i].lastExpandTime = DateTime.Now;
                    matchMakingSockets[i].maxRange += 100;
                    matchMakingSockets[i].minRange -= 100;
                }
            }

            for(int i = 0; i < matchMakingSockets.Count; i++)
            {
                /*
                for(int x = 0; x < matchMakingSockets.Count; x++)
                {
                    if(matchMakingSockets[i].elo <= matchMakingSockets[x].maxRange && matchMakingSockets[i].elo >= matchMakingSockets[x].minRange)
                    {
                        // match found
                        matchMakingSockets.Remove(matchMakingSockets[i]);
                        matchMakingSockets.Remove(matchMakingSockets[x]);

                        Room roomA = ServerManager.instance.server.rooms.FirstOrDefault(p => p.roomId == matchMakingSockets[i].roomId);
                        Room roomB = ServerManager.instance.server.rooms.FirstOrDefault(p => p.roomId == matchMakingSockets[x].roomId);

                        if(roomA != null)
                        {
                            roomA.matchmaking = false;
                            roomA.inMatch = true;
                            roomA.matchIp = "";
                        }
                        if (roomB != null)
                        {
                            roomB.matchmaking = false;
                            roomB.inMatch = true;
                            roomB.matchIp = "";
                        }
                    }
                }
                */
            }
        }

    }
}
