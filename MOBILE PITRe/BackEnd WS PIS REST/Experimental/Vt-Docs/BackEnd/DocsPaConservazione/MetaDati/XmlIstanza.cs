using System;
using System.Collections.Generic;
using DocsPaConservazione.Metadata.Istanza;
//using DocsPaVO.utente;
using DocsPaVO.areaConservazione;
using BusinessLogic.Utenti;
using DocsPaConservazione.Metadata.Common;

namespace DocsPaConservazione.Metadata
{
    public class XmlIstanza
    {
        DocsPaConservazione.Metadata.Istanza.Istanza istanza;

        public string  XmlFile 
        {
            get
            {
               
                return   Utils.SerializeObject<DocsPaConservazione.Metadata.Istanza.Istanza>(istanza,true );
            }
        }

        public  DocsPaConservazione.Metadata.Istanza.Istanza  Istanza
        {
            get
            {
                return istanza;
            }
            set
            {
                value = istanza;
            }

        }



        public XmlIstanza(InfoConservazione infoCons, DocsPaVO.utente.InfoUtente infoUtenteConservazione)
        {
            if (this.istanza == null)
                istanza = new DocsPaConservazione.Metadata.Istanza.Istanza();


            istanza.ID = infoCons.SystemID;
            // istanza.DataCreazione = Utils.formattaData(Utils.convertiData(infoCons.Data_Apertura)); //<<<--??
            // istanza.DataInvio = Utils.formattaData(Utils.convertiData(infoCons.Data_Invio));
            istanza.DataCreazione = infoCons.Data_Apertura; //<<<--??
            istanza.DataInvio = infoCons.Data_Invio;
            // istanza.DataChiusura = Utils.formattaData(DateTime.Now);
            istanza.DataChiusura = Utils.FormattaDataOraIta(DateTime.Now);
            istanza.Descrizione = infoCons.Descrizione;
            istanza.Tipologia = infoCons.TipoConservazione;

            DocsPaVO.utente.Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuolo(infoCons.IdRuoloInUo);
            DocsPaVO.utente.UnitaOrganizzativa unitaOrganizzativa = ruolo.uo;
           
            List<UnitaOrganizzativa> uoL = new List<UnitaOrganizzativa>();
            UnitaOrganizzativa uoXML = Utils.convertiUO(unitaOrganizzativa);
            uoL.Add(uoXML);

            istanza.ResponsabileConservazione = new ResponsabileConservazione { 
                Utente = UserManager.getUtente(infoUtenteConservazione.idPeople).descrizione 
            };

            istanza.SoggettoProduttore = new SoggettoProduttore { 
                Amministrazione = Utils.getInfoAmministrazione (infoCons), 
                GerarchiaUO = uoL.ToArray() ,
                Creatore = Utils.getCreatore(infoCons, ruolo) 
            };
        }
    }
}
