using System;
using System.Collections;
using System.Drawing;
using StampaPDF.StampaBustePdf.Table_Object;

namespace StampaPDF.StampaBustePdf.PDFObject
{
	/// <summary>
	/// Summary description for PdfTextArea.
	/// </summary>
	public class PdfTextArea : PdfObject
	{
		internal ArrayList tl;
		internal string text;
		internal PdfDocument PdfDocument
		{
			get
			{
				return this.PdfArea.PdfDocument;
			}
		}
		//internal ArrayList TextLines;
		internal PdfArea textArea;
		/// <summary>
		/// gets the Area available for the Text
		/// </summary>
		public PdfArea PdfArea
		{
			get
			{
				return this.textArea;
			}
		}
		internal System.Drawing.Font Font;
		internal System.Drawing.Color Color=System.Drawing.Color.Black;
		internal int maxlines
		{
			get
			{
				return (int)(this.textArea.height/this.lineHeight);
			}
		}
		internal double lineHeight;
		
		internal int DrawnLines
		{
			get
			{
				int l=this.RenderLines().Count;
				if (l>this.maxlines) return maxlines;
				return l;
			}

		}
		/// <summary>
		/// returns the Text that can't fit inside the estabilished area
		/// </summary>
		public string OverFlowText
		{
			get
			{
				ArrayList lines=this.RenderLines();
				string s="";
				for (int index=this.maxlines;index<lines.Count;index++)
				{
					s+=lines[index];
				}
				return s;
			}
		}
		internal ContentAlignment textAlign;
		/// <summary>
		/// creates a new PdfTextArea
		/// </summary>
		/// <param name="Font">the font that will be used</param>
		/// <param name="Color">the color of the font that will be used</param>
		/// <param name="TextArea">the estabilished area for the Text</param>
		/// <param name="PdfTextAlign">the ContentAlignment for the Text inside the area</param>
		/// <param name="Text">the text that will be written inside the area</param>
		public PdfTextArea(System.Drawing.Font Font,System.Drawing.Color Color,PdfArea TextArea,ContentAlignment PdfTextAlign,string Text)
		{
			if (Text==null) throw new Exception("Text cannot be null.");
			this.Font=Font;
			this.Color=Color;
			this.textArea=TextArea;
			this.text=Text;
			this.textAlign=PdfTextAlign;
			this.lineHeight=(double)(Font.Size);
		}
		internal ArrayList RenderLines()
		{
			if (tl==null)
			{
				tl=new ArrayList();
				
				string line="",oldline="";
				double lineWidth=0,oldlineWidth=0;
				char[] textchars=text.ToCharArray();

				ArrayList words=new ArrayList();

				string aWord="";
				for (int index=0;index<textchars.Length;index++)
				{
					char c=textchars[index];
					switch (c)
					{
						case ' ':
						{
							if (aWord!="") words.Add(aWord); 
							words.Add(" "); aWord=""; break;
						}
						case '\n': words.Add(aWord); words.Add("\n"); aWord=""; break;
						default: aWord+=c; break;
					}
				}
				if (aWord!="") words.Add(aWord);
				
				for (int s2index=0;s2index<words.Count;s2index++)
				{
					string s=(string)words[s2index];
					oldline=line;
					line+=s;
					double wordWidth=Utility.Measure(Font,s);
					oldlineWidth=lineWidth;
					lineWidth+=wordWidth;
					
					if (s=="\n")
					{
						tl.Add(oldline);
						line="";
						lineWidth=0;
					}
					else
						if (lineWidth>this.textArea.width)
					{
						if (oldline!=" "&&oldline!="")
						{
							tl.Add(oldline); 
							if (s==" ")
							{ line=""; lineWidth=0;}
							else
							{ line=s; lineWidth=wordWidth;}
						} 
					}
					if (s2index==words.Count-1)
						if (line!=" "&&line!="")
							tl.Add(line); 
										
					
					
				}
			}
			return this.tl;
		}
		internal string ToLineStream()
		{
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			//string text="";
			double posx,posy;
			ArrayList al=this.RenderLines();
			//if (this.LineFits)
			int l=al.Count;
			int ml=this.maxlines;
			for(int index=0;((index<l)&&(index<ml));index++)
			{
				//string line=(al[index] as string).TrimEnd(new char[]{' '});
				string line=(al[index] as string);
				posx=textArea.posx;
				posy=textArea.posy;
				double xdiff=0,ydiff=0;
				if ((this.textAlign!=ContentAlignment.MiddleLeft)&&
					(this.textAlign!=ContentAlignment.TopLeft)&&
					(this.textAlign!=ContentAlignment.BottomLeft))
				{
					xdiff=textArea.width-Utility.Measure(this.Font,line);
					if ((this.textAlign==ContentAlignment.TopCenter)||
						(this.textAlign==ContentAlignment.MiddleCenter)||
						(this.textAlign==ContentAlignment.BottomCenter)) xdiff=xdiff/2;
				}
				if ((this.textAlign!=ContentAlignment.TopCenter)&&
					(this.textAlign!=ContentAlignment.TopLeft)&&
					(this.textAlign!=ContentAlignment.TopRight))
				{
					ydiff=(double)(this.lineHeight-(this.Font.Height*0.6)
						+(this.maxlines-this.DrawnLines)*this.lineHeight);
					if ((this.textAlign==ContentAlignment.MiddleLeft)||
						(this.textAlign==ContentAlignment.MiddleCenter)||
						(this.textAlign==ContentAlignment.MiddleRight)) ydiff=ydiff/2;
				}
				posy+=ydiff;
				sb.Append("1 0 0 1 ");
				sb.Append((posx+xdiff).ToString("0.##").Replace(",","."));
				sb.Append(" ");
				sb.Append((this.PdfDocument.PH-posy-(this.lineHeight*(index))-(Font.Height*0.525)).ToString("0.##").Replace(",","."));
				sb.Append(" Tm (");
				sb.Append(Utility.TextEncode(line));
				sb.Append(") Tj\n");
			}
			return sb.ToString();
		}
		internal string CompleteLineStream()
		{
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			sb.Append("BT\n");
			sb.Append(Utility.FontToFontLine(this.Font));
			sb.Append(Utility.ColorrgLine(this.Color));
			sb.Append(this.ToLineStream());
			sb.Append("ET\n");
			return sb.ToString();
		}
		
		internal override int StreamWrite(System.IO.Stream stream)
		{
			int num=this.id;
			string text=this.CompleteLineStream();
			Byte[] part2;
			if (PdfDocument.FlateCompression) part2=Utility.Deflate(text); else
				part2=System.Text.ASCIIEncoding.ASCII.GetBytes(text);

			string s1="";
			s1+=num.ToString()+" 0 obj\n";
			s1+="<< /Lenght "+part2.Length;
			if (PdfDocument.FlateCompression) s1+=" /Filter /FlateDecode";
			s1+=">>\n";
			s1+="stream\n";

			string s3="\nendstream\n";
			s3+="endobj\n";
				
			Byte[] part1=System.Text.ASCIIEncoding.ASCII.GetBytes(s1);
			Byte[] part3=System.Text.ASCIIEncoding.ASCII.GetBytes(s3);
				
				
			stream.Write(part1,0,part1.Length);
			stream.Write(part2,0,part2.Length);
			stream.Write(part3,0,part3.Length);

			return part1.Length+part2.Length+part3.Length;
		}

	}
}
