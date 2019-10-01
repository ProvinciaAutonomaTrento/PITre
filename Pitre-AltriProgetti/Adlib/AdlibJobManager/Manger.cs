using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdlibJobManager
{
    public class Manager : IDisposable
    {
         private bool disposed = false;
        JobManager jm = null;
        string errors = "";
        string tempFolder;
        public Manager(string ServiceUrl, string UserID,string PassWord )
        {
            tempFolder = AppDomain.CurrentDomain.BaseDirectory + "\\Cache\\" + Guid.NewGuid().ToString().Replace("-", "");

            jm = new JobManager();
            jm.UseMtom = true;
            jm.FolderForOutputDocuments = tempFolder;
          
            jm.SubmitDocsSeparately = false;
            //            "http://swenpdfapp/Adlib/JobManagementService/JobManagementService.svc"
            jm.Initialize(ServiceUrl);
            jm.Register(UserID, PassWord, ref errors);
           
        }

        public byte[] convertToPdf(string fileName, byte[] inputContent)
        {
            string file = System.IO.Path.GetTempFileName();
            System.IO.File.WriteAllBytes(file, inputContent);
            List<string> files = new List<string>();
            files.Add(file);
            List<JobManagerFile> result = jm.SubmitJobs(files, ref errors);
            List<Guid> g = (from a in result select a.jobID).ToList();
            jm.WaitAllReady(result);
            jm.DownloadFiles(result);
            jm.ReleaseJobs(result);
            
            return result.FirstOrDefault ().resultContent;
        }
        



        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if(!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if(disposing)
                {
                    // Dispose managed resources.
                    //component.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.
                //CloseHandle(handle);
                //handle = IntPtr.Zero;

                // Note disposing has been done.
                if (jm != null)
                    jm.Unregister(null, null, ref errors);

                System.IO.Directory.Delete(tempFolder, true);

                disposed = true;

            }
        }

        ~Manager()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

    }
}
