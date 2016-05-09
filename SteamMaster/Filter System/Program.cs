using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Filter_System.Filter_Core.Filters.Filter_Pre_Calculations;
using Filter_System.Filter_Core.Filters.Filter_Pre_Calculations.Models;

namespace Filter_System
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch timeStopwatch = Stopwatch.StartNew();

            Pre_Calculations calculations = new Pre_Calculations();
            calculations.DiversifictaionCalculation();

            //timeStopwatch.Stop();

            //Console.WriteLine($"Elapsed Time for constructor methods: {timeStopwatch.Elapsed}");

            //timeStopwatch.Reset();
            //timeStopwatch.Start();

            //calculations.FileAppSimilarity();

            //timeStopwatch.Stop();

            //Console.WriteLine($"Elapsed Time for calculating AppSimilarity: {timeStopwatch.Elapsed}");

            //var what = calculations.TagCombinationWithAppIDs;


            //Console.WriteLine(what.Count);


            List<AppWorkClass> wubbaList = calculations.GetAppsSimilarToApp(271590, 6);

            wubbaList.Sort();

            foreach (var entry in wubbaList)
            {
                if (entry.Score < 100)
                {
                    Console.WriteLine(entry.AppID + " score: " + entry.Score);
                }
            }



            Console.WriteLine("Donas");
            Console.ReadKey();
        }
    }
}
