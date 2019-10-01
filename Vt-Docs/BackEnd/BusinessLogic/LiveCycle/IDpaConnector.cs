using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.IO;

namespace LCDocsPaService.Services
{
    [ServiceContract]
    public interface IDpaConnector
    {
        [OperationContract]
        UploadResponse UploadFileStream(RemoteFileInfo request);

        [OperationContract]
        bool UploadFileArray(string FileName, byte[] FileByteArray);
    }


    [MessageContract]
    public class UploadResponse
    {
        [MessageBodyMember(Order = 1)]
        public bool UploadSucceeded { get; set; }
    }


    [MessageContract]
    public class RemoteFileInfo : IDisposable
    {
        [MessageHeader(MustUnderstand = true)]
        public string FileName;


        [MessageBodyMember(Order = 1)]
        public System.IO.Stream FileByteStream;

        public void Dispose()
        {
            if (FileByteStream != null)
            {
                FileByteStream.Close();
                FileByteStream = null;
            }
        }
    }
}
