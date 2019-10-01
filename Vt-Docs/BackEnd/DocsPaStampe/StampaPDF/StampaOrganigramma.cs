using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace StampaPDF
{
    public class StampaOrganigramma
    {
        
        public DocsPaVO.documento.FileDocumento convertPDF(XmlDocument XMLFile)
        {
            MemoryStream ms = new MemoryStream();
            Document pdfDoc = new Document(PageSize.A4.Rotate(), 10f, 10f, 5f, 5f);
            PdfWriter.GetInstance(pdfDoc, ms);

            Font Fontbold = new Font(BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1257, BaseFont.EMBEDDED), 12f, 1);
            Font FontHeader = new Font(BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1257, BaseFont.EMBEDDED), 12f);

            string xml = XMLFile.InnerXml;
            ORGANIGRAMMA org = null;
            using (TextReader tr = new StringReader(xml))
            {
                XmlSerializer SerializerObj = new XmlSerializer(typeof(ORGANIGRAMMA));
                org = (ORGANIGRAMMA)SerializerObj.Deserialize(tr);
            }


            pdfDoc.Open();
            int lineCount = 0;
            int totPages = (org.RECORD.Length / 44) + 1;
            int currPage = 1;
            string titoloReport = org.title;
            foreach (ORGANIGRAMMA.record r in org.RECORD)
            {
                if (lineCount == 0)
                {
                    string header = String.Format("DocsPA - {0} - Pagina: {1} di {2}", titoloReport, currPage++, totPages);
                    pdfDoc.Add(new Paragraph(header, FontHeader));
                    pdfDoc.Add(new Paragraph(28f));
                }

                string str = String.Format("{0}\r\n", r.desc);
                pdfDoc.Add(new Phrase(12f, str, Fontbold));
                lineCount++;

                if (lineCount == 44)
                {
                    lineCount = 0;
                    pdfDoc.NewPage();
                }
            }


            pdfDoc.Close();

            DocsPaVO.documento.FileDocumento filePdf = new DocsPaVO.documento.FileDocumento();
            filePdf.content = ms.ToArray();
            filePdf.estensioneFile = "pdf";
            filePdf.name = "stampaOrganigramma";
            filePdf.fullName = "stampaOrganigramma.pdf";
            filePdf.length = filePdf.content.Length;
            filePdf.contentType = "application/pdf";
            return  filePdf;
        }
        

        public class ORGANIGRAMMA
        {
            [XmlAttribute]
            public string title { get; set; }
            [XmlElement]
            public record[] RECORD;

            public class record
            {
                [XmlAttribute]
                public string tipo { get; set; }
                [XmlAttribute]
                public string desc { get; set; }
            }
        }
    }
  
}
