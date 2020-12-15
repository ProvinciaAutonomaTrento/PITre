using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Qualifica
{
    [Serializable()]
    public class PeopleGroupsQualifiche
    {
        public int SYSTEM_ID;

        public int ID_AMM;

        public int ID_UO;

        public int ID_GRUPPO;

        public int ID_PEOPLE;

        public int ID_QUALIFICA;

        public String CODICE;

        public string DESCRIZIONE;

        public PeopleGroupsQualifiche() { }
    } 
}
