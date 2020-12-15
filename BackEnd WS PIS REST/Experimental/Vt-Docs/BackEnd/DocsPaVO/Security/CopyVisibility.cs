using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Security
{
    [Serializable()]
    public class CopyVisibility
    {
        //Ruolo Origine
        public string idCorrGlobRuoloOrigine  { get; set; }
        public string idGruppoRuoloOrigine  { get; set; }
        public string descRuoloOrigine { get; set; }
        public string codRuoloOrigine  { get; set; }
        
        //Ruolo Destinazione
        public string idCorrGlobRuoloDestinazione { get; set; }
        public string idGruppoRuoloDestinazione { get; set; }
        public string descRuoloDestinazione { get; set; }
        public string codRuoloDestinazione { get; set; }

        //Criteri di copia
        public string idAmm { get; set; }
        public bool docProtocollati { get; set; }
        public bool docNonProtocollati { get; set; }
        public bool fascicoliProcedimentali { get; set; }
        public bool visibilitaAttiva { get; set; }
        public bool precedenteCopiaVisibilita { get; set; }
        public string estendiVisibilita { get; set; }                
    }
}
