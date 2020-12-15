using System;
using System.Collections.Generic;
using System.Xml.Serialization;


namespace DocsPaVO.areaConservazione
{

    [Serializable()]
    public class InfoEsibizione
    {

        public string SystemID;
        public string IdAmm;
        public string IdPeople;
        public string IdRuoloInUo;
        public string statoEsibizione;
        
        public string Note;
        public string Descrizione;
        public string NoteRifiuto;
        public string Data_Creazione;
        public string Data_Certificazione;
        public string Data_Chiusura;
        public string Data_Rifiuto;
        
        public string MarcaTemporale;
        public string FirmaResponsabile;
                
        public bool isCertificata;
        public bool isRichiestaCertificazione;
        public string idProfileCertificazione;

        public string richiedente;


    }
}
