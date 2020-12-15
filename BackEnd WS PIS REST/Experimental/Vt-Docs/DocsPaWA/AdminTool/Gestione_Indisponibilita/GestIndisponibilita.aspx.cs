using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace DocsPAWA.AdminTool.Gestione_Indisponibilita
{
    public partial class GestIndisponibilita : System.Web.UI.Page
    {
        private DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
        protected DocsPAWA.DocsPaWR.Disservizio disservizio = new DocsPaWR.Disservizio();

        protected void Page_Load(object sender, EventArgs e)
        {
            disservizio = wws.getInfoDisservizio();
            if (disservizio.system_id != string.Empty)
                btn_Delete.Visible = true;
            else btn_Delete.Visible = false;
            
            if (!IsPostBack)
            {
                lbl_position.Text = "&nbsp;&bull;&nbsp;Amministrazione: " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "1");
                this.riempiCampi();
            }
            
            
            
        }

        
        private void riempiCampi()
        {
            //disservizio = wws.getInfoDisservizio();
            if (!string.IsNullOrEmpty(disservizio.system_id))
            {

                if (disservizio.stato.ToUpper() == "ATTIVO")
                {
                    lbl_stato.Text = "Disattivo";
                    lbl_stato.CssClass = "testo_rosso";
                    btn_ripristina.Visible = true;
                    btn_avvia.Visible = false;
                    //Disabilito le textbox di notifica
                    txt_email.Enabled = false;
                    txt_notifica.Enabled = false;
                    chk_email.Enabled = false;
                    btn_Delete.Visible = false;
                    btn_notifica.Visible = false;
                    
                }
                else
                {
                    lbl_stato.Text = "Attivo";
                    lbl_stato.CssClass = "testo_verde";
                    btn_avvia.Visible = true;
                    btn_ripristina.Visible = false;
                    txt_email.Enabled = true;
                    txt_notifica.Enabled = true;
                    chk_email.Enabled = true;
                    btn_notifica.Visible = true;
                    
                }
                if (disservizio.testo_notifica != string.Empty)
                    txt_notifica.Text = disservizio.testo_notifica;

                if (disservizio.testo_cortesia != string.Empty)
                    txt_cortesia.Text = disservizio.testo_cortesia;

                if (disservizio.testo_email_notifica != string.Empty)
                    txt_email.Text = disservizio.testo_email_notifica;

                if (disservizio.testo_email_ripresa != string.Empty)
                    txt_ripresa.Text = disservizio.testo_email_ripresa;

                btn_Delete.Visible = true;
                
            }
            else
            {
                lbl_stato.Text = "Attivo";
                lbl_stato.CssClass = "testo_verde";
                btn_Delete.Visible = false;
                btn_notifica.Visible = false;
                btn_avvia.Visible = false;
            }
               
        }

        protected void btn_Salva_Click(object sender, System.EventArgs e)
        {
            this.Salva();
            
        }

        protected void btn_Delete_Click(object sender, System.EventArgs e)
        {
            msg_elimina.Confirm("Si vuole eliminare definitivamente il disservizio?");
            
        }

        private void Salva()
        {
            if (disservizio.system_id != string.Empty)
            {
                if (!string.IsNullOrEmpty(txt_notifica.Text))
                    disservizio.testo_notifica = txt_notifica.Text;
                else
                {
                    this.ClientScript.RegisterStartupScript(this.GetType(), "ValidationMessage", "<script>alert('Riempire i campi obbligatori!');</script>");
                    return;
                }
                if (!string.IsNullOrEmpty(txt_cortesia.Text))
                    disservizio.testo_cortesia = txt_cortesia.Text;
                else
                {
                    this.ClientScript.RegisterStartupScript(this.GetType(), "ValidationMessage", "<script>alert('Riempire i campi obbligatori!');</script>");
                    return;
                }
                disservizio.testo_email_ripresa = txt_ripresa.Text;
                disservizio.testo_email_notifica = txt_email.Text;
               
                bool result = wws.updateDisservizio(disservizio);
            }
            else
            {
                if (!string.IsNullOrEmpty(txt_notifica.Text))
                    disservizio.testo_notifica = txt_notifica.Text;
                else
                {
                    this.ClientScript.RegisterStartupScript(this.GetType(), "ValidationMessage", "<script>alert('Riempire i campi obbligatori!');</script>");
                    return;
                }
                if (!string.IsNullOrEmpty(txt_cortesia.Text))
                    disservizio.testo_cortesia = txt_cortesia.Text;
                else
                {
                    this.ClientScript.RegisterStartupScript(this.GetType(), "ValidationMessage", "<script>alert('Riempire i campi obbligatori!');</script>");
                    return;
                }
                disservizio.testo_email_ripresa = txt_ripresa.Text;
                disservizio.testo_email_notifica = txt_email.Text;
                bool result = wws.creaDisservizio(disservizio);

            }
            disservizio = wws.getInfoDisservizio();
            this.riempiCampi();
            this.scriviFile(disservizio.testo_cortesia);

        }

        private void scriviFile(string testo)
        {
            string folder= string.Empty;
            string filename = "disservizio.txt";
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["FILE_NOTIFICA_DISSERVIZIO"]))
            {
                folder = System.Configuration.ConfigurationManager.AppSettings["FILE_NOTIFICA_DISSERVIZIO"].ToString();
                
                if (!Directory.Exists(folder))
                    // Create the subfolder
                    System.IO.Directory.CreateDirectory(folder);

                // Combine the new file name with the path
                folder = System.IO.Path.Combine(folder, filename);
                    
                // Create the file and write to it.
                if (!System.IO.File.Exists(folder))
                {
                    using (System.IO.FileStream fs = System.IO.File.Create(folder))
                    {
                        byte[] info = new System.Text.UTF8Encoding(true).GetBytes(testo);
                        fs.Write(info, 0, info.Length);

                    }

                }
                else
                {
                    using (System.IO.FileStream fs = System.IO.File.Create(folder))
                    {
                        byte[] info = new System.Text.UTF8Encoding(true).GetBytes(testo);
                        fs.Write(info, 0, info.Length);

                    }
                }

                
            }
        }

        private void deleteFile()
        {
            string folder= string.Empty;
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["FILE_NOTIFICA_DISSERVIZIO"]))
            {
                folder = System.Configuration.ConfigurationManager.AppSettings["FILE_NOTIFICA_DISSERVIZIO"].ToString();
                string[] files = Directory.GetFiles(folder);
                foreach (string file in files)
                {
                    
                    if (file.Equals(folder + "\\disservizio.txt"))
                    {
                        folder = System.IO.Path.Combine(folder, file);
                        System.IO.File.Delete(folder);
                    }
                }
                        
            }

        }

        
        protected void btn_ripristina_Click(object sender, System.EventArgs e)
        {
            //this.Salva();
            msg_ripristina.Confirm("Si vuole riavviare il servizio?");

        }

        protected void btn_notifica_Click(object sender, System.EventArgs e)
        {
            disservizio = wws.getInfoDisservizio();
            this.Salva();
            if (disservizio.notificato.Equals("3") && chk_email.Checked == true){
                //Il disservizio è già stato notificato
                msg_notifica.Confirm("Disservizio già notificato. Notificare di nuovo?");
 
            }
            else
            {
                if (disservizio.notificato.Equals("0") && chk_email.Checked == true)
                {
                    msg_notifica.Confirm("Sarà inviata una mail a tutti gli utenti. Continuare?");

                }
                else
                {
                    if(chk_email.Checked == false)
                        msg_notifica.Confirm("Per inviare la notifica anche via email selezionare la Checkbox corrispondente. Continuare senza inviare l'email?");
                }
            }
            
            
        }

        protected void btn_avvia_Click(object sender, EventArgs e)
        {
            //this.Salva();
            msg_avvia.Confirm("Si vuole avviare il disservizio del sistema?");
        }
        /// <summary>
        ///  gestione dell'evento del messaggio di conferma
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void msg_notifica_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                if (chk_email.Checked == true && !string.IsNullOrEmpty(disservizio.testo_email_notifica))
                {
                    wws.setStatoNotificaDisservizio(disservizio.system_id, "1");
                    //this.ClientScript.RegisterStartupScript(this.GetType(), "ValidationMessage", "<script>alert('Il disservizio è stato notificato agli utenti.');</script>");
                    return;
                }
                else
                {
                    if (chk_email.Checked == true && string.IsNullOrEmpty(disservizio.testo_email_notifica))
                    {
                        this.ClientScript.RegisterStartupScript(this.GetType(), "ValidationMessage", "<script>alert('Inserire il testo della mail.');</script>");
                        return;
                    }
                }
                if (chk_email.Checked == false)
                {
                    //notifico senza l'email
                    wws.setStatoNotificaDisservizio(disservizio.system_id, "0");
                    return;
                }

            }
        }


        protected void msg_avvia_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e){
            
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                //Cambio lo stato del disservizio a attivo
                if (disservizio.testo_cortesia == string.Empty){
                    this.ClientScript.RegisterStartupScript(this.GetType(), "ValidationMessage", "<script>alert('Inserire testo per la pagina di cortesia!');</script>");
                    //Response.Write("<script>alert('Inserire testo per la pagina di cortesia!');</script>");
                    return;
                }
                disservizio.stato = "attivo";
                wws.cambiaStatoDisservizio(disservizio.stato, disservizio.system_id);
                //btn_avvia.Visible = false;
                //btn_ripristina.Visible = true;
                disservizio = wws.getInfoDisservizio();
                this.riempiCampi();    
            }    
        }

        protected void msg_elimina_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                if (disservizio.system_id != null)
                {
                    if (disservizio.stato == "attivo")
                        this.ClientScript.RegisterStartupScript(this.GetType(), "ValidationMessage", "<script>alert('Ripristinare il servizio prima di cancellarlo!');</script>");

                    else
                    {
                        wws.deleteDisservizio(disservizio.system_id);
                        txt_cortesia.Text = "";
                        txt_email.Text = "";
                        txt_ripresa.Text = "";
                        txt_notifica.Text = "";
                        lbl_stato.Text = "Attivo";
                        lbl_stato.CssClass = "testo_verde";
                        btn_Delete.Visible = false;
                        btn_notifica.Visible = false;
                        btn_avvia.Visible = false;
                    }
                }
                this.deleteFile();
            }

        }

        protected void msg_ripristina_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                disservizio = wws.getInfoDisservizio();

                if (chk_ripresa.Checked == true && string.IsNullOrEmpty(disservizio.testo_email_ripresa))
                {
                    this.ClientScript.RegisterStartupScript(this.GetType(), "ValidationMessage", "<script>alert('Aggiungere testo email di ripresa del servizio.');</script>");
                    return;
                }
                else
                {
                    if (chk_ripresa.Checked == true && !string.IsNullOrEmpty(disservizio.testo_email_ripresa))
                    {
                        wws.setStatoNotificaDisservizio(disservizio.system_id, "2");
                        //Riattivo il servizio
                        disservizio.stato = "disattivo";
                        wws.cambiaStatoDisservizio(disservizio.stato, disservizio.system_id);
                        this.ClientScript.RegisterStartupScript(this.GetType(), "ValidationMessage", "<script>alert('La ripresa del servizio è stata notificata via email a tutti gli utenti.');</script>");

                    }
                }
                if (chk_ripresa.Checked == false)
                {
                    //Riattivo il servizio
                    disservizio.stato = "disattivo";
                    wws.cambiaStatoDisservizio(disservizio.stato, disservizio.system_id);
                }



                disservizio = wws.getInfoDisservizio();

                this.riempiCampi();
            }
        }

        /*protected void chk_email_CheckedChanged(object sender, System.EventArgs e)
        {
            if ((sender as CheckBox).Checked == true)
            {
                btn_notifica.Visible = true;
            }
            else
                btn_notifica.Visible = false;
        }*/
    }
}