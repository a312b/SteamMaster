using SteamSharpCore.steamUser.models;

namespace PageRank
{
    class PRUserGame : PRGame
    {
        public double PlayTime => _gameInfo.playtime_forever;      //Total time played in minutes
        public double RecentPlayTime => _gameInfo.playtime_2weeks; //Time played in the last two weeks in minutes
        private readonly UserGameTime.Game _gameInfo;
        //public double AveragePlayTime { get; set; }
        
        public PRUserGame(PRGame game, UserGameTime.Game gameInfo) : base(game.AppID, game.TagVector, game.GameTags, game.Title)
        {
            _gameInfo = gameInfo;
        }
    }
}
