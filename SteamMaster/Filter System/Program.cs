using System;
using System.Collections.Generic;
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

            //Dictionary<int, string> woop = new Dictionary<int, string>();

            // woop.Add(1, "Hej");
            // woop.Add(2, "Dig");

            // woop[2] = "Loooooow rider";

            // Console.WriteLine($"{woop[2]}");

            // Console.ReadKey();


            //string dob = "wub";
            //dob += "balubba";

            //string dib = "wabbalubba";
            //string deb = "wubbalubbasdfsdfsdfsdfsdgherheheeddddddddddddddddddddddddddddddh";

            //Console.WriteLine(dob.GetHashCode());
            //Console.WriteLine(dib.GetHashCode());
            //Console.WriteLine(deb.GetHashCode());

            //List<int> tal = new List<int>();

            //for (int i = 0; i < 20; i++)
            //{
            //    tal.Add(i + 1);
            //}

            //var result = UseFullMethods.Combinations(tal, 3);

            //int combinations = 0;

            //foreach (var entry in result)
            //{
            //    List<int> printList = entry.ToList();
            //    foreach (var number in printList)
            //    {
            //        Console.Write(number);
            //    }
            //    Console.WriteLine();
            //    combinations++;
            //}

            //Console.WriteLine();
            //Console.WriteLine("Combinationer: " + combinations);


            Pre_Calculations calculations = new Pre_Calculations();
            calculations.DiversifictaionCalculation();

        }
    }
}
