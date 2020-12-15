using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain
{
    /// <summary>
    /// asd
    /// </summary>
    //[DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class Folder
    {
        /// <summary>
        /// asd
        /// </summary>
        //[DataMember]
        public string Id { get; set; }
        /// <summary>
        /// asd
        /// </summary>
        //[DataMember]
        public string Description { get; set; }
        /// <summary>
        /// asd
        /// </summary>
        //[DataMember]
        public string IdParent { get; set; }
        /// <summary>
        /// asd
        /// </summary>
        //[DataMember]
        public string IdProject { get; set; }

        //[DataMember]
        public string CreationDate { get; set; }
        /// <summary>
        /// asd
        /// </summary>
        //[DataMember]
        //public Domain.Folder[] SubFolders { get; set; }
    }
}