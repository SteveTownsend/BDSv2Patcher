using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace BDSPatcher
{
    public class Config
    {
        public IList<string[]> skipNifs { get; }

        private IList<string[]> ParseNifFilter(string filterData)
        {
            IList<string[]> nifFilter = new List<string[]>();
            if (!String.IsNullOrEmpty(filterData))
            {
                foreach (string filter in filterData.Split('|'))
                {
                    string[] filterElements = filter.Split(',');
                    if (filterElements.Length > 0)
                    {
                        nifFilter.Add(filterElements);
                    }
                }
            }
            return nifFilter;
        }

        public Config(string configFilePath)
        {
            // override if config is well-formed
            if (!File.Exists(configFilePath))
            {
                Console.WriteLine("\"config.json\" cannot be found in the users Data folder, aborting.");
                throw new InvalidDataException("\"config.json\" cannot be found in the users Data folder, aborting.");
            }
            else
            {
                JObject configJson = JObject.Parse(File.ReadAllText(configFilePath));
                var generalKeys = configJson["general"]!;
                skipNifs = ParseNifFilter((string)generalKeys["skipNifs"]!);
            }
        }

        public bool IsNifValid(string nifPath)
        {
            // check blacklist, exclude NIF if all substrings in an entry match
            foreach (string[] filterElements in skipNifs)
            {
                if (filterElements
                    .Where(x => !string.IsNullOrEmpty(x))
                    .All(v => nifPath.Contains(v, StringComparison.OrdinalIgnoreCase)))
                    return false;
            }
            // if not blacklisted, good to go
            return true;
        }
    }
}
