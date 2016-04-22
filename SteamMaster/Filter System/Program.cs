using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filter_System
{
    class Program
    {
        static void Main(string[] args)
        {
           Dictionary<int, string> woop = new Dictionary<int, string>();

            woop.Add(1, "Hej");
            woop.Add(2, "Dig");

            woop[2] = "Loooooow rider";

            Console.WriteLine($"{woop[2]}");

            Console.ReadKey();
        }
    }
}
