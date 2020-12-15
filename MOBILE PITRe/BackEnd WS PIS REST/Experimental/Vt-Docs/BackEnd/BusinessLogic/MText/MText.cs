using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaDB;
using DocsPaVO.MText;


namespace BusinessLogic.MText
{
    public class MText
    {
        /// <summary>
        /// Funzione per il salvataggio del path del documento M/Text associato ad una data versione di 
        /// un documento
        /// </summary>
        /// <param name="mTextDocInfo">Informazioni sul documento per cui impostare l'FQN</param>
        public static void SetMTextFullQualifiedName(MTextDocumentInfo mTextDocInfo)
        {
            using (DocsPaDB.Query_DocsPAWS.MText mtextProvider = new DocsPaDB.Query_DocsPAWS.MText())
            {
                mtextProvider.SetMTextFullQualifiedName(mTextDocInfo);
            }
        }

        /// <summary>
        /// Funzione per il reperimento del path del documento M/Text associato ad una data versione di un
        /// documento
        /// </summary>
        /// <param name="mTextDocumentInfo">Informazioni sul documento di cui recuperare l'FQN</param>
        public static MTextDocumentInfo GetMTextFullQualifiedName(MTextDocumentInfo mTextDocumentInfo) 
        {
            using (DocsPaDB.Query_DocsPAWS.MText mtextProvider = new DocsPaDB.Query_DocsPAWS.MText())
            {
                return mtextProvider.GetMTextFullQualifiedName((DocsPaVO.MText.MTextDocumentInfo)mTextDocumentInfo.Clone());
            }

        }
    }
}
