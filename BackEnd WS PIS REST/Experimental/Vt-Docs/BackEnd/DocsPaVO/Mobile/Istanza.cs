using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocsPaVO.Mobile
{
    public class Istanza
    {
        public string Nome { get; set; }
        public string Descrizione { get; set; }
        public string Tipo { get; set; }
        public string URL { get; set; }

        public Istanza() { }

        public Istanza(string nome, string descrizione, string tipo, string url)
        {
            this.Nome = nome;
            this.Descrizione = descrizione;
            this.Tipo = tipo;
            this.URL = url;
        }
    }
}
