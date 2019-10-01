using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NttDatalLibrary
{
    public class CustomColumnCheckBox : ITemplate
    {
        private DataControlRowType templateType;
        private string columnName;

        public CustomColumnCheckBox(DataControlRowType type, string colname)
        {
            templateType = type;
            columnName = colname;
        }

        public void InstantiateIn(System.Web.UI.Control container)
        {
            //Literal literal = new Literal();
            CheckBox checkbox = new CheckBox();
            checkbox.EnableViewState = true;
            // Create the content for the different row types.
            switch (templateType)
            {
               
                case DataControlRowType.Header:
                    //literal.Text = "<input id=\"cb_selectall\" type=\"checkbox\" name=\"cb_selectall\">";
                    checkbox.ID = "cb_selectall";
                    checkbox.Attributes["onclick"] = "cb_selectall();";
                    container.Controls.Add(checkbox);
                    break;
                case DataControlRowType.DataRow:
                    //literal.Text = "<input id=\"checkDocumento\" type=\"checkbox\" name=\"checkDocumento\">";
                    checkbox.ID = "checkDocumento";
                    container.Controls.Add(checkbox);
                    break;
            }
        }

    }
}
