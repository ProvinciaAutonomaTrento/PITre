<%@ Control Language="c#" AutoEventWireup="false" Codebehind="StampaEtichetta.ascx.cs" Inherits="ProtocollazioneIngresso.StampaEtichetta" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<html>
	<head>
    <title></title>
    <object id="ctrlPrintPen" codebase="../activex/DocsPa_PrintPen.CAB#version=1,1,0,0" height="0px"
	    width="0px" classid="CLSID:2860F27F-FC9F-4CDA-B0CB-55A5BD09C52E" >
	    <param name="_ExtentX" value="26"/>
	    <param name="_ExtentY" value="26"/>
	    <param name="PortaCOM" value="1"/>
	    <param name="Text" value="DFPrintPen Test"/>
	    <param name="NumCopie" value="1"/>
	    <param name="TimeOut" value="60"/>
	    <param name="Dispositivo" value="Penna"/>
	    <param name="Amministrazione" value=""/>
	    <param name="UrlFileIni" value=""/>
	    <param name="Fascicolo" value=""/>
	    <param name="Classifica" value=""/>
	    <param name="Amministrazione_Etichetta" value=""/>
	    <param name="NumeroDocumento" value=""/>
	    <param name="CodiceUoProtocollatore" value=""/>
	    <param name="CodiceRegistroProtocollo" value=""/>
	    <param name="TipoProtocollo" value=""/>
	    <param name="NumeroProtocollo" value=""/>
	    <param name="AnnoProtocollo" value=""/>
	    <param name="DataProtocollo" value=""/>
	    <param name="NumeroAllegati" value=""/>
        <param name="NumeroStampe" value=""/>
        <param name="NumeroStampeEffettuate" value=""/>
        <param name="NumeroStampaCorrente" value=""/>
        <param name="OraCreazione" value=""/>
	    <param name="Q1" value="Q690"/>
	    <param name="Q2" value="24+0"/>
	    <param name="P1" value="A63"/>
	    <param name="P2" value="696"/>
	    <param name="P3" value="3"/>
	    <param name="P4" value="4"/>
	    <param name="P5" value="3"/>
	    <param name="P6" value="1"/>
    </object>

    
    <script type="text/javascript">

	// Caricamento dati stampa
	function FillPrintData() {
	    document.frmProtIngresso.ctrlPrintPen.Classifica = document.frmProtIngresso.StampaEtichetta_hd_classifica.value;
	    document.frmProtIngresso.ctrlPrintPen.Fascicolo = document.frmProtIngresso.StampaEtichetta_hd_fascicolo.value;
		document.frmProtIngresso.ctrlPrintPen.Amministrazione_Etichetta = document.frmProtIngresso.StampaEtichetta_hd_amministrazioneEtichetta.value;
        document.frmProtIngresso.ctrlPrintPen.NumeroDocumento = document.frmProtIngresso.StampaEtichetta_hd_num_doc.value;
        document.frmProtIngresso.ctrlPrintPen.CodiceUoProtocollatore = document.frmProtIngresso.StampaEtichetta_hd_coduo_proto.value;
        document.frmProtIngresso.ctrlPrintPen.CodiceRegistroProtocollo = document.frmProtIngresso.StampaEtichetta_hd_codreg_proto.value;
        document.frmProtIngresso.ctrlPrintPen.TipoProtocollo = document.frmProtIngresso.StampaEtichetta_hd_tipo_proto.value;
        document.frmProtIngresso.ctrlPrintPen.NumeroProtocollo = document.frmProtIngresso.StampaEtichetta_hd_num_proto.value;
        document.frmProtIngresso.ctrlPrintPen.AnnoProtocollo = document.frmProtIngresso.StampaEtichetta_hd_anno_proto.value;
        document.frmProtIngresso.ctrlPrintPen.DataProtocollo = document.frmProtIngresso.StampaEtichetta_hd_data_proto.value;
        document.frmProtIngresso.ctrlPrintPen.UrlFileIni = document.frmProtIngresso.StampaEtichetta_hd_UrlIniFileDispositivo.value;
        document.frmProtIngresso.ctrlPrintPen.Amministrazione = document.frmProtIngresso.StampaEtichetta_hd_descrizioneAmministrazione.value;
        document.frmProtIngresso.ctrlPrintPen.Dispositivo = document.frmProtIngresso.StampaEtichetta_hd_dispositivo.value;
        document.frmProtIngresso.ctrlPrintPen.Text = document.frmProtIngresso.StampaEtichetta_hd_signature.value;
        document.frmProtIngresso.ctrlPrintPen.NumeroStampe = document.frmProtIngresso.StampaEtichetta_hd_num_stampe.value;
        document.frmProtIngresso.ctrlPrintPen.NumeroStampeEffettuate = document.frmProtIngresso.StampaEtichetta_hd_num_stampe_effettuate.value;
        document.frmProtIngresso.ctrlPrintPen.NumeroStampaCorrente = document.frmProtIngresso.StampaEtichetta_hd_num_stampe_effettuate.value;
        document.frmProtIngresso.ctrlPrintPen.OraCreazione = document.frmProtIngresso.StampaEtichetta_hd_ora_creazione.value;
        try
		{   
		    // NB: attributo presente a partire dalla versione 3.5.0 dell'ocx printpen
		    document.frmProtIngresso.ctrlPrintPen.NumeroAllegati = document.frmProtIngresso.StampaEtichetta_hd_numeroAllegati.value;
		}
		catch(ex)
		{}
		
	    try
	    {   
	        // NB: attributo presente a partire dalla versione 3.6.1 dell'ocx printpen
	        document.frmProtIngresso.ctrlPrintPen.DataCreazione = document.frmProtIngresso.StampaEtichetta_hd_dataCreazione.value;
	        document.frmProtIngresso.ctrlPrintPen.CodiceUoCreatore = document.frmProtIngresso.StampaEtichetta_hd_codiceUoCreatore.value;
	    }
	    catch(ex)
	    {}
	    
	    try
	    {   
	        // NB: attributo presente a partire dalla versione 3.7.12 dell'ocx printpen
	        document.frmProtIngresso.ctrlPrintPen.DescrizioneRegistroProtocollo = document.frmProtIngresso.StampaEtichetta_hd_descreg_proto.value;
	        document.frmProtIngresso.ctrlPrintPen.ModelloDispositivo = document.frmProtIngresso.StampaEtichetta_hd_modello_dispositivo.value;
        }
        catch(ex)
	    {}
	}

	// stampa segnatura protocollo
	function PrintSignature(showMsg) {

		var retValue=showMsg;
		
		if (retValue)
		{
			retValue=window.confirm("Si desidera stampare la segnatura?");
		}
		else
		{
			retValue=true;
		}
					
		if (retValue) 
		{
			try {

				// Caricamento dati stampa
			    FillPrintData();
                var result=false;
                var i;
                var num_stampa_corr=document.frmProtIngresso.StampaEtichetta_hd_num_stampe_effettuate.value;
                for (i = 0; i < document.frmProtIngresso.StampaEtichetta_hd_num_stampe.value; i++) 
                {
                    var result = document.frmProtIngresso.ctrlPrintPen.Stampa();
                    ++num_stampa_corr;
                    document.frmProtIngresso.ctrlPrintPen.NumeroStampaCorrente = num_stampa_corr.toString();
                }
				if (result) {
				    // Stampa eseguita correttamente,
				    // aggiornamento contatore numero stampe effettuate
				    // richiamando la pagina "WriteContatoreStampaEtichetta.aspx"
				    // fornendo, come parametri da querystring, 
				    // - il numero di stampe effettuate finora
				    // - il numero di stampe da effettuare
				    
                    var http = CreateObject("MSXML2.XMLHTTP");
				    /*http.Open("POST",
                        "WriteContatoreStampaEtichetta.aspx?numeroStampeEffettuate=" + document.frmProtIngresso.ctrlPrintPen.NumeroStampeEffettuate + "&numeroStampeDaEffettuare=" + document.frmProtIngresso.ctrlPrintPen.NumeroStampe, false);
				    http.send();*/

				    http.Open("POST",
                         "WriteContatoreStampaEtichetta.aspx?numeroStampeEffettuate=" + (parseInt(document.frmProtIngresso.ctrlPrintPen.NumeroStampeEffettuate) - 1).toString() + "&numeroStampeDaEffettuare=" + document.frmProtIngresso.ctrlPrintPen.NumeroStampe, false);
				    http.send();
				}

			} 
			catch(e) 
			{
				alert("Errore nella stampa della segnatura o stampante etichette non installata");
			}					
		}
	}
	
</script>

    </head>
	<body>
        <input id="hd_fascicolo" type="hidden" name="hd_fascicolo" runat="server"/>
        <input id="hdnSpedisciConInterop" type="hidden" name="hdnSpedisciConInterop"/>
        <input id="hd_num_proto" type="hidden" name="hd_num_proto" runat="server"/>
        <input id="hd_anno_proto" type="hidden" name="hd_anno_proto" runat="server"/>
        <input id="hd_data_proto" type="hidden" name="hd_data_proto" runat="server"/>
        <input id="hd_classifica" type="hidden" name="hd_classifica" runat="server"/>
        <input id="hd_dispositivo" type="hidden" name="hd_dispositivo" runat="server"/>
        <input id="hd_amministrazioneEtichetta" type="hidden" name="hd_amministrazioneEtichetta" runat="server"/> 
        <input id="hd_descrizioneAmministrazione" type="hidden" name="hd_descrizioneAmministrazione" runat="server"/>
        <input id="hd_UrlIniFileDispositivo" type="hidden" name="hd_UrlIniFileDispositivo" runat="server"/>
        <input id="hd_tipoProtocollazione" type="hidden" name="hd_tipoProtocollazione" runat="server"/>
        <input id="hd_coduo_proto" type="hidden" name="hd_coduo_proto" runat="server"/>
        <input id="hd_codreg_proto" type="hidden" name="hd_codreg_proto" runat="server"/>
        <input id="hd_tipo_proto" type="hidden" name="hd_tipo_proto" runat="server"/>
        <input id="hd_num_doc" type="hidden" name="hd_num_doc" runat="server"/>
        <input id="hd_signature" type="hidden" name="hd_num_doc" runat="server" />
	    <input id="hd_numeroAllegati" type="hidden" name="hd_numeroAllegati" runat="server" />
	    <input id="hd_codiceUoCreatore" type="hidden" name="hd_codiceUoCreatore" runat="server" />
	    <input id="hd_dataCreazione" type="hidden" name="hd_dataCreazione" runat="server" />
	    <input id="hd_descreg_proto" type="hidden" name="hd_descreg_proto" runat="server" />
        <input id="hd_modello_dispositivo" type="hidden" name="hd_modello_dispositivo" runat="server" />
        <input id="hd_num_stampe" type="hidden" name="hd_num_stampe" runat="server" />
        <input id="hd_num_stampe_effettuate" type="hidden" name="hd_num_stampe_effettuate" runat="server" /> 
        <input id="hd_ora_creazione" type="hidden" name="hd_ora_creazione" runat="server" /> 
    </body>
</html>
