using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPAWA.Deleghe
{
    public class OrganigrammaTreeNode : Microsoft.Web.UI.WebControls.TreeNode
    {
        // Tipo Nodo [Possibili Valori: U=(Unità organizz.), R=(Ruolo), U=(Utente) ]
        public string getTipoNodo()
        {
            return ViewState["TipoNodo"].ToString();
        }
        public void setTipoNodo(string id)
        {
            ViewState["TipoNodo"] = id;
        }

        // IDCorrGlobale
        public string getIDCorrGlobale()
        {
            return ViewState["IDCorrGlobale"].ToString();
        }
        public void setIDCorrGlobale(string id)
        {
            ViewState["IDCorrGlobale"] = id;
        }

        // Codice
        public string getCodice()
        {
            return ViewState["Codice"].ToString();
        }
        public void setCodice(string id)
        {
            ViewState["Codice"] = id;
        }

        // CodiceRubrica
        public string getCodiceRubrica()
        {
            return ViewState["CodiceRubrica"].ToString();
        }
        public void setCodiceRubrica(string id)
        {
            ViewState["CodiceRubrica"] = id;
        }

        // Descrizione
        public string getDescrizione()
        {
            return ViewState["Descrizione"].ToString();
        }
        public void setDescrizione(string id)
        {
            ViewState["Descrizione"] = id;
        }

        // Livello
        public string getLivello()
        {
            return ViewState["Livello"].ToString();
        }
        public void setLivello(string id)
        {
            ViewState["Livello"] = id;
        }

        // Amministrazione
        public string getIDAmministrazione()
        {
            return ViewState["IDAmministrazione"].ToString();
        }
        public void setIDAmministrazione(string id)
        {
            ViewState["IDAmministrazione"] = id;
        }

        // AOO interoperabilità
        public string getCodRegInterop()
        {
            return ViewState["CodRegInterop"].ToString();
        }
        public void setCodRegInterop(string id)
        {
            ViewState["CodRegInterop"] = id;
        }

        // Tipo ruolo
        public string getIDTipoRuolo()
        {
            return ViewState["IDTipoRuolo"].ToString();
        }
        public void setIDTipoRuolo(string id)
        {
            ViewState["IDTipoRuolo"] = id;
        }

        // ID Groups
        public string getIDGruppo()
        {
            return ViewState["IDGruppo"].ToString();
        }
        public void setIDGruppo(string id)
        {
            ViewState["IDGruppo"] = id;
        }

        // ID People
        public string getIDPeople()
        {
            return ViewState["IDPeople"].ToString();
        }
        public void setIDPeople(string id)
        {
            ViewState["IDPeople"] = id;
        }

        // Ruolo Di riferimento
        public string getDiRiferimento()
        {
            return ViewState["DiRiferimento"].ToString();
        }
        public void setDiRiferimento(string id)
        {
            ViewState["DiRiferimento"] = id;
        }

        // Percorso
        public string getPercorso()
        {
            return ViewState["Percorso"].ToString();
        }
        public void setPercorso(string id)
        {
            ViewState["Percorso"] = id;
        }

        //Ruolo Responsabile
        public string getResponsabile()
        {
            return ViewState["Responsabile"].ToString();
        }
        public void setResponsabile(string id)
        {
            ViewState["Responsabile"] = id;
        }

        public string getRuoliUO()
        {
            return ViewState["RuoliUO"].ToString();
        }
        public void setRuoliUO(string id)
        {
            ViewState["RuoliUO"] = id;
        }

        public string getSottoUO()
        {
            return ViewState["SottoUO"].ToString();
        }
        public void setSottoUO(string id)
        {
            ViewState["SottoUO"] = id;
        }
    }
}