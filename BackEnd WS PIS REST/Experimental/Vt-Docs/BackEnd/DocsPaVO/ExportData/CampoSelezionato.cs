using System;
using System.Collections.Generic;
using System.Text;
    
namespace DocsPaVO.ExportData
{
    public class CampoSelezionato    
    {
        public string nomeCampo = string.Empty;
        public string campoComune = string.Empty;
        public string campoStandard = string.Empty;
        public string fieldID = string.Empty;

        /// <summary>
        /// System id del campo. Viene preso in considerazione se sono attive le griglie e
        /// si sta esportando un campo profilato. Deve essere valorizzato con il system id del campo profilato
        /// </summary>
        public int SystemId { get; set; }

        public CampoSelezionato() 
        {
            this.SystemId = 0;
        }
    }
}
