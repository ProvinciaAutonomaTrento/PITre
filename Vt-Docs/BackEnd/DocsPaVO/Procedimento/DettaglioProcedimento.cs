using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Procedimento
{
    public class DettaglioProcedimento
    {
        private String _idProject;
        private String _idTipoAtto;
        private String _descrizione;
        private String _descTipoAtto;
        private StatoProcedimento[] _stati;

        public String IdProject
        {
            get
            {
                return _idProject;
            }
            set
            {
                _idProject = value;
            }
        }

        public String Descrizione
        {
            get
            {
                return _descrizione;
            }
            set
            {
                _descrizione = value;
            }
        }

        public String IdTipoAtto
        {
            get
            {
                return _idTipoAtto;
            }
            set
            {
                _idTipoAtto = value;
            }
        }

        public String DescrizioneTipoAtto
        {
            get
            {
                return _descTipoAtto;
            }
            set
            {
                _descTipoAtto = value;
            }
        }

        public StatoProcedimento[] Stati
        {
            get
            {
                return _stati;
            }
            set
            {
                _stati = value;
            }
        }
    }
}
