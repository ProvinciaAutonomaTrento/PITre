using System;

namespace InformaticaTrentinaPCL.Interfaces
{
    /// <summary>
    /// Classe astratta utilizzata per mostrare i dati nelle row di seleziona assegnatario delega e Assegna->SelezionaAssegnatario
    /// </summary>
    public abstract class AbstractRecipient
    {
        public Type InstanceType
        {
            get { return GetType(); }
            private set {}
        }

        public abstract string getId();
        public abstract void SetIdRuoloDelegato(string roleId);
        public abstract string GetIdRuoloDelegato();
        public abstract string getIdCorrespondant();
        public abstract RecipientType getRecipientType();
        public abstract string getTitle();
        public abstract string getSubtitle();
        public abstract bool isPreferred();
        public abstract void setPreferred(bool isFavorite);
        
        /// <summary>
        /// NON CAMBIARE L'ORDINE DELL'ENUM PERCHE' E' USATA NEL MAPPING DELLA LISTA CORRISPONDENTI
        /// </summary>
        public enum RecipientType
        {
            OFFICE = 0,
            ROLE = 1,
            USER = 2,
            MODEL            
        }
    }
}