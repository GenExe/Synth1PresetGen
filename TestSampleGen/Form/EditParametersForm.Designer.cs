namespace TestSampleGen
{
    partial class EditParametersForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbParameters = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // lbParameters
            // 
            this.lbParameters.FormattingEnabled = true;
            this.lbParameters.ItemHeight = 20;
            this.lbParameters.Location = new System.Drawing.Point(18, 18);
            this.lbParameters.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lbParameters.Name = "lbParameters";
            this.lbParameters.Size = new System.Drawing.Size(388, 364);
            this.lbParameters.TabIndex = 0;
            // 
            // EditParametersForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ClientSize = new System.Drawing.Size(426, 403);
            this.Controls.Add(this.lbParameters);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "EditParametersForm";
            this.ShowInTaskbar = false;
            this.Text = "Edit Parameters...";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EditParametersForm_FormClosing);
            this.Load += new System.EventHandler(this.EditParametersForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lbParameters;
    }
}