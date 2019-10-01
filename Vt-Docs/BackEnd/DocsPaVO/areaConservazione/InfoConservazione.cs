using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.areaConservazione
{
    [Serializable()]
    public class InfoConservazione
    {
        public string SystemID;
        public string IdAmm;
        public string IdPeople;
        public string IdRuoloInUo;
        public string StatoConservazione;
        public string TipoSupporto;
        public string Note;
        public string Descrizione;
        public string Data_Apertura;
        public string Data_Invio;
        public string Data_Conservazione;
        public string Data_Prox_Verifica;
        public string Data_Ultima_Verifica;
        public string Data_Riversamento;
        public string MarcaTemporale;
        public string FirmaResponsabile;
        public string LocazioneFisica;
        public string TipoConservazione;
        public string userID;
        public string IdGruppo;
        public string decrSupporto;
        //questi attributi potrebbero non essere valorizzati
        public string numCopie = string.Empty;
        public string noteRifiuto = string.Empty;
        public string formatoDoc = string.Empty;
        public string automatica = string.Empty;
        public bool consolida = false;
        public string idPolicyValidata = string.Empty;
        public bool predefinita = false;
        public int validationMask;
        // MEV 1.5 esito verifica
        public Int32 esitoVerifica;

        /// <summary>
        /// Indica se l'istanza è in fase di preparazione (da "Nuova" a "InLavorazione") 
        /// </summary>
        public bool IstanzaInPreparazione = false;

        /// <summary>
        /// 
        /// </summary>
        public enum EsitoValidazioneMask
        {
            MarcaValida   = 1,    //1  -> 0000 0001  
            FirmaValida   = 2,    //2  -> 0000 0010
            FormatoValido = 4,    //4  -> 0000 0100
            PolicyValida  = 8,    //8  -> 0000 1000
            DimensioneValida = 16,//16 -> 0001 0000
            FileLeggibili = 32,   //32 -> 0010 0000  -> serve per mettere in lavorazione
            TestEseguito  = 128   //128-> 1000 0000 -> Test automatico è stato eseguito
        }

        public enum EsitoVerifica
        {
            NonEffettuata = 0,
            Successo = 1,    
            DirettamenteConvertibili = 2, 
            IndirettamenteConvertibili = 3,
            Fallita = 4,
            Errore = 5
        }
        
    }
}
