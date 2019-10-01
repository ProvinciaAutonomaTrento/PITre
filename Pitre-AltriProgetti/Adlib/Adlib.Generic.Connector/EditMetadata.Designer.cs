namespace Adlib.Director.SampleApplication
{
    partial class EditMetadata
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditMetadata));
            this.textBoxMetadataName = new System.Windows.Forms.TextBox();
            this.textBoxMetadataValue = new System.Windows.Forms.TextBox();
            this.richTextBoxMetadataData = new System.Windows.Forms.RichTextBox();
            this.labelName = new System.Windows.Forms.Label();
            this.labelValue = new System.Windows.Forms.Label();
            this.labelData = new System.Windows.Forms.Label();
            this.btnEditMetadataBrowse = new System.Windows.Forms.Button();
            this.btnEditMetadataOK = new System.Windows.Forms.Button();
            this.btnEditMetadataCancel = new System.Windows.Forms.Button();
            this.openFileDialogData = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // textBoxMetadataName
            // 
            this.textBoxMetadataName.Location = new System.Drawing.Point(18, 25);
            this.textBoxMetadataName.Name = "textBoxMetadataName";
            this.textBoxMetadataName.Size = new System.Drawing.Size(323, 21);
            this.textBoxMetadataName.TabIndex = 0;
            // 
            // textBoxMetadataValue
            // 
            this.textBoxMetadataValue.Location = new System.Drawing.Point(18, 65);
            this.textBoxMetadataValue.Name = "textBoxMetadataValue";
            this.textBoxMetadataValue.Size = new System.Drawing.Size(323, 21);
            this.textBoxMetadataValue.TabIndex = 0;
            // 
            // richTextBoxMetadataData
            // 
            this.richTextBoxMetadataData.Location = new System.Drawing.Point(18, 104);
            this.richTextBoxMetadataData.Name = "richTextBoxMetadataData";
            this.richTextBoxMetadataData.Size = new System.Drawing.Size(477, 233);
            this.richTextBoxMetadataData.TabIndex = 1;
            this.richTextBoxMetadataData.Text = "";
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(15, 9);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(34, 13);
            this.labelName.TabIndex = 2;
            this.labelName.Text = "Name";
            // 
            // labelValue
            // 
            this.labelValue.AutoSize = true;
            this.labelValue.Location = new System.Drawing.Point(15, 49);
            this.labelValue.Name = "labelValue";
            this.labelValue.Size = new System.Drawing.Size(33, 13);
            this.labelValue.TabIndex = 2;
            this.labelValue.Text = "Value";
            // 
            // labelData
            // 
            this.labelData.AutoSize = true;
            this.labelData.Location = new System.Drawing.Point(15, 88);
            this.labelData.Name = "labelData";
            this.labelData.Size = new System.Drawing.Size(30, 13);
            this.labelData.TabIndex = 2;
            this.labelData.Text = "Data";
            // 
            // btnEditMetadataBrowse
            // 
            this.btnEditMetadataBrowse.Location = new System.Drawing.Point(508, 104);
            this.btnEditMetadataBrowse.Name = "btnEditMetadataBrowse";
            this.btnEditMetadataBrowse.Size = new System.Drawing.Size(32, 23);
            this.btnEditMetadataBrowse.TabIndex = 3;
            this.btnEditMetadataBrowse.Text = "...";
            this.btnEditMetadataBrowse.UseVisualStyleBackColor = true;
            this.btnEditMetadataBrowse.Click += new System.EventHandler(this.btnEditMetadataBrowse_Click);
            // 
            // btnEditMetadataOK
            // 
            this.btnEditMetadataOK.Location = new System.Drawing.Point(330, 348);
            this.btnEditMetadataOK.Name = "btnEditMetadataOK";
            this.btnEditMetadataOK.Size = new System.Drawing.Size(102, 27);
            this.btnEditMetadataOK.TabIndex = 4;
            this.btnEditMetadataOK.Text = "OK";
            this.btnEditMetadataOK.UseVisualStyleBackColor = true;
            this.btnEditMetadataOK.Click += new System.EventHandler(this.btnEditMetadataOK_Click);
            // 
            // btnEditMetadataCancel
            // 
            this.btnEditMetadataCancel.Location = new System.Drawing.Point(438, 348);
            this.btnEditMetadataCancel.Name = "btnEditMetadataCancel";
            this.btnEditMetadataCancel.Size = new System.Drawing.Size(102, 27);
            this.btnEditMetadataCancel.TabIndex = 4;
            this.btnEditMetadataCancel.Text = "Cancel";
            this.btnEditMetadataCancel.UseVisualStyleBackColor = true;
            this.btnEditMetadataCancel.Click += new System.EventHandler(this.btnEditMetadataCancel_Click);
            // 
            // openFileDialogData
            // 
            this.openFileDialogData.FileName = "openFileDialogData";
            // 
            // EditMetadata
            // 
            this.AcceptButton = this.btnEditMetadataOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(552, 386);
            this.Controls.Add(this.textBoxMetadataName);
            this.Controls.Add(this.textBoxMetadataValue);
            this.Controls.Add(this.richTextBoxMetadataData);
            this.Controls.Add(this.btnEditMetadataBrowse);
            this.Controls.Add(this.btnEditMetadataOK);
            this.Controls.Add(this.btnEditMetadataCancel);
            this.Controls.Add(this.labelData);
            this.Controls.Add(this.labelValue);
            this.Controls.Add(this.labelName);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EditMetadata";
            this.ShowIcon = false;
            this.Text = "Edit Metadata Item";
            this.Load += new System.EventHandler(this.EditMetadata_Load);
            this.Shown += new System.EventHandler(this.EditMetadata_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxMetadataName;
        private System.Windows.Forms.TextBox textBoxMetadataValue;
        private System.Windows.Forms.RichTextBox richTextBoxMetadataData;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label labelValue;
        private System.Windows.Forms.Label labelData;
        private System.Windows.Forms.Button btnEditMetadataBrowse;
        private System.Windows.Forms.Button btnEditMetadataOK;
        private System.Windows.Forms.Button btnEditMetadataCancel;
        private System.Windows.Forms.OpenFileDialog openFileDialogData;
    }
}