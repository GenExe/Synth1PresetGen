using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Linq;

namespace MidiVstTest.Utility
{
    static class Synth1PresetUtility
    {
        public static void CreateDifferenceJson(string preset1Path, string preset2Path)
        {
            var presets = new List<Synth1Preset>();
            //check if the path is a folder
            if (Path.HasExtension(preset1Path) && Path.HasExtension(preset2Path))
            {
                //var fileInfo1 = new FileInfo(preset1Path);
                //var fileInfo2 = new FileInfo(preset2Path);

                XDocument doc1 = XDocument.Load(preset1Path);
                XDocument doc2 = XDocument.Load(preset2Path);

                string name = doc1.Element("Synth1Preset").Attribute("name").Value + doc2.Element("Synth1Preset").Attribute("name").Value + "dif.xml";

                foreach (var parameter in doc1.Element("Synth1Preset").Elements())
                {
                    var parameter2 = doc2.Element("Synth1Preset").Elements().Where(e => e.Attribute("index").Value == parameter.Attribute("index").Value).First();
                    var value1 = Convert.ToSingle(parameter.Attribute("value").Value, CultureInfo.InvariantCulture);
                    var value2 = Convert.ToSingle(parameter2.Attribute("value").Value, CultureInfo.InvariantCulture);

                    Console.WriteLine(parameter.Attribute("index").Value + "  " + (value2 - value1));
                    
                }
            }
        }

        static void CreateV112PresetsFromRange(int start, int end)
        {
            var lines = new List<string>();
            for(int i = start; i <= end; i++)
            {
                lines.Add(i.ToString());
                lines.Add("color=default");
                lines.Add("ver=113");
               
                lines.Add("0," + i.ToString());
                lines.Add("45," + i.ToString());
                lines.Add("76," + i.ToString());
                lines.Add("1," + i.ToString());
                lines.Add("2," + i.ToString());
                lines.Add("3," + i.ToString());
                lines.Add("4," + i.ToString());
                lines.Add("5," + i.ToString());
                lines.Add("6," + i.ToString());
                lines.Add("7," + i.ToString());
                lines.Add("8," + i.ToString());
                lines.Add("9," + i.ToString());
                lines.Add("10," + i.ToString());
                lines.Add("11," + i.ToString());
                lines.Add("12," + i.ToString());
                lines.Add("13," + i.ToString());
                lines.Add("71," + i.ToString());
                lines.Add("72," + i.ToString());
                lines.Add("91," + i.ToString());
                lines.Add("95," + i.ToString());
                lines.Add("96," + i.ToString());
                lines.Add("97," + i.ToString());
                lines.Add("14," + i.ToString());
                lines.Add("15," + i.ToString());
                lines.Add("16," + i.ToString());
                lines.Add("17," + i.ToString());
                lines.Add("18," + i.ToString());
                lines.Add("19," + i.ToString());
                lines.Add("20," + i.ToString());
                lines.Add("21," + i.ToString());
                lines.Add("22," + i.ToString());
                lines.Add("23," + i.ToString());
                lines.Add("24," + i.ToString());
                lines.Add("25," + i.ToString());
                lines.Add("26," + i.ToString());
                lines.Add("27," + i.ToString());
                lines.Add("28," + i.ToString());
                lines.Add("29," + i.ToString());
                lines.Add("30," + i.ToString());
                lines.Add("59," + i.ToString());
                lines.Add("31," + i.ToString());
                lines.Add("32," + i.ToString());
                lines.Add("33," + i.ToString());
                lines.Add("34," + i.ToString());
                lines.Add("65," + i.ToString());
                lines.Add("82," + i.ToString());
                lines.Add("35," + i.ToString());
                lines.Add("83," + i.ToString());
                lines.Add("36," + i.ToString());
                lines.Add("98," + i.ToString());
                lines.Add("37," + i.ToString());
                lines.Add("66," + i.ToString());
                lines.Add("64," + i.ToString());
                lines.Add("52," + i.ToString());
                lines.Add("53," + i.ToString());
                lines.Add("54," + i.ToString());
                lines.Add("55," + i.ToString());
                lines.Add("56," + i.ToString());
                lines.Add("60," + i.ToString());
                lines.Add("61," + i.ToString());
                lines.Add("62," + i.ToString());
                lines.Add("63," + i.ToString());
                lines.Add("90," + i.ToString());
                lines.Add("77," + i.ToString());
                lines.Add("78," + i.ToString());
                lines.Add("79," + i.ToString());
                lines.Add("80," + i.ToString());
                lines.Add("81," + i.ToString());
                lines.Add("38," + i.ToString());
                lines.Add("94," + i.ToString());
                lines.Add("39," + i.ToString());
                lines.Add("74," + i.ToString());
                lines.Add("73," + i.ToString());
                lines.Add("93," + i.ToString());
                lines.Add("75," + i.ToString());
                lines.Add("84," + i.ToString());
                lines.Add("85," + i.ToString());
                lines.Add("92," + i.ToString());
                lines.Add("40," + i.ToString());
                lines.Add("86," + i.ToString());
                lines.Add("50," + i.ToString());
                lines.Add("87," + i.ToString());
                lines.Add("88," + i.ToString());
                lines.Add("51," + i.ToString());
                lines.Add("89," + i.ToString());
                lines.Add("57," + i.ToString());
                lines.Add("41," + i.ToString());
                lines.Add("42," + i.ToString());
                lines.Add("43," + i.ToString());
                lines.Add("44," + i.ToString());
                lines.Add("67," + i.ToString());
                lines.Add("68," + i.ToString());
                lines.Add("58," + i.ToString());
                lines.Add("46," + i.ToString());
                lines.Add("47," + i.ToString());
                lines.Add("48," + i.ToString());
                lines.Add("49," + i.ToString());
                lines.Add("69," + i.ToString());
                lines.Add("70," + i.ToString());
                System.IO.File.WriteAllLines(@".\" + (i + 1).ToString("D3") + ".sy1", lines);
                lines.Clear();
            }
        }

        // used for first time generation

        //public static void Main(string[] args)
        //{
        //    createPresetsFromRange(0, 127);
        //}
    }
}
