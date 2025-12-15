// Add simple controls to form: three listboxes and buttons
using System.Windows.Forms;
using System.Drawing;

namespace Projet_Victor_c_
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            this.listBoxHosts = new ListBox();
            this.listBoxIf = new ListBox();
            this.listBoxRules = new ListBox();
            this.btnAddHost = new Button();
            this.btnAddIf = new Button();
            this.btnAddRule = new Button();
            this.btnSave = new Button();

            this.SuspendLayout();
            // 
            // listBoxHosts
            // 
            this.listBoxHosts.Location = new Point(12, 12);
            this.listBoxHosts.Size = new Size(240, 300);
            // 
            // listBoxIf
            // 
            this.listBoxIf.Location = new Point(262, 12);
            this.listBoxIf.Size = new Size(240, 300);
            // 
            // listBoxRules
            // 
            this.listBoxRules.Location = new Point(512, 12);
            this.listBoxRules.Size = new Size(260, 300);
            // 
            // btnAddHost
            // 
            this.btnAddHost.Location = new Point(12, 320);
            this.btnAddHost.Size = new Size(75, 23);
            this.btnAddHost.Text = "Add Host";
            // 
            // btnAddIf
            // 
            this.btnAddIf.Location = new Point(93, 320);
            this.btnAddIf.Size = new Size(75, 23);
            this.btnAddIf.Text = "Add Interface";
            // 
            // btnAddRule
            // 
            this.btnAddRule.Location = new Point(174, 320);
            this.btnAddRule.Size = new Size(75, 23);
            this.btnAddRule.Text = "Add Rule";
            // 
            // btnSave
            // 
            this.btnSave.Location = new Point(512, 320);
            this.btnSave.Size = new Size(75, 23);
            this.btnSave.Text = "Save";

            // 
            // Form1
            // 
            this.ClientSize = new Size(800, 450);
            this.Controls.Add(this.listBoxHosts);
            this.Controls.Add(this.listBoxIf);
            this.Controls.Add(this.listBoxRules);
            this.Controls.Add(this.btnAddHost);
            this.Controls.Add(this.btnAddIf);
            this.Controls.Add(this.btnAddRule);
            this.Controls.Add(this.btnSave);
            this.Text = "Network Manager - Simple";
            this.ResumeLayout(false);
        }

        #endregion

        private ListBox listBoxHosts;
        private ListBox listBoxIf;
        private ListBox listBoxRules;
        private Button btnAddHost;
        private Button btnAddIf;
        private Button btnAddRule;
        private Button btnSave;
    }
}
