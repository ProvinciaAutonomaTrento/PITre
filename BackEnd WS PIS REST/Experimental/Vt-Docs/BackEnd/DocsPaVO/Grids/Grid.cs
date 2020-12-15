using System.Collections.Generic;
using System.Linq;
using System;
using System.Xml.Serialization;
using System.Drawing;
using System.ComponentModel;

namespace DocsPaVO.Grid
{

    /// <summary>
    /// Questa classe rappresenta le impostazioni relative ad una griglia
    /// custom
    /// </summary>
    [Serializable()]
    [XmlInclude(typeof(DocsPaVO.Grid.Grid.GridTypeEnumeration))]
    [XmlInclude(typeof(DocsPaVO.Grid.SpecialField))]
    public class Grid
    {
        /// <summary>
        /// L'enumerazione dei tipi di ricerca in cui è possibile 
        /// ricercare una griglia
        /// </summary>
        [Serializable()]
        public enum GridTypeEnumeration
        {
            Document = 0,
            Project = 1,
            Transmission = 2,
            NotRecognized = 3,
            DocumentInProject = 4
        }

        /// <summary>
        /// Ordinamento crescente o descrente
        /// </summary>
        [Serializable()]
        public enum OrderDirectionEnum
        {
            Asc = 0,
            Desc = 1
        }

        /// <summary>
        /// 
        /// </summary>
        public Grid()
        {
            
        }

        /// <summary>
        /// Funzione per l'inizializzazione di un nuovo oggetto per
        /// la memorizzazione delle impostazioni relative ad una griglia
        /// di ricerca
        /// </summary>
        /// <param name="gridId">L'identificativo univoco della griglia</param>
        /// <param name="gridType">Tipo di ricerca cui si riferisce questa griglia</param>
        public Grid(string gridId, List<String> templatesId, GridTypeEnumeration gridType)
        {
            this.GridId = gridId;
            //this.TemplatesId = new List<string>();
            this.TemplatesId = templatesId;
            this.GridType = gridType;
            this.Fields = new List<Field>();

        }

        /// <summary>
        /// Identificativo univoco della griglia
        /// </summary>
        public string GridId { get; set; }

        /// <summary>
        /// L'id del template di cui questa griglia rappresenta una personalizzazione
        /// </summary>
        [XmlArray()]
        [XmlArrayItem(typeof(string))]
        public List<string> TemplatesId { get; set; }

        /// <summary>
        /// L'identificativo univoco della ricerca salvata legata a questa griglia
        /// </summary>
        public string RapidSearchId { get; set; }

        /// <summary>
        /// Il tipo di ricerca cui si riferisce questa griglia
        /// </summary>
        public GridTypeEnumeration GridType { get; set; }

        /// <summary>
        /// I campi che compongono questa griglia
        /// </summary>
        [XmlArray()]
        [XmlArrayItem(typeof(Field))]
        public List<Field> Fields { get; set; }

        /// <summary>
        /// Funzione per l'aggiunta di un campo a questa griglia
        /// Il campo verrà aggiunto solo se non ne esiste già uno uguale
        /// </summary>
        /// <param name="fieldSettings"></param>
        public void AddField(Field field)
        {
            if (!this.Fields.Contains(field))
                this.Fields.Add(field);
        }

        /// <summary>
        /// Funzione per il reperimento delle impostazioni relative ad un campo
        /// </summary>
        /// <returns>L'eitchetta del campo da reperire</returns>
        public Field GetField(string originalLabel)
        {
            return this.Fields.Where(e => e.OriginalLabel == originalLabel).FirstOrDefault();
        }

        /// <summary>
        /// Campo su cui ordinare
        /// </summary>
        public Field FieldForOrder { get; set; }

        /// <summary>
        /// Tipo di ordinamento da attuare sul campo
        /// </summary>
        public OrderDirectionEnum OrderDirection { get; set; }

        public string GridName { get; set; }

        /// <summary>
        /// Colore assunto dalle celle per cui non ha senso un particolare campo profilato.
        /// Ad esempio se si decide di visualizzare il campo A di un template ma i risultati
        /// contengono anche dati su docunmenti di altre tipologie, per tali documenti
        /// le celle corrispondenti al campo A verranno colorate con il colore impostato
        /// </summary>
        [DefaultValue("99CCCC")]
        public String ColorForFieldWithotTemplate { get; set; }
        
    }
}