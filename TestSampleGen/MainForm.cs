using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using NAudio.Midi;
using NAudio.Wave;

using Jacobi.Vst.Core;
using System.Xml.Linq;
using MidiVstTest.Utility;

namespace MidiVstTest
{
    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class MainForm : Form
    {
        VSTForm vstForm = null;
        MidiIn midiIn;
        MidiOut midiOut;
        bool isKeyDown = false;

        int testCount = 0;

        public static Dictionary<string, string> LastDirectoryUsed = new Dictionary<string, string>();
        private string recordDestinationPath = "C:/Users/patri_000/TestSamples";

        public MainForm()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();

   
        }

        void LoadToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (vstForm != null)
            {
                vstForm.Dispose();
                vstForm = null;

                showToolStripMenuItem.Enabled = false;
                editParametersToolStripMenuItem.Enabled = false;
                loadToolStripMenuItem.Text = "Load...";
            }
            else
            {
                var ofd = new OpenFileDialog();
                ofd.Title = "Select VST:";
                ofd.Filter = "VST Files (*.dll)|*.dll";
                if (LastDirectoryUsed.ContainsKey("VSTDir"))
                {
                    ofd.InitialDirectory = LastDirectoryUsed["VSTDir"];
                }
                else
                {
                    ofd.InitialDirectory = UtilityAudio.GetVstDirectory();
                }
                DialogResult res = ofd.ShowDialog();

                if (res != DialogResult.OK || !File.Exists(ofd.FileName)) return;

                try
                {
                    if (LastDirectoryUsed.ContainsKey("VSTDir"))
                    {
                        LastDirectoryUsed["VSTDir"] = Directory.GetParent(ofd.FileName).FullName;
                    }
                    else
                    {
                        LastDirectoryUsed.Add("VSTDir", Directory.GetParent(ofd.FileName).FullName);
                    }
                    vstForm = new VSTForm(ofd.FileName, "nix");
                    vstForm.Show();

                    showToolStripMenuItem.Enabled = true;
                    editParametersToolStripMenuItem.Enabled = true;

                    loadToolStripMenuItem.Text = "Unload...";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        }

        void ShowToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (vstForm != null)
            {
                if (vstForm.Visible) vstForm.BringToFront();
                else vstForm.Visible = true;
            }
        }

        void EditParametersToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (vstForm != null)
                vstForm.ShowEditParameters();
        }

        void ButtonClearLogClick(object sender, EventArgs e)
        {
            progressLog1.ClearLog();
        }

        void StartPresetRecording(object sender, EventArgs e)
        {
            var BackgroundThread = new Thread(new ThreadStart(PresetRecording)) {IsBackground = true};
            BackgroundThread.Start();
        }

        void PresetRecording()
        {
            int programNumber = 0;
            int testDataCount = 0;

            // Synth1 can max save 12800 programs
            while (programNumber < 12800)
            {
                if (!UtilityAudio.IsStreamingToDisk())
                {
                    VSTForm.vst.pluginContext.PluginCommandStub.SetProgram(programNumber);
                    var programmName = VSTForm.vst.pluginContext.PluginCommandStub.GetProgramName();

                    // step over empty programs
                    if (programmName != "initial sound")
                    {
                        UtilityAudio.SaveStream(Path.Combine(recordDestinationPath, "Test" + testDataCount + ".wav"));

                        // start audio recording
                        UtilityAudio.StartStreamingToDisk();

                        const byte midiNote = 60; // C4
                        byte midiVelocity = 100;

                        progressLog1.LogMessage(Color.Blue, "TestSample No. " + testDataCount +" recording from presets started...");

                        UtilityAudio.StartReadNonRealtime();

                        // only bother with the keys that trigger midi notes
                        if (VSTForm.vst != null && midiNote != 0)
                        {
                            // start synth processing
                            VSTForm.vst.MIDI_NoteOn(midiNote, midiVelocity);
                        }

                        // sleep for duration how long the note is hold
                        Thread.Sleep(7);

                        midiVelocity = 0;

                        if (VSTForm.vst != null && midiNote != 0)
                        {
                            // stop synth processing
                            VSTForm.vst.MIDI_NoteOn(midiNote, midiVelocity);
                        }

                        var testDataXml =
                            new XDocument(
                                new XElement("Synth1Testdata",
                                    new XElement("TestdataCount", testDataCount),
                                    new XElement("PresetName", programmName)
                                )
                            );

                        for (int i = 0; i < 99; i++)
                        {
                            var parameter = new XElement("Parameter",
                                new XAttribute("index", i), new XAttribute("vstValue", VSTForm.vst.pluginContext.PluginCommandStub.GetParameter(i)));

                            testDataXml.Element("Synth1Testdata")?.Add(parameter);
                        }

                        testDataXml.Save(Path.Combine(recordDestinationPath, "Test" + testDataCount + ".xml"));

                        testDataCount++;
                    }

                    programNumber++;
                }

                Thread.Sleep(5);
            }

        }

        void SelectTestSampleDestination(object sender, EventArgs e)
        {
            using (var fldrDlg = new FolderBrowserDialog())
            {
                if (fldrDlg.ShowDialog() == DialogResult.OK)
                {
                    recordDestinationPath = fldrDlg.SelectedPath;
                }
            }
        }

        void MainFormFormClosing(object sender, FormClosingEventArgs e)
        {
            if (midiIn != null)
            {
                midiIn.Dispose();
                midiIn = null;
            }
            if (midiOut != null)
            {
                midiOut.Dispose();
                midiOut = null;
            }
            if (vstForm != null)
            {
                vstForm.Dispose();
                vstForm = null;
            }
            UtilityAudio.Dispose();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void MainForm_Load_1(object sender, EventArgs e)
        {

        }
    }
}
