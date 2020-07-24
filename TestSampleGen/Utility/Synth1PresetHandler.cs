using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace MidiVstTest.Utility
{
    public static class Synth1PresetHandler
    {
        private static Dictionary<int, Dictionary<int, float>>[] _parameterTables;
        private static Dictionary<int, Dictionary<float, int>> _parameterTableV113;

        public static List<Synth1Preset> GetPresetsFromFolderAndSubfolders(string folderPath)
        {
            var presets = new List<Synth1Preset>();

            //check if the path is a folder

            if (Path.HasExtension(folderPath)) return presets;

            DirectoryInfo d = new DirectoryInfo(folderPath);

            foreach (var file in d.GetFiles("*.sy1", SearchOption.AllDirectories))
            {
                var fileLines = File.ReadAllLines(file.FullName);

                if (fileLines.Length < 15) continue;

                var synth1Preset = new Synth1Preset(fileLines);

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
                    vstParameters.Add(parameter.Key, ConvertParaToVstPara(parameter.Key, parameter.Value, preset.Version1));
                }
            }

            return vstParameters;
        }

        public static float ConvertParaToVstPara(int parameter, int value, int version)
        {
            CreateParameterTables(@"C:\Repos\Synth1PresetGen\TestSampleGen\Synth1ParaConvertFiles");
            if (value > 127) value = 127;

            return _parameterTables[version][value][parameter];
        }

        public static int ConvertVstParaToParaV113(int parameter, float value)
        {
            CreateParameterTables(@"C:\Repos\Synth1PresetGen\TestSampleGen\Synth1ParaConvertFiles");



            return _parameterTableV113[parameter][value];
        }

        private static void CreateParameterTables(string folderPath)
        {
            if (_parameterTables != null) return;
            _parameterTables = new Dictionary<int, Dictionary<int, float>>[14];
            // if the path points not to a folder
            if (Path.HasExtension(folderPath)) return;

            var directory = new DirectoryInfo(folderPath);



            foreach (var d in directory.GetDirectories())
            {
                int version = int.Parse(d.Name) - 100;
                _parameterTables[version] = new Dictionary<int, Dictionary<int, float>>();



                var files = d.GetFiles("*.xml");

                foreach (var file in files)
                {
                    var doc1 = XDocument.Load(file.FullName);
                    var paraValue = int.Parse(doc1.Element("Synth1PresetV" + d.Name).Attribute("name").Value);
                    _parameterTables[version].Add(paraValue, new Dictionary<int, float>());

                    foreach (var parameter in doc1.Element("Synth1PresetV" + d.Name).Elements())
                    {
                        var index = int.Parse(parameter.Attribute("index").Value);
                        var tempValue = Convert.ToSingle(parameter.Attribute("value").Value, CultureInfo.InvariantCulture);
                        _parameterTables[version][paraValue].Add(index, tempValue);
                    }
                }
            }
            _parameterTableV113 = new Dictionary<int, Dictionary<float, int>>();
            foreach (var keyValuePair in _parameterTables[13][1])
            {
                _parameterTableV113.Add(keyValuePair.Key, new Dictionary<float, int>());
            }
            for (int i = 0; i < 128; i++)
            {
                foreach (var pair in _parameterTables[13][i])
                {
                    if(!_parameterTableV113[pair.Key].ContainsKey(pair.Value)) _parameterTableV113[pair.Key].Add(pair.Value, i);
                }
            }
        }


        //public static void Main(string[] args)
        //{
        //    //var presets = GetPresetsFromFolderAndSubfolders(@"C:\Users\patri_000\Desktop\Synth1Presets\Ami Evan\Ami Evan Bank");

        //    //Console.WriteLine(presets[0].Name + "  Para1 Preset Value: " + presets[0].Parameters[5] + "  Vst Value:" + ConvertParaToVstPara(5, presets[0].Parameters[5], presets[0].Version1));

        //    var number = ConvertParaToVstPara(14, 6, 13);

        //    Console.WriteLine(number);

        //    var integ = ConvertVstParaToParaV113(14, number);

        //    Console.WriteLine(integ);

        //}


    }
}
