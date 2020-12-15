using System;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using DocsPaVO.utente;

namespace DocsPaVO.amministrazione
{
    /// <summary>
    /// Oggetto contenente i dati di connessione relativi all'utente amministratore
    /// </summary>
    public class InfoUtenteAmministratore : InfoUtente
    {
        /// <summary>
        /// 
        /// </summary>
        public InfoUtenteAmministratore()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ut"></param>
        /// <param name="ruo"></param>
        public InfoUtenteAmministratore(Utente ut, Ruolo ruo)
            : base(ut, ruo)
        {
        }
        
        /// <summary>
        /// 
        /// </summary>
        public string nome = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string cognome = string.Empty;

        /// <summary>
        /// Tipo di amministratore
        /// </summary>
        public string tipoAmministratore = string.Empty;
        
        /// <summary>
        /// 
        /// </summary>
        [XmlArray()]
        [XmlArrayItem(typeof(DocsPaVO.amministrazione.Menu))]
        public ArrayList VociMenu = new ArrayList();
    }
}
