using System.Collections.Generic;
using System.Linq;
using System;
using System.Xml.Serialization;
using System.Drawing;
using System.ComponentModel;

namespace DocsPaVO.Grids
{
    /// <summary>
    /// Questa classe rappresenta l'oggetto di base restituito dalle ricerche documenti o fascicoli.
    /// L'oggetto ha una lista di oggetti custom con i valori interessatin dalla ricerca
    /// </summary>
    [Serializable()]
    public class SearchObject
    {
        /// <summary>
        /// Identificativo dell'oggetto cercato (ID del documento o del fascicolo oppure della trasmissione)
        /// </summary>
        public string SearchObjectID { get; set; }

        /// <summary>
        /// I campi che compongono questa griglia
        /// </summary>
        [XmlArray()]
        [XmlArrayItem(typeof(SearchObjectField))]
        public List<SearchObjectField> SearchObjectField { get; set; }

        /// <summary>
        /// Funzione per l'aggiunta di un campo a questa griglia
        /// Il campo verrà aggiunto solo se non ne esiste già uno uguale
        /// </summary>
        /// <param name="fieldSettings"></param>
        public void AddField(SearchObjectField searchObjectField)
        {
            if (!this.SearchObjectField.Contains(searchObjectField))
                this.SearchObjectField.Add(searchObjectField);
        }
    }
}
