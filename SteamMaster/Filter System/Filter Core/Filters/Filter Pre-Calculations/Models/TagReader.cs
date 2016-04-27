using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filter_System.Filter_Core.Filters.Filter_Pre_Calculations
{
    class TagReader
    {
        private List<List<string>> TagMatrix = new List<List<string>>();
        Dictionary<string, int> TagEnumerator = new Dictionary<string, int>();
        private List<string> AppID = new List<string>();
        private StreamReader reader;

        private List<int> IntAppID = new List<int>(); 

        public TagReader(string path)
        {
            reader = new StreamReader(path);
            Start();
        }

        public Tuple<List<string>, List<List<int>>> GenerateTagMatrix()
        {
            List<List<int>> TagNumberMatrix = new List<List<int>>();

            foreach (List<string> list in TagMatrix)
            {
                TagNumberMatrix.Add(GetTagVector(list));
            }
            return new Tuple<List<string>, List<List<int>>>(AppID, TagNumberMatrix);

        }

        private List<int> GetTagVector(List<string> list)
        {
            int[] vector = new int[TagEnumerator.Count];

            foreach (var pair in TagEnumerator)
            {
                if (list.Contains(pair.Key))
                    vector[pair.Value] = 1;
            }
            return vector.ToList();

        }

        public void Start()
        {
            while (!reader.EndOfStream)
            {
                string[] lineSegments = reader.ReadLine().Split(':');
                string ID = lineSegments[0];
                List<string> tags = lineSegments[1].Split(',').ToList();
                TagMatrix.Add(tags);
               // AppID.Add(ID);

                int appID;
                int.TryParse(ID, out appID);
                IntAppID.Add(appID);
                
            }
            reader.Close();
            GenerateTagEnumerator();

        }
        private void GenerateTagEnumerator()
        {
            int tagNumber = 0;
            foreach (List<string> list in TagMatrix)
            {
                foreach (string tag in list)
                {
                    if (TagEnumerator.ContainsKey(tag)) continue;
                    if (!string.IsNullOrWhiteSpace(tag))
                        TagEnumerator.Add(tag, tagNumber++);
                }
            }

        }

        public Tuple<List<int>, List<List<string>>> AppIDWithTag()
        {
            return new Tuple<List<int>, List<List<string>>>(IntAppID, TagMatrix);
        } 
    }
}
