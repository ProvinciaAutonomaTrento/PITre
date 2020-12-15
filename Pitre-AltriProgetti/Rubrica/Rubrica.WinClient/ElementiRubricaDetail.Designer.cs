namespace Rubrica.WinClient
{
    partial class ElementiRubricaDetail
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
            this.listView1 = new System.Windows.Forms.ListView();
            this.Id = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Codice = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Descrizione = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label12 = new System.Windows.Forms.Label();
            this.txtCodiceAoo = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtUtenteCreatore = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtDataUltimaModifica = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtDataCreazione = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtFax = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtTelefono = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtIndirizzo = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtDescrizione = new System.Windows.Forms.TextBox();
            this.txtCodice = new System.Windows.Forms.TextBox();
            this.lblId = new System.Windows.Forms.Label();
            this.txtId = new System.Windows.Forms.TextBox();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.cmdTipoCorrispondente = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Id,
            this.Codice,
            this.Descrizione});
            this.listView1.FullRowSelect = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(17, 13);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(381, 118);
            this.listView1.TabIndex = 4;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // Id
            // 
            this.Id.Text = "Id";
            // 
            // Codice
            // 
            this.Codice.Text = "Codice";
            // 
            // Descrizione
            // 
            this.Descrizione.Text = "Descrizione";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(14, 410);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(80, 13);
            this.label12.TabIndex = 66;
            this.label12.Text = "DataCreazione:";
            // 
            // txtCodiceAoo
            // 
            this.txtCodiceAoo.Location = new System.Drawing.Point(167, 327);
            this.txtCodiceAoo.Name = "txtCodiceAoo";
            this.txtCodiceAoo.Size = new System.Drawing.Size(231, 20);
            this.txtCodiceAoo.TabIndex = 65;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(14, 462);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(82, 13);
            this.label11.TabIndex = 64;
            this.label11.Text = "UtenteCreatore:";
            // 
            // txtUtenteCreatore
            // 
            this.txtUtenteCreatore.Location = new System.Drawing.Point(167, 459);
            this.txtUtenteCreatore.Name = "txtUtenteCreatore";
            this.txtUtenteCreatore.ReadOnly = true;
            this.txtUtenteCreatore.Size = new System.Drawing.Size(231, 20);
            this.txtUtenteCreatore.TabIndex = 63;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(14, 436);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(102, 13);
            this.label10.TabIndex = 62;
            this.label10.Text = "DataUltimaModifica:";
            // 
            // txtDataUltimaModifica
            // 
            this.txtDataUltimaModifica.Location = new System.Drawing.Point(167, 433);
            this.txtDataUltimaModifica.Name = "txtDataUltimaModifica";
            this.txtDataUltimaModifica.ReadOnly = true;
            this.txtDataUltimaModifica.Size = new System.Drawing.Size(231, 20);
            this.txtDataUltimaModifica.TabIndex = 61;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(14, 327);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(62, 13);
            this.label9.TabIndex = 60;
            this.label9.Text = "CodiceAoo:";
            // 
            // txtDataCreazione
            // 
            this.txtDataCreazione.Location = new System.Drawing.Point(167, 407);
            this.txtDataCreazione.Name = "txtDataCreazione";
            this.txtDataCreazione.ReadOnly = true;
            this.txtDataCreazione.Size = new System.Drawing.Size(231, 20);
            this.txtDataCreazione.TabIndex = 59;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(14, 301);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(36, 13);
            this.label8.TabIndex = 58;
            this.label8.Text = "EMail:";
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(167, 301);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(231, 20);
            this.txtEmail.TabIndex = 57;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(14, 275);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(27, 13);
            this.label7.TabIndex = 56;
            this.label7.Text = "Fax:";
            // 
            // txtFax
            // 
            this.txtFax.Location = new System.Drawing.Point(167, 275);
            this.txtFax.Name = "txtFax";
            this.txtFax.Size = new System.Drawing.Size(231, 20);
            this.txtFax.TabIndex = 55;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 249);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 13);
            this.label6.TabIndex = 54;
            this.label6.Text = "Telefono:";
            // 
            // txtTelefono
            // 
            this.txtTelefono.Location = new System.Drawing.Point(167, 249);
            this.txtTelefono.Name = "txtTelefono";
            this.txtTelefono.Size = new System.Drawing.Size(231, 20);
            this.txtTelefono.TabIndex = 53;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 223);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 13);
            this.label5.TabIndex = 52;
            this.label5.Text = "Indirizzo:";
            // 
            // txtIndirizzo
            // 
            this.txtIndirizzo.Location = new System.Drawing.Point(167, 223);
            this.txtIndirizzo.Name = "txtIndirizzo";
            this.txtIndirizzo.Size = new System.Drawing.Size(231, 20);
            this.txtIndirizzo.TabIndex = 51;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 197);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 13);
            this.label4.TabIndex = 50;
            this.label4.Text = "Descrizione:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 171);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 49;
            this.label3.Text = "Codice:";
            // 
            // txtDescrizione
            // 
            this.txtDescrizione.Location = new System.Drawing.Point(167, 197);
            this.txtDescrizione.Name = "txtDescrizione";
            this.txtDescrizione.Size = new System.Drawing.Size(231, 20);
            this.txtDescrizione.TabIndex = 48;
            // 
            // txtCodice
            // 
            this.txtCodice.Location = new System.Drawing.Point(167, 171);
            this.txtCodice.Name = "txtCodice";
            this.txtCodice.Size = new System.Drawing.Size(231, 20);
            this.txtCodice.TabIndex = 47;
            // 
            // lblId
            // 
            this.lblId.AutoSize = true;
            this.lblId.Location = new System.Drawing.Point(14, 145);
            this.lblId.Name = "lblId";
            this.lblId.Size = new System.Drawing.Size(19, 13);
            this.lblId.TabIndex = 68;
            this.lblId.Text = "Id:";
            // 
            // txtId
            // 
            this.txtId.Location = new System.Drawing.Point(167, 145);
            this.txtId.Name = "txtId";
            this.txtId.ReadOnly = true;
            this.txtId.Size = new System.Drawing.Size(231, 20);
            this.txtId.TabIndex = 67;
            // 
            // txtUrl
            // 
            this.txtUrl.Location = new System.Drawing.Point(167, 353);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(231, 20);
            this.txtUrl.TabIndex = 69;
            // 
            // cmdTipoCorrispondente
            // 
            this.cmdTipoCorrispondente.DisplayMember = "Descrizione";
            this.cmdTipoCorrispondente.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmdTipoCorrispondente.FormattingEnabled = true;
            this.cmdTipoCorrispondente.Location = new System.Drawing.Point(167, 380);
            this.cmdTipoCorrispondente.Name = "cmdTipoCorrispondente";
            this.cmdTipoCorrispondente.Size = new System.Drawing.Size(231, 21);
            this.cmdTipoCorrispondente.TabIndex = 70;
            this.cmdTipoCorrispondente.Tag = "Value";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 356);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 13);
            this.label1.TabIndex = 71;
            this.label1.Text = "Url Amministrazione:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 383);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 13);
            this.label2.TabIndex = 72;
            this.label2.Text = "Tipo Corrispondente:";
            // 
            // ElementiRubricaDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmdTipoCorrispondente);
            this.Controls.Add(this.txtUrl);
            this.Controls.Add(this.lblId);
            this.Controls.Add(this.txtId);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.txtCodiceAoo);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.txtUtenteCreatore);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtDataUltimaModifica);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtDataCreazione);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtFax);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtTelefono);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtIndirizzo);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtDescrizione);
            this.Controls.Add(this.txtCodice);
            this.Controls.Add(this.listView1);
            this.Name = "ElementiRubricaDetail";
            this.Size = new System.Drawing.Size(411, 491);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtCodiceAoo;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtUtenteCreatore;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtDataUltimaModifica;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtDataCreazione;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtFax;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtTelefono;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtIndirizzo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtDescrizione;
        private System.Windows.Forms.TextBox txtCodice;
        private System.Windows.Forms.Label lblId;
        private System.Windows.Forms.TextBox txtId;
        private System.Windows.Forms.ColumnHeader Id;
        private System.Windows.Forms.ColumnHeader Codice;
        private System.Windows.Forms.ColumnHeader Descrizione;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.ComboBox cmdTipoCorrispondente;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}
