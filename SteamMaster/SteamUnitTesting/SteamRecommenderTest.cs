using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SteamSharpCore;
using DatabaseCore;
using DatabaseCore.lib.converter.models;
using SteamUI;
using RecommenderSystemCore;
using SteamSharpCore.steamStore.models;
using SteamSharpCore.steamUser.models;
using gameDatabase = DatabaseCore.Database;


namespace SteamUnitTesting
{
    [TestClass]
    public class SteamRecommenderTests
    {
        private readonly gameDatabase _database = new gameDatabase();
        private readonly SteamSharp _steamSharp = new SteamSharp("CA552B4A7A38C341BF5CE9F29B136A3C");
        [TestMethod]
        public void DatabaseGamesByIdRequestTest()
        {
            //Arrange 
            List<int> testAppId = new List<int> {240};

            //Act
            Dictionary<int, Game> userGameListFromIds = _database.FindGamesById(testAppId);

            //Assert
            Assert.AreEqual(userGameListFromIds[240].Title, "Counter-Strike: Source");
        }

        [TestMethod]
        public void SteamSharpGameTimeListTest()
        {
            //Arrange
            const string steamId = "76561198019106142";

            //Act
            List<UserGameTime.Game> testGame = _steamSharp.SteamUserGameTimeListById(steamId);

            //Assert
            Assert.AreEqual(testGame[0].appid, 10180);
        }

        [TestMethod]
        public void SteamSharpUserDataTest()
        {
           //Arrange
            string[] idArray = {"76561198019106142"};

            //Act
            List<SteamUser.Player> playerList = _steamSharp.SteamUserListByIds(idArray);

            //Assert
            Assert.AreEqual(playerList[0].timecreated, 1262258157);
        }
    }
}
