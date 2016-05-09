using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SteamSharpCore;
using DatabaseCore;
using DatabaseCore.lib.converter.models;
using SteamUI;
using RecommenderSystemCore;
using SteamSharpCore.steamSpy.models;
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
            List<int> testAppId = new List<int> { 240 };
            const string expected = "Counter-Strike: Source";

            //Act
            Dictionary<int, Game> userGameListFromIds = _database.FindGamesById(testAppId);

            //Assert
            string actual = userGameListFromIds[240].Title;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SteamSharpGameTimeListTest()
        {
            //Arrange
            const string steamId = "76561198019106142";
            const int excepted = 10180;

            //Act
            List<UserGameTime.Game> testGame = _steamSharp.SteamUserGameTimeListById(steamId);

            //Assert
            int actual = testGame[0].appid;
            Assert.AreEqual(excepted, actual);
        }

        [TestMethod]
        public void SteamSharpUserDataTest()
        {
            //Arrange
            string[] idArray = { "76561198019106142" };
            const int excepted = 1262258157;

            //Act
            List<SteamUser.Player> playerList = _steamSharp.SteamUserListByIds(idArray);

            //Assert
            int actual = playerList[0].timecreated;
            Assert.AreEqual(excepted, actual);
        }

        [TestMethod]
        public void SteamSpyDataTest()
        {
            //Arrange
            SteamSpyData steamSpyDataById = new SteamSpyData();
            const string excepted = "Counter-Strike: Source";

            //Act
            steamSpyDataById = _steamSharp.GameSteamSpyDataById("240");

            //Assert
            string actual = steamSpyDataById.name;
            Assert.AreEqual(excepted, actual);
        }
    }
}
