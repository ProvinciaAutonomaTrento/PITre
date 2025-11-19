// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections;
using System.Linq;
using System.Text;
using DocsPaVO.documento;
using System.Xml.Serialization;

namespace DocsPaVO.OggettiLite
{
    [Serializable()]
    public class ProtocolloLite
    {
        public string numero;
        public string dataProtocollazione;
        public string anno;
        public string segnatura;
        public string daProtocollare;
        public string invioConferma;
        public string modMittDest;
        public string modMittInt;
        public bool ModUffRef = false;
        //public bool modificaRispostaProtocollo = false;		
        public ProtocolloAnnullato protocolloAnnullato;
        //public InfoDocumento rispostaProtocollo;
        public string descMezzoSpedizione;
        public int mezzoSpedizione;
        public string stampeEffettuate;
    }

    [Serializable()]
    public class ProtocolloEntrataLite : ProtocolloLite
    {
        public DocsPaVO.OggettiLite.CorrispondenteLite mittente;
        public DocsPaVO.OggettiLite.CorrispondenteLite mittenteIntermedio;
        public DocsPaVO.OggettiLite.CorrispondenteLite ufficioReferente;
        public string descrizioneProtocolloMittente;
        public string dataProtocolloMittente;
        public bool daAggiornareMittente = false;
        public bool daAggiornareMittenteIntermedio = false;

        public DocsPaVO.OggettiLite.CorrispondenteLite[] mittenti;
        public bool daAggiornareMittentiMultipli = false;
    }

    [Serializable()]
    public class ProtocolloUscitaLite : ProtocolloLite
    {
        public bool daAggiornareDestinatari = false;
        public bool daAggiornareDestinatariConoscenza = false;
        public bool daAggiornareMittente = false;
        public DocsPaVO.OggettiLite.CorrispondenteLite mittente;
        public DocsPaVO.OggettiLite.CorrispondenteLite ufficioReferente;

        public DocsPaVO.OggettiLite.CorrispondenteLite[] destinatari;

        public DocsPaVO.OggettiLite.CorrispondenteLite[] destinatariConoscenza;
    }

    [Serializable()]
    public class ProtocolloInternoLite : ProtocolloUscitaLite
    {

    }
}
