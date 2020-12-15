using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Data;
using System.Web;
namespace NttDatalLibrary
{
    public class CustomColumnIcon : ITemplate
    {
        private DataControlRowType templateType;
        private string columnName;

        private DataTable GrigliaResult
        {
            get
            {
                return (DataTable)HttpContext.Current.Session["GrigliaResult"];

            }
            set
            {
                HttpContext.Current.Session["GrigliaResult"] = value;
            }
        }

        public CustomColumnIcon(DataControlRowType type, string colname)
        {
            templateType = type;
            columnName = colname;
        }

        public void InstantiateIn(System.Web.UI.Control container)
        {
            
            switch (templateType)
            {
                case DataControlRowType.DataRow:
                    CustomImageButton ib = new CustomImageButton();
                    //dettaglio del documento
                    ib.CssClass = "clickableLeft";
                    ib.ImageUrl = "../Images/Icons/view_doc_grid.png";
                    ib.OnMouseOutImage = "../Images/Icons/view_doc_grid.png";
                    ib.OnMouseOverImage = "../Images/Icons/view_doc_grid_hover.png";
                    ib.ImageUrlDisabled = "../Images/Icons/view_doc_grid_disabled.png";
                    ib.ID = "visualizzadocumento";
                    ib.EnableViewState = true;
                    container.Controls.Add(ib);

                    //estensione del File
                    ib = new CustomImageButton();
                    ib.CssClass = "clickableLeft";
                    ib.ID = "estensionedoc";
                    ib.EnableViewState = true;
                    container.Controls.Add(ib);

                    //file firmato
                    ib = new CustomImageButton();
                    ib.CssClass = "clickableLeft";
                    ib.ImageUrl = "../Images/Icons/icon_p7m.png";
                    ib.OnMouseOutImage = "../Images/Icons/icon_p7m.png";
                    ib.OnMouseOverImage = "../Images/Icons/icon_p7m_hover.png";
                    ib.ID = "firmato";
                    ib.EnableViewState = true;
                    container.Controls.Add(ib);

                    ib = new CustomImageButton();
                    ib.CssClass = "clickableLeft";
                    ib.ImageUrl = "../Images/Icons/response_protocol.png";
                    ib.OnMouseOutImage = "../Images/Icons/response_protocol.png";
                    ib.OnMouseOverImage = "../Images/Icons/response_protocol_hover.png";
                    ib.ImageUrlDisabled = "../Images/Icons/response_protocol_disabled.png";
                    ib.ID = "newDocumentAnswer";
                    ib.EnableViewState = true;
                    ib.Visible = false;
                    container.Controls.Add(ib);

                    //ADL
                    ib = new CustomImageButton();
                    ib.ID = "adl";
                    ib.CssClass = "clickableLeft";
                    ib.EnableViewState = true;
                    container.Controls.Add(ib);

                    ib = new CustomImageButton();
                    ib.ID = "adlrole";
                    ib.CssClass = "clickableLeft";
                    ib.EnableViewState = true;
                    container.Controls.Add(ib);

                    // conservazione
                    ib = new CustomImageButton();
                    ib.ID = "conservazione";
                    ib.CssClass = "clickableLeft";
                    ib.EnableViewState = true;
                    container.Controls.Add(ib);

                    //Stato processi avviati
                    ib = new CustomImageButton();
                    ib.CssClass = "clickableLeft";
                    ib.ImageUrl = "../Images/Icons/LibroFirma/Process_State_2.png";
                    ib.OnMouseOutImage = "../Images/Icons/LibroFirma/Process_State_2.png";
                    ib.OnMouseOverImage = "../Images/Icons/LibroFirma/Process_State_hover_2.png";
                    ib.ImageUrlDisabled = "../Images/Icons/LibroFirma/Process_State_disabled_2.png";
                    ib.ID = "visualizzaStatoProcessiFirmaAvviati";
                    ib.EnableViewState = true;
                    ib.Visible = false;
                    container.Controls.Add(ib);

                    //Info processi avviati
                    ib = new CustomImageButton();
                    ib.CssClass = "clickableLeft";
                    ib.ImageUrl = "../Images/Icons/LibroFirma/InfoProcessiAvviati2.png";
                    ib.OnMouseOutImage = "../Images/Icons/LibroFirma/InfoProcessiAvviati2.png";
                    ib.OnMouseOverImage = "../Images/Icons/LibroFirma/InfoProcessiAvviati2_hover.png";
                    ib.ImageUrlDisabled = "../Images/Icons/LibroFirma/InfoProcessiAvviati2_disabled.png";
                    ib.ID = "visualizzaProcessiFirmaAvviati";
                    ib.EnableViewState = true;
                    ib.Visible = false;
                    container.Controls.Add(ib);

                    //elimina file
                    ib = new CustomImageButton();
                    ib.CssClass = "clickableLeft";
                    ib.ID = "eliminadocumento";
                    ib.ImageUrl = "../Images/Icons/delete2.png";
                    ib.OnMouseOutImage = "../Images/Icons/delete2.png";
                    ib.OnMouseOverImage = "../Images/Icons/delete2_hover.png";
                    ib.ImageUrlDisabled = "../Images/Icons/delete2_disabled.png";
                    ib.EnableViewState = true;
                    container.Controls.Add(ib);

                    ib.Dispose();
                break;
            }
        }

    }
}
