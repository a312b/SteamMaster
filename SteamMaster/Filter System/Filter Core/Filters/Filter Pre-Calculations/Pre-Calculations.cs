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
        }

        private Tuple<List<int>, List<List<string>>> AppIDWithTags;
        private Dictionary<int, List<string>> AppIDTAgDictionary = new Dictionary<int, List<string>>(); 

        private void LoadTags()
        {
            TagReader tagReader = new TagReader(@"C:\Users\jeppe\Dropbox\Software\Projekt\P2\TagsForAppID.txt");
            AppIDWithTags = tagReader.AppIDWithTag();

            for (int i = 0; i < AppIDWithTags.Item1.Count; i++)
            {
                AppIDTAgDictionary.Add(AppIDWithTags.Item1[i], AppIDWithTags.Item2[i]);
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

        private Tuple<List<int>, List<double>> GetAppsSimilarToApp(int appID)
        {
            List<int> appIDList = AppIDWithTags.Item1;
            List<double> recommendationValue = new List<double>();

            Tuple<List<int>, List<double>> appIDWithRecValue = new Tuple<List<int>, List<double>>(appIDList,
                recommendationValue);

            foreach (var appEntry in appIDList)
            {
            }

            return appIDWithRecValue;
        }


        private Dictionary<int, List<List<string>>> CalculateTagCombosForID(int appID, int Range) //Calculate all possible tag combinations for a appID. Range indicates how many tags are relevant.
        {
            List<string> appIDTags = new List<string>();
            AppIDTAgDictionary.TryGetValue(appID, out appIDTags);
            Dictionary<int, List<List<string>>> returnDictionary = new Dictionary<int, List<List<string>>>();

            int TagRange = appIDTags.Count < Range ? appIDTags.Count : Range;

            for (int i = TagRange; i >= 0; i--)
            {
                IEnumerable<IEnumerable<string>> tagIEnumerable =
                    appIDTags.GetRange(0, TagRange)
                        .Combinations(i);

                List<List<string>> tagList = tagIEnumerable.Select(entry => entry.ToList()).ToList();
                returnDictionary.Add(i, tagList);
            }
            return returnDictionary;
        }

        private double CompareTags(int appIDOriginal, int appIDCompareTo)
        {
            List<string> appID2List;
            AppIDTAgDictionary.TryGetValue(appIDCompareTo, out appID2List);


            if (appID2List != null)
            {
            }

        }

        private Dictionary<int, List<string>> getMultipleTagCombinations(int maxTagCombination, int minTagcombination)
        {
            Dictionary<int, List<string>> returnDictionary = new Dictionary<int, List<string>>();

            for (int i = minTagcombination; i <= maxTagCombination; i++)
            {
                Dictionary<int, List<string>> workDictionary = TagCombinationCalculation()


            }
        } 

#endregion
    }
}
