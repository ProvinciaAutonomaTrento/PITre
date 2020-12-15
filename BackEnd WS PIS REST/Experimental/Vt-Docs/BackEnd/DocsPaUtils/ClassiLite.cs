using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Xml.Serialization;

namespace DocsPaUtils
{
    public static class ClassiLite
    {


        #region CONVERSIONE DA SCHEDA DOCUMENTO LITE a SCHEDA DOCUMENTO COMPLETA

        //1.
        public  static DocsPaVO.documento.SchedaDocumento getSchedaDoc(DocsPaVO.OggettiLite.SchedaDocumentoLite schedaLite)
        {
            DocsPaVO.documento.SchedaDocumento schedaDoc = new DocsPaVO.documento.SchedaDocumento();
            schedaDoc.accessRights = schedaLite.accessRights;
            schedaDoc.allegati = schedaLite.allegati;
            schedaDoc.appId = schedaLite.appId;
            schedaDoc.assegnato = schedaLite.assegnato;
            schedaDoc.autore = schedaLite.autore;
            schedaDoc.checkOutStatus = schedaLite.checkOutStatus;
            schedaDoc.cod_rf_prot = schedaLite.cod_rf_prot;
            schedaDoc.codiceApplicazione = schedaLite.codiceApplicazione;
            schedaDoc.codiceFascicolo = schedaLite.codiceFascicolo;
            schedaDoc.commissioneRef = schedaLite.commissioneRef;
            schedaDoc.ConsolidationState = schedaLite.ConsolidationState;
            schedaDoc.creatoreDocumento = schedaLite.creatoreDocumento;
            schedaDoc.daAggiornareParoleChiave = schedaLite.daAggiornareParoleChiave;
            schedaDoc.daAggiornarePrivato = schedaLite.daAggiornarePrivato;
            schedaDoc.daAggiornareTipoAtto = schedaLite.daAggiornareTipoAtto;
            schedaDoc.dataCreazione = schedaLite.dataCreazione;
            schedaDoc.dataScadenza = schedaLite.dataScadenza;
            schedaDoc.datiEmergenza = schedaLite.datiEmergenza;
            schedaDoc.descMezzoSpedizione = schedaLite.descMezzoSpedizione;
            schedaDoc.destinatariCCModificati = schedaLite.destinatariCCModificati;
            schedaDoc.destinatariModificati = schedaLite.destinatariModificati;
            schedaDoc.docNumber = schedaLite.docNumber;
            schedaDoc.documenti = schedaLite.documenti;
            schedaDoc.documento_da_pec = schedaLite.documento_da_pec;
            schedaDoc.documentoPrincipale = schedaLite.documentoPrincipale;
            schedaDoc.dtaArrivoDaStoricizzare = schedaLite.dtaArrivoDaStoricizzare;
            schedaDoc.eredita = schedaLite.eredita;
            schedaDoc.evidenza = schedaLite.evidenza;
            schedaDoc.fascicolato = schedaLite.fascicolato;
            schedaDoc.folder = schedaLite.folder;
            schedaDoc.id_rf_invio_ricevuta = schedaLite.id_rf_invio_ricevuta;
            schedaDoc.id_rf_prot = schedaLite.id_rf_prot;
            schedaDoc.idFasciaEta = schedaLite.idFasciaEta;
            schedaDoc.idFascProtoTit = schedaLite.idFascProtoTit;
            schedaDoc.idPeople = schedaLite.idPeople;
            schedaDoc.idTitolario = schedaLite.idTitolario;
            schedaDoc.inArchivio = schedaLite.inCestino;
            schedaDoc.inConservazione = schedaLite.inConservazione;
            schedaDoc.InfoAtipicita = schedaLite.InfoAtipicita;

            schedaDoc.interop = schedaLite.interop;
            schedaDoc.isRiprodotto = schedaLite.isRiprodotto;
            schedaDoc.mezzoSpedizione = schedaLite.mezzoSpedizione;
            schedaDoc.modificaRispostaDocumento = schedaLite.modificaRispostaDocumento;
            schedaDoc.modOggetto = schedaLite.modOggetto;
            schedaDoc.noteDocumento = schedaLite.noteDocumento;
            schedaDoc.numInFasc = schedaLite.numInFasc;
            schedaDoc.numOggetto = schedaLite.numOggetto;
            schedaDoc.numProtTit = schedaLite.numProtTit;
            schedaDoc.oggetto = schedaLite.oggetto;
            schedaDoc.oraCreazione = schedaLite.oraCreazione;
            schedaDoc.paroleChiave = schedaLite.paroleChiave;
            schedaDoc.personale = schedaLite.personale;
            schedaDoc.predisponiProtocollazione = schedaLite.predisponiProtocollazione;
            schedaDoc.pregresso = schedaLite.pregresso;
            schedaDoc.previousVersionsHidden = schedaLite.previousVersionsHidden;
            schedaDoc.privato = schedaLite.privato;
            schedaDoc.protocollatore = schedaLite.protocollatore;
            schedaDoc.protocollo = getProtocolloCompleto(schedaLite.protocollo);
            schedaDoc.protocolloTitolario = schedaLite.protocolloTitolario;
            schedaDoc.registro = schedaLite.registro;
            schedaDoc.repositoryContext = schedaLite.repositoryContext;
            schedaDoc.riferimentoMittente = schedaLite.riferimentoMittente;
            schedaDoc.rispostaDocumento = schedaLite.rispostaDocumento;
            schedaDoc.spedizioneDocumento = getSpedizioneDocumento(schedaLite.spedizioneDocumento);
            schedaDoc.systemId = schedaLite.systemId;
            schedaDoc.template = schedaLite.template;
            schedaDoc.tipologiaAtto = schedaLite.tipologiaAtto;
            schedaDoc.tipoProto = schedaLite.tipoProto;
            schedaDoc.tipoSesso = schedaLite.tipoSesso;
            schedaDoc.typeId = schedaLite.typeId;
            schedaDoc.userId = schedaLite.userId;

            return schedaDoc;
        }
        //2.
        public static DocsPaVO.documento.Protocollo getProtocolloCompleto(DocsPaVO.OggettiLite.ProtocolloLite protLite)
        {
            DocsPaVO.documento.Protocollo proto;
            proto = new DocsPaVO.documento.Protocollo();
            if (protLite == null)
                return null;

            if (protLite.GetType() == typeof(DocsPaVO.OggettiLite.ProtocolloEntrataLite))
            {
                DocsPaVO.documento.ProtocolloEntrata protoA = new DocsPaVO.documento.ProtocolloEntrata();
                DocsPaVO.OggettiLite.ProtocolloEntrataLite protLiteApp = (DocsPaVO.OggettiLite.ProtocolloEntrataLite)protLite;
                protoA = new DocsPaVO.documento.ProtocolloEntrata();
                protoA.anno = protLiteApp.anno;
                protoA.daAggiornareMittente = protLiteApp.daAggiornareMittente;
                protoA.daAggiornareMittenteIntermedio = protLiteApp.daAggiornareMittenteIntermedio;
                protoA.daAggiornareMittentiMultipli = protLiteApp.daAggiornareMittentiMultipli;

                protoA.daProtocollare = protLiteApp.daProtocollare;
                protoA.dataProtocollazione = protLiteApp.dataProtocollazione;
                protoA.dataProtocolloMittente = protLiteApp.dataProtocolloMittente;

                protoA.descMezzoSpedizione = protLiteApp.descMezzoSpedizione;
                protoA.descrizioneProtocolloMittente = protLiteApp.descrizioneProtocolloMittente;

                protoA.invioConferma = protLiteApp.invioConferma;
                protoA.mezzoSpedizione = protLiteApp.mezzoSpedizione;
                protoA.mittente = getCorrispondente(protLiteApp.mittente);
                protoA.mittenteIntermedio = getCorrispondente(protLiteApp.mittenteIntermedio);

                protoA.mittenti = getCorrispondenti(protoA.mittenti);

                protoA.modMittDest = protLiteApp.modMittDest;
                protoA.modMittInt = protLiteApp.modMittInt;
                protoA.ModUffRef = protLiteApp.ModUffRef;
                protoA.numero = protLiteApp.numero;
                protoA.protocolloAnnullato = protLiteApp.protocolloAnnullato;
                protoA.segnatura = protLiteApp.segnatura;
                protoA.stampeEffettuate = protLiteApp.stampeEffettuate;
                protoA.ufficioReferente = getCorrispondente(protLiteApp.ufficioReferente);

                return protoA;

            }

            if (protLite.GetType() == typeof(DocsPaVO.OggettiLite.ProtocolloUscitaLite))
            {
                DocsPaVO.documento.ProtocolloUscita protoU = new DocsPaVO.documento.ProtocolloUscita();
                DocsPaVO.OggettiLite.ProtocolloUscitaLite protLiteApp = (DocsPaVO.OggettiLite.ProtocolloUscitaLite)protLite;
                protoU = new DocsPaVO.documento.ProtocolloUscita();
                protoU.anno = protLiteApp.anno;

                protoU.daAggiornareDestinatari = protLiteApp.daAggiornareDestinatari;
                protoU.daAggiornareDestinatariConoscenza = protLiteApp.daAggiornareDestinatariConoscenza;
                protoU.daAggiornareMittente = protLiteApp.daAggiornareMittente;
                protoU.daProtocollare = protLiteApp.daProtocollare;
                protoU.dataProtocollazione = protLiteApp.dataProtocollazione;
                protoU.descMezzoSpedizione = protLiteApp.descMezzoSpedizione;
                protoU.destinatari = getCorrispondenti(protLiteApp.destinatari);
                protoU.destinatariConoscenza = getCorrispondenti(protLiteApp.destinatariConoscenza);
                protoU.modMittDest = protLiteApp.modMittDest;
                protoU.modMittInt = protLiteApp.modMittInt;
                protoU.ModUffRef = protLiteApp.ModUffRef;
                protoU.numero = protLiteApp.numero;
                protoU.protocolloAnnullato = protLiteApp.protocolloAnnullato;
                protoU.segnatura = protLiteApp.segnatura;
                protoU.stampeEffettuate = protLiteApp.stampeEffettuate;
                protoU.ufficioReferente = getCorrispondente(protLiteApp.ufficioReferente);

                return protoU;

            }

            if (protLite.GetType() == typeof(DocsPaVO.OggettiLite.ProtocolloInternoLite))
            {
                DocsPaVO.documento.ProtocolloInterno protoI = new DocsPaVO.documento.ProtocolloInterno();
                DocsPaVO.OggettiLite.ProtocolloInternoLite protLiteApp = (DocsPaVO.OggettiLite.ProtocolloInternoLite)protLite;
                protoI = new DocsPaVO.documento.ProtocolloInterno();
                protoI.anno = protLiteApp.anno;

                protoI.daAggiornareDestinatari = protLiteApp.daAggiornareDestinatari;
                protoI.daAggiornareDestinatariConoscenza = protLiteApp.daAggiornareDestinatariConoscenza;
                protoI.daAggiornareMittente = protLiteApp.daAggiornareMittente;
                protoI.daProtocollare = protLiteApp.daProtocollare;
                protoI.dataProtocollazione = protLiteApp.dataProtocollazione;
                protoI.descMezzoSpedizione = protLiteApp.descMezzoSpedizione;
                protoI.destinatari = getCorrispondenti(protLiteApp.destinatari);
                protoI.destinatariConoscenza = getCorrispondenti(protLiteApp.destinatariConoscenza);
                protoI.modMittDest = protLiteApp.modMittDest;
                protoI.modMittInt = protLiteApp.modMittInt;
                protoI.ModUffRef = protLiteApp.ModUffRef;
                protoI.numero = protLiteApp.numero;
                protoI.protocolloAnnullato = protLiteApp.protocolloAnnullato;
                protoI.segnatura = protLiteApp.segnatura;
                protoI.stampeEffettuate = protLiteApp.stampeEffettuate;
                protoI.ufficioReferente = getCorrispondente(protLiteApp.ufficioReferente);

                return protoI;

            }

            return proto;
        }
        //3. DA COMPLETARE
        public static DocsPaVO.Spedizione.SpedizioneDocumento getSpedizioneDocumento(DocsPaVO.OggettiLite.SpedizioneDocumentoLite spedizioneLite)
        {
            if (spedizioneLite == null)
                return null;

            DocsPaVO.Spedizione.SpedizioneDocumento spedizioneDocumento;
            spedizioneDocumento = new DocsPaVO.Spedizione.SpedizioneDocumento();
            //DA FARE
            spedizioneDocumento.DestinatariEsterni = getDestinatariEsterni(spedizioneLite.DestinatariEsterni);
            spedizioneDocumento.DestinatariInterni = getDestinatariInterni(spedizioneLite.DestinatariInterni);
            spedizioneDocumento.IdDocumento = spedizioneLite.IdDocumento;
            spedizioneDocumento.IdRegistroRfMittente = spedizioneLite.IdRegistroRfMittente;
            spedizioneDocumento.listaDestinatariNonRaggiungibili = spedizioneLite.listaDestinatariNonRaggiungibili;
            spedizioneDocumento.mailAddress = spedizioneLite.mailAddress;
            spedizioneDocumento.Spedito = spedizioneLite.Spedito;

            return spedizioneDocumento;
        }
        //4.
        public static ArrayList getCorrispondenti(ArrayList corrLite)
        {
            if (corrLite == null)
                return null;
            ArrayList corrispondenti = new ArrayList();
            for (int i = 0; i < corrLite.Count; i++)
            {
                DocsPaVO.utente.Corrispondente corr = getCorrispondente((DocsPaVO.OggettiLite.CorrispondenteLite)corrLite[i]);
                corrispondenti.Add(corr);
            }
            return corrispondenti;
        }
        //5.
        public static DocsPaVO.utente.Corrispondente getCorrispondente(DocsPaVO.OggettiLite.CorrispondenteLite corLite)
        {
            DocsPaVO.utente.Corrispondente corr = new DocsPaVO.utente.Corrispondente();
            if (corLite == null)
                return null;

            switch (corLite.tipoCorrispondente)
            {
                case "U":
                    {
                        DocsPaVO.utente.UnitaOrganizzativa corrUO = new DocsPaVO.utente.UnitaOrganizzativa();
                        corrUO.cap = corLite.cap;
                        corrUO.citta = corLite.citta;
                        corrUO.codiceAmm = corLite.codiceAmm;
                        corrUO.codiceAOO = corLite.codiceAOO;
                        corrUO.codfisc = corLite.codfisc;
                        corrUO.codiceRubrica = corLite.codiceRubrica;
                        corrUO.descrizione = corLite.descrizione;
                        corrUO.dta_fine = corLite.dta_fine;
                        corrUO.email = corLite.email;
                        corrUO.fax = corLite.fax;
                        corrUO.idAmministrazione = corLite.idAmministrazione;
                        corrUO.idRegistro = corLite.idRegistro;
                        corrUO.indirizzo = corLite.indirizzo;
                        corrUO.localita = corLite.localita;
                        corrUO.nazionalita = corLite.nazionalita;
                        corrUO.note = corLite.note;
                        corrUO.prov = corLite.prov;
                        corrUO.systemId = corLite.systemId;
                        corrUO.telefono1 = corLite.telefono1;
                        corrUO.telefono2 = corLite.telefono2;
                        corrUO.tipoCorrispondente = corLite.tipoCorrispondente;
                        return corrUO;
                        break;
                    }

                case "P":
                    {
                        DocsPaVO.utente.Utente corrP = new DocsPaVO.utente.Utente();
                        corrP.cap = corLite.cap;
                        corrP.citta = corLite.citta;
                        corrP.codiceAmm = corLite.codiceAmm;
                        corrP.codiceAOO = corLite.codiceAOO;
                        corrP.codfisc = corLite.codfisc;
                        corrP.codiceRubrica = corLite.codiceRubrica;
                        corrP.descrizione = corLite.descrizione;
                        corrP.dta_fine = corLite.dta_fine;
                        corrP.email = corLite.email;
                        corrP.fax = corLite.fax;
                        corrP.idAmministrazione = corLite.idAmministrazione;
                        corrP.idRegistro = corLite.idRegistro;
                        corrP.indirizzo = corLite.indirizzo;
                        corrP.localita = corLite.localita;
                        corrP.nazionalita = corLite.nazionalita;
                        corrP.note = corLite.note;
                        corrP.prov = corLite.prov;
                        corrP.systemId = corLite.systemId;
                        corrP.telefono1 = corLite.telefono1;
                        corrP.telefono2 = corLite.telefono2;
                        corrP.tipoCorrispondente = corLite.tipoCorrispondente;
                        return corrP;
                        break;
                    }

                case "R":
                    {
                        DocsPaVO.utente.Ruolo corrR = new DocsPaVO.utente.Ruolo();
                        corrR.cap = corLite.cap;
                        corrR.citta = corLite.citta;
                        corrR.codiceAmm = corLite.codiceAmm;
                        corrR.codiceAOO = corLite.codiceAOO;
                        corrR.codfisc = corLite.codfisc;
                        corrR.codiceRubrica = corLite.codiceRubrica;
                        corrR.descrizione = corLite.descrizione;
                        corrR.dta_fine = corLite.dta_fine;
                        corrR.email = corLite.email;
                        corrR.fax = corLite.fax;
                        corrR.idAmministrazione = corLite.idAmministrazione;
                        corrR.idRegistro = corLite.idRegistro;
                        corrR.indirizzo = corLite.indirizzo;
                        corrR.localita = corLite.localita;
                        corrR.nazionalita = corLite.nazionalita;
                        corrR.note = corLite.note;
                        corrR.prov = corLite.prov;
                        corrR.systemId = corLite.systemId;
                        corrR.telefono1 = corLite.telefono1;
                        corrR.telefono2 = corLite.telefono2;
                        corrR.tipoCorrispondente = corLite.tipoCorrispondente;
                        return corrR;
                        break;

                    }
                default :
                    {
                        DocsPaVO.utente.Corrispondente corrC = new DocsPaVO.utente.Corrispondente();
                        corrC.cap = corLite.cap;
                        corrC.citta = corLite.citta;
                        corrC.codiceAmm = corLite.codiceAmm;
                        corrC.codiceAOO = corLite.codiceAOO;
                        corrC.codfisc = corLite.codfisc;
                        corrC.codiceRubrica = corLite.codiceRubrica;
                        corrC.descrizione = corLite.descrizione;
                        corrC.dta_fine = corLite.dta_fine;
                        corrC.email = corLite.email;
                        corrC.fax = corLite.fax;
                        corrC.idAmministrazione = corLite.idAmministrazione;
                        corrC.idRegistro = corLite.idRegistro;
                        corrC.indirizzo = corLite.indirizzo;
                        corrC.localita = corLite.localita;
                        corrC.nazionalita = corLite.nazionalita;
                        corrC.note = corLite.note;
                        corrC.prov = corLite.prov;
                        corrC.systemId = corLite.systemId;
                        corrC.telefono1 = corLite.telefono1;
                        corrC.telefono2 = corLite.telefono2;
                        corrC.tipoCorrispondente = corLite.tipoCorrispondente;

                        return corrC;
                    }
                    return corr;
            }
            return corr;
        }


        //6.
        public static DocsPaVO.Spedizione.DestinatarioEsterno getDestinatarioEsterno(DocsPaVO.OggettiLite.DestinatarioEsternoLite destEsternoLite)
        {
            if (destEsternoLite == null)
                return null;
            DocsPaVO.Spedizione.DestinatarioEsterno destEsterno = new DocsPaVO.Spedizione.DestinatarioEsterno();

            destEsterno.DataUltimaSpedizione = destEsternoLite.DataUltimaSpedizione;
            //destEsterno.DatiDestinatari = getDatiDestinatariLite(destEsternoLite.DatiDestinatari);  //da fare
            destEsterno.Email = destEsternoLite.Email;
            destEsterno.Id = destEsternoLite.Id;
            destEsterno.IncludiInSpedizione = destEsternoLite.IncludiInSpedizione;
            destEsterno.Interoperante = destEsternoLite.Interoperante;
            destEsterno.StatoSpedizione = destEsternoLite.StatoSpedizione;
            return destEsterno;


        }


        //7.
        public static DocsPaVO.Spedizione.DestinatarioInterno getDestinatarioInterno(DocsPaVO.OggettiLite.DestinatarioInternoLite destInternoLite)
        {
            if (destInternoLite == null)
                return null;
            DocsPaVO.Spedizione.DestinatarioInterno destInterno = new DocsPaVO.Spedizione.DestinatarioInterno();

            destInterno.DataUltimaSpedizione = destInternoLite.DataUltimaSpedizione;
            destInterno.DatiDestinatario = getCorrispondente(destInternoLite.DatiDestinatario);
            destInterno.DisabledTrasm = destInternoLite.DisabledTrasm;
            destInterno.Email = destInternoLite.Email;
            destInterno.Id = destInternoLite.Id;
            destInterno.IncludiInSpedizione = destInternoLite.IncludiInSpedizione;
            destInterno.StatoSpedizione = destInternoLite.StatoSpedizione;
            return destInterno;


        }

        //8
        public static System.Collections.Generic.List<DocsPaVO.Spedizione.DestinatarioEsterno> getDestinatariEsterni(System.Collections.Generic.List<DocsPaVO.OggettiLite.DestinatarioEsternoLite> destLite)
        {
            if (destLite == null)
                return null;
            System.Collections.Generic.List<DocsPaVO.Spedizione.DestinatarioEsterno> corrispondenti = new System.Collections.Generic.List<DocsPaVO.Spedizione.DestinatarioEsterno>();
            for (int i = 0; i < destLite.Count; i++)
            {
                DocsPaVO.Spedizione.DestinatarioEsterno corr = getDestinatarioEsterno((DocsPaVO.OggettiLite.DestinatarioEsternoLite)destLite[i]);
                corrispondenti[i] = corr;
            }
            return corrispondenti;
        }

        //9 
        public static System.Collections.Generic.List<DocsPaVO.utente.Corrispondente> getDatiDestinatari(System.Collections.Generic.List<DocsPaVO.OggettiLite.CorrispondenteLite> datiCorLite)
        {
            System.Collections.Generic.List<DocsPaVO.utente.Corrispondente> datiCor = new System.Collections.Generic.List<DocsPaVO.utente.Corrispondente>();
            if (datiCorLite == null)
                return null;

            datiCor = new System.Collections.Generic.List<DocsPaVO.utente.Corrispondente>();
            for (int i = 0; i < datiCorLite.Count; i++)
            {
                datiCor.Add(getCorrispondente(datiCorLite[i]));
            }

            return datiCor;
        }

        public static System.Collections.Generic.List<DocsPaVO.Spedizione.DestinatarioInterno> getDestinatariInterni(System.Collections.Generic.List<DocsPaVO.OggettiLite.DestinatarioInternoLite> destLite)
        {
            if (destLite == null)
                return null;
            System.Collections.Generic.List<DocsPaVO.Spedizione.DestinatarioInterno> corrispondenti = new System.Collections.Generic.List<DocsPaVO.Spedizione.DestinatarioInterno>();
            for (int i = 0; i < destLite.Count; i++)
            {
                DocsPaVO.Spedizione.DestinatarioInterno corr = getDestinatarioInterno((DocsPaVO.OggettiLite.DestinatarioInternoLite)destLite[i]);
                corrispondenti[i] = corr;
            }
            return corrispondenti;
        }


        #endregion


        #region CONVERSIONE DA SCHEDA DOCUMENTO COMPLETA a SCHEDA DOCUMENTO LITE

        //1. lite
        public static DocsPaVO.OggettiLite.SchedaDocumentoLite getSchedaLite(DocsPaVO.documento.SchedaDocumento schedaDoc)
        {
            DocsPaVO.OggettiLite.SchedaDocumentoLite schedaLite = new DocsPaVO.OggettiLite.SchedaDocumentoLite();
            schedaLite.accessRights = schedaDoc.accessRights;
            schedaLite.allegati = schedaDoc.allegati;
            schedaLite.appId = schedaDoc.appId;
            schedaLite.assegnato = schedaDoc.assegnato;
            schedaLite.autore = schedaDoc.autore;
            schedaLite.checkOutStatus = schedaDoc.checkOutStatus;
            schedaLite.cod_rf_prot = schedaDoc.cod_rf_prot;
            schedaLite.codiceApplicazione = schedaDoc.codiceApplicazione;
            schedaLite.codiceFascicolo = schedaDoc.codiceFascicolo;
            schedaLite.commissioneRef = schedaDoc.commissioneRef;
            schedaLite.ConsolidationState = schedaDoc.ConsolidationState;
            schedaLite.creatoreDocumento = schedaDoc.creatoreDocumento;
            schedaLite.daAggiornareParoleChiave = schedaDoc.daAggiornareParoleChiave;
            schedaLite.daAggiornarePrivato = schedaDoc.daAggiornarePrivato;
            schedaLite.daAggiornareTipoAtto = schedaDoc.daAggiornareTipoAtto;
            schedaLite.dataCreazione = schedaDoc.dataCreazione;
            schedaLite.dataScadenza = schedaDoc.dataScadenza;
            schedaLite.datiEmergenza = schedaDoc.datiEmergenza;
            schedaLite.descMezzoSpedizione = schedaDoc.descMezzoSpedizione;
            schedaLite.destinatariCCModificati = schedaDoc.destinatariCCModificati;
            schedaLite.destinatariModificati = schedaDoc.destinatariModificati;
            schedaLite.docNumber = schedaDoc.docNumber;
            schedaLite.documenti = schedaDoc.documenti;
            schedaLite.documento_da_pec = schedaDoc.documento_da_pec;
            schedaLite.documentoPrincipale = schedaDoc.documentoPrincipale;
            schedaLite.dtaArrivoDaStoricizzare = schedaDoc.dtaArrivoDaStoricizzare;
            schedaLite.eredita = schedaDoc.eredita;
            schedaLite.evidenza = schedaDoc.evidenza;
            schedaLite.fascicolato = schedaDoc.fascicolato;
            schedaLite.folder = schedaDoc.folder;
            schedaLite.id_rf_invio_ricevuta = schedaDoc.id_rf_invio_ricevuta;
            schedaLite.id_rf_prot = schedaDoc.id_rf_prot;
            schedaLite.idFasciaEta = schedaDoc.idFasciaEta;
            schedaLite.idFascProtoTit = schedaDoc.idFascProtoTit;
            schedaLite.idPeople = schedaDoc.idPeople;
            schedaLite.idTitolario = schedaDoc.idTitolario;
            schedaLite.inCestino = schedaDoc.inCestino;
            schedaLite.inConservazione = schedaDoc.inConservazione;
            schedaLite.InfoAtipicita = schedaDoc.InfoAtipicita;

            schedaLite.interop = schedaDoc.interop;
            schedaLite.isRiprodotto = schedaDoc.isRiprodotto;
            schedaLite.mezzoSpedizione = schedaDoc.mezzoSpedizione;
            schedaLite.modificaRispostaDocumento = schedaDoc.modificaRispostaDocumento;
            schedaLite.modOggetto = schedaDoc.modOggetto;
            schedaLite.noteDocumento = schedaDoc.noteDocumento;
            schedaLite.numInFasc = schedaDoc.numInFasc;
            schedaLite.numOggetto = schedaDoc.numOggetto;
            schedaLite.numProtTit = schedaDoc.numProtTit;
            schedaLite.oggetto = schedaDoc.oggetto;
            schedaLite.oraCreazione = schedaDoc.oraCreazione;
            schedaLite.paroleChiave = schedaDoc.paroleChiave;
            schedaLite.personale = schedaDoc.personale;
            schedaLite.predisponiProtocollazione = schedaDoc.predisponiProtocollazione;
            schedaLite.pregresso = schedaDoc.pregresso;
            schedaLite.previousVersionsHidden = schedaDoc.previousVersionsHidden;
            schedaLite.privato = schedaDoc.privato;
            schedaLite.protocollatore = schedaDoc.protocollatore;
            schedaLite.protocollo = getProtocolloLite(schedaDoc.protocollo);  //da fare
            schedaLite.protocolloTitolario = schedaDoc.protocolloTitolario;
            schedaLite.registro = schedaDoc.registro;
            schedaLite.repositoryContext = schedaDoc.repositoryContext;
            schedaLite.riferimentoMittente = schedaDoc.riferimentoMittente;
            schedaLite.rispostaDocumento = schedaDoc.rispostaDocumento;
            schedaLite.spedizioneDocumento = getSpedizioneDocumentoLite(schedaDoc.spedizioneDocumento);
            schedaLite.systemId = schedaDoc.systemId;
            schedaLite.template = schedaDoc.template;
            schedaLite.tipologiaAtto = schedaDoc.tipologiaAtto;
            schedaLite.tipoProto = schedaDoc.tipoProto;
            schedaLite.tipoSesso = schedaDoc.tipoSesso;
            schedaLite.typeId = schedaDoc.typeId;
            schedaLite.userId = schedaDoc.userId;

            return schedaLite;
        }

        //2. lite
        public static DocsPaVO.OggettiLite.ProtocolloLite getProtocolloLite(DocsPaVO.documento.Protocollo proto)
        {
            DocsPaVO.OggettiLite.ProtocolloLite protLite;
            protLite = new DocsPaVO.OggettiLite.ProtocolloLite();
            if (proto == null)
                return null;

            if (proto.GetType() == typeof(DocsPaVO.documento.ProtocolloEntrata))
            {
                DocsPaVO.OggettiLite.ProtocolloEntrataLite protoLiteA = new DocsPaVO.OggettiLite.ProtocolloEntrataLite();
                DocsPaVO.documento.ProtocolloEntrata protoApp = (DocsPaVO.documento.ProtocolloEntrata)proto;
                protoLiteA = new DocsPaVO.OggettiLite.ProtocolloEntrataLite();
                protoLiteA.anno = protoApp.anno;
                protoLiteA.daAggiornareMittente = protoApp.daAggiornareMittente;
                protoLiteA.daAggiornareMittenteIntermedio = protoApp.daAggiornareMittenteIntermedio;
                protoLiteA.daAggiornareMittentiMultipli = protoApp.daAggiornareMittentiMultipli;

                protoLiteA.daProtocollare = protoApp.daProtocollare;
                protoLiteA.dataProtocollazione = protoApp.dataProtocollazione;
                protoLiteA.dataProtocolloMittente = protoApp.dataProtocolloMittente;

                protoLiteA.descMezzoSpedizione = protoApp.descMezzoSpedizione;
                protoLiteA.descrizioneProtocolloMittente = protoApp.descrizioneProtocolloMittente;

                protoLiteA.invioConferma = protoApp.invioConferma;
                protoLiteA.mezzoSpedizione = protoApp.mezzoSpedizione;
                protoLiteA.mittente = getCorrispondenteLite(protoApp.mittente);
                protoLiteA.mittenteIntermedio = getCorrispondenteLite(protoApp.mittenteIntermedio);

                protoLiteA.mittenti = getCorrispondentiLite(protoApp.mittenti);

                protoLiteA.modMittDest = protoApp.modMittDest;
                protoLiteA.modMittInt = protoApp.modMittInt;
                protoLiteA.ModUffRef = protoApp.ModUffRef;
                protoLiteA.numero = protoApp.numero;
                protoLiteA.protocolloAnnullato = protoApp.protocolloAnnullato;
                protoLiteA.segnatura = protoApp.segnatura;
                protoLiteA.stampeEffettuate = protoApp.stampeEffettuate;
                protoLiteA.ufficioReferente = getCorrispondenteLite(protoApp.ufficioReferente);

                return protoLiteA;

            }

            if (proto.GetType() == typeof(DocsPaVO.documento.ProtocolloUscita))
            {
                DocsPaVO.OggettiLite.ProtocolloUscitaLite protoLiteU = new DocsPaVO.OggettiLite.ProtocolloUscitaLite();
                DocsPaVO.documento.ProtocolloUscita protoApp = (DocsPaVO.documento.ProtocolloUscita)proto;
                protoLiteU = new DocsPaVO.OggettiLite.ProtocolloUscitaLite();
                protoLiteU.anno = protoApp.anno;

                protoLiteU.daAggiornareDestinatari = protoApp.daAggiornareDestinatari;
                protoLiteU.daAggiornareDestinatariConoscenza = protoApp.daAggiornareDestinatariConoscenza;
                protoLiteU.daAggiornareMittente = protoApp.daAggiornareMittente;
                protoLiteU.daProtocollare = protoApp.daProtocollare;
                protoLiteU.dataProtocollazione = protoApp.dataProtocollazione;
                protoLiteU.descMezzoSpedizione = protoApp.descMezzoSpedizione;
                protoLiteU.destinatari = getCorrispondentiLite(protoApp.destinatari);
                protoLiteU.destinatariConoscenza = getCorrispondentiLite(protoApp.destinatariConoscenza);
                protoLiteU.modMittDest = protoApp.modMittDest;
                protoLiteU.modMittInt = protoApp.modMittInt;
                protoLiteU.ModUffRef = protoApp.ModUffRef;
                protoLiteU.numero = protoApp.numero;
                protoLiteU.protocolloAnnullato = protoApp.protocolloAnnullato;
                protoLiteU.segnatura = protoApp.segnatura;
                protoLiteU.stampeEffettuate = protoApp.stampeEffettuate;
                protoLiteU.ufficioReferente = getCorrispondenteLite(protoApp.ufficioReferente);

                return protoLiteU;

            }


            if (proto.GetType() == typeof(DocsPaVO.documento.ProtocolloInterno))
            {
                DocsPaVO.OggettiLite.ProtocolloInternoLite protoLiteI = new DocsPaVO.OggettiLite.ProtocolloInternoLite();
                DocsPaVO.documento.ProtocolloInterno protoApp = (DocsPaVO.documento.ProtocolloInterno)proto;
                protoLiteI = new DocsPaVO.OggettiLite.ProtocolloInternoLite();
                protoLiteI.anno = protoApp.anno;

                protoLiteI.daAggiornareDestinatari = protoApp.daAggiornareDestinatari;
                protoLiteI.daAggiornareDestinatariConoscenza = protoApp.daAggiornareDestinatariConoscenza;
                protoLiteI.daAggiornareMittente = protoApp.daAggiornareMittente;
                protoLiteI.daProtocollare = protoApp.daProtocollare;
                protoLiteI.dataProtocollazione = protoApp.dataProtocollazione;
                protoLiteI.descMezzoSpedizione = protoApp.descMezzoSpedizione;
                protoLiteI.destinatari = getCorrispondentiLite(protoApp.destinatari);
                protoLiteI.destinatariConoscenza = getCorrispondentiLite(protoApp.destinatariConoscenza);
                protoLiteI.modMittDest = protoApp.modMittDest;
                protoLiteI.modMittInt = protoApp.modMittInt;
                protoLiteI.ModUffRef = protoApp.ModUffRef;
                protoLiteI.numero = protoApp.numero;
                protoLiteI.protocolloAnnullato = protoApp.protocolloAnnullato;
                protoLiteI.segnatura = protoApp.segnatura;
                protoLiteI.stampeEffettuate = protoApp.stampeEffettuate;
                protoLiteI.ufficioReferente = getCorrispondenteLite(protoApp.ufficioReferente);

                return protoLiteI;

            }

            return protLite;
        }


        //3. Lite DA COMPLETARE
        public static DocsPaVO.OggettiLite.SpedizioneDocumentoLite getSpedizioneDocumentoLite(DocsPaVO.Spedizione.SpedizioneDocumento spedizioneDocumento)
        {
            if (spedizioneDocumento == null)
                return null;

            DocsPaVO.OggettiLite.SpedizioneDocumentoLite spedizioneDocumentoLite;
            spedizioneDocumentoLite = new DocsPaVO.OggettiLite.SpedizioneDocumentoLite();
            //DA FARE
            //spedizioneDocumentoLite.DestinatariEsterni = getDestinatariEsterniLite(spedizioneDocumento.DestinatariEsterni);
            //spedizioneDocumentoLite.DestinatariInterni = spedizioneDocumento.DestinatariInterni;
            spedizioneDocumentoLite.IdDocumento = spedizioneDocumento.IdDocumento;
            spedizioneDocumentoLite.IdRegistroRfMittente = spedizioneDocumento.IdRegistroRfMittente;
            spedizioneDocumentoLite.listaDestinatariNonRaggiungibili = spedizioneDocumento.listaDestinatariNonRaggiungibili;
            spedizioneDocumentoLite.mailAddress = spedizioneDocumento.mailAddress;
            spedizioneDocumentoLite.Spedito = spedizioneDocumento.Spedito;

            return spedizioneDocumentoLite;
        }

        //4. lite
        public static ArrayList getCorrispondentiLite(ArrayList corrispondenti)
        {
            if (corrispondenti == null)
                return null;
            ArrayList corrLite = new ArrayList();
            for (int i = 0; i < corrispondenti.Count; i++)
            {
                DocsPaVO.OggettiLite.CorrispondenteLite corr = getCorrispondenteLite((DocsPaVO.utente.Corrispondente)corrispondenti[i]);
                corrLite.Add(corr);
            }
            return corrLite;
        }

        //5. lite
        public static DocsPaVO.OggettiLite.CorrispondenteLite getCorrispondenteLite(DocsPaVO.utente.Corrispondente corrispondente)
        {
            DocsPaVO.OggettiLite.CorrispondenteLite corrLite = new DocsPaVO.OggettiLite.CorrispondenteLite();
            if (corrispondente == null)
                return null;

            switch (corrispondente.tipoCorrispondente)
            {
                case "U":
                    {
                        DocsPaVO.OggettiLite.CorrispondenteLite corrUO = new DocsPaVO.OggettiLite.CorrispondenteLite();
                        corrUO.cap = corrispondente.cap;
                        corrUO.citta = corrispondente.citta;
                        corrUO.codiceAmm = corrispondente.codiceAmm;
                        corrUO.codiceAOO = corrispondente.codiceAOO;
                        corrUO.codfisc = corrispondente.codfisc;
                        corrUO.codiceRubrica = corrispondente.codiceRubrica;
                        corrUO.descrizione = corrispondente.descrizione;
                        corrUO.dta_fine = corrispondente.dta_fine;
                        corrUO.email = corrispondente.email;
                        corrUO.fax = corrispondente.fax;
                        corrUO.idAmministrazione = corrispondente.idAmministrazione;
                        corrUO.idRegistro = corrispondente.idRegistro;
                        corrUO.indirizzo = corrispondente.indirizzo;
                        corrUO.localita = corrispondente.localita;
                        corrUO.nazionalita = corrispondente.nazionalita;
                        corrUO.note = corrispondente.note;
                        corrUO.prov = corrispondente.prov;
                        corrUO.systemId = corrispondente.systemId;
                        corrUO.telefono1 = corrispondente.telefono1;
                        corrUO.telefono2 = corrispondente.telefono2;
                        corrUO.tipoCorrispondente = corrispondente.tipoCorrispondente;
                        return corrUO;
                        break;
                    }

                case "P":
                    {
                        DocsPaVO.OggettiLite.CorrispondenteLite corrP = new DocsPaVO.OggettiLite.CorrispondenteLite();
                        corrP.cap = corrispondente.cap;
                        corrP.citta = corrispondente.citta;
                        corrP.codiceAmm = corrispondente.codiceAmm;
                        corrP.codiceAOO = corrispondente.codiceAOO;
                        corrP.codfisc = corrispondente.codfisc;
                        corrP.codiceRubrica = corrispondente.codiceRubrica;
                        corrP.descrizione = corrispondente.descrizione;
                        corrP.dta_fine = corrispondente.dta_fine;
                        corrP.email = corrispondente.email;
                        corrP.fax = corrispondente.fax;
                        corrP.idAmministrazione = corrispondente.idAmministrazione;
                        corrP.idRegistro = corrispondente.idRegistro;
                        corrP.indirizzo = corrispondente.indirizzo;
                        corrP.localita = corrispondente.localita;
                        corrP.nazionalita = corrispondente.nazionalita;
                        corrP.note = corrispondente.note;
                        corrP.prov = corrispondente.prov;
                        corrP.systemId = corrispondente.systemId;
                        corrP.telefono1 = corrispondente.telefono1;
                        corrP.telefono2 = corrispondente.telefono2;
                        corrP.tipoCorrispondente = corrispondente.tipoCorrispondente;
                        return corrP;
                        break;
                    }

                case "R":
                    {
                        DocsPaVO.OggettiLite.CorrispondenteLite corrR = new DocsPaVO.OggettiLite.CorrispondenteLite();
                        corrR.cap = corrispondente.cap;
                        corrR.citta = corrispondente.citta;
                        corrR.codiceAmm = corrispondente.codiceAmm;
                        corrR.codiceAOO = corrispondente.codiceAOO;
                        corrR.codfisc = corrispondente.codfisc;
                        corrR.codiceRubrica = corrispondente.codiceRubrica;
                        corrR.descrizione = corrispondente.descrizione;
                        corrR.dta_fine = corrispondente.dta_fine;
                        corrR.email = corrispondente.email;
                        corrR.fax = corrispondente.fax;
                        corrR.idAmministrazione = corrispondente.idAmministrazione;
                        corrR.idRegistro = corrispondente.idRegistro;
                        corrR.indirizzo = corrispondente.indirizzo;
                        corrR.localita = corrispondente.localita;
                        corrR.nazionalita = corrispondente.nazionalita;
                        corrR.note = corrispondente.note;
                        corrR.prov = corrispondente.prov;
                        corrR.systemId = corrispondente.systemId;
                        corrR.telefono1 = corrispondente.telefono1;
                        corrR.telefono2 = corrispondente.telefono2;
                        corrR.tipoCorrispondente = corrispondente.tipoCorrispondente;
                        return corrR;              
                    }
                default:
                    {
                        DocsPaVO.OggettiLite.CorrispondenteLite corr = new DocsPaVO.OggettiLite.CorrispondenteLite();
                        corr.cap = corrispondente.cap;
                        corr.citta = corrispondente.citta;
                        corr.codiceAmm = corrispondente.codiceAmm;
                        corr.codiceAOO = corrispondente.codiceAOO;
                        corr.codfisc = corrispondente.codfisc;
                        corr.codiceRubrica = corrispondente.codiceRubrica;
                        corr.descrizione = corrispondente.descrizione;
                        corr.dta_fine = corrispondente.dta_fine;
                        corr.email = corrispondente.email;
                        corr.fax = corrispondente.fax;
                        corr.idAmministrazione = corrispondente.idAmministrazione;
                        corr.idRegistro = corrispondente.idRegistro;
                        corr.indirizzo = corrispondente.indirizzo;
                        corr.localita = corrispondente.localita;
                        corr.nazionalita = corrispondente.nazionalita;
                        corr.note = corrispondente.note;
                        corr.prov = corrispondente.prov;
                        corr.systemId = corrispondente.systemId;
                        corr.telefono1 = corrispondente.telefono1;
                        corr.telefono2 = corrispondente.telefono2;
                        corr.tipoCorrispondente = corrispondente.tipoCorrispondente;
                        return corr;              
                    }

                    return corrLite;
            }
            return corrLite;
        }

        //6. lite 
        public static DocsPaVO.OggettiLite.DestinatarioEsternoLite getDestinatarioEsternoLite(DocsPaVO.Spedizione.DestinatarioEsterno destEsterno)
        {
            if (destEsterno == null)
                return null;
            DocsPaVO.OggettiLite.DestinatarioEsternoLite destEsternoLite = new DocsPaVO.OggettiLite.DestinatarioEsternoLite();

            destEsternoLite.DataUltimaSpedizione = destEsternoLite.DataUltimaSpedizione;
            //destEsternoLite.DatiDestinatari = getDatiDestinatariLite(destEsterno.DatiDestinatari);  //da fare
            destEsternoLite.Email = destEsterno.Email;
            destEsternoLite.Id = destEsterno.Id;
            destEsternoLite.IncludiInSpedizione = destEsterno.IncludiInSpedizione;
            destEsternoLite.Interoperante = destEsterno.Interoperante;
            destEsternoLite.StatoSpedizione = destEsterno.StatoSpedizione;
            return destEsternoLite;

        }

        //7. lite
        public static DocsPaVO.OggettiLite.DestinatarioInternoLite getDestinatarioInternoLite(DocsPaVO.Spedizione.DestinatarioInterno destInterno)
        {
            if (destInterno == null)
                return null;
            DocsPaVO.OggettiLite.DestinatarioInternoLite destInternoLite = new DocsPaVO.OggettiLite.DestinatarioInternoLite();

            destInternoLite.DataUltimaSpedizione = destInternoLite.DataUltimaSpedizione;
            destInternoLite.DatiDestinatario = getCorrispondenteLite(destInterno.DatiDestinatario);
            destInternoLite.DisabledTrasm = destInterno.DisabledTrasm;
            destInternoLite.Email = destInterno.Email;
            destInternoLite.Id = destInterno.Id;
            destInternoLite.IncludiInSpedizione = destInterno.IncludiInSpedizione;
            destInternoLite.StatoSpedizione = destInterno.StatoSpedizione;
            return destInternoLite;


        }

        //8 lite
        public static System.Collections.Generic.List<DocsPaVO.OggettiLite.DestinatarioEsternoLite> getDestinatariEsterniLite(System.Collections.Generic.List<DocsPaVO.Spedizione.DestinatarioEsterno> corr)
        {
            if (corr == null)
                return null;
            System.Collections.Generic.List<DocsPaVO.OggettiLite.DestinatarioEsternoLite> corrispondenti = new System.Collections.Generic.List<DocsPaVO.OggettiLite.DestinatarioEsternoLite>();
            for (int i = 0; i < corr.Count; i++)
            {
                DocsPaVO.OggettiLite.DestinatarioEsternoLite corrL = getDestinatarioEsternoLite((DocsPaVO.Spedizione.DestinatarioEsterno)corr[i]);
                corrispondenti[i] = corrL;
            }
            return corrispondenti;
        }

        //9 lite
        public static System.Collections.Generic.List<DocsPaVO.OggettiLite.CorrispondenteLite> getDatiDestinatariLite(System.Collections.Generic.List<DocsPaVO.utente.Corrispondente> datiCor)
        {
            System.Collections.Generic.List<DocsPaVO.OggettiLite.CorrispondenteLite> datiCorLite = new System.Collections.Generic.List<DocsPaVO.OggettiLite.CorrispondenteLite>();
            if (datiCor == null)
                return null;

            datiCorLite = new System.Collections.Generic.List<DocsPaVO.OggettiLite.CorrispondenteLite>();
            for (int i = 0; i < datiCor.Count; i++)
            {
                datiCorLite.Add(getCorrispondenteLite(datiCor[i]));
            }

            return datiCorLite;
        }

        #endregion

    }
}
