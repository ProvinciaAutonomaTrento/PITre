using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.IO;

namespace DocsPaVO.FileManager
{
    [MessageContract]
    public class RemoteFileInfo : IDisposable
    {
        public RemoteFileInfo()
        {
            this.FileTransferInfo = new FileTransferInfo();
        }

        [MessageBodyMember(Order = 1)]
        public Stream FileData;

        [MessageHeader(MustUnderstand = true)]
        public FileTransferInfo FileTransferInfo;

        public void Dispose()
        {
            if (FileData != null)
            {
                FileData.Close();
                FileData = null;
            }

        }
    }
}
