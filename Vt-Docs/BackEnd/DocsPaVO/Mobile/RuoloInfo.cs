using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.utente;
using DocsPaVO.amministrazione;

namespace DocsPaVO.Mobile
{
    public class RuoloInfo
    {

        public string Descrizione
        {
            get; 
            set; 
        }

        public string IdGruppo
        {
            get; 
            set;
        }

        public string Codice
        {
            get; 
            set;
        }

        public string IdUO
        {
            get;
            set;
        }

        public string Livello
        {
            get;
            set;
        }

        public List<RegistroInfo> Registri
        {
            get; 
            set; 
        }

        public string Id
        {
            get; 
            set;
        }

        public static RuoloInfo buildInstance(Ruolo input)
        {
            RuoloInfo res = new RuoloInfo();
            res.Descrizione=input.descrizione;
            List<RegistroInfo> listReg = new List<RegistroInfo>();
            foreach (Object reg in input.registri)
            {
                listReg.Add(RegistroInfo.buildInstance((Registro) reg));
            }
            res.Registri = listReg;
            res.IdGruppo = input.idGruppo;
            res.Id = input.systemId;
            res.Codice = input.codice;
            res.Livello = input.livello;
            if (input.uo != null) res.IdUO = input.uo.systemId;
            return res;
        }

    }
}