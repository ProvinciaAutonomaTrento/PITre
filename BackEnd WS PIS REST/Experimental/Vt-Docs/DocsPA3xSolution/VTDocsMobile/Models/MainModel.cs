using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VTDocsMobile.VTDocsWSMobile;
using System.Runtime.Serialization;


namespace VTDocs.mobile.fe.model
{
    public class MainModel
    {
        public Tab TabShow { get; set; }
        public string PreviousTabName { get; set; }
        public string PreviousTabName2 { get; set; }
        public ITabModel TabModel { get; set; }
        public int ToDoListTotalElements { get; set; }
        public string DescrUtente { get; set; }
        public string IdRuolo { get; set; }
        public string DescrRuolo { get; set; }
        public bool SessionExpired { get; set; }
        public List<string> Errori { get; set; }
        public List<Ruolo> Ruoli { get; set; }
        public ITabModel PreviousTabModel { get; set; }
        public ITabModel PreviousTabModel2 { get; set; }
        public int NumDeleghe { get; set; }
        public Delega DelegaEsercitata { get; set; }
        public bool HSMSignResult { get; set; } // risultato della firma HSM
        public bool HSMRequestOTPResult { get; set; } // risultato della richiesta OTP
        public bool HSMVerifySignResult { get; set; } // risultato verifica documento firmato
        public bool HSMIsAllowedOTP { get; set; } // gestisce l'abilitazione delle richieste OTP
        public InfoDocFirmato InfoDocFirme { get; set; }
        public Memento HSMMemento { get; set; }
        public bool IsAutorizedLF { get; set; }
    }

    public enum Tab
    {
        TODO_LIST,
        DETTAGLIO_DF_TRASM,
        DETTAGLIO_DOC,
        DETTAGLIO_FASC,
        RICERCA,
        TRASMISSIONE,
        LISTA_DELEGHE,
        CREA_DELEGA,
        SMISTAMENTO,
        AREA_DI_LAVORO,
        LIBRO_FIRMA
    }

    public interface ITabModel
    {

    }

    public abstract class PaginateModel : ITabModel{
        protected int _numTotResults;
        protected int _numResForPage;

        public PaginateModel(int numTotResults, int numResForPage){

            _numTotResults = numTotResults;
            _numResForPage = numResForPage;
        }

        public int NumElements
        {
            get;
            set;
        }

        public int NumResultsForPage
        {
            get
            {
                return _numResForPage;
            }
            
        }
        public string IdParent
        {
            get;
            set;
        }

        public string NomeParent
        {
            get;
            set;
        }

        public int NumTotPages
        {
            get
            {
                if (_numTotResults == 0)
                {
                    return 0;
                }
                else
                {
                    int res;
                    res = _numTotResults / _numResForPage;
                    //controllo che non ci sia resto altrimenti aggiungo una pagina
                    if (_numTotResults % _numResForPage > 0)
                    {
                        res++;
                    }
                    return res;
                }

            }
        }

        public int CurrentPage
        {
            get;
            set;
        }

}

    public class ToDoListModel : PaginateModel
    {

        public ToDoListModel(int numTotResults, int numResForPage) : base(numTotResults,numResForPage)
        {
        }

        public ToDoListElement[] ToDoListElements { get; set; }

        public bool SetDataVista;

    }

    public class RicercaModel : PaginateModel
    {
        public RicercaModel(int numTotResults, int numResForPage) : base(numTotResults,numResForPage)
        {
        }

        public RicercaSalvata[] Ricerche
        {
            get;
            set;
        }

        public RicercaElement[] Risultati
        {
            get;
            set;
        }

        public string IdRicercaSalvata
        {
            get;
            set;
        }

        public RicercaSalvataType TypeRicercaSalvata
        {
            get;
            set;
        }

        public string Testo
        {
            get;
            set;
        }

        public RicercaType TypeRicerca
        {
            get;
            set;
        }

    }

    public class AdlModel : PaginateModel
    {
        public AdlModel(int numTotResults, int numResForPage)
            : base(numTotResults, numResForPage)
        {
        }

        public string RicercaInAdl 
        {
            get;
            set;
        }

        public RicercaElement[] Risultati
        {
            get;
            set;
        }

        public string Testo
        {
            get;
            set;
        }

        public RicercaType TypeRicerca
        {
            get;
            set;
        }

    }

    public class TrasmissioneModel : ITabModel
    {
        public ModelloTrasm[] ModelliTrasm
        {
            get;
            set;
        }

        public DocInfo DocInfo
        {
            get;
            set;
        }

        public FascInfo FascInfo
        {
            get;
            set;
        }

        public string IdTrasmPerAcc
        {
            get;
            set;
        }

    }

    public class OperationResponse
    {
        public bool Success
        {
            get;
            set;
        }

        public string Error
        {
            get;
            set;
        }
    }

    public class ListaDelegheModel : ITabModel
    {
        public Delega[] DelegheRicevute { get; set; }
        public Delega[] DelegheAssegnate { get; set; }
        //Mev Deleghe impostate
        public Delega[] DelegheImpostate { get; set; }

    }

    public class CreaDelegaModel : ITabModel
    {
        public ModelloDelegaInfo[] ModelliDelega { get; set; }
    }


    public class DettaglioDFTrasmModel : ITabModel
    {
        public DocInfo DocInfo { get; set; }
        public DocInfo[] Allegati { get; set; }
        public FascInfo FascInfo { get; set; }
        public TrasmInfo TrasmInfo { get; set; }
        public string IdTrasm { get; set; }
        public string IdEvento { get; set; }
    }

    public class AccettaDelegheRequest
    {
        public DelegaInfo[] Deleghe { get; set; }
    }

    public class SmistamentoModel : PaginateModel
    {

        public SmistamentoModel(int numTotResults, int numResForPage) : base(numTotResults,numResForPage)
        {
        }

        public SmistamentoTree Tree { get; set; }

        public SmistamentoElement[] SmistamentoElements { get; set; }

        public ModelloTrasm[] ModelliTrasm{get;set;}

        public bool CollapseRuoli;

        public bool SetDataVista;

    }

    public class FirmaHSMModel : ITabModel
    {
        public bool FirmatoResult; 
    }

    public class LibroFirmaModel : PaginateModel
    {
        public LibroFirmaModel(int numTotResults, int numResForPage) : base(numTotResults, numResForPage)
        {
        }

        public LibroFirmaElement[] LibroFirmaElements { get; set; }

        public bool ExistsSignCades { get; set; }
        public bool ExistsSignPades { get; set; }
        public bool IsAutorizedLf;
        public string Testo { get; set; }
        public RicercaType TypeRicerca { get; set; }
    }

    public class ToDoListMemento
        {
            public List<IdName> Path{get;set;}
            public int CurrentPage{get;set;}
            public int NumElements{get;set;}
            public string IdTrasm { get; set; }
            public string IdEvento { get; set; }

            public ToDoListMemento(List<IdName> path,int currentPage,int numElements,string idTrasm, string idEvento)
            {
                Path = path;
                CurrentPage=currentPage;
                NumElements = numElements;
                IdTrasm = idTrasm;
                IdEvento = idEvento;
            }

            public ToDoListMemento(int currentPage, int numElements)
            {
                Path = new List<IdName>();
                CurrentPage = currentPage;
                NumElements = numElements;
            }

        }

    public class AdlMemento
    {
        public List<IdName> Path { get; set; }
        public int CurrentPage { get; set; }
        public string IdRicSalvata { get; set; }
        public RicercaSalvataType TipoRicSalvata { get; set; }
        public string Testo { get; set; }
        public RicercaType TipoRicerca { get; set; }
        public bool DoRicerca { get; set; }

        public AdlMemento(List<IdName> path, int currentPage, string idRicSalvata, RicercaSalvataType tipoRicSalvata, string testo, RicercaType tipoRic, bool doRicerca)
        {
            Path = path;
            CurrentPage = currentPage;
            IdRicSalvata = idRicSalvata;
            TipoRicSalvata = tipoRicSalvata;
            Testo = testo;
            TipoRicerca = tipoRic;
            DoRicerca = doRicerca;
        }
    }

    public class RicercaMemento
    {
        public List<IdName> Path { get; set; }
        public int CurrentPage { get; set; }
        public string IdRicSalvata { get; set; }
        public RicercaSalvataType TipoRicSalvata { get; set; }
        public string Testo { get; set; }
        public RicercaType TipoRicerca { get; set; }
        public bool DoRicerca { get; set; }

        public RicercaMemento(List<IdName> path, int currentPage, string idRicSalvata, RicercaSalvataType tipoRicSalvata,string testo,RicercaType tipoRic,bool doRicerca)
        {
            Path = path;
            CurrentPage=currentPage;
            IdRicSalvata = idRicSalvata;
            TipoRicSalvata = tipoRicSalvata;
            Testo = testo;
            TipoRicerca = tipoRic;
            DoRicerca = doRicerca;
        }
    }

    public class LibroFirmaMemento
    { 
        public List<IdName> Path{get;set;}
        public int CurrentPage{get;set;}
        public int NumElements{get;set;}
        public string Testo { get; set; }
        public RicercaType TipoRicerca { get; set; }
        public bool DoRicerca { get; set; }



        public LibroFirmaMemento(int currentPage, int numElements, string testo, RicercaType tipoRic)
        {
            Path = new List<IdName>();
            CurrentPage = currentPage;
            NumElements = numElements;
            Testo = testo;
            TipoRicerca = tipoRic;
        }
    }

    public class IdName
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public IdName(string id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }

    public class Ruolo{
        public string Id { get; set; }
        public string Descrizione { get; set; }

        public Ruolo(RuoloInfo input){
            this.Id=input.Id;
            this.Descrizione=input.Descrizione;
        }
    }



}
