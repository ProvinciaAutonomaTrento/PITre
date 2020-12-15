<%@ Register TagPrefix="cc2" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register TagPrefix="uc1" TagName="CheckInOutPanel" Src="../CheckInOut/CheckInOutPanel.ascx" %>
<%@ Register Src="../FormatiDocumento/SupportedFileTypeController.ascx" TagName="SupportedFileTypeController"
    TagPrefix="uc2" %>

<%@ Register Src="../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>

<%@ Page Language="c#" CodeBehind="tabPulsantiDoc.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.fascicolo.tabPulsantiDoc" %>

<%@ Register Src="../ActivexWrappers/ClientModelProcessor.ascx" TagName="ClientModelProcessor"
    TagPrefix="uc4" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head id="Head1" runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <meta http-equiv="Pragma" content="no-cache">
    <meta http-equiv="Expires" content="-1">
    <%--<link id="linkCss" runat="server" />--%>

    <LINK href="../CSS/ProfilazioneDinamica.css" type="text/css" rel="stylesheet">
	<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
	<script language="JavaScript" src="../CSS/ETcalendar.js"></script>

    <script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>

    <script type="text/javascript">

        function OpenPopUpExportFasc(idFasc) {
            var newLeft = (screen.availWidth - 850);
            var newTop = (screen.availHeight - 600);
            var myUrl = "../EsportaFascicolo/esportaFasc.aspx?idFasc=" + idFasc;
            var retValue = window.showModalDialog(myUrl, '', 'dialogWidth:700px;dialogHeight:380px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:' + newTop + ';left:' + newLeft);
            if (retValue = 'chiuso') window.document.p_File.submit();
        }
       /* function ApriPopUpImportDoc(cod, idTit) {
            var newLeft = (screen.availWidth - 850);
            var newTop = (screen.availHeight - 600);
            //var cod = document.getElementById("txt_fascdesc").value;
            var myUrl = "../ImportMassivoDoc/importaDoc.aspx?codFasc=" + cod + "&idTitolario=" + idTit;
            var retValue = window.showModalDialog(myUrl, '', 'dialogWidth:700px;dialogHeight:380px;status:no;resizable:yes;scroll:no;center:yes;help:no;close:no;top:' + newTop + ';left:' + newLeft);
            if (retValue = 'chiuso') window.document.p_File.submit();

        }*/
        function ApriPopUpImportDoc(idTit) {
            var newLeft = (screen.availWidth - 850);
            var newTop = (screen.availHeight - 600);
            var myUrl = "../ImportMassivoDoc/importaDoc.aspx?idTitolario=" + idTit;
            var retValue = window.showModalDialog(myUrl, '', 'dialogWidth:700px;dialogHeight:380px;status:no;resizable:yes;scroll:no;center:yes;help:no;close:no;top:' + newTop + ';left:' + newLeft);
            if (retValue = 'chiuso') window.document.p_File.submit();
        }

        function ApriFinestraRicercaDocPerClassifica(parameters) {
            var newLeft = (screen.availWidth - 602);
            var newTop = (screen.availHeight - 689);
            var myUrl = "../popup/RicercaDocumentiPerClassifica.aspx?" + parameters;
            rtnValue = window.showModalDialog(myUrl, '', 'dialogWidth:595px;dialogHeight:643px;status:no;dialogLeft:' + newLeft + '; dialogTop:' + newTop + ';center:no;resizable:yes;scroll:no;help:no;');
            window.document.p_File.submit();
        }
		    // Handler evento javascript legato al pulsante di conversione PDF
		    function img_converti_onClick()
		    {
//		       
		    }

		    function ShowDialogSearchDocuments() {
		        var retValue = window.showModalDialog('../ricercaDoc/FiltriRicercaDocumenti.aspx?prov=fasc',
												'',
											'dialogWidth:650px;dialogHeight:280px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no');

		        p_File.txtFilterDocumentsRetValue.value = retValue;

		        if (retValue)
		            ShowWaitCursor()
		    }

		    function confirmDel() {

		        var agree = confirm("Il documento verrà rimosso dal fascicolo. Continuare?");
		        if (agree) {
		            document.getElementById("txt_confirmDel").value = "si";
		            return true;
		        }

		    }
		    function ShowWaitCursor() {
		        window.document.body.style.cursor = "wait";
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
//			   
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
						"dialogWidth:430px;dialogHeight:250px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no");
				
				if (retValue)
				{
					// Firma digitale effettuata correttamente
					//p_File.submit();
					
					// Aggiornamento pagina "tabGestioneDoc"								
					RefreshTabGestioneDoc();
				}
				
				return retValue;
			}
			
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
				
				window.showModalDialog('../popup/dettagliFirmaDoc.aspx',
										'',
										'dialogHeight: ' + height + 'px; dialogWidth: ' + width + 'px; resizable: yes;status:no;scroll:yes;help:no;close:no');
			}
			
			//popup scelta stampaSegnatura
			function ApriFinestra()
			{
			    var newLeft=(screen.availWidth -(screen.availWidth-5) );
			    var newTop=(screen.availHeight-545);	
				//var retValue = window.showModalDialog('../popup/printLabelPdfFrame.aspx','','dialogWidth:300px;dialogHeight:370px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:100;left:150');
				var retValue = window.showModalDialog('../popup/printLabelPdfFrame.aspx','','dialogWidth:410px;dialogHeight:510px;status:no;resizable:no;scroll:no;center:no;help:no;close:no;dialogLeft:'+newLeft+';dialogTop:'+newTop+';');
				p_File.hd_conferma.value = retValue;
					
			}
			
			function ShowFile()
			{
			   
			}
			
    </script>

    <script type="text/javascript" id="btn_visualizza_click" event="onclick()" for="btn_visualizza">
            return ShowFile();
    </script>

    <script type="text/javascript" id="btn_visualizza_con_segn_click" event="onclick()"
        for="btn_visualizza_con_segn">
			return ShowFile();
    </script>

    <script type="text/javascript" id="btn_Zoom_click" event="onclick()" for="btn_Zoom">
			return ShowFile();
    </script>

</head>
<body bottommargin="0" leftmargin="0" topmargin="0" rightmargin="0" ms_positioning="GridLayout">
    <form id="p_File" method="post" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="File" />
    <uc4:ClientModelProcessor ID="clientModelProcessor" runat="server" />
    <uc2:SupportedFileTypeController ID="supportedFileTypeController" runat="server" />
    <input id="hdn_doc" type="hidden" name="hdn_doc" runat="server"/>
    <input id="hdn_size_max" type="hidden" name="hdn_size_max" runat="server"/>
    <input id="hd_conferma" type="hidden" name="hd_conferma" runat="server"/>
    <input id="txtFilterDocumentsRetValue" type="hidden" runat="server"/>
	<input id="txt_confirmDel" type="hidden" name="txt_confirmDel" runat="server" />
    <table height="98%" cellspacing="0" cellpadding="0" width="100%" align="center" border="0">
        <tr height="5%">
            <td width="100%">
                <table class="info" cellspacing="0" cellpadding="0" width="100%" align="center">
                  
                      <tr>
                        <!-- ###################################   RIGA LABEL ########################################## -->
                        <!-- label Acquisisci -->
                        <td class="tabDoc" align="center" width="75" style="height: 14px">
                            Inserisci
                        </td>
                        <!-- label Visualizza -->
                        <td class="tabDoc" id="td1" align="center" width="140" runat="server"
                            style="height: 14px">
                            Visualizza
                        </td>
                      
                        <!-- label Blocca / Rilascia -->
                        <td class="tabDoc" align="center" width="156" style="height: 14px">
                            Importa/Esporta
                        </td>
                        <!-- label Modello -->
                        <td class="tabDoc" id="td_label_modello" align="center" width="77" runat="server"
                            style="height: 14px">
                            Filtri
                        </td>
                    </tr>
                    
                    <tr>
                        <!-- ###################################   RIGA PULSANTI ########################################## -->
                   <td class="item_editbox2" valign="middle" align="center" width="60" bgcolor="#ffffff" height="43">
                            <!-- tasto Acquisisci -->
                            <table height="43" width="100%" cellspacing="0" cellpadding="0" bgcolor="#ffffff" border="0">
                               <tr>
                                 <td valign="middle">
                                     &nbsp;</td>
                                 <td valign="middle">
                                     &nbsp;<cc2:imagebutton ID="btn_inserisciDoc" Width="33px" Height="30px" runat="server" Tipologia="DO_DOC_ACQUISISCI"
                                    DisabledUrl="../images/bottoniera/documenti_in_folder_disattivo.gif" AlternateText="Inserisci documento"
                                    ImageUrl="../images/bottoniera/documenti_in_folder_attivo.gif" 
                                    ImageAlign="Bottom" onclick="btn_inserisciDoc_Click" >
                                </cc2:imagebutton>
                                 </td>
                                 <td>
                                     &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp; </td>
                            </tr>
                           </table>
                   </td>
                   <td>
                     <!-- tasto Visualizza -->
                          <table height="43" width="100%" cellspacing="0" cellpadding="0" bgcolor="#ffffff" border="0">
                               <tr>
                                   <td>
                                     &nbsp;&nbsp; </td>
                                                <td valign="middle" align="left" width="100%" >
                                                    <cc2:imagebutton ID="btn_visualizzaDoc" Width="29" Height="30px" runat="server"
                                                        Tipologia="DO_DOC_VISUALIZZA" DisabledUrl="../images/bottoniera/ggg_non_attivo.gif"
                                                        AlternateText="Visualizza elenco documenti" ImageUrl="../images/bottoniera/ggg_attivo.gif"
                                                        ImageAlign="Bottom" onclick="btn_visualizzaDoc_Click"></cc2:imagebutton>
                                                </td>
                                 <td >
                                     &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </td>
                            </tr>
                          </table>
                    </td>
                           

                    <td class="item_editbox2" valign="middle" width="140" height="43" align="center">
                            <!-- tasti Importa-esporta -->
                            <table height="43" width="140" cellspacing="0" cellpadding="0" bgcolor="#ffffff"
                                border="0" align="center">
                               <tr>

                                    
                                    <td id="td_segnatura" valign="middle" width="35"  align="center">
                                        <table cellspacing="0" cellpadding="0" border="0">
                                            <tr>
                                            <td>
                                            &nbsp;&nbsp;
                                            </td>
                                               <td valign="middle" align="left">

                                    <cc2:imagebutton ID="btn_importaDoc" Width="33px" Height="30px" runat="server" Tipologia="IMP_DOC_MASSIVA"
                                    DisabledUrl="../images/bottoniera/doc_imp_disattiva.gif" AlternateText="Importa documenti"
                                    ImageUrl="../images/bottoniera/doc_imp_attiva.gif" onclick="btn_importaDoc_Click" 
                                    ImageAlign="Middle">
                                    </cc2:imagebutton>

                                    </td>
                                    <td>
                                            &nbsp;&nbsp;
                                            </td>
                                 <td valign="middle" align="left">
                                     <cc2:imagebutton ID="btn_esportaDoc" Width="33px" Height="30px" runat="server" Tipologia="EXP_DOC_MASSIVA"
                                    DisabledUrl="../images/bottoniera/doc_exp_disattiva.gif" AlternateText="Esporta documenti"
                                    ImageUrl="../images/bottoniera/doc_exp_attiva.gif" onclick="btn_esportaDoc_Click"
                                    ImageAlign="Middle">
                                </cc2:imagebutton>
                                </td>
                                       </tr>
                                        </table>
                                    </td>
                                  </tr>
                           </table>
                    </td>
                    <td class="item_editbox2" valign="middle" width="100%" height="43" align="center">

                      <!-- tasti Filtro -->
                                    <table  height="43" width="100%" cellspacing="0" cellpadding="0" bgcolor="#ffffff"
                                border="0" align="center">
                                        <tr>
                                          <td id="td_segnatura2" valign="middle" width="35"  align="center">
                                            <table cellspacing="0" cellpadding="0" border="0">
                                                <tr>
                                                  <td>
                                            &nbsp;&nbsp;
                                            </td>
                                                    <td valign="middle" align="left"  >
                                                              <cc2:imagebutton ID="btnFilterDocs" runat="server" Width="28px" Height="30px" AlternateText="Filtra documenti"
                                    autopostback="false" Tipologia=""  ImageUrl="../images/bottoniera/filtro_attivo.gif" DisabledUrl="../images/bottoniera/filtro_disattivo.gif"
                                    ImageAlign="Middle" onclick="btnFilterDocs_Click"></cc2:imagebutton>
                                                    </td>
                                                    <td>
                                            &nbsp;&nbsp;
                                            </td>
                                          
                                                    <td valign="middle" align="left">
                                                                 <cc2:imagebutton ID="btnShowAllDocs" runat="server" Width="28px" Height="30px" AlternateText="Rimuovi filtro sui documenti"
                                                          autopostback="false" Tipologia=""  ImageUrl="../images/bottoniera/filtri_attivo.gif" DisabledUrl="../images/bottoniera/filtri_disattivo.gif"
                                                            ImageAlign="Middle" onclick="btnShowAllDocs_Click" Enabled="false"></cc2:imagebutton>
                                                    </td>
                                                </tr>
                                            </table>
                                           </td>
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
                    iHeight="100%" iWidth="100%" Scrolling="auto" Marginheight="0" Marginwidth="0"
                    name="iFrameDoc"></cc2:IFrameWebControl>
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
