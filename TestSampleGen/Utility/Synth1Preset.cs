using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;


namespace TestSampleGen.Utility
{
    public class Synth1Preset
    {
        public string Name { get; }
        public int Version1 { get; }
        public Dictionary<int, int> Parameters { get; }

        public Synth1Preset(string[] fileLines) { 
        
            Name = fileLines[0];
            Version1 = int.Parse(Regex.Match(fileLines[2], @"\d+").Value);
            Parameters = new Dictionary<int, int>();
            for (int i = 3; i < fileLines.Length; i++)
            {
                if (Regex.IsMatch(fileLines[i], "^[0-9]?[0-9]?[0-9]?,[0-9]?[0-9]?[0-9]?$"))
                {
                    var parameternumber = int.Parse(Regex.Match(fileLines[i], "^[0-9]?[0-9]?[0-9]?").Value);
                    var parameterValue = int.Parse(Regex.Match(fileLines[i], "[0-9]?[0-9]?[0-9]?$").Value);
                    Parameters.Add(parameternumber, parameterValue);
                }
            }
        }
    }
}
