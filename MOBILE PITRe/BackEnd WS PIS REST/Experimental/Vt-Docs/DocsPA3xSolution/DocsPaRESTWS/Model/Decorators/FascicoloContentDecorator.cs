using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocsPaVO.fascicolazione;
using DocsPaVO.documento;
using DocsPaDB.Query_DocsPAWS;
using System.Collections;
using log4net;

namespace DocsPaRESTWS.Model.Decorators
{
    public delegate C ProcessFolderDelegate<C>(Folder fold);

    public delegate C ProcessInfoDocumentoDelegate<C>(InfoDocumento infoDoc);

    public class FascicoloContentDecorator<C> : ListDecorator<C>
    {
        private ILog logger=LogManager.GetLogger(typeof(FascicoloContentDecorator<C>));
        private string _idPeople;
        private string _idGruppo;
        private string _folderId;
        private string _fascId;
        private string _descCercata;
        private ProcessFolderDelegate<C> _folderDel;
        private ProcessInfoDocumentoDelegate<C> _infoDocDel;

        public FascicoloContentDecorator(string idPeople, string idGruppo, string folderId, string fascId, string descCercata, ProcessFolderDelegate<C> folderDel, ProcessInfoDocumentoDelegate<C> infoDocDel)
        {
            this._idPeople = idPeople;
            this._idGruppo = idGruppo;
            this._folderId = folderId;
            this._fascId = fascId;
            this._folderDel = folderDel;
            this._infoDocDel = infoDocDel;
            this._descCercata = descCercata;
        }

        public override List<C> execute()
        {
            logger.Info("begin");
            List<C> res = new List<C>();
            Fascicoli fasc = new Fascicoli();
            Folder folder = null;
            logger.Debug("folderId: " + _folderId + " fascId: " + _fascId);
            if (_folderId.Equals(_fascId))
            {
                folder = fasc.GetFolderByIdFascicolo(_idPeople, _idGruppo, _fascId);
            }
            else
            {
                folder = fasc.GetFolderByIdFascicoloAndIdFolder(_idPeople, _idGruppo, _fascId,_folderId);
            }
            //if (folder != null && folder.childs != null && folder.childs.Count > 0)
            //{
            //    logger.Debug("subfolder trovati: " + folder.childs.Count);
            //    foreach (Object temp in folder.childs)
            //    {
            //        Folder fold = (Folder)temp;
            //        res.Add(this._folderDel(fold));
            //    }
            //}
            //ArrayList list = BusinessLogic.Fascicoli.FolderManager.getDocumenti(_idGruppo, _idPeople, folder);
            //logger.Debug("documenti trovati: " + list.Count);
            //foreach (Object temp in list)
            //{
            //    InfoDocumento infoDoc = (InfoDocumento)temp;
            //    res.Add(this._infoDocDel(infoDoc));
            //}
            res.AddRange(getDocInFolder(_idGruppo, _idPeople, _fascId, folder.systemID, "", _descCercata));
            logger.Info("end");
            return res;
        }

        protected override List<C> executeList(List<C> input)
        {
            return null;
        }

        private List<C> getDocInFolder(string idGruppo, string idPeople, string fascId, string folderId, string path, string descCercata)
        {
            List<C> res = new List<C>();
            Fascicoli fasc = new Fascicoli();
            
            Folder folder = null;
            folder = fasc.GetFolderByIdFascicoloAndIdFolder(idPeople, idGruppo, fascId, folderId);
            ArrayList list = BusinessLogic.Fascicoli.FolderManager.getDocumenti(idGruppo, idGruppo, folder);
            foreach (Object temp in list)
            {
                InfoDocumento infoDoc = (InfoDocumento)temp;
                infoDoc.noteCestino = path + "\\" + folder.descrizione;
                if (!string.IsNullOrWhiteSpace(descCercata))
                {
                    if(infoDoc.oggetto.ToUpper().Contains(descCercata.ToUpper()))
                        res.Add(this._infoDocDel(infoDoc));
                }else
                res.Add(this._infoDocDel(infoDoc));
            }
            if (folder != null && folder.childs != null && folder.childs.Count > 0)
            {
                foreach (Object temp in folder.childs)
                {
                    Folder fold = (Folder)temp;
                    res.AddRange(getDocInFolder(idGruppo, idPeople, fascId, fold.systemID, path + "\\" + folder.descrizione, descCercata));
                }
            }
            return res;
        }
    }
}