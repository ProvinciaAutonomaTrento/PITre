using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.FlussoAutomatico
{
    public class FlussoMessaggi
    {
        private Messaggio messaggio;
        private List<Messaggio> messaggiSuccessivi;

        public Messaggio MESSAGGIO
        {
            get
            {
                return messaggio;
            }
            set
            {
                messaggio = value;
            }
        }

        public List<Messaggio> MESSAGGI_SUCCESSIVI
        {
            get
            {
                return messaggiSuccessivi;
            }
            set
            {
                messaggiSuccessivi = value;
            }
        }
    }
}
