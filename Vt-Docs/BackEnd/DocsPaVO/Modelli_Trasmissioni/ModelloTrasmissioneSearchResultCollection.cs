using System.Collections.Generic;
using System;

namespace DocsPaVO.Modelli_Trasmissioni
{
    [Serializable()]
    public class ModelloTrasmissioneSearchResultCollection : IList<ModelloTrasmissioneSearchResult>
    {
        // Lista dei modelli di trasmissione
        private List<ModelloTrasmissioneSearchResult> models = new List<ModelloTrasmissioneSearchResult>();

        public int IndexOf(ModelloTrasmissioneSearchResult item)
        {
            return this.models.IndexOf(item);
        }

        public void Insert(int index, ModelloTrasmissioneSearchResult item)
        {
            this.models.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            this.models.RemoveAt(index);
        }

        public ModelloTrasmissioneSearchResult this[int index]
        {
            get
            {
                return this.models[index];
            }
            set
            {
                this.models[index] = value;
            }
        }

        public void Add(ModelloTrasmissioneSearchResult item)
        {
            this.models.Add(item);
        }

        public void Clear()
        {
            this.models.Clear();
        }

        public bool Contains(ModelloTrasmissioneSearchResult item)
        {
            return this.models.Contains(item);
        }

        public void CopyTo(ModelloTrasmissioneSearchResult[] array, int arrayIndex)
        {
            this.models.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this.models.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(ModelloTrasmissioneSearchResult item)
        {
            return this.models.Remove(item);
        }

        public IEnumerator<ModelloTrasmissioneSearchResult> GetEnumerator()
        {
            return this.models.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.models.GetEnumerator();
        }
    }
}
