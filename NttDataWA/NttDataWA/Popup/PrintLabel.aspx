<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrintLabel.aspx.cs" Inherits="NttDataWA.Popup.PrintLabel" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
        <META HTTP-EQUIV="Pragma" CONTENT="no-cache">
        <META HTTP-EQUIV="Expires" CONTENT="-1">	
        <%// Carica activeX solo se la stampante non è Dymo e non sono in uso le applet
            if (componentType != "3" && componentType != "4" && printer != "DYMO_LABEL_WRITER_400")
          { %>
            <object id="ctrlPrintPen"
				    classid="CLSID:2860F27F-FC9F-4CDA-B0CB-55A5BD09C52E"
				    codebase="../activex/DocsPa_PrintPen.CAB#version=1,1,0,0" VIEWASTEXT>
		    </object>	
        <%} %>
        <title>Stampa in corso</title>
        <script src="../Scripts/jquery-1.8.1.min.js" type="text/javascript"></script>
        <script src="../Scripts/DYMO.Label.Framework.latest.js?v3" type="text/javascript"></script>
        <script src="../Scripts/webclientconnector.js" type="text/javascript"></script>
		<script type="text/javascript">
		    var appletS = undefined;

		    function start_stampe() {
//		            if (document.forms[0].hd_num_proto.value != "") {
		        var l_segnatura = document.forms[0].hd_num_proto.value;
		                pf_printSignatureWithPenna(l_segnatura);
//		            }
//		            else {
//		                alert('Numero di protocollo non assegnato!');
//		            }
		            parent.closeAjaxModal('PrintLabel', '');
		        
		    }

		    function pf_printSignatureWithPenna(signature) {
		        try {
		            ctrlPrintPen.Classifica = document.forms[0].hd_classifica.value;
		            ctrlPrintPen.Fascicolo = document.forms[0].hd_fascicolo.value;
		            ctrlPrintPen.Amministrazione_Etichetta = document.forms[0].hd_amministrazioneEtichetta.value;
		            ctrlPrintPen.NumeroDocumento = document.forms[0].hd_num_doc.value;
		            ctrlPrintPen.CodiceUoProtocollatore = document.forms[0].hd_coduo_proto.value;
		            ctrlPrintPen.CodiceRegistroProtocollo = document.forms[0].hd_codreg_proto.value;
		            ctrlPrintPen.TipoProtocollo = document.forms[0].hd_tipo_proto.value;
		            ctrlPrintPen.NumeroProtocollo = document.forms[0].hd_num_proto.value;
		            ctrlPrintPen.AnnoProtocollo = document.forms[0].hd_anno_proto.value;
		            ctrlPrintPen.DataProtocollo = document.forms[0].hd_data_proto.value;
		        }
		        catch (e) {
		        }

		        try {
		            //NB: attributi presente a partire dalla versione 3.5.0 dell'ocx printpen
		            ctrlPrintPen.NumeroAllegati = document.forms[0].hd_numero_allegati.value;
		        }
		        catch (e) {
		        }

		        try {
		            // NB: attributo presente a partire dalla versione 3.6.1 dell'ocx printpen
		            ctrlPrintPen.DataCreazione = document.forms[0].hd_dataCreazione.value;
		            ctrlPrintPen.CodiceUoCreatore = document.forms[0].hd_codiceUoCreatore.value;
		        }
		        catch (ex)
		    { }

		        try {
		            // NB: attributo presente a partire dalla versione 3.7.12 dell'ocx printpen
		            ctrlPrintPen.DescrizioneRegistroProtocollo = document.forms[0].hd_descreg_proto.value;
		        }
		        catch (ex)
		    { }

		        try {
		            ctrlPrintPen.UrlFileIni = document.forms[0].hd_UrlIniFileDispositivo.value;
		            ctrlPrintPen.Amministrazione = document.forms[0].hd_descrizioneAmministrazione.value;
		        }
		        catch (e) {
		        }
		        try {
		            ctrlPrintPen.DataArrivo = document.forms[0].hd_dataArrivo.value;
		            ctrlPrintPen.DataArrivoEstesa = document.forms[0].hd_dataArrivoEstesa.value;
		        }
		        catch (e) {
		        }
		        try {
		            ctrlPrintPen.Dispositivo = document.forms[0].hd_dispositivo.value;
		            ctrlPrintPen.ModelloDispositivo = document.forms[0].hd_modello_dispositivo.value;
		            ctrlPrintPen.NumeroStampe = document.forms[0].hd_num_stampe.value;
		            ctrlPrintPen.NumeroStampeEffettuate = document.forms[0].hd_num_stampe_effettuate.value;
		            ctrlPrintPen.NumeroStampaCorrente = document.forms[0].hd_num_stampe_effettuate.value;
		            ctrlPrintPen.OraCreazione = document.forms[0].hd_ora_creazione.value;
		            ctrlPrintPen.Text = signature;
		            var retValue = false;
		            var i;
		            var num_stampa_corr = document.forms[0].hd_num_stampe_effettuate.value;
		            for (i = 0; i < document.forms[0].hd_num_stampe.value; i++) {
		                // verifica doc grigio e richiama funzione corretta activeX
		                if (document.forms[0].hd_tipo_proto.value == 'NP') {
		                    try {
		                        ctrlPrintPen.StampaGrigio();
		                    }
		                    catch (e) {
		                        alert("Stampa documento grigio non supportata");
		                    }
		                }
		                else {
		                    retValue = ctrlPrintPen.Stampa();
		                }
		                ++num_stampa_corr;
		                ctrlPrintPen.NumeroStampaCorrente = num_stampa_corr.toString();
		            }
		            retValue = true;
		            if (retValue) {
		                // Stampa eseguita correttamente,
		                // aggiornamento contatore numero stampe effettuate
		                // richiamando la pagina "WriteContatoreStampaEtichetta.aspx"
		                // fornendo, come parametri da querystring, 
		                // - il numero di stampe effettuate finora
		                // - il numero di stampe da effettuare
//		                var http = CreateObject("MSXML2.XMLHTTP");
		                /*http.Open("POST",
		                "WriteContatoreStampaEtichetta.aspx?numeroStampeEffettuate=" + ctrlPrintPen.NumeroStampeEffettuate + "&numeroStampeDaEffettuare=" + ctrlPrintPen.NumeroStampe, false);*/
//		                http.Open("POST",
//                         "WriteContatoreStampaEtichetta.aspx?numeroStampeEffettuate=" + (parseInt(ctrlPrintPen.NumeroStampeEffettuate) - 1).toString() + "&numeroStampeDaEffettuare=" + ctrlPrintPen.NumeroStampe, false);
//		                http.send();
		            }
		        }
		        catch (e) {
		            alert("Errore.\n" + e.message.toString());
		        }
		    }

		    // Creazione oggetto activex con gestione errore
		    function CreateObject(objectType) {
		        try {
		            return new ActiveXObject(objectType);
		        }
		        catch (ex) {
		            alert("Oggetto '" + objectType + "' non istanziato");
		        }
		    }

		    function PrintWithApplet(strToPrint, printer) {
		        parent.$('#PrintLabel_panel>#ifrm_PrintLabel').show();
		        if (strToPrint != '') {
		            if (appletS == undefined) {
		                appletS = window.document.plugins[0];
		            }

		            if (appletS == undefined) {
		                appletS = document.applets[0];
		                if (appletS == undefined) {
		                    appletS = document.applets[1];
		                }
		            }
                  
		            //alert(appletS.testChiamata('Ciao'));
		            //alert(strToPrint);
                    var num_stampe = 0;
                    if (parent.document.getElementById('TxtPrintLabel') == undefined) {
                        num_stampe = 1;
                    }
                    else {
                        num_stampe = parent.document.getElementById('TxtPrintLabel').value; 
                    }
		            try {
		                 //document.forms[0].hd_num_stampe_effettuate.value;
		                for (i = 0; i < num_stampe; i++) {
		                    appletS.print(strToPrint, printer);
		                }
		            }
		            catch (ex) {
		                alert('Applet error.');
		            }

		            //alert('Inviato a stampante.');
		        }
		        else {
		            alert('Errori nella stampa.')
		        }

		        parent.closeAjaxModal('PrintLabel', '');
		        return true;
		    }

			function PrintWithSocket(strToPrint, printer) {
		        parent.$('#PrintLabel_panel>#ifrm_PrintLabel').show();
		        if (strToPrint != '') {
		            //alert(appletS.testChiamata('Ciao'));
		            //alert(strToPrint);
                    var num_stampe = 0;
                    if (parent.document.getElementById('TxtPrintLabel') == undefined) {
                        num_stampe = 1;
                    }
                    else {
                        num_stampe = parent.document.getElementById('TxtPrintLabel').value; 
                    }
		            try {
		                //document.forms[0].hd_num_stampe_effettuate.value;
		                var callback = null;
		                for (i = 0; i < num_stampe; i++) {
		                    if (i == (num_stampe-1))
		                        callback = function (msg, connection) { parent.closeAjaxModal('PrintLabel', ''); connection.close(); };
		                    printA(strToPrint, printer, callback);
		                }
		            }
		            catch (ex) {
		                alert('Socket error.');
		                alert("Errore.\n" + ex.message.toString());
		            }
		        }
		        else {
		            alert('Errori nella stampa.')
		        }
		    }
		 
    	</script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:HiddenField ID="hd_dispositivo" runat="server" />
    <asp:HiddenField ID="hd_modello_dispositivo" runat="server" />
        <asp:HiddenField ID="hd_amministrazioneEtichetta" runat="server" />
        <asp:HiddenField ID="hd_classifica" runat="server" />
        <asp:HiddenField ID="hd_fascicolo" runat="server" />
        <asp:HiddenField ID="hd_fascicoloDesc" runat="server" />
        <asp:HiddenField ID="hd_descrizioneAmministrazione" runat="server" />
        <asp:HiddenField ID="hd_UrlIniFileDispositivo" runat="server" />
        <asp:HiddenField ID="txt_num_stampe" runat="server" />
        <asp:HiddenField ID="hd_num_stampe" runat="server" />
        <asp:HiddenField ID="hd_num_stampe_effettuate" runat="server" />
        <asp:HiddenField ID="hd_num_doc" runat="server" />
        <asp:HiddenField ID="hd_dataCreazione" runat="server" />
        <asp:HiddenField ID="hd_codiceUoCreatore" runat="server" />
        <asp:HiddenField ID="hd_ora_creazione" runat="server" />
        <asp:HiddenField ID="hd_tipo_proto" runat="server" />
        <asp:HiddenField ID="hd_coduo_proto" runat="server" />
        <asp:HiddenField ID="hd_codreg_proto" runat="server" />
        <asp:HiddenField ID="hd_descreg_proto" runat="server" />
        <asp:HiddenField ID="hd_num_proto" runat="server" />
        <asp:HiddenField ID="hd_anno_proto" runat="server" />
        <asp:HiddenField ID="hd_data_proto" runat="server" />
        <asp:HiddenField ID="hd_numero_allegati" runat="server" />
        <asp:HiddenField ID="hd_dataArrivo" runat="server" />
        <asp:HiddenField ID="hd_dataArrivoEstesa" runat="server" />
        <asp:HiddenField ID="NumeroStampaCorrente" runat="server" />
        <asp:HiddenField ID="OraCreazione" runat="server" />
        <asp:HiddenField ID="Text" runat="server" />
        <img src="../Images/Common/loading.gif" />
    </div>
    <asp:PlaceHolder ID="plcApplet" runat="server">
            <applet id='printApplet' 
                    code= 'com.nttdata.printApplet.gui.PrintApplet' 
                    codebase= '<%=Page.ResolveClientUrl("~/Libraries/")%>'
                    archive='PrintApplet.jar'
		            width= '10' height = '9'>
                <param name="java_arguments" value="-Xms64m" />
                <param name="java_arguments" value="-Xmx128m" />
            </applet>
        </asp:PlaceHolder>
    </form>
</body>
</html>
