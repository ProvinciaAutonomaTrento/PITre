using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocsPaVO.utente;
using DocsPaVO.documento;
using DocsPaVO.fascicolazione;
using DocsPaVO.addressbook;
using DocsPaVO.Mobile;

namespace DocsPaWS.Mobile.Builders
{
    public class CorrispondenteBuilder
    {
        private Corrispondente _corrispondente;
        private delegate Corrispondente getCorrFasc(CorrFascInput input);
        private delegate Corrispondente getCorrDoc(CorrDocInput schedaDoc);
        private static Dictionary<string, getCorrFasc> _corrFascDel;
        private static Dictionary<string, getCorrDoc> _corrDocDel;

        private class CorrFascInput
        {
            public Fascicolo fascicolo;
            public Ruolo ruolo;
            public InfoUtente infoUtente;
        }

        private class CorrDocInput
        {
            public SchedaDocumento documento;
            public Ruolo ruolo;
            public InfoUtente infoUtente;
        }

        static CorrispondenteBuilder()
        {
            _corrFascDel = new Dictionary<string, getCorrFasc>();
            _corrFascDel.Add("UT_P", UTPFasc);
            _corrFascDel.Add("R_P", RPFasc);
            _corrFascDel.Add("UO_P", UOPFasc);
            _corrFascDel.Add("RSP_P", RSPPFasc);
            _corrFascDel.Add("R_S", RSFasc);
            _corrFascDel.Add("RSP_M", RSPMFasc);
            _corrFascDel.Add("S_M",SMFasc);
            _corrDocDel = new Dictionary<string, getCorrDoc>();
            _corrDocDel.Add("UT_P", UTPDoc);
            _corrDocDel.Add("R_P", RPDoc);
            _corrDocDel.Add("UO_P", UOPDoc);
            _corrDocDel.Add("RSP_P", RSPPDoc);
            _corrDocDel.Add("R_S", RSDoc);
            _corrDocDel.Add("RSP_M", RSPMDoc);
            _corrDocDel.Add("S_M", SMDoc);
        }

        public CorrispondenteBuilder(string tipoDestinatario, SchedaDocumento schedaDoc)
        {
            CorrDocInput input = null;
            _corrispondente = _corrDocDel[tipoDestinatario](input);
        }

        public CorrispondenteBuilder(string tipoDestinatario, Fascicolo fascicolo,Ruolo ruolo,UserInfo userInfo)
        {
            CorrFascInput input = null;
            _corrispondente = _corrFascDel[tipoDestinatario](input);
        }

        public Corrispondente Corrispondente
        {
            get
            {
                return _corrispondente;
            }
        }

        #region Documento

        private static Corrispondente UTPDoc(CorrDocInput input)
        {
            Corrispondente corr = new Corrispondente();
            string utenteProprietario = string.Empty;
            if (input.documento.protocollatore != null && input.documento.protocollo != null && !string.IsNullOrEmpty(input.documento.protocollo.numero))
            {
                if (input.documento.creatoreDocumento != null)
                {
                    utenteProprietario = input.documento.creatoreDocumento.idPeople;
                }
                else utenteProprietario = input.documento.protocollatore.utente_idPeople;

            }
            else
            {
                utenteProprietario = input.documento.creatoreDocumento.idPeople;
            }
            corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByIdPeople(utenteProprietario, TipoUtente.INTERNO, input.infoUtente);
            return corr;
        }

        private static Corrispondente RPDoc(CorrDocInput input)
        {
            Corrispondente corr = new Corrispondente();
            string idCorrGlobaliRuolo = string.Empty;
            if (input.documento.protocollatore != null && input.documento.protocollo != null && !string.IsNullOrEmpty(input.documento.protocollo.numero))
            {
                if (input.documento.creatoreDocumento != null)
                {
                    idCorrGlobaliRuolo = input.documento.creatoreDocumento.idCorrGlob_Ruolo;
                }
                else
                    idCorrGlobaliRuolo = input.documento.protocollatore.ruolo_idCorrGlobali;
            }
            else
            {
                idCorrGlobaliRuolo = input.documento.creatoreDocumento.idCorrGlob_Ruolo;
            }
            corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliRuolo, TipoUtente.INTERNO,input.infoUtente);
            return corr;
        }

        private static Corrispondente UOPDoc(CorrDocInput input)
        {
            Corrispondente corr = new Corrispondente();
            string idCorrGlobaliUo = string.Empty;
            if (input.documento.protocollatore != null && input.documento.protocollo != null && !string.IsNullOrEmpty(input.documento.protocollo.numero))
            {
                if (input.documento.creatoreDocumento != null)
                {
                    idCorrGlobaliUo = input.documento.creatoreDocumento.idCorrGlob_UO;
                }
                else
                    idCorrGlobaliUo = input.documento.protocollatore.uo_idCorrGlobali;
            }
            else
            {
                idCorrGlobaliUo = input.documento.creatoreDocumento.idCorrGlob_UO;
            }
            corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliUo, TipoUtente.INTERNO,input.infoUtente);
            return corr;
        }

        private static Corrispondente RSPPDoc(CorrDocInput input)
        {
            Corrispondente corr = new Corrispondente();
            string idCorrGlobaliUo = string.Empty;
            string idCorr = string.Empty;
            if (input.documento.protocollatore != null && input.documento.protocollo != null && !string.IsNullOrEmpty(input.documento.protocollo.numero))
            {
                if (input.documento.creatoreDocumento != null)
                {
                    idCorrGlobaliUo = input.documento.creatoreDocumento.idCorrGlob_UO;
                }
                else
                {
                    idCorrGlobaliUo = input.documento.protocollatore.uo_idCorrGlobali;
                }
            }
            else
            {
                idCorrGlobaliUo = input.documento.creatoreDocumento.idCorrGlob_UO;
            }
            idCorr = input.ruolo.systemId;
            string idCorrGlobaliRuoloRespUo = BusinessLogic.Utenti.UserManager.getRuoloRespUofromUo(idCorrGlobaliUo, "R", idCorr);

            if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
            {
                corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliRuoloRespUo, TipoUtente.INTERNO,input.infoUtente);
            }
            else
            {
                corr = null;
            }
            return corr;
        }

        private static Corrispondente RSDoc(CorrDocInput input)
        {
            Corrispondente corr = new Corrispondente();
            string idCorrGlobaliUo = string.Empty;
            string idCorr = String.Empty;
            if (input.documento.protocollatore != null && input.documento.protocollo != null && !string.IsNullOrEmpty(input.documento.protocollo.numero))
            {
                if (input.documento.creatoreDocumento != null)
                {
                    idCorrGlobaliUo = input.documento.creatoreDocumento.idCorrGlob_UO;
                }
                else
                {
                    idCorrGlobaliUo = input.documento.protocollatore.uo_idCorrGlobali;
                }
            }
            else
            {
                idCorrGlobaliUo = input.documento.creatoreDocumento.idCorrGlob_UO;
            }
            idCorr = input.ruolo.systemId;
            string idCorrGlobaliRuoloRespUo = BusinessLogic.Utenti.UserManager.getRuoloRespUofromUo(idCorrGlobaliUo, "S", idCorr);

            if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
            {
                corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliRuoloRespUo, TipoUtente.INTERNO,input.infoUtente);
            }
            else
            {
                corr = null;
            }
            return corr;
        }

        private static Corrispondente RSPMDoc(CorrDocInput input)
        {
            Corrispondente corr = new Corrispondente();
            string idCorrGlobaliUo = input.ruolo.uo.systemId;
            string idCorr = input.ruolo.systemId;

            string idCorrGlobaliRuoloRespUo = BusinessLogic.Utenti.UserManager.getRuoloRespUofromUo(idCorrGlobaliUo, "R", idCorr);

            if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
            {
                corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliRuoloRespUo, TipoUtente.INTERNO,input.infoUtente);
            }
            else
            {
                corr = null;
            }
            return corr;
        }

        private static Corrispondente SMDoc(CorrDocInput input)
        {
            Corrispondente corr = new Corrispondente();
            string idCorrGlobaliUo = input.ruolo.uo.systemId;
            string idCorr = input.ruolo.systemId;
            string idCorrGlobaliRuoloRespUo = BusinessLogic.Utenti.UserManager.getRuoloRespUofromUo(idCorrGlobaliUo, "S", idCorr);

            if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
            {
                corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliRuoloRespUo, TipoUtente.INTERNO,input.infoUtente);
            }
            else
            {
                corr = null;
            }
            return corr;
        }

        #endregion
        #region Fascicolo

        private static Corrispondente UTPFasc(CorrFascInput input)
        {
            Corrispondente corr = new Corrispondente();
            string utenteProprietario = string.Empty;
            utenteProprietario = input.fascicolo.creatoreFascicolo.idPeople;
            corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByIdPeople(utenteProprietario, TipoUtente.INTERNO,input.infoUtente);
            return corr;
        }

        private static Corrispondente RPFasc(CorrFascInput input)
        {
            Corrispondente corr = new Corrispondente();
            string idCorrGlobaliUo = string.Empty;
            idCorrGlobaliUo = input.fascicolo.creatoreFascicolo.idCorrGlob_UO;
            corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliUo, TipoUtente.INTERNO, input.infoUtente);
            return corr;
        }

        private static Corrispondente UOPFasc(CorrFascInput input)
        {
            Corrispondente corr = new Corrispondente();
            string idCorrGlobaliUo = string.Empty;
            idCorrGlobaliUo = input.fascicolo.creatoreFascicolo.idCorrGlob_UO;
            corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliUo, TipoUtente.INTERNO, input.infoUtente);
            return corr;
        }

        private static Corrispondente RSPPFasc(CorrFascInput input)
        {
            Corrispondente corr = new Corrispondente();
            string idCorrGlobaliUo = string.Empty;
            string idCorr = string.Empty;

            idCorrGlobaliUo = input.fascicolo.creatoreFascicolo.idCorrGlob_UO;
            idCorr = input.fascicolo.creatoreFascicolo.idCorrGlob_Ruolo;
            string idCorrGlobaliRuoloRespUo = BusinessLogic.Utenti.UserManager.getRuoloRespUofromUo(idCorrGlobaliUo, "R", idCorr);

            if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
            {
                corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliRuoloRespUo, TipoUtente.INTERNO,input.infoUtente);
            }
            else
            {
                corr = null;
            }
            return corr;
        }

        private static Corrispondente RSFasc(CorrFascInput input)
        {
            Corrispondente corr = new Corrispondente();
            string idCorrGlobaliUo = string.Empty;
            string idCorr = string.Empty;

            idCorrGlobaliUo = input.fascicolo.creatoreFascicolo.idCorrGlob_UO;
            idCorr = input.fascicolo.creatoreFascicolo.idCorrGlob_Ruolo;
            string idCorrGlobaliRuoloRespUo = BusinessLogic.Utenti.UserManager.getRuoloRespUofromUo(idCorrGlobaliUo, "S", idCorr);
            if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
            {
                corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliRuoloRespUo, TipoUtente.INTERNO,input.infoUtente);
            }
            else
            {
                corr = null;
            }
            return corr;
        }

        private static Corrispondente RSPMFasc(CorrFascInput input)
        {
            Corrispondente corr = new Corrispondente();
            string idCorrGlobaliUo = input.ruolo.uo.systemId;
            string idCorr = input.ruolo.systemId;
            string idCorrGlobaliRuoloRespUo = BusinessLogic.Utenti.UserManager.getRuoloRespUofromUo(idCorrGlobaliUo, "R", idCorr);
            if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
            {
                corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliRuoloRespUo, TipoUtente.INTERNO,input.infoUtente);
            }
            else
            {
                corr = null;
            }
            return corr;
        }

        private static Corrispondente SMFasc(CorrFascInput input)
        {
            Corrispondente corr = new Corrispondente();
            string idCorrGlobaliUo = input.ruolo.uo.systemId;
            string idCorr = input.ruolo.systemId;
            string idCorrGlobaliRuoloRespUo = BusinessLogic.Utenti.UserManager.getRuoloRespUofromUo(idCorrGlobaliUo, "S", idCorr);
            if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
            {
                corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliRuoloRespUo, TipoUtente.INTERNO,input.infoUtente);
            }
            else
            {
                corr = null;
            }
            return corr;
        }
        #endregion
    }
}