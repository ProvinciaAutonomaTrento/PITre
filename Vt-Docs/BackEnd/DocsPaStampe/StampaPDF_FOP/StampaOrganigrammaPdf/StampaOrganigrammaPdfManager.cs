using System.Xml;
using System.Xml.Xsl;
using org.apache.fop.apps;
using org.xml.sax;
using java.io;

namespace StampaPDF.StampaOrganigrammaPdf
{
	/// <summary>
	/// Summary description for StampaOrganigrammaPdfManager.
	/// </summary>
	public class StampaOrganigrammaPdfManager
	{
		public StampaOrganigrammaPdfManager()
		{			
		}

		public DocsPaVO.documento.FileDocumento convertPDF(XmlDocument XMLFile, string XSLUrlPath)
		{						
			DocsPaVO.documento.FileDocumento filePdf = new DocsPaVO.documento.FileDocumento();
//			string xslFilePath = "xslfo.xsl";
			
			XslTransform tr = new XslTransform();
			XmlUrlResolver res = new XmlUrlResolver();
			res.Credentials = System.Net.CredentialCache.DefaultCredentials;
			System.IO.StringWriter sw = new System.IO.StringWriter();
//			tr.Load(xslFilePath,res); 
			tr.Load(XSLUrlPath,res);

			tr.Transform(XMLFile.CreateNavigator(),null,sw,res) ;

			string fullfodoc = sw.ToString();
			
			InputSource source = new InputSource(new java.io.StringReader(fullfodoc));
			ByteArrayOutputStream output = new ByteArrayOutputStream();				
			Driver driver = new Driver(source, output);
			driver.setRenderer(Driver.RENDER_PDF);
			driver.run();
			output.close();

			int sz = output.buf.Length;
			byte[] pdf = new byte[sz];
			for (int i = 0; i < sz; i++)
				pdf[i] = (byte)output.buf[i];

			filePdf.content = pdf;
			filePdf.estensioneFile = "pdf";
			filePdf.name = "stampaOrganigramma";
			filePdf.fullName = "stampaOrganigramma.pdf";
			filePdf.length = (int)pdf.Length;
			filePdf.contentType ="application/pdf";

//			FileStream fs = new FileStream("filePdf.pdf",FileMode.Create);
//			BinaryWriter bw = new BinaryWriter(fs);
//			bw.Write(pdf);
//			bw.Close();
//			fs.Close();			

			return filePdf;
		}
	}
}
