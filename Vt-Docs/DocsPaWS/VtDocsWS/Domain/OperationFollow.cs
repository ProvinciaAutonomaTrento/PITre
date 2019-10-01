using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain
{

    [DataContract(Name = "OperationFollowDomainObject")]
    public enum OperationFollow
    {
        [EnumMember]
        AddFolder,
        [EnumMember]
        RemoveFolder,
        [EnumMember]
        AddDoc,
        [EnumMember]
        RemoveDoc
    }
}