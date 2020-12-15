using System;
using System.Linq.Expressions;
using InformaticaTrentinaPCL.OpenFile;
using InformaticaTrentinaPCL.Utils;

namespace InformaticaTrentinaPCL.Home.MVPD
{
    /// <summary>
    /// This class is used to mark the cell as Loader
    /// </summary>
    public class DocumentListLoader : AbstractDocumentListItem
    {
        #region Don't use this getters, this is a Marker Object to show the loader

        public override string GetData()
        {
            return "";
        }

        public override string GetEstensione()
        {
            return "";
        }

        public override string GetIdDocumento()
        {
            return "";
        }

        public override string GetIdTrasmissione()
        {
            return "";
        }


        public override string getIdEvento()
        {
            return ""; //TODO Check 

        }

        public override string GetInfo()
        {
            return "";
        }

        public override string GetMittente()
        {
            return "";
        }

        public override string GetOggetto()
        {
            return "";
        }

        public override string GetRagione(){
            return "";    
        }

        public override SignatureInfo getSignatureInfo()
        {
            return SignatureInfo.Create();
        }

        public override void SetFlags()
        {
            return;
        }

        #endregion
    }

}
