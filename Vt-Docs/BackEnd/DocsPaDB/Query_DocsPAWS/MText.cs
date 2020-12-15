using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using DocsPaUtils.Data;
using System.Data;
using DocsPaVO.MText;

namespace DocsPaDB.Query_DocsPAWS
{
    /// <summary>
    /// Classe per l'esecuzione delle operazioni su database relative ad MText
    /// </summary>
    public class MText : DBProvider
    {

        private const String SET_FQN = "SetMTextFullQualifiedName";
        private const String GET_FQN = "GetMTextFullQualifiedName";

        /// <summary>
        /// Funzione per il salvataggio del full qualified name di un documento
        /// </summary>
        /// <param name="mTextDocInfo">Informazioni sul documento per cui impostare l'FQN</param>
        public void SetMTextFullQualifiedName(MTextDocumentInfo mTextDocInfo)
        {
            // Creazione lista dei parametri
            ArrayList parameters = new ArrayList();
            parameters.Add(new ParameterSP("docNumber", mTextDocInfo.DocumentDocNumber, DirectionParameter.ParamInput));
            parameters.Add(new ParameterSP("fqn", mTextDocInfo.FullQualifiedName, DirectionParameter.ParamInput));

            // Esecuzione store
            try
            {
                this.ExecuteStoreProcedure(SET_FQN, parameters);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Si è verificato un problema durante il salvataggio dell'FQN associato al documento.");
            }
 
        }

        /// <summary>
        /// Funzione per il reperimento del full qualified name associato ad un documento
        /// </summary>
        /// <param name="mTextDocInfo">Informazioni sul documento di cui reperire l'FQN</param>
        public MTextDocumentInfo GetMTextFullQualifiedName(MTextDocumentInfo mTextDocInfo)
        {
            ParameterSP fullQualifiedName = new ParameterSP("fqn", String.Empty, 500, DirectionParameter.ParamOutput, DbType.String);
            // Creazione lista dei parametri
            ArrayList parameters = new ArrayList();
            parameters.Add(new ParameterSP("versionId", mTextDocInfo.DocumentVersionId, DirectionParameter.ParamInput));
            parameters.Add(new ParameterSP("docNumber", mTextDocInfo.DocumentDocNumber, DirectionParameter.ParamInput));
            parameters.Add(fullQualifiedName);

            // Esecuzione store
            try
            {
                DataSet ds = new DataSet();
                this.ExecuteStoredProcedure(GET_FQN, parameters, ds);
                
            }
            catch (Exception e)
            {
                throw new ApplicationException("Si è verificato un problema durante il reperimento dell'FQN associato al documento.");
            }

            // Impostazione e restituzione FQN
            mTextDocInfo.FullQualifiedName = fullQualifiedName.Valore.ToString();
            return mTextDocInfo;

        }

    }
}
