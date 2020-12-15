<%@ Page Language="c#" CodeBehind="FirmaDigitaleMassiva.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.MassiveOperation.FirmaDigitaleMassiva" MasterPageFile="~/MassiveOperation/MassiveMasterPage.Master" %>
<%@ Register Src="../SmartClient/DigitalSignWrapper.ascx" TagName="CapicomWrapper" TagPrefix="uc3" %>
<%@ Register Src="../ActivexWrappers/CapicomWrapper.ascx" TagName="CapicomWrapper" TagPrefix="uc2" %>
<%@ Register Src="../AcrobatIntegration/ClientController.ascx" TagName="ClientController" TagPrefix="uc1" %>
<%@ Register TagPrefix="cc2" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>

<asp:Content ID="Head" ContentPlaceHolderID="Head" runat="server">
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

			    var indexCert = document.getElementById('<%=lstListaCertificati.ClientID %>').selectedIndex;
				
				if(isNaN(indexCert) || indexCert==-1) 
				{
				    docStatus = false;
				    docStatusDescription = "Nessun certificato selezionato";
				}
				else
				{
				    var selectedValue = document.getElementById('<%=lstListaCertificati.ClientID %>').options[indexCert].Value;
				    
				    if (idDocumento != null && idDocumento != "")
                    {
				        // Nel caso in cui è stato fornito l'id di un documento, 
                        // modifica lo stato di sessione del sistema
                        var http = new ActiveXObject("MSXML2.XMLHTTP");
                        http.Open("POST", "../FirmaDigitale/FirmaMultiplaChangeSessionContext.aspx?idDocumento=" + idDocumento, false);
                        http.send();
                        //alert('status = ' + http.status);

                        if (http.status != 200 || 
                            (http.statusText != null && http.statusText != "OK"))
                        {
                            // Si è verificato un errore, reperimento del messaggio
                            docStatus = false;
                            docStatusDescription = http.statusText;
                        }
				    }

				    if (docStatus) {
				        var url = "../FirmaDigitale/VisualizzaDocumentoFirmato.aspx?idDocumento=" + idDocumento;

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
				                        sincrona.Open("POST", "../FirmaDigitale/ConvPDFSincrona.aspx", false);
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
				        var signedValue = CapicomWrapper_SignData(content, selectedValue, (tipoFirma == "cosign"), 2, "My");

                        if (signedValue == null)
                        {
                            docStatus = false;
                            docStatusDescription = "Errore nella firma digitale del documento";
                        }
                        else
                        {
				            http.Open("POST", "../documento/docSalva.aspx" + "?idDocumento=" + idDocumento + "&tipofirma=" + tipoFirma + "&signedAsPdf=" + signedAsPdf, false);
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
				http.Open("POST", "../FirmaDigitale/FirmaDigitaleResultStatusPage.aspx?status=" + docStatus + "&statusDescription=" + docStatusDescription + "&idDocument=" + idDocumento, false);
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
			    var chkConverti = document.getElementById("<%=chkConverti.ClientID%>");
			    var optLocale = document.getElementById("<%=optLocale.ClientID%>");
			    var optCentrale = document.getElementById("<%=optCentrale.ClientID%>");
			    var pnlConversione = document.getElementById("<%=pnlConversione.ClientID%>");
			    
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
			
			function FetchListaCertificati()
			{
			    var list = CapicomWrapper_GetCertificateList(2, "MY");
                
                var e = new Enumerator(list);
                var i=1;
                for (;!e.atEnd();e.moveNext())	
                {
                    var cert = e.item();

                    var option = document.createElement("OPTION");
                    document.getElementById("<%=lstListaCertificati.ClientID %>").options.add(option);
		            props = cert.SubjectName.split(",");

		            for(j=0;j<props.length;j++) 
		            {
			            if(props[j].substr(0,1)==" ")
				            props[j]=props[j].substr(1);
			            if(props[j].substr(0,3)=="CN=")
				            option.innerText = props[j].substr(3);
		            }

		            option.Value = i;	
		            i++;	
                }
			}
			
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
			    var optLocale = document.getElementById("<%=optLocale.ClientID %>");
			    var optCentrale = document.getElementById("<%=optCentrale.ClientID %>");
			    var pnlConversione = document.getElementById("<%=pnlConversione.ClientID %>");
			    
			    
			    // Recupero dello stato di checking della chkConverti
			    var chkConverti = document.getElementById("<%=chkConverti.ClientID %>").checked;
			    
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
			    var optLocale = document.getElementById("<%=optLocale.ClientID%>");
			    var convertInPDF = document.getElementById("<%=chkConverti.ClientID%>").checked;
			    
			    // Si restituisce l'and fra le due variabili
			    return optLocale.checked && convertInPDF;
			}
			
			// Analizza l'interfaccia al fine di determinare se bisogna convertire
			// il documento centralmente
			function ConvertCentrally()
			{
				// Recupero del riferimento all'option button "Centrale" e valutazione
			    // dello stato di flagging dell'opzione "Converti in PDF"
			    var optCentrale = document.getElementById("<%=optCentrale.ClientID%>");
			    var convertInPDF = document.getElementById("<%=chkConverti.ClientID%>").checked;
			    
			    // Si restituisce l'and fra le due variabili
			    return optCentrale.checked && convertInPDF;
			
			}
			
    </script>
</asp:Content>

<asp:Content ID="Form" ContentPlaceHolderID="Form" runat="server">
    <uc1:ClientController ID="AcrobatClientController" runat="server" />
    <uc2:CapicomWrapper ID="capicomWrapper" runat="server" />
    <table class="info_grigio" style="height: 100%; width:100%;" cellpadding="0" align="center" border="0">
        <tr>
            <td class="titolo_scheda" align="center">
                <asp:Label ID="lblListaCertificati" runat="server">Lista certificati</asp:Label>
            </td>
        </tr>
        <tr>
            <td class="titolo_scheda">
                <select language="javascript" class="testo_grigio" id="lstListaCertificati" style="width: 100%"
                    size="9" name="selectCert" runat="server">
                </select>
            </td>
        </tr>
        <tr>
            <td class="titolo_scheda">
                <asp:CheckBox ID="chkConverti" runat="server" Text="Converti in pdf" Checked="false" />
                <asp:Panel ID="pnlConversione" runat="server" >
                    <asp:RadioButton ID="optLocale" GroupName="grpLocCentr" CssClass="testo_grigio" runat="server"
                        Text="Locale" Checked="true" />&nbsp;&nbsp;
                    <asp:RadioButton ID="optCentrale" GroupName="grpLocCentr" CssClass="testo_grigio"
                        runat="server" Text="Centrale" />
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td>
              <asp:Label ID="lblDocumentCount" runat="server" CssClass="testo_grigio"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
