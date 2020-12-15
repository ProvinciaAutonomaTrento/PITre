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
using System.Text.RegularExpressions;

namespace DocsPAWA.popup
{
	/// <summary>
	/// Summary description for printLabel.
	/// </summary>
    public class printLabel : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.Button btn_ok;
		protected System.Web.UI.WebControls.Button UpSx;
		protected System.Web.UI.WebControls.Button UpDx;
		protected System.Web.UI.WebControls.Button DownSx;
		protected System.Web.UI.WebControls.Button DownDx;
		protected System.Web.UI.WebControls.Button btn_chiudi;
        protected System.Web.UI.WebControls.Button btn_save;
        protected System.Web.UI.WebControls.Button btn_only_print;
        protected System.Web.UI.WebControls.Button btn_refresh_preview;
		protected System.Web.UI.WebControls.TextBox tbxPosX;
		protected System.Web.UI.WebControls.TextBox tbxPosY;
		private string personalize="";
		protected System.Web.UI.WebControls.TextBox docHeight;
		protected System.Web.UI.WebControls.TextBox docWidth;
		protected System.Web.UI.HtmlControls.HtmlInputHidden tb_hidden;
		private string [] posPers;
        protected System.Web.UI.WebControls.RadioButtonList Tim_Segn_List;
        protected System.Web.UI.WebControls.DropDownList CarattereList;
        protected System.Web.UI.WebControls.DropDownList ColoreList;
        //protected System.Web.UI.WebControls.DropDownList RotazioneList;
        //protected System.Web.UI.WebControls.DropDownList OrientaList;
        protected System.Web.UI.WebControls.CheckBox chkInfoFirma;
        protected System.Web.UI.WebControls.RadioButtonList infoPosFirmaList;
        protected System.Web.UI.WebControls.RadioButtonList rblTipoFirma;
        protected System.Web.UI.WebControls.Panel pnlTipoFirma;
        protected System.Web.UI.WebControls.Label label3;
        private string firma = "";

        public bool IsPermanentDisplaysSegnature
        {
            get
            {
                return (utils.InitConfigurationKeys.GetValue("0", "FE_PERMANENT_DISPLAYS_SEGNATURE").Equals("1")) ? true : false;
            }
        }

		private void Page_Load(object sender, System.EventArgs e)
		{
            //se la pagina è richiesta per il posizionamento della segnatura nel documento acquisito ed è attiva la chiave permanent_displays_signature(mev 109 inps)
            if (utils.InitConfigurationKeys.GetValue("0", "FE_PERMANENT_DISPLAYS_SEGNATURE").Equals("1") && Request.QueryString["proto"] == null)
            {
                DocsPaWR.SchedaDocumento schedaCorrente = DocumentManager.getDocumentoSelezionato(this);
                string diritti = "0";
                if (schedaCorrente != null)
                    diritti = schedaCorrente.accessRights;
                // Abilito il pulsante 'Salva Versione con segnatura' se l'utente ha i permessi di
                if (diritti.Equals("0") || diritti.Equals("255") || diritti.Equals("63"))
                    this.btn_save.Visible = true;
                //disabilito il pulsante di conferma ed abilito il pulsante 'Applicata a stampa'
                this.btn_ok.Visible = false;
                this.btn_only_print.Visible = true;
            }
            Page.ClientScript.RegisterStartupScript(this.GetType(), "onloadWindow", "<script>closeWind(window.event);</script>");
            btn_chiudi.Attributes["onClick"] = "window.close(); window.dialogArguments.document.location=window.dialogArguments.document.location;";
			//btn_ok.Attributes["onClick"] = "isNum_p();";
			Utils.DefaultButton(this,ref tbxPosX,ref btn_ok);
			Utils.DefaultButton(this,ref tbxPosY,ref btn_ok);
            if (IsPermanentDisplaysSegnature)
            {
                tbxPosX.AutoPostBack = false;
                tbxPosY.AutoPostBack = false;
            }
            
            //se non è un timbro non si può selezionare l'orientamento
            //if (this.Tim_Segn_List.SelectedValue != "true")
            //{
            //    this.OrientaList.Enabled = false;
            //}
            //else
            //{
            //    this.OrientaList.Enabled = true;
            //}

			if (tb_hidden.Value!=null)
			{
				if (tb_hidden.Value=="tbxPosX")
				{
					SetFocus(this,tbxPosX);
				}
				if (tb_hidden.Value=="tbxPosY")
				{
					SetFocus(this,tbxPosY);
				}
			}

            //VALORIZZO LE DROPDOWNLIST DEL CARATTERE E DEL COLORE********************************
            DocsPAWA.AdminTool.Manager.AmministrazioneManager amm = new DocsPAWA.AdminTool.Manager.AmministrazioneManager();
            amm.ListaAmministrazioni();
            string idAmm = UserManager.getUtente().idAmministrazione;
            DocsPAWA.DocsPaWR.InfoAmministrazione currAmm = new DocsPAWA.DocsPaWR.InfoAmministrazione();
            foreach (DocsPAWA.DocsPaWR.InfoAmministrazione infoAmm in amm.getListaAmministrazioni())
            {
                if (infoAmm.IDAmm.Equals(idAmm))
                {
                    currAmm = infoAmm;
                    break;
                }
            }
            DocsPAWA.DocsPaWR.color[] colore = currAmm.Timbro.color;
            //se non ho caricato i colori ho solo l'item blank per questo deve essere < 2.
            if (ColoreList.Items.Count < 2)
            {
                ColoreList.Items.Add("");
                for (int i = 0; i < colore.Length; i++)
                {
                    ColoreList.Items.Add(colore[i].descrizione);
                    //if (colore[i].id == currAmm.Timbro_colore)
                    //{
                    //    ColoreList.SelectedIndex = i + 1; //aggiungo 1 perchè gli id partono da 1 e non da 0.
                    //}
                }
            }
            DocsPAWA.DocsPaWR.carattere[] carat = currAmm.Timbro.carattere;
            //se non ho caricato i caratteri ho solo l'item blank per questo deve essere < 2.
            if (CarattereList.Items.Count < 2)
            {
                CarattereList.Items.Add("");
                for (int j = 0; j < carat.Length; j++)
                {
                    CarattereList.Items.Add(carat[j].caratName + " - " + carat[j].dimensione);
                }
            }

            firma = FileManager.getSelectedFile().firmato;
			if (!IsPostBack)
			{
				//recupero dalla session le label properties
                
                #region appo
                if (Session["labelProperties"] != null)
                {
                    DocsPaWR.FileDocumento lbProp = new DocsPAWA.DocsPaWR.FileDocumento();
                    lbProp.LabelPdf = (DocsPAWA.DocsPaWR.labelPdf)Session["labelProperties"];
                    double a = Convert.ToDouble(lbProp.LabelPdf.pdfHeight);
                    double b = Convert.ToDouble(lbProp.LabelPdf.pdfWidth);
                    
                    docHeight.Text = Convert.ToString(Convert.ToInt32(a));
                    docWidth.Text = Convert.ToString(Convert.ToInt32(b));

                   
                    /*
                    if (Session["primaPag"] == null)
                    {
                        string posPagFirma = Session["firma"].ToString();
                    }

                   */
                    //verifico se sono state scelte posizioni personalizzate
                    if (Session["personalize"] != null)
                    {
                        string position = Session["personalize"].ToString();
                        posPers = position.Split(Convert.ToChar("-"));
                        if (posPers.Length > 1)
                        {
                            lbProp.LabelPdf.default_position = "posPers";
                        }
                    }

                    switch (lbProp.LabelPdf.default_position)
                    {
                        case "pos_upSx": //è in posizione 0
                            tbxPosX.Text = lbProp.LabelPdf.positions[0].PosX;
                            tbxPosY.Text = lbProp.LabelPdf.positions[0].PosY;
                            UpSx.BackColor = System.Drawing.Color.DarkGray;
                            SetFocus(this, UpSx);
                            break;

                        case "pos_upDx": //è in posizione 1
                            if (Convert.ToInt32(lbProp.LabelPdf.positions[1].PosX) > Convert.ToInt32(docWidth.Text))
                            {
                                tbxPosX.Text = docWidth.Text;
                            }
                            else
                            {
                                tbxPosX.Text = lbProp.LabelPdf.positions[1].PosX;
                            }
                            if (Convert.ToInt32(lbProp.LabelPdf.positions[1].PosY) > Convert.ToInt32(docHeight.Text))
                            {
                                tbxPosY.Text = docHeight.Text;
                            }
                            else
                            {
                                tbxPosY.Text = lbProp.LabelPdf.positions[1].PosY;
                            }

                            UpDx.BackColor = System.Drawing.Color.DarkGray;
                            SetFocus(this, UpDx);
                            break;
                        case "pos_downSx": //è in posizione 2
                            if (Convert.ToInt32(lbProp.LabelPdf.positions[2].PosX) > Convert.ToInt32(docWidth.Text))
                            {
                                tbxPosX.Text = docWidth.Text;
                            }
                            else
                            {
                                tbxPosX.Text = lbProp.LabelPdf.positions[2].PosX;
                            }
                            if (Convert.ToInt32(lbProp.LabelPdf.positions[2].PosY) > Convert.ToInt32(docHeight.Text))
                            {
                                tbxPosY.Text = docHeight.Text;
                            }
                            else
                            {
                                tbxPosY.Text = lbProp.LabelPdf.positions[2].PosY;
                            }


                            DownSx.BackColor = System.Drawing.Color.DarkGray;
                            SetFocus(this, DownSx);
                            break;
                        case "pos_downDx": //è in posizione 3
                            if (Convert.ToInt32(lbProp.LabelPdf.positions[3].PosX) > Convert.ToInt32(docWidth.Text))
                            {
                                tbxPosX.Text = docWidth.Text;
                            }
                            else
                            {
                                tbxPosX.Text = lbProp.LabelPdf.positions[3].PosX;
                            }
                            if (Convert.ToInt32(lbProp.LabelPdf.positions[3].PosY) > Convert.ToInt32(docHeight.Text))
                            {
                                tbxPosY.Text = docHeight.Text;
                            }
                            else
                            {
                                tbxPosY.Text = lbProp.LabelPdf.positions[3].PosY;
                            }
                            DownDx.BackColor = System.Drawing.Color.DarkGray;
                            SetFocus(this, DownDx);
                            break;
                        case "posPers": // posizione personalizzata
                            tbxPosX.Text = posPers[0].ToString();
                            tbxPosY.Text = posPers[1].ToString();
                            break;
                        default:
                            //svuoto le tbx
                            tbxPosX.Text = "";
                            tbxPosY.Text = "";

                            break;
                    }

                    //Carico le caratteristiche del Timbro/Segnatura
                    Tim_Segn_List.SelectedValue = "false";
                    if (lbProp.LabelPdf.orientamento != null && lbProp.LabelPdf.orientamento != string.Empty)
                    {
                        this.Tim_Segn_List.SelectedValue = lbProp.LabelPdf.orientamento.ToLower();
                        if (Tim_Segn_List.SelectedValue.ToLower() != "false")
                        {
                            Session.Add("tipoLabel", true);
                        }
                        else
                        {
                            Session.Add("tipoLabel", false);
                        }
                        Session.Add("orientamento", Tim_Segn_List.SelectedValue);
                    }
                    else
                    {
                        //CaricaValoriDefault();
                        Session.Add("orientamento", "false");
                        Session.Add("tipoLabel", false);

                    }

                    //Mev Firma1 <
                    if (Session["notimbro"] != null)
                    {
                        if (Session["notimbro"].ToString() == "true")
                        {
                            Session.Add("orientamento", "false");
                            Session.Add("tipoLabel", false);
                            Tim_Segn_List.SelectedValue = "notimbro";
                        }
                    }
                    //>
                    //if (lbProp.LabelPdf.tipoLabel != null)
                    //{
                    //    Tim_Segn_List.SelectedValue = "false";
                    //    if (lbProp.LabelPdf.tipoLabel)
                    //    {
                    //        if (lbProp.LabelPdf.orientamento != null)
                    //        {
                    //            OrientaList.SelectedValue = lbProp.LabelPdf.orientamento;
                    //        }
                    //        OrientaList.Enabled = true;
                    //        Tim_Segn_List.SelectedValue = "true";
                    //    }
                    //}
                    if (lbProp.LabelPdf.label_rotation != null && lbProp.LabelPdf.label_rotation != string.Empty)
                    {
                        //RotazioneList.SelectedValue = lbProp.LabelPdf.label_rotation;
                    }
                    
                    // MODIFICA PER CARATTERE E COLORE ***************************************
                    //if (!string.IsNullOrEmpty(lbProp.LabelPdf.font_color))
                    if (!string.IsNullOrEmpty(lbProp.LabelPdf.sel_color))
                    {
                        ColoreList.SelectedIndex = System.Convert.ToInt32(lbProp.LabelPdf.sel_color); //lbProp.LabelPdf.font_color;
                    }
                    //if (!(string.IsNullOrEmpty(lbProp.LabelPdf.font_size) || string.IsNullOrEmpty(lbProp.LabelPdf.font_type)))
                    if(!string.IsNullOrEmpty(lbProp.LabelPdf.sel_font))
                    {
                        CarattereList.SelectedIndex = System.Convert.ToInt32(lbProp.LabelPdf.sel_font); //lbProp.LabelPdf.font_type + " - " + lbProp.LabelPdf.font_size;
                    }
                    
                    if (firma.Equals("1"))
                    {
                        // Caricamento dati stampa firma digitale
                        this.chkInfoFirma.Checked = (lbProp.LabelPdf.digitalSignInfo != null);

                        if (this.chkInfoFirma.Checked)
                        {
                            if (lbProp.LabelPdf.digitalSignInfo.printOnFirstPage)
                                this.infoPosFirmaList.SelectedValue = "printOnFirstPage";
                            else
                            {
                                this.infoPosFirmaList.SelectedValue = "printOnLastPage";
                            }
                            //Mev Firma1 <
                            if (lbProp.LabelPdf.digitalSignInfo.printFormatSign == DocsPaWR.TypePrintFormatSign.Sign_Short)
                                this.rblTipoFirma.SelectedValue = "SIGN_SHORT";
                            else
                                this.rblTipoFirma.SelectedValue = "SIGN_EXT";
                            //>
                        }

                        this.infoPosFirmaList.Enabled = this.chkInfoFirma.Checked;
                        //Mev Firma1 <
                        this.rblTipoFirma.Enabled = this.chkInfoFirma.Checked;
                        //>
                    }
                    else
                    {
                        this.chkInfoFirma.Enabled = false;
                        this.label3.Enabled = false;
                        this.infoPosFirmaList.Enabled = false;
                        //Mev Firma1 <
                          this.infoPosFirmaList.Enabled = false;
                        //>
                    }
                }
                else
                {
                    docWidth.Text = "585";
                    docHeight.Text = "842";
                    CaricaValoriDefault();
                }
                #endregion
                if (Request.QueryString["proto"] == "true")
                {
                    //chiamata da docProtocollo
                    docWidth.Text = "585";
                    docHeight.Text = "842";                  
                    this.chkInfoFirma.Enabled = false;
                    this.label3.Enabled = false;
                    this.infoPosFirmaList.Enabled = false;
                    //Mev Firma1 <
                     this.rblTipoFirma.Enabled = false;
                    //>
                }

                //MEV-Firma 1 < Visualizza i controlli di firma Completa/Sintetica
                pnlTipoFirma.Visible = false;
                string keyDettFirma =DocsPAWA.utils.InitConfigurationKeys.GetValue(currAmm.IDAmm, "FE_DETTAGLI_FIRMA");

                if (!String.IsNullOrEmpty (keyDettFirma))
                    if (keyDettFirma.Equals("1"))
                        pnlTipoFirma.Visible = true;
                //>

                

                //MEV-Firma1 < Disabilita "Nessun Timbro/Sign." se doc non firmato
                Tim_Segn_List.Items[3].Enabled = (firma.Equals("1")) ? true : false;
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
            this.UpSx.Click += new System.EventHandler(this.btn_Click);
            this.UpDx.Click += new System.EventHandler(this.btn_Click);
            this.DownSx.Click += new System.EventHandler(this.btn_Click);
            this.DownDx.Click += new System.EventHandler(this.btn_Click);
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            this.btn_save.Click += new System.EventHandler(this.btn_ok_Click);
            this.btn_only_print.Click += new System.EventHandler(this.btn_ok_Click);
            this.ColoreList.SelectedIndexChanged += new System.EventHandler(this.ColoreList_SelectedIndexChanged);
            this.CarattereList.SelectedIndexChanged += new System.EventHandler(this.CarattereList_SelectedIndexChanged);
            this.tbxPosX.TextChanged += new System.EventHandler(this.tbxPos_TextChanged);
            this.tbxPosY.TextChanged += new System.EventHandler(this.tbxPos_TextChanged);
            this.btn_refresh_preview.Click += new System.EventHandler(this.tbxPos_TextChanged);
            this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion




		private void btn_ok_Click(object sender, System.EventArgs e)
		{
			if ((tbxPosX.Text != "") || (tbxPosY.Text !=""))
			{
				if (Session["labelProperties"]!=null)
				{
					DocsPaWR.FileDocumento lbProp = new DocsPAWA.DocsPaWR.FileDocumento();
					lbProp.LabelPdf = (DocsPAWA.DocsPaWR.labelPdf) Session["labelProperties"];

                    if (this.chkInfoFirma.Checked)
                    {
                        lbProp.LabelPdf.digitalSignInfo = new DocsPaWR.labelPdfDigitalSignInfo();
                        lbProp.LabelPdf.digitalSignInfo.printOnFirstPage = this.infoPosFirmaList.Items.FindByValue("printOnFirstPage").Selected;
                        lbProp.LabelPdf.digitalSignInfo.printOnLastPage = this.infoPosFirmaList.Items.FindByValue("printOnLastPage").Selected;
                        //Mev Firma1 <
                          lbProp.LabelPdf.digitalSignInfo.printFormatSign = (this.rblTipoFirma.SelectedValue == "SIGN_EXT") ? DocsPaWR.TypePrintFormatSign.Sign_Extended : DocsPaWR.TypePrintFormatSign.Sign_Short; 
                        //>
                        Session.Add("printOnFirstPage", lbProp.LabelPdf.digitalSignInfo.printOnFirstPage);
                        Session.Add("printOnLastPage", lbProp.LabelPdf.digitalSignInfo.printOnLastPage);
                        //Mev Firma1 <
                          Session.Add("printFormatSign", lbProp.LabelPdf.digitalSignInfo.printFormatSign);                        
                        //>
                    }
                    else
                    {
                        lbProp.LabelPdf.digitalSignInfo = null;
                        Session.Remove("printOnFirstPage");
                        Session.Remove("printOnLastPage");
                        //Mev Firma <
                          Session.Remove("printFormatSign");
                        //>
                    }

                    #region verifiche di posizionamento personalizzato di Y
					// se viene scelta posizione Y = 0 viene impostata come 
					// dimensione minima quella del font
					if (tbxPosY.Text == "0")
					{
						tbxPosY.Text = lbProp.LabelPdf.font_size;
					}
					// se viene scelta la massima dimensione in Y viene sottratta 
					// la massima alla dimensione del font
					if (tbxPosY.Text == lbProp.LabelPdf.pdfHeight)
					{
						tbxPosY.Text = Convert.ToString((Convert.ToInt32(lbProp.LabelPdf.pdfHeight) - Convert.ToInt32(lbProp.LabelPdf.font_size)));
					}
					#endregion

					#region verifiche di posizionamento personalizzato di X
					// le verifiche per la X dal momento che deve essere 
					// calcolata l'occupazione della segnatura in pixel
					// viene effettuata e depurata sul backend
					#endregion
				}
                /*
                if ((tbxPosX.Text != "" && isNan(tbxPosX)) && (tbxPosY.Text != "" && isNan(tbxPosY)) && (this.Request.QueryString["proto"] != "true"))
				{
					personalize = tbxPosX.Text+"-"+tbxPosY.Text;
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "chiamata_isNum", "<script>isNum_p();</script>");
					Session.Add("personalize",personalize);
				}
				else
				{
                    //chiamata da docProtocollo
                    if (this.Request.QueryString["proto"] == "true")
                    {
                        personalize = tbxPosX.Text + "-" + tbxPosY.Text;
                        Session.Add("personalize", personalize);
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Verifico dati", "<script>isNumProtocollo();</script>");
                       // Page.ClientScript.RegisterStartupScript(this.GetType(), "AproModale", "<script>ApriModaleVisDocPdf(); </script>");
                    }
                    else
                    {
                        // VALORI NON NUMERICI
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "chiamata_isNum", "<script>isNum_p();</script>");
                    }
				}*/
                //chiamata da docProtocollo
                if (this.Request.QueryString["proto"] != null && this.Request.QueryString["proto"] == "true")
                {
                    personalize = tbxPosX.Text + "-" + tbxPosY.Text;
                    Session.Add("personalize", personalize);
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Verifico dati", "<script>isNumProtocollo();</script>");
                    // Page.ClientScript.RegisterStartupScript(this.GetType(), "AproModale", "<script>ApriModaleVisDocPdf(); </script>");
                }
                else if (Session["personalize"] == null || Session["personalize"].ToString().Equals(""))
                {
                    DocsPAWA.AdminTool.Manager.AmministrazioneManager amm = new DocsPAWA.AdminTool.Manager.AmministrazioneManager();
                    amm.ListaAmministrazioni();
                    string idAmm = UserManager.getUtente().idAmministrazione;
                    DocsPAWA.DocsPaWR.InfoAmministrazione currAmm = new DocsPAWA.DocsPaWR.InfoAmministrazione();
                    foreach (DocsPAWA.DocsPaWR.InfoAmministrazione infoAmm in amm.getListaAmministrazioni())
                    {
                        if (infoAmm.IDAmm.Equals(idAmm))
                        {
                            currAmm = infoAmm;
                            break;
                        }
                    }
                    // seleziona la posizione configurata in Amministrazione
                    //DocsPaVO.amministrazione.posizione currPos = new DocsPaVO.amministrazione.posizione();
                    DocsPaWR.posizione currPos = new DocsPAWA.DocsPaWR.posizione();
                    foreach (DocsPaWR.posizione pos in currAmm.Timbro.positions)
                    {
                        if (pos.id.Equals(currAmm.Timbro_posizione))
                        {
                            currPos = pos;
                            Session.Add("personalize", pos.posName);
                            break;
                        }
                    }
                }

			}
			else
			{   // VALORI NON INSERITI
				Page.ClientScript.RegisterStartupScript(this.GetType(),"chiamata_isNum","<script>isNum_p();</script>");
			}
            string Rotazione = ""; //this.RotazioneList.SelectedValue;
            Session.Add("rotazione", Rotazione);
            //string Orientamento = this.OrientaList.SelectedValue;
            //Session.Add("orientamento", Orientamento);
            Session.Add("carattere", this.CarattereList.SelectedIndex);
            Session.Add("colore", this.ColoreList.SelectedIndex);

            //solo se la modale è richiamata dalla tabDoc
            if (this.Request.QueryString["proto"] == null)
            {
                //se ho cliccato su "Applica a stampa" setto la variabile di sessione, per la corretta visualizzazione del doc alla pressione del button Zoom nella finestra principale 
                if (((Button)sender).ID.Equals("btn_only_print"))
                    Session.Add("VisSegn", "1");
                else
                {
                    if (Session["VisSegn"] != null)
                        Session.Remove("VisSegn");
                }

                //se ho richiesto di salvare il documento con impressa la segnatura o semplicemente di stampare il doc con segnatura ne tengo traccia nella sessione
                if (((Button)sender).ID.Equals("btn_save"))
                    Session.Add("permanent_displays_segnature", "true");
                //questa chiave di sessione permette il refresh del documento nella pagina chiamante
                Session.Add("refreshDocWithSegnature", "true");
                string scriptJS = "<script>" +
                "window.close();" +
                "window.dialogArguments.document.location=window.dialogArguments.document.location;" +
                "</script>";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "close", scriptJS);
            }

            // Garantisce che nella selezione del comando “Visualizza dati con segnatura” siano sempre e 
            // solo mostrati i dati identificativi del documento. 
            // Non vengono mantenute in sessione eventuali selezioni operate nella popup di posizionamento dati.
            if (Session["SHOWDOCWITHSEGNATURE"] != null) Session.Remove("SHOWDOCWITHSEGNATURE");
            //>
		}

        private void btn_Click(object sender, System.EventArgs e)
        {
            Button btn = (Button)sender;
            string btnName = btn.ID;

            switch (btnName)
            {
                case "UpSx":
                    personalize = "pos_upSx";
                    Session.Add("personalize", personalize);
                    if (this.Request.QueryString["proto"] == "true")
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "AproModale", "<script>ApriModaleVisDocPdf(); </script>");
                    }

                    break;


                case "UpDx":
                    personalize = "pos_upDx";
                    Session.Add("personalize", personalize);
                    if (this.Request.QueryString["proto"] == "true")
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "AproModale", "<script>ApriModaleVisDocPdf(); </script>");
                    }
                    break;

                case "DownSx":
                    personalize = "pos_downSx";
                    Session.Add("personalize", personalize);
                    if (this.Request.QueryString["proto"] == "true")
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "AproModale", "<script>ApriModaleVisDocPdf(); </script>");
                    }
                    break;

                case "DownDx":
                    personalize = "pos_downDx";
                    Session.Add("personalize", personalize);
                    if (this.Request.QueryString["proto"] == "true")
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "AproModale", "<script>ApriModaleVisDocPdf(); </script>");
                    }
                    break;

            }
            CaricaValoriDefault(personalize);
            if (this.Request.QueryString["proto"] == null)
            {
                string valore = string.Empty, valTipo = string.Empty, valRotaz = string.Empty, valCarat = string.Empty, valColor = string.Empty, valOrienta = string.Empty;
                //Mev Firma1 < 
                string valNoTimbro = string.Empty;
                //>
                if (Session["personalize"] != null)
                {
                    valore = Session["personalize"].ToString();
                    //Session.Remove("personalize");
                    if (Session["tipoLabel"] != null)
                    {
                        valTipo = Session["tipoLabel"].ToString();
                    }
                    if (Session["rotazione"] != null)
                    {
                        valRotaz = Session["rotazione"].ToString();
                    }
                    if (Session["carattere"] != null)
                    {
                        valCarat = Session["carattere"].ToString();
                    }
                    if (Session["colore"] != null)
                    {
                        valColor = Session["colore"].ToString();
                    }
                    if (Session["orientamento"] != null)
                    {
                        valOrienta = Session["orientamento"].ToString();
                    }

                    //Mev Firma1 <
                    if (Session["notimbro"] != null)
                    {
                        valNoTimbro = Session["notimbro"].ToString();
                    }
                    //>
                }


                string url = "../documento/docVisualizzaFrame.aspx?id=" + Session.SessionID + "&plusEtic=1&pos=" + valore + "&tipo=" + valTipo + "&rotazione=" + valRotaz + "&carattere=" + valCarat + "&colore=" + valColor + "&orientamento=" + valOrienta + "&notimbro=" + valNoTimbro;
                string scriptJS = "<script>" +
                    " var windowOpener=window.dialogArguments;" +
                    "windowOpener.iFrameDoc.document.location='" + url + "';" +
                    "</script>";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "aggiornaDoc", scriptJS);
            }
            // Garantisce che nella selezione del comando “Visualizza dati con segnatura” siano sempre e 
            // solo mostrati i dati identificativi del documento. 
            // Non vengono mantenute in sessione eventuali selezioni operate nella popup di posizionamento dati.
            if (Session["SHOWDOCWITHSEGNATURE"] != null) Session.Remove("SHOWDOCWITHSEGNATURE");
            //>
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void chkInfoFirma_CheckedChanged(object sender, EventArgs e)
        {
            this.infoPosFirmaList.Enabled = this.chkInfoFirma.Checked;
            //Mev Firma1 <
              this.rblTipoFirma.Enabled = this.chkInfoFirma.Checked;
            //Mev Firma1 - Se viene deselezionata la checkbox di firma allora viene forzata la selezione del radiobuttom di segnatura  
              if (!chkInfoFirma.Checked)
              {
                  this.Tim_Segn_List.SelectedValue = "false";
                  Session.Add("tipoLabel", false);
                  Session.Add("orientamento", "false");
                  Session.Add("notimbro", "false");
              }       
            //>
        }

		#region utilsMethod
		public static void SetFocus(Page page,object control)
		{
			// verifica che il controllo a cui si vuole dare il focus non sia nullo
			if(control==null)
				return;

			// rileva l'ID univoco del controllo
			string ctrlID = ((WebControl)control).UniqueID;

			// implementa scripting client-side per focus
			string script;
			script = "<script language=\"javascript\">";
			script += "var control = document.getElementById(\"" + ctrlID + "\");";
			script += "if(control!=null){control.focus();";
			// se è un TextBox, oltre a dare il focus, seleziona il testo
			if(control.GetType().ToString()=="System.Web.UI.WebControls.TextBox")
			{
				script += "control.select();";
			}
			script += "}<";
			script += "/script>";

			// "inietta" il codice client-side nell'output HTML della pagina
            page.ClientScript.RegisterStartupScript(page.GetType(), "Focus", script); 
		}


		public static bool isNan(System.Web.UI.WebControls.TextBox tb)
		{
			bool result=false;
			
			string pattern="^[0-9]*$";
			Regex NumberPattern =new Regex(pattern);
			result = NumberPattern.IsMatch(tb.Text);
			return result;

		}




		#endregion

        protected void Tim_Segn_List_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (this.Tim_Segn_List.SelectedValue != "true")
            //{
            //    this.OrientaList.Enabled = false;
            //}
            //else
            //{
            //    this.OrientaList.Enabled = true;
            //}
            bool TipoLabel = false;
            string Orientamento = "false";
            if (this.Tim_Segn_List.SelectedValue.ToLower() != "false")
            {
                if (this.Tim_Segn_List.SelectedValue.ToLower() == "orizzontale")
                {
                    TipoLabel = true;
                    Orientamento = "orizzontale";
                }
                else
                {
                    if (this.Tim_Segn_List.SelectedValue.ToLower() == "verticale")
                    {
                        TipoLabel = true;
                        Orientamento = "verticale";
                    }
                }
            }
            Session.Add("tipoLabel", TipoLabel);
            Session.Add("orientamento", Orientamento);

            //anteprima documento
            if (this.Request.QueryString["proto"] == null)
            {
                string valore = string.Empty, valTipo = string.Empty, valRotaz = string.Empty, valCarat = string.Empty, valColor = string.Empty, valOrienta = string.Empty;

                //Mev Firma1 <
                 string valNoTimbro = string.Empty; 
                //>
                if (Session["personalize"] != null)
                    valore = Session["personalize"].ToString();
                if (Session["tipoLabel"] != null)
                    valTipo = Session["tipoLabel"].ToString();
                if (Session["rotazione"] != null)
                    valRotaz = Session["rotazione"].ToString();
                if (Session["colore"] != null)
                    valColor = Session["colore"].ToString();
                if (Session["orientamento"] != null)
                    valOrienta = Session["orientamento"].ToString();

                //Mev Firma1<
                if (Tim_Segn_List.SelectedValue.ToString().ToUpper() == "NOTIMBRO")
                {
                    Session.Add("notimbro", "true");
                    valNoTimbro = "true";
                    //abilita il chk di firma
                    chkInfoFirma.Checked = true;
                    //abilita il radio di print Inizio Pagina
                    infoPosFirmaList.Enabled = true;
                    //abilita il radio di print tipo firma Completa/Sintetica
                    rblTipoFirma.Enabled = true;
                }
                else
                {
                    Session.Add("notimbro", "false");
                    valNoTimbro = "false";
                }//>

                //set valore carattere
                if (Session["carattere"] == null)
                    Session.Add("carattere", CarattereList.SelectedIndex.ToString());
                else
                    Session["carattere"] = CarattereList.SelectedIndex.ToString();
                valCarat = Session["carattere"].ToString();

                string url = "../documento/docVisualizzaFrame.aspx?id=" + Session.SessionID + "&plusEtic=1&pos=" + valore + "&tipo=" + valTipo + "&rotazione=" + valRotaz + "&carattere=" + valCarat + "&colore=" + valColor + "&orientamento=" + valOrienta + "&notimbro=" + valNoTimbro;
                string scriptJS = "<script>" +
                    " var windowOpener=window.dialogArguments;" +
                    "windowOpener.iFrameDoc.document.location='" + url + "';" +
                    "</script>";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "aggiornaDoc", scriptJS);
            }
        }

        //serve ad aggiornare il preview del documento al cambio del colore associato alla segnatura
        private void ColoreList_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.Request.QueryString["proto"] == null)
            {
                string valore = string.Empty, valTipo = string.Empty, valRotaz = string.Empty, valCarat = string.Empty, valColor = string.Empty, valOrienta = string.Empty;
                //Mev Firma1<
                string valNoTimbro = string.Empty;
                //>
                if (Session["personalize"] != null)
                    valore = Session["personalize"].ToString();
                if (Session["tipoLabel"] != null)
                    valTipo = Session["tipoLabel"].ToString();
                if (Session["rotazione"] != null)
                    valRotaz = Session["rotazione"].ToString();
                if (Session["carattere"] != null)
                    valCarat = Session["carattere"].ToString();
                if (Session["orientamento"] != null)
                    valOrienta = Session["orientamento"].ToString();
                //Mev Firma1<
                if (Session["notimbro"] != null)
                    valNoTimbro = Session["notimbro"].ToString();
                //>


                //set valore colore
                if (Session["colore"] == null)
                    Session.Add("colore", ColoreList.SelectedIndex.ToString());
                else
                    Session["colore"] = ColoreList.SelectedIndex.ToString();
                valColor = Session["colore"].ToString();
                string url = "../documento/docVisualizzaFrame.aspx?id=" + Session.SessionID + "&plusEtic=1&pos=" + valore + "&tipo=" + valTipo + "&rotazione=" + valRotaz + "&carattere=" + valCarat + "&colore=" + valColor + "&orientamento=" + valOrienta + "&notimbro=" + valNoTimbro;
                string scriptJS = "<script>" +
                    " var windowOpener=window.dialogArguments;" +
                    "windowOpener.iFrameDoc.document.location='" + url + "';" +
                    "</script>";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "aggiornaDoc", scriptJS);
            }
        }

        //serve ad aggiornare il preview del documento al cambio del font associato alla segnatura
        protected void CarattereList_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.Request.QueryString["proto"] == null)
            {
                string valore = string.Empty, valTipo = string.Empty, valRotaz = string.Empty, valCarat = string.Empty, valColor = string.Empty, valOrienta = string.Empty;
                //Mev Firma1<
                string valNoTimbro = string.Empty;
                //>
                if (Session["personalize"] != null)
                    valore = Session["personalize"].ToString();
                if (Session["tipoLabel"] != null)
                    valTipo = Session["tipoLabel"].ToString();
                if (Session["rotazione"] != null)
                    valRotaz = Session["rotazione"].ToString();
                if (Session["colore"] != null)
                    valColor = Session["colore"].ToString();
                if (Session["orientamento"] != null)
                    valOrienta = Session["orientamento"].ToString();
                //Mev Firma1<
                if (Session["notimbro"] != null)
                    valNoTimbro= Session["notimbro"].ToString();
                //>
                //set valore carattere
                if (Session["carattere"] == null)
                    Session.Add("carattere", CarattereList.SelectedIndex.ToString());
                else
                    Session["carattere"] = CarattereList.SelectedIndex.ToString();
                valCarat = Session["carattere"].ToString();

                string url = "../documento/docVisualizzaFrame.aspx?id=" + Session.SessionID + "&plusEtic=1&pos=" + valore + "&tipo=" + valTipo + "&rotazione=" + valRotaz + "&carattere=" + valCarat + "&colore=" + valColor + "&orientamento=" + valOrienta + "&notimbro=" + valNoTimbro;
                string scriptJS = "<script>" +
                    " var windowOpener=window.dialogArguments;" +
                    "windowOpener.iFrameDoc.document.location='" + url + "';" +
                    "</script>";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "aggiornaDoc", scriptJS);
            }
        }

        // aggiorna il preview del documento al variare della coordinata di posizionamento X
        protected void tbxPos_TextChanged(object sender, System.EventArgs e)
        {
            if ((tbxPosX.Text != "" && isNan(tbxPosX)) && (tbxPosY.Text != "" && isNan(tbxPosY)) && (this.Request.QueryString["proto"] != "true"))
            {
                string valore = string.Empty, valTipo = string.Empty, valRotaz = string.Empty, valCarat = string.Empty, valColor = string.Empty, valOrienta = string.Empty;
                //Mev File1 <
                string valNoTimbro = string.Empty;
                //>
                personalize = tbxPosX.Text + "-" + tbxPosY.Text;
                //set valore
                if (Session["personalize"] == null)
                    Session.Add("personalize", personalize);
                else
                    Session["personalize"] = personalize;
                valore = Session["personalize"].ToString();

                if (Session["tipoLabel"] != null)
                    valTipo = Session["tipoLabel"].ToString();
                if (Session["rotazione"] != null)
                    valRotaz = Session["rotazione"].ToString();
                if (Session["colore"] != null)
                    valColor = Session["colore"].ToString();
                if (Session["orientamento"] != null)
                    valOrienta = Session["orientamento"].ToString();
                //Mev Firma1<
                if (Session["notimbro"] != null)
                    valNoTimbro = Session["notimbro"].ToString();
                //>

                if (Session["carattere"] != null)
                    valCarat = Session["carattere"].ToString();

                Page.ClientScript.RegisterStartupScript(this.GetType(), "chiamata_isNum", "<script>isNum_p();</script>");
                string url = "../documento/docVisualizzaFrame.aspx?id=" + Session.SessionID + "&plusEtic=1&pos=" + valore + "&tipo=" + valTipo + "&rotazione=" + valRotaz + "&carattere=" + valCarat + "&colore=" + valColor + "&orientamento=" + valOrienta + "&notimbro=" + valNoTimbro;
                string scriptJS = "<script>" +
                    " var windowOpener=window.dialogArguments;" +
                    "windowOpener.iFrameDoc.document.location='" + url + "';" +
                    "</script>";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "aggiornaDoc", scriptJS);
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "chiamata_isNum", "<script>isNum_p();</script>");
            }
        }

        //Serve a caricare i valori di default se non sono in sessione per visualizzarli sul front-end
        protected void CaricaValoriDefault()
        {
            // prende tutte le amm.ni disponibili
            DocsPAWA.AdminTool.Manager.AmministrazioneManager amm = new DocsPAWA.AdminTool.Manager.AmministrazioneManager();
            amm.ListaAmministrazioni();
            string idAmm = UserManager.getUtente().idAmministrazione;
            DocsPAWA.DocsPaWR.InfoAmministrazione currAmm = new DocsPAWA.DocsPaWR.InfoAmministrazione();
            foreach (DocsPAWA.DocsPaWR.InfoAmministrazione infoAmm in amm.getListaAmministrazioni())
            {
                if (infoAmm.IDAmm.Equals(idAmm))
                {
                    currAmm = infoAmm;
                    break;
                }
            }

            // carico il colore ed il carattere di default dall'amministrazione se NON sono in sessione!!!
            if (Session["carattere"] != null)
            {
                CarattereList.SelectedIndex = System.Convert.ToInt32(Session["carattere"].ToString());
            }
            else
            {
                if (!string.IsNullOrEmpty(currAmm.Timbro_carattere))
                {
                    CarattereList.SelectedIndex = System.Convert.ToInt32(currAmm.Timbro_carattere);
                }
                Session.Add("carattere", CarattereList.SelectedIndex);
            }

            if (Session["colore"] != null)
            {
                ColoreList.SelectedIndex = System.Convert.ToInt32(Session["colore"].ToString());
            }
            else
            {
                if (!string.IsNullOrEmpty(currAmm.Timbro_colore))
                {
                    ColoreList.SelectedIndex = System.Convert.ToInt32(currAmm.Timbro_colore);
                }
                Session.Add("colore", ColoreList.SelectedIndex);
            }

            // carico l'orientamento di default, se il timbro è disabilitato sarà sempre la segnatura
            Tim_Segn_List.SelectedValue = currAmm.Timbro_orientamento.ToLower();
            if (Tim_Segn_List.SelectedValue == null || Tim_Segn_List.SelectedValue == string.Empty)
            {
                Tim_Segn_List.SelectedValue = "false";
            }
            Session.Add("orientamento", Tim_Segn_List.SelectedValue);
            if (Tim_Segn_List.SelectedValue.ToLower() == "false")
            {
                Session.Add("tipoLabel", false);
            }
            else
            {
                Session.Add("tipoLabel", true);
            }
            
            // seleziona la posizione configurata in Amministrazione
            //DocsPaVO.amministrazione.posizione currPos = new DocsPaVO.amministrazione.posizione();
            DocsPaWR.posizione currPos = new DocsPAWA.DocsPaWR.posizione();
            foreach (DocsPaWR.posizione pos in currAmm.Timbro.positions)
            {
                if(pos.id.Equals(currAmm.Timbro_posizione))
                {
                    currPos = pos;
                    break;
                }
            }
            // metto in sessione la posizione di default
            personalize = currPos.posName;
            Session.Add("personalize", personalize);
            // carica la posizione configurata sul front-end
            switch (currPos.posName)
            {
                case "pos_upSx": //è in posizione 0
                    tbxPosX.Text = currPos.PosX;
                    tbxPosY.Text = currPos.PosY;
                    UpSx.BackColor = System.Drawing.Color.DarkGray;
                    SetFocus(this, UpSx);
                    break;

                case "pos_upDx": //è in posizione 1
                    if (Convert.ToInt32(currPos.PosX) > Convert.ToInt32(docWidth.Text))
                    {
                        tbxPosX.Text = docWidth.Text;
                    }
                    else
                    {
                        tbxPosX.Text = currPos.PosX;
                    }
                    if (Convert.ToInt32(currPos.PosY) > Convert.ToInt32(docHeight.Text))
                    {
                        tbxPosY.Text = docHeight.Text;
                    }
                    else
                    {
                        tbxPosY.Text = currPos.PosY;
                    }
                    UpDx.BackColor = System.Drawing.Color.DarkGray;
                    SetFocus(this, UpDx);
                    break;

                case "pos_downSx": //è in posizione 2
                    if (Convert.ToInt32(currPos.PosX) > Convert.ToInt32(docWidth.Text))
                    {
                        tbxPosX.Text = docWidth.Text;
                    }
                    else
                    {
                        tbxPosX.Text = currPos.PosX;
                    }
                    if (Convert.ToInt32(currPos.PosY) > Convert.ToInt32(docHeight.Text))
                    {
                        tbxPosY.Text = docHeight.Text;
                    }
                    else
                    {
                        tbxPosY.Text = currPos.PosY;
                    }
                    DownSx.BackColor = System.Drawing.Color.DarkGray;
                    SetFocus(this, DownSx);
                    break;

                case "pos_downDx": //è in posizione 3
                    if (Convert.ToInt32(currPos.PosX) > Convert.ToInt32(docWidth.Text))
                    {
                        tbxPosX.Text = docWidth.Text;
                    }
                    else
                    {
                        tbxPosX.Text = currPos.PosX;
                    }
                    if (Convert.ToInt32(currPos.PosY) > Convert.ToInt32(docHeight.Text))
                    {
                        tbxPosY.Text = docHeight.Text;
                    }
                    else
                    {
                        tbxPosY.Text = currPos.PosY;
                    }
                    DownDx.BackColor = System.Drawing.Color.DarkGray;
                    SetFocus(this, DownDx);
                    break;

                default:
                    //svuoto le tbx
                    tbxPosX.Text = "";
                    tbxPosY.Text = "";
                    break;
            }
        }

        //Serve a caricare i valori in base al bottone selezionato per visualizzarli sul front-end
        protected void CaricaValoriDefault(string selPos)
        {
            // prende tutte le amm.ni disponibili
            DocsPAWA.AdminTool.Manager.AmministrazioneManager amm = new DocsPAWA.AdminTool.Manager.AmministrazioneManager();
            amm.ListaAmministrazioni();
            string idAmm = UserManager.getUtente().idAmministrazione;
            DocsPAWA.DocsPaWR.InfoAmministrazione currAmm = new DocsPAWA.DocsPaWR.InfoAmministrazione();
            foreach (DocsPAWA.DocsPaWR.InfoAmministrazione infoAmm in amm.getListaAmministrazioni())
            {
                if (infoAmm.IDAmm.Equals(idAmm))
                {
                    currAmm = infoAmm;
                    break;
                }
            }

            // carico il colore ed il carattere di default dall'amministrazione se NON sono in sessione!!!
            if (Session["carattere"] != null)
            {
                CarattereList.SelectedIndex = System.Convert.ToInt32(Session["carattere"].ToString());
            }
            else
            {
                if (!string.IsNullOrEmpty(currAmm.Timbro_carattere))
                {
                    CarattereList.SelectedIndex = System.Convert.ToInt32(currAmm.Timbro_carattere);
                }
                Session.Add("carattere", CarattereList.SelectedIndex);
            }

            if (Session["colore"] != null)
            {
                ColoreList.SelectedIndex = System.Convert.ToInt32(Session["colore"].ToString());
            }
            else
            {
                if (!string.IsNullOrEmpty(currAmm.Timbro_colore))
                {
                    ColoreList.SelectedIndex = System.Convert.ToInt32(currAmm.Timbro_colore);
                }
                Session.Add("colore", ColoreList.SelectedIndex);
            }

            // seleziona la posizione configurata in Amministrazione
            //DocsPaVO.amministrazione.posizione currPos = new DocsPaVO.amministrazione.posizione();
            DocsPaWR.posizione currPos = new DocsPAWA.DocsPaWR.posizione();
            foreach (DocsPaWR.posizione pos in currAmm.Timbro.positions)
            {
                if (pos.posName.Equals(selPos))
                {
                    currPos = pos;
                    break;
                }
            }
            // deseleziona i bottoni a livello grafico
            UpSx.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
            UpDx.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
            DownSx.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
            DownDx.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);

            //CHP
            if (Session["labelProperties"] != null)
            {
                DocsPaWR.FileDocumento lbProp = new DocsPAWA.DocsPaWR.FileDocumento();
                lbProp.LabelPdf = (DocsPAWA.DocsPaWR.labelPdf)Session["labelProperties"];

                if (this.chkInfoFirma.Checked)
                {
                    lbProp.LabelPdf.digitalSignInfo = new DocsPaWR.labelPdfDigitalSignInfo();
                    lbProp.LabelPdf.digitalSignInfo.printOnFirstPage = this.infoPosFirmaList.Items.FindByValue("printOnFirstPage").Selected;
                    lbProp.LabelPdf.digitalSignInfo.printOnLastPage = this.infoPosFirmaList.Items.FindByValue("printOnLastPage").Selected;
                    //Mev Firma1 <
                      lbProp.LabelPdf.digitalSignInfo.printFormatSign = (this.rblTipoFirma.SelectedValue == "SIGN_EXT") ? DocsPaWR.TypePrintFormatSign.Sign_Extended : DocsPaWR.TypePrintFormatSign.Sign_Short;
                    //>

                    Session.Add("printOnFirstPage", lbProp.LabelPdf.digitalSignInfo.printOnFirstPage);
                    Session.Add("printOnLastPage", lbProp.LabelPdf.digitalSignInfo.printOnLastPage);
                    //Mev Firma1 <
                      Session.Add("printFormatSign", lbProp.LabelPdf.digitalSignInfo.printFormatSign);
                    //>
                }
                else
                {
                    lbProp.LabelPdf.digitalSignInfo = null;
                    Session.Remove("printOnFirstPage");
                    Session.Remove("printOnLastPage");
                    //Mev Firma <
                     Session.Remove("printFormatSign");
                    //>
                }

                #region verifiche di posizionamento personalizzato di Y
                // se viene scelta posizione Y = 0 viene impostata come 
                // dimensione minima quella del font
                if (tbxPosY.Text == "0")
                {
                    tbxPosY.Text = lbProp.LabelPdf.font_size;
                }
                // se viene scelta la massima dimensione in Y viene sottratta 
                // la massima alla dimensione del font
                if (tbxPosY.Text == lbProp.LabelPdf.pdfHeight)
                {
                    tbxPosY.Text = Convert.ToString((Convert.ToInt32(lbProp.LabelPdf.pdfHeight) - Convert.ToInt32(lbProp.LabelPdf.font_size)));
                }
                #endregion

                #region verifiche di posizionamento personalizzato di X
                // le verifiche per la X dal momento che deve essere 
                // calcolata l'occupazione della segnatura in pixel
                // viene effettuata e depurata sul backend
                #endregion
            }

            // carica la posizione configurata sul front-end
            switch (selPos)
            {
                case "pos_upSx": //è in posizione 0
                    tbxPosX.Text = currPos.PosX;
                    tbxPosY.Text = currPos.PosY;
                    //Page.ClientScript.RegisterStartupScript(this.GetType(), "Set_X_Y", "<script>document.all." + tbxPosX.ClientID + ".value = " + currPos.PosX + "; document.all." + tbxPosY.ClientID + ".value = " + currPos.PosY + "; </script>");
                    UpSx.BackColor = System.Drawing.Color.DarkGray;
                    SetFocus(this, UpSx);
                    break;

                case "pos_upDx": //è in posizione 1
                    tbxPosX.Text = currPos.PosX;
                    tbxPosY.Text = currPos.PosY;
                    //Page.ClientScript.RegisterStartupScript(this.GetType(), "Set_X_Y", "<script>document.all." + tbxPosX.ClientID + ".value = " + currPos.PosX + "; document.all." + tbxPosY.ClientID + ".value = " + currPos.PosY + "; </script>");
                    UpDx.BackColor = System.Drawing.Color.DarkGray;
                    SetFocus(this, UpDx);
                    break;

                case "pos_downSx": //è in posizione 2
                    tbxPosX.Text = currPos.PosX;
                    tbxPosY.Text = currPos.PosY;
                    //Page.ClientScript.RegisterStartupScript(this.GetType(), "Set_X_Y", "<script>document.all." + tbxPosX.ClientID + ".value = " + currPos.PosX + "; document.all." + tbxPosY.ClientID + ".value = " + currPos.PosY + "; </script>");
                    DownSx.BackColor = System.Drawing.Color.DarkGray;
                    SetFocus(this, DownSx);
                    break;

                case "pos_downDx": //è in posizione 3
                    tbxPosX.Text = currPos.PosX;
                    tbxPosY.Text = currPos.PosY;
                    //Page.ClientScript.RegisterStartupScript(this.GetType(), "Set_X_Y", "<script>document.all." + tbxPosX.ClientID + ".value = " + currPos.PosX + "; document.all." + tbxPosY.ClientID + ".value = " + currPos.PosY + "; </script>");
                    DownDx.BackColor = System.Drawing.Color.DarkGray;
                    SetFocus(this, DownDx);
                    break;

                default:
                    //svuoto le tbx
                    tbxPosX.Text = "";
                    tbxPosY.Text = "";
                    break;
            }
        }

	}
}
