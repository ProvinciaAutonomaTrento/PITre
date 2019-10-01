using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
// using static DocsPAWA.dataSet.DataSetLFascContDoc;

namespace DocsPAWA.dataSet
{
    /// <summary>
    ///Represents the strongly named DataTable class.
    ///</summary>
    public partial class DataSetLFascContDoc : global::System.Data.DataSet
    {

        public partial class element1DataTable : global::System.Data.TypedTableBase<element1Row>
        {

            [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
            public element1Row Addelement1RowInClassifica(string codice, string descrizione, string vociClassificazione, int chiave, string descrizioneFolders, string stato, string codRegistro, string idRegistro, string accessRights, string idTitolario, string systemId, string fascPrimaria)
            {
                element1Row rowelement1Row = ((element1Row)(this.NewRow()));
                rowelement1Row.ItemArray = new object[] {
                                                            codice,
                                                            descrizione,
                                                            vociClassificazione,
                                                            chiave,
                                                            descrizioneFolders,
                                                            stato,
                                                            codRegistro, idRegistro, accessRights,idTitolario, systemId, fascPrimaria};
                this.Rows.Add(rowelement1Row);
                return rowelement1Row;
            }
        }
    }
}