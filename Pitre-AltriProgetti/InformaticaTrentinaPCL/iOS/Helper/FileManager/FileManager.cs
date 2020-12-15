using System;
using Foundation;
using InformaticaTrentinaPCL.iOS.TabBar.Document.Open.Storyboard;
using InformaticaTrentinaPCL.OpenFile.MVP;
using UIKit;

namespace InformaticaTrentinaPCL.iOS.Helper.FileManager
{
    public class FileManager:IFileSystemManager
    {

        static private FileManager instance;

        public static FileManager Instance()
        {
            if (instance == null)
            {
                instance = new FileManager();
            }

            return instance;
        }

        private FileManager()
        {
        }

        public void saveFileAndOpen(byte[] inputBytes, string fileName, string extension, Object extra)
        {
            if (!(extra is UIViewController))
            {
                Console.WriteLine("Errore: Object Extra deve essere un UIViewController");
                throw new ArgumentException();
            }

            UIViewController vc = (UIViewController)extra;
            if (inputBytes == null) return;
            NSData pdfFile = NSData.FromArray(inputBytes);
            UIViewControllerSeeDocument webView = (UIViewControllerSeeDocument)Utility.GetControllerFromStoryboard("Storyboard_OpenDocument", "UIViewControllerSeeDocument");
            webView.data = pdfFile;
            webView.extension = extension;
            webView.fileName = fileName;
            vc.PresentViewController(webView, true, null);
            
        }
    }
}
