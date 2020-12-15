using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using log4net;

namespace DocsPaWS.Mobile.PdfToImage
{
    public class PDFConvertWrapper
    {
        private PDFConvert _convert;
        private static PDFConvertWrapper _instance = new PDFConvertWrapper();
        private ILog logger = LogManager.GetLogger(typeof(PDFConvertWrapper));

        private PDFConvertWrapper()
        {
            _convert = new PDFConvert();
            _convert.FitPage = true;
            _convert.JPEGQuality = 100;
            _convert.ResolutionX = 140;
            _convert.ResolutionY = 140;
            _convert.OutputFormat = "png16m";// "jpeg";
            _convert.TextAlphaBit = 4;
        }

        public static PDFConvertWrapper GetInstance()
        {
            return _instance;
        }

        public byte[] Convert(byte[] input,int currentPage,int dimX,int dimY)
        {
            lock (_convert)
            {
                try
                {
                    _convert.FirstPageToConvert = currentPage;
                    _convert.LastPageToConvert = currentPage;
                    _convert.Height = dimY;
                    _convert.Width = dimX;
                    string inputName = Path.GetTempPath() +   Guid.NewGuid() + ".pdf";
                    logger.Debug("input name: " + inputName);
                    string outputName = Path.GetTempPath() +   Guid.NewGuid() + ".tmp";
                    logger.Debug("output name: " + outputName);
                    BinaryWriter writer = new BinaryWriter(File.Open(inputName, FileMode.Create));
                    writer.Write(input);
                    writer.Close();
                    logger.Debug("conversione...");
                    bool res = _convert.Convert(inputName, outputName);
                    logger.Debug("conversione eseguita");
                    if (res == false) return null;
                    BinaryReader reader = new BinaryReader(File.Open(outputName, FileMode.Open));
                    byte[] arr = new byte[reader.BaseStream.Length];
                    reader.Read(arr, 0, arr.Length);
                    reader.Close();
                    logger.Debug("cancellazione temporanei...");
                    File.Delete(inputName);
                    File.Delete(outputName);
                    logger.Debug("cancellazione temporanei eseguita");
                    return arr;
                }
                catch (FileNotFoundException fe)
                {
                    logger.Error("pagina "+currentPage+" non presente");
                    throw new PDFConvertException(PDFConvertExceptionCode.PAGE_NOT_FOUND);
                }catch (Exception e)
                {
                    logger.Error("eccezione: " + e);
                    throw new PDFConvertException();
                }
            }
        }
    }
}
