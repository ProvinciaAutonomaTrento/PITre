using System;
using System.Text;
using StampaPDF.StampaBustePdf.Table_Object;

namespace StampaPDF.StampaBustePdf.PDFObject
{
	internal class PdfImageContent : PdfObject
	{
		private string name,imagename;
		private int imageid,width,height;
		private double dpi;
		private double TargetWidth
		{
			get
			{
				return this.width*72/this.dpi;
			}
		}
		private double TargetHeight
		{
			get
			{
				return this.height*72/this.dpi;
			}
		}
		double posx,posy;
		internal string Name
		{
			get
			{
				return this.name;
			}
		}
		internal PdfImageContent(int id,string imagename,int imageid,int width,int height,double posx,double posy,double DPI)
		{
			this.name=name;
			this.imagename=imagename;
			this.imageid=imageid;
			this.id=id;
			this.height=height;
			this.width=width;
			this.posx=posx;
			this.dpi=DPI;
			this.posy=posy-this.TargetHeight;
			
			
		}
		internal override int StreamWrite(System.IO.Stream stream)
		{
			
			string s="";
			s+="q\n";
			s+=this.TargetWidth.ToString()+" 0 0 ";
			s+=this.TargetHeight.ToString()+" "+posx.ToString("0.##")+" "+posy.ToString("0.##")+" cm\n";
			s+="/"+this.imagename+" Do\n";
			s+="Q\n";
				
			s=s.Replace(",",".");

			Byte[] b=ASCIIEncoding.ASCII.GetBytes(s);
				
			string r=this.HeadObj;
			r+="<< /Lenght "+b.Length.ToString()+">>\n";
			r+="stream\n";
			r+=s;

			r+="endstream\n";
			r+="endobj\n";
			Byte[] b2=ASCIIEncoding.ASCII.GetBytes(r);
			stream.Write(b2,0,b2.Length);
			return b2.Length;
		}

	}
	
}