using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;

namespace ConservazioneWA.PopUp
{
    public partial class TestLeggibilita : System.Web.UI.Page
    {
        string idConservazione;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Response.Expires = -1;


                btn_verifica.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btn_verifica.Attributes.Add("onmouseout", "this.className='cbtn';");

                btn_chiudi.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btn_chiudi.Attributes.Add("onmouseout", "this.className='cbtn';");

                idConservazione = this.Request.QueryString["idConservazione"];
                this.hd_idIstanza.Value = idConservazione;
                WSConservazioneLocale.InfoUtente infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);
                WSConservazioneLocale.ItemsConservazione[] items = ConservazioneWA.Utils.ConservazioneManager.getItemsConservazione(idConservazione, infoUtente);
                int i = 0;

                DataTable tabella1 = new DataTable();
                tabella1.Columns.Add(new DataColumn("docnumber"));
                tabella1.Columns.Add(new DataColumn("desc"));
                tabella1.Columns.Add(new DataColumn("tipo"));
                
                string[] riga1 = new string[3];
                foreach (WSConservazioneLocale.ItemsConservazione item in items)
                {
                    riga1[0]=item.DocNumber;
                    riga1[1] = item.desc_oggetto;
                    riga1[2] = item.tipoFile;
                    
                    tabella1.Rows.Add(riga1);
                    sel_file_da_aprire.Items.Add(item.DocNumber/* + item.tipoFile*/);
                    //sel_numero_file.Items.Add((i+1).ToString());
                    i++;
                }
                hd_totDocs.Value = (i).ToString();
                if (i < 3)
                {
                    rbtn_numero_file.Enabled = false;
                    lbl_numero_file.Enabled = false;
                    tb_num_file.Enabled = false;
                }
                else
                {
                    tb_num_file.Text = "3";
                }
                tb_percent_file.Text = "100";
                GridView1.DataSource = tabella1;
                GridView1.DataBind();
            }
        }

        protected void btn_chiudi_Click(object sender, EventArgs e)
        {
            Response.Write("<script>window.close();</script>");
        }

        

        protected void btn_verifica_Click(object sender, EventArgs e)
        {
            bool errore = false; int number; int totDocs;
            Int32.TryParse(hd_totDocs.Value, out totDocs);
            Int32.TryParse(tb_num_file.Text, out number);
            if (!rbtn_file_da_aprire.Checked && !rbtn_numero_file.Checked && !rbtn_percent_file.Checked)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_selezione", "alert('Selezionare un opzione!');", true);
                errore=true;
            }
            else
            {
                if (rbtn_numero_file.Checked && string.IsNullOrEmpty(tb_num_file.Text))
                {
                    errore = true;
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_insNum", "alert('Inserire il numero di file da verificare!');", true);
                }else if (rbtn_numero_file.Checked && !Int32.TryParse(tb_num_file.Text,out number))
                {
                    errore = true;
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_insNum2", "alert('Inserire un valore numerico.');", true);
                }else if(rbtn_numero_file.Checked && (number>totDocs||number<3) ){
                    errore = true;
                    if (totDocs > 2)
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_insNum3", "alert('Il valore inserito non è valido. Inserire un valore compreso tra 3 e " + totDocs + "');", true);
                    else
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_inNum4", "alert('Il valore inserito non è valido. Numero di documenti non sufficiente per la selezione casuale.');", true);
                
                }else 
                    if(rbtn_percent_file.Checked&&(string.IsNullOrEmpty(tb_percent_file.Text)||!Int32.TryParse(tb_percent_file.Text,out number))){
                    errore= true;
                    ScriptManager.RegisterStartupScript(this.Page,this.GetType(),"alt_percent1","alert('Inserire una percentuale valida.');", true);
                    }
                    else if (rbtn_percent_file.Checked && (number>100||number<1))
                    {
                        errore = true;
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_percent2", "alert('Inserire una percentuale valida.');", true);
                    }
                    else
                    
                    if (rbtn_file_da_aprire.Checked)
                {
                    errore = true;
                    foreach (GridViewRow grv in GridView1.Rows)
                    {
                        if (((CheckBox)grv.FindControl("chk_doc")).Checked) errore = false;
                    }
                    if(errore)
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_selezione", "alert('Selezionare quali file verificare!');", true);
                }
                
            }
            if (!errore)
            {
                if (rbtn_numero_file.Checked)
                {
                    string files = "";
                    WSConservazioneLocale.InfoUtente infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);
                    WSConservazioneLocale.ItemsConservazione[] items = ConservazioneWA.Utils.ConservazioneManager.getItemsConservazione(this.hd_idIstanza.Value, infoUtente);
                    number= Int32.Parse(tb_num_file.Text);
                    Random rdm = new Random();
                    string[] presi = new string[number];
                    int i=0;
                    while(i<number)
                    {
                        int j = rdm.Next(totDocs);
                        if (!presi.Contains(items[j].DocNumber))
                        {
                            files += items[j].DocNumber + ",";
                            presi[i] = items[j].DocNumber;
                            i++;
                        }
                    }
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "verificaLeggibilita", "showVerificaLeggibilita('" + this.hd_idIstanza.Value + "','" + files + "',2);", true);
                
                    //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "verificaLeggibilita", "showVerificaLeggibilita('" + this.hd_idIstanza.Value + "','no'," + sel_numero_file.Items[sel_numero_file.SelectedIndex].Text + ");", true);
                }
                if (rbtn_percent_file.Checked)
                {
                    string files = "";
                    WSConservazioneLocale.InfoUtente infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);
                    WSConservazioneLocale.ItemsConservazione[] items = ConservazioneWA.Utils.ConservazioneManager.getItemsConservazione(this.hd_idIstanza.Value, infoUtente);
                    
                    int percent = Int32.Parse(tb_percent_file.Text);
                    double op = (totDocs * 100 / percent);
                    number = (int)Math.Ceiling((double)((double)(totDocs * percent) / 100));
                    Random rdm = new Random();
                    string[] presi = new string[number];
                    int i = 0;
                    while (i < number)
                    {
                        int j = rdm.Next(totDocs);
                        if (!presi.Contains(items[j].DocNumber))
                        {
                            files += items[j].DocNumber + ",";
                            presi[i] = items[j].DocNumber;
                            i++;
                        }
                    }
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "verificaLeggibilita", "showVerificaLeggibilita('" + this.hd_idIstanza.Value + "','" + files + "',2);", true);
                
                }
                if (rbtn_file_da_aprire.Checked)
                {
                    string files = "";
                    foreach (GridViewRow gvr in GridView1.Rows)
                    {
                        if (((CheckBox)gvr.FindControl("chk_doc")).Checked)
                        {
                            files += gvr.Cells[0].Text+",";
                        }
                    }
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "verificaLeggibilita", "showVerificaLeggibilita('" + this.hd_idIstanza.Value + "','" + files + "',2);", true);
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiusura", "window.close();", true);
                
            }
        }


    }
}