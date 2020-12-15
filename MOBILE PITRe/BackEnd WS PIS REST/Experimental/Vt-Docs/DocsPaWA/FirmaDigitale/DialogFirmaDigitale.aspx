<%@ Page Language="c#" CodeBehind="DialogFirmaDigitale.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.FirmaDigitale.DialogFirmaDigitale" %>
<%@ Register Src="../SmartClient/DigitalSignWrapper.ascx" TagName="CapicomWrapper" TagPrefix="uc2" %>
<%@ Register Src="../ActivexWrappers/CapicomWrapper.ascx" TagName="CapicomWrapper" TagPrefix="uc3" %>
<%@ Register Src="../AcrobatIntegration/ClientController.ascx" TagName="ClientController" TagPrefix="uc1" %>
<%@ Register TagPrefix="cc2" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd" >
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title>
        <%=GetMaskTitle()%>
    </title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR" />
    <meta content="C#" name="CODE_LANGUAGE" />
    <meta content="JavaScript" name="vs_defaultClientScript" />
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
	<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet" />
	<script type="text/javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
    <base target="_self" />
    <script type="text/javascript">
            function ApplySign() {
                // Reperimento id documenti in modalità multiselezione
                var docs = "<%=this.GetSelectedDocumentsIds()%>";

                if (docs != null && docs != "") {
                    return this.SignDocuments(docs.split("|"));
                }
                else {
                    alert('Nessun documento selezionato per la firma');
                    return false;
                }
			}
			
			function CloseWindow() {
			    window.returnValue = true;
				window.close();
            }

            function dummy()
            { }

            // Firma digitale di più documenti
			function SignDocuments(docs) {
			    var retValue = true;

			    for (var i = 0; i < docs.length; i++) {
			        this.SignDocument('<%=this.TipoFirma%>', docs[i]);
			    }

			    return retValue;
			}
            
			function SignDocument(tipoFirma, idDocumento) {
			    var docStatus = true;
			    var docStatusDescription = "";
			    var signType = "<%=GetSignType()%>";

			    if (signType == "1") 
                {
                    signType = "C"; 
                    if (document.getElementById("chkPades"))
			            if (document.getElementById("chkPades").checked)
			                signType = "P";
			    }

			    var temp = document.getElementById("lstListaCertificati");
				var indexCert= temp.selectedIndex;
				
				if(isNaN(indexCert) || indexCert==-1) 
				{
				    docStatus = false;
				    docStatusDescription = "Nessun certificato selezionato";
				}
				else
				{	
				    var selectedValue= temp.options[indexCert].Value;
				    
				    if (idDocumento != null && idDocumento != "")
                    {
				        // Nel caso in cui è stato fornito l'id di un documento, 
                        // modifica lo stato di sessione del sistema
                        var http = new ActiveXObject("MSXML2.XMLHTTP");
                        http.Open("POST", "FirmaMultiplaChangeSessionContext.aspx?fromDoc=1&idDocumento=" + idDocumento, false);
                        http.send();

                        if (http.status != 200) 
                        {
                            // Si è verificato un errore, reperimento del messaggio
                            docStatus = false;
                            docStatusDescription = http.statusText;
                        }
				    }

				    if (docStatus) {
				        var url = "VisualizzaDocumentoFirmato.aspx?idDocumento=" + idDocumento + "&signType=" + signType + "&tipoFirma=" + tipoFirma;

				        var http = new ActiveXObject("MSXML2.XMLHTTP");
				        http.Open("POST", url, false);
				        http.send();

				        //iFrameSignedDoc.location = url;

				        var content = null;
				        var signedAsPdf = false;
				
				        var convLoc = ConvertLocally();
				        var convCentr = ConvertCentrally();

				        // Conversione del file da firmare in pdf
				        // Se il sistema è configurato per convertire il documento in pdf prima
				        // della firma...
				        if (convCentr || convLoc) {
				            // ...se l'utente vuole convertire il documento si procede con la conversione
				            // prefirma
				            var fileFormat = "<%=GetFileExtension()%>";

				            if (fileFormat.indexOf(".p7m") == -1 && fileFormat.indexOf(".pdf") == -1) {
				                // Se è richiesta conversione pdf prefirma...
				                if (document.getElementById("chkConverti").checked) {
				                    // ...se è richiesta la conversione centrale...
				                    if (convCentr) {
				                        // ...si procede con la  conversione sincrona
				                        var sincrona = new ActiveXObject("MSXML2.XMLHTTP");
				                        sincrona.Open("POST", "ConvPDFSincrona.aspx", false);
				                        sincrona.send();

				                        if (sincrona.status == 200)
				                            content = sincrona.responseBody;
				                    }
				                    else {
				                        // ...altrimenti si procede con la conversione locale
				                        content = ConvertPdfStream(http.responseBody, fileFormat, false);
				                    }

				                    signedAsPdf = (content != null);

				                    if (!signedAsPdf) {
				                        docStatusDescription = "Non è stato possibile convertire il file in formato PDF.\n" +
					                                            "Il file verrà firmato nel suo formato originale.";
				                    }
				                }
				            }

				            if (content == null)
				                content = http.responseBody;
				        }
				        else {
				            content = http.responseBody;
				        }

				        // Applicazione della firma digitale al documento
				        var signedValue = null;


				        if (signType == "0") {
				            signedValue = CapicomWrapper_SignData(content, selectedValue, (tipoFirma == "cosign"), 2, "My");
				        }
				        else {
				            signedValue = CapicomWrapper_SignHash(content, selectedValue, (tipoFirma == "cosign"), 2, "My");
				        }


                        if (signedValue == null)
                        {
                            docStatus = false;
                            docStatusDescription = "Errore nella firma digitale del documento";
                        }
                        else
                        {
                            http.Open("POST", "../documento/docSalva.aspx" + "?idDocumento=" + idDocumento + "&tipofirma=" + tipoFirma + "&signedAsPdf=" + signedAsPdf + "&signType=" + signType, false);
				            http.send(signedValue);

				            if (http.status != 0 && http.status != 200) {
				                docStatus = false;
				                docStatusDescription = "Errore durante l\'invio del documento firmato.\n" + http.statusText + "\n" + http.responseText;
				            }
				        }
				    }
				}

				// Invio informazioni sullo stato della firma
				var http = new ActiveXObject("MSXML2.XMLHTTP");
				http.Open("POST", "FirmaDigitaleResultStatusPage.aspx?status=" + docStatus + "&statusDescription=" + docStatusDescription + "&idDocument=" + idDocumento, false);
				http.send();

				return (docStatus == 0);
			}
			
			// Funzione per l'inizializzazione dell'interfaccia grafica
			function Initialize() {
			    // Recupero della configurazione relativa alla conversione pdf
			    // locale e centrale e alla sua abilitazione
                var isEnabledConv = ("<%=ConvertPdfOnSign%>" == "True");
			    var convPdfLocale = (isEnabledConv && IsIntegrationActiveAndInstalled());
			    var convPdfCentrale = (isEnabledConv && "<%=IsEnabledConvPDFSincrona%>" == "True");
			    
			    // Recupero dei riferimenti alla checkbox "Converti in pdf", ai due option button "Locale"
			    // e "Centrale" e al pannello che contiene i due option button
			    var chkConverti = document.getElementById("chkConverti");
			    var optLocale = document.getElementById("optLocale");
			    var optCentrale = document.getElementById("optCentrale");
			    var pnlConversione = document.getElementById("pnlConversione");
			    
			    // Se il sistema non è configurato per convertire in pdf prima della
			    // firma o se lo è ma nessuna delle due conversioni è operativa,
			    // bisogna disabilitare la checkbox "Converti in PDF", togliere
			    // il flag e nascondere le due opzioni di conversione
			    if(!((isEnabledConv && convPdfLocale) || (isEnabledConv && convPdfCentrale)))
			    {
			        chkConverti.checked = false;
			        chkConverti.disabled = true;
			        pnlConversione.style.visibility = 'hidden';
			    }
			    			    
			    // Fetch della lista dei certificati
			    FetchListaCertificati();
			    
			    // Simula il click sul check box "Converti in PDF"
			    OnClickConvertPDF();
			}

			function FetchListaCertificati() {
			    var list = CapicomWrapper_GetCertificateList(2, "MY");

			    var e = new Enumerator(list);
			    var i = 1;
			    for (; !e.atEnd(); e.moveNext()) {
			        var cert = e.item();

			        var option = document.createElement("OPTION");
			        frmDialogFirmaDigitale.lstListaCertificati.options.add(option);
			        props = cert.SubjectName.split(",");

			        for (j = 0; j < props.length; j++) {
			            if (props[j].substr(0, 1) == " ")
			                props[j] = props[j].substr(1);
			            if (props[j].substr(0, 3) == "CN=")
			                option.innerText = props[j].substr(3);
			        }

			        option.Value = i;
			        i++;
			    }
			}

//			function FetchListaCertificati()
//			{
//			    
//                var list = CapicomWrapper_GetCertificateList(2, "MY");
//			    var e = new Enumerator(list);
//                var i=0;
//                for (;!e.atEnd();e.moveNext())	
//                {
//                    var cert = e.item();

//                    var option = document.createElement("OPTION");
//                    var temp = document.getElementById("lstListaCertificati");
//                    temp.options.add(option);
//                    option.innerText = cert;
//                    option.Value = i;
//		            i++;	
//                }
//			}
			
			// Funzione per la gestione del click sulla check box "Converti in PDF"
			function OnClickConvertPDF()
			{
			    // Recupero della configurazione relativa alla conversione pdf
			    // locale e centrale e alla sua abilitazione
                var isEnabledConv = ("<%=ConvertPdfOnSign%>" == "True");
			    var convPdfLocale = (isEnabledConv && IsIntegrationActiveAndInstalled());
			    var convPdfCentrale = (isEnabledConv && "<%=IsEnabledConvPDFSincrona%>" == "True");
			    
			    // Recupero dei riferimenti alla checkbox "Converti in pdf", ai due option button "Locale"
			    // e "Centrale" e al pannello che contiene i due option button
			    var optLocale = document.getElementById("optLocale");
			    var optCentrale = document.getElementById("optCentrale");
			    var pnlConversione = document.getElementById("pnlConversione");
			    
			    
			    // Recupero dello stato di checking della chkConverti
			    var chkConverti = document.getElementById("chkConverti").checked;
			    
			    // Se chkConverti è flaggato bisogna abilitare il pannello altrimenti bisogna
			    // disabilitarlo
			    if(chkConverti)
			        pnlConversione.disabled = false;
			    else
			        pnlConversione.disabled = true; 
			    
			    optLocale.setAttribute("disabled", !(convPdfLocale && chkConverti));
		        optCentrale.setAttribute("disabled", !(convPdfCentrale && chkConverti));
		        
		        // Se è attiva la conversione locale la si preferisce altrimenti si seleziona
		        // la conversione locale
		        if(convPdfLocale)
		            optLocale.checked = true;
		        else
		            optCentrale.checked = true;
			    
			}
			
			// Analizza l'interfaccia al fine di determinare se bisogna convertire
			// il documento localmente
			function ConvertLocally()
			{
			    // Recupero del riferimento all'option button "Locale" e valutazione
			    // dello stato di flagging dell'opzione "Converti in PDF"
			    var optLocale = document.getElementById("optLocale");
			    var convertInPDF = document.getElementById("chkConverti").checked;
			    
			    // Si restituisce l'and fra le due variabili
			    return optLocale.checked && convertInPDF;
			}
			
			// Analizza l'interfaccia al fine di determinare se bisogna convertire
			// il documento centralmente
			function ConvertCentrally()
			{
				// Recupero del riferimento all'option button "Centrale" e valutazione
			    // dello stato di flagging dell'opzione "Converti in PDF"
			    var optCentrale = document.getElementById("optCentrale");
			    var convertInPDF = document.getElementById("chkConverti").checked;
			    
			    // Si restituisce l'and fra le due variabili
			    return optCentrale.checked && convertInPDF;
			
			}
			
    </script>
</head>
<body onload="Initialize();">
    <form id="frmDialogFirmaDigitale" method="post" runat="server">
    <uc1:ClientController ID="AcrobatClientController" runat="server" />
    
    <table class="info_grigio" width="500px" cellpadding="0" align="center">
        <tr>
            <td class="titolo_scheda" align="center">
                <asp:Label ID="lblListaCertificati" runat="server">Lista certificati</asp:Label>
            </td>
        </tr>
        <tr>
            <td class="titolo_scheda">
                <select language="javascript" class="testo_grigio" id="lstListaCertificati" 
                    size="9" name="selectCert" runat="server" style="width: 100%">
                </select>
            </td>
        </tr>
        <tr>
            <td class="titolo_scheda">
                <asp:CheckBox ID="chkConverti" runat="server" Text="Converti in pdf" Checked="false" />&nbsp;&nbsp;
                <asp:CheckBox ID="chkPades" runat="server" Text="Firma Pades" Checked="false" />
                <asp:Panel ID="pnlConversione" runat="server" >
                    <asp:RadioButton ID="optLocale" GroupName="grpLocCentr" CssClass="testo_grigio" runat="server"
                        Text="Locale" Checked="true" />&nbsp;&nbsp;
                    <asp:RadioButton ID="optCentrale" GroupName="grpLocCentr" CssClass="testo_grigio"
                        runat="server" Text="Centrale" />
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td></td>
        </tr>
        <tr>
            <td class="titolo_scheda" align="center">
                <asp:Button ID="btnApplicaFirma" runat="server" Text="Applica firma" CssClass="pulsante69" OnClientClick="ApplySign();" OnClick="btnApplicaFirma_OnClick"></asp:Button>
                        &nbsp;
                <asp:Button ID="btnAnnulla" runat="server" Text="Annulla" OnClientClick="CloseWindow()" CssClass="pulsante69"></asp:Button>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Label ID="lblDocumentCount" runat="server" CssClass="testo_grigio"></asp:Label>
                <br />
                <asp:Panel ID="pnlResultContainer" runat="server" ScrollBars="Auto" Height="200px">
                    <asp:DataGrid id="grdResult" SkinID="datagrid" runat="server" Width="100%" BorderWidth="1px" ShowHeader="true" CellPadding="1" BorderColor="Gray" AutoGenerateColumns="False">
		                <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
		                <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
		                <ItemStyle CssClass="bg_grigioN"></ItemStyle>
                        <HeaderStyle HorizontalAlign="Center" CssClass="testo_grigio_scuro"></HeaderStyle>								  
			            <Columns>
                            <asp:TemplateColumn HeaderText="Id Documento">
                                <HeaderStyle HorizontalAlign="Center" Width="30%" />
                                <ItemTemplate>
                                    <asp:Label ID="lblIdDocumento" runat="server" Text='<%#((DocsPAWA.FirmaDigitale.FirmaDigitaleResultStatus) Container.DataItem).IdDocument%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Esito">
                                <HeaderStyle HorizontalAlign="Center" Width="70%"/>
                                <ItemTemplate>
                                    <asp:Label ID="lblEsito" runat="server" Font-Bold="true" Text='<%#(((DocsPAWA.FirmaDigitale.FirmaDigitaleResultStatus) Container.DataItem).Status ? "OK" : "KO")%>'></asp:Label>
                                    <br />
                                    <asp:Label ID="lblDescrizioneEsito" runat="server" Text='<%#((DocsPAWA.FirmaDigitale.FirmaDigitaleResultStatus) Container.DataItem).StatusDescription%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                    </asp:DataGrid>
                </asp:Panel>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
