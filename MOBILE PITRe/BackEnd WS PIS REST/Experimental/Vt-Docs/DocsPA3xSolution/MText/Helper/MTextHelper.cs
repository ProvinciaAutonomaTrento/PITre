using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using MText.DomainObjects;
using System.Text.RegularExpressions;
using System.Text;

namespace MText.Helper
{
    public class MTextHelper
    {
        /// <summary>
        /// Funzione per la generazione di un datasource a partire dalla lista dei campi profilati
        /// appertenti alla tipologia associata al documento M/Text
        /// </summary>
        /// <param name="dataBinding">Nome del databinding da utilizzare per la creazione del documento</param>
        /// <param name="mTextFieldsInfo">Lista di coppie Etichetta - Valore da utilizzare per la creazione del documento M/Text</param>
        /// <returns>Array di byte con le informazioni sul data source</returns>
        public static Byte[] GetDataSource(String dataBinding, List<LabelValuePair> mTextFieldsInfo)
        {
            MemoryStream memoryStream = new MemoryStream();
            XmlWriterSettings xws = new XmlWriterSettings();
            
            // Abilitazione indentazione dell'XML prodotto
            xws.Indent = true;

            // Creazione del file xml con il data source
            using (XmlWriter xw = XmlWriter.Create(memoryStream, xws))
            {
                XDocument xmlDataSource = new XDocument(
                new XElement(ClearString(dataBinding),
                    from obj in mTextFieldsInfo
                    select GetValue(obj)));

                xmlDataSource.WriteTo(xw);
            }

            // Prelevamento dello steam come array di byte, chiusura dello stream
            // e restituzione risultato
            Byte[] dataSource = memoryStream.ToArray();
            memoryStream.Close();

            return dataSource;

        }

        /// <summary>
        /// Funzione per la creazione di un elemento XML con le informazioni
        /// su un campo profilato.
        /// </summary>
        /// <param name="entry">Coppia etichetta / valore</param>
        /// <returns>Elemento XML da aggiungere al data source per la creazione del documento M/Text</returns>
        private static XElement GetValue(LabelValuePair entry)
        {
            //return new XElement(
            //    ClearString(entry.Key),
            //    String.Format("![CDATA[{0}]]", entry.Value));

            return new XElement(
                ClearString(entry.Label),
                entry.Value);
        }

        /// <summary>
        /// Funzone per la pulizia di una stringa. Vengono sostituiti tutti i caratteri diversi da lettera o numero con un carattere _
        /// e la rende tutta minuscola
        /// </summary>
        /// <param name="stringToClean">Stringa da pulire</param>
        /// <returns>String pulita</returns>
        private static String ClearString(String stringToClean)
        {
            string pattern = @"[^a-zA-Z]";
            Regex rgx = new Regex(pattern);
            String r = rgx.Replace(stringToClean, "_");
            return r.ToLower();
        }

        /// <summary>
        /// Funzione per l'analisi del full qualified name di un databinding e creazione
        /// di un oggetto con nome ed FQN del data binding
        /// </summary>
        /// <param name="dataBinding">FQN da analizzare</param>
        /// <returns>Un oggetto con le informazioni su di un modello</returns>
        public static ModelInfo DecodeBindingInformation(String dataBinding)
        {
            ModelInfo model = new ModelInfo();
            
            // Il nome del modello è la stringa compresa fra l'ultimo slash e l'ultimo punto
            model.Name = Path.GetFileNameWithoutExtension(dataBinding);

            // L'FQN del modello è tutta la stringa dataBinding
            model.Path = dataBinding;

            return model;
        }

    }
}
