<%@ Register TagPrefix="cc2" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register TagPrefix="uc1" TagName="CheckInOutPanel" Src="../CheckInOut/CheckInOutPanel.ascx" %>
<%@ Register Src="../FormatiDocumento/SupportedFileTypeController.ascx" TagName="SupportedFileTypeController"
    TagPrefix="uc2" %>
<%@ Register TagPrefix="uc3" TagName="AclDocumento" Src="AclDocumento.ascx" %>
<%@ Register Src="../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>

<%@ Page Language="c#" CodeBehind="tabDoc.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.documento.tabDoc" %>

<%@ Register Src="../ActivexWrappers/ClientModelProcessor.ascx" TagName="ClientModelProcessor"
    TagPrefix="uc4" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <meta http-equiv="Pragma" content="no-cache">
    <meta http-equiv="Expires" content="-1">
    <link id="linkCss" runat="server" />
    <script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
    <script type="text/javascript">
		
		    // Handler evento javascript legato al pulsante di conversione PDF
		    function img_converti_onClick()
		    {
		        window.document.body.style.cursor="wait";
		        
		        var imgConverti = document.getElementById("<%=this.img_converti.ClientID%>");
		        imgConverti.style.display = 'none';
		        
		        disableDdlStati();
		    }
		    
		    // Funzione che disabilita la ddl degli stati e nasconde l'icona di converisone pdf
		    function disableDdlStati()
		    {
		        try
		        {    
	                if(window.top.principale.iFrame_sx.document.IframeTabs.document.getElementById('ddl_statiSuccessivi') != null)
	                {
	                    window.top.principale.iFrame_sx.document.IframeTabs.document.getElementById('ddl_statiSuccessivi').disabled = true;
	                }
	                
		        }
		        catch(e){}
		    }
		    
		    function enableDdlStati()
		    {
		        try
		        {    
	                if(window.top.principale.iFrame_sx.document.IframeTabs.document.getElementById('ddl_statiSuccessivi') != null)
	                {
	                    window.top.principale.iFrame_sx.document.IframeTabs.document.getElementById('ddl_statiSuccessivi').disabled = false;
	                }
		        }
		        catch(e){}
		    }
		    
			// Visualizzazione maschera per la gestione
			// della creazione del documento tramite modello
			// inserito nella profilazione dinamica
			function ProcessDocumentModel(idDocument, documentNumber)
			{
			    var retValue = false;
			    
			    if ("<%=HasModelProcessorSelected()%>" == "True")
			    {
			        // Visualizzazione 
			        var filePath = ShowSaveDialogBox("","<%=GetModelProcessorSupportedExtensions()%>", "Blocca documento");
    			    
				    if (filePath != "")
				    {
				        ApreAttendi("Elaborazione documento in corso...");
        			    
				        // Elabotazione modelli lato client
                        if (ClientModelProcessor_ProcessModel(filePath, idDocument, "<%=this.ModelloDocumentoCorrente%>"))
				        {
                            CheckOutDocument(filePath,
                                                "",
                                                idDocument,
                                                documentNumber,
                                                false,
                                                true,
                                                false,
                                                true,
                                                false)    
                        }
                        
                        ChiudeAttendi();
				    }				
				}		
				else
				    alert("Nessun word processor impostato");
				    
				return retValue;
			}

            // Genera un documento a partire da un modello M/TEXT
			function ProcessMTEXTModel(idDocument, documentNumber)
			{
			    var retValue = false;
			    
			    // Visualizzazione 
			    var filePath = "C:\MTEXT";

				ApreAttendi("Elaborazione documento in corso...");

				// Invia una richiesta all'M/TEXT MODEL PROVIDER
                var http=CreateObject("MSXML2.XMLHTTP");
			    http.Open("POST","../models/mtext/Create.aspx?idDocument=" + idDocument + "&documentNumber=" + documentNumber + "&modelId=xx",false);
                http.send();

			    var response=http.responseText;
               
                if (response != "" && http.status == 200)
			    {

                    // Apri applet M/TEXT
		            var newUrl = response.toString().split("|")[1];
                    var fqn = "mtext://"+response.toString().split("|")[0];
                    // Memorizza fqn
                    CheckOutDocument(fqn,"",idDocument,documentNumber,
                                    false,true,false,false,false,true);  
                    ChiudeAttendi();
                    //window.showModalDialog(newUrl, '', 'status=0,toolbar=0,location=0,menubar=0');
		            var mTextPopup = window.open(newUrl, '_blank', 'status=0,toolbar=0,location=0,menubar=0;');
                    mTextPopup.focus();

			    } else {
                    alert(response.toString().split("|")[1]);
                    ChiudeAttendi();
                }
                   
                
                return retValue;
			}

			// Visualizzazione maschera per l'acquisizione dei documenti			
			function ShowAcquire()
			{
				var ret = ApriAcquisisciDocumento();
				
				if (Boolean(ret))
				    p_File.submit();
				    
				return Boolean(ret);
			}
	
			function ShowDialogFirmaDigitale(tipoFirma)
			{
				var retValue=window.showModalDialog(
						"../FirmaDigitale/DialogFirmaDigitale.aspx?TipoFirma=" + tipoFirma,
						                "",
						                "dialogWidth:500px;dialogHeight:450px;status:no;resizable:no;scroll:auto;center:yes;help:no;close:no");
				
				if (retValue)
				{
					// Firma digitale effettuata correttamente
					//p_File.submit();
					
					// Aggiornamento pagina "tabGestioneDoc"								
					RefreshTabGestioneDoc();
				}
				
				return retValue;
			}

            function ShowDialogFirmaHSM(NomeServizioFirmaHSM)
			{
				var retValue=window.showModalDialog(
						"../FirmaDigitale/DialogFirmaHSM.aspx?NomeServizioFirmaHSM=" + NomeServizioFirmaHSM,
						                "",
						                "dialogWidth:500px;dialogHeight:250px;status:no;resizable:no;scroll:auto;center:yes;help:no;close:no");

				if (retValue)
				{
					// Aggiornamento pagina "tabGestioneDoc"								
					RefreshTabGestioneDoc();
				}
				
				return retValue;
			}
            

//            function ShowDialogFirmaHSM(NomeServizioFirmaHSM, systemId, docnumber)
//			{
//				var retValue=window.showModalDialog(
//						"../FirmaDigitale/DialogFirmaHSM.aspx?NomeServizioFirmaHSM=" + NomeServizioFirmaHSM + "&systemId=" + systemId + "&docnumber=" + docnumber,
//						                "",
//						                "dialogWidth:500px;dialogHeight:250px;status:no;resizable:no;scroll:auto;center:yes;help:no;close:no");

//				if (retValue)
//				{
//					// Aggiornamento pagina "tabGestioneDoc"								
//					//RefreshTabGestioneDoc();

//                 top.principale.iFrame_sx.document.location="tabGestioneDoc.aspx?tab=versioni";
//                    
//				}
//				
//				return retValue;
//			}
			
			// Aggiornamento visualizzazione pagina "tabGestioneDoc"
			function RefreshTabGestioneDoc()
			{
				var nomeTab=top.principale.iFrame_sx.document.getElementById('hd_currentTabName').value;
				
				if (nomeTab!=null && nomeTab!='')
					top.principale.iFrame_sx.document.location="tabGestioneDoc.aspx?tab=" + nomeTab;
			}
						
			// Visualizzazione popup dettagli firma
			function ShowMaskDettagliFirma()
			{
				var height=screen.availHeight;
				var width=screen.availWidth;
				
				height=(height * 90) / 100;
				width=(width * 90) / 100;
				var HasSignedDocumentOnSession = "<%=HasSignedDocumentOnSession()%>";
				window.showModalDialog('../popup/dettagliFirmaDoc.aspx'+HasSignedDocumentOnSession,
										'',
										'dialogHeight: ' + height + 'px; dialogWidth: ' + width + 'px; resizable: yes;status:no;scroll:yes;help:no;close:no');
			}
			
			//popup scelta stampaSegnatura
			function ApriFinestra()
			{
			    var newLeft=(screen.availWidth -(screen.availWidth-5) );
			    var newTop=(screen.availHeight-545);	
				//var retValue = window.showModalDialog('../popup/printLabelPdfFrame.aspx','','dialogWidth:300px;dialogHeight:370px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:100;left:150');
				//var retValue = window.showModalDialog('../popup/printLabelPdfFrame.aspx','','dialogWidth:410px;dialogHeight:550px;status:no;resizable:no;scroll:no;center:no;help:no;close:no;dialogLeft:'+newLeft+';dialogTop:'+newTop+';');
				window.showModelessDialog('../popup/printLabelPdfFrame.aspx',window,'dialogWidth:410px;dialogHeight:550px;status:no;resizable:no;scroll:no;center:no;help:no;close:no;dialogLeft:'+newLeft+';dialogTop:'+newTop+';');
					
			}
			
			function ShowFile()
			{
			    var fileExtension = "<%=GetCurrentFileExtension()%>";
			    if (SF_ValidateFileFormat("file." + fileExtension) && SF_ValidateSize(fileExtension, <%=GetCurrentFileSize()%>))
			    {
                   window.document.body.style.cursor='wait';
		            ShowWaitingDonwloadPage();
		            return true;
			    }
			    else
			        return false;
			}

            function ApriFinestraTimestamp()
            {
                var w = window.screen.width;
			    var h = window.screen.height;
			    var top = (h-400)/2;
			    var left = (w-600)/2;

                //window.open('../popup/timestampDocumento.aspx','','top='+top+',left='+left+',width=600px,height=400px,scrollbars=NO');                
                window.showModalDialog('../popup/timestampModal.aspx?Chiamante=../popup/timestampDocumento.aspx', '', 'dialogWidth:650px;dialogHeight:400px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:' + top + ';left:' + left);
			}

            // callback per click sul pulsante di Zoom
            function OpenWindowZoom()
            {
                //chiamata server
                CallServerZoom("", "");
            }

            //invocata dopo aver ricevuto l'url dal server
            function ReceiveServerUrlData(rValue)
            {   
                var targetName = "zoom";
                ApriFinestraMassimizzata(rValue,targetName);
            }
            // end callback pulsante Zoom
    </script>
    <script type="text/javascript" id="btn_visualizza_click" event="onclick()" for="btn_visualizza">
            return ShowFile();
    </script>
    <script type="text/javascript" id="btn_visualizza_con_segn_click" event="onclick()"
        for="btn_visualizza_con_segn">
			return ShowFile();
    </script>
    <!--<script type="text/javascript" id="btn_Zoom_click" event="onclick()" for="btn_Zoom">
			return ShowFile();
    </script>-->
</head>
<body bottommargin="0" leftmargin="0" topmargin="0" rightmargin="0" ms_positioning="GridLayout">
    <form id="p_File" method="post" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="File" />
    <uc4:ClientModelProcessor ID="clientModelProcessor" runat="server" />
    <uc2:SupportedFileTypeController ID="supportedFileTypeController" runat="server" />
    <input id="hdn_doc" type="hidden" name="hdn_doc" runat="server">
    <input id="hdn_size_max" type="hidden" name="hdn_size_max" runat="server">
    <input id="hd_conferma" type="hidden" name="hd_conferma" runat="server">
    <uc3:AclDocumento ID="aclDocumento" runat="server"></uc3:AclDocumento>
    <table height="98%" cellspacing="0" cellpadding="0" width="100%" align="center" border="0">
        <tr height="5%">
            <td width="100%">
                <table class="info" cellspacing="0" cellpadding="0" width="99%" align="center">
                    <tr>
                        <!-- ###################################   RIGA LABEL ########################################## -->
                        <!-- label Acquisisci -->
                        <td class="tabDoc" align="center" width="75" style="height: 14px">
                            Acquisisci
                        </td>
                        <!-- label Visualizza -->
                        <td class="tabDoc" id="td_contiene_visual" align="center" width="140" runat="server"
                            style="height: 14px">
                            Visualizza
                        </td>
                        <!-- label Firma -->
                        <td class="tabDoc" align="center" width="116" style="height: 14px">
                            Firma
                        </td>
                        <!-- label Blocca / Rilascia -->
                        <td class="tabDoc" align="center" width="156" style="height: 14px">
                            Blocca/Rilascia
                        </td>
                        <!-- label Modello -->
                        <td class="tabDoc" id="td_label_modello" align="center" width="77" runat="server"
                            style="height: 14px">
                            Modello
                        </td>
                    </tr>
                    <tr>
                        <!-- ###################################   RIGA PULSANTI ########################################## -->
                        <td class="item_editbox2" valign="middle" align="center" width="75" bgcolor="#ffffff"
                            height="43">
                            <!-- tasti Acquisisci -->
                            <cc2:ImageButton ID="btn_acquisisci" Width="59" Height="30px" runat="server" Tipologia="DO_DOC_ACQUISISCI"
                                DisabledUrl="../images/bottoniera/all_aggiungi_nonattivo.gif" AlternateText="Acquisisci documento"
                                ImageUrl="../images/bottoniera/all_aggiungi_attivo.gif" ImageAlign="Middle">
                            </cc2:ImageButton>
                        </td>
                        <!--<td class="item_editbox2" valign="middle" width="140" height="43">-->
                        <td class="item_editbox2" valign="middle" width="174" height="43">
                            <!-- tasti Visualizza -->
                            <!--<table height="43" width="140" cellspacing="0" cellpadding="0" bgcolor="#ffffff"
                                border="0">-->
                                <table height="43" width="174" cellspacing="0" cellpadding="0" bgcolor="#ffffff"
                                border="0">
                                <tr>
                                    <td width="4">
                                    </td>
                                    <!-- 1° tasto Visualizza -->
                                    <td id="td_visualizza" valign="middle" width="29" runat="server">
                                        <cc2:ImageButton ID="btn_visualizza" runat="server" Tipologia="DO_DOC_VISUALIZZA"
                                            DisabledUrl="../images/tabDoc/visualizza_noattivo_p.gif" AlternateText="Visualizza documento"
                                            ImageUrl="../images/tabDoc/visualizza_attivo_p.gif" ImageAlign="Middle"></cc2:ImageButton>
                                    </td>
                                    <td width="4">
                                    </td>
                                    <!-- 2° tasto Visualizza -->
                                    <td valign="middle" width="29">
                                        <cc2:ImageButton ID="btn_Zoom" Width="29" Height="30" runat="server" Tipologia="DO_DOC_VISUALIZZAZOOM"
                                            DisabledUrl="../images/tabDocImages/zoom_nonattivo.gif" AlternateText="Zoom"
                                            ImageUrl="../images/tabDocImages/zoom_attivo.gif" ImageAlign="Middle" OnClientClick="OpenWindowZoom();return false;"></cc2:ImageButton>
                                    </td>
                                    <td width="4">
                                    </td>
                                    <!-- 3°/4° tasto Visualizza -->
                                    <td id="td_segnatura" valign="middle" width="72" runat="server">
                                        <table cellspacing="0" cellpadding="0" border="0">
                                            <tr>
                                                <td valign="middle">
                                                    <cc2:ImageButton ID="btn_visualizza_con_segn" Width="29" Height="30px" runat="server"
                                                        Tipologia="DO_DOC_VISUALIZZA" DisabledUrl="../images/tabDoc/visualizza_noattivo_p_segn.gif"
                                                        AlternateText="Visualizza documento con segnatura (solo PDF)" ImageUrl="../images/tabDoc/visualizza_attivo_p_segn.gif"
                                                        ImageAlign="Bottom"></cc2:ImageButton>
                                                </td>
                                                <td width="8">
                                                </td>
                                                <td valign="middle">
                                                    <cc2:ImageButton ID="btn_posiziona_segn" Width="29" Height="30px" runat="server"
                                                        DisabledUrl="../images/tabDoc/posiziona_segn_noattivo.gif" AlternateText="Posizionamento della segnatura"
                                                        ImageUrl="../images/tabDoc/posiziona_segn_attivo.gif" ImageAlign="Middle"></cc2:ImageButton>
                                                </td>
                                                
                                            </tr>
                                        </table>
                                    </td>
                                    <td width="4">
                                    </td>
                                    <td valign="middle">
                                      <cc2:ImageButton ID="btn_segn_firma_automatica" Width="29" Height="30px" runat="server"
                                          AlternateText="Segnatura Elettronica"
                                          ImageUrl="../images/tabDoc/signature_auto.png" DisabledUrl="../images/tabDoc/signature_auto_disabled.png"
                                          ImageAlign="Middle" Tipologia="DO_STAMPA_SEGN_AUTOMATICA"
                                          Visible = "false"></cc2:ImageButton>
                                    </td>
                                </tr>
                            </table>
                            <td class="item_editbox2" valign="middle" align="center" width="116" bgcolor="#ffffff"
                                height="43">
                                <!-- tasti Firma -->
                                <!--<table height="43" cellspacing="0" cellpadding="0" width="116" bgcolor="#ffffff"
                                    border="0">-->
                                    <table height="43" cellspacing="0" cellpadding="0" width="152" bgcolor="#ffffff"
                                    border="0">
                                    <tr>
                                        <td width="8">
                                        </td>
                                        <td valign="middle">
                                            <!-- 1° tasto Firma -->
                                            <cc2:ImageButton ID="btn_firma" runat="server" Width="28px" Height="30px" Tipologia="DO_DOC_FIRMA"
                                                DisabledUrl="../images/tabDocImages/firma_small_nonattivo.gif" AlternateText="Firma esterna"
                                                ImageUrl="../images/tabDocImages/firma_small_attivo.gif" ImageAlign="Middle">
                                            </cc2:ImageButton>
                                        </td>
                                        <td width="8">
                                        </td>
                                        <td valign="middle">
                                            <!-- 2° tasto Firma -->
                                            <cc2:ImageButton ID="btn_cofirma" runat="server" Width="28px" Height="30px" Tipologia="DO_DOC_FIRMA"
                                                DisabledUrl="../images/tabDocImages/cofirma_nonattivo.gif" AlternateText="Co-firma"
                                                ImageUrl="../images/tabDocImages/cofirma_attivo.gif" Enabled="False" ImageAlign="Middle">
                                            </cc2:ImageButton>
                                        </td>
                                        <td style="width: 8px">
                                        </td>
                                        <td valign="middle">
                                            <!-- 3° tasto Firma -->
                                            <img language="javascript" id="btn_DettagliFirma" onclick="return btn_DettagliFirma_onclick()"
                                                height="30" alt="Dettaglio firma" src="../images/tabDocImages/firma_dettagli_nonattivo.gif"
                                                width="28" runat="server"/>
                                        </td>
                                        <td width="8">
                                        </td>
                                        <td valign="middle">
                                            <!-- 4° tasto Firma_HSM -->
                                            <cc2:ImageButton ID="imgbtn_firmaHSM" runat="server" Width="28px" Height="30px" Tipologia="FIRMA_HSM"
                                                DisabledUrl="../images/tabDocImages/firma_small_nonattivo_hsm.gif" AlternateText="Firma HSM"
                                                ImageUrl="../images/tabDocImages/firma_small_attivo_hsm.gif" ImageAlign="Middle" Visible="false">
                                            </cc2:ImageButton>
                                        </td>
                                        <td width="8">
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        <td class="item_editbox2" valign="middle" align="center" width="200" bgcolor="#ffffff"
                            height="43">
                            <uc1:CheckInOutPanel ID="checkInOutPanel" runat="server" RelativeFolderPath="../CheckInOut/">
                            </uc1:CheckInOutPanel>
                        </td>
                        <td class="item_editbox2" valign="middle" align="center" width="77" bgcolor="#ffffff"
                            height="43">
                            <!-- tasti Modello -->
                            <cc2:ImageButton ID="btn_modello" runat="server" Width="28px" Height="30px" AlternateText="Apri modello documento"
                                ImageUrl="../images/modello_attivo.gif" DisabledUrl="../images/modello_nonattivo.gif"
                                ImageAlign="Middle"></cc2:ImageButton>
                        </td>
                    </tr>
                    <tr>
                        <!-- ###################################   RIGA DETTAGLI DOC ########################################## -->
                        <td class="tabDoc" valign="middle" colspan="5">
                            <table cellspacing="0" cellpadding="0" width="100%" border="0">
                                <tr>
                                    <td class="testo_grigio_scuro" id="tipo" valign="bottom" width="15%" runat="server">
                                        &nbsp;&nbsp;Tipo:&nbsp;<asp:ImageButton ID="img_tipo" runat="server" Visible="False"
                                            ImageUrl="../images/tabDocImages/icon_pdf.gif" AlternateText="Tipologia File"
                                            OnClick="img_tipo_Click"></asp:ImageButton>
                                    </td>
                                    <td class="testo_grigio_scuro" id="dimensione" valign="bottom" width="25%" runat="server">
                                        Dimen.:&nbsp;<asp:Label ID="lbl_size" runat="server" CssClass="testo_grigio9"></asp:Label>
                                    </td>
                                    <td class="testo_grigio_scuro" id="firmatoLabel" valign="bottom" width="20%" runat="server">
                                        <cc2:ImageButton ID="btn_timestamp" runat="server" Visible="false" AlternateText="Apri lista timestamp del documento"
                                            OnClientClick="ApriFinestraTimestamp()" Width="18px" Height="18px"></cc2:ImageButton>
                                        &nbsp;&nbsp;&nbsp;&nbsp;Firmato:&nbsp;<asp:ImageButton ID="img_firmato" runat="server"
                                             Visible="False" ImageUrl="../images/ico_visto.gif" OnClick ="img_firmato_Click" ToolTip = "Click per verificare la CRL" OnClientClick = "window.document.body.style.cursor='wait'"></asp:ImageButton>
                                    </td>
                                    <td class="testo_grigio_scuro" id="cartaceo" valign="bottom" width="18%" runat="server">
                                        Cartaceo:<asp:Image ID="img_cartaceo" runat="server" Visible="False" ImageUrl="../images/ico_visto.gif">
                                        </asp:Image>
                                    </td>
                                    <asp:Panel ID="pnl_converti" runat="server" Visible="false">
                                        <td class="testo_grigio_scuro" id="converti" valign="bottom" width="22%" runat="server">
                                            Converti Pdf:&nbsp<asp:ImageButton ID="img_converti" runat="server" ImageUrl="../images/tabDocImages/icon_pdf.gif"
                                                OnClick="img_converti_Click" OnClientClick="img_converti_onClick()" ToolTip="Richiesta di conversione PDF">
                                            </asp:ImageButton><asp:Label ID="lbl_converti" runat="server" class="testo_grigio_scuro"></asp:Label>
                                        </td>
                                    </asp:Panel>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr height="98%">
            <!-- ###################################   AREA DI VISUALIZZAZIONE DOCUMENTO   ########################################## -->
            <td>
                <cc2:IFrameWebControl ID="iFrameDoc" runat="server" BorderWidth="0px" BorderStyle="None"
                    iHeight="100%" iWidth="100%" Scrolling="no" Marginheight="0" Marginwidth="0"
                    name="iFrameDoc"></cc2:IFrameWebControl>
            </td>
        </tr>
        <tr height="98%">
            <!-- ###################################   AREA DI VISUALIZZAZIONE DOCUMENTO FIRMATO   ########################################## -->
            <td>
                <cc2:IFrameWebControl ID="iFrameSignedDoc" runat="server" BorderWidth="0px" BorderStyle="None"
                    iHeight="0px" iWidth="0px" Scrolling="auto" Marginheight="0" Marginwidth="0"
                    name="iFrameSignedDoc"></cc2:IFrameWebControl>
            </td>
        </tr>
    </table>
    <object id="MSXML3" style="display: none" codebase="../activex/msxml3.cab#version=8,00,7820,0"
        type="application/x-oleobject" height="0" width="0" data="data:application/x-oleobject;base64,EQ/Z9nOc0xGzLgDAT5kLtA=="
        classid="clsid:f5078f32-c551-11d3-89b9-0000f81fe221" viewastext>
    </object>
    <object id="CAPICOM1" codebase="../activex/capicom.dll#version=1,0,0,1" height="0"
        width="0" data="data:application/x-oleobject;base64,IGkzJfkDzxGP0ACqAGhvEzwhRE9DVFlQRSBIVE1MIFBVQkxJQyAiLS8vVzNDLy9EVEQgSFRNTCA0LjAgVHJhbnNpdGlvbmFsLy9FTiI+DQo8SFRNTD48SEVBRD4NCjxNRVRBIGh0dHAtZXF1aXY9Q29udGVudC1UeXBlIGNvbnRlbnQ9InRleHQvaHRtbDsgY2hhcnNldD13aW5kb3dzLTEyNTIiPg0KPE1FVEEgY29udGVudD0iTVNIVE1MIDYuMDAuMjgwMC4xMTI2IiBuYW1lPUdFTkVSQVRPUj48L0hFQUQ+DQo8Qk9EWT4NCjxQPiZuYnNwOzwvUD48L0JPRFk+PC9IVE1MPg0K"
        classid="clsid:A996E48C-D3DC-4244-89F7-AFA33EC60679" viewastext>
    </object>
    </form>
</body>
</html>
