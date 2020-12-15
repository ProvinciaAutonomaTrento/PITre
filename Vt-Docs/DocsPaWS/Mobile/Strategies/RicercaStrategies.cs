using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocsPaVO.Mobile.Requests;
using DocsPaVO.Mobile.Responses;
using DocsPaVO.Mobile;
using System.Collections;
using DocsPaVO.documento;
using DocsPaVO.ricerche;
using DocsPaVO.filtri;
using DocsPaVO.utente;
using DocsPaVO.fascicolazione;
using DocsPaWS.Mobile.Decorators;
using DocsPaVO.filtri.ricerca;
using log4net;
using DocsPaVO.amministrazione;

namespace DocsPaWS.Mobile.Ricerca
{
    public abstract class RicercaStrategy
    {
        public abstract bool canSolve(RicercaRequest request);
        public abstract void buildResponse(RicercaRequest request,RicercaResponse response);
        protected string buildMethodName = "buildResponse";
        private static RicercaStrategy[] _allStrategies= new RicercaStrategy[] { 
            new RicercaSalvataFascicoloStrategy(), 
            new RicercaSalvataDocumentoStrategy(),
            new RicercaTestoFascicoloStrategy(),
            new RicercaAdlTestoFascicoloStrategy(),
            new RicercaTestoDocumentoStrategy(),
            new RicercaAdlTestoDocumentoStrategy(),
            new RicercaTestoDocFascStrategy(),
            new FolderContentStrategy() };

        public static RicercaStrategy GetStrategy(RicercaRequest request)
        {
            foreach (RicercaStrategy strategy in _allStrategies)
            {
                if (strategy.canSolve(request)) return strategy;
            }
            return null;
        }

        protected FiltroRicerca[][] getFiltri(string input)
        {
            Type t = typeof(ArrayOfArrayOfFiltroRicerca);
            System.Xml.Serialization.XmlSerializer _serializer = new System.Xml.Serialization.XmlSerializer(t);
            System.IO.StringReader sr = new System.IO.StringReader(input);

            System.Xml.XmlTextReader _reader = new System.Xml.XmlTextReader(sr);
            ArrayOfArrayOfFiltroRicerca res = (ArrayOfArrayOfFiltroRicerca)_serializer.Deserialize(_reader);
            return res.Filtri;
        }
    }

    #region Strategies per le ricerche salvate
    internal class RicercaSalvataFascicoloStrategy : RicercaStrategy
    {
        private ILog logger = LogManager.GetLogger(typeof(RicercaSalvataFascicoloStrategy));

        public override bool canSolve(RicercaRequest request)
        {
            return !string.IsNullOrEmpty(request.IdRicercaSalvata) && (request.TypeRicercaSalvata == RicercaSalvataType.RIC_FASCICOLO) && string.IsNullOrEmpty(request.ParentFolderId);
        }

        public override void buildResponse(RicercaRequest request, RicercaResponse response)
        {
            logger.Debug("ricerca di tipo fascicolo");
            int numRec;
            int numTotPage;
            SearchItem si = BusinessLogic.Documenti.InfoDocManager.GetSearchItem(Int32.Parse(request.IdRicercaSalvata));
            FiltroRicerca[][] filtri = getFiltri(si.filtri);
            InfoUtente iu = request.UserInfo.InfoUtente;
            iu.idGruppo = request.IdGruppo;
            iu.idCorrGlobali = request.IdCorrGlobali;

            List<SearchResultInfo> idProjectList = null;

            ArrayList result = BusinessLogic.Fascicoli.FascicoloManager.getListaFascicoliPaging(iu, null, null, filtri[0], request.EnableUfficioRef, request.EnableProfilazione, true, out numTotPage, out  numRec, request.RequestedPage, request.PageSize, false, out idProjectList, null, string.Empty);
            response.TotalRecordCount = numRec;
            response.Risultati = new List<RicercaElement>();
            foreach (Fascicolo temp in result)
            {
                response.Risultati.Add(RicercaElement.buildInstance(temp));
            }
        }
    }

    internal class RicercaSalvataDocumentoStrategy : RicercaStrategy
    {
        private ILog logger = LogManager.GetLogger(typeof(RicercaSalvataDocumentoStrategy));
        public override bool canSolve(RicercaRequest request)
        {
            return !string.IsNullOrEmpty(request.IdRicercaSalvata) && (request.TypeRicercaSalvata == RicercaSalvataType.RIC_DOCUMENTO);
        }

        public override void buildResponse(RicercaRequest request, RicercaResponse response)
        {
            int numRec;
            int numTotPage;
            List<SearchResultInfo> idProfileList = new List<SearchResultInfo>();
            SearchItem si = BusinessLogic.Documenti.InfoDocManager.GetSearchItem(Int32.Parse(request.IdRicercaSalvata));
            FiltroRicerca[][] filtri = getFiltri(si.filtri);
            ArrayList result = BusinessLogic.Documenti.InfoDocManager.getQueryPaging(request.IdGruppo, request.UserInfo.IdPeople, filtri, false, request.RequestedPage, request.PageSize, true, out numTotPage, out numRec, false, out idProfileList,false);
            response.TotalRecordCount = numRec;
            response.Risultati = new List<RicercaElement>();
            foreach (InfoDocumento temp in result)
            {
                response.Risultati.Add(RicercaElement.buildInstance(temp));
            }
        }

    }
    #endregion
    #region Strategies per le ricerche testuali
    internal class RicercaTestoFascicoloStrategy : RicercaStrategy
    {
        private ILog logger = LogManager.GetLogger(typeof(RicercaTestoFascicoloStrategy));

        public override bool canSolve(RicercaRequest request)
        {
            return string.IsNullOrEmpty(request.IdRicercaSalvata) && (request.TypeRicerca== RicercaType.RIC_FASCICOLO) && string.IsNullOrEmpty(request.ParentFolderId);
        }

        public override void buildResponse(RicercaRequest request, RicercaResponse response)
        {
            logger.Debug("ricerca di fascicolo per testo");
            int numRec;
            int numTotPage;

            if (string.IsNullOrEmpty(request.Text))
            {
                response.TotalRecordCount = 0;
                response.Risultati = new List<RicercaElement>();
            }
            else
            {
                FiltroRicerca[] filtri = new FiltroRicerca[1];
                filtri[0] = new FiltroRicerca();
                filtri[0].argomento = DocsPaVO.filtri.fascicolazione.listaArgomenti.TITOLO.ToString();
                filtri[0].valore = request.Text;



                //Filtro ricerca temporale -1anno
                /*FiltroRicerca fascDataPrec = new FiltroRicerca();
                fascDataPrec.argomento = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_PRECEDENTE_IL.ToString();
                fascDataPrec.valore = DateTime.Now.Date.AddDays(1).ToShortDateString();
                FiltroRicerca fascDataSuccAl = new FiltroRicerca();
                fascDataSuccAl.argomento = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_SUCCESSIVA_AL.ToString();
                fascDataSuccAl.valore = DateTime.Now.AddYears(-1).Date.ToShortDateString();

                filtri[1] = fascDataSuccAl;
                filtri[2] = fascDataPrec;
                */

                InfoUtente iu = request.UserInfo.InfoUtente;
                iu.idGruppo = request.IdGruppo;
                iu.idCorrGlobali = request.IdCorrGlobali;

                List<SearchResultInfo> idProjectList = null;
                logger.Debug("testo da cercare: " + request.Text);
                ArrayList result = BusinessLogic.Fascicoli.FascicoloManager.getListaFascicoliPaging(iu, null, null, filtri, request.EnableUfficioRef, request.EnableProfilazione, true, out numTotPage, out  numRec, request.RequestedPage, request.PageSize, false, out idProjectList, null, string.Empty);
                logger.Debug("numero totale risultati: " + numRec);
                response.TotalRecordCount = numRec;
                response.Risultati = new List<RicercaElement>();
                foreach (Fascicolo temp in result)
                {
                    response.Risultati.Add(RicercaElement.buildInstance(temp));
                }
            }
        }
    }

    internal class RicercaAdlTestoFascicoloStrategy : RicercaStrategy
    {
        private ILog logger = LogManager.GetLogger(typeof(RicercaAdlTestoFascicoloStrategy));

        public override bool canSolve(RicercaRequest request)
        {
            return string.IsNullOrEmpty(request.IdRicercaSalvata) && (request.TypeRicerca == RicercaType.RIC_FASCICOLO_ADL ) && string.IsNullOrEmpty(request.ParentFolderId);
        }

        public override void buildResponse(RicercaRequest request, RicercaResponse response)
        {
            logger.Debug("ricerca in Adl di fascicolo per testo");
            int numRec;
            int numTotPage;
            FiltroRicerca[] filtri = new FiltroRicerca[3];
            filtri[0] = new FiltroRicerca();
            filtri[0].argomento = DocsPaVO.filtri.fascicolazione.listaArgomenti.TITOLO.ToString();
            filtri[0].valore = request.Text;

            /*
            //Filtro ricerca temporale -1anno
            FiltroRicerca fascDataPrec = new FiltroRicerca();
            fascDataPrec.argomento = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_PRECEDENTE_IL.ToString();
            fascDataPrec.valore = DateTime.Now.Date.ToShortDateString();
            FiltroRicerca fascDataSuccAl = new FiltroRicerca();
            fascDataSuccAl.argomento = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_SUCCESSIVA_AL.ToString();
            fascDataSuccAl.valore = DateTime.Now.AddYears(-1).Date.ToShortDateString();

            filtri[1] = fascDataSuccAl;
            filtri[2] = fascDataPrec;
            */


            FiltroRicerca fascInAdl = new FiltroRicerca();
            fascInAdl.argomento = DocsPaVO.filtri.fascicolazione.listaArgomenti.DOC_IN_FASC_ADL.ToString();
            fascInAdl.valore = String.Format("{0}@{1}", request.UserInfo.IdPeople, request.IdCorrGlobali);
            filtri[1] = fascInAdl;


            //filtro per titolari come nel frontend vero.
            List<OrgTitolario> titolari = BusinessLogic.Amministrazione.TitolarioManager.getTitolariUtilizzabili(request.UserInfo.IdAmministrazione).Cast<OrgTitolario>().ToList();
            string listaTitolari = string.Empty;
            foreach (OrgTitolario tit in titolari)
                listaTitolari += "," + tit.ID.ToString();

            listaTitolari = listaTitolari.Substring(1);
            FiltroRicerca titInAdl = new FiltroRicerca();
            titInAdl.argomento = DocsPaVO.filtri.fascicolazione.listaArgomenti.ID_TITOLARIO.ToString();
            titInAdl.valore = listaTitolari;
            filtri[2] = titInAdl;


            InfoUtente iu = request.UserInfo.InfoUtente;
            iu.idGruppo = request.IdGruppo;
            iu.idCorrGlobali = request.IdCorrGlobali;

            List<SearchResultInfo> idProjectList = null;
            logger.Debug("testo da cercare: " + request.Text);
            ArrayList result = BusinessLogic.Fascicoli.FascicoloManager.getListaFascicoliPaging(iu, null, null, filtri, request.EnableUfficioRef, request.EnableProfilazione, true, out numTotPage, out  numRec, request.RequestedPage, request.PageSize, false, out idProjectList, null, string.Empty);
            logger.Debug("numero totale risultati: " + numRec);
            response.TotalRecordCount = numRec;
            response.Risultati = new List<RicercaElement>();
            foreach (Fascicolo temp in result)
            {
                response.Risultati.Add(RicercaElement.buildInstance(temp));
            }
        }
    }

    internal class RicercaTestoDocumentoStrategy : RicercaStrategy
    {
        private ILog logger = LogManager.GetLogger(typeof(RicercaTestoDocumentoStrategy));

        public override bool canSolve(RicercaRequest request)
        {
            return string.IsNullOrEmpty(request.IdRicercaSalvata) && (request.TypeRicerca == RicercaType.RIC_DOCUMENTO) && string.IsNullOrEmpty(request.ParentFolderId);
        }

        public override void buildResponse(RicercaRequest request, RicercaResponse response)
        {
            logger.Debug("ricerca di documento per testo");
            int numRec;
            int numTotPage;


            if (string.IsNullOrEmpty(request.Text))
            {
                response.TotalRecordCount = 0;
                response.Risultati = new List<RicercaElement>();
            }
            else
            {
                FiltroRicerca[][] filtriArray = new FiltroRicerca[1][];
                List<FiltroRicerca> filtri = new List<FiltroRicerca>(); ;
                if (!string.IsNullOrEmpty(request.Text))
                {
                    FiltroRicerca fOgg = new FiltroRicerca();
                    fOgg.argomento = listaArgomenti.OGGETTO.ToString();
                    fOgg.valore = request.Text;//
                    filtri.Add(fOgg);
                    /*
                    FiltroRicerca fOggAnno = new FiltroRicerca();
                    fOggAnno.argomento = listaArgomenti.ANNO_PROTOCOLLO.ToString();
                    fOggAnno.valore = System.DateTime.Now.Year.ToString();
                    filtri.Add(fOggAnno);
                     */
                }
                listaArgomenti[] defaultArgomenti = new listaArgomenti[]{
                    listaArgomenti.PROT_ARRIVO,
                    listaArgomenti.PROT_PARTENZA,
                    listaArgomenti.PROT_INTERNO,
                    listaArgomenti.GRIGIO,
                    listaArgomenti.PREDISPOSTO
                };
                foreach (listaArgomenti arg in defaultArgomenti)
                {
                    FiltroRicerca temp = new FiltroRicerca();
                    temp.argomento = arg.ToString();
                    temp.valore = "true";
                    filtri.Add(temp);
                }


                //Filtro ricerca temporale -1anno
                FiltroRicerca fDataPrec = new FiltroRicerca();
                fDataPrec.argomento = listaArgomenti.DATA_CREAZIONE_PRECEDENTE_IL.ToString();
                fDataPrec.valore = DateTime.Now.Date.AddDays(1).ToString ("dd/MM/yyyy");
                FiltroRicerca fDataSuccAl = new FiltroRicerca();
                fDataSuccAl.argomento = listaArgomenti.DATA_CREAZIONE_SUCCESSIVA_AL.ToString();
                fDataSuccAl.valore = DateTime.Now.AddDays(-31).Date.ToString ("dd/MM/yyyy");


                filtri.Add(fDataPrec);
                filtri.Add(fDataSuccAl);

                filtriArray[0] = filtri.ToArray();
                InfoUtente iu = request.UserInfo.InfoUtente;
                iu.idGruppo = request.IdGruppo;
                iu.idCorrGlobali = request.IdCorrGlobali;
                List<SearchResultInfo> idProfileList = new List<SearchResultInfo>();
                ArrayList result = BusinessLogic.Documenti.InfoDocManager.getQueryPaging(request.IdGruppo, request.UserInfo.IdPeople, filtriArray, false, request.RequestedPage, request.PageSize, true, out numTotPage, out numRec, false, out idProfileList, false);
                response.TotalRecordCount = numRec;
                response.Risultati = new List<RicercaElement>();
                foreach (InfoDocumento temp in result)
                {
                    response.Risultati.Add(RicercaElement.buildInstance(temp));
                }
            }
        }
    }


    internal class RicercaAdlTestoDocumentoStrategy : RicercaStrategy
    {
        private ILog logger = LogManager.GetLogger(typeof(RicercaAdlTestoDocumentoStrategy));

        public override bool canSolve(RicercaRequest request)
        {
            return string.IsNullOrEmpty(request.IdRicercaSalvata) && (request.TypeRicerca == RicercaType.RIC_DOCUMENTO_ADL) && string.IsNullOrEmpty(request.ParentFolderId);
        }

        public override void buildResponse(RicercaRequest request, RicercaResponse response)
        {
            logger.Debug("ricerca in Adl di documento per testo");
            int numRec;
            int numTotPage;
            FiltroRicerca[][] filtriArray = new FiltroRicerca[1][];
            List<FiltroRicerca> filtri = new List<FiltroRicerca>(); ;
            if (!string.IsNullOrEmpty(request.Text))
            {
                FiltroRicerca fOgg = new FiltroRicerca();
                fOgg.argomento = listaArgomenti.OGGETTO.ToString();
                fOgg.valore = request.Text;//
                filtri.Add(fOgg);
                /*
                FiltroRicerca fOggAnno = new FiltroRicerca();
                fOggAnno.argomento = listaArgomenti.ANNO_PROTOCOLLO.ToString();
                fOggAnno.valore = System.DateTime.Now.Year.ToString();
                filtri.Add(fOggAnno);
                 */
            }
            listaArgomenti[] defaultArgomenti = new listaArgomenti[]{
                listaArgomenti.PROT_ARRIVO,
                listaArgomenti.PROT_PARTENZA,
                listaArgomenti.PROT_INTERNO,
                listaArgomenti.GRIGIO,
                listaArgomenti.PREDISPOSTO
            };
            foreach (listaArgomenti arg in defaultArgomenti)
            {
                FiltroRicerca temp = new FiltroRicerca();
                temp.argomento = arg.ToString();
                temp.valore = "true";
                filtri.Add(temp);
            }

            //Per l'adl magari non mettiamo il limete a un anno
            /*
            //Filtro ricerca temporale -1anno
            FiltroRicerca fDataPrec = new FiltroRicerca();
            fDataPrec.argomento = listaArgomenti.DATA_CREAZIONE_PRECEDENTE_IL.ToString();
            fDataPrec.valore = DateTime.Now.Date.ToShortDateString();
            FiltroRicerca fDataSuccAl = new FiltroRicerca();
            fDataSuccAl.argomento = listaArgomenti.DATA_CREAZIONE_SUCCESSIVA_AL.ToString();
            fDataSuccAl.valore = DateTime.Now.AddYears(-1).Date.ToShortDateString();


            filtri.Add(fDataPrec);
            filtri.Add(fDataSuccAl);
            */

            //Filtro ADL
//            DOC_IN_ADL
            //String.Format("{0}@{1}", infoUt.idPeople, ruolo.systemId),
            FiltroRicerca fAdldoc = new FiltroRicerca();
            fAdldoc.argomento = listaArgomenti.DOC_IN_ADL.ToString();
            Ruolo  AdlRuolo= BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo (request.IdGruppo) ;
            fAdldoc.valore = String.Format("{0}@{1}", request.UserInfo.IdPeople, request.IdCorrGlobali);
            filtri.Add(fAdldoc);

            //Filtro per Registro attivo:
            string listaRegistri = string.Empty;
            foreach (DocsPaVO.utente.Registro reg in AdlRuolo.registri)
            {
                if (!reg.flag_pregresso)
                {
                    listaRegistri += "," + reg.systemId.ToString();
                }

            }
            listaRegistri = listaRegistri.Substring(1);


            FiltroRicerca fAdlReg = new FiltroRicerca();
            fAdlReg.argomento = listaArgomenti.REGISTRO.ToString();
            fAdlReg.valore = listaRegistri;
            filtri.Add(fAdlReg);


            filtriArray[0] = filtri.ToArray();
            InfoUtente iu = request.UserInfo.InfoUtente;
            iu.idGruppo = request.IdGruppo;
            iu.idCorrGlobali = request.IdCorrGlobali;
            List<SearchResultInfo> idProfileList = new List<SearchResultInfo>();
            ArrayList result = BusinessLogic.Documenti.InfoDocManager.getQueryPaging(request.IdGruppo, request.UserInfo.IdPeople, filtriArray, false, request.RequestedPage, request.PageSize, true, out numTotPage, out numRec, false, out idProfileList, false);
            response.TotalRecordCount = numRec;
            response.Risultati = new List<RicercaElement>();
            foreach (InfoDocumento temp in result)
            {
                response.Risultati.Add(RicercaElement.buildInstance(temp));
            }
        }
    }



    internal class RicercaTestoDocFascStrategy : RicercaStrategy
    {
        private ILog logger = LogManager.GetLogger(typeof(RicercaTestoDocFascStrategy));

        public override bool canSolve(RicercaRequest request)
        {
            return string.IsNullOrEmpty(request.IdRicercaSalvata) && (request.TypeRicerca == RicercaType.RIC_DOC_FASC) && string.IsNullOrEmpty(request.ParentFolderId);
        }

        public override void buildResponse(RicercaRequest request, RicercaResponse response)
        {
            int numRec;
            int numTotPage;
            

            //Filtro ricerca temporale -1anno
            FiltroRicerca fascDataPrec = new FiltroRicerca();
            fascDataPrec.argomento = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_PRECEDENTE_IL.ToString();
            fascDataPrec.valore = DateTime.Now.Date.AddDays(1).ToString ("dd/MM/yyyy");
            FiltroRicerca fascDataSuccAl = new FiltroRicerca();
            fascDataSuccAl.argomento = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_SUCCESSIVA_AL.ToString();
            fascDataSuccAl.valore = DateTime.Now.AddYears(-1).Date.ToString("dd/MM/yyyy");



            logger.Debug("ricerca di documenti e fascicoli per testo");
            List<RicercaElement> risultati = new List<RicercaElement>();
            FiltroRicerca[] filtriFasc = new FiltroRicerca[3];
            filtriFasc[0] = new FiltroRicerca();
            filtriFasc[0].argomento = DocsPaVO.filtri.fascicolazione.listaArgomenti.TITOLO.ToString();
            filtriFasc[0].valore = request.Text;
            filtriFasc[1] = fascDataSuccAl;
            filtriFasc[2] = fascDataPrec;
            InfoUtente iu = request.UserInfo.InfoUtente;
            iu.idGruppo = request.IdGruppo;
            iu.idCorrGlobali = request.IdCorrGlobali;
            logger.Debug("testo da cercare: " + request.Text);
            List<SearchResultInfo> idProjectList = null;
            //ArrayList resultFasc = BusinessLogic.Fascicoli.FascicoloManager.getListaFascicoliPaging(iu, null, null, filtriFasc, request.EnableUfficioRef, request.EnableProfilazione, true, out numTotPage, out  numRec, request.RequestedPage, request.PageSize, false, out idProjectList, null, string.Empty);
            
            
            
            ArrayList resultFasc = BusinessLogic.Fascicoli.FascicoloManager.getListaFascicoli(iu, null, filtriFasc, request.EnableUfficioRef, request.EnableProfilazione, true,null,null,null);
            logger.Debug("numero totale risultati: " + resultFasc.Count);
            foreach (Fascicolo temp in resultFasc)
            {
                risultati.Add(RicercaElement.buildInstance(temp));
            }
            FiltroRicerca[][] filtriArrayDoc = new FiltroRicerca[1][];
            List<FiltroRicerca> filtriDoc = new List<FiltroRicerca>(); ;
           
            if (!string.IsNullOrEmpty(request.Text))
            {
                FiltroRicerca fOgg = new FiltroRicerca();
                fOgg.argomento = listaArgomenti.OGGETTO.ToString();
                fOgg.valore = request.Text;
                filtriDoc.Add(fOgg);
            }
            listaArgomenti[] defaultArgomenti = new listaArgomenti[]{
                listaArgomenti.PROT_ARRIVO,
                listaArgomenti.PROT_PARTENZA,
                listaArgomenti.PROT_INTERNO,
                listaArgomenti.GRIGIO,
                listaArgomenti.PREDISPOSTO
            };
            foreach (listaArgomenti arg in defaultArgomenti)
            {
                FiltroRicerca temp = new FiltroRicerca();
                temp.argomento = arg.ToString();
                temp.valore = "true";
                filtriDoc.Add(temp);
            }


            //Filtro ricerca temporale -1anno
            FiltroRicerca fDataPrec = new FiltroRicerca();
            fDataPrec.argomento = listaArgomenti.DATA_CREAZIONE_PRECEDENTE_IL.ToString();
            fDataPrec.valore = DateTime.Now.Date.AddDays(1).ToString("dd/MM/yyyy");
            FiltroRicerca fDataSuccAl = new FiltroRicerca();
            fDataSuccAl.argomento = listaArgomenti.DATA_CREAZIONE_SUCCESSIVA_AL.ToString();
            fDataSuccAl.valore = DateTime.Now.AddYears(-1).Date.ToString("dd/MM/yyyy");

            filtriDoc.Add(fDataPrec);
            filtriDoc.Add(fDataSuccAl);
            List<SearchResultInfo> idProfileList = new List<SearchResultInfo>();

            filtriArrayDoc[0] = filtriDoc.ToArray();
            //ArrayList resultDoc = BusinessLogic.Documenti.InfoDocManager.getQueryPaging(request.IdGruppo, request.UserInfo.IdPeople, filtriArrayDoc, true, request.RequestedPage, request.PageSize, true, out numTotPage, out numRec, false, out idProfileList, false);
            ArrayList resultDoc = BusinessLogic.Documenti.InfoDocManager.getQuery(request.IdGruppo, request.UserInfo.IdPeople, filtriArrayDoc);
            foreach (InfoDocumento temp in resultDoc)
            {
                risultati.Add(RicercaElement.buildInstance(temp));
            }
            risultati.Sort(new RicercaElementComparer());
            PaginatorDecorator<RicercaElement> pag = new PaginatorDecorator<RicercaElement>(request.RequestedPage, request.PageSize, risultati);
            response.Risultati = pag.execute();
            response.TotalRecordCount = pag.TotalResultCount;
        }
    }
    #endregion

    internal class FolderContentStrategy : RicercaStrategy
    {
        private ILog logger = LogManager.GetLogger(typeof(FolderContentStrategy));

        public override bool canSolve(RicercaRequest request)
        {
            return !string.IsNullOrEmpty(request.ParentFolderId);
        }

        public override void buildResponse(RicercaRequest request, RicercaResponse response)
        {
            logger.Debug("contenuto del folder con id " + request.ParentFolderId);
            FascicoloContentDecorator<RicercaElement> fcDec = new FascicoloContentDecorator<RicercaElement>(request.UserInfo.IdPeople, request.IdGruppo, request.ParentFolderId, request.FascId, RicercaElement.buildInstance, RicercaElement.buildInstance);
            PaginatorDecorator<RicercaElement> pag = new PaginatorDecorator<RicercaElement>(request.RequestedPage, request.PageSize, fcDec);
            response.Risultati = pag.execute();
            response.TotalRecordCount = pag.TotalResultCount;
        }
    }

    internal class RicercaElementComparer : IComparer<RicercaElement>
    {

        public int Compare(RicercaElement x, RicercaElement y)
        {
            if (x.Oggetto == null) return -1;
            if (y.Oggetto == null) return 1;
            return x.Oggetto.CompareTo(y.Oggetto);
        }
    }
}