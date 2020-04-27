using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Linq;

namespace TestSampleGen.Utility
{
    public static class Synth1PresetHandler
    {
        private static Dictionary<int, Dictionary<int, float>> _parameterTable;

        public static List<Synth1Preset> GetPresets(string folderPath)
        {
            var presets = new List<Synth1Preset>();

            //check if the path is a folder

            if (Path.HasExtension(folderPath)) return presets;

            DirectoryInfo d = new DirectoryInfo(folderPath);

            foreach (var file in d.GetFiles("*.sy1"))
            {

                var synth1Preset = new Synth1Preset(File.ReadAllLines(file.FullName));

                Console.WriteLine(synth1Preset.Name + synth1Preset.Version1);
                    
                presets.Add(synth1Preset);
            }
            return presets;
        }

        public static Dictionary<int, float> VstParametersFromPreset(Synth1Preset preset)
        {
            //TODO: logic for different preset versions

            var vstParameters = new Dictionary<int, float>();

            if (preset != null)
            {
                foreach (var parameter in preset.Parameters)
                {
                    vstParameters.Add(parameter.Key, ConvertParaToVstPara(parameter.Key, parameter.Value));
                }
            }

            return vstParameters;
        }

        private static float ConvertParaToVstPara(int parameter, int value)
        {
            CreateParameterTables();

            return _parameterTable[value][parameter];
        }

        private static void CreateParameterTables()
        {
            //TODO: logic for different preset versions

            if (_parameterTable != null) return;

            _parameterTable = new Dictionary<int, Dictionary<int, float>>();

            const string folderPath = @"C:\Repos\PresetGen\TestSampleGen\Synth1ParaConvertFiles";

            // if the path points not to a folder
            if (Path.HasExtension(folderPath)) return;

            var d = new DirectoryInfo(folderPath);

            var files = d.GetFiles("*.xml");

            foreach (var file in files)
            {
                var doc1 = XDocument.Load(file.FullName);
                var paraValue = int.Parse(doc1.Element("Synth1Preset").Attribute("name").Value);
                _parameterTable.Add(paraValue, new Dictionary<int, float>());

                foreach (var parameter in doc1.Element("Synth1Preset").Elements())
                {
                    var index = int.Parse(parameter.Attribute("index").Value);
                    var tempValue = Convert.ToSingle(parameter.Attribute("value").Value, CultureInfo.InvariantCulture);
                    _parameterTable[paraValue].Add(index, tempValue);
                }
            }
        }


        //public static void Main(string[] args)
        //{
        //    var presets = GetPresets(@"C:\Repos\PresetGen\TestSampleGen\Synth1Presets");

        //    Console.WriteLine(presets[0].Name + "  Para1 Preset Value: " + presets[0].Parameters[5] + "  Vst Value:" + ConvertParaToVstPara(5, presets[0].Parameters[5]));

        //}


    }
}
