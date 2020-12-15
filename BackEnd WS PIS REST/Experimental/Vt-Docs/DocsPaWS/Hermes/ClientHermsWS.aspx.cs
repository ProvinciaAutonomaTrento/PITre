using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocsPaWS.Hermes
{
    public partial class ClientHermsWS : System.Web.UI.Page
    {
        BandoSyncRequest _bando = new BandoSyncRequest();
        protected void Page_Load(object sender, EventArgs e)
        {

            _bando.Mandante = "mandante";
            _bando.NumeroGaraAppalto = 1;
            _bando.OrganizzazioneAcquisti = "è proprio lui";
            _bando.GruppoAcquisti = "gruppo :s";
            _bando.DescrizioneGara = "Ecco, quesst è la mia gara";
            _bando.DataScadenzaGara = DateTime.Now;
            _bando.DataRicezioneRelazioneTecnica = DateTime.Now;
            _bando.DataPropostaAggiudicazioneTrattativa = DateTime.Now;
            _bando.DataEmissioneGara = DateTime.Now;
            _bando.DataAperturaBusteTecniche = DateTime.Now;
            _bando.DataAperturaBusteEconomiche = DateTime.Now;
            _bando.DataAnnulamentoGara = DateTime.Now;
            _bando.DataAggiudicazione = DateTime.Now;
            
        }

        protected void invio_Click(object sender, EventArgs e)
        {
            Hermes h = new Hermes();
            BandoSyncResponse risposta =  h.InsertBando(_bando);
            l_risposta.Text = risposta.DescrEsito;
        }
    }
}