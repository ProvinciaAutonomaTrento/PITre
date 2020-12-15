using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.AdminTool.Manager
{
    public class StrutturaSottofascicoliManager
    {
        private readonly DocsPaWebService _ws = new DocsPaWebService();

        internal DataTable GetListStrutture(string idAmministrazione)
        {
            return _ws.GetStruttureSottofascicoli(idAmministrazione);
        }

        private TreeNode GetParent(TreeNodeCollection childnodes, string value)
        {
            for (var i = 0; i < childnodes.Count; i++)
            {
                if (childnodes[i].Value == value)
                    return childnodes[i];

                if (childnodes[i].ChildNodes.Count > 0)
                {
                    var parent = GetParent(childnodes[i].ChildNodes, value);
                    if (parent != null)
                        return parent;
                }
            }
            return null;
        }

        internal EsitoOperazione DeleteStrutture(string IDAmministrazione, int itemid)
        {
            return _ws.DeleteStrutturaSottofascicoli(itemid, int.MinValue, int.MinValue, IDAmministrazione);
        }

        internal DataTable GetTreeStrutture(string idAmministrazione, int idstruttura)
        {
            var data = _ws.GetStruttureSottofascicoliById(idAmministrazione, int.MinValue, int.MinValue, idstruttura);
            SetIndentationLevel(ref data);
            return data;
        }

        internal DataTable GetTreeStrutture()
        {
            var data = new DataTable();

            data.Columns.Add("SYSTEM_ID", typeof(int));
            data.Columns.Add("ID_PARENT", typeof(int));
            data.Columns.Add("NAME", typeof(string));
            data.Columns.Add("IndentationLevel", typeof(int));
            data.Columns.Add("Index", typeof(int));
            data.Columns.Add("Last", typeof(int));

            return data;
        }

        internal void SetIndentationLevel(ref DataTable data)
        {
            if (!data.Columns.Contains("IndentationLevel"))
                data.Columns.Add("IndentationLevel", typeof(int));

            if (!data.Columns.Contains("Index"))
                data.Columns.Add("Index", typeof(int));

            if (!data.Columns.Contains("Last"))
                data.Columns.Add("Last", typeof(int));

            // gets a collection of top-level rows
            var topLevelRows = from DataRow struttureRow
                in data.Rows
                where struttureRow.IsNull(1)
                orderby struttureRow[0]
                select struttureRow;

            // recursively builds a collection consisting of all the top level rows
            int index = 0;
            SetIndentationRecursive(topLevelRows, ref data, 0, ref index);
            MarkLastLeaf(ref data);
        }

        private void MarkLastLeaf(ref DataTable data)
        {
            DataRow[] rows = data.Select("", "IndentationLevel");

            for (int i = 0; i < rows.Length-1; i++)
            {
                int nextlevel = Convert.ToInt32(rows[i + 1]["IndentationLevel"]);
                int curlevel = Convert.ToInt32(rows[i]["IndentationLevel"]);

                int index = data.Rows.IndexOf(rows[i]);
                data.Rows[index]["Last"] = curlevel <= nextlevel ? 0 : 1;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="inputRows"></param>
        /// <param name="data"></param>
        /// <param name="indentationLevel"></param>
        private void SetIndentationRecursive(IEnumerable<DataRow> inputRows, ref DataTable data, int indentationLevel, ref int itemindex)
        {
            foreach (var inputRow in inputRows)
            {
                var index = data.Rows.IndexOf(inputRow);
                data.Rows[index]["IndentationLevel"] = indentationLevel;
                data.Rows[index]["Index"] = itemindex++;
                data.Rows[index]["Last"] = 0;

                var selectedRows = data.Select(string.Format("ID_PARENT={0}", data.Rows[index]["SYSTEM_ID"]));
                if (selectedRows.Any())
                    SetIndentationRecursive(selectedRows, ref data, indentationLevel + 1, ref itemindex);
            }
        }

        internal DataTable RemoveNode(DataTable data, int index)
        {
            var result = data.Select(string.Format("ID_PARENT = {0}", index));
            if (!result.Any())
                throw new Exception("Non è possibile cancellare un nodo con figli");

            data.Rows.RemoveAt(index);
            return data;
        }

        internal DataTable AddNode(DataTable data, int? parentid, string nodename, string idtemplate, InfoUtente info, Ruolo ruolo)
        {
            DataRow row = data.Rows.Add(data.Rows.Count + 1, parentid, nodename,  0, data.Rows.Count - 1, 0);
            if (parentid.HasValue)
                SetIndentationLevel(ref data);

            return data;
        }

        internal DataTable UpdateNode(DataTable data, int id, string value, string idtemplate)
        {
            var result = data.Select(string.Format("SYSTEM_ID = {0}", id));
            var index = data.Rows.IndexOf(result[0]);

            data.Rows[index][2] = value;

            return data;
        }

        internal EsitoOperazione SaveStruttura(int id, DataTable data, string name, InfoUtente info, Ruolo ruolo, string idamm)
        {
            if (id != int.MinValue)
                UpdateStrutturaInProject(id, data, name, info, ruolo, idamm);

            return _ws.SaveStrutturaSottofascicoli(id, data, name, idamm);
        }

        private void UpdateStrutturaInProject(int id, DataTable data, string name, InfoUtente info, Ruolo ruolo, string idamm)
        {
            DataTable struttura = GetTreeStrutture(idamm, id);

            while (struttura.Columns.Count > 3)
                struttura.Columns.RemoveAt(struttura.Columns.Count - 1);

            while (data.Columns.Count > 3)
                data.Columns.RemoveAt(data.Columns.Count - 1);

            var vadded = data.AsEnumerable().Except(struttura.AsEnumerable(), new MyDataRowComparer());
            var vremoved = struttura.AsEnumerable().Except(data.AsEnumerable(), new MyDataRowComparer());
            var vchanged = (from a in vadded join b in vremoved on a["SYSTEM_ID"].ToString() equals b["SYSTEM_ID"].ToString()
                            where a["NAME"].ToString() != b["NAME"].ToString()
                            select a);

            if (vchanged.Count() > 0)
                updateNodiProject(vchanged, vremoved, id, idamm);

            var vaddedcleaned = from a in vadded where !vchanged.Any(b => a[0].Equals(b[0])) select a;
            if (vaddedcleaned.Count() > 0)
                insertNodiProject(vaddedcleaned, data, id, info, ruolo, idamm);

            //var vremovedcleaned = from a in vremoved where !vchanged.Any(b => a[0].Equals(b[0])) select a;
            //if (vremovedcleaned.Count() > 0)
            //    deleteNodiProject(vremovedcleaned, id, info);
        }

        private void deleteNodiProject(IEnumerable<DataRow> vremovedcleaned, int id, InfoUtente info)
        {
            foreach (var item in vremovedcleaned)
            {
                _ws.RemoveStrutturaFromFascicolo(item["NAME"].ToString(), id.ToString(), info);
            }
        }

        private void insertNodiProject(IEnumerable<DataRow> vaddedcleaned, DataTable struttura, int id, InfoUtente info, Ruolo rule, string idamm)
        {
            foreach (var item in vaddedcleaned)
            {
                string parent_name = string.Empty;
                if(!string.IsNullOrEmpty(item["ID_PARENT"].ToString()))
                {
                    string query = string.Format("SYSTEM_ID = {0}", item["ID_PARENT"].ToString());
                    DataRow[] parents = struttura.Select(query);
                    if (parents != null)
                        parent_name = parents.FirstOrDefault()["NAME"].ToString();
                }

                _ws.AddStrutturaToFascicolo(parent_name, item["NAME"].ToString(), id.ToString(), info, rule, idamm);
            }
        }

        private void updateNodiProject(IEnumerable<DataRow> vchanged, IEnumerable<DataRow> vremoved, int id, string idamm)
        {
            foreach (var item in vchanged)
            {
                string oldname = (from r in vremoved
                                  where r["SYSTEM_ID"].ToString() == item["SYSTEM_ID"].ToString()
                                  select r["NAME"].ToString()).FirstOrDefault();

                _ws.RenameStrutturaToFascicolo(oldname, item["NAME"].ToString(), id.ToString(), idamm);
            }
        }

        internal bool IsNodoStrutturaInFascicoliEmpty(DataTable data, int index, string strutturaid, out int numfascicoli, string idamm)
        {
            var rows = data.Select(string.Format("INDEX = {0}", index)).FirstOrDefault<DataRow>();
            return _ws.IsNodoStrutturaInFascicoliEmpty(strutturaid, Convert.ToString(rows[2]), idamm, out numfascicoli);
        }

        internal DataTable RemoveNode(DataTable data, DataRow item, string idtemplate, InfoUtente info)
        {
            data.Rows.Remove(item);
            SetIndentationLevel(ref data);
            return data;
        }
    }

    internal class MyDataRowComparer : IEqualityComparer<DataRow>
    {
        public bool Equals(DataRow x, DataRow y)
        {
            int iNumCol = x.Table.Columns.Count > y.Table.Columns.Count ? y.Table.Columns.Count : x.Table.Columns.Count;

            for (int i = 0; i < iNumCol; i++)
            {
                if (x[i].ToString() != y[i].ToString())
                    return false;
            }
            return true;
        }

        public int GetHashCode(DataRow obj)
        {
            return obj.ToString().GetHashCode();
        }
    }

}