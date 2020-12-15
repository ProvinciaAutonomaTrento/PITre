using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Reflection;

namespace Rubrica.Library.Data
{
    /// <summary>
    /// Rappresenta un elemento nella rubrica
    /// </summary>
    [Serializable]
    public class ElementoRubrica
    {
        public ElementoRubrica()
        {
            this.Emails = new EMailList();
            this.Urls = new List<UrlInfo>();
        }

        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Codice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Descrizione { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Indirizzo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Telefono { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Fax { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Citta { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Cap { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Provincia { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Nazione { get; set; }

        /// <summary>
        /// Email preferita associata al corrispondente (Mantenuto per retrocompatibilità)
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Lista delle email con relative note associate al corrispondente
        /// </summary>
        public EMailList Emails { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Amministrazione { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AOO { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UtenteCreatore { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime DataCreazione { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime DataUltimaModifica { get; set; }

        /// <summary>
        /// Tipo di corrispondente
        /// </summary>
        public Tipo TipoCorrispondente { get; set; }

        /// <summary>
        /// Url dell'amministrazione cui appartiene il corrispondente
        /// </summary>
        public List<UrlInfo> Urls { get; set; }

        /// <summary>
        /// Indica se l'elemento di rubrica comune è stato pubblicato da frontend(vale 1 in caso positivo)
        /// </summary>
        public string CHA_Pubblicato { get; set; }

        //Emanuela:  aggiungo i campi partita iva e codice fiscale
        /// <summary>
        /// 
        /// </summary>
        public string CodiceFiscale { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PartitaIva { get; set; }

        /// <summary>
        /// Il canale preferenziale (Lettera, EMAIL, ecc...).
        /// </summary>
        public string Canale;

        
    }

    /// <summary>
    /// Enumerazione dei possibili tipi di corrispondente
    /// </summary>
    [Serializable()]
    public enum Tipo
    {
        /// <summary>
        /// Unità Organizzativa
        /// </summary>
        UO,
        /// <summary>
        /// RF
        /// </summary>
        RF
    }
}