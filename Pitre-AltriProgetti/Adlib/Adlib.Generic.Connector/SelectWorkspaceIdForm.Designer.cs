namespace Adlib.Director.SampleApplication
{
    partial class SelectWorkspaceIdForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectWorkspaceIdForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.listViewRepositoryInfo = new System.Windows.Forms.ListView();
            this.btnSelectIdOK = new System.Windows.Forms.Button();
            this.btnSelectIdCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listViewRepositoryInfo);
            this.groupBox1.Location = new System.Drawing.Point(13, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(409, 158);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select WorkspaceId";
            // 
            // listViewRepositoryInfo
            // 
            this.listViewRepositoryInfo.FullRowSelect = true;
            this.listViewRepositoryInfo.GridLines = true;
            this.listViewRepositoryInfo.Location = new System.Drawing.Point(16, 19);
            this.listViewRepositoryInfo.Name = "listViewRepositoryInfo";
            this.listViewRepositoryInfo.Size = new System.Drawing.Size(376, 120);
            this.listViewRepositoryInfo.TabIndex = 1;
            this.listViewRepositoryInfo.UseCompatibleStateImageBehavior = false;
            this.listViewRepositoryInfo.View = System.Windows.Forms.View.Details;
            this.listViewRepositoryInfo.DoubleClick += new System.EventHandler(this.listViewRepositoryInfo_DoubleClick);
            // 
            // btnSelectIdOK
            // 
            this.btnSelectIdOK.Location = new System.Drawing.Point(105, 185);
            this.btnSelectIdOK.Name = "btnSelectIdOK";
            this.btnSelectIdOK.Size = new System.Drawing.Size(102, 27);
            this.btnSelectIdOK.TabIndex = 10;
            this.btnSelectIdOK.Text = "OK";
            this.btnSelectIdOK.UseVisualStyleBackColor = true;
            this.btnSelectIdOK.Click += new System.EventHandler(this.btnSelectIdOK_Click);
            // 
            // btnSelectIdCancel
            // 
            this.btnSelectIdCancel.Location = new System.Drawing.Point(226, 185);
            this.btnSelectIdCancel.Name = "btnSelectIdCancel";
            this.btnSelectIdCancel.Size = new System.Drawing.Size(102, 27);
            this.btnSelectIdCancel.TabIndex = 11;
            this.btnSelectIdCancel.Text = "Cancel";
            this.btnSelectIdCancel.UseVisualStyleBackColor = true;
            this.btnSelectIdCancel.Click += new System.EventHandler(this.btnSelectIdCancel_Click);
            // 
            // SelectWorkspaceIdForm
            // 
            this.AcceptButton = this.btnSelectIdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 229);
            this.Controls.Add(this.btnSelectIdOK);
            this.Controls.Add(this.btnSelectIdCancel);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectWorkspaceIdForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Transformation WorkspaceId";
            this.Load += new System.EventHandler(this.SelectWorkspaceIdForm_Load);
            this.Shown += new System.EventHandler(this.SelectWorkspaceVersionIdForm_Shown);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSelectIdOK;
        private System.Windows.Forms.Button btnSelectIdCancel;
        private System.Windows.Forms.ListView listViewRepositoryInfo;
    }
}