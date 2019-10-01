using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace DocsPaVO.FileManager
{
    [ServiceContract]
    public interface IFileManager
    {
        [OperationContract]
         RemoteFileInfo DownloadFile(SendFileRequest request);
    }
}
