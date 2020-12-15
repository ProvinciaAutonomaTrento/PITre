using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.filtri;

namespace DocsPaVO.Mobile
{
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://localhost")]
    public class FiltroRicercaWrapper : FiltroRicerca
    {

    }

    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class ArrayOfFiltroRicerca
    {
        [System.Xml.Serialization.XmlElementAttribute("FiltroRicerca")]
        public FiltroRicercaWrapper[] filtroRicerca;

    }

    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class ArrayOfArrayOfFiltroRicerca
    {
        [System.Xml.Serialization.XmlElementAttribute("ArrayOfFiltroRicerca")]
        public ArrayOfFiltroRicerca[] arrayOfFiltroRicerca;

        [System.Xml.Serialization.XmlIgnore()]
        public FiltroRicerca[][] Filtri
        {
            get
            {
                FiltroRicerca[][] res=new FiltroRicerca[arrayOfFiltroRicerca.Length][];
                for(int i=0;i<arrayOfFiltroRicerca.Length;i++)
                {
                    res[i]=arrayOfFiltroRicerca[i].filtroRicerca;
                }
                return res;
            }
        }
    }
}
