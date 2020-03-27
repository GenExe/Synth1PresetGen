using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using CommonUtils.VSTPlugin;
using Jacobi.Vst.Interop.Host;
using NAudio.Wave;
using TestSampleGen;

// Copied from the microDRUM project
// https://github.com/microDRUM
// I think it is created by massimo.bernava@gmail.com
// Modified by perivar@nerseth.com, patrick0steurer@gmail.com
namespace MidiVstTest.Utility
{
    public enum AudioLibrary
    {
        Null,
        NAudio
    }

    public static class UtilityAudio
    {
        private static AudioLibrary _usedLibrary = AudioLibrary.Null;
        private static VST _generalVst = null;
        private static MixerForm _mixerForm = null;

        //OWN
        private static bool _streamingToDisk = false;

        // NAUDIO
        private static List<WaveStream> _samples = new List<WaveStream>();
        private static IWavePlayer _playbackDevice = null;
        private static RecordableMixerStream32 _mixer32 = null;

        private static VSTStream _vstStream = null;
        private static WaveChannel32 _mp3Channel = null;
        private static long _mp3Position = 0;

        //=============================================

        public static bool OpenAudio(AudioLibrary audioLibrary, string asioDevice)
        {
            if (_usedLibrary != AudioLibrary.Null || (_playbackDevice != null && _playbackDevice.PlaybackState == PlaybackState.Playing)) return false;

            _usedLibrary = audioLibrary;

            if (_usedLibrary == AudioLibrary.NAudio)
            {
                // NAUDIO
                _mixer32 = new RecordableMixerStream32 { AutoStop = false };

                // remove this comments if you wanna hear the sounds, but then only realtime recording is possible
                // 
                //if (asioDevice != null) {
                //	playbackDevice = new AsioOut(asioDevice);
                //}

                //// if failed, try normal wave out
                //if (playbackDevice == null) playbackDevice = new WaveOut();
                //if (playbackDevice == null) return false;
                //playbackDevice.Init(Mixer32);

                //=============================================
            }
            return true;
        }

        public static void StartAudio()
        {
            _playbackDevice?.Play();
        }


        public static void Dispose()
        {
            _mp3Channel?.Dispose();

            DisposeVst();

            if (_usedLibrary == AudioLibrary.NAudio)
            {
                // NAUDIO
                _samples.Clear();
                if (_playbackDevice != null)
                {
                    _playbackDevice.Stop();
                    _playbackDevice.Dispose();
                    _playbackDevice = null;
                }

                if (_mixer32 != null) { _mixer32.Dispose(); _mixer32 = null; }
            }
            _usedLibrary = AudioLibrary.Null;
        }





        public static VST LoadVst(string vstPath, IntPtr hWnd)
        {
            DisposeVst();

            _generalVst = new VST();

            var hcs = new HostCommandStub { Directory = System.IO.Path.GetDirectoryName(vstPath) };

            try
            {
                _generalVst.pluginContext = VstPluginContext.Create(vstPath, hcs);
                _generalVst.pluginContext.PluginCommandStub.Open();
                _generalVst.pluginContext.PluginCommandStub.SetProgram(13000);
                _generalVst.pluginContext.PluginCommandStub.EditorOpen(hWnd);
                _generalVst.pluginContext.PluginCommandStub.MainsChanged(true);

                _vstStream = new VSTStream();
                _vstStream.ProcessCalled += _generalVst.Stream_ProcessCalled;
                _vstStream.pluginContext = _generalVst.pluginContext;
                _vstStream.SetWaveFormat(44100, 2);

                _mixer32.AddInputStream(_vstStream);

                return _generalVst;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return null;
        }

        //TODO: weck
        public static void LoadMp3(string mp3Path)
        {
            var mp3Reader = new Mp3FileReader(mp3Path);
            WaveStream pcmStream = WaveFormatConversionStream.CreatePcmStream(mp3Reader);
            WaveStream blockAlignedStream = new BlockAlignReductionStream(pcmStream);

            _mp3Channel?.Dispose();
            _mp3Channel = new WaveChannel32(blockAlignedStream);
            _mp3Position = 0;
            _mixerForm = new MixerForm(_mp3Channel);
        }
        //TODO: weck
        public static void PlayMp3()
        {
            if (_mp3Channel == null || _mixer32.ContainsInputStream(_mp3Channel)) return;

            _mixer32.AddInputStream(_mp3Channel);
            _mp3Channel.Position = _mp3Position;
        }
        //TODO: weck
        public static void PauseMp3()
        {
            _mp3Position = _mp3Channel.Position;
            _mixer32.RemoveInputStream(_mp3Channel);
        }
        //TODO: weck
        public static void StopMp3()
        {
            _mp3Position = 0;
            _mp3Channel.CurrentTime = TimeSpan.Zero;
            _mixer32.RemoveInputStream(_mp3Channel);
        }
        //TODO: weck
        public static TimeSpan GetMp3TotalTime()
        {
            return _mp3Channel?.TotalTime ?? TimeSpan.Zero;
        }
        //TODO: weck
        public static TimeSpan GetMp3CurrentTime()
        {
            return _mp3Channel?.CurrentTime ?? TimeSpan.Zero;
        }

        public static string GetVstDirectory()
        {
            var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE")?.OpenSubKey("VST");
            if (key != null) return key.GetValue("VstPluginsPath").ToString();
            else
            {
                key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE")?.OpenSubKey("XLN Audio\\Addictive Drums");
                return key?.GetValue("VSTPath").ToString();
            }
        }

        public static void DisposeVst()
        {
            _mixer32?.RemoveInputStream(_vstStream);

            _vstStream?.Dispose();
            _vstStream = null;

            _generalVst?.Dispose();
            _generalVst = null;
        }

        //TODO: weck
        public static bool IsMp3Played()
        {
            return _mixer32.ContainsInputStream(_mp3Channel);
        }
        //TODO: weck
        public static void ShowMixer()
        {
            _mixerForm?.Show();
        }

        public static void SaveStream(string savePath)
        {
            _mixer32.StreamMixToDisk(savePath);
        }

        public static void StartStreamingToDisk()
        {
            _mixer32.StartStreamingToDisk();
            _streamingToDisk = true;
        }

        public static void StopStreamingToDisk()
        {
            _mixer32.StopStreamingToDisk();
            _streamingToDisk = false;
        }

        public static bool IsStreamingToDisk()
        {
            return _streamingToDisk;
        }

        public static void StartReadNonRealtime()
        {
            var thread = new Thread(ReadNonRealtime);
            thread.Start();
        }

        public static void ReadNonRealtime()
        {
            while (_streamingToDisk)
            {
                _mixer32.Read(new byte[4048], 0, 4048);
            }
        }
    }
}
