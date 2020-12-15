using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DocsPaDB.Query_DocsPAWS
{
    public class StruttureSottoFascicoli : DBProvider
    {
        private static ILog _log = LogManager.GetLogger(typeof(StruttureSottoFascicoli));

        public DataTable getStruttureSottofascicoli(string idamm)
        {
            _log.Debug("START : DocsPaDB > Query_DocsPAWS > StruttureSottoFascicoli> getStruttureSottofascicoli");
            DataSet dataset = new DataSet();
            
            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_STRUTTURE_SOTTOFASCICOLI");
                queryMng.setParam("idAmm", idamm);
                String commandText = queryMng.getSQL();
                _log.Debug(commandText);
                new DocsPaDB.DBProvider().ExecuteQuery(dataset, commandText);
                _log.Debug("END : DocsPaDB > Query_DocsPAWS > StruttureSottoFascicoli> getStruttureSottofascicoli");
            }
            catch (Exception e)
            {
                _log.Debug("SQL - getStruttureSottofascicoli - ERRORE : " + e.Message);
            }

            if (dataset.Tables.Count > 0)
                return dataset.Tables[0];
            else
                return null;
        }

        public DataTable getStruttureSottofascicoli(string idamm, int idfascicolo, int idtitolario, int idtemplate)
        {
            ArrayList sp_params = new ArrayList();
            DataSet data = new DataSet();

            DocsPaUtils.Data.ParameterSP output = new DocsPaUtils.Data.ParameterSP("ID_OBJECT", 0, DocsPaUtils.Data.DirectionParameter.ParamOutput);

            sp_params.Add(new DocsPaUtils.Data.ParameterSP("ID_FASCICOLO", (idfascicolo == int.MinValue ? "" : idfascicolo.ToString())));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("ID_TITOLARIO", (idtitolario == int.MinValue ? "" : idtitolario.ToString())));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("ID_TEMPLATE", (idtemplate == int.MinValue ? "" : idtemplate.ToString())));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("ID_AMM", idamm.ToString()));

            sp_params.Add(output);

            ExecuteStoredProcedure("SP_GET_PROJECT_STRUCTURE", sp_params, data);

            if (data.Tables.Count > 0)
                return data.Tables[0];
            else
                return null;
        }

        public void saveStrutturaSottofascicoli(int id, DataTable data, string name, string idAmm)
        {
            using (DBProvider provider = new DBProvider())
            {
                provider.BeginTransaction();
                bool Isnew = id == int.MinValue;

                try
                {
                    // Salvo nella template
                    if (Isnew)
                    {
                        insertTemplateStruttura(ref id, name, idAmm);
                        insertStrutturaFascicoli(id, data);
                    }   
                    else
                    {
                        updateTemplateStruttura(id, name);
                        updateStrutturaFascicoli(id, data, idAmm);
                    }

                    provider.CommitTransaction();
                }
                catch (Exception ex)
                {
                    provider.RollbackTransaction();
                    _log.Error(ex);
                }
            }
        }

        private void updateStrutturaFascicoli(int id, DataTable data, string idAmm)
        {
            DataTable db = getStruttureSottofascicoli(idAmm, int.MinValue, int.MinValue, id);

            var vadded = data.AsEnumerable().Except(db.AsEnumerable(), new MyDataRowComparer());
            var vremoved = db.AsEnumerable().Except(data.AsEnumerable(), new MyDataRowComparer());
            //var vremoved = data.AsEnumerable().Except(db.AsEnumerable(), new MyDataRowComparer());
            //var vadded = db.AsEnumerable().Except(data.AsEnumerable(), new MyDataRowComparer());

            var vchanged = (from a in vadded join b in vremoved on a[0] equals b[0] select a);
            if(vchanged.Count() > 0)
                updateNodiStruttura(vchanged, id);

            var vaddedcleaned = from a in vadded where !vchanged.Any(b => a[0].Equals(b[0])) select a;
            if(vaddedcleaned.Count() > 0)
                insertNodiStruttura(vaddedcleaned, id);

            var vremovedcleaned = from a in vremoved where !vchanged.Any(b => a[0].Equals(b[0])) select a;
            if(vremovedcleaned.Count() > 0)
                deleteNodiStruttura(vremovedcleaned, id);
        }

        private void deleteNodiStruttura(IEnumerable<DataRow> rows, int id)
        {
 	        foreach (DataRow row in rows)
            {
                _log.Debug("START : DocsPaDB > Query_DocsPAWS > StruttureSottoFascicoli> deleteNodiStruttura");

                try
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_NODI_STRUTTURA");
                    q.setParam("SystemId", row[0].ToString());

                    string command = q.getSQL();
                    _log.Debug(command);
                
                    new DocsPaDB.DBProvider().ExecuteNonQuery(command);
                    _log.Debug("END : DocsPaDB > Query_DocsPAWS > StruttureSottoFascicoli> deleteNodiStruttura");
                }
                catch (Exception e)
                {
                    _log.Debug("SQL - deleteNodiStruttura - ERRORE : " + e.Message);
                }
            }
        }

        private void insertNodiStruttura(IEnumerable<DataRow> rows, int id)
        {
            DocsPaUtils.Data.ParameterSP output = new DocsPaUtils.Data.ParameterSP("ID", 0, DocsPaUtils.Data.DirectionParameter.ParamOutput);
 	        for (int i = 0; i < rows.Count(); i++)
			{
                int nodeid = int.MinValue, 
                    prevnodeid = Convert.ToInt32(rows.ElementAt(i)["SYSTEM_ID"]);

                ExecuteStoreProcedure("SP_INSERT_STRUCTURE_NODE", new ArrayList()
                {
                    new DocsPaUtils.Data.ParameterSP("NAME", rows.ElementAt(i)["NAME"]),
                    new DocsPaUtils.Data.ParameterSP("PARENT_ID", rows.ElementAt(i)["ID_PARENT"]),
                    new DocsPaUtils.Data.ParameterSP("TEMPLATE_ID", id.ToString()),
                    output
                });

                if (output.Valore == null)
                    throw new Exception("Errore nella SP_INSERT_STRUCTURE_NODE");

                nodeid = Convert.ToInt32(output.Valore);
                (from r in rows where !r.IsNull("ID_PARENT") && Convert.ToInt32(r["ID_PARENT"]) == prevnodeid select r)
                    .ToList().ForEach(x => x["ID_PARENT"] = nodeid);
            }
        }

        private void updateNodiStruttura(IEnumerable<DataRow> rows, int id)
        {
 	        foreach (DataRow row in rows)
            {
                _log.Debug("START : DocsPaDB > Query_DocsPAWS > StruttureSottoFascicoli> updateNodiStruttura");

                try
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_NODI_STRUTTURA");
                    q.setParam("IdParent", string.IsNullOrEmpty(row[1].ToString()) ? "NULL" : row[1].ToString());
                    q.setParam("Name", row[2].ToString());
                    q.setParam("IdTemplate", id.ToString());
                    q.setParam("SystemId", row[0].ToString());

                    string command = q.getSQL();
                    _log.Debug(command);
                
                    new DocsPaDB.DBProvider().ExecuteNonQuery(command);
                    _log.Debug("END : DocsPaDB > Query_DocsPAWS > StruttureSottoFascicoli> updateNodiStruttura");
                }
                catch (Exception e)
                {
                    _log.Debug("SQL - updateNodiStruttura - ERRORE : " + e.Message);
                }
            }
        }

        private DataTable GetRowsAdded(DataTable db, DataTable data)
        {
            var rowsOnlyInDt1 = data.AsEnumerable().Where(r => !db.AsEnumerable()
                                .Any(r2 => r[0] == r2[0] && r[1] == r2[1] && r[2] == r2[2]));

            return rowsOnlyInDt1.CopyToDataTable();
        }

        private void insertStrutturaFascicoli(int idtemplate, DataTable data)
        {
            // gets a collection of top-level rows
            DataRow[] topLevelRows = (from DataRow struttureRow
                                      in data.Rows
                                      where struttureRow.IsNull(1)
                                      select struttureRow).ToArray();

            // recursively builds a collection consisting of all the top level rows
            insertStruttureRecursive(topLevelRows, ref data, idtemplate);
        }

        private void insertStruttureRecursive(DataRow[] rows, ref DataTable data, int idtemplate)
        {
            for (int i = 0; i < rows.Count(); i++)
            {
                int id = insertStrutturaNode(rows[i], idtemplate);

                DataRow[] selectedRows = data.Select(string.Format("ID_PARENT={0}", rows[i]["SYSTEM_ID"]));
                if (selectedRows.Count() > 0)
                {
                    for (int j = 0; j < selectedRows.Count(); j++)
                        selectedRows[j]["ID_PARENT"] = id;

                    insertStruttureRecursive(selectedRows, ref data, idtemplate);
                }
            }
        }

        private int insertStrutturaNode(DataRow row, int idtemplate)
        {
            DocsPaUtils.Data.ParameterSP output = new DocsPaUtils.Data.ParameterSP("ID", 0, DocsPaUtils.Data.DirectionParameter.ParamOutput);
            ExecuteStoreProcedure("SP_INSERT_STRUCTURE_NODE", new ArrayList()
            {
                new DocsPaUtils.Data.ParameterSP("NAME", row["NAME"]),
                new DocsPaUtils.Data.ParameterSP("PARENT_ID", row["ID_PARENT"]),
                new DocsPaUtils.Data.ParameterSP("TEMPLATE_ID", idtemplate.ToString()),
                output
            });

            return output.Valore == null ? -1 : Convert.ToInt32(output.Valore);
        }

        private void updateTemplateStruttura(int id, string name)
        {
            _log.Debug("START : DocsPaDB > Query_DocsPAWS > StruttureSottoFascicoli> updateTemplateStruttura");

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_STRUTTURE_TEMPLATE");
                q.setParam("ID", id.ToString());
                q.setParam("NAME", name);

                string command = q.getSQL();
                _log.Debug(command);
                
                new DocsPaDB.DBProvider().ExecuteNonQuery(command);
                _log.Debug("END : DocsPaDB > Query_DocsPAWS > StruttureSottoFascicoli> updateTemplateStruttura");
            }
            catch (Exception e)
            {
                _log.Debug("SQL - updateTemplateStruttura - ERRORE : " + e.Message);
            }
        }

        private void insertTemplateStruttura(ref int id, string name, string idAmm)
        {
            DocsPaUtils.Data.ParameterSP outParam = new DocsPaUtils.Data.ParameterSP("ID", 0, DocsPaUtils.Data.DirectionParameter.ParamOutput);
            int result = ExecuteStoreProcedure("SP_INSERT_PROJECT_TEMPLATE", new ArrayList()
            {
                new DocsPaUtils.Data.ParameterSP("NAME", name),
                new DocsPaUtils.Data.ParameterSP("ID_AMM", idAmm),
                outParam
            });

            if (outParam.Valore != null)
                id = Convert.ToInt32(outParam.Valore);
        }

        public void deleteStrutturaSottofascicoli(int id, string idAmm)
        {
            _log.Debug("START : DocsPaDB > Query_DocsPAWS > StruttureSottoFascicoli> deleteStrutturaSottofascicoli");

            try
            {
                // Delete PROJECT_STRUCTURE
                DataTable nodi = getStruttureSottofascicoli(idAmm, int.MinValue, int.MinValue, id);
                deleteNodiStruttura(nodi.AsEnumerable(), id);

                // Delete PROJECT_TEMPLATE
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_STRUTTURE_TEMPLATE");
                q.setParam("SystemId", id.ToString());

                string command = q.getSQL();
                _log.Debug(command);
                new DBProvider().ExecuteNonQuery(command);

                _log.Debug("END : DocsPaDB > Query_DocsPAWS > StruttureSottoFascicoli> deleteStrutturaSottofascicoli");
            }
            catch (Exception e)
            {
                _log.Debug("SQL - deleteStrutturaSottofascicoli - ERRORE : " + e.Message);
            }
        }

        public DataTable getTemplateStrutturaRelation(int idtemplate, int idtipofascicolo, int idtitolario)
        {
            _log.Debug("START : DocsPaDB > Query_DocsPAWS > StruttureSottoFascicoli> getTemplateStrutturaRelation");
            DataSet dataset = new DataSet();

            try
            {
                ExecuteStoredProcedure("SP_GET_REL_PROJECT_TEMPLATE", new ArrayList()
                {
                    new DocsPaUtils.Data.ParameterSP("ID_TEMPLATE", idtemplate == int.MinValue ? "" : idtemplate.ToString()),
                    new DocsPaUtils.Data.ParameterSP("ID_FASCICOLO", idtipofascicolo == int.MinValue ? "" : idtipofascicolo.ToString()),
                    new DocsPaUtils.Data.ParameterSP("ID_TITOLARIO", idtitolario == int.MinValue ? "" : idtitolario.ToString())
                }, dataset);

                _log.Debug("END : DocsPaDB > Query_DocsPAWS > StruttureSottoFascicoli> getTemplateStrutturaRelation");
            }
            catch (Exception e)
            {
                _log.Error("SQL - getTemplateStrutturaRelation - ERRORE : " + e.Message);
            }

            return dataset.Tables[0];
        }

        public bool saveStrutturaSottofascicoli(int idtemplate, int idtipofascicolo, int idtitolario)
        {
            _log.Debug("START : DocsPaDB > Query_DocsPAWS > StruttureSottoFascicoli> saveStrutturaSottofascicoli");
            bool ret = false;
            DataSet data = new DataSet();
            ArrayList parameters = new ArrayList()
            {
                new DocsPaUtils.Data.ParameterSP("ID_TEMPLATE", idtemplate == int.MinValue ? "" : idtemplate.ToString()),
                new DocsPaUtils.Data.ParameterSP("ID_FASCICOLO", idtipofascicolo == int.MinValue ? "" : idtipofascicolo.ToString()),
                new DocsPaUtils.Data.ParameterSP("ID_TITOLARIO", idtitolario == int.MinValue ? "" : idtitolario.ToString())
            };

            try
            {
                ExecuteStoredProcedure("SP_GET_REL_PROJECT_TEMPLATE", parameters, data);

                if(data.Tables[0].Rows.Count > 0)
                    ExecuteStoreProcedure("SP_UPD_REL_PROJECT_TEMPLATE", parameters);
                else
                    ExecuteStoreProcedure("SP_INS_REL_PROJECT_TEMPLATE", parameters);

                ret = true;
                _log.Debug("END : DocsPaDB > Query_DocsPAWS > StruttureSottoFascicoli> saveStrutturaSottofascicoli");
            }
            catch (Exception e)
            {
                ret = false;
                _log.Error("SQL - saveStrutturaSottofascicoli - ERRORE : " + e.Message);
            }

            return ret;
        }

        public bool IsNodoStrutturaInFascicoliEmpty(string strutturaid, string nodename, out int numfascicoli, string idamm)
        {
            bool result = false;
            try
            {
                // SELECT
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_NODO_IN_FASCICOLI");
                q.setParam("ID_TEMPLATE", strutturaid);
                q.setParam("NODE_NAME", nodename);
                q.setParam("PARAM1", string.Format("AND ID_AMM = {0}", idamm));

                string count = string.Empty;
                string command = q.getSQL();
                _log.Debug(command);

                result = new DBProvider().ExecuteScalar(out count, command);
                numfascicoli = Convert.ToInt32(count);
            }
            catch (Exception e)
            {
                _log.Debug("SQL - IsNodoStrutturaInFascicoliEmpty - ERRORE : " + e.Message);
                numfascicoli = 0;
            }

            return result;
        }

        public DocsPaVO.fascicolazione.Folder[] GetFoldersByStrutturaTemplate(string name, string idTemplate, string idamm)
        {
            DocsPaVO.fascicolazione.Folder[] folders = new DocsPaVO.fascicolazione.Folder[0];

            try
            {
                DataSet data = new DataSet();
                DocsPaUtils.Query query = null;

                if(string.IsNullOrEmpty(name))
                    query = DocsPaUtils.InitQuery.getInstance().getQuery("S_ROOTFOLDER_IN_FASCICOLI");
                else
                {
                    query = DocsPaUtils.InitQuery.getInstance().getQuery("S_FOLDERTEMPLATE_IN_FASCICOLI");
                    query.setParam("NAME", name);
                }

                query.setParam("PARAM1", string.IsNullOrEmpty(idamm) ?
                    string.Empty : string.Format(" AND ID_AMM = {0}", idamm));

                query.setParam("ID_TEMPLATE", idTemplate);
                String sql = query.getSQL();
                _log.Debug(sql);
                new DocsPaDB.DBProvider().ExecuteQuery(data, sql);

                if (data.Tables.Count <= 0)
                    return folders;

                folders = new DocsPaVO.fascicolazione.Folder[data.Tables[0].Rows.Count];
                for (int i = 0; i < data.Tables[0].Rows.Count; i++)
                {
                    folders[i] = new DocsPaVO.fascicolazione.Folder();
                    DataRow row = data.Tables[0].Rows[i];

                    folders[i].systemID = row["ID_FOLDER"].ToString();
                    folders[i].idParent = row["ID_PARENT_FOLDER"].ToString();
                    folders[i].idFascicolo = row["ID_FASCICOLO"].ToString();
                    folders[i].descrizione = row["FOLDER_DESCRIPTION"].ToString();
                    folders[i].dtaApertura = row["APERTURA"].ToString().Trim();
                }
            }
            catch (Exception ex)
            {
                _log.Debug("GetFoldersByStrutturaTemplate - Errore", ex);
            }

            return folders;
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
