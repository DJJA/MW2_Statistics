﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MW2_Statistics
{
    public static class MW2EventHandler
    {
        private static int MatchId
        {
            get { return DataBase.GetCurrentMatchId(); }
        }

        private static bool mWaitingforEndMatch = false;    // When exitlevel: executed appears we need to wait with ending the match and to wait till the line ------------- appears, because we need to handle events between those lines

        public static void HandleMW2Event(MW2Event e)
        {
            //if (mMatchId == -1 && e.Type != "initgame")     // If we start become host after hostmigration
            //    return;

            // When you leave game as host:
            //  0:26 ShutdownGame:
            //  0:26------------------------------------------------------------

            switch (e.Type)
            {
                case "initgame":
                    if (e.Timestamp == "0:00")    // New match began
                    {
                        if (MatchId != -1)
                            DataBase.EndMatch(MatchId); // When you are host and you leave, the match won't be ended by this application. Can't read that from the "games_mp.log" file.
                    }
                    if (MatchId == -1)
                        DataBase.RegisterNewMatch();
                    break;
                case "exitlevel: executed":         // Match has finished
                    mWaitingforEndMatch = true;
                    break;
                case "------------------------------------------------------------":
                    if (mWaitingforEndMatch)
                    {
                        mWaitingforEndMatch = false;
                        DataBase.EndMatch(MatchId);
                    }
                    break;
                case "j":
                case "q":
                case "say":
                case "sayteam":
                    UpdatePlayer(e.Victim);         // With these types there's only a victim, no attacker
                    break;
                case "weapon":
                    UpdatePlayer(e.Victim);
                    UpdateWeapon(e.Weapon);
                    break;
                case "k":
                case "d":
                    long vicPlayerId = UpdatePlayer(e.Victim);
                    long attPlayerId = UpdatePlayer(e.Attacker);
                    int wepId = UpdateWeapon(e.Weapon);

                    if (attPlayerId == -1)              // Player damaged himself
                        attPlayerId = vicPlayerId;
                    DataBase.RegisterHit(vicPlayerId, attPlayerId, MatchId, wepId, e.Damage, e.HitLocation, e.MeansOfDeath, e.Type == "k" ? true : false);
                    break;
            }
        }

        private static long UpdatePlayer(Player p)
        {
            if (p.EntityNumber == -1)                       // If entityNumber is -1, the player was not defined (eg Player took falling damage)
                return -1;

            long playerId = GetPlayerId(p.SteamId);
            AddAlias(playerId, p.Name);
            DataBase.UpdatePlayerLastSeen(playerId);
            return playerId;
        }

        private static int UpdateWeapon(string weaponName)
        {
            int result;
            result = DataBase.GetWeaponId(weaponName);
            if(result == -1)
            {
                DataBase.AddWeapon(weaponName);
                result = DataBase.GetWeaponId(weaponName);
            }
            return result;
        }

        private static long GetPlayerId(UInt64 steamId)
        {
            long result;
            result = DataBase.GetPlayerIdBySteamId(steamId);
            if(result == -1)
            {
                DataBase.AddPlayer(steamId);
                result = DataBase.GetPlayerIdBySteamId(steamId);
            }
            return result;
        }

        private static void AddAlias(long playerId, string playerName)
        {
            if(!DataBase.PlayerAliasExists(playerName, playerId, MatchId))
            {
                DataBase.AddPlayerAlias(playerName, playerId, MatchId);
            }
        }
    }
}
