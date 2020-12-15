namespace Rubrica.WinClient
{
    partial class frmGestioneRubrica
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
            this.tbRubrica = new System.Windows.Forms.TabControl();
            this.tabUtenti = new System.Windows.Forms.TabPage();
            this.detailUtentiRubrica = new Rubrica.WinClient.UtentiRubricaDetail();
            this.tabElementi = new System.Windows.Forms.TabPage();
            this.detailElementiRubrica = new Rubrica.WinClient.ElementiRubricaDetail();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.txtNewPassword = new System.Windows.Forms.TextBox();
            this.btnChangePassword = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbRubrica.SuspendLayout();
            this.tabUtenti.SuspendLayout();
            this.tabElementi.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbRubrica
            // 
            this.tbRubrica.Controls.Add(this.tabUtenti);
            this.tbRubrica.Controls.Add(this.tabElementi);
            this.tbRubrica.Location = new System.Drawing.Point(12, 48);
            this.tbRubrica.Name = "tbRubrica";
            this.tbRubrica.SelectedIndex = 0;
            this.tbRubrica.Size = new System.Drawing.Size(418, 517);
            this.tbRubrica.TabIndex = 0;
            this.tbRubrica.Selected += new System.Windows.Forms.TabControlEventHandler(this.tbRubrica_Selected);
            // 
            // tabUtenti
            // 
            this.tabUtenti.Controls.Add(this.detailUtentiRubrica);
            this.tabUtenti.Location = new System.Drawing.Point(4, 22);
            this.tabUtenti.Name = "tabUtenti";
            this.tabUtenti.Size = new System.Drawing.Size(410, 465);
            this.tabUtenti.TabIndex = 0;
            this.tabUtenti.Text = "Utenti";
            this.tabUtenti.UseVisualStyleBackColor = true;
            // 
            // detailUtentiRubrica
            // 
            this.detailUtentiRubrica.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailUtentiRubrica.Location = new System.Drawing.Point(0, 0);
            this.detailUtentiRubrica.Name = "detailUtentiRubrica";
            this.detailUtentiRubrica.Size = new System.Drawing.Size(410, 465);
            this.detailUtentiRubrica.TabIndex = 0;
            // 
            // tabElementi
            // 
            this.tabElementi.Controls.Add(this.detailElementiRubrica);
            this.tabElementi.Location = new System.Drawing.Point(4, 22);
            this.tabElementi.Name = "tabElementi";
            this.tabElementi.Size = new System.Drawing.Size(410, 491);
            this.tabElementi.TabIndex = 1;
            this.tabElementi.Text = "Elementi rubrica";
            this.tabElementi.UseVisualStyleBackColor = true;
            // 
            // detailElementiRubrica
            // 
            this.detailElementiRubrica.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailElementiRubrica.Location = new System.Drawing.Point(0, 0);
            this.detailElementiRubrica.Name = "detailElementiRubrica";
            this.detailElementiRubrica.Size = new System.Drawing.Size(410, 491);
            this.detailElementiRubrica.TabIndex = 0;
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(355, 571);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 5;
            this.btnDelete.Text = "&Cancella";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(274, 571);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "&Salva";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(193, 571);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(75, 23);
            this.btnNew.TabIndex = 3;
            this.btnNew.Text = "&Nuovo";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // txtNewPassword
            // 
            this.txtNewPassword.Location = new System.Drawing.Point(112, 10);
            this.txtNewPassword.Name = "txtNewPassword";
            this.txtNewPassword.Size = new System.Drawing.Size(156, 20);
            this.txtNewPassword.TabIndex = 6;
            this.txtNewPassword.UseSystemPasswordChar = true;
            // 
            // btnChangePassword
            // 
            this.btnChangePassword.Location = new System.Drawing.Point(274, 8);
            this.btnChangePassword.Name = "btnChangePassword";
            this.btnChangePassword.Size = new System.Drawing.Size(75, 23);
            this.btnChangePassword.TabIndex = 7;
            this.btnChangePassword.Text = "Imposta";
            this.btnChangePassword.UseVisualStyleBackColor = true;
            this.btnChangePassword.Click += new System.EventHandler(this.btnChangePassword_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Nuova password:";
            // 
            // frmGestioneRubrica
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(441, 597);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnChangePassword);
            this.Controls.Add(this.txtNewPassword);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnNew);
            this.Controls.Add(this.tbRubrica);
            this.Name = "frmGestioneRubrica";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gestione Rubrica";
            this.Load += new System.EventHandler(this.frmGestioneRubrica_Load);
            this.tbRubrica.ResumeLayout(false);
            this.tabUtenti.ResumeLayout(false);
            this.tabElementi.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tbRubrica;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.TabPage tabUtenti;
        private System.Windows.Forms.TabPage tabElementi;
        private ElementiRubricaDetail detailElementiRubrica;
        private UtentiRubricaDetail detailUtentiRubrica;
        private System.Windows.Forms.TextBox txtNewPassword;
        private System.Windows.Forms.Button btnChangePassword;
        private System.Windows.Forms.Label label1;
    }
}

