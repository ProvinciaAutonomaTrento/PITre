using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Report
{
    public class HeaderColumnCollection : IList<HeaderProperty>
    {
        private List<HeaderProperty> headerPropertyList;

        public HeaderColumnCollection()
        {
            headerPropertyList = new List<HeaderProperty>();
        }

        /// <summary>
        /// Costruttore per la creazione di un nuova collection a partire da una collection esistente
        /// </summary>
        /// <param name="collection"></param>
        public HeaderColumnCollection(IEnumerable<HeaderProperty> collection)
        {
            headerPropertyList = new List<HeaderProperty>(collection);
        }

        /// <summary>
        /// Metodo per l'individuazione dell'indice occupato da un elemento all'interno di questa
        /// collection
        /// </summary>
        /// <param name="item">Elemento da ricercare</param>
        /// <returns>Indice dell'elemento all'interno della collection</returns>
        public int IndexOf(HeaderProperty item)
        {
            return headerPropertyList.IndexOf(item);
        }

        /// <summary>
        /// Metodo per l'inserimento di un elemento in una determinata posizione
        /// </summary>
        /// <param name="index">Indice in cui inserire l'elemento</param>
        /// <param name="item">Elemento da inserire nella posizione index</param>
        public void Insert(int index, HeaderProperty item)
        {
            headerPropertyList.Insert(index, item);
        }

        /// <summary>
        /// Metodo per la rimozione di un elemento da una specifica posizione
        /// </summary>
        /// <param name="index">Indice dell'elemento da rimuovere</param>
        public void RemoveAt(int index)
        {
            headerPropertyList.RemoveAt(index);
        }

        /// <summary>
        /// Accessore basato su indice dell'oggetto
        /// </summary>
        /// <param name="index">Indice da gestire</param>
        /// <returns>Elemento alla posizione index</returns>
        public HeaderProperty this[int index]
        {
            get
            {
                return headerPropertyList[index];
            }
            set
            {
                headerPropertyList[index] = value;
            }
        }

        /// <summary>
        /// Restituzione di un elemento con un certo nome originale
        /// </summary>
        /// <param name="originalName">Nome da ricercare</param>
        /// <returns>Proprietà della colonna</returns>
        public HeaderProperty this[String originalName]
        {
            get
            {
                HeaderProperty retVal = null;
                try
                {
                    retVal = headerPropertyList.FirstOrDefault(e => e.OriginalName.ToLower() == originalName.ToLower());
                }
                catch
                {
                    // Ignored
                }

                return retVal;
            }
        }

        /// <summary>
        /// Metodo per l'aggiunta di un elemento a questa collection
        /// </summary>
        /// <param name="item">Elemento da aggiungere</param>
        public void Add(HeaderProperty item)
        {
            headerPropertyList.Add(item);
        }

        /// <summary>
        /// Metodo per la pulizia di questa collection
        /// </summary>
        public void Clear()
        {
            headerPropertyList.Clear();
        }

        /// <summary>
        /// Metodo per verificare se questa collection contiene un determinato elemento
        /// </summary>
        /// <param name="item">Item da verificare</param>
        /// <returns>Booleano che indica se l'elemento è contenuto nella collection</returns>
        public bool Contains(HeaderProperty item)
        {
            return headerPropertyList.Contains(item);
        }

        /// <summary>
        /// Metodo per copiare il contenuto di questa collection in un array a partire
        /// da una specifica posizione
        /// </summary>
        /// <param name="array">Array in cui copiare</param>
        /// <param name="arrayIndex">Indice da cui cominciare la copia</param>
        public void CopyTo(HeaderProperty[] array, int arrayIndex)
        {
            headerPropertyList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Metodo per il conteggio degli elementi di questa collection
        /// </summary>
        public int Count
        {
            get { return headerPropertyList.Count; }
        }

        /// <summary>
        /// Metodo per verificare se questa collection è di sola lettura
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Metodo per la rimozione di un elemento da questa collection
        /// </summary>
        /// <param name="item">Elemento da rimuovere</param>
        /// <returns>Estiop dell'azione di rimozione</returns>
        public bool Remove(HeaderProperty item)
        {
            return headerPropertyList.Remove(item);
        }

        /// <summary>
        /// Metodo per la generazione di un enumeratore per scorrere la collection
        /// </summary>
        /// <returns>Un enumeratore per socrrere gli elementi della collection</returns>
        public IEnumerator<HeaderProperty> GetEnumerator()
        {
            return headerPropertyList.GetEnumerator();
        }

        /// <summary>
        /// Metodo per la generazione di un enumeratore per scorrere gli elementi di questa collection
        /// </summary>
        /// <returns>Enumeratore per scorrere gli elementi della collection</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return headerPropertyList.GetEnumerator();
        }
    }
}
