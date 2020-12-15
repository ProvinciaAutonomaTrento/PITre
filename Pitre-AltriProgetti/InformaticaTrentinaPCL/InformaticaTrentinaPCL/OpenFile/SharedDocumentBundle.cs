using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Utils;

namespace InformaticaTrentinaPCL.OpenFile
{
    public class SharedDocumentBundle : AbstractDocumentListItem
    {
        public List<DocInfo> documents;
        public string docId;

        public SharedDocumentBundle()
        {
        }
        
        public SharedDocumentBundle(List<DocInfo> documents, string docId)
        {
            this.documents = documents;
            this.docId = docId;
        }

        #region #### AbstractDocumentListItem

        public override string GetData()
        {
            return documents[0]?.GetData();
        }

        public override string GetMittente()
        {
            return documents[0]?.mittente;
        }

        public override string GetOggetto()
        {
            return documents[0]?.oggetto;
        }

        public override string GetInfo()
        {
            return documents[0]?.GetInfo();
        }

        public override string GetEstensione()
        {
            return documents[0]?.GetEstensione();
        }

        public override string GetIdTrasmissione()
        {
            return documents[0]?.GetIdTrasmissione();
        }

        public override string getIdEvento()
        {
            return documents[0]?.getIdEvento();
        }

        public override string GetIdDocumento()
        {
            return docId;
        }

        public override string GetRagione()
        {
            return documents[0]?.GetRagione();
        }

        public override SignatureInfo getSignatureInfo()
        {
            return documents[0]?.getSignatureInfo();
        }

        public override void SetFlags()
        {
            return;
        }

        #endregion
    }
}
