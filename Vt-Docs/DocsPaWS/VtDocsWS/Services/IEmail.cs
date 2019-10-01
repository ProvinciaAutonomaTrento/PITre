using System;
using System.Collections.Generic;
using System.Linq;
//using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Web;

namespace VtDocsWS.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IEmail" in both code and config file together.
    [ServiceContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public interface IEmail
    {
        [OperationContract]
        VtDocsWS.Services.Email.SendMail.SendMailResponse SendMail(VtDocsWS.Services.Email.SendMail.SendMailRequest request);
    }
}
