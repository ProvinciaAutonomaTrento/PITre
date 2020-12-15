using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;

namespace SAAdminTool.AdminTool.Gestione_Grafica
{
    public partial class Grafica : System.Web.UI.Page
    {
        protected System.Web.UI.WebControls.Label lbl_position;
        protected System.Web.UI.WebControls.Label lbl_avviso;
        protected System.Web.UI.WebControls.Label sfondo;
        protected System.Web.UI.WebControls.DropDownList colori;
        protected System.Web.UI.WebControls.DropDownList ddl_TemaAmministraz;
        protected System.Web.UI.WebControls.DropDownList ddl_segnatura;
        protected System.Web.UI.WebControls.TextBox _red;
        protected System.Web.UI.WebControls.TextBox _blu;
        protected System.Web.UI.WebControls.TextBox _green;
        protected System.Web.UI.WebControls.Button ModificaEnte;
        protected System.Web.UI.WebControls.Button ModificaEnteSfondo;
        protected System.Web.UI.WebControls.Button ModificaColoreTesto;
        protected System.Web.UI.WebControls.Button ModificaSfondoTesto;
        protected System.Web.UI.WebControls.Button btn_modificaTema;
        protected System.Web.UI.WebControls.Button btn_segnatura;
        protected System.Web.UI.HtmlControls.HtmlInputFile uploadLogoEnte;
        protected System.Web.UI.HtmlControls.HtmlInputFile uploadSfondo;
        protected System.Web.UI.HtmlControls.HtmlInputFile uploadSfondo2;
        protected System.Web.UI.WebControls.Image logoEnte;
        protected System.Web.UI.WebControls.Image sfondoLogoEnte;
        protected System.Web.UI.WebControls.Image sfondoTesto;
        protected System.Web.UI.HtmlControls.HtmlTableCell backgroundTesto;
        protected System.Web.UI.WebControls.TableCell Td1;
        protected System.Web.UI.WebControls.TableCell Td2;
        private string idAmm;
        protected System.Web.UI.WebControls.Button btn_refresh;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Response.Expires = -1;
            Session["AdminBookmark"] = "GestioneGrafica";
            idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
            if (!IsPostBack)
            {
                this.lbl_position.Text = "&nbsp;&bull;&nbsp;Amministrazione: " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "1");
                if (this.ddl_TemaAmministraz.Items.Count == 0)
                {
                    caricaDdlTema(idAmm);
                    caricaDdlSegnatura(idAmm);
                }
            }
        }
 
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        private void InitializeComponent()
        {
            this.colori.SelectedIndexChanged += new System.EventHandler(this.colori_SelectedIndexChanged);
            this.ModificaEnte.Click += new System.EventHandler(this.Mod_LogoEnte);
            this.ModificaEnteSfondo.Click += new System.EventHandler(this.Mod_SfondoEnte);
            this.ModificaSfondoTesto.Click += new System.EventHandler(this.Mod_Sfondo);
            this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new System.EventHandler(this.Grafica_PreRender);
        }

        private void caricaDdlTema(string idAmm)
        {
            SAAdminTool.DocsPaWR.DocsPaWebService ws = new SAAdminTool.DocsPaWR.DocsPaWebService();
            ArrayList temi = new ArrayList(ws.getListaTemi());
            System.Web.UI.WebControls.ListItem list = new System.Web.UI.WebControls.ListItem();
            list.Value = "0";
            list.Text = "";
            ddl_TemaAmministraz.Items.Add(list);

            for (int i = 0; i < temi.Count; i++)
            {
                System.Web.UI.WebControls.ListItem li = new System.Web.UI.WebControls.ListItem();
                string[] valori = temi[i].ToString().Split('^');
                li.Value = valori[0];
                li.Text = valori[1];
                ddl_TemaAmministraz.Items.Add(li);
            }

            string temaAmmin = ws.getCssAmministrazione(idAmm);
            if (temaAmmin != null && !temaAmmin.Equals(""))
            {
                string[] realTema = temaAmmin.Split('^');
                this.ddl_TemaAmministraz.SelectedIndex = Convert.ToInt32(realTema[1]);
            }
            else
                this.ddl_TemaAmministraz.SelectedIndex = 0;
        }

        private void caricaDdlSegnatura(string idAmm)
        {
            SAAdminTool.DocsPaWR.DocsPaWebService ws = new SAAdminTool.DocsPaWR.DocsPaWebService();
            //System.Web.UI.WebControls.ListItem list = new System.Web.UI.WebControls.ListItem();
            //list.Value = "0";
            //list.Text = "Nero";
            //ddl_segnatura.Items.Add(list);

            //list.Value = "1";
            //list.Text = "Blu";
            //ddl_segnatura.Items.Add(list);
            
            //list.Value = "2";
            //list.Text = "Rosso";
            //ddl_segnatura.Items.Add(list);

            string segnAmm = ws.getSegnAmm(idAmm);
            if (segnAmm != null && !segnAmm.Equals(""))
                this.ddl_segnatura.SelectedIndex = Convert.ToInt32(segnAmm);
            else
                this.ddl_segnatura.SelectedIndex = 0;
        }

        private void Grafica_PreRender(object sender, System.EventArgs e)
        {
            string coloreAmministrazione = findFontColor(idAmm);
            if (coloreAmministrazione != "")
            {
                string[] colorSplit = coloreAmministrazione.Split('^');
                this._red.Text = colorSplit[0];
                this._green.Text = colorSplit[1];
                this._blu.Text = colorSplit[2];
                this.sfondo.ForeColor = System.Drawing.Color.FromArgb(Convert.ToInt16(this._red.Text), Convert.ToInt16(this._green.Text), Convert.ToInt16(this._blu.Text));
            }
            else
            {
                this._red.Text = "255";
                this._green.Text = "255";
                this._blu.Text = "255";
                this.sfondo.ForeColor = System.Drawing.Color.FromArgb(Convert.ToInt16(this._red.Text), Convert.ToInt16(this._green.Text), Convert.ToInt16(this._blu.Text));
            }

            //string colorePulsantiera = findPulsColor(idAmm);
            //if (colorePulsantiera != "")
            //{
            //    string esadec = "0123456789ABCDEF";
            //    string result = "#";

            //    string[] colorSplit = colorePulsantiera.Split('^');
            //    this.puls_red.Text = colorSplit[0];
            //    this.puls_green.Text = colorSplit[1];
            //    this.puls_blu.Text = colorSplit[2];
            //    int r1 = Convert.ToInt32(Math.Floor(Convert.ToDouble(this.puls_red.Text) / 16));
            //    result += esadec.Substring(r1, 1);
            //    result += esadec.Substring((Convert.ToInt32(this.puls_red.Text) % 16), 1);
            //    int r2 = Convert.ToInt32(Math.Floor(Convert.ToDouble(this.puls_green.Text) / 16));
            //    result += esadec.Substring(r2, 1);
            //    result += esadec.Substring((Convert.ToInt32(this.puls_green.Text) % 16), 1);
            //    int r3 = Convert.ToInt32(Math.Floor(Convert.ToDouble(this.puls_blu.Text) / 16));
            //    result += esadec.Substring(r3, 1);
            //    result += esadec.Substring((Convert.ToInt32(this.puls_blu.Text) % 16), 1);

            //    this.td_pulsantiera.BgColor = result;
            //}
            //else
            //{
            //    this.puls_red.Text = "129";
            //    this.puls_green.Text = "14";
            //    this.puls_blu.Text = "06";
            //    this.td_pulsantiera.BgColor = "#810d06";
            //}

            this.colori.SelectedIndex = 0;
            //this.btn_documenti.ImageUrl = utils.InitImagePath.getInstance(idAmm).getPath("DOCUMENTI");
            //this.btn_documentiSel.ImageUrl = utils.InitImagePath.getInstance(idAmm).getPath("DOCUMENTI_ATTIVO");
            //this.btn_ricerca.ImageUrl = utils.InitImagePath.getInstance(idAmm).getPath("RICERCA");
            //this.btn_ricercaSel.ImageUrl = utils.InitImagePath.getInstance(idAmm).getPath("RICERCA_ATTIVO");
            //this.btn_gestione.ImageUrl = utils.InitImagePath.getInstance(idAmm).getPath("GESTIONE");
            //this.btn_gestioneSel.ImageUrl = utils.InitImagePath.getInstance(idAmm).getPath("GESTIONE_ATTIVO");
            //this.btn_opzioni.ImageUrl = utils.InitImagePath.getInstance(idAmm).getPath("OPZIONI");
            //this.btn_aiuto.ImageUrl = utils.InitImagePath.getInstance(idAmm).getPath("AIUTO");
            //this.btn_esci.ImageUrl = utils.InitImagePath.getInstance(idAmm).getPath("ESCI");
            this.logoEnte.ImageUrl = utils.InitImagePath.getInstance(idAmm).getPath("LOGO");
            this.sfondoLogoEnte.ImageUrl = utils.InitImagePath.getInstance(idAmm).getPath("BKG_TESTO");
            this.sfondoTesto.ImageUrl = utils.InitImagePath.getInstance(idAmm).getPath("BKG_LOGO");

            SAAdminTool.DocsPaWR.DocsPaWebService ws = new SAAdminTool.DocsPaWR.DocsPaWebService();
            string path = ws.getpath("FrontEnd");
            if (fileExist("backgroundlogo_" + idAmm + ".gif", "FrontEnd"))
                this.backgroundTesto.Attributes.Add("background", path + "\\" + "backgroundlogo_" + idAmm + ".gif");
            else
                if (fileExist("backgroundlogo_" + idAmm + ".jpg", "FrontEnd"))
                    this.backgroundTesto.Attributes.Add("background", path + "\\" + "backgroundlogo_" + idAmm + ".jpg");
                else
                    this.backgroundTesto.Attributes.Add("background", "../../images/testata/320/sf2.jpg");
           
            //string urlSfondo = this.sfondoLogoEnte.ImageUrl.Substring(2);
            //this.backgroundTesto.Attributes.Add("background", "../../" + urlSfondo);
        }

        private void colori_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.colori.SelectedIndex != 0)
            {
                string color = this.colori.SelectedValue;
                string[] colorSplit = color.Split('^');
                this._red.Text = colorSplit[0];
                this._blu.Text = colorSplit[2];
                this._green.Text = colorSplit[1];
                this.sfondo.ForeColor = System.Drawing.Color.FromArgb(Convert.ToInt16(this._red.Text), Convert.ToInt16(this._green.Text), Convert.ToInt16(this._blu.Text));
            }
        }

        private bool fileExist(string fileName, string type)
        {
            return FileManager.fileExist(fileName, type);
        }

        private string findFontColor(string idAmm)
        {
            return FileManager.findFontColor(idAmm);
        }

        //private string findPulsColor(string idAmm)
        //{
        //    return FileManager.findPulsColor(idAmm);
        //}

        private bool verificaFile(System.Web.UI.HtmlControls.HtmlInputFile upload)
        {
            if (upload.Value == "" || upload.Value == null)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "selezioneNonValida_1", "alert('Selezionare un file valido.');", true);
                return false;
            }

            //Controllo del tipo di file
            if (upload.Value != "")
            {
                if (upload.Value != null)
                {
                    string[] path = upload.Value.Split('.');
                    if (path.Length != 0)
                    {
                        if (path[path.Length - 1].ToLower() != "gif" && path[path.Length - 1].ToLower() != "jpg")
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "selezioneNonValida_2", "alert('I files validi sono solo quelli con estensione .gif o .jpg');", true);
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private bool insertFile(System.Web.UI.HtmlControls.HtmlInputFile upload, string typeFile, string name)
        {
            HttpPostedFile p = upload.PostedFile;
            Stream fs = p.InputStream;
            byte[] dati = new byte[fs.Length];
            fs.Read(dati, 0, (int)fs.Length);
            fs.Close();
            bool result = true;

            int num = p.FileName.LastIndexOf('\\');
            string fileInput = p.FileName.Substring(num + 1);
            string[] estensione = fileInput.Split('.');

            SAAdminTool.DocsPaWR.DocsPaWebService ws = new SAAdminTool.DocsPaWR.DocsPaWebService();
            string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
            string idAmm = amministrazione[3];
            string nomeFile = name + idAmm + "." + estensione[1];

            ws.Timeout = System.Threading.Timeout.Infinite;

            SAAdminTool.AdminTool.Manager.SessionManager sessionManager = new SAAdminTool.AdminTool.Manager.SessionManager();

            try
            {
                result = insert(sessionManager.getUserAmmSession(), dati, nomeFile, ws.getpath(typeFile));
                utils.InitImagePath.getInstance(idAmm).clear();
                return result;
            }
            catch (Exception e)
            {
                //Debugger.Write("Errore in DocsPaWS.asmx  - metodo: insertFile", e);
                return false;
            }

            return result;
        }

        public static bool insert(DocsPaWR.InfoUtente infoUtente, byte[] dati, string nomeFile, string serverPath)
        {
            bool result = true;

            try
            {
                //Controllo se esiste la Directory nel path dove vengono salvate le immagini modificabili.
                //Se esiste copio il file excel li' dentro, altrimenti la creo e ci copio il file.
                //In ogni caso poichè il nome del file è fisso, anche se quest'ultimo esiste viene sovrascritto.
                // Debugger.Write("Metodo \"insertFile\" classe \"AmministraManager\" : inizio salvataggio file " + nomeFile);
                //if (Directory.Exists(serverPath))
                //{
                    string[] nome = nomeFile.Split('.');
                    if(nome[1].ToUpper().Equals("GIF") && File.Exists(serverPath + "\\" + nome[0] + ".jpg"))
                        File.Delete(serverPath + "\\" + nome[0] + ".jpg");
                    if(nome[1].ToUpper().Equals("JPG") &&  File.Exists(serverPath + "\\" + nome[0] + ".gif"))
                        File.Delete(serverPath + "\\" + nome[0] + ".gif");

                    FileStream fs1 = new FileStream(serverPath + "\\" + nomeFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    fs1.Write(dati, 0, dati.Length);
                    fs1.Close();
                //}
                //else
                //{
                //    Directory.CreateDirectory(serverPath);
                //    FileStream fs1 = new FileStream(serverPath + "\\" + nomeFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                //    fs1.Write(dati, 0, dati.Length);
                //    fs1.Close();
                //}
            }
            catch (Exception ex)
            {
                //Debugger.Write("Metodo \"insertFile\" classe \"AmministraManager\" ERRORE : " + ex.Message);
                
                result = false;
                return result;
            }

            return result;
        }

        protected void Mod_LogoEnte(object sender, EventArgs e)
        {
            if (verificaFile(uploadLogoEnte))
            {
                if (!this.insertFile(uploadLogoEnte, "FrontEnd", "logoente_"))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "erratoInserimento", "alert('Inserimento file con esito negativo')", true);
                }
            }
        }

        protected void Mod_SfondoEnte(object sender, EventArgs e)
        {
            if (verificaFile(uploadSfondo))
            {
                if (!this.insertFile(uploadSfondo, "FrontEnd", "backgroundlogo_"))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "erratoInserimento", "alert('Inserimento file con esito negativo')", true);
                }
            }
        }

        protected void Mod_Sfondo(object sender, EventArgs e)
        {
            if (verificaFile(uploadSfondo2))
            {
                if (!this.insertFile(uploadSfondo2, "FrontEnd", "backgroundlogoente_"))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "erratoInserimento", "alert('Inserimento file con esito negativo');", true);
                }
            }
        } 

        protected void Mod_Tema(object sender, EventArgs e)
        {
            if (ddl_TemaAmministraz.SelectedIndex != 0)
            {
                SAAdminTool.DocsPaWR.DocsPaWebService ws = new SAAdminTool.DocsPaWR.DocsPaWebService();
                if (!ws.setTemaAmministrazione(idAmm, Convert.ToInt32(ddl_TemaAmministraz.SelectedValue)))
                    RegisterClientScriptBlock("jb", "<script>alert('Errore nella modifica del Tema dell\\'Amministrazione');</script>");
                else
                {
                    idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
                    utils.InitImagePath iip = SAAdminTool.utils.InitImagePath.getInstance(idAmm);
                    iip.clear();
                    utils.InitImagePath.getInstance(idAmm);
                    RegisterClientScriptBlock("modOk", "<script>alert('Modifica del tema avvenuta correttamente.');</script>");
                }
            }
        }

        protected void Mod_Segn(object sender, EventArgs e)
        {
            SAAdminTool.DocsPaWR.DocsPaWebService ws = new SAAdminTool.DocsPaWR.DocsPaWebService();
            if (!ws.setColoreSegnatura(idAmm, Convert.ToInt32(ddl_segnatura.SelectedValue)))
                RegisterClientScriptBlock("jb", "<script>alert('Errore nella modifica del colore segnatura dell\\'Amministrazione');</script>");
            else
            {
                idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
                utils.InitImagePath iip = SAAdminTool.utils.InitImagePath.getInstance(idAmm);
                iip.clear();
                utils.InitImagePath.getInstance(idAmm);
                RegisterClientScriptBlock("modOk", "<script>alert('Modifica del colore segnatura avvenuta correttamente.');</script>");
            }
        }

        protected void Aggiorna(object sender, EventArgs e)
        {
            idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
            utils.InitImagePath iip = SAAdminTool.utils.InitImagePath.getInstance(idAmm);
            iip.clear();
            utils.InitImagePath.getInstance(idAmm);
        }

        protected void Mod_Click(object sender, EventArgs e)
        {
            string coloreSelezionato = "";

            if (Convert.ToInt16(this._red.Text) < 0 || Convert.ToInt16(this._red.Text) > 255)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "erratoValoreRosso", "alert('Inserire un valore numerico tra 0 e 255 per il rosso');", true);
                return;
            }
            if (Convert.ToInt16(this._blu.Text) < 0 || Convert.ToInt16(this._blu.Text) > 255)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "erratoValoreBlu", "alert('Inserire un valore numerico tra 0 e 255 per il blu');", true);
                return;
            }
            if (Convert.ToInt16(this._green.Text) < 0 || Convert.ToInt16(this._green.Text) > 255)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "erratoValoreVerde", "alert('Inserire un valore numerico tra 0 e 255 per il verde');", true);
                return;
            }

            if (this.colori.SelectedIndex == 0)
                coloreSelezionato = this._red.Text + "^" + this._green.Text + "^" + this._blu.Text;
            else
                coloreSelezionato = this.colori.SelectedValue;

            bool res = FileManager.changeColor(idAmm, coloreSelezionato);
            if (!res)
                ClientScript.RegisterStartupScript(this.GetType(), "errataModifica", "alert('La modifica del colore del testo non ha avuto esito positivo');", true);
        }

        //protected void Mod_SfondoPulsantiera(object sender, EventArgs e)
        //{
        //    string coloreSelezionato = "";

        //    if (Convert.ToInt16(this.puls_red.Text) < 0 || Convert.ToInt16(this.puls_red.Text) > 255)
        //    {
        //        ClientScript.RegisterStartupScript(this.GetType(), "erratoValoreRosso", "alert('Inserire un valore numerico tra 0 e 255 per il rosso');", true);
        //        return;
        //    }
        //    if (Convert.ToInt16(this.puls_blu.Text) < 0 || Convert.ToInt16(this.puls_blu.Text) > 255)
        //    {
        //        ClientScript.RegisterStartupScript(this.GetType(), "erratoValoreBlu", "alert('Inserire un valore numerico tra 0 e 255 per il blu');", true);
        //        return;
        //    }
        //    if (Convert.ToInt16(this.puls_green.Text) < 0 || Convert.ToInt16(this.puls_green.Text) > 255)
        //    {
        //        ClientScript.RegisterStartupScript(this.GetType(), "erratoValoreVerde", "alert('Inserire un valore numerico tra 0 e 255 per il verde');", true);
        //        return;
        //    }

        //    if (this.ddl_sfondoPuls.SelectedIndex == 0)
        //        coloreSelezionato = this.puls_red.Text + "^" + this.puls_green.Text + "^" + this.puls_blu.Text;
        //    else
        //        coloreSelezionato = this.ddl_sfondoPuls.SelectedValue;

        //    bool res = FileManager.changePulsColor(idAmm, coloreSelezionato);
        //    if (!res)
        //        ClientScript.RegisterStartupScript(this.GetType(), "errataModifica", "alert('La modifica del colore del testo non ha avuto esito positivo');", true);
        //}


    }
}
