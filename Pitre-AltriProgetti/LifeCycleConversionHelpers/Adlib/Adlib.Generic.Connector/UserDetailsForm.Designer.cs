namespace Adlib.Director.SampleApplication
{
    partial class UserDetailsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserDetailsForm));
            this.labelName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxUserName = new System.Windows.Forms.TextBox();
            this.textBoxPwd = new System.Windows.Forms.TextBox();
            this.btnSelectIdOK = new System.Windows.Forms.Button();
            this.btnSelectIdCancel = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxFriendlyName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(10, 18);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(55, 13);
            this.labelName.TabIndex = 0;
            this.labelName.Text = "Username";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Password";
            // 
            // textBoxUserName
            // 
            this.textBoxUserName.Location = new System.Drawing.Point(13, 34);
            this.textBoxUserName.Name = "textBoxUserName";
            this.textBoxUserName.Size = new System.Drawing.Size(256, 20);
            this.textBoxUserName.TabIndex = 1;
            // 
            // textBoxPwd
            // 
            this.textBoxPwd.Location = new System.Drawing.Point(13, 80);
            this.textBoxPwd.Name = "textBoxPwd";
            this.textBoxPwd.PasswordChar = '*';
            this.textBoxPwd.Size = new System.Drawing.Size(256, 20);
            this.textBoxPwd.TabIndex = 1;
            this.textBoxPwd.UseSystemPasswordChar = true;
            // 
            // btnSelectIdOK
            // 
            this.btnSelectIdOK.Location = new System.Drawing.Point(25, 164);
            this.btnSelectIdOK.Name = "btnSelectIdOK";
            this.btnSelectIdOK.Size = new System.Drawing.Size(102, 27);
            this.btnSelectIdOK.TabIndex = 1;
            this.btnSelectIdOK.Text = "OK";
            this.btnSelectIdOK.UseVisualStyleBackColor = true;
            this.btnSelectIdOK.Click += new System.EventHandler(this.btnSelectIdOK_Click);
            // 
            // btnSelectIdCancel
            // 
            this.btnSelectIdCancel.Location = new System.Drawing.Point(153, 164);
            this.btnSelectIdCancel.Name = "btnSelectIdCancel";
            this.btnSelectIdCancel.Size = new System.Drawing.Size(102, 27);
            this.btnSelectIdCancel.TabIndex = 13;
            this.btnSelectIdCancel.Text = "Cancel";
            this.btnSelectIdCancel.UseVisualStyleBackColor = true;
            this.btnSelectIdCancel.Click += new System.EventHandler(this.btnSelectIdCancel_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 108);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(126, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Component friendly name";
            // 
            // textBoxFriendlyName
            // 
            this.textBoxFriendlyName.Location = new System.Drawing.Point(13, 124);
            this.textBoxFriendlyName.Name = "textBoxFriendlyName";
            this.textBoxFriendlyName.Size = new System.Drawing.Size(256, 20);
            this.textBoxFriendlyName.TabIndex = 1;
            // 
            // UserDetailsForm
            // 
            this.AcceptButton = this.btnSelectIdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(281, 212);
            this.Controls.Add(this.textBoxUserName);
            this.Controls.Add(this.textBoxPwd);
            this.Controls.Add(this.textBoxFriendlyName);
            this.Controls.Add(this.btnSelectIdOK);
            this.Controls.Add(this.btnSelectIdCancel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UserDetailsForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Enter user credentials";
            this.Load += new System.EventHandler(this.UserDetailsForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxUserName;
        private System.Windows.Forms.TextBox textBoxPwd;
        private System.Windows.Forms.Button btnSelectIdOK;
        private System.Windows.Forms.Button btnSelectIdCancel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxFriendlyName;
    }
}