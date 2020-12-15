using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using DocsPaVO.areaConservazione;
using System.Collections;

namespace DocsPaConservazione
{
    public class RicercheHtml : IComparable
    {
        public ItemsConservazione itemsCons;
        public string valoreRicerca;
        protected string tipoRicerca;
        public string titoloRicerca;
        public string descContatore;

        protected int docNumber;
        protected int numProt;
        protected DateTime data;
        protected string oggetto;
        protected string fascicolo;
        protected string fileName;
        protected string mittente;
        protected string creatoreDocumento;
        public string tipologiaDoc;
        protected string tipoContatore;
        protected string segnaturaContatore;

        /// <summary>
        /// Questo oggetto serve a personalizzare i criteri di ricerca sugli items di conservazione
        /// </summary>
        /// <param name="items"></param>
        /// <param name="tipo"></param>
        public RicercheHtml(ItemsConservazione items, string tipo)
        {
            itemsCons = items;
            tipoRicerca = tipo;
            switch (tipoRicerca)
            {
                case "docNumber":
                    titoloRicerca = "numero documento";
                    valoreRicerca = itemsCons.DocNumber;
                    docNumber = Convert.ToInt32(valoreRicerca);
                    break;
                case "segnatura":
                    titoloRicerca = "segnatura o numero di documento";
                    valoreRicerca = itemsCons.numProt_or_id;
                    if (!string.IsNullOrEmpty(itemsCons.numProt))
                    {
                        numProt = Convert.ToInt32(itemsCons.numProt);
                    }
                    else
                    {
                        numProt = Convert.ToInt32(itemsCons.DocNumber);
                    }
                    break;
                case "oggetto":
                    titoloRicerca = "descrizione oggetto";
                    valoreRicerca = itemsCons.desc_oggetto;
                    if (!string.IsNullOrEmpty(valoreRicerca))
                    {
                        oggetto = valoreRicerca;
                    }
                    else
                    {
                        valoreRicerca = "descrizione oggetto mancante";
                        oggetto = "descrizione oggetto mancante";
                    }
                    break;
                case "fascicolo":
                    titoloRicerca = "codice fascicolo";
                    valoreRicerca = itemsCons.CodFasc;
                    if (!string.IsNullOrEmpty(valoreRicerca))
                    {
                        fascicolo = valoreRicerca;
                    }
                    else
                    {
                        valoreRicerca = "non fascicolati";
                        fascicolo = "non fascicolati";
                    }
                    break;
                case "data":
                    titoloRicerca = "data di creazione o protocollazione";
                    valoreRicerca = itemsCons.data_prot_or_create;
                    DateTime outDate = new DateTime();
                    CultureInfo culture = new CultureInfo("it-IT");
                    string[] formati = { "dd/MM/yyyy HH.mm.ss", "dd/MM/yyyy H.mm.ss", "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy H:mm:ss", "dd/MM/yyyy" };
                    if (DateTime.TryParseExact(valoreRicerca, formati, culture.DateTimeFormat, System.Globalization.DateTimeStyles.AllowWhiteSpaces, out outDate))
                    {
                        data = DateTime.ParseExact(valoreRicerca, formati, culture.DateTimeFormat, System.Globalization.DateTimeStyles.AllowWhiteSpaces);
                    }
                    else
                    {
                        valoreRicerca = valoreRicerca.Replace(".", ":");
                        data = Convert.ToDateTime(valoreRicerca, culture);
                    }
                    break;
                case "fileName":
                    titoloRicerca = "nome file";
                    valoreRicerca = itemsCons.pathCD;
                    fileName = valoreRicerca;
                    break;
                case "mittente":
                    titoloRicerca = "mittente";
                    valoreRicerca = itemsCons.mittente;
                    if (!string.IsNullOrEmpty(valoreRicerca))
                    {
                        mittente = valoreRicerca;
                    }
                    else
                    {
                        valoreRicerca = "nessun mittente";
                        mittente = valoreRicerca;
                    }
                    break;
                case "creatoreDocumento":
                    titoloRicerca = "creatore documento";
                    valoreRicerca = itemsCons.creatoreDocumento;
                    if (!string.IsNullOrEmpty(valoreRicerca))
                    {
                        creatoreDocumento = valoreRicerca;
                    }
                    else
                    {
                        valoreRicerca = "non specificato";
                        creatoreDocumento = valoreRicerca;
                    }
                    break;
                case "tipologiaDocumento":
                    titoloRicerca = "tipologia documento";
                    valoreRicerca = itemsCons.tipo_atto;
                    if (!string.IsNullOrEmpty(valoreRicerca))
                    {
                        tipologiaDoc = valoreRicerca;
                    }
                    else
                    {
                        valoreRicerca = "nessuna tipologia";
                        tipologiaDoc = valoreRicerca;
                    }
                    break; 
                case "docContatore":
                    if (itemsCons.descContatore != "non valorizzati")
                    {
                        titoloRicerca = "contatore " + itemsCons.descContatore;
                    }
                    else
                    {
                        titoloRicerca = "contatore";
                    }
                   // valoreRicerca = "";
                    valoreRicerca = items.segnaturaContatore;
                    this.segnaturaContatore = valoreRicerca;
                    break;

                default:
                    titoloRicerca = "numero documento";
                    valoreRicerca = itemsCons.DocNumber;
                    docNumber = Convert.ToInt32(valoreRicerca);
                    break;
            }
        }

        public RicercheHtml(Contatore contatore, string tipo)
        {
            switch (tipo)
            {
                case "tipoContatore":
                    itemsCons = contatore.items;
                    tipoRicerca = "contatore";
                    titoloRicerca = "contatore";
                    valoreRicerca = contatore.descContatore;
                    descContatore = contatore.descContatore;
                    tipoContatore = valoreRicerca;//contatore.valoreContatore;
                    tipologiaDoc = contatore.tipoDoc;
                    tipoRicerca = tipo;
                    break;
            }
        }


        int IComparable.CompareTo(object obj)
        {
            if (obj is RicercheHtml)
            {
                RicercheHtml compareItems = (RicercheHtml)obj;
                //faccio un case sul type per decidere come compararli!!!
                switch (compareItems.tipoRicerca)
                {
                    case "docNumber":
                        return this.docNumber.CompareTo(compareItems.docNumber);
                    case "segnatura":
                        return this.numProt.CompareTo(compareItems.numProt);
                    case "oggetto":
                        return this.oggetto.CompareTo(compareItems.oggetto);
                    case "fascicolo":
                        return this.fascicolo.CompareTo(compareItems.fascicolo);
                    case "data":
                        return this.data.CompareTo(compareItems.data);
                    case "fileName":
                        return this.fileName.CompareTo(compareItems.fileName);
                    case "mittente":
                        return this.mittente.CompareTo(compareItems.mittente);
                    case "creatoreDocumento":
                        return this.creatoreDocumento.CompareTo(compareItems.creatoreDocumento);
                    case "tipologiaDocumento":
                        return this.tipologiaDoc.CompareTo(compareItems.tipologiaDoc);
                    case "tipoContatore":
                        return this.tipoContatore.CompareTo(compareItems.tipoContatore);
                    case "docContatore":
                        return this.segnaturaContatore.CompareTo(compareItems.segnaturaContatore);
                    default:
                        return this.docNumber.CompareTo(compareItems.docNumber);
                }
            }
            else
            {
                throw new ArgumentException("Object is not an RicercheHtml");
            }
        }
    }


    public class Contatore
    {
        public string descContatore { get; set; }
        public ItemsConservazione items {get; set;}
        public string tipoDoc { get; set; }
        public string valoreContatore { get; set; }
        public string segnatura { get; set; }     
    }
}
