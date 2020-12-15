using System.Collections.Generic;
using DocsPaVO.amministrazione;
using DocsPaVO.areaConservazione;
using DocsPaConservazione.Metadata.Fascicolo;
using DocsPaConservazione.Metadata.Common;
using BusinessLogic.ExportDati;
using BusinessLogic.Utenti;
using DocsPaVO.documento;
using System.Linq;
using System;

namespace DocsPaConservazione.Metadata
{
    public class XmlFascEsibizione
    {

        DocsPaConservazione.Metadata.Fascicolo.Fascicolo fascicolo;

        public string XmlFile
        {
            get
            {
                return Utils.SerializeObject<DocsPaConservazione.Metadata.Fascicolo.Fascicolo>(fascicolo, true);
            }
        }

        public DocsPaConservazione.Metadata.Fascicolo.Fascicolo Istanza
        {
            get
            {
                return fascicolo;
            }
            set
            {
                value = fascicolo;
            }
        }

        public XmlFascEsibizione(InfoEsibizione infoEs, string ID_Project, FolderConservazione[] folderConservazione)
        {

            if (fascicolo == null)
                fascicolo = new Fascicolo.Fascicolo();

            DocsPaVO.utente.Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuolo(infoEs.IdRuoloInUo);
            DocsPaVO.utente.Utente Utente = UserManager.getUtente(infoEs.IdPeople);
            DocsPaVO.utente.UnitaOrganizzativa unitaOrganizzativa = ruolo.uo;
            DocsPaVO.utente.InfoUtente infoUtente = UserManager.GetInfoUtente(Utente, ruolo);
            DocsPaVO.fascicolazione.Fascicolo dpaFascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(ID_Project, infoUtente);

      
            List<UnitaOrganizzativa> uoL = new List<UnitaOrganizzativa>();
            UnitaOrganizzativa uoXML = Utils.convertiUO(unitaOrganizzativa);
            uoL.Add(uoXML);

            InfoAmministrazione infoAmm = BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(infoEs.IdAmm);

            fascicolo.SoggettoProduttore = new SoggettoProduttore
            {
                Amministrazione = new Metadata.Common.Amministrazione { CodiceAmministrazione = infoAmm.Codice, DescrizioneAmministrazione = infoAmm.Descrizione },
                GerarchiaUO = new GerarchiaUO { UnitaOrganizzativa = uoL.ToArray() },
                Creatore = new Creatore()
                {
                    CodiceRuolo = ruolo.codiceRubrica,
                    DescrizioneRuolo = ruolo.descrizione,
                    CodiceUtente = UserManager.getUtente(infoEs.IdPeople).userId,
                    DescrizioneUtente = UserManager.getUtente(infoEs.IdPeople).descrizione
                }
            };

            if (dpaFascicolo!=null && dpaFascicolo.template != null)
            {
                Tipologia t = new Tipologia { NomeTipologia = dpaFascicolo.template.DESCRIZIONE, CampoTipologia = Utils.getCampiTipologia(dpaFascicolo.template) };
                fascicolo.Tipologia = t;
            }

            if(!string.IsNullOrEmpty(fascicolo.Codice))
            {
                fascicolo.Codice = dpaFascicolo.codice;
                fascicolo.DataChiusura = Utils.formattaData(Utils.convertiData(dpaFascicolo.chiusura));
                fascicolo.DataCreazione = Utils.formattaData(Utils.convertiData(dpaFascicolo.apertura));
                fascicolo.Descrizione = dpaFascicolo.descrizione;

                OrgNodoTitolario nodo = BusinessLogic.Amministrazione.TitolarioManager.getNodoTitolario(dpaFascicolo.idTitolario);
                fascicolo.TitolarioDiRiferimento = nodo.Descrizione;
                fascicolo.Classificazione = nodo.Codice;

                fascicolo.LivelloRiservatezza = Utils.convertiLivelloRiservatezza(dpaFascicolo.privato);
                fascicolo.Numero = dpaFascicolo.numFascicolo;
                fascicolo.Contenuto = creaStrutturaContenuto(folderConservazione, ID_Project, infoUtente, ref fascicolo);
            }
        }

        public Contenuto creaStrutturaContenuto(FolderConservazione[] folders, string idFascicolo, DocsPaVO.utente.InfoUtente infoUtente, ref DocsPaConservazione.Metadata.Fascicolo.Fascicolo fascicolo)
        {
            Contenuto cnt = new Contenuto();
            List<Sottofascicolo> sfLst = new List<Sottofascicolo>();
            List<object> sfLstPuliti = new List<object>();

            int nrDocumenti = 0;
            int nrSottofascicoli = 0;
            List<object> contentItems = new List<object>();
            List<DateTime> dtList = new List<DateTime>();

            foreach (FolderConservazione f in folders)
            {

                if (f.parent != idFascicolo)//E' il fascicolo stesso (record C)
                    nrSottofascicoli++;

                List<object> objSfLst = new List<object>();
                if (f.ID_Profile != null)
                {
                    foreach (string item in f.ID_Profile)
                    {
                        nrDocumenti++;
                        InfoDocumento id = BusinessLogic.Documenti.DocManager.GetInfoDocumento(infoUtente, item, item);
                        DateTime dc = Utils.convertiData(id.dataApertura);
                        dtList.Add(dc);
                        String dataCreazione = Utils.formattaData(dc);

                        Fascicolo.Documento d = new Fascicolo.Documento { IDdocumento = item, Oggetto = id.oggetto, DataCreazione = dataCreazione, LivelloRiservatezza = Utils.convertiLivelloRiservatezza(id.privato) };
                        // Documento d = new Documento { IDdocumento = item };

                        objSfLst.Add(d);
                    }
                }
                Sottofascicolo sottof = new Sottofascicolo { Codice = f.systemID, Descrizione = f.descrizione, parent = f.parent, Items = objSfLst.ToArray() };
                sfLst.Add(sottof);
            }

            List<FolderConservazione> fclst = new List<FolderConservazione>();
            foreach (Sottofascicolo s in sfLst)
            {
                var b = (from pippo in sfLst where pippo.parent == s.Codice select pippo).ToArray();
                List<object> otmp = s.Items.ToList();
                otmp.AddRange(b);
                s.Items = otmp.ToArray();
            }

            foreach (Sottofascicolo s in sfLst)
                if (s.parent == idFascicolo)
                    foreach (object si in s.Items)
                        sfLstPuliti.Add(si);


            cnt.Items = sfLstPuliti.ToArray();
            cnt.ConsistenzaDocumenti = nrDocumenti.ToString();
            cnt.ConsistenzaSottofascicoli = nrSottofascicoli.ToString();

            dtList.Sort();

            fascicolo.EstremoCronologicoInferiore = Utils.formattaData(dtList.LastOrDefault());
            fascicolo.EstremoCronologicoSuperiore = Utils.formattaData(dtList.FirstOrDefault());
            return cnt;
        }

    }
}
