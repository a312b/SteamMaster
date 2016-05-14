using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using SteamSharpCore.steamStore.models;
using SteamSharpCore.steamUser.models;

namespace RecommenderSystemCore.User_Data_Handling.Models
{
    public class UserGameWorkClass : IComparable<UserGameWorkClass>
    {
        public UserGameWorkClass(UserGameTime.Game game)
        {
            appID = game.appid;
            PlayTimeForever = game.playtime_forever;
            PlayTime2Weeks = game.playtime_2weeks;

            LoadTagList(game.appid);
        }

        public UserGameWorkClass(UserGameTime.Game game, double score) : this(game)
        {
            Score = score;
        }


        private void LoadTagList(int appID)
        {
            List<SteamStoreGame.Tag> tagList =
                new DatabaseCore.Database().FindGamesById(new List<int>(appID))[appID].Tags;

            foreach (var tag in tagList)
            {
                TagList.Add(tag.description);
            }
        }

        public int appID { get; private set; }

        public int PlayTimeForever { get; private set; }

        public int PlayTime2Weeks { get; private set; }

        public double Score { get; set; }

        public List<string> TagList { get; private set; }

        public int CompareTo(UserGameWorkClass other) //Sort after the score of the App
        {
            int sourceScore = Convert.ToInt32(Score);
            int compareScore = Convert.ToInt32(other.Score);
            return sourceScore - compareScore;
        }
    }
}
