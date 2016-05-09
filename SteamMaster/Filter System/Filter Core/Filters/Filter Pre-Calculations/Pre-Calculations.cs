using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DatabaseCore;
using DatabaseCore.lib.converter.models;
using Filter_System.Filter_Core.Filters.Filter_Pre_Calculations.Models;

namespace Filter_System.Filter_Core.Filters.Filter_Pre_Calculations
{
    public class Pre_Calculations
    {
        public Pre_Calculations()
        {
            LoadTags();
            ConstructAppWorkClassDictionary();
            ConstructCombinationDictionary();
        }

        private Tuple<List<int>, List<List<string>>> AppIDWithTags;
        static readonly ListComparer<string> listComparer = new ListComparer<string>();

        public Dictionary<List<string>, List<int>> TagCombinationWithAppIDs = new Dictionary<List<string>, List<int>>(listComparer); 

        private Dictionary<int, AppWorkClass> IDAppWorkClass = new Dictionary<int, AppWorkClass>();

        private void LoadTags()
        {
            Database mongoDatabase = new Database();

            List<Game> allGames = mongoDatabase.FindAllGamesList();

            List<int> AppID = new List<int>();
            List<List<string>> Tags = new List<List<string>>();

            foreach (var game in allGames)
            {
                AppID.Add(game.SteamAppId);
                Tags.Add(game.Tags.Select(tag => tag.ToString()).ToList());
            }

            AppIDWithTags = new Tuple<List<int>, List<List<string>>>(AppID, Tags);
        }

        private void ConstructCombinationDictionary()
        {
            foreach (int ID in AppIDWithTags.Item1)
            {
                var workDictionary = CalculateTagCombosForID(ID, 10);

                foreach (var entry in workDictionary)
                {
                    foreach (var combination in entry.Value)
                    {
                        combination.Sort();
                        if (TagCombinationWithAppIDs.ContainsKey(combination))
                        {
                            TagCombinationWithAppIDs[combination].Add(ID); 
                        }
                        else
                        {
                            TagCombinationWithAppIDs.Add(combination, new List<int> { ID });
                        }
                    }
                }
            }
        }

        private void ConstructAppWorkClassDictionary()
        {
            for (int i = 0; i < AppIDWithTags.Item1.Count; i++)
            {
                int ID = AppIDWithTags.Item1[i];
                IDAppWorkClass.Add(ID, new AppWorkClass(ID, AppIDWithTags.Item2[i]));
            }
        }

        #region DiversificationCalculations

        public void DiversifictaionCalculation()
        {

            Dictionary<List<string>, int> TagCombinationDictionary = TagCombinationCalculation(4, 15, AppIDWithTags.Item2);


            #region test region

            //StreamWriter writer = new StreamWriter(@"C:\Users\jeppe\Dropbox\Software\Projekt\P2\TagCombinations.txt", false);

            //List<KeyValuePair<List<string>, int>> sortedDic = TagCombinationDictionary.ToList();

            //sortedDic.Sort((x, y) => y.Value.CompareTo(x.Value));

            //foreach (var entry in sortedDic)
            //{
            //    if (entry.Value > 5)
            //    {
            //        writer.WriteLine($"{entry.Key[0]}, {entry.Key[1]}, {entry.Key[2]}, {entry.Key[3]}, {entry.Key[4]}");
            //        writer.WriteLine($":        {entry.Value}");
            //    }
            //}

            //writer.Close();
            //Console.WriteLine("Dones");
            #endregion


        }


        private Dictionary<List<string>, int> TagCombinationCalculation(int minimumTagAmount, int tagRange, IEnumerable<List<string>> tagListCollection)
        {
            Dictionary<List<string>, int> resultDictionary = new Dictionary<List<string>, int>();

            foreach (var tagList in tagListCollection)
            {
                if (tagList.Count > minimumTagAmount && !tagList.Contains("Trains"))
                {
                    int range = tagRange;
                    var result = tagList.GetRange(0, tagList.Count < range ? tagList.Count : range).Combinations(5);
                    foreach (var entry in result)
                    {
                        List<string> workList = entry.ToList();
                        resultDictionary = AddOrCountToCombinationDictionary(workList);
                    }
                }
            }
            return resultDictionary;
        }

        private Dictionary<List<string>, int> TagCombinationCalculation(int minimumTagAmount, int tagRange, params List<string>[] tagListArray)
        {
            List<List<string>> tagListCollection = tagListArray.ToList();

            return TagCombinationCalculation(minimumTagAmount, tagRange, tagListCollection);
        }

        private Dictionary<List<string>, int> AddOrCountToCombinationDictionary(List<string> tagCombinationList)
        {
            tagCombinationList.Sort();
            Dictionary<List<string>, int> returnDictionary = new Dictionary<List<string>, int>();

            if (returnDictionary.ContainsKey(tagCombinationList))
            {
                returnDictionary[tagCombinationList]++;
            }
            else if (!tagCombinationList.Contains("Action") && !tagCombinationList.Contains("Indie") && !tagCombinationList.Contains("TrackIR"))
            {
                returnDictionary.Add(tagCombinationList, 1);
            }

            return returnDictionary;
        }


        #endregion

        #region Tag Similarity calculations

        public void FileAppSimilarity()
        {
            Dictionary<int, List<AppWorkClass>> similarAppCollection = CalculateSimilarAppToAllApps();

            foreach (var AppList in similarAppCollection)
            {
                StreamWriter writer = new StreamWriter($@"C:\Users\jeppe\Dropbox\Software\Projekt\P2\{AppList.Key} SimilarApp.txt", false); //Lav local

                writer.WriteLine($"AppID: {AppList.Key}");

                foreach (var App in AppList.Value)
                {
                    if (App.Score > 200)
                    {
                        writer.WriteLine($"{App.AppID} : {App.Score}");
                    }
                }

                writer.Close();
            }
        }

        public Dictionary<int, List<AppWorkClass>> CalculateSimilarAppToAllApps()
        {
            Dictionary<int, List<AppWorkClass>> returnDictionary = new Dictionary<int, List<AppWorkClass>>(); 

            foreach (var App in IDAppWorkClass)
            {
                returnDictionary.Add(App.Key, GetAppsSimilarToApp(App.Key, 6));
            }

            return returnDictionary;
        }

        public List<AppWorkClass> GetAppsSimilarToApp(int appID, int relevantTagRange)
        {
            List<int> appIDList = AppIDWithTags.Item1;
            appIDList.Sort();

            Dictionary<int, AppWorkClass> AppWCDictionary = IDAppWorkClass;

            Dictionary<int, List<List<string>>> TagCombosForID = CalculateTagCombosForID(appID, relevantTagRange);

            #region point giving to similar tagged games
            for (int i = relevantTagRange; i >= 1; i--)
            {
                if (TagCombosForID.ContainsKey(i))
                {
                    foreach (var combination in TagCombosForID[i])
                    {
                        combination.Sort();
                        if (TagCombinationWithAppIDs.ContainsKey(combination))
                        {
                            foreach (int ID in TagCombinationWithAppIDs[combination]) 
                            {
                                AppWCDictionary[ID].Score += i;
                            }
                        }
                    }
                }
            }
            #endregion

            List<AppWorkClass> returnList = AppWCDictionary.Values.ToList();
            returnList.Sort();

            int valueAfterSort = 1;

            foreach (var App in returnList)
            {
                App.Score = valueAfterSort;
                valueAfterSort++;
            }

            return returnList;
        }

        private Dictionary<int, List<List<string>>> CalculateTagCombosForID(int appID, int Range) //Calculate all possible tag combinations for an appID. Range indicates how many tags are relevant.
                                                                                                 // Dictionary returned consist of an int key that represents the amount of tags for the combinations represented in the value
        {
            AppWorkClass workClass;
            IDAppWorkClass.TryGetValue(appID, out workClass);
            List<string> appIDTags = workClass.TagList;
            Dictionary<int, List<List<string>>> returnDictionary = new Dictionary<int, List<List<string>>>();

            if (appIDTags != null && appIDTags.Count != 0)
            {
                int tagRange = appIDTags.Count < Range ? appIDTags.Count : Range;

                for (int i = tagRange; i >= 1; i--)
                {
                    IEnumerable<IEnumerable<string>> tagIEnumerable =
                        appIDTags.GetRange(0, tagRange)
                            .Combinations(i);

                    List<List<string>> tagList = tagIEnumerable.Select(entry => entry.ToList()).ToList();
                    returnDictionary.Add(i, tagList);

                }
            }

            return returnDictionary;
        }

#endregion
    }
}
