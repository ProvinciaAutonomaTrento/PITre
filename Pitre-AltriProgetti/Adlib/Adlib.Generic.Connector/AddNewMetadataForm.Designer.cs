namespace Adlib.Director.SampleApplication
{
    partial class AddNewMetadataForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddNewMetadataForm));
            this.comboBoxName = new System.Windows.Forms.ComboBox();
            this.richTextBoxMetadataData = new System.Windows.Forms.RichTextBox();
            this.btnAddMetadataBrowse = new System.Windows.Forms.Button();
            this.btnAddMetadataOK = new System.Windows.Forms.Button();
            this.btnAddMetadataCancel = new System.Windows.Forms.Button();
            this.labelData = new System.Windows.Forms.Label();
            this.labelName = new System.Windows.Forms.Label();
            this.openFileDialogData = new System.Windows.Forms.OpenFileDialog();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonAddMdValue = new System.Windows.Forms.Button();
            this.listBoxMdValues = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // comboBoxName
            // 
            this.comboBoxName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxName.FormattingEnabled = true;
            this.comboBoxName.Location = new System.Drawing.Point(17, 21);
            this.comboBoxName.Name = "comboBoxName";
            this.comboBoxName.Size = new System.Drawing.Size(477, 21);
            this.comboBoxName.TabIndex = 0;
            this.comboBoxName.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // richTextBoxMetadataData
            // 
            this.richTextBoxMetadataData.Location = new System.Drawing.Point(265, 139);
            this.richTextBoxMetadataData.Name = "richTextBoxMetadataData";
            this.richTextBoxMetadataData.Size = new System.Drawing.Size(336, 225);
            this.richTextBoxMetadataData.TabIndex = 5;
            this.richTextBoxMetadataData.Text = "";
            // 
            // btnAddMetadataBrowse
            // 
            this.btnAddMetadataBrowse.Location = new System.Drawing.Point(206, 341);
            this.btnAddMetadataBrowse.Name = "btnAddMetadataBrowse";
            this.btnAddMetadataBrowse.Size = new System.Drawing.Size(38, 23);
            this.btnAddMetadataBrowse.TabIndex = 7;
            this.btnAddMetadataBrowse.Text = "...";
            this.btnAddMetadataBrowse.UseVisualStyleBackColor = true;
            this.btnAddMetadataBrowse.Click += new System.EventHandler(this.btnAddMetadataBrowse_Click);
            // 
            // btnAddMetadataOK
            // 
            this.btnAddMetadataOK.Location = new System.Drawing.Point(378, 370);
            this.btnAddMetadataOK.Name = "btnAddMetadataOK";
            this.btnAddMetadataOK.Size = new System.Drawing.Size(102, 27);
            this.btnAddMetadataOK.TabIndex = 8;
            this.btnAddMetadataOK.Text = "OK";
            this.btnAddMetadataOK.UseVisualStyleBackColor = true;
            this.btnAddMetadataOK.Click += new System.EventHandler(this.btnAddMetadataOK_Click);
            // 
            // btnAddMetadataCancel
            // 
            this.btnAddMetadataCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnAddMetadataCancel.Location = new System.Drawing.Point(499, 370);
            this.btnAddMetadataCancel.Name = "btnAddMetadataCancel";
            this.btnAddMetadataCancel.Size = new System.Drawing.Size(102, 27);
            this.btnAddMetadataCancel.TabIndex = 9;
            this.btnAddMetadataCancel.Text = "Cancel";
            this.btnAddMetadataCancel.UseVisualStyleBackColor = true;
            this.btnAddMetadataCancel.Click += new System.EventHandler(this.btnAddMetadataCancel_Click);
            // 
            // labelData
            // 
            this.labelData.AutoSize = true;
            this.labelData.Location = new System.Drawing.Point(14, 123);
            this.labelData.Name = "labelData";
            this.labelData.Size = new System.Drawing.Size(79, 13);
            this.labelData.TabIndex = 6;
            this.labelData.Text = "Possible values";
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(14, 5);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(170, 13);
            this.labelName.TabIndex = 10;
            this.labelName.Text = "Select predefined Metadata Name";
            // 
            // openFileDialogData
            // 
            this.openFileDialogData.FileName = "openFileDialogData";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Location = new System.Drawing.Point(17, 61);
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.ReadOnly = true;
            this.textBoxDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxDescription.Size = new System.Drawing.Size(584, 55);
            this.textBoxDescription.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Description";
            // 
            // buttonAddMdValue
            // 
            this.buttonAddMdValue.Location = new System.Drawing.Point(206, 239);
            this.buttonAddMdValue.Name = "buttonAddMdValue";
            this.buttonAddMdValue.Size = new System.Drawing.Size(38, 23);
            this.buttonAddMdValue.TabIndex = 15;
            this.buttonAddMdValue.Text = "=>";
            this.buttonAddMdValue.UseVisualStyleBackColor = true;
            this.buttonAddMdValue.Click += new System.EventHandler(this.buttonAddMdValue_Click);
            // 
            // listBoxMdValues
            // 
            this.listBoxMdValues.FormattingEnabled = true;
            this.listBoxMdValues.Location = new System.Drawing.Point(17, 139);
            this.listBoxMdValues.Name = "listBoxMdValues";
            this.listBoxMdValues.Size = new System.Drawing.Size(167, 225);
            this.listBoxMdValues.TabIndex = 16;
            this.listBoxMdValues.DoubleClick += new System.EventHandler(this.listBoxMdValues_DoubleClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(263, 122);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Actual value";
            // 
            // AddNewMetadataForm
            // 
            this.AcceptButton = this.btnAddMetadataOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(619, 405);
            this.Controls.Add(this.listBoxMdValues);
            this.Controls.Add(this.buttonAddMdValue);
            this.Controls.Add(this.comboBoxName);
            this.Controls.Add(this.textBoxDescription);
            this.Controls.Add(this.richTextBoxMetadataData);
            this.Controls.Add(this.btnAddMetadataBrowse);
            this.Controls.Add(this.btnAddMetadataOK);
            this.Controls.Add(this.btnAddMetadataCancel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelData);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "AddNewMetadataForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add Metadata Item";
            this.Shown += new System.EventHandler(this.AddNewMetadataForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxName;
        private System.Windows.Forms.RichTextBox richTextBoxMetadataData;
        private System.Windows.Forms.Button btnAddMetadataBrowse;
        private System.Windows.Forms.Button btnAddMetadataOK;
        private System.Windows.Forms.Button btnAddMetadataCancel;
        private System.Windows.Forms.Label labelData;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.OpenFileDialog openFileDialogData;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonAddMdValue;
        private System.Windows.Forms.ListBox listBoxMdValues;
        private System.Windows.Forms.Label label2;
    }
}