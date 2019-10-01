namespace Rubrica.WinClient
{
    partial class UtentiRubricaDetail
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblId = new System.Windows.Forms.Label();
            this.txtId = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtNome = new System.Windows.Forms.TextBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.Id = new System.Windows.Forms.ColumnHeader();
            this.Nome = new System.Windows.Forms.ColumnHeader();
            this.Amministratore = new System.Windows.Forms.ColumnHeader();
            this.label10 = new System.Windows.Forms.Label();
            this.txtDataUltimaModifica = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtDataCreazione = new System.Windows.Forms.TextBox();
            this.chkAmministratore = new System.Windows.Forms.CheckBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblId
            // 
            this.lblId.AutoSize = true;
            this.lblId.Location = new System.Drawing.Point(11, 148);
            this.lblId.Name = "lblId";
            this.lblId.Size = new System.Drawing.Size(19, 13);
            this.lblId.TabIndex = 75;
            this.lblId.Text = "Id:";
            // 
            // txtId
            // 
            this.txtId.Location = new System.Drawing.Point(164, 148);
            this.txtId.Name = "txtId";
            this.txtId.ReadOnly = true;
            this.txtId.Size = new System.Drawing.Size(231, 20);
            this.txtId.TabIndex = 74;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 174);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 72;
            this.label3.Text = "Nome:";
            // 
            // txtNome
            // 
            this.txtNome.Location = new System.Drawing.Point(164, 174);
            this.txtNome.Name = "txtNome";
            this.txtNome.Size = new System.Drawing.Size(231, 20);
            this.txtNome.TabIndex = 70;
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Id,
            this.Nome,
            this.Amministratore});
            this.listView1.FullRowSelect = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(14, 16);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(381, 118);
            this.listView1.TabIndex = 69;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // Id
            // 
            this.Id.Text = "Id";
            // 
            // Nome
            // 
            this.Nome.Text = "Nome";
            this.Nome.Width = 112;
            // 
            // Amministratore
            // 
            this.Amministratore.Text = "Amministratore";
            this.Amministratore.Width = 143;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(11, 285);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(102, 13);
            this.label10.TabIndex = 77;
            this.label10.Text = "DataUltimaModifica:";
            // 
            // txtDataUltimaModifica
            // 
            this.txtDataUltimaModifica.Location = new System.Drawing.Point(164, 285);
            this.txtDataUltimaModifica.Name = "txtDataUltimaModifica";
            this.txtDataUltimaModifica.ReadOnly = true;
            this.txtDataUltimaModifica.Size = new System.Drawing.Size(231, 20);
            this.txtDataUltimaModifica.TabIndex = 76;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(11, 259);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(80, 13);
            this.label12.TabIndex = 79;
            this.label12.Text = "DataCreazione:";
            // 
            // txtDataCreazione
            // 
            this.txtDataCreazione.Location = new System.Drawing.Point(164, 259);
            this.txtDataCreazione.Name = "txtDataCreazione";
            this.txtDataCreazione.ReadOnly = true;
            this.txtDataCreazione.Size = new System.Drawing.Size(231, 20);
            this.txtDataCreazione.TabIndex = 78;
            // 
            // chkAmministratore
            // 
            this.chkAmministratore.AutoSize = true;
            this.chkAmministratore.Location = new System.Drawing.Point(164, 227);
            this.chkAmministratore.Name = "chkAmministratore";
            this.chkAmministratore.Size = new System.Drawing.Size(94, 17);
            this.chkAmministratore.TabIndex = 80;
            this.chkAmministratore.Text = "Amministratore";
            this.chkAmministratore.UseVisualStyleBackColor = true;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(164, 201);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(231, 20);
            this.txtPassword.TabIndex = 81;
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 204);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 82;
            this.label1.Text = "Password:";
            // 
            // UtentiRubricaDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.chkAmministratore);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.txtDataCreazione);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtDataUltimaModifica);
            this.Controls.Add(this.lblId);
            this.Controls.Add(this.txtId);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtNome);
            this.Controls.Add(this.listView1);
            this.Name = "UtentiRubricaDetail";
            this.Size = new System.Drawing.Size(425, 329);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblId;
        private System.Windows.Forms.TextBox txtId;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtNome;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader Id;
        private System.Windows.Forms.ColumnHeader Nome;
        private System.Windows.Forms.ColumnHeader Amministratore;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtDataUltimaModifica;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtDataCreazione;
        private System.Windows.Forms.CheckBox chkAmministratore;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label1;
    }
}
