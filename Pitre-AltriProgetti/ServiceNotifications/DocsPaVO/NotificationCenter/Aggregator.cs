using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.NotificationCenter
{
    /// <summary>
    /// Questa classe implementa il concetto di aggregatore di ruoli
    /// </summary>
    public class Aggregator
    {
        #region Private Fields

        private string _id_aur;
        private string _type_aur;
        private string _typeNotify;
        #endregion

        #region Property

        public string IDAUR
        {
            get
            {
                return _id_aur;
            }

            set
            {
                _id_aur = value;
            }
        }

        public string TYPEAUR
        {
            get
            {
                return _type_aur;
            }

            set
            {
                _type_aur = value;
            }
        }

        public string TYPENOTIFY
        {
            get
            {
                return _typeNotify;
            }

            set
            {
                _typeNotify = value;
            }
        }
        #endregion
    }
}
