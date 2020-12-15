using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.NotificationCenter
{
    /// <summary>
    /// Interfaccia con le voci della notifica comuni a tutti gli oggetti di dominio vt-docs
    /// </summary>
    public class Items
    {
        #region private field

        private string _itemIdObject;
        private string _itemDescription;
        private string _itemSpecialized;
        #endregion

        #region public property

        /// <summary>
        /// Id oggetto di dominio.
        /// </summary>
        public string ID_OBJECT
        {
            get 
            {
                return _itemIdObject;
            }

            set 
            {
                _itemIdObject = value;
            }
        }

        /// <summary>
        /// Questo valore si riferisce alla descrizione della notifica ed è legato all'oggetto di dominio
        /// </summary>
        public string ITEM_DESCRIPTION
        {
            get
            {
                return _itemDescription;
            }

            set
            {
                _itemDescription = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ITEM_SPECIALIZED
        {
            get
            {
                return _itemSpecialized;
            }

            set
            {
                _itemSpecialized = value;
            }
        }
        #endregion
    }
}
