using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using DocsPaDocumentale_OCS.CorteContentServices;

namespace DocsPaDocumentale_OCS.OCSServices
{
    /// <summary>
    /// Mapping dei codici restituiti dai servizi
    /// </summary>
    public sealed class OCSResultTypeCodes
    {
        /// <summary>
        /// 
        /// </summary>
        private OCSResultTypeCodes() { }

        /// <summary>
        /// Operazione andata a buon fine
        /// </summary>
        public const string SUCCESSFULL = "000";

        public const string GENERIC_ERROR = "800";
        public const string INSUFFICIENT_INFORMATIONS = "801";
        public const string NOT_ITEM_FOUND="901";
        public const string MALFORMED_URL="902";
        public const string HASH_CODE_VERIFY_ERROR="903";
        public const string INVALID_USERNAME_PASSWORD="700";
        public const string ITEM_NOT_PARENT_ASSOCISATED="904";
        public const string ITEM_NOT_CHILD_ASSOCISATED="905";
        public const string CATEGORY_NOT_PRESENT="906";
        public const string PATH_ITEM_NOT_VALID="907";
        public const string AUTHENTICATION_ERROR="601";
        public const string GROUP_NOT_FOUND="602";
        public const string CLOSE_SESSION_ERROR="500";
        public const string OPEN_SESSION_ERROR="501";
        public const string SERVICE_NOT_AVAIBLE="908";
        public const string SEARCH_RIGHT_EXPRESSION_NOT_VALID="400";
        public const string SEARCH_LEFT_EXPRESSION_NOT_VALID="401";
        public const string SEARCH_RESULT_NOT_UNIQUE="402";
        public const string DOCUMENT_NOT_VERSIONED="912";
        public const string TRANSACTION_NOT_VALID = "914";
    }

    /// <summary>
    /// Eccezione per gestire i result type dei servizi corteconti
    /// </summary>
    public class OCSException : ApplicationException 
    {
        #region Constructors, costants, variables

        /// <summary>
        /// 
        /// </summary>
        private ResultType _result = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public OCSException(ResultType result)
        {
            this._result = result;
        }

		#endregion

        /// <summary>
        /// Dettagli dell'esito della chiamata ad un servizio OCS
        /// </summary>
        public ResultType Result
        {
            get
            {
                return this._result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override string Message
        {
            get
            {
                if (this.Result.code != OCSResultTypeCodes.SUCCESSFULL)
                    return string.Format("CorteContentServicesResult Code: '{0}' - Message: '{1}'", this.Result.code, this.Result.message);
                else
                    return string.Empty;
            }
        }
    }
}
