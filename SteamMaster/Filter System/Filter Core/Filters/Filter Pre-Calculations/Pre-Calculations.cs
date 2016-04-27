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
        private Dictionary<List<string>, int> TagCombinationDictionary = new Dictionary<List<string>, int>(new ListComparer()); 

        public void DiversifictaionCalculation()
        {
            TagCombinationCalculation();
            // test region
            #region

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


        private void TagCombinationCalculation()
        {
            for (int i = 0; i < AppIDWithTags.Item1.Count; i++)
            {
                List<string> tagList = AppIDWithTags.Item2[i];
                if (tagList.Count > 4 && !tagList.Contains("Trains"))
                {
                    int range = 15;
                    tagList = tagList.GetRange(0, tagList.Count < range ? tagList.Count : range);
                    var result = tagList.Combinations(5);
                    foreach (var entry in result)
                    {
                        List<string> workList = entry.ToList();
                        AddOrCountToCombinationDictionary(workList);
                    }
                }

            }
        }

        private void AddOrCountToCombinationDictionary(List<string> tagCombinationList)
        {
            tagCombinationList.Sort();

            if (TagCombinationDictionary.ContainsKey(tagCombinationList))
            {
                TagCombinationDictionary[tagCombinationList]++;
            }
            else if (!tagCombinationList.Contains("Action") && !tagCombinationList.Contains("Indie") && !tagCombinationList.Contains("TrackIR"))
            {
                TagCombinationDictionary.Add(tagCombinationList, 1);
            }
        }

        private void LoadTags()
        {
            TagReader tagReader = new TagReader(@"C:\Users\jeppe\Dropbox\Software\Projekt\P2\TagsForAppID.txt");
            AppIDWithTags = tagReader.AppIDWithTag();
        }
    }
}
