using System;
using System.Collections;

namespace DocsPaAS400.fields
{
	/// <summary>
	/// Summary description for Oggetto.
	/// </summary>
	public class Oggetto
	{
		private string val;
        private string idReg;
		private string idAmm;
		private string numProt;

		public Oggetto(string val)
		{
			this.val=val;
		}

		public Oggetto(ArrayList valueList)
		{
            this.val=concatValues(valueList);
		}

		public string getValue()
		{
             return this.val;
		}

		public string getIdAmm()
		{
             return this.idAmm;
		}

		public string getIdReg()
		{
             return this.idReg;
		}

		public string getNumProt()
		{
			return this.numProt;
		}

		public void setIdAmm(string idAmm)
		{
             this.idAmm=idAmm;
		}

		public void setIdReg(string idReg)
		{
             this.idReg=idReg;
		}

		public void setNumProt(string numProt)
		{
			this.numProt=numProt;
		}

		public ArrayList split(int rowSize)
		{
			ArrayList res=new ArrayList();
			int numOggettoRows=getNumRows(val.Length,rowSize);
			for(int i=0;i<numOggettoRows;i++)
			{
				string temp=null;
				if(val.Substring(rowSize*i).Length>=rowSize)
				{
					temp=val.Substring(rowSize*i,rowSize);
				}
				else
				{
					temp=val.Substring(rowSize*i);
				}
                res.Add(temp);
			}
			return res;
		}

		private int getNumRows(int totalSize,int rowSize)
		{ 
			int res=0;
			if(totalSize % rowSize == 0)
			{
				res=totalSize/rowSize;

			}
			else
			{
				res=(totalSize/rowSize)+1;
			}
			return res;

		}

		private string concatValues(ArrayList values)
		{
            string res=null;
			for(int i=0;i<values.Count;i++)
			{
               res=res+(string) values[i];
			}
			return res;
		}
	}
}
