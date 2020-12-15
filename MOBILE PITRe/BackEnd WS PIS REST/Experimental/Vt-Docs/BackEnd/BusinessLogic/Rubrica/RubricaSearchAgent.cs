using System;
using System.Collections;

namespace BusinessLogic.Rubrica
{
	#region Nuova Rubrica
		
	/// <summary>
	/// Filtro per l'elaborazione dei dati restituiti dalle ricerche
	/// </summary>
	/// <remarks>
	/// Questo delegate può essere usato per creare funzioni di filtro
	/// dei risultati di ricerca da eseguire al termine della ricerca stessa
	/// al fine di ottimizzare, restringere o comunque elaborare l'insieme
	/// di dati ottenuto.
	/// </remarks>
	/// <param name="qr">L'insieme di parametri con cui è stata effettuata la ricerca. E'
	/// un'istanza della classe <see cref="DocsPaVO.rubrica.ParametriRicercaRubrica"/></param>
	/// <param name="ers">L'insieme di risultati restituito dalla procedura di ricerca, che può essere
	/// modificato dalla funzione di filtro.</param>
	public delegate void RubricaSearchFilter (DocsPaVO.rubrica.ParametriRicercaRubrica qr, ref ArrayList ers,DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica);

	/// <summary>
	/// Classe astratta per la gestione delle ricerche
	/// </summary>
	public abstract class RubricaSearchAgent 
	{
#if DOCSPA_20
		/// <summary>
		/// Un'istanza della classe <see cref="DocsPaDM.utils.CommonObjects"/>
		/// </summary>
		protected DocsPaDM.utils.CommonObjects co;
#endif
		/// <summary>
		/// L'eventuale filtro di ricerca (vedi <see cref="RubricaSearchFilter"/>
		/// </summary>
		protected RubricaSearchFilter SearchFilter;

		/// <summary>
		/// Il costruttore della classe base
		/// </summary>
		/// <param name="_co">Un'istanza della classe <see cref="DocsPaDM.utils.CommonObjects"/></param>
		protected RubricaSearchAgent(
#if DOCSPA_20
			DocsPaDM.utils.CommonObjects _co
#endif			
			)
		{
#if DOCSPA_20
			co = _co;
#endif
			SearchFilter = null;
		}

		/// <summary>
		/// Il metodo principale per l'esecuzione delle ricerche
		/// </summary>
		/// <param name="ParametriRicerca">Un'istanza della classe <see cref="DocsPaVo.rubrica.ParametriRicercaRubrica"/> 
		/// contenente i parametri di ricerca</param>
		/// <returns>Un <see cref="System.Collections.ArrayList">ArrayList</see> contenente il risultato</returns>
		/// <remarks>
		/// <para>
		/// Le classi che implementano questo metodo devono restituire un 
		/// <see cref="System.Collections.ArrayList">ArrayList</see> contenente i 
		/// risultati della ricerca. </para>
		/// Ogni elemento restituito è un'istanza della classe
		/// <see cref="DocsPaVO.rubrica.ElementoRubrica">ElementoRubrica</see>
		/// </remarks>
		public abstract ArrayList Search(DocsPaVO.rubrica.ParametriRicercaRubrica ParametriRicerca, DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica);


        /// <summary>
        /// Il metodo principale per l'esecuzione delle ricerche
        /// </summary>
        /// <param name="ParametriRicerca">Un'istanza della classe <see cref="DocsPaVo.rubrica.ParametriRicercaRubrica"/> 
        /// contenente i parametri di ricerca</param>
        /// <returns>Un <see cref="System.Collections.ArrayList">ArrayList</see> contenente il risultato</returns>
        /// <remarks>
        /// <para>
        /// Le classi che implementano questo metodo devono restituire un 
        /// <see cref="System.Collections.ArrayList">ArrayList</see> contenente i 
        /// risultati della ricerca. </para>
        /// Ogni elemento restituito è un'istanza della classe
        /// <see cref="DocsPaVO.rubrica.ElementoRubrica">ElementoRubrica</see>
        /// </remarks>
        public abstract ArrayList SearchPaging(DocsPaVO.rubrica.ParametriRicercaRubrica ParametriRicerca, DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica, int firstRowNum, int maxRowForPage, out int totale);

        //public abstract ArrayList SearchForCorr(string codice, DocsPaVO.rubrica.SmistamentoRubrica smistaRubrica);
			
		/// <summary>
		/// Restituisce un singolo elemento dato il codice rubrica
		/// </summary>
		/// <param name="codice">Il codice rubrica associato all'elemento da cercare</param>
		/// <param name="smistamentoRubrica">Oggetto necessario a contenele le informazioni
		/// per filtrare o meno le informazioni relative allo smistamento</param>
		/// <returns>Un'istanza della classe <see cref="DocsPaVO.rubrica.ElementoRubrica">ElementoRubrica</see>
		/// contenente i dati dell'elemento trovato oppure null</returns>
		public abstract DocsPaVO.rubrica.ElementoRubrica SearchSingle(string codice, DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica, string condRegistri);

        /// <summary>
        /// Restituisce un singolo elemento dato il codice rubrica senza calcoli gerarchia è più veloce del SearchSingle
        /// </summary>
        /// <param name="codice"></param>
        /// <returns></returns>
		public abstract DocsPaVO.rubrica.ElementoRubrica SearchSingleSimple(string codice);

        /// <summary>
        /// Restituisce un singolo elemento data la systemId, senza calcoli gerarchia è più veloce del SearchSingle
        /// </summary>
        /// <param name="codice"></param>
        /// <returns></returns>
        public abstract DocsPaVO.rubrica.ElementoRubrica SearchSingleSimpleBySystemId(string systemId);

		/// <summary>
		/// Restituisce un singolo elemento dato il codice rubrica
		/// </summary>
		/// <param name="codice">Il codice rubrica associato all'elemento da cercare</param>
		/// <returns>Un'istanza della classe <see cref="DocsPaVO.rubrica.ElementoRubrica">ElementoRubrica</see>
		/// contenente i dati dell'elemento trovato oppure null</returns>
		public abstract ArrayList SearchRange(string[] codici, DocsPaVO.addressbook.TipoUtente tipoIE);


        public abstract ArrayList GetChildrenElement(string elementID, string childrensType);
	
		/// <summary>
		/// Restituisce la gerarchia dell'elemento selezionato
		/// </summary>
		/// <param name="codice">Il codice dell'elemento rubrica</param>
		/// <returns>Le classi che implementano questo metodo devono
		/// restituire un <see cref="System.Collections.ArrayList">ArrayList</see>
		/// contenente gli elementi della catena gerarchica che dall'elemento
		/// di più alto livello arriva all'elemento il cui codice
		/// è stato indicato come parametro. Gli elementi restituiti sono
		/// altrettante istanza della classe 
		/// <see cref="DocsPaVO.rubrica.ElementoRubrica">ElementoRubrica</see>
		/// e sono ordinati all'interno dell'ArrayList dal più alto in grado
		/// al più in basso.</returns>
		public abstract ArrayList GetHierarchy (string codice, DocsPaVO.addressbook.TipoUtente tipoIE, DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica);

		public abstract void CheckChildrenExistence (ref DocsPaVO.rubrica.ElementoRubrica[] ers, string idAmm);

		public abstract void CheckChildrenExistence (ref DocsPaVO.rubrica.ElementoRubrica[] ers, bool checkUo, bool checkRuoli, bool checkUtenti, string idAmm);

		/// <summary>
		/// Restituisce la gerarchia dell'elemento selezionato
		/// </summary>
		/// <param name="codice">Il codice dell'elemento rubrica</param>
		/// <returns>Le classi che implementano questo metodo devono
		/// restituire un <see cref="System.Collections.ArrayList">ArrayList</see>
		/// contenente gli elementi della catena gerarchica che dall'elemento
		/// di più alto livello arriva all'elemento il cui codice
		/// è stato indicato come parametro. Gli elementi restituiti sono
		/// altrettante istanza della classe 
		/// <see cref="DocsPaVO.rubrica.ElementoRubrica">ElementoRubrica</see>
		/// e sono ordinati all'interno dell'ArrayList dal più alto in grado
		/// al più in basso.</returns>
		public abstract ArrayList GetHierarchyRange (string[] codici, DocsPaVO.addressbook.TipoUtente[] tipiIE);

		public abstract ArrayList GetRootItems(DocsPaVO.addressbook.TipoUtente tipoIE, DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica);

        public abstract ArrayList SearchRangeSystemID(string[] valoriRicerca);
	}

	#endregion

}
