using System;
using System.Xml.Serialization;

namespace DocsPaVO.filtri 
{
    [Serializable()]
    [XmlInclude(typeof(DocsPaVO.filtri.trasmissione.listaArgomenti))]
    [XmlInclude(typeof(DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione))]
    [XmlInclude(typeof(DocsPaVO.filtri.trasmissione.listaArgomentiNascosti))]	
	[XmlInclude(typeof(DocsPaVO.filtri.ricerca.listaArgomenti))]	
	[XmlInclude(typeof(DocsPaVO.filtri.fascicolazione.listaArgomenti))]
	[XmlInclude(typeof(DocsPaVO.filtri.stampaRegistro.listaArgomenti))]
  
    public class FiltroRicerca 
	{
		// il valore della variabile argomento deve essere l'indice selezionato preso da un enum
        public string argomento;
		public string valore;

        // indica la modalità di ricerca di un filtro di tipo testo
        public SearchTextOptionsEnum searchTextOptions = SearchTextOptionsEnum.WholeWord;

		// utilizzato solo per far apparire l'enum nel WSDL
        public DocsPaVO.filtri.trasmissione.listaArgomenti listaFiltriTrasmissione;
        public DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione listaFiltriModelliTrasmissione;
        public DocsPaVO.filtri.trasmissione.listaArgomentiNascosti listaFiltriTrasmissioneNascosti;
    	public DocsPaVO.filtri.ricerca.listaArgomenti listaFiltriDocumento;
    	public DocsPaVO.filtri.fascicolazione.listaArgomenti listaFiltriFascicolo;
        public DocsPaVO.filtri.stampaRegistro.listaArgomenti listaFiltriStampaRegistro;
        
        public string nomeCampo;
	}

    /// <summary>
    /// Classe che definisce le informazioni relative ad un testo da ricercare in un campo fulltext
    /// </summary>
    public class SearchTextItem
    {
        // Operatori in caso ricerca valori multipli su singolo campo:
        //  &&
        //  ||

        private string _textToSearch = string.Empty;
        // [System.ComponentModel.DefaultValueAttribute(SearchTextOptionsEnum.WholeWord)]
        private SearchTextOptionsEnum _searchOption = SearchTextOptionsEnum.WholeWord;

        /// <summary>
        /// 
        /// </summary>
        public SearchTextItem()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textToSearch"></param>
        public SearchTextItem(string textToSearch)
        {
            this.TextToSearch = textToSearch;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textToSearch"></param>
        /// <param name="searchOption"></param>
        public SearchTextItem(string textToSearch, SearchTextOptionsEnum searchOption)
            : this(textToSearch)
        {
            this.SearchOption = searchOption;
        }

        /// <summary>
        /// 
        /// </summary>
        public string TextToSearch
        {
            get
            {
                return this._textToSearch;
            }
            set
            {
                this._textToSearch = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public SearchTextOptionsEnum SearchOption
        {
            get
            {
                return this._searchOption;
            }
            set
            {
                this._searchOption = value;
            }
        }
    }

    /// <summary>
    /// Enumeration che identifica i tipi di ricerca del testo:
    //      Parola intera
    //      Parola che inizia con
    //      Parte della parola
    /// </summary>
    public enum SearchTextOptionsEnum
    {
        WholeWord,
        InitWithWord,
        ContainsWord
    }
}
