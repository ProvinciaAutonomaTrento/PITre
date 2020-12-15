using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace SAAdminTool.UserControls.ScrollElementsList
{
    public partial class ScrollElementsList : System.Web.UI.UserControl
    {
        protected ObjScrollElementsList objScrollElementsList = null;

        protected void Page_Load(object sender, EventArgs e){}

        protected void Page_PreRender(object sender, EventArgs e)
        {
            #region Codice che gestisce lo scroll di elementi in sessione
            //if (Session["ObjScrollElementsList"] != null && ScrollManager.enableScrollElementsList())
            //{
            //    objScrollElementsList = (ObjScrollElementsList)Session["ObjScrollElementsList"];
            //    setLabelAndButton(objScrollElementsList);
            //}
            #endregion

            #region Codice che gestisce lo scroll di elementi nel CONTEXT (tasto back)
            if (SiteNavigation.CallContextStack.CallerContext != null && SiteNavigation.CallContextStack.CallerContext.objScrollElementsList != null && ScrollManager.enableScrollElementsList())
            {
                objScrollElementsList = ScrollManager.getFromContextObjScrollElementsList();
                setLabelAndButton(objScrollElementsList);
            }
            else
            {
                this.Visible = false;
            }
            #endregion
        }

        private void setLabelAndButton(ObjScrollElementsList objScrollElementsList)
        {
            //Calcolo l'elemento selezionato
            int selectedElement = (objScrollElementsList.selectedElement + 1) + (objScrollElementsList.pageSize * objScrollElementsList.selectedPage);

            //Imposto la label dello UserControl
            switch(objScrollElementsList.searchContext)
            {
                case ObjScrollElementsList.EmunSearchContext.RICERCA_DOCUMENTI:
                    lbl_ScrollElementsList.Text = "Doc. " + selectedElement + "/" + objScrollElementsList.totalNumberOfElements + " - Pag. " + (objScrollElementsList.selectedPage + 1);
                    break;

                case ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_DOC_TO_DO_LIST: 
                case ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_DOC :
                    lbl_ScrollElementsList.Text = "Trasm. Doc. " + selectedElement + "/" + objScrollElementsList.totalNumberOfElements + " - Pag. " + (objScrollElementsList.selectedPage + 1);
                    break;

                case ObjScrollElementsList.EmunSearchContext.RICERCA_DOC_IN_FASC:
                    lbl_ScrollElementsList.Text = "Doc. in Fasc. " + selectedElement + "/" + objScrollElementsList.totalNumberOfElements + " - Pag. " + (objScrollElementsList.selectedPage + 1);
                    break;
                case ObjScrollElementsList.EmunSearchContext.RICERCA_FASCICOLI:
                    lbl_ScrollElementsList.Text = "Fasc. " + selectedElement + "/" + objScrollElementsList.totalNumberOfElements + " - Pag. " + (objScrollElementsList.selectedPage + 1);
                    break;
                case ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_FASC_TO_DO_LIST:
                case ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_FASC:
                    lbl_ScrollElementsList.Text = "Trasm. Fasc. " + selectedElement + "/" + objScrollElementsList.totalNumberOfElements + " - Pag. " + (objScrollElementsList.selectedPage + 1);
                    break;
            }

            //Il risultato della ricerca è costituito da un solo elemento, entrambi i bottoni disabilitati
            if (objScrollElementsList.totalNumberOfElements == 1)
            {
                btn_ScrollPrev.Enabled = false;
                btn_ScrollNext.Enabled = false;
                return;
            }

            //Selezionato il primo elemento
            if (objScrollElementsList.selectedElement == 0 && objScrollElementsList.selectedPage == 0)
            {
                btn_ScrollPrev.Enabled = false;
                btn_ScrollNext.Enabled = true;
                return;
            }

            //Selezionato l'ultimo elemento
            if (selectedElement == objScrollElementsList.totalNumberOfElements)
            {
                btn_ScrollNext.Enabled = false;
                btn_ScrollPrev.Enabled = true;
                return;
            }

            //Selezionato un elemento non iniziale o finale
            btn_ScrollPrev.Enabled = true;
            btn_ScrollNext.Enabled = true;

            //aggiunto per risolvere il problema in cui se da un documento di una ricerca si entra nel dettaglio del suo allegato,
            // premendo il tasto back si ritorna al documento ma le freccette riportano doc 0 di n permettendo il click sul tasto indietro
            if (selectedElement == 0)
                btn_ScrollPrev.Enabled = false;

            return;            
        }
              
        protected void btn_Scroll_Click(object sender, ImageClickEventArgs e)
        {
            objScrollElementsList = ScrollManager.getFromContextObjScrollElementsList();

            if (Session["dictionaryCorrispondente"] != null)
                Session.Remove("dictionaryCorrispondente");

            if (objScrollElementsList != null)
            {
                //Controllo che tipo di scorrimento va fatto "NEXT" o "PREV"
                ImageButton imgBtn = (ImageButton)sender;
                ScrollManager.ScrollDirection scrollDirection = ScrollManager.ScrollDirection.NO_DIRECTION;
                if (imgBtn.ID == "btn_ScrollNext")
                    scrollDirection = ScrollManager.ScrollDirection.NEXT;    
                if (imgBtn.ID == "btn_ScrollPrev")
                    scrollDirection = ScrollManager.ScrollDirection.PREV;

                string script = string.Empty;
                ScrollManager scrollManager;
                
            /*    //Work-Around per capire se il next o prev cambia categoria

                string IdDoc = "";
                string IdFasc = "";
                string whatisit = "";
                ArrayList list = new ArrayList();

                switch (scrollDirection)
                {
                    case ScrollManager.ScrollDirection.NEXT:
                        
                        list = objScrollElementsList.objList;
                        if (objScrollElementsList.selectedElement.Equals(7))
                        {
                            IdDoc =
                                ((SAAdminTool.DocsPaWR.infoToDoList) (list[0])).
                                    sysIdDoc;
                            IdFasc = ((SAAdminTool.DocsPaWR.infoToDoList)(list[0])).sysIdFasc;
                        }
                        else
                        {
                            IdDoc =
                                ((SAAdminTool.DocsPaWR.infoToDoList)(list[objScrollElementsList.selectedElement + 1])).
                                    sysIdDoc;
                            IdFasc = ((SAAdminTool.DocsPaWR.infoToDoList)(list[objScrollElementsList.selectedElement +1])).sysIdFasc;
                        }
                        
                        if (string.IsNullOrEmpty(IdDoc))
                        {
                            whatisit = "F";
                            objScrollElementsList.searchContext =
                                ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_FASC_TO_DO_LIST;
                        }
                        else if(string.IsNullOrEmpty(IdFasc))
                        {
                            whatisit = "D";
                            objScrollElementsList.searchContext =
                                ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_DOC_TO_DO_LIST;
                        }
                        break;

                    case ScrollManager.ScrollDirection.PREV:

                        list = objScrollElementsList.objList;
                        if (objScrollElementsList.selectedElement.Equals(0))
                        {

                            IdDoc = ((SAAdminTool.DocsPaWR.infoToDoList) (list[7])).sysIdDoc;
                            IdFasc = ((SAAdminTool.DocsPaWR.infoToDoList) (list[7])).sysIdFasc;
                        }
                        else
                        {
                            IdDoc = ((SAAdminTool.DocsPaWR.infoToDoList)(list[objScrollElementsList.selectedElement-1])).sysIdDoc;
                            IdFasc = ((SAAdminTool.DocsPaWR.infoToDoList)(list[objScrollElementsList.selectedElement-1])).sysIdFasc;
                        }
                        if (string.IsNullOrEmpty(IdDoc))
                        {
                            whatisit = "F";
                            objScrollElementsList.searchContext =
                                ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_FASC_TO_DO_LIST;
                        }
                        else if(string.IsNullOrEmpty(IdFasc))
                        {
                            whatisit = "D";
                            objScrollElementsList.searchContext =
                                ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_DOC_TO_DO_LIST;
                        }
                        break;

                }
             */

                //Verifico che tipo di lista devo scorrere
                switch (objScrollElementsList.searchContext)
                {
                    case ObjScrollElementsList.EmunSearchContext.RICERCA_DOCUMENTI:
                    case ObjScrollElementsList.EmunSearchContext.RICERCA_DOC_IN_FASC:
                        scrollManager = new ScrollManagerDoc();
                        script = scrollManager.move(objScrollElementsList, this.Page, scrollDirection);
                        break;

                    case ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_DOC_TO_DO_LIST:
                        scrollManager = new ScrollManagerTrasm();
                        script = scrollManager.move(objScrollElementsList, this.Page, scrollDirection);
                        break;
                    case ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_DOC:
                    case ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_FASC_TO_DO_LIST:
                        scrollManager = new ScrollManagerTrasm();
                        script = scrollManager.move(objScrollElementsList, this.Page, scrollDirection);
                        break;
                    case ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_FASC:
                        scrollManager = new ScrollManagerTrasm();
                        script = scrollManager.move(objScrollElementsList, this.Page, scrollDirection);
                        break;

                    case ObjScrollElementsList.EmunSearchContext.RICERCA_FASCICOLI:
                        scrollManager = new ScrollManagerFasc();
                        script = scrollManager.move(objScrollElementsList, this.Page, scrollDirection);
                        break;
                }

                //Registro lo script per chiamare la pagina del nuovo elemento
                if(!string.IsNullOrEmpty(script))
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "pageDocument", script, true);
                
            }
        }
    }
}
