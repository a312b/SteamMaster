using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Filter_System.Filter_Core.Filters.Filter_Pre_Calculations.Models;

namespace Filter_System.Filter_Core.Filters.Filter_Pre_Calculations
{
    public class Pre_Calculations
    {
        public Pre_Calculations()
        {
            LoadTags();
            ConstructCombinationDictionary();
        }

        private Tuple<List<int>, List<List<string>>> AppIDWithTags;
        private Dictionary<int, List<string>> AppIDTAgDictionary = new Dictionary<int, List<string>>(); 
        private Dictionary<List<string>, List<int>> TagCombinationWithAppIDs = new Dictionary<List<string>, List<int>>(); 

        private void LoadTags()
        {
            TagReader tagReader = new TagReader(@"C:\Users\jeppe\Dropbox\Software\Projekt\P2\TagsForAppID.txt");
            AppIDWithTags = tagReader.AppIDWithTag();
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

        #region DiversificationCalculations

        public void DiversifictaionCalculation()
        {

            Dictionary<List<string>, int> TagCombinationDictionary = TagCombinationCalculation(4, 15, AppIDWithTags.Item2);


            #region test region

            StreamWriter writer = new StreamWriter(@"C:\Users\jeppe\Dropbox\Software\Projekt\P2\TagCombinations.txt", false);

            List<KeyValuePair<List<string>, int>> sortedDic = TagCombinationDictionary.ToList();

            sortedDic.Sort((x, y) => y.Value.CompareTo(x.Value));

            foreach (var entry in sortedDic)
            {
                if (entry.Value > 5)
                {
                    writer.WriteLine($"{entry.Key[0]}, {entry.Key[1]}, {entry.Key[2]}, {entry.Key[3]}, {entry.Key[4]}");
                    writer.WriteLine($":        {entry.Value}");
                }
            }

            writer.Close();
            Console.WriteLine("Dones");
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

        private List<int> GetAppsSimilarToApp(int appID, int relevantTagRange)
        {
            List<int> appIDList = AppIDWithTags.Item1;
            appIDList.Sort();
            List<double> recommendationValue = new List<double>();

            Dictionary<int, double> appIDRecValueDictionary = appIDList.ToDictionary<int, int, double>(ID => ID, Value => 0);

            Dictionary<int, List<List<string>>> TagCombosForID = CalculateTagCombosForID(appID, relevantTagRange);

            #region point giving to similar tagged games
            for (int i = relevantTagRange; i >= 1; i--)
            {
                foreach (var combination in TagCombosForID[i])
                {
                    if (TagCombinationWithAppIDs.ContainsKey(combination))
                    {
                        foreach (int ID in TagCombinationWithAppIDs[combination])
                        {
                            appIDRecValueDictionary[ID] += i;
                        }
                    }
                }
            }
            #endregion

            recommendationValue.AddRange(appIDList.Select(ID => appIDRecValueDictionary[ID]));

            Tuple<List<int>, List<double>> appIDWithRecValue = new Tuple<List<int>, List<double>>(appIDList,
            recommendationValue);
            


            return A
        }


        private Dictionary<int, List<List<string>>> CalculateTagCombosForID(int appID, int Range) //Calculate all possible tag combinations for an appID. Range indicates how many tags are relevant.
            // Dictionary returned consist of an int key that represents the amount of tags for the combinations represented in the value
        {
            List<string> appIDTags = new List<string>();
            AppIDTAgDictionary.TryGetValue(appID, out appIDTags);
            Dictionary<int, List<List<string>>> returnDictionary = new Dictionary<int, List<List<string>>>();

            if (appIDTags != null || appIDTags.Count != 0)
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

        


        //private double CompareTags(int appIDOriginal, int appIDCompareTo)
        //{
        //    List<string> appID2List;
        //    AppIDTAgDictionary.TryGetValue(appIDCompareTo, out appID2List);


        //    if (appID2List != null)
        //    {
        //    }

        //}

        //private Dictionary<int, List<string>> getMultipleTagCombinations(int maxTagCombination, int minTagcombination)
        //{
        //    Dictionary<int, List<string>> returnDictionary = new Dictionary<int, List<string>>();

        //    for (int i = minTagcombination; i <= maxTagCombination; i++)
        //    {
        //        Dictionary<int, List<string>> workDictionary = TagCombinationCalculation()


        //    }
        //} 

#endregion
    }
}
