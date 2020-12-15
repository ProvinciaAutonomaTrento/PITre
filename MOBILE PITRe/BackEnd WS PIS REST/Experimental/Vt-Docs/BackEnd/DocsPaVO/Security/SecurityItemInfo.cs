using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Security
{
    /// <summary>
    /// Oggetto contenente i metadati di security relativi ad un oggetto DocsPa
    /// </summary>
    [Serializable()]
    public class SecurityItemInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public enum SecurityAccessRightsEnum
        {
            /// <summary>
            /// Diritto proprietario di un oggetto docspa per l'utente
            /// </summary>
            ACCESS_RIGHT_0 = 0,

            /// <summary>
            /// Diritto in sola lettura per l'utente cui è stato trasmesso un oggetto con ragione accetta / rifiuta
            /// <remarks>
            /// La trasmissione è in stato da accettare / rifiutare
            /// </remarks>
            /// </summary>
            ACCESS_RIGHT_20 = 20,

            /// <summary>
            /// Diritto in sola lettura di un oggetto docspa per l'utente
            /// </summary>
            ACCESS_RIGHT_45 = 45,

            /// <summary>
            /// Diritto in lettura / scrittura di un oggetto docspa per l'utente (acquisito, ad es, per trasmissione)
            /// </summary>
            ACCESS_RIGHT_63 = 63,

            /// <summary>
            /// Diritto proprietario di un oggetto docspa per un ruolo
            /// </summary>
            ACCESS_RIGHT_255 = 255,
        }


        /// <summary>
        /// 
        /// </summary>
        public SecurityItemInfo()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static SecurityItemInfo NewItem(System.Data.IDataReader reader)
        {
            SecurityItemInfo item = new SecurityItemInfo();
            item.IdPeople = reader.GetValue(reader.GetOrdinal("id_people")).ToString();
            item.IdGroup = reader.GetValue(reader.GetOrdinal("id_group")).ToString();
            item.AccessRights = (SecurityAccessRightsEnum) 
                        Enum.Parse(typeof(SecurityAccessRightsEnum), reader.GetValue(reader.GetOrdinal("accessrights")).ToString(), true);
            item.TipoDiritto = reader.GetValue(reader.GetOrdinal("tipodiritto")).ToString();
            return item;
        }

        /// <summary>
        /// Se valorizzato, indica l'utente che ha il diritto sull'oggetto in security
        /// </summary>
        public string IdPeople { get; set; }

        /// <summary>
        /// Se valorizzato, indica il ruolo dell'utente che ha il diritto sull'oggetto in security
        /// </summary>
        public string IdGroup { get; set; }

        /// <summary>
        /// Indica la permission sull'oggetto
        /// </summary>
        public SecurityAccessRightsEnum AccessRights { get; set; }

        /// <summary>
        /// Indica il tipo di ownersship sull'oggetto 
        /// </summary>
        public string TipoDiritto { get; set; }

        /// <summary>
        /// Indica se l'istanza si riferisce ad un diritto a persona o ruolo
        /// </summary>
        public bool IsPeople
        {
            get
            {
                return (string.IsNullOrEmpty(this.IdGroup) && !string.IsNullOrEmpty(this.IdPeople));
            }
        }
    }
}
