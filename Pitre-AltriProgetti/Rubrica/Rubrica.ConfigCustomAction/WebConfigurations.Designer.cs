namespace Rubrica.ConfigCustomAction
{
    partial class WebConfigurations
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblDatabaseType = new System.Windows.Forms.Label();
            this.cboDatabaseTypes = new System.Windows.Forms.ComboBox();
            this.grdConnectionStringProperties = new System.Windows.Forms.PropertyGrid();
            this.txtWebConfigPath = new System.Windows.Forms.TextBox();
            this.lblWebConfigPath = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnLogRootPath = new System.Windows.Forms.Button();
            this.lblLogRootPath = new System.Windows.Forms.Label();
            this.txtLogRootPath = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(223, 376);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(304, 376);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "&Annulla";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblDatabaseType
            // 
            this.lblDatabaseType.AutoSize = true;
            this.lblDatabaseType.Location = new System.Drawing.Point(12, 58);
            this.lblDatabaseType.Name = "lblDatabaseType";
            this.lblDatabaseType.Size = new System.Drawing.Size(78, 13);
            this.lblDatabaseType.TabIndex = 3;
            this.lblDatabaseType.Text = "Tipo database:";
            // 
            // cboDatabaseTypes
            // 
            this.cboDatabaseTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDatabaseTypes.FormattingEnabled = true;
            this.cboDatabaseTypes.Location = new System.Drawing.Point(15, 74);
            this.cboDatabaseTypes.Name = "cboDatabaseTypes";
            this.cboDatabaseTypes.Size = new System.Drawing.Size(364, 21);
            this.cboDatabaseTypes.TabIndex = 3;
            this.cboDatabaseTypes.SelectedIndexChanged += new System.EventHandler(this.cboDatabaseTypes_SelectedIndexChanged);
            // 
            // grdConnectionStringProperties
            // 
            this.grdConnectionStringProperties.Location = new System.Drawing.Point(15, 101);
            this.grdConnectionStringProperties.Name = "grdConnectionStringProperties";
            this.grdConnectionStringProperties.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.grdConnectionStringProperties.Size = new System.Drawing.Size(364, 179);
            this.grdConnectionStringProperties.TabIndex = 4;
            this.grdConnectionStringProperties.ToolbarVisible = false;
            // 
            // txtWebConfigPath
            // 
            this.txtWebConfigPath.Location = new System.Drawing.Point(15, 29);
            this.txtWebConfigPath.Name = "txtWebConfigPath";
            this.txtWebConfigPath.ReadOnly = true;
            this.txtWebConfigPath.Size = new System.Drawing.Size(333, 20);
            this.txtWebConfigPath.TabIndex = 1;
            // 
            // lblWebConfigPath
            // 
            this.lblWebConfigPath.AutoSize = true;
            this.lblWebConfigPath.Location = new System.Drawing.Point(12, 9);
            this.lblWebConfigPath.Name = "lblWebConfigPath";
            this.lblWebConfigPath.Size = new System.Drawing.Size(65, 13);
            this.lblWebConfigPath.TabIndex = 6;
            this.lblWebConfigPath.Text = "Web.config:";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(354, 29);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(27, 20);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnLogRootPath
            // 
            this.btnLogRootPath.Location = new System.Drawing.Point(354, 335);
            this.btnLogRootPath.Name = "btnLogRootPath";
            this.btnLogRootPath.Size = new System.Drawing.Size(27, 20);
            this.btnLogRootPath.TabIndex = 8;
            this.btnLogRootPath.Text = "...";
            this.btnLogRootPath.UseVisualStyleBackColor = true;
            this.btnLogRootPath.Click += new System.EventHandler(this.btnLogRootPath_Click);
            // 
            // lblLogRootPath
            // 
            this.lblLogRootPath.AutoSize = true;
            this.lblLogRootPath.Location = new System.Drawing.Point(12, 315);
            this.lblLogRootPath.Name = "lblLogRootPath";
            this.lblLogRootPath.Size = new System.Drawing.Size(52, 13);
            this.lblLogRootPath.TabIndex = 9;
            this.lblLogRootPath.Text = "Log path:";
            // 
            // txtLogRootPath
            // 
            this.txtLogRootPath.Location = new System.Drawing.Point(15, 335);
            this.txtLogRootPath.Name = "txtLogRootPath";
            this.txtLogRootPath.ReadOnly = true;
            this.txtLogRootPath.Size = new System.Drawing.Size(333, 20);
            this.txtLogRootPath.TabIndex = 7;
            // 
            // WebConfigurations
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(393, 409);
            this.Controls.Add(this.btnLogRootPath);
            this.Controls.Add(this.lblLogRootPath);
            this.Controls.Add(this.txtLogRootPath);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.lblWebConfigPath);
            this.Controls.Add(this.txtWebConfigPath);
            this.Controls.Add(this.grdConnectionStringProperties);
            this.Controls.Add(this.cboDatabaseTypes);
            this.Controls.Add(this.lblDatabaseType);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WebConfigurations";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configurazioni";
            this.Load += new System.EventHandler(this.WebConfigurations_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblDatabaseType;
        private System.Windows.Forms.ComboBox cboDatabaseTypes;
        private System.Windows.Forms.PropertyGrid grdConnectionStringProperties;
        private System.Windows.Forms.TextBox txtWebConfigPath;
        private System.Windows.Forms.Label lblWebConfigPath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnLogRootPath;
        private System.Windows.Forms.Label lblLogRootPath;
        private System.Windows.Forms.TextBox txtLogRootPath;
    }
}

