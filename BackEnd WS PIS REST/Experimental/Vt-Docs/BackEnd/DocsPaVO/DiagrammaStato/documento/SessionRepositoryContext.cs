using System;
using System.Collections.Generic;
using System.Text;

namespace DocsPaVO.documento
{
    /// <summary>
    /// Contesto di esecuzione del repository dei documenti in sessione
    /// </summary>
    [Serializable()]
    public class SessionRepositoryContext
    {
        /// <summary>
        /// 
        /// </summary>
        public SessionRepositoryContext()
        {
            // Creazione del token di identificazione del repository
            this.Token = Guid.NewGuid().ToString("D").Replace("-", string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        public SessionRepositoryContext(DocsPaVO.utente.InfoUtente owner) : this()
        {
            this.Owner = owner;
        }

        /// <summary>
        /// Owner del repository di sessione
        /// </summary>
        public DocsPaVO.utente.InfoUtente Owner
        {
            get;
            set;
        }

        /// <summary>
        /// Identificativo univoco del repository di sessione
        /// </summary>
        public string Token
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDocumentoGrigio
        {
            get;
            set;
        }
    }
}
