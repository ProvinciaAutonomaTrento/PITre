using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfRouting
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IServiceRouter" in both code and config file together.
    [ServiceContract]
    public interface IServiceRouter
    {
              
        // TODO: Add your service operations here
        [OperationContract(Action = "*", ReplyAction = "*")]
        System.ServiceModel.Channels.Message MessageRoute(System.ServiceModel.Channels.Message value);
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
  
}
