using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

using NAudio.Midi;
using NAudio.Utils;

using Jacobi.Vst.Core;
using System.Xml.Linq;

namespace MidiVstTest
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.vSTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editParametersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.progressLog1 = new NAudio.Utils.ProgressLog();
            this.buttonClearLog = new System.Windows.Forms.Button();
            this.loadVstB = new System.Windows.Forms.Button();
            this.showVstB = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.selectDestB = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.vSTToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(758, 33);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // vSTToolStripMenuItem
            // 
            this.vSTToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.showToolStripMenuItem,
            this.editParametersToolStripMenuItem});
            this.vSTToolStripMenuItem.Name = "vSTToolStripMenuItem";
            this.vSTToolStripMenuItem.Size = new System.Drawing.Size(58, 29);
            this.vSTToolStripMenuItem.Text = "VST";
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(248, 34);
            this.loadToolStripMenuItem.Text = "Load...";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.LoadToolStripMenuItemClick);
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.Enabled = false;
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(248, 34);
            this.showToolStripMenuItem.Text = "Show...";
            this.showToolStripMenuItem.Click += new System.EventHandler(this.ShowToolStripMenuItemClick);
            // 
            // editParametersToolStripMenuItem
            // 
            this.editParametersToolStripMenuItem.Enabled = false;
            this.editParametersToolStripMenuItem.Name = "editParametersToolStripMenuItem";
            this.editParametersToolStripMenuItem.Size = new System.Drawing.Size(248, 34);
            this.editParametersToolStripMenuItem.Text = "Edit Parameters...";
            this.editParametersToolStripMenuItem.Click += new System.EventHandler(this.EditParametersToolStripMenuItemClick);
            // 
            // progressLog1
            // 
            this.progressLog1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressLog1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.progressLog1.Location = new System.Drawing.Point(15, 91);
            this.progressLog1.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.progressLog1.Name = "progressLog1";
            this.progressLog1.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.progressLog1.Size = new System.Drawing.Size(724, 500);
            this.progressLog1.TabIndex = 4;
            // 
            // buttonClearLog
            // 
            this.buttonClearLog.Location = new System.Drawing.Point(627, 43);
            this.buttonClearLog.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonClearLog.Name = "buttonClearLog";
            this.buttonClearLog.Size = new System.Drawing.Size(112, 35);
            this.buttonClearLog.TabIndex = 6;
            this.buttonClearLog.Text = "Clear Log";
            this.buttonClearLog.UseVisualStyleBackColor = true;
            this.buttonClearLog.Click += new System.EventHandler(this.ButtonClearLogClick);
            // 
            // loadVstB
            // 
            this.loadVstB.Location = new System.Drawing.Point(15, 45);
            this.loadVstB.Name = "loadVstB";
            this.loadVstB.Size = new System.Drawing.Size(112, 35);
            this.loadVstB.TabIndex = 7;
            this.loadVstB.Text = "Load Vst";
            this.loadVstB.UseVisualStyleBackColor = true;
            this.loadVstB.Click += new System.EventHandler(this.LoadToolStripMenuItemClick);
            // 
            // showVstB
            // 
            this.showVstB.Location = new System.Drawing.Point(133, 45);
            this.showVstB.Name = "showVstB";
            this.showVstB.Size = new System.Drawing.Size(112, 35);
            this.showVstB.TabIndex = 8;
            this.showVstB.Text = "Show Vst";
            this.showVstB.UseVisualStyleBackColor = true;
            this.showVstB.Click += new System.EventHandler(this.ShowToolStripMenuItemClick);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(15, 602);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(200, 35);
            this.button3.TabIndex = 9;
            this.button3.Text = "Start Preset Recording";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.StartPresetRecording);
            // 
            // selectDestB
            // 
            this.selectDestB.Location = new System.Drawing.Point(221, 602);
            this.selectDestB.Name = "selectDestB";
            this.selectDestB.Size = new System.Drawing.Size(200, 35);
            this.selectDestB.TabIndex = 10;
            this.selectDestB.Text = "Select Destination";
            this.selectDestB.UseVisualStyleBackColor = true;
            this.selectDestB.Click += new System.EventHandler(this.SelectTestSampleDestination);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(758, 649);
            this.Controls.Add(this.selectDestB);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.showVstB);
            this.Controls.Add(this.loadVstB);
            this.Controls.Add(this.buttonClearLog);
            this.Controls.Add(this.progressLog1);
            this.Controls.Add(this.menuStrip1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "MainForm";
            this.Text = "MidiVstTest";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormFormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load_1);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		private System.Windows.Forms.Button buttonClearLog;
		private NAudio.Utils.ProgressLog progressLog1;
		private System.Windows.Forms.ToolStripMenuItem editParametersToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem vSTToolStripMenuItem;
		private System.Windows.Forms.MenuStrip menuStrip1;
        private Button loadVstB;
        private Button showVstB;
        private Button button3;
        private Button selectDestB;
    }
}
