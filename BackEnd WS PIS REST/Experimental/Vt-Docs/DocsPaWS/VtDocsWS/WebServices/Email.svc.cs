using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using VtDocsWS.Services;

namespace VtDocsWS.WebServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Email" in code, svc and config file together.
    
    /// <summary>
    /// Metodi per l'invio delle mail
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(Namespace = "http://nttdata.com/2012/Pi3")]
    public class Email : IEmail
    {

        public Services.Email.SendMail.SendMailResponse SendMail(Services.Email.SendMail.SendMailRequest request)
        {
            Services.Email.SendMail.SendMailResponse response = Manager.EmailManager.SendMail(request);
            
            Utils.CheckFaultException(response);
            return response;

        }
        //sendmail
    }
}
