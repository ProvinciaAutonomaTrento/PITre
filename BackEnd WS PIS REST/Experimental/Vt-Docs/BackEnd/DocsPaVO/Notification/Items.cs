using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Notification
{
    public class Items
    {
        #region private field

        private string _item1;
        private string _item2;
        private string _item3;
        private string _item4;

        #endregion

        #region public property

        /// <summary>
        /// 
        /// </summary>
        public string ITEM1
        {
            get
            {
                return _item1;
            }

            set
            {
                _item1 = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ITEM2
        {
            get
            {
                return _item2;
            }

            set
            {
                _item2 = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ITEM3
        {
            get
            {
                return _item3;
            }

            set
            {
                _item3 = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ITEM4
        {
            get
            {
                return _item4;
            }

            set
            {
                _item4 = value;
            }
        }

        #endregion
    }
}
