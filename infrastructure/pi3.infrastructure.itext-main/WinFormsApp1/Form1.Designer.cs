// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System.Windows.Forms;

namespace WinFormsApp1
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
            lblStatus = new Label();
            txtConnectionInfo = new TextBox();
            btnSign = new Button();

            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            notifyIcon = new NotifyIcon(components);
            trayContextMenu = new ContextMenuStrip(components);
            signMenuItem = new ToolStripMenuItem();
            configMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            exitMenuItem = new ToolStripMenuItem();

            SuspendLayout();
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(65, 373);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(59, 25);
            lblStatus.TabIndex = 0;
            lblStatus.Text = "label1";
            // 
            // txtConnectionInfo
            // 
            txtConnectionInfo.Location = new Point(314, 372);
            txtConnectionInfo.Name = "txtConnectionInfo";
            txtConnectionInfo.Size = new Size(150, 31);
            txtConnectionInfo.TabIndex = 1;
            // 
            // btnSign
            // 
            btnSign.Enabled = false;
            btnSign.Location = new Point(586, 364);
            btnSign.Name = "btnSign";
            btnSign.Size = new Size(112, 34);
            btnSign.TabIndex = 2;
            btnSign.Text = "Sign";
            btnSign.UseVisualStyleBackColor = true;
            btnSign.Click += btnSign_Click;
            // 
            // notifyIcon
            // 
            notifyIcon.ContextMenuStrip = trayContextMenu;
            notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            notifyIcon.Text = "Signature Client";
            notifyIcon.Visible = true;
            notifyIcon.DoubleClick += notifyIcon_DoubleClick;
            // 
            // trayContextMenu
            // 
            trayContextMenu.ImageScalingSize = new Size(24, 24);
            trayContextMenu.Items.AddRange(new ToolStripItem[] {
            signMenuItem,
            configMenuItem,
            toolStripSeparator1,
            exitMenuItem});
            trayContextMenu.Name = "trayContextMenu";
            trayContextMenu.Size = new Size(193, 132);
            // 
            // signMenuItem
            // 
            signMenuItem.Name = "signMenuItem";
            signMenuItem.Size = new Size(192, 32);
            signMenuItem.Text = "Firma documento";
            signMenuItem.Click += signMenuItem_Click;
            // 
            // configMenuItem
            // 
            configMenuItem.Name = "configMenuItem";
            configMenuItem.Size = new Size(192, 32);
            configMenuItem.Text = "Configurazione";
            configMenuItem.Click += configMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(189, 6);
            // 
            // exitMenuItem
            // 
            exitMenuItem.Name = "exitMenuItem";
            exitMenuItem.Size = new Size(192, 32);
            exitMenuItem.Text = "Esci";
            exitMenuItem.Click += exitMenuItem_Click;

            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnSign);
            Controls.Add(txtConnectionInfo);
            Controls.Add(lblStatus);
            notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            //notifyIcon.Icon = Properties.Resources.AppIcon; 
            Name = "Form1";
            Text = "Signature Client";
            FormClosing += Form1_FormClosing;
            Resize += Form1_Resize;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblStatus;
        private TextBox txtConnectionInfo;
        private Button btnSign;

        private NotifyIcon notifyIcon;
        private ContextMenuStrip trayContextMenu;
        private ToolStripMenuItem signMenuItem;
        private ToolStripMenuItem configMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem exitMenuItem;
    }
}
