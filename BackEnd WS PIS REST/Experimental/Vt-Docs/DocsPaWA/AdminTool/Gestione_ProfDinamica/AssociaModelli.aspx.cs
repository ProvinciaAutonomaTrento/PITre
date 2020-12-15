using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Text;
using DocsPAWA.utils;
using System.Collections.Generic;
using MText.DomainObjects;
using DocsPAWA.models;
using System.Linq;
using MText;

namespace DocsPAWA.AdminTool.Gestione_ProfDinamica
{
	/// <summary>
	/// Summary description for AssociaModelliTrasm.
	/// </summary>
	public class AssociaModelli : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Button btn_conferma;
		protected System.Web.UI.WebControls.Label lbl_titolo;
		protected System.Web.UI.WebControls.CheckBox CheckBox1;
		protected System.Web.UI.WebControls.CheckBox CheckBox2;
        protected System.Web.UI.WebControls.CheckBox CheckBox3;
        protected System.Web.UI.WebControls.CheckBox CheckBox4;
        protected System.Web.UI.WebControls.CheckBox CheckBox5;
		protected System.Web.UI.WebControls.ImageButton ImageButton1;
		protected System.Web.UI.WebControls.ImageButton ImageButton2;
        protected System.Web.UI.WebControls.ImageButton ImageButton3;
        protected System.Web.UI.WebControls.ImageButton ImageButton4;
        protected System.Web.UI.WebControls.ImageButton ImageButton5;
        protected System.Web.UI.WebControls.ImageButton ImageButton6;
		protected System.Web.UI.HtmlControls.HtmlInputFile uploadPathUno;
        protected System.Web.UI.HtmlControls.HtmlInputFile uploadPathDue;
        protected System.Web.UI.HtmlControls.HtmlInputFile uploadPathSU;
		protected System.Web.UI.HtmlControls.HtmlInputFile uploadPathAllUno;
        protected System.Web.UI.HtmlControls.HtmlInputFile uploadPathModExc;
        protected System.Web.UI.HtmlControls.HtmlTableRow Tr2;

        protected System.Web.UI.WebControls.ImageButton Modello1;
        protected System.Web.UI.WebControls.ImageButton Modello2;
        protected System.Web.UI.WebControls.ImageButton ModelloSU;
        protected System.Web.UI.WebControls.ImageButton Allegato;
        protected System.Web.UI.WebControls.ImageButton ModelloExc;
        protected System.Web.UI.WebControls.Panel PanelSU;
        protected System.Web.UI.WebControls.HyperLink linkTag;

        protected System.Web.UI.HtmlControls.HtmlTableRow trModelChoiceMainDocument, 
                                                          trModelChoiceAtt,
                                                          trGenerateMTextSourceModel;
        protected System.Web.UI.WebControls.DropDownList ddlModelTypeMain, ddlMTextModelMain, ddlModelTypeAtt, ddlMTextModelAtt;
        protected System.Web.UI.WebControls.Panel pnlModelTypeMain, pnlModelTypeAtt;
        protected System.Web.UI.WebControls.TextBox txtModelNameMain, txtModelNameAtt;
        protected System.Web.UI.WebControls.Button btnAddTemplateDoc, btnAddTemplateAtt;
        

        /// <summary>
        /// Url della pagina di visualizzazione del documento (nel frame)
        /// </summary>
        protected StringBuilder _frameSrc = new StringBuilder(String.Empty);

        private bool SHOW_STAMPA_UNIONE
        {
            get
            {
                string valoreChiave = utils.InitConfigurationKeys.GetValue("0", "FE_STAMPA_UNIONE");
                return "1".Equals(valoreChiave);
            }
        }


		private void Page_Load(object sender, System.EventArgs e)
		{
            Modello1.Enabled = false;
            Modello2.Enabled = false;
            Allegato.Enabled = false;
            ModelloSU.Enabled = false;

            // Se i modelli M/Text sono attivi e non si è in postback vengono rese visibili le righe con
            // le drop down per la scelta del tipo del modello e vengono popolate le liste dei modelli
            if (MTextUtils.IsActiveMTextIntegration() && !IsPostBack)
            {
                this.trGenerateMTextSourceModel.Visible = true;
                this.trModelChoiceMainDocument.Visible = true;
                //this.trModelChoiceAtt.Visible = true;
                try
                {
                    List<ModelInfo> models = ModelProviderFactory<MTextModelProvider>.GetInstance().GetModels();
                    foreach (ModelInfo model in models)
                    {
                        this.ddlMTextModelMain.Items.Add(new ListItem(model.Name, model.Path));
                        this.ddlMTextModelAtt.Items.Add(new ListItem(model.Name, model.Path));
                    }

                }
                catch (Exception ex)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Errore durante il reperimento dei modelli M/Text');", true);
                    
                }
            }


            this.linkTag.NavigateUrl = "metaTagsCampiWORD_PITRE.doc";
            string valoreChiave;
            valoreChiave = utils.InitConfigurationKeys.GetValue("0", "FE_EXPORT_DA_MODELLO");
            if (!string.IsNullOrEmpty(valoreChiave) && valoreChiave.Equals("1"))
            {
                Tr2.Visible = true;
            }
            else
                Tr2.Visible = false;


			DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates) Session["templateSelPerModelli"];

            // Se l'estensione del modello 1 è MTXT viene attivata la visualizzazione M/Text
            // altrimenti quella normale
            if (!String.IsNullOrEmpty(template.PATH_MODELLO_1_EXT) && template.PATH_MODELLO_1_EXT == "MTXT" && !IsPostBack)
            {
                this.ddlModelTypeMain.SelectedIndex = 1;
                this.CheckBox1.Checked = true;
                this.pnlModelTypeMain.Visible = true;
                this.uploadPathUno.Visible = false;
                
                // Toppa temporanea in attesa di prox versione M/Text
                int index = this.ddlMTextModelMain.Items.IndexOf(new ListItem(MTextUtils.GetNameFromQualifiedName(template.PATH_MODELLO_1), template.PATH_MODELLO_1));
                if (index == -1)
                {
                    this.ddlMTextModelMain.Items.Add(new ListItem(MTextUtils.GetNameFromQualifiedName(template.PATH_MODELLO_1), template.PATH_MODELLO_1));
                    index = this.ddlMTextModelMain.Items.Count - 1;
                    
                }
                // Fine toppa temporanea

                this.ddlMTextModelMain.SelectedIndex = index;
            }
            else
                if (template.PATH_MODELLO_1 != "")
                {
                    CheckBox1.Checked = true;
                    Modello1.Enabled = true;
                }

            // Se l'estensione del modello 2 è MTXT viene attivata la visualizzazione M/Text
            // altrimenti quella normale
            if (!String.IsNullOrEmpty(template.PATH_MODELLO_2_EXT) && template.PATH_MODELLO_2_EXT == "MTXT" && !IsPostBack)
            {
                this.ddlModelTypeAtt.SelectedIndex = 1;
                this.CheckBox2.Checked = true;
                this.pnlModelTypeAtt.Visible = true;
                this.uploadPathDue.Visible = false;

                // Toppa temporanea in attesa di prox versione M/Text
                int index = this.ddlMTextModelAtt.Items.IndexOf(new ListItem(MTextUtils.GetNameFromQualifiedName(template.PATH_MODELLO_2), template.PATH_MODELLO_2));
                if (index == -1)
                {
                    this.ddlMTextModelAtt.Items.Add(new ListItem(MTextUtils.GetNameFromQualifiedName(template.PATH_MODELLO_2), template.PATH_MODELLO_2));
                    index = this.ddlMTextModelAtt.Items.Count - 1;

                }
                // Fine toppa temporanea
                
                this.ddlMTextModelAtt.SelectedIndex = index;
            }
            else
                if (template.PATH_MODELLO_2 != "")
                {
                    CheckBox2.Checked = true;
                    Modello2.Enabled = true;
                }

            if (template.PATH_MODELLO_STAMPA_UNIONE != "")
            {
                CheckBox4.Checked = true;
                ModelloSU.Enabled = true;
            }
            if (template.PATH_ALLEGATO_1 != "")
            {
                CheckBox3.Checked = true;
                Allegato.Enabled = true;
            }

            if (!string.IsNullOrEmpty(template.PATH_MODELLO_EXCEL))
            {
                CheckBox5.Checked = true;
                ModelloExc.Enabled = true;
            }

			if(System.Configuration.ConfigurationManager.AppSettings["MODELLO_DOCUMENTO"] != null && System.Configuration.ConfigurationManager.AppSettings["MODELLO_DOCUMENTO"] == "1")
			{
				uploadPathDue.Visible = false;
				CheckBox2.Visible = false;
				ImageButton2.Visible = false;
			}

            if (SHOW_STAMPA_UNIONE)
            {
                this.PanelSU.Visible = true;
            }
            else
            {
                this.PanelSU.Visible = false;
            }
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.btn_conferma.Click += new System.EventHandler(this.btn_conferma_Click);
			this.ImageButton1.Click += new System.Web.UI.ImageClickEventHandler(this.ImageButton1_Click);
			this.ImageButton2.Click += new System.Web.UI.ImageClickEventHandler(this.ImageButton2_Click);
            this.ImageButton3.Click += new System.Web.UI.ImageClickEventHandler(this.ImageButton3_Click);
            this.ImageButton4.Click += new System.Web.UI.ImageClickEventHandler(this.ImageButton4_Click);
            this.ImageButton6.Click += new System.Web.UI.ImageClickEventHandler(this.ImageButton6_Click);
            this.Modello1.Click += new System.Web.UI.ImageClickEventHandler(this.btn_Modello1_Click);
            this.Modello2.Click += new System.Web.UI.ImageClickEventHandler(this.btn_Modello2_Click);
            this.ModelloSU.Click += new System.Web.UI.ImageClickEventHandler(this.btn_ModelloSU_Click);
            this.Allegato.Click += new System.Web.UI.ImageClickEventHandler(this.btn_Allegato_Click);
            this.ModelloExc.Click += new System.Web.UI.ImageClickEventHandler(this.btn_modelloExc_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion		



        private void btn_Modello1_Click(object sender, System.EventArgs e)
        {
            DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates) Session["templateSelPerModelli"];
            if (template.PATH_MODELLO_1 != null)
            {
                Session.Add("ModelloDaVisualizzare", template.PATH_MODELLO_1);
                Page.ClientScript.RegisterStartupScript(this.GetType(), "visualizzaModello", "VisualizzaModello();", true);
                
            }
        }
        
        private void btn_Modello2_Click(object sender, System.EventArgs e)
        {
            DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["templateSelPerModelli"];
            if (template.PATH_MODELLO_2 != null)
            {
                Session.Add("ModelloDaVisualizzare", template.PATH_MODELLO_2);
                Page.ClientScript.RegisterStartupScript(this.GetType(), "visualizzaModello", "VisualizzaModello();", true);
            }
        }

        private void btn_ModelloSU_Click(object sender, System.EventArgs e)
        {
            DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["templateSelPerModelli"];
            if (template.PATH_MODELLO_STAMPA_UNIONE != null)
            {
                Session.Add("ModelloDaVisualizzare", template.PATH_MODELLO_STAMPA_UNIONE);
                Page.ClientScript.RegisterStartupScript(this.GetType(), "visualizzaModello", "VisualizzaModello();", true);
            }
        }

        private void btn_Allegato_Click(object sender, System.EventArgs e)
        {
            DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["templateSelPerModelli"];
            if (template.PATH_ALLEGATO_1 != null)
            {
                Session.Add("ModelloDaVisualizzare", template.PATH_ALLEGATO_1);
                Page.ClientScript.RegisterStartupScript(this.GetType(), "visualizzaModello", "VisualizzaModello();", true);
            }
        }

        private void btn_modelloExc_Click(object sender, System.EventArgs e)
        {
            DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["templateSelPerModelli"];
            if (!string.IsNullOrEmpty(template.PATH_MODELLO_EXCEL))
            {
                Session.Add("ModelloDaVisualizzare", template.PATH_MODELLO_EXCEL);
                Page.ClientScript.RegisterStartupScript(this.GetType(),"visualizzaModello", "VisualizzaModello();", true);
            }
        }

        #region Eventi M/Text

        protected void ddlModelTypeMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ddlModelTypeMain.SelectedIndex == 0)
            {
                // Modello Classico
                this.uploadPathUno.Visible = true;
                this.pnlModelTypeMain.Visible = false;
            }
            else
            {
                // Modello M/Text
                this.uploadPathUno.Visible = false;
                this.pnlModelTypeMain.Visible = true;
            }
 
        }

        protected void ddlModelTypeAtt_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ddlModelTypeMain.SelectedIndex == 0)
            {
                // Modello Classico
                this.uploadPathDue.Visible = true;
                this.pnlModelTypeAtt.Visible = false;
            }
            else
            {
                // Modello M/Text
                this.uploadPathDue.Visible = false;
                this.pnlModelTypeAtt.Visible = true;
            }
 
        }

        #endregion


        private void btn_conferma_Click(object sender, System.EventArgs e)
		{
            string extPathUno = string.Empty;
			//Controllo che siano file RTF
			if(uploadPathUno.Value != "")
			{                
				string [] pathUno = uploadPathUno.Value.Split('.');
                extPathUno = pathUno[pathUno.Length - 1];
                if (pathUno.Length != 0 && pathUno[pathUno.Length - 1] != "rtf" && pathUno[pathUno.Length - 1].ToUpper() != "PPT" && pathUno[pathUno.Length - 1].ToUpper() != "PPTX")
				{
					RegisterStartupScript("selezioneNonValida","<script>alert('E possibile selezionare solo modelli in formato RTF o PPT. Selezionare un altro modello !');</script>");
					return;
				}
			}
			
			if(uploadPathDue.Value != "")
			{
				string [] pathDue = uploadPathDue.Value.Split('.');
				if(pathDue.Length != 0 && pathDue[pathDue.Length-1] != "rtf")
				{
                    RegisterStartupScript("selezioneNonValida", "<script>alert('E possibile selezionare solo modelli in formato RTF. Selezionare un altro modello !');</script>");
					return;
				}
			}

            if (!string.IsNullOrEmpty(uploadPathSU.Value))
            {
                string[] pathSU= uploadPathSU.Value.Split('.');
                if (pathSU.Length != 0 && pathSU[pathSU.Length - 1] != "rtf")
                {
                    RegisterStartupScript("selezioneNonValida", "<script>alert('E possibile selezionare solo modelli in formato RTF. Selezionare un altro modello !');</script>");
                    return;
                }
            }

            if (uploadPathAllUno.Value != "")
            {
                string[] pathAllUno = uploadPathAllUno.Value.Split('.');
                if (pathAllUno.Length != 0 && pathAllUno[pathAllUno.Length - 1] != "rtf")
                {
                    RegisterStartupScript("selezioneNonValida", "<script>alert('E possibile selezionare solo modelli in formato RTF. Selezionare un altro modello !');</script>");
                    return;
                }
            }

            if (!string.IsNullOrEmpty(uploadPathModExc.Value))
            {
                string[] pathModExc = uploadPathModExc.Value.Split('.');
                if (pathModExc.Length != 0 && pathModExc[pathModExc.Length - 1] != "xls")
                {
                    RegisterStartupScript("selezioneNonValida", "<script>alert('E possibile selezionare solo modelli in formato XLS. Selezionare un altro modello !');</script>");
                    return;
                }
            }

			//Controllo che non sia stato inserito il modello 2 senza aver effettuato una scelta anche per il modello 1
			if(uploadPathDue.Value != "" &&  ((DocsPAWA.DocsPaWR.Templates) Session["templateSelPerModelli"]).PATH_MODELLO_1 == "" )
			{
				if(uploadPathUno.Value == "")
				{
					RegisterStartupScript("selezioneNonValida","<script>alert('Per inserire il \"Modello 2\" è necessario che sia presente anche il \"Modello 1\" !');</script>");
					return;
				}
			}

            


			string[] amministrazione = ((string) Session["AMMDATASET"]).Split('@');
			string codiceAmministrazione  = amministrazione[0];


            // Salvataggio del modello M/Text se c'è
            if (this.ddlModelTypeMain.SelectedIndex == 1)
            {
                DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["templateSelPerModelli"];

                template.PATH_MODELLO_1 = this.ddlMTextModelMain.SelectedValue;
                template.PATH_MODELLO_1_EXT = "MTXT";
                
                ProfilazioneDocManager.salvaModelli(null, template.DESCRIZIONE, codiceAmministrazione, "MTextModel.MTXT", "MTXT", template, this);
 
            }
            else
			    //Controllo e salvataggio dei modelli
			    if(uploadPathUno.Value != "" && Session["templateSelPerModelli"] != null)
			    {
				    DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates) Session["templateSelPerModelli"];
				    HttpPostedFile p = uploadPathUno.PostedFile;
				    Stream fs = p.InputStream;
				    byte[] dati = new byte[fs.Length];
				    fs.Read(dati,0,(int)fs.Length);
				    fs.Close();
				
				    ((DocsPAWA.DocsPaWR.Templates) Session["templateSelPerModelli"]).PATH_MODELLO_1 = "Modelli\\"+codiceAmministrazione+"\\"+template.DESCRIZIONE+"\\Modello1." + extPathUno;
                    ((DocsPAWA.DocsPaWR.Templates) Session["templateSelPerModelli"]).PATH_MODELLO_1_EXT = extPathUno;
                    ProfilazioneDocManager.salvaModelli(dati, template.DESCRIZIONE, codiceAmministrazione, "Modello1." + extPathUno, extPathUno, (DocsPAWA.DocsPaWR.Templates)Session["templateSelPerModelli"], this);
			    }

            // Salvataggio del modello M/Text se c'è
            if (this.ddlModelTypeAtt.SelectedIndex == 1)
            {
                DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["templateSelPerModelli"];

                template.PATH_MODELLO_2 = this.ddlMTextModelAtt.SelectedValue;
                template.PATH_MODELLO_2_EXT = "MTXT";
                ProfilazioneDocManager.salvaModelli(null, template.DESCRIZIONE, codiceAmministrazione, "MTextModel.MTXT", "MTXT", template, this);

            }
            else
			    if(uploadPathDue.Value != "" && Session["templateSelPerModelli"] != null)
			    {
				    DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates) Session["templateSelPerModelli"];
				    HttpPostedFile p = uploadPathDue.PostedFile;
				    Stream fs = p.InputStream;
				    byte[] dati = new byte[fs.Length];
				    fs.Read(dati,0,(int)fs.Length);
				    fs.Close();
				
				    ((DocsPAWA.DocsPaWR.Templates) Session["templateSelPerModelli"]).PATH_MODELLO_2 = "Modelli\\"+codiceAmministrazione+"\\"+template.DESCRIZIONE+"\\Modello2.rtf";
				    ProfilazioneDocManager.salvaModelli(dati,template.DESCRIZIONE,codiceAmministrazione,"Modello2.rtf","doc",(DocsPAWA.DocsPaWR.Templates) Session["templateSelPerModelli"], this);
			    }

            if (uploadPathSU.Value != "" && Session["templateSelPerModelli"] != null)
            {
                DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["templateSelPerModelli"];
                HttpPostedFile p = uploadPathSU.PostedFile;
                Stream fs = p.InputStream;
                byte[] dati = new byte[fs.Length];
                fs.Read(dati, 0, (int)fs.Length);
                fs.Close();
                ((DocsPAWA.DocsPaWR.Templates)Session["templateSelPerModelli"]).PATH_MODELLO_STAMPA_UNIONE = "Modelli\\" + codiceAmministrazione + "\\" + template.DESCRIZIONE + "\\ModelloSU.rtf";
                ProfilazioneDocManager.salvaModelli(dati, template.DESCRIZIONE, codiceAmministrazione, "ModelloSU.rtf", "doc", (DocsPAWA.DocsPaWR.Templates)Session["templateSelPerModelli"], this);
            }
            if (uploadPathAllUno.Value != "" && Session["templateSelPerModelli"] != null)
            {
                DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["templateSelPerModelli"];
                HttpPostedFile p = uploadPathAllUno.PostedFile;
                Stream fs = p.InputStream;
                byte[] dati = new byte[fs.Length];
                fs.Read(dati, 0, (int)fs.Length);
                fs.Close();

                ((DocsPAWA.DocsPaWR.Templates)Session["templateSelPerModelli"]).PATH_ALLEGATO_1 = "Modelli\\" + codiceAmministrazione + "\\" + template.DESCRIZIONE + "\\Allegato1.rtf";
                ProfilazioneDocManager.salvaModelli(dati, template.DESCRIZIONE, codiceAmministrazione, "Allegato1.rtf", "doc", (DocsPAWA.DocsPaWR.Templates)Session["templateSelPerModelli"], this);
            }

            if (!string.IsNullOrEmpty(uploadPathModExc.Value) && Session["templateSelPerModelli"] != null)
            {
                DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["templateSelPerModelli"];
                HttpPostedFile p = uploadPathModExc.PostedFile;
                Stream fs = p.InputStream;
                byte[] dati = new byte[fs.Length];
                fs.Read(dati, 0, (int)fs.Length);
                fs.Close();
                ((DocsPAWA.DocsPaWR.Templates)Session["templateSelPerModelli"]).PATH_MODELLO_EXCEL = "Modelli\\" + codiceAmministrazione + "\\" + template.DESCRIZIONE + "\\ModelloEXC.xls";
                ProfilazioneDocManager.salvaModelli(dati, template.DESCRIZIONE, codiceAmministrazione, "ModelloEXC.xls", "doc", (DocsPAWA.DocsPaWR.Templates)Session["templateSelPerModelli"], this);
            }

			RegisterStartupScript("ChiudiAssociazioneModelli","<script>window.close();</script>");
		}

		private void ImageButton1_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			if(CheckBox1.Checked)
			{
				string[] amministrazione = ((string) Session["AMMDATASET"]).Split('@');
				string codiceAmministrazione  = amministrazione[0];
				
				((DocsPAWA.DocsPaWR.Templates) Session["templateSelPerModelli"]).PATH_MODELLO_1 = "";
				DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates) Session["templateSelPerModelli"];
				ProfilazioneDocManager.eliminaModelli(template.DESCRIZIONE,codiceAmministrazione,"Modello1."+template.PATH_MODELLO_1_EXT,template.PATH_MODELLO_1_EXT,(DocsPAWA.DocsPaWR.Templates) Session["templateSelPerModelli"], this);

                // Se è attivo M/Text viene visualizzata la modalità Classica per il modello principale
                if (MTextUtils.IsActiveMTextIntegration())
                {
                    this.ddlModelTypeMain.SelectedIndex = 0;
                    this.pnlModelTypeMain.Visible = false;
                    this.uploadPathUno.Visible = true;
                }

				CheckBox1.Checked = false;
			}		
		}

		private void ImageButton2_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			if(CheckBox2.Checked)
			{
				string[] amministrazione = ((string) Session["AMMDATASET"]).Split('@');
				string codiceAmministrazione  = amministrazione[0];
				
				((DocsPAWA.DocsPaWR.Templates) Session["templateSelPerModelli"]).PATH_MODELLO_2 = "";		
				DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates) Session["templateSelPerModelli"];
			 	ProfilazioneDocManager.eliminaModelli(template.DESCRIZIONE,codiceAmministrazione,"Modello2.rtf","doc",(DocsPAWA.DocsPaWR.Templates) Session["templateSelPerModelli"], this);

                // Se è attivo M/Text viene visualizzata la modalità Classica per il modello principale
                if (MTextUtils.IsActiveMTextIntegration())
                {
                    this.ddlModelTypeAtt.SelectedIndex = 0;
                    this.pnlModelTypeAtt.Visible = false;
                    this.uploadPathDue.Visible = true;
                }

				CheckBox2.Checked = false;
			}			
		}

        protected void ImageButton3_Click(object sender, ImageClickEventArgs e)
        {
            if (CheckBox3.Checked)
            {
                string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
                string codiceAmministrazione = amministrazione[0];

                ((DocsPAWA.DocsPaWR.Templates)Session["templateSelPerModelli"]).PATH_ALLEGATO_1 = "";
                DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["templateSelPerModelli"];
                ProfilazioneDocManager.eliminaModelli(template.DESCRIZIONE, codiceAmministrazione, "Allegato1.rtf", "doc", (DocsPAWA.DocsPaWR.Templates)Session["templateSelPerModelli"], this);
                CheckBox3.Checked = false;
            }		
        }

        protected void ImageButton4_Click(object sender, ImageClickEventArgs e)
        {
            if (CheckBox4.Checked)
            {
                string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
                string codiceAmministrazione = amministrazione[0];

                ((DocsPAWA.DocsPaWR.Templates)Session["templateSelPerModelli"]).PATH_MODELLO_STAMPA_UNIONE = "";
                DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["templateSelPerModelli"];
                ProfilazioneDocManager.eliminaModelli(template.DESCRIZIONE, codiceAmministrazione, "ModelloSU.rtf", "doc", (DocsPAWA.DocsPaWR.Templates)Session["templateSelPerModelli"], this);
                CheckBox4.Checked = false;
            }
        }

        protected void ImageButton6_Click(object sender, ImageClickEventArgs e)
        {
            if (CheckBox5.Checked)
            {
                string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
                string codiceAmministrazione = amministrazione[0];

                ((DocsPAWA.DocsPaWR.Templates)Session["templateSelPerModelli"]).PATH_MODELLO_EXCEL = "";
                DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["templateSelPerModelli"];
                ProfilazioneDocManager.eliminaModelli(template.DESCRIZIONE, codiceAmministrazione, "ModelloEXC.xls", "doc", (DocsPAWA.DocsPaWR.Templates)Session["templateSelPerModelli"], this);
                CheckBox5.Checked = false;
            }
        }

        #region Event Handler per i pulsanti di aggiunta manuale di un datasource. Sezione da rimuovere a risoluzione bug da parte di M/Text

        protected void btnAddTemplateDoc_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(this.txtModelNameMain.Text))
            {
                try
                {
                    String mTextBindingName = MTextUtils.GetNameFromQualifiedName(this.txtModelNameMain.Text);
                    this.ddlMTextModelMain.Items.Add(new ListItem(mTextBindingName, this.txtModelNameMain.Text));
                }
                catch (Exception ex)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('Nome del data bind non valido.');", true);
                }

            }

        }

        protected void btnAddTemplateAtt_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(this.txtModelNameAtt.Text))
            {
                try
                {
                    String mTextBindingName = MTextUtils.GetNameFromQualifiedName(this.txtModelNameAtt.Text);
                    this.ddlMTextModelAtt.Items.Add(new ListItem(mTextBindingName, this.txtModelNameAtt.Text));
                }
                catch (Exception ex)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('Nome del data bind non valido.');", true);
                }

            }

        }

        #endregion

        #region Gestione pulsanti per la generazione di un datasource di esempio da utilizzare per la creazione 

        /// <summary>
        /// URL alla pagina per la generazione di un file XML di esempio da utilizzare per la creazione
        /// del template M/Text
        /// </summary>
        protected String UrlToMainSourceGeneratorPage
        {
            get
            {
                return String.Format("{0}/Models/MText/ModelGenerator.aspx", Utils.getHttpFullPath());
            }
        }

        #endregion

    }
}
