<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register TagPrefix="cc2" Namespace="Utilities" Assembly="MessageBox" %>
<%@ Register TagPrefix="uc1" TagName="AclDocumento" Src="AclDocumento.ascx" %>
<%@ Page language="c#" Codebehind="docProtocollo.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.documento.docProtocollo"	%>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc3" %>
<%@ Register src="Oggetto.ascx" tagname="Oggetto" tagprefix="uc4" %>
<%@ Register Src="../UserControls/Calendar.ascx" TagName="Calendario" TagPrefix="uc3" %>
<%@ Register src="../Note/DettaglioNota.ascx" tagname="DettaglioNota" tagprefix="uc2" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register Src="../ActivexWrappers/FsoWrapper.ascx" TagName="FsoWrapper" TagPrefix="uc3" %>
<%@ Register Src="../ActivexWrappers/AdoStreamWrapper.ascx" TagName="AdoStreamWrapper" TagPrefix="uc2" %>
<%@ Register Src="../ActivexWrappers/ShellWrapper.ascx" TagName="ShellWrapper" TagPrefix="uc1" %>
<%@ Register src="../ActivexWrappers/ClientModelProcessor.ascx" tagname="ClientModelProcessor" tagprefix="uc5" %>
<%@ Register Src="../UserControls/DocumentConsolidation.ascx" TagName="DocumentConsolidation" TagPrefix="uc6" %>
<%@ Register TagName="RicercaVeloce" TagPrefix="ut8" Src="~/UserControls/RubricaVeloce.ascx" %>
<%@ Register Assembly="AjaxControlToolkit, Version=3.0.30930.28736, Culture=neutral, PublicKeyToken=28f01b0e84b6d53e"
    Namespace="AjaxControlToolkit" TagPrefix="cc8" %>
<!DOCTYPE HTML	PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<HEAD runat="server">	
	    <title></title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR" />
		<meta content="C#" name="CODE_LANGUAGE" />
		<meta content="JavaScript" name="vs_defaultClientScript" />
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<script language="javascript" src="../LIBRERIE/rubrica.js"></script>	        
		<link href="../CSS/docspa_30.css" type="text/css" rel="stylesheet" />

		<script language="javascript" id="btn_protocolla_P_click" event="onclick()" for="btn_protocolla_P">
			   if(document.getElementById("abilitaModaleVis").value == "true" && document.getElementById("fieldsOK").value == "true")
			    {
			        AvvisoVisibilita();
			    }
			    document.getElementById('btn_protocolla_P').style.display='none';
			    document.getElementById('btn_protocollaDisabled').style.display='';
			    window.document.body.style.cursor='wait';	
			    if(top.principale.iFrame_dx.document.iFrameDoc.document.frames[0] != null && top.principale.iFrame_dx.document.iFrameDoc.document.frames[0].document.forms.length != 0)
			    {
			        top.principale.iFrame_dx.document.iFrameDoc.document.frames[0].document.forms[0].submit();
			    }	
           
            			
		</script>
		<script type="text/javascript" language="javascript" id="btn_invio_mail_click" event="onclick()" for="btn_invio_mail">
		   window.document.body.style.cursor='wait';
		</script>
		<script type="text/javascript" language="javascript" id="btn_protocollaGiallo_P_click" event="onclick()" for="btn_protocollaGiallo_P">
			//document.getElementById("abilitaModaleVis").value = "1";
			//var check_ogg = '<%=(string.IsNullOrEmpty(this.ctrl_oggetto.oggetto_text)) || (this.ctrl_oggetto.oggetto_text.Length > 2000)%>';
			//alert(check_ogg);
			if(document.getElementById("abilitaModaleVis").value == "true" && document.getElementById("fieldsOK").value == "true")
			{
			    AvvisoVisibilita();
			}
			document.getElementById('btn_protocollaGiallo_P').style.display='none';
			document.getElementById('btn_protocollaDisabled').style.display='';
			window.document.body.style.cursor='wait';
			if(top.principale.iFrame_dx.document.iFrameDoc.document.frames[0] != null && top.principale.iFrame_dx.document.iFrameDoc.document.frames[0].document.forms.length != 0)
			{
			    top.principale.iFrame_dx.document.iFrameDoc.document.frames[0].document.forms[0].submit();
			}	
		</script>
		<script type="text/javascript" language="javascript" id="btn_salva_P_click" event="onclick()" for="btn_salva_P">
			window.document.body.style.cursor='wait';
			if(top.principale.iFrame_dx.document.iFrameDoc.document.frames[0] != null && top.principale.iFrame_dx.document.iFrameDoc.document.frames[0].document.forms.length != 0)
			{
			    top.principale.iFrame_dx.document.iFrameDoc.document.frames[0].document.forms[0].submit();
			}	
		</script>		
	    <script type="text/javascript" language="javascript">

	            
            function ShowDetailsSendDocResponse()
            {
	            var args=new Object;
	            args.window=window;

	            var height=screen.availHeight;
	            var width=screen.availWidth;
            	
	            height=(height * 63) / 100;
	            width=(width * 70) / 100;
            	
	            width=920;
	            height=500;

	            window.showModelessDialog('../Interoperabilita/SendDocResponse.aspx',
			            '',
			            'dialogHeight: ' + height + 'px; dialogWidth: ' + width + 'px; resizable: no;status:no;scroll:yes;help:no;close:no');
            }

            function apriFinestraConferma(conferma)
            {
                var returnValue = window.showModalDialog('../popup/confermaSpedizione.aspx?conferma=' + conferma,'','dialogWidth:490px;dialogHeight:180px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:0;left:0');
                if(returnValue == null)
                {
                    document.getElementById("hdConfirmSpedisci").value = "errore";
	                document.getElementById("hdnSpedisciConInterop").value = "errore";
		            document.getElementById("hdn_idRegRF").value = "errore";
                }
                else
                {
                    docProtocollo.hdn_returnConfermaSped.value = returnValue;
                    var mySplitResult = returnValue.split("^");
                    if(mySplitResult[0] == 'OK')
                    {
		                document.getElementById("hdConfirmSpedisci").value = "Yes";
		                document.getElementById("hdnSpedisciConInterop").value = '1';
		                document.getElementById("hdn_idRegRF").value = mySplitResult[1];
		                document.docProtocollo.submit();	
		                doWait();
                    }
                }
            }

            function apriRFSegnatura(codice, ddlTempl)
            {
                var returnValue = window.showModalDialog('../popup/sceltaRFSegnatura.aspx?codice=' + codice,'','dialogWidth:500px;dialogHeight:260px;status:no;resizable:yes;scroll:no;center:yes;help:no;close:no;top:0;left:0');
                if(returnValue == null)
                {
                    document.getElementById("hdnCodRFSegnatura").value = "errore";
	                document.getElementById("hdnIdRFSegnatura").value = "errore";
                }
                else
                {
                    var mySplitResult = returnValue.split("^");
                    if(mySplitResult[0] == 'OK')
                    {
		                document.getElementById("hdnCodRFSegnatura").value = mySplitResult[2];
		                document.getElementById("hdnIdRFSegnatura").value = mySplitResult[1];
		                var codRF_ar = mySplitResult[2].split(" - ");
		                var codRF = "";
		                if(codRF_ar[0] != '')
		                {
		                    codRF = codRF_ar[0];
		                }
		                else
		                {
		                    codRF = mySplitResult[2]
		                }
    		            
		                var url = window.document.location.href;
		                if(url.indexOf('?') == -1)
		                    url = url + '?';
		                    else
		                    url = url + '&';
    		                
		                    url = url + 'protocolla=1&idRFSeg=' + mySplitResult[1] + '&codRFSeg=' + codRF;
                    	if (ddlTempl != '0')
                        	url = url + '&ddlTempl=' + ddlTempl;
		                window.document.location = url;	
                    }
                }
            }

            function apriRFRicevuta(codice, idRfSeg, codRfSeg)
            {
                var returnValue = window.showModalDialog('../popup/sceltaRFSegnatura.aspx?codice=' + codice,'','dialogWidth:460px;dialogHeight:260px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:0;left:0');
                if(returnValue == null)
                {
                    document.getElementById("hdnCodRFSegnatura").value = "errore";
	                document.getElementById("hdnIdRFSegnatura").value = "errore";
                }
                else
                {
                    var mySplitResult = returnValue.split("^");
                    if(mySplitResult[0] == 'OK')
                    {
		                var codRF_ar = mySplitResult[2].split(" - ");
		                var codRF = "";
		                if(codRF_ar[0] != '')
		                {
		                    codRF = codRF_ar[0];
		                }
		                else
		                {
		                    codRF = mySplitResult[2]
		                }		           
		                var url = window.document.location.href;
		                if(url.indexOf('?') == -1)
		                    url = url + '?';
		                    else
		                    url = url + '&';
    		                
		                    url = url + 'protocolla=1&idRFSeg=' + idRfSeg + '&codRFSeg=' + codRfSeg + '&idRFRicevuta=' + mySplitResult[1];
		                window.document.location = url;	
                    }
                }
            }

            function apriRFInvioManuale(codice)
            {
                var returnValue = window.showModalDialog('../popup/sceltaRFSegnatura.aspx?codice=' + codice,'','dialogWidth:460px;dialogHeight:260px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:0;left:0');
                if(returnValue == null)
                {
                    document.getElementById("hdnCodRFSegnatura").value = "errore";
	                document.getElementById("hdnIdRFSegnatura").value = "errore";
                }
                else
                {
                    var mySplitResult = returnValue.split("^");
                    if(mySplitResult[0] == 'OK')
                    {
		                var codRF_ar = mySplitResult[2].split(" - ");
		                var codRF = "";
		                if(codRF_ar[0] != '')
		                {
		                    codRF = codRF_ar[0];
		                }
		                else
		                {
		                    codRF = mySplitResult[2]
		                }		           
		                var url = window.document.location.href;
		                if(url.indexOf('?') == -1)
		                    url = url + '?';
		                    else
		                    url = url + '&';
    		                
		                    url = url + 'invioManuale=1&idRFRicevuta=' + mySplitResult[1];
		                window.document.location = url;	
                    }
                }
            }

            function callDettCorr(param) {

                var features = "dialogWidth:650px; dialogHeight:750px;scroll:yes";
                var splitted = param.split('&');
                var cod = splitted[0];
                var ie = splitted[1];
                var t = splitted[2];
                var sysid = splitted[3];
                var idDoc = splitted[4];
                var btn_nuovo = "";
                var btn_nuovo_bis = "";
                var sameMail = "";
                var avviso = "";
                var idAOOCOLL = "";
                if (splitted.length > 5) {
                    if (splitted[5] == "same_mail") {
                        sameMail = splitted[5];
                        idAOOCOLL = splitted[6];
                    }
                    else if (splitted[5] == "btn_nuovo") {
                        btn_nuovo = splitted[5];
                        if (splitted.length == 7)
                            if (splitted[6] == 'avviso2')
                                avviso = 2;
                    }
                    else if (splitted[5] == "avviso1") {
                        avviso = 1; 
                    }
                }
                if (splitted.length == 8)
                    avviso = 2;
                if (splitted.length > 8) {
                    btn_nuovo = "btn_nuovo";
                    avviso = 2;
                }
                var returnValue = window.showModalDialog('../popup/rubrica/Dettagli.aspx?cod=' + cod + '&ie=' + ie + '&t=' + t + '&rc=&sysid=' + sysid + '&newCorr=1&idDoc=' + idDoc + '&sameMail=' + sameMail + '&idAOOCOLL=' + idAOOCOLL + '&btn_nuovo=' + btn_nuovo + '&avviso=' + avviso + '&k=1', '', features);
                if (returnValue != null) {
                    window.returnValue = returnValue;
                    window.location.href = window.location.href + "&protocolla=1&"+ returnValue;
                }
                
            }

            function alert_k2() {
                var features = "dialogWidth:200px; dialogHeight:100px;scroll:no";
                var returnValue = window.showModalDialog('../popup/rubrica/AlertNoInteropZeroK2.aspx', '', features);
                if (returnValue != null) {
                    window.returnValue = returnValue;
                    window.location.href = window.location.href + "&protocolla=1&" + returnValue;
                }
            }

            function ProtocollazioneInterna(target)
            {
	            if(docProtocollo.hd_tipoProtocollazione.value == 'Own')
	            {
	                target += '&interno=true';
	            }	
            	
	            return target;
            }

            function verificaApriRubrica(wnd, target, abilita)
            {
	            if (abilita.toLowerCase()=='true')
	            {
		            target = ProtocollazioneInterna(target);
		            if((docProtocollo.hd_tipoProtocollazione.value == 'Out') && (target == 'mitt'))
		            {
			            target += 'P';
		            }
		            ApriRubrica(wnd, target);
	            }
            }
            
            function ApriFinestraADL(pagina) 
            {
	            win=window.open(pagina, "AreaLavoro","width=680,height=450, scrollbars=yes"); 
	            win.focus();
            }
            
            function verificaApriRubricaSemplice(wnd, target, abilita)
            {
	            if (abilita.toLowerCase()=='true')
	            {
		            target = ProtocollazioneInterna(target);
		            ApriRubricaSemplice(wnd, target);
	            }
            }

            function verificaChangeCursorT(tipo,name,abilita)
            {
	            if (abilita.toLowerCase()=='true')
	            {
		            ChangeCursorT(tipo,name);
	            }	
            }

            function doWait()
            {
	            window.parent.parent.document.getElementById ("please_wait").style.top = 250;
	            window.parent.parent.document.getElementById ("please_wait").style.left = 320;
	            window.parent.parent.document.getElementById ("please_wait").style.display = '';
            }	

            function _ApriRubrica(target)
            {
	            var r = new Rubrica();
	            r.CorrType = r.Esterni;
            	
	            switch (target) {
		            // Mittente su protocollo in ingresso
		            case "mitt":
			            r.CallType = r.CALLTYPE_PROTO_IN;
			            break;
            			
		            // Mittente intermedio su protocollo in ingresso			
		            case "mittInt":
			            r.CallType = r.CALLTYPE_PROTO_IN_INT;
			            break;		
            			
		            // Destinatari			
		            case "dest":
			            r.CallType = r.CALLTYPE_PROTO_OUT;
			            break;			
            			
		            // Mittente su protocollo in uscita			
		            case "mittOut":
			            r.CallType = r.CALLTYPE_PROTO_OUT_MITT;
			            break;		
            			
		            // Mittente su protocollo interno		
		            case "mittInterno":
			            r.CallType = r.CALLTYPE_PROTO_INT_MITT;
			            break;	
            			
		            // Destinatario su protocollo interno		
		            case "destInterno":
			            r.CallType = r.CALLTYPE_PROTO_INT_DEST;
			            break;				
		            // Ufficio referente su protocollo in entrata
		            case "uffref_proto":
			            r.CallType = r.CALLTYPE_UFFREF_PROTO;
			            break;
                    //Mittenti Multipli
			        case "mittMultiplo":
			            r.CallType = r.CALLTYPE_MITT_MULTIPLI;
			            break;									
	            }
	            var res = r.Apri(); 		
            }
		
			var w = window.screen.width;
			var h = window.screen.height;
			var new_w = (w-100)/2;
			var new_h = (h-400)/2;
			
			function apriPopupAnteprima(btn_salva,btn_protocolla,btn_protocolla_g) 
			{
				//window.open('AnteprimaProfDinamica.aspx','','top = '+ new_h +' left = '+new_w+' width=500,height=400,scrollbars=YES');
				//window.showModalDialog('AnteprimaProfDinModal.aspx?Chiamante=AnteprimaProfDinamica.aspx','','dialogWidth:510px;dialogHeight:400px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:'+ new_h +';left:'+new_w);
				//window.showModalDialog('AnteprimaProfDinModal.aspx?Chiamante=AnteprimaProfDinamica.aspx&btn_salva='+btn_salva+'&btn_protocolla='+btn_protocolla+'&btn_protocolla_g='+btn_protocolla_g,'','dialogWidth:510px;dialogHeight:400px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:'+ new_h +';left:'+new_w);
				window.showModalDialog("AnteprimaProfDinModal.aspx?Chiamante=AnteprimaProfDinamica.aspx?pulsanti="+btn_salva+"-"+btn_protocolla+"-"+btn_protocolla_g,'','dialogWidth:510px;dialogHeight:400px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:'+ new_h +';left:'+new_w);
				//document.docProtocollo.submit();				
			}	
			
			// Script per la gestione dialog RicercaProtocolliUscita
			function ApriFinestraProtocolliInUscita()
			{
				var newLeft=(screen.availWidth-602);
				var newTop=(screen.availHeight-649);	
			
				rtnValue=window.showModalDialog('../popup/RicercaProtocolliUscita.aspx','','dialogWidth:596px;dialogHeight:641px;status:no;dialogLeft:'+newLeft+'; dialogTop:'+newTop+';center:no;resizable:no;scroll:yes;help:no;');				
				window.document.docProtocollo.submit();
			}
			
			// Script per la gestione dialog RicercaProtocolliUscita
			function ApriFinestraProtocolliInIngresso()
			{
				var newLeft=(screen.availWidth-602);
				var newTop=(screen.availHeight-649);	
			
				rtnValue=window.showModalDialog('../popup/RicercaProtocolliIngresso.aspx','','dialogWidth:596px;dialogHeight:641px;status:no;dialogLeft:'+newLeft+'; dialogTop:'+newTop+';center:no;resizable:no;scroll:yes;help:no;');				
				window.document.docProtocollo.submit();
			}

			// Script per la gestione dialog RicercaProtocolli
			function ApriFinestraDocumenti(tipo) {
			    var newLeft = (screen.availWidth - 602);
			    var newTop = (screen.availHeight - 649);

			    rtnValue = window.showModalDialog('../popup/RicercaProtocolli.aspx?tipo=' + tipo, '', 'dialogWidth:596px;dialogHeight:641px;status:no;dialogLeft:' + newLeft + '; dialogTop:' + newTop + ';center:no;resizable:no;scroll:yes;help:no;');
			    window.document.docProtocollo.submit();
			}

			function ApriFinestraScegliDestinatari()
			{
			
			
				rtnValue=window.showModalDialog('../popup/ScegliDestinatari.aspx','','dialogWidth:596px;dialogHeight:300px;status:no;center:yes;resizable:no;scroll:yes;help:no;');				
				top.principale.iFrame_sx.document.location = 'tabGestioneDoc.aspx?tab=protocollo'
			}
			
			//Apre la finistra contenente i protocolli a cui rispondere
			function ShowDialogRispostaProtocollo(sys,reg,segn,tipoProto) 
			{
				var newLeft=(screen.availWidth-602);
				var newTop=(screen.availHeight-625);	
				
				var retValue=
					window.showModalDialog('../popup/listaDocInRisposta.aspx?sys=' + sys + '&regis=' + reg + '&seg=' + segn + '&tipo=' + tipoProto,'',
											'dialogHeight:620px; dialogWidth:595px;status:no;dialogLeft:'+newLeft+';dialogTop:'+newTop+';center:no;help:no;');
																					
				if (retValue=='true')
				{	
					window.open('../documento/gestionedoc.aspx?tab=protocollo','principale');
				}
			}
			//
			function ApriFinestraScegliUO(corrDesc)
			{
				var newLeft=(screen.availWidth-602);
				var newTop=(screen.availHeight-625);	
				
				rtnValue=window.showModalDialog('../popup/scegliUoUtente.aspx?win=protocollo&rubr='+corrDesc,'','dialogWidth:550px;dialogHeight:290px;status:no;dialogLeft:'+newLeft+'; dialogTop:'+newTop+';center:no;resizable:no;scroll:no;help:no;');				
				window.document.docProtocollo.submit();					
			}
			
            //dettagli cor
			function ApriFinestraCor(url, indexCor, tipoCorr) 
            {
                //Controllo selezione corrispondente  
                var s = indexCor;
                if (url.indexOf("tipoCor=M") != -1 && s.indexOf("&") != -1) {
                    indexCor = indexCor.replace(/&/g, "________");
                }
                var tipoCor = url.substr(url.length - 1, 1); 	
	            var newLeft=(screen.availWidth-595);
	            var newTop=(screen.availHeight-625);		//625
	            if (tipoCor == "M" || tipoCor == "I")
               {
                   if (indexCor == "")
		            { 
			            alert("Dati sul Mittente non trovati");
			            return;
		            }
		            var urlPar = url + "&indexCor=" + indexCor;			
	                window.open(urlPar,"DettagliCor","toolbar=no,location=no,directories=no, scrollbars=no,status=no,top="+newTop+",left="+newLeft+"resizable=no,copyhistory=no, width=440, height=580"); 
	            }
	            else if (tipoCor == "D" || tipoCor == "C")
	            {
		            if (indexCor == "-1")
		            {
			            alert("Destinatario non selezionato");
			            return;
		            }
		            var urlPar = url + "&indexCor=" + indexCor;			
	               window.open(urlPar,"DettagliCor","toolbar=no,location=no,directories=no, scrollbars=no,top="+newTop+",left="+newLeft+"status=no,resizable=no,copyhistory=no, width=440, height=580");
	            }
	        }

	       function ApriFinestraDettMittMultipli(url, indexCor, tipoCorr) {
	           //Mittenti Multipli
	           var tipoCor = url.substr(url.length - 2, 2);
	           var newLeft = (screen.availWidth - 595);
	           var newTop = (screen.availHeight - 625); 	//625
	           if (tipoCor == "MD") {
	               if (indexCor == "-1") {
	                   alert("Mittente non selezionato");
	                   return;
	               }
	               var urlPar = url + "&indexCor=" + indexCor;
	               window.open(urlPar, "DettagliCor", "toolbar=no,location=no,directories=no, scrollbars=no,status=no,top=" + newTop + ",left=" + newLeft + "resizable=no,copyhistory=no, width=440, height=580");
	           }
	       }

            function ApriFinestraNotificaCor(url,indexCor,tipoCorr)
            {
               //controllo selezione corrispondente  
	            var tipoCor = url.substr(url.length-1,1); 	
	            //var newLeft=(screen.availWidth-300);
	            //var newTop=(screen.availHeight-625);
	            var newLeft=(screen.availWidth-602);
	            var newTop=(screen.availHeight-649);	
            			
              
	            var urlPar = url + "&indexCor=" + indexCor;	
	            //window.open(urlPar,"Notifica","toolbar=no,location=no,directories=no, scrollbars=no,top="+newTop+",left="+newLeft+"status=no,resizable=no,copyhistory=no, width=550, height=450");
	            window.showModalDialog(urlPar,'Notifica','dialogWidth:660px;dialogHeight:600px;status:no;resizable:no;scroll:auto;center:yes;help:no;close:no;top:'+ newTop +';left:'+newLeft);
            }

            function ApriFinestraSpedizioni(url)
            {
               //controllo selezione corrispondente  
	            var newLeft=(screen.availWidth-300);
	            var newTop=(screen.availHeight-625);                
	            window.showModalDialog(url,'Spedizioni','dialogWidth:580px;dialogHeight:475px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:'+ newTop +';left:'+newLeft);
            }
            
            function ApriconfirmSpedizioneDoc()
            {
	            document.getElementById('btn_spedisci_P').style.display='none';
	            document.getElementById('btn_spedisciDisabled').style.display='';
  	            var response = window.confirm("Il documento è già stato spedito.\nConfermi la spedizione?");
	            if (response)
	            {
		            document.getElementById("hdConfirmSpedisci").value = "Yes";
		            document.getElementById("hdnSpedisciConInterop").value = '1';
		            document.docProtocollo.submit();	
	            }	
	            else
	            {
		            document.getElementById("hdConfirmSpedisci").value = 'No';
		            if(window.parent.parent.document.getElementById ("please_wait")!=null)
		            {	
			            window.parent.parent.document.getElementById ("please_wait").style.display = 'none';
		            }
		            document.getElementById("hdnSpedisciConInterop").value = '0';
            		
		            document.getElementById('btn_spedisci_P').style.display='';
		            document.getElementById('btn_spedisciDisabled').style.display='none';
	            }		
            }

            function ApriconfirmSpedizioneDocAcquisito()
            {
	            document.getElementById('btn_spedisci_P').style.display='none';
	            document.getElementById('btn_spedisciDisabled').style.display='';
   	            var response = window.confirm("Non è stato acquisito alcun documento elettronico.\nConfermi la spedizione?");
	            if (response)
	            {
		            document.getElementById("hdConfirmSpedisciAcquisito").value = "Yes";
		            document.getElementById("hdnSpedisciConInterop").value = '1';
                    document.docProtocollo.submit();	
                }	
	            else
	            {
		            document.getElementById("hdConfirmSpedisciAcquisito").value = 'No';
		            if(window.parent.parent.document.getElementById ("please_wait")!=null)
		            {	
			            window.parent.parent.document.getElementById ("please_wait").style.display = 'none';
		            }
		            document.getElementById("hdnSpedisciConInterop").value = '0';
  		            document.getElementById('btn_spedisci_P').style.display='';
		            document.getElementById('btn_spedisciDisabled').style.display='none';
	            }		
            }

            function ApriconfirmSpedizioneDocUnica(avviso)
            {
	            document.getElementById('btn_spedisci_P').style.display='none';
	            document.getElementById('btn_spedisciDisabled').style.display='';
	            
  	            var response = window.confirm(avviso + "\nConfermi la spedizione?");
	            if (response)
	            {
		            document.getElementById("hdConfirmSpedisci").value = "Yes";
		            document.getElementById("hdnSpedisciConInterop").value = '1';
            			
		            document.docProtocollo.submit();	
		            
		            doWait();
            		
	            }	
	            else
	            {
		            document.getElementById("hdConfirmSpedisci").value = 'No';		          
		            document.getElementById("hdnSpedisciConInterop").value = '0';
            		
		            document.getElementById('btn_spedisci_P').style.display='';
		            document.getElementById('btn_spedisciDisabled').style.display='none';
	            }		
            }
            
		    function nascondi()
		    {
			    document.getElementById('btn_protocollaDisabled').style.display='none';
			    document.getElementById('btn_spedisciDisabled').style.display='none';
		    }
		
		    //Apre la PopUp Modale per la ricerca dei fascicoli
		    //utile per la fascicolazione rapida
		    function ApriRicercaFascicoli(codiceClassifica) 
		    {
			    var newUrl;
    		    newUrl="../popup/ricercaFascicoli.aspx?codClassifica=" + codiceClassifica+"&caller=protocollo";
    		    var newLeft=(screen.availWidth-615);
			    var newTop=(screen.availHeight-710);	
    	        // apertura della ModalDialog
			    rtnValue = window.showModalDialog(newUrl,"","dialogWidth:615px;dialogHeight:700px;status:no;resizable:no;scroll:auto;dialogLeft:"+newLeft+";dialogTop:"+newTop+";center:no;help:no;"); 
			   
			    if (rtnValue == "Y")
			    {
				    window.document.docProtocollo.submit();
			    }
		    }
		
		    // Script apertura Modale Stampa Etichetta PDF A4
		   function OpenModalLabel()
		   {
		     var newLeft=(screen.availWidth -(screen.availWidth-5) );
			 var newTop=(screen.availHeight-545);	
			 // apertura della ModalDialog
		     window.showModalDialog('../popup/printLabelPdfFrame.aspx?proto=true','','dialogWidth:410px;dialogHeight:550px;status:no;resizable:no;scroll:no;center:no;help:no;close:no;dialogLeft:'+newLeft+';dialogTop:'+newTop+';');
		    }
		    
		    function ApriDescrizioneCampo(tipoCampo)
		    {
		        var newLeft=(screen.availWidth-600);
			    var newTop=(screen.availHeight-622);	
    			var newUrl;
			
			    newUrl="../popup/dettaglioCampo.aspx?tipoCampo=" + tipoCampo;
			
			    window.showModalDialog(newUrl,"","dialogWidth:440px;dialogHeight:450px;status:no;resizable:no;scroll:no;dialogLeft:"+newLeft+";dialogTop:"+newTop+";center:no;help:no;"); 
    	    }
    	    
    	    function SxRefresh()
    	    {
    	        top.principale.iFrame_sx.document.location='docProtocollo.aspx';
    	    }    	        	    
    	    
    	    function VaiRispostaProtocollo()
			{
			   top.principale.document.location = "../documento/gestionedoc.aspx?tab=protocollo";
			}

			function StampaRicevutaPdf() {
			    var retValue = false;

			    try {
			        // stampa la ricevuta solo se esiste la segnatura (infatti i predisposti non hanno la segnatura!)
			        if (document.getElementById("lbl_segnatura").value != '') {

			            window.open("<%=DocsPAWA.Utils.getHttpFullPath()%>/documento/VisualizzaStampaRicevutaPdf.aspx?IdDocument=<%=GetIdDocumento()%>");
			        }
			        else {
			            alert('Non è possibile ottenere la ricevuta di un documento NON protocollato');
			        }
			    }
			    catch (ex) {
			        alert(ex.message.toString());
			        retValue = false;
			    }

			    return retValue;
			}

			function StampaRicevuta() {
			    var retValue = false;
			    
				try
				{
				    // stampa la ricevuta solo se esiste la segnatura (infatti i predisposti non hanno la segnatura!)
				    if (document.getElementById("lbl_segnatura").value != '') 				    				    
				    {				    				    
			            var fso = FsoWrapper_CreateFsoObject();
			            var filePath = fso.GetSpecialFolder(2).Path + "\\stampaRicevutaDocspa.<%=GetModelProcessorDefaultExtension()%>";

                        // Elaborazione del documento per il modello di stampa richiesto
                        if (ClientModelProcessor_ProcessModel(filePath, "<%=GetIdDocumento()%>", "STAMPA_RICEVUTA")) {
                            // Apertura del file con l'applicazione proprietaria
                            ShellWrappers_Execute(filePath);
                            retValue = true;
                        }
					}
					else
					{
					    alert('Non è possibile ottenere la ricevuta di un documento NON protocollato');
					}
				}
				catch (ex)
				{
				    alert(ex.message.toString());
				    retValue = false;
				}

				return retValue;
			}    
			
			function CreateObject(objectType)
	        {
		        try
		        {
			        return new ActiveXObject(objectType);
		        }
		        catch (ex)
		        {
			        alert("Oggetto '" + objectType + "' non istanziato");
		        }	
	        }	
	        
	        function VisualizzaAttendi()
			{				
				window.document.body.style.cursor='wait';
				
				var w_width = 200;
				var w_height = 50;	
				var t = (screen.availHeight - w_height) / 2;
				var l = (screen.availWidth - w_width) / 2;	
				if(document.getElementById ("WAIT"))
				{		
					document.getElementById ("WAIT").style.top = t;
					document.getElementById ("WAIT").style.left = l;
					document.getElementById ("WAIT").style.width = w_width;
					document.getElementById ("WAIT").style.height = w_height;				
					document.getElementById ("WAIT").style.visibility = "visible";
				}				
			}	
			
			function ApriStoriaConservazione(idProfile)
            {
                var newLeft=(screen.availWidth-600);
		        var newTop=(screen.availHeight-622);	
			    var newUrl;
    		
		        newUrl="../popup/storiaDocConservato.aspx?idProfile=" + idProfile;
    		
		        window.showModalDialog(newUrl,"","dialogWidth:650px;dialogHeight:350px;status:no;resizable:no;scroll:no;dialogLeft:"+newLeft+";dialogTop:"+newTop+";center:no;help:no;");
		    }

		    function ShowDialogSpedizioneDocumento() {
		        var pageHeight = (screen.availHeight / 1.1);
		        var pageWidth = (screen.availWidth / 1.1);
                
		        return (window.showModalDialog('../Spedizione/SpedizioneDocumento.aspx', null, 'dialogWidth:' + pageWidth + 'px;dialogHeight:' + pageHeight + 'px;status:no;resizable:no;scroll:yes;center:yes;help:no;close:no;') == "True");
		    }
	        
	        function OnErrorSpedizioneAutomaticaDocumento() {
	            alert('Attenzione: non è stato possibile spedire il documento ad uno o più destinatari');
	            
	            if (ShowDialogSpedizioneDocumento())
	                top.principale.iFrame_sx.document.location = 'tabGestioneDoc.aspx?tab=protocollo';
	        }
            
            function AvvisoVisibilita()
            {
                var newLeft=(screen.availWidth-500);
		        var newTop=(screen.availHeight-500);	
			    var newUrl;
    		
		        newUrl="../popup/estensioneVisibilita.aspx";
    		    
	            if(IsCheckBoxRequired())
	            {
	                retValue=window.showModalDialog(newUrl,"","dialogWidth:306px;dialogHeight:188px;status:no;resizable:no;scroll:no;dialogLeft:"+newLeft+";dialogTop:"+newTop+";center:yes;help:no;");
    	            
	                if(retValue == 'NO')
	                {
	                    document.getElementById("estendiVisibilita").value = "true";
	                }
	                else
	                {
	                    document.getElementById("estendiVisibilita").value = "false";
	                }
	            }
            }

            function IsCheckBoxRequired() {
                if (document.getElementById("chkPrivato") != null &&
              document.getElementById("chkPrivato").checked == true) {
                    return true;
                }
                else {
                    return false;
                }
            }

            function ApriFinestraMultiCorrispondentiMittenti() {
                rtnValue = window.showModalDialog('../popup/MultiDestinatari.aspx?tipo=M', '', 'dialogWidth:700px;dialogHeight:350px;status:no;center:yes;resizable:no;scroll:yes;help:no;');
                top.principale.iFrame_sx.document.location = 'tabGestioneDoc.aspx?tab=protocollo'
            }

            function ApriFinestraMultiCorrispondentiDestinatari() {
                rtnValue = window.showModalDialog('../popup/MultiDestinatari.aspx?tipo=D', '', 'dialogWidth:700px;dialogHeight:350px;status:no;center:yes;resizable:no;scroll:yes;help:no;');
                top.principale.iFrame_sx.document.location = 'tabGestioneDoc.aspx?tab=protocollo'
            }

            // Scelta effettuata nella maschera di scelta a tre posizioni
            var isChoice = 0;
            /*
             * Funzione per mostrare un popup di richiesta conferma alla prosecuzione della protocollazione quando è
             * stato ricevuto un protocollo marcato privato dal mittente
            */
            function protoPrivatoDaMittente() {

                var txt = 'Attenzione, il documento è stato marcato come privato da AOO mittente, vuoi mantenere questa impostazione?';
                var caption = '<%= GetWindowTitle() %>';
                vbMsg(txt, caption)

            }

		</script>
        <script language="vbscript" type="text/vbscript">

            Function vbMsg(isTxt,isCaption)

                testVal = MsgBox(isTxt,3,isCaption)
                isChoice = testVal

                If isChoice <> 2 Then
                    document.docProtocollo.action = "docProtocollo.aspx?userChoice=" & isChoice
                    document.docProtocollo.submit
                End If
            End Function

        </script>
		<script type="text/javascript">
		    // Al load della pagina vengono registrati gli eventi di mouse move
		    // su documento e lbl_segnatura
		    function pageLoad() {
		        // Prelevamento di un riferimento alla casella di testo con la segnatura
		        var lblSegnatura = $get("lbl_segnatura");

		        // Se lbl_segnatura contiene del testo, vengono associati gli eventi
		        if (lblSegnatura.value != null && lblSegnatura.value.trim() != "") {
		            // Associazione degli eventi
		            $addHandler(lblSegnatura, "mousemove", showZoom);
		            $addHandler(lblSegnatura, "mouseout", hideZoom);
		        }
		        
		    }

		    // Quando ci si muove all'interno della lbl_segnatura bisogna mostrare
            // lo zoom sulla segnatua
		    function showZoom(sender, args) {
		        // Prelevamento di un riferimento alla lbl_segnatura
		        var lblSegnatura = $get("lbl_segnatura");

		        // Se lbl_segnatura contiene del testo, viene mostrato nel pannello di zoom
		        // di zoom
		        if (lblSegnatura.value != null && lblSegnatura.value.trim() != "") {
		            // Calcolo dei bounds della casella lbl_segnatura
		            var lblSegnaturaBounds = Sys.UI.DomElement.getBounds(lblSegnatura);

		            // La segnatura da visualizzare
		            var segnValue = new Sys.StringBuilder();
		            var numChar = 0;
		            
		            // Spezzettamento della segnatura in modo da mostrare solo 18 caratteri per linea
		            for (var i = 0; i < lblSegnatura.value.length; i++) {
		                segnValue.append(lblSegnatura.value.charAt(i));

		                numChar++;

		                if (numChar == 20) {
		                    segnValue.append('<br />');
		                    numChar = 0;
                        }
                    }

		            // Copia della segnatura nello span lblZoom
		            lblZoom.innerHTML = segnValue;

                    // Prelevamento di un riferimento all'extender di zoom
		            this._popup = $find('mpeZoom');

		            // Prelevamento di un riferimento al pannello dello zoom
		            var zoom = $get("pnlZoom");

		            // Prelevamento dei bounds del pannello di zoom
		            var pnlZoomBounds = Sys.UI.DomElement.getBounds(zoom);

		            // Ridimensionamento del pannello
		            zoom.style.width = lblSegnaturaBounds.width;

		            // Riposizionamento del popup sotto alla casella della segnatura
		            Sys.UI.DomElement.setLocation(zoom, 0, 0);

                    // Viene mostrato il popup
		            this._popup.show();

		            // Posizionamento del pannello di zoom
		            // sotto la casella di testo della segnatura
		            zoom.style.top = lblSegnaturaBounds.y + (lblSegnaturaBounds.height / 2);
		            
		        }
		       
		    }

            // Quando ci si muove sul documento bisogna nascondre lo zoom
		    function hideZoom(sender, args) {
		        this._popup = $find('mpeZoom');
		        this._popup.hide();
		    }            
		</script>	

        <style type="text/css">
    .autocomplete_completionListElementbis
    {
        height: 280px;
        list-style-type: none;
        margin: 0px;
        padding: 0px;
        font-size: 10px;
        color: #333333;
        line-height: 18px;
        border: 1px solid #333333;
        overflow: auto;
        padding-left: 1px;
        background-color: #ffffff;
        font-family: Verdana, Arial, sans-serif;
        z-index : 1004;
    }
    
    .single_itembis
    {
        border-bottom: 1px dashed #cccccc;
        padding-top: 2px;
        padding-bottom: 2px;
    }
    
    .single_item_hoverbis
    {
        border-bottom: 1px dashed #cccccc;
        background-color: #9d9e9c;
        color: #000000;
        padding-top: 2px;
        padding-bottom: 2px;
    }

    .selectedWord{
        font-weight:bold;
        color:#000000;
        text-decoration:underline;
    }

</style>
	</HEAD>
	<body>
		<form id="docProtocollo" method="post" runat="server">
		<asp:ScriptManager ID="ScriptManager" AsyncPostBackTimeout="360000" runat="server"></asp:ScriptManager>
            <asp:HiddenField ID="fieldsOK" runat="server" />
            <asp:HiddenField ID="isInterno" runat="server" />
            <asp:HiddenField ID="appoIdMod" runat="server" />
            <asp:HiddenField ID="appoIdAmm" runat="server" />
            <asp:HiddenField ID="abilitaModaleVis" runat="server" />
            <asp:HiddenField ID="estendiVisibilita" runat="server" />
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Protocollo" />	
		    <uc5:ClientModelProcessor ID="clientModelProcessor" runat="server" />
			<input id="h_tipoAtto" style="Z-INDEX: 101; LEFT: 336px; WIDTH: 24px; POSITION: absolute; TOP: 40px; HEIGHT: 18px"
				type="hidden" name="h_tipoAtto" runat="server" />
			<input id="hd_tipoProtocollazione" style="Z-INDEX: 102; LEFT: 8px; POSITION: absolute; TOP: 64px"
				type="hidden" name="hd_tipoProtocollazione" runat="server" /> 
			<!--x stampa etichette:-->
			<input id="hd_UrlIniFileDispositivo" type="hidden" name="hd_UrlIniFileDispositivo" runat="server" />
			<input id="hd_dispositivo" type="hidden" name="hd_dispositivo" runat="server" /> 
			<input id="hd_amministrazioneEtichetta" type="hidden" name="hd_amministrazioneEtichetta" runat="server" /> 
			<input id="hd_descrizioneAmministrazione" type="hidden" name="hd_descrizioneAmministrazione" runat="server" /> 
			<input id="hd_classifica" type="hidden" name="hd_classifica" runat="server" />
			<input id="hd_fascicolo" type="hidden" name="hd_fascicolo" runat="server" /> 
			<input id="hdnSpedisciConInterop" type="hidden" name="hdnSpedisciConInterop" runat="server" />
			<input id="hd_num_proto" type="hidden" name="hd_num_proto" runat="server" /> 
			<input id="hd_anno_proto" type="hidden" name="hd_anno_proto" runat="server" />
			<input id="hd_data_proto" type="hidden" name="hd_data_proto" runat="server" /> 
			<input id="hd_codreg_proto" type="hidden" name="hd_codreg_proto" runat="server" />
			<input id="hd_descreg_proto" type="hidden" name="hd_descreg_proto" runat="server" />
			<input id="hd_coduo_proto" type="hidden" name="hd_coduo_proto" runat="server" /> 
			<input id="hd_tipo_proto" type="hidden" name="hd_tipo_proto" runat="server" />
			<input id="hd_num_doc" type="hidden" name="hd_num_doc" runat="server" /> 
			<input id="hdConfirmSpedisci" type="hidden" name="hdConfirmSpedisci" runat="server" />
			<input id="hdConfirmSpedisciAcquisito" type="hidden" name="hdConfirmSpedisciAcquisito" runat="server" /> 
			<input id="hd_numero_allegati" type="hidden" name="hd_numero_allegati" runat="server" />
			<input id="hd_codiceUoCreatore" type="hidden" name="hd_codiceUoCreatore" runat="server" />
			<input id="hd_dataCreazione" type="hidden" name="hd_dataCreazione" runat="server" />
			<input id="isFascRequired" type="hidden" name="isFascRequired" runat="server" />
            <input type="hidden" name="hdn_returnConfermaSped" id="hdn_returnConfermaSped" runat="server" />
            <input type="hidden" name="hdn_idRegRF" id="hdn_idRegRF" runat="server" />
			<input id="hdnCodRFSegnatura" type="hidden" name="hdnCodRFSegnatura" runat="server" />
			<input id="hdnIdRFSegnatura" type="hidden" name="hdnIdRFSegnatura" runat="server" />
            <input id="hd_modello_dispositivo" type="hidden" name="hd_modello_dispositivo" runat="server" />
            <input id="hd_num_stampe" type="hidden" name="hd_num_stampe" runat="server" />
            <input id="hd_num_stampe_effettuate" type="hidden" name="hd_num_stampe_effettuate" runat="server" />
            <input id="hd_ora_creazione" type="hidden" name="hd_ora_creazione" runat="server" />
            <input id="hd_dataArrivo" type="hidden" name="hd_dataArrivo" runat="server" />
            <input id="hd_dataArrivoEstesa" type="hidden" name="hd_dataArrivoEstesa" runat="server" />
			<!--end x stampa etichette:-->
			<uc1:AclDocumento id="aclDocumento" runat="server"></uc1:AclDocumento>
			<div id="divScroll">
				<table id="tbl_contenitore" height="100%" cellspacing="0" cellpadding="0" width="415" align="center" border="0">
					<tr vAlign="top">
						<td style="WIDTH: 413px" align="left">
							<table class="contenitore" id="Table1" height="100%" cellSpacing="0" cellPadding="0" width="100%" border="0">
								<tr height="5" valign="bottom" align="center">
									<td class="titolo_scheda" height="5" colspan="1">
										<asp:Label id="lblStatoConsolidamento"  CssClass="testo_red"  runat="server" Width="100%" Visible="false"></asp:Label>
									</td>
								</tr>
								<tr>
									<td class="titolo_scheda" align="center">
										<table id="Table10" cellSpacing="0" cellPadding="0" width="95%" border="0" >
										<!--table id="Table2" width="95%" cellSpacing="0" cellPadding="0" width="95%" class="info_grigio1"-->
											<tr >
												<td vAlign="middle" width="33%">
												    <table class="info_grigio1" cellSpacing="0" cellPadding="0" width="100%"><tr>
												        <td vAlign="middle" align="left" width="100%" >
												            
								
												            <asp:radiobuttonlist id="rbl_InOut_P" CssClass="testo_grigio_scuro" 
                                                                Runat="server" Width="110px" RepeatDirection="Horizontal" BorderWidth=0 CellPadding=1 CellSpacing=0 
                                                                AutoPostBack="true">
												    	        <asp:ListItem Value="In" id="rbIn" Selected="True"></asp:ListItem>
														        <asp:ListItem Value="Out" id="rbOut"></asp:ListItem>
														        <asp:ListItem Value="Own" id="rbOwn"></asp:ListItem>
													        </asp:radiobuttonlist></td>
												        </tr>
												    </table>
												</td>
												<td align="right" valign=middle>
                                                    <uc6:DocumentConsolidation id="documentConsolidationCtrl" runat="server"></uc6:DocumentConsolidation>
												    <asp:checkbox id="chkPrivato"  BorderWidth=0 Runat="server" CssClass="testo_grigio_scuro" Text="Privato" TextAlign="Left" Checked="False" tooltip="Documento creato con visibilità limitata al solo ruolo e utente proprietario"></asp:checkbox>
												</td>
												<td class="titolo_scheda" valign="middle" align="right">
													<table id="Table7" border="0" width="100%">
														<tr>														    														   
														    <td class="testo_grigio">
														        <cc1:imagebutton ImageAlign="Middle" id="btn_storiaCons" Height="16" ImageUrl="../images/proto/conservazione_d.gif"
																	Runat="server" AlternateText="Visualizza storia conservazione documento" Width="18" Visible="false"></cc1:imagebutton>
                                                                <cc1:imagebutton ImageAlign="Middle" id="btn_inoltra" Height="16" ImageUrl="../images/proto/inoltra.gif"
																	Runat="server" AlternateText="Inoltra il documento"></cc1:imagebutton>													
														        
														        <cc1:imagebutton ImageAlign="Middle" id="btn_aggiungi_P" Height="16" ImageUrl="../images/proto/ins_area.gif"
																	Runat="server" AlternateText="Inserisci documento in area di lavoro"  DisabledUrl="../images/proto/ins_area.gif" Tipologia="DO_ADD_ADL"></cc1:imagebutton>													
														        
														        <cc1:imagebutton id="btn_log" Runat="server" AlternateText="Mostra Storia Documento" ImageAlign="Middle"
																	DisabledUrl="../images/proto/storia.gif" Height="17px"
																	ImageUrl="../images/proto/storia.gif"></cc1:imagebutton>												
															    <cc1:imagebutton id="btn_notifica" Runat="server" AlternateText="Ricevute Spedizione" DisabledUrl="../images/proto/notificheEmail.gif" ImageAlign="Middle"
																	Tipologia="" Height="17" ImageUrl="../images/proto/notificheEmail.gif" Visible="false"></cc1:imagebutton>
															    <cc1:imagebutton id="btn_modificaOgget_P" Runat="server" AlternateText="Modifica" DisabledUrl="../images/proto/matita.gif" ImageAlign="Middle"
																	Tipologia="DO_PROT_SALVA" Height="17" ImageUrl="../images/proto/matita.gif"></cc1:imagebutton>
															    <cc1:imagebutton id="btn_visibilita" Runat="server" AlternateText="Mostra visibilità" ImageAlign="Middle"
																	DisabledUrl="../images/proto/ico_visibilita2.gif" Tipologia="DO_PRO_VISIBILITA" Height="17px"
																	ImageUrl="../images/proto/ico_visibilita2.gif"></cc1:imagebutton></td>
														
												<td>
                                                <cc1:imagebutton id="img_busta_pec" Runat="server" AlternateText="Documento spedito tramite la posta" ImageAlign="Middle"
																	 DisabledUrl="../images/proto/busta_email.jpg"  Visible = "false"
																	 Height="16px" Width="17px"    ImageUrl="../images/proto/bustaCertificata_small.JPG"></cc1:imagebutton>
                                                </td>
                                                </tr>
                                                	</table>
	    
                                                </td>
											</tr>
										</table>
									</td>
								</tr>
								<tr><!-- Segnatura -->
									<td>
										<table class="info_grigio" id="tbl_segnatura" cellSpacing="0" cellPadding="0" width="95%" align="center" border="0">
											<tr>
												<td class="titolo_scheda" vAlign="middle">&nbsp;&nbsp;Segnatura</td>
												<td vAlign="top" align="right">
													<table id="Table3">
														<tr>
                                                            <td><asp:textbox id="txt_num_stampe" CssClass="testo_grigio" Runat="server" MaxLength="3" Width="30px" Text="1"></asp:textbox></td>
															<td><cc1:imagebutton id="btn_stampaSegn_P" Runat="server" Width="18" AlternateText="Stampa etichetta"
																	DisabledUrl="../images/proto/stampa.gif" Tipologia="DO_PROT_SE_STAMPA" ImageUrl="../images/proto/stampa.gif"></cc1:imagebutton></td>
															<td><cc1:imagebutton id="btn_StampaVoidLabel" Runat="server" Width="18" AlternateText="Stampa Segnatura A4"
																	 DisabledUrl="../images/proto/stampaA4.gif" ImageUrl="../images/proto/stampaA4.gif" Enabled="False" ImageAlign="AbsMiddle"  OnClientClick="OpenModalLabel();"></cc1:imagebutton></td>
															<td><cc1:imageButton id="btn_stampa_ricevuta" runat="server" Width="18" 
                                                                    AlternateText="Stampa ricevuta" ImageAlign=AbsMiddle 
                                                                    ImageUrl="../images/proto/ricevuta.gif" 
                                                                    DisabledUrl="../images/proto/ricevuta.gif" 
                                                                    onclick="btn_stampa_ricevuta_Click" /></td>
                                                                    								<td>
																	<cc1:ImageButton ID="btn_invio_mail" Runat="server" Width="18" Height="17" AlternateText="Invia Ricevuta" ImageUrl="../images/proto/invioRicevuta.jpg"  DisabledUrl="../images/proto/invioRicevuta.jpg" Enabled="false"/>
																	</td>
														</tr>
													</table>
												</td>
											</tr>
											<tr>
												<td colSpan="2" height="20" valign="top">&nbsp;
												    <asp:textbox id="txt_dataSegn" CssClass="testo_grigio" Runat="server" MaxLength="16" BackColor="#D9D9D9" ReadOnly="True" width="30%"></asp:textbox>
													<asp:textbox id="lbl_segnatura" CssClass="testo_segnatura" Runat="server" ReadOnly="True" width="64%"></asp:textbox>
													<asp:Panel id="pnlZoom" runat="server" BorderColor="Black" BorderStyle="Solid" BorderWidth="2px" BackColor="#d9d9d9">
														    <span style="font-size: 22pt; 
														        font-family: Verdana; text-align:center;" id="lblZoom"></span>
													</asp:Panel>  
                                                    <cc3:ModalPopupExtender ID="mpeZoom" RepositionMode="None" runat="server" DynamicServicePath="" 
                                                        Enabled="True" TargetControlID="pnlZoom" PopupControlID="pnlZoom">
                                                    </cc3:ModalPopupExtender>
                                                        
												</td>
											</tr>
										</table>
									</td>
								</tr>
								<tr><td height="2"></td></tr>
								<asp:Panel ID="pnl_protocolloTitolario" runat="server" Visible="false">
							        <tr>
							            <td>
							    	        <!-- INIZIO TABELLA PROTOCOLLO TITOLARIO-->
								            <TABLE class="info_grigio" id="TABLE9" cellSpacing="0" cellPadding="0" width="95%" align="center" border="0">
										        <TR>
											        <TD style="padding-left:8px;">
											            <asp:Label ID="lbl_etProtTitolario" runat="server" CssClass="titolo_scheda" Width="20%"></asp:Label>
											            <asp:Label ID="lbl_txtProtTitolario" runat="server" CssClass="testo_grigio" Width="70%"></asp:Label>
											        </td>
										        </TR>										
									        </TABLE>
							            </td>
							        </tr>
							        <tr><td height="2"></td></tr>
							    </asp:Panel>
								<tr>
									<td>
										<!-- Oggettario -->
										<table class="info_grigio" id="tbl_oggettario" cellSpacing="0" cellPadding="0" width="95%" align="center" border="0">
											<tr>
												<td class="titolo_scheda" vAlign="middle">&nbsp;&nbsp;Oggetto&nbsp;*</td>
												<td valign="middle" align="right">
													<table id="Table4">
														<tr>
														    <!-- Aggiunta del bottone per il correttore ortografico -->
														    <td><cc1:imagebutton ID="btn_Correttore" runat="server" Width="19px" AlternateText="Correttore ortografico"
																	DisabledUrl="../images/proto/check_spell.gif"  Height="17" Visible="false"
																	ImageUrl="../images/proto/check_spell.gif" OnClick="btn_Correttore_Click" Enabled="false" ></cc1:imagebutton></td>
														    <!-- fine aggiunta -->
														    <td><cc1:imagebutton ID="imgDescOgg" runat="server" Width="19px" AlternateText="Descrizione campo oggetto"
																	DisabledUrl="../images/rubrica/l_exp_o_grigia.gif"  Height="17"
																	ImageUrl="../images/rubrica/l_exp_o_grigia.gif" OnClick="imgDescOgg_Click" Enabled=false></cc1:imagebutton></td>
															<TD><cc1:imagebutton id="btn_RubrOgget_P" Runat="server" Width="19px" AlternateText="Seleziona un  oggetto nell'oggettario"
																	DisabledUrl="../images/proto/ico_oggettario.gif" Tipologia="DO_PROT_OG_OGGETTARIO" Height="17"
																	ImageUrl="../images/proto/ico_oggettario.gif"></cc1:imagebutton></TD>
															
															<TD><cc1:imagebutton id="btn_storiaOgg_P" Runat="server" Width="18" AlternateText="Storia" DisabledUrl="../images/proto/storia.gif"
																	Tipologia="DO_PROT_OG_STORIA" Height="17" ImageUrl="../images/proto/storia.gif"></cc1:imagebutton></TD>
														</tr>
													</table>
												</td>
											</tr>
											<tr>
												<td colspan="2"><uc4:Oggetto ID="ctrl_oggetto" runat="server" /></td>
											</tr>
										</table>
									</td>
								</tr>
								<tr><td height="2"></td></tr>
								<TR>
									<TD>
										<table class="info_grigio" id="tbl_mittente1" cellSpacing="0" cellPadding="0" width="95%"
											align="center" border="0">
											<TR>
												<TD class="titolo_scheda" vAlign="middle"><asp:panel id="pnl_star_YES" Runat="server" Visible="True">&nbsp;&nbsp;Mittente *</asp:panel><asp:panel id="pnl_star_NO" Runat="server" Visible="False">&nbsp;&nbsp;Mittente</asp:panel></TD>
												<TD vAlign="middle" align="right">
													<table id="Table5">
														<TR>
															<TD><asp:panel id="pnl_mit_sempl" Runat="server" Visible="False"><IMG class="testo_grigio" id="btn_rubrica_p_sempl" alt="Seleziona mittente nella rubrica"
																		src="../images/proto/rubrica.gif" width="29" runat="server">
																</asp:panel><asp:panel id="pnl_mit" Runat="server" Visible="True"><IMG 
                        class=testo_grigio id=btn_rubrica_p 
                        onmouseover="verificaChangeCursorT('hand','btn_rubrica_p','<%=this.btn_rubrica_p_state%>')" 
                        alt="Seleziona mittente nella rubrica" 
                        src="../images/proto/rubrica.gif" width=29 
                        runat="server">
																</asp:panel></TD>
															<TD><cc1:imagebutton id="btn_DetMit_P" Runat="server" Width="18" AlternateText="Dettagli" DisabledUrl="../images/proto/dettaglio.gif"
																	Tipologia="DO_IN_MIT_DETTAGLI" Height="16" ImageUrl="../images/proto/dettaglio.gif"></cc1:imagebutton></TD>
															<td><cc1:imagebutton id="btn_StoriaMitt" Runat="server" Width="18" AlternateText="Storia" DisabledUrl="../images/proto/storia.gif"
																	Tipologia="DO_PROT_OG_STORIA" Height="17" ImageUrl="../images/proto/storia.gif"></cc1:imagebutton></td>
									<%--								<td>
																	<cc1:ImageButton ID="btn_invio_mail" Runat="server" Width="18" Height="17" AlternateText="Invia Ricevuta" ImageUrl="../images/proto/invioRicevuta.jpg"  DisabledUrl="../images/proto/invioRicevuta.jpg" Enabled="false"/>
																	</td>--%>
														</TR>
													</table>
												</TD>
											</TR>
                                            <ut8:RicercaVeloce id="rubrica_veloce" runat="server" Visible="false" OnEnabledChanged = "RubricaVeloce_OnEnabledChanged"></ut8:RicercaVeloce>
											<TR>
												<TD colSpan="2" height="24">&nbsp;
													<asp:textbox id="txt_CodMit_P" CssClass="testo_grigio" Runat="server" Width="75px" AutoPostBack="True"></asp:textbox>
													<asp:textbox id="txt_DescMit_P"  CssClass="testo_grigio" Runat="server" Width="287px" MaxLength="128"></asp:textbox>
                                                    <asp:HiddenField ID="hiddenIdCodMit_p" runat="server" />
                                                    <asp:HiddenField ID="hiddenMitt" runat="server" Value="0" />
                                                    <asp:Panel runat="server" ID="pnl_mittente_veloce" Visible="false">
                                                    <script type="text/javascript">
                                                        function acePopulated(sender, e) {
                                                            var behavior = $find('AutoCompleteExIngressoBIS');
                                                            var target = behavior.get_completionList();
                                                            if (behavior._currentPrefix != null) {
                                                                var prefix = behavior._currentPrefix.toLowerCase();
                                                                var i;
                                                                for (i = 0; i < target.childNodes.length; i++) {
                                                                    var sValue = target.childNodes[i].innerHTML.toLowerCase();
                                                                    if (sValue.indexOf(prefix) != -1) {
                                                                        var fstr = target.childNodes[i].innerHTML.substring(0, sValue.indexOf(prefix));

                                                                        var pstr = target.childNodes[i].innerHTML.substring(fstr.length, fstr.length + prefix.length);

                                                                        var estr = target.childNodes[i].innerHTML.substring(fstr.length + prefix.length, target.childNodes[i].innerHTML.length);

                                                                        target.childNodes[i].innerHTML = fstr + '<span class="selectedWord">' + pstr + '</span>' + estr;
                                                                    }
                                                                }
                                                            }
                                                        }

                                                        function aceSelected(sender, e) {
                                                            var value = e.get_value();
                                                            if (!value) {

                                                                if (e._item.parentElement && e._item.parentElement.tagName == "LI")

                                                                    value = e._item.parentElement.attributes["_value"].value;

                                                                else if (e._item.parentElement && e._item.parentElement.parentElement.tagName == "LI")

                                                                    value = e._item.parentElement.parentElement.attributes["_value"].value;

                                                                else if (e._item.parentNode && e._item.parentNode.tagName == "LI")

                                                                    value = e._item.parentNode._value;

                                                                else if (e._item.parentNode && e._item.parentNode.parentNode.tagName == "LI")

                                                                    value = e._item.parentNode.parentNode._value;

                                                                else value = "";

                                                            }
                                                            var searchText = $get('<%=txt_DescMit_P.ClientID %>').value;
                                                            searchText = searchText.replace('null', '');
                                                            var testo = value;
                                                            var indiceFineCodice = testo.lastIndexOf(')');
                                                            document.getElementById("<%=this.txt_DescMit_P.ClientID%>").focus();
                                                            document.getElementById("<%=this.txt_DescMit_P.ClientID%>").value = "";
                                                            var indiceDescrizione = testo.lastIndexOf('(');
                                                            var descrizione = testo.substr(0, indiceDescrizione - 1);
                                                            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
                                                            document.getElementById('txt_CodMit_P').value = codice;
                                                            document.getElementById('txt_DescMit_P').value = descrizione;
                                                            if (testo.indexOf('[RC]') > 0) {
                                                                document.getElementById('hiddenMitt').value = '2';
                                                            }
                                                            else {
                                                                var inizioCodiceRubrica = testo.lastIndexOf('[') + 1;
                                                                var fineCodiceRubrica = testo.lastIndexOf(']') - 2;
                                                                var codiceRubrica = testo.substr(inizioCodiceRubrica, fineCodiceRubrica - inizioCodiceRubrica + 2);
                                                                document.getElementById('hiddenMitt').value = codiceRubrica;
                                                            }
                                                            setTimeout('__doPostBack(\'txt_CodMit_P\',\'\')', 0);

                                                        } 


                                                      </script>
                                                        <cc8:AutoCompleteExtender runat="server" ID="mittente_veloce" TargetControlID="txt_DescMit_P"
                                                            CompletionListCssClass="autocomplete_completionListElementbis" CompletionListItemCssClass="single_itembis"
                                                            CompletionListHighlightedItemCssClass="single_item_hoverbis" ServiceMethod="GetListaCorrispondentiVeloce"
                                                            MinimumPrefixLength="2" CompletionInterval="500" EnableCaching="true" CompletionSetCount="20"
                                                            DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                            UseContextKey="true" OnClientItemSelected="aceSelected" BehaviorID="AutoCompleteExIngressoBIS" 
                                                            OnClientPopulated="acePopulated ">
                                                         </cc8:AutoCompleteExtender>
                                                </asp:Panel>
                                                  </TD>
											</TR>
                                            
										</table>
									</TD>
								</TR>
								<!--Mittenti Multipli-->
                                <asp:Panel id="panel_DettaglioMittenti" Runat="server" Visible="false">
                                <tr>
                                    <td  height="2" align="center">
                                    </td>
                                </tr>
                                <tr>
									<td>
										<table class="info_grigio" id="Table11" cellSpacing="0" cellPadding="0" width="95%" align="center" border="0">
										<TR>
                                                <TD class="titolo_scheda" vAlign="middle">&nbsp;&nbsp;Mittenti Multipli</TD>
                                                <td>
                                                    <cc1:ImageButton id="btn_downMittente" runat="server" AlternateText="Inserisci Mittente multiplo" DisabledUrl="../images/proto/ico_freccia_giu.gif" ImageUrl="../images/proto/ico_freccia_giu.gif"></cc1:ImageButton>
                                                    &nbsp;
									                <cc1:ImageButton id="btn_upMittente" runat="server" AlternateText="Inserisci Mittente" DisabledUrl="../images/proto/ico_freccia_su.gif" ImageUrl="../images/proto/ico_freccia_su.gif"></cc1:ImageButton>
                                                </td>
												<TD vAlign="middle" align="right">
                                                    <table>
                                                        <tr>
                                                        <td><IMG id="btn_RubrMittMultiplo" height="16" alt="Seleziona mittente nella rubrica" src="../images/proto/rubrica.gif" width="29" runat="server"></td>
                                                        <%--<td><cc1:ImageButton id="btn_insMittMultiplo" runat="server" AlternateText="Inserisci mittente" DisabledUrl="../images/proto/aggiungi.gif" ImageUrl="../images/proto/aggiungi.gif"></cc1:ImageButton></td>--%>
                                                        <td><cc1:imagebutton id="btn_StoriaMittMultipli" Runat="server" Width="18" AlternateText="Storia" DisabledUrl="../images/proto/storia.gif" Height="17" ImageUrl="../images/proto/storia.gif"></cc1:imagebutton></td>
                                                        <td><cc1:imagebutton id="btn_dettMittMultipli" Runat="server" Width="18" AlternateText="Dettagli" DisabledUrl="../images/proto/dettaglio.gif" Height="16" ImageUrl="../images/proto/dettaglio.gif"></cc1:imagebutton></td>                                                                                                                
                                                        <td><cc1:imagebutton id="btn_CancMittMultiplo" Width="19" Runat="server" AlternateText="Cancella Mittente" DisabledUrl="../images/proto/cancella.gif" Height="17px" ImageUrl="../images/proto/cancella.gif"></cc1:imagebutton></td>
                                                        </tr>
                                                    </table>
                                                </TD>                                                
                                        </TR>
                                        <ut8:RicercaVeloce id="rubrica_veloce_mitt_multi" runat="server" Visible="false" OnEnabledChanged = "RubricaVeloce_OnEnabledChanged"></ut8:RicercaVeloce>
                                        <asp:HiddenField ID="txt_cod_mitt_mult_nascosto" runat="server"/>
                                        <asp:HiddenField ID="txt_desc_mitt_mult_nascosto" runat="server" />
                                        <asp:Button id="btn_nascosto_mitt_multipli" runat="server" style="display: none;"/>
                                        <%--<tr>
                                            <TD colSpan="3" height="25">&nbsp;
									            <asp:textbox id="txt_codMittMultiplo" AutoPostBack="True" Width="75px" Runat="server" CssClass="testo_grigio"></asp:textbox>
									            <asp:textbox id="txt_descMittMultiplo" AutoComplete="off" Width="287px" Runat="server" CssClass="testo_grigio"></asp:textbox>
                                            </TD>                                       
                                        </tr>
                                        <tr>
                                            <td vAlign="middle" colSpan="3" align="right">
                                                <table>
                                                        <tr>
                                                        <td><cc1:imagebutton id="btn_dettMittMultipli" Runat="server" Width="18" AlternateText="Dettagli" DisabledUrl="../images/proto/dettaglio.gif" Height="16" ImageUrl="../images/proto/dettaglio.gif"></cc1:imagebutton></td>                                                                                                                
                                                        <td><cc1:imagebutton id="btn_CancMittMultiplo" Width="19" Runat="server" AlternateText="Cancella Mittente" DisabledUrl="../images/proto/cancella.gif" Height="17px" ImageUrl="../images/proto/cancella.gif"></cc1:imagebutton></td>
                                                        </tr>
                                                </table>
                                            </td>
                                        </tr>--%>
                                        <tr>
                                            <td vAlign="top" colSpan="3" height="35">&nbsp;
                                                <asp:ListBox id="lbx_mittMultiplo" runat="server" CssClass="testo_grigio" width="365px" Rows="2"></asp:ListBox>
                                            </td>
                                        </tr>                                        
                                        </table>
                                    </td>
                                </tr>                                
                                </asp:Panel>
                                <asp:Panel ID="pnl_mezzoSpedizione" runat="server" Visible="false">
								<tr><td height="2"></td></tr>
								<!-- inizio tabella per mezzo di spedizione -->
								<tr>
								    <td>
										<table class="info_grigio" id="Table8" cellSpacing="0" cellPadding="0" width="95%" align="center" border="0">
										    <tr><td height="2px" colspan="3"></td></tr>
											<tr>
											    <td class="titolo_scheda">&nbsp;&nbsp;Mezzo di spedizione</td>
											    <td class="titolo_scheda"><asp:Label ID="lbl_mezzoSpedizione" runat="server" Visible="false"></asp:Label></td>
											    <td align="left" class="titolo_scheda"><asp:DropDownList ID="ddl_spedizione" runat="server" AutoPostBack="true" CssClass="testo_grigio"></asp:DropDownList></td>
											</tr>
										    <tr><td height="2px" colspan="3"></td></tr>
										</table>
								    </td>
								</tr>
								</asp:Panel>
                                <!-- mittente intermedio-->
								<asp:panel id="panel_Mit" runat="server" Visible="True">
								<tr><td height="2"></td></tr>
								<tr>
									<td>
										<table class="info_grigio" id="tbl_mittente" cellSpacing="0" cellPadding="0" width="95%" align="center" border="0">
											<asp:Panel id="panel_mittInt" Runat="server" Visible="false">
											<TR>
                                                <TD class="titolo_scheda" vAlign="middle">&nbsp;&nbsp;Mittente intermedio</TD>
												<TD vAlign="middle" align="right">
													<TABLE id="Table6">
														<TR>
															<TD>
																<asp:Panel id="pnl_mit_int_semplice" Runat="server" Visible="False">
																	<IMG id="btn_RubrMitInt_Sempl" height="16" alt="Seleziona mittente intermedio nella rubrica"
																		src="../images/proto/rubrica.gif" width="29" runat="server">
																</asp:Panel>
																<asp:Panel id="pnl_mit_int" Runat="server" Visible="True">
																	<IMG id="btn_RubrMitInt" height="16" alt="Seleziona mittente intermedio nella rubrica"
																		src="../images/proto/rubrica.gif" width="29" runat="server">
																</asp:Panel>
															</TD>
															<TD>
																<cc1:imagebutton id="btn_DetMitInt_P" Width="18" Runat="server" AlternateText="Dettagli" DisabledUrl="../images/proto/dettaglio.gif"
																	Tipologia="DO_IN_MII_DETTAGLI" Height="16" ImageUrl="../images/proto/dettaglio.gif"></cc1:imagebutton>
															</TD> 
																
                                                        </TR>
													</TABLE>
												</TD>
											</TR>
											<tr>
												<TD colSpan="2" height="25">&nbsp;
													<asp:textbox id="txt_CodMitInt_P" AutoPostBack="True" Width="75px" Runat="server" CssClass="testo_grigio"></asp:textbox>
													<asp:textbox id="txt_DescMitInt_P" AutoComplete="off" Width="287px" Runat="server" CssClass="testo_grigio"></asp:textbox></TD>
											</tr>
											</asp:Panel>                                            
                                            <tr>
											    <td colspan="2">
											        <table border="0" width="100%">
											            <tr>
											                <td colspan="3" height="1"></td>
											                <td height="24" width="110px" rowspan="2">
											                    <table class="info_grigio">
											                        <tr>
											                            <td class="titolo_scheda" height="24" width="100px">Data arrivo<uc3:Calendario id="txt_DataArrivo_P" runat="server"  /></td>
											                        </tr>
                                                                    <asp:Panel ID="OraPervenuto" Runat="server" Visible="false">
                                                                    <tr>
                                                                        <td  class="titolo_scheda" height="24" width="100px">Ora arrivo<br /><cc1:TimeMask id="txt_OraPervenuto_P" runat="server" Width="75px" CssClass="testo_grigio"></cc1:TimeMask>&nbsp;&nbsp;<cc1:imagebutton id="btn_storiaData" Runat="server" Width="18" AlternateText="Storia" DisabledUrl="../images/proto/storia.gif"
																	Tipologia="DO_PROT_OG_STORIA" Height="17" ImageUrl="../images/proto/storia.gif"></cc1:imagebutton></td>
                                                                    </tr>
                                                                    </asp:Panel>
											                    </table>
											                </td>
											            </tr>
											            <tr>
											                <td class="titolo_scheda" height="24" width="120px">&nbsp;Protocollo mittente<br />&nbsp;&nbsp;<asp:textbox id="txt_NumProtMit_P" Width="110px" Runat="server" CssClass="testo_grigio" MaxLength="128"></asp:textbox></td>
											                <td class="titolo_scheda" height="24" width="100px">&nbsp;in data<uc3:Calendario id="txt_DataProtMit_P" runat="server" /></td>
											                <td class="titolo_scheda" vAlign="middle" width="18px" align="right"><cc1:imagebutton id="btn_verificaPrec_P" Width="18" Runat="server" AlternateText="Verifica Prec"
														DisabledUrl="../images/proto/ico_risposta.gif" Tipologia="DO_IN_PRO_VERIFICAPREC" Height="16"
														ImageUrl="../images/proto/ico_risposta.gif"></cc1:imagebutton>&nbsp;</td>
											            </tr>
											            <asp:Panel id="pnl_riferimentoMittente" runat="server" Visible="false">
											            <tr>
											                <td colspan="4" class="titolo_scheda">&nbsp;Riferimento&nbsp;<asp:TextBox ID="txt_riferimentoMittente" runat="server" CssClass="testo_grigio" Width="80%"></asp:TextBox></td>
											            </tr>
											            </asp:Panel>
											        </table>
											    </td>
											</tr>
										</table>
									</td>
								</tr>
								</asp:panel>
								<asp:panel id="panel_Dest" runat="server" Visible="True">
								<!-- tabella destinatario-->
								<tr><td height="2"></td></tr>
								<TR>
									<TD><!--table cellSpacing="0" cellPadding="0" width="95%" align="center" border="1" bordercolor=red>
										<TR>
											<TD-->
										<TABLE class="info_grigio" id="tbl_destinatario" cellSpacing="0" cellPadding="0" width="95%"
											align="center">
											<TR>
												<TD class="titolo_scheda" vAlign="middle">&nbsp;&nbsp;Destinatario</TD>
												<TD vAlign="middle" align="right">
													<TABLE>
														<TR>
													
															<TD>
																<cc1:imagebutton id="btn_el_spediz" Width="18" Runat="server" AlternateText="Spedizione" DisabledUrl="../images/proto/spedisceEmail.gif.gif"
																	Height="17px" ImageUrl="../images/proto/spedisceEmail.gif" Visible=false></cc1:imagebutton></TD>
															<TD>
																<cc1:imagebutton id="btn_StampaBuste_P" Width="19" Runat="server" AlternateText="Stampa le buste dei destinatari"
																	DisabledUrl="../images/proto/stampa.gif" Height="17px" ImageUrl="../images/proto/stampa.gif"></cc1:imagebutton></TD>
															<TD>
																<asp:Panel id="pnl_rubr_dest_Semplice" Runat="server" Visible="False">
																	<IMG id="btn_RubrDest_Sempl_P" height="19" alt="Seleziona un destinatario nella rubrica"
																		src="../images/proto/rubrica.gif" width="29" runat="server">
																</asp:Panel>
																<asp:Panel id="pnl_rubr_des" Runat="server" Visible="True">
																	<IMG id="btn_RubrDest_P" height="19" alt="Seleziona un destinatario nella rubrica" src="../images/proto/rubrica.gif"
																		width="29" runat="server">
																</asp:Panel></TD> <!-- <TD>
<cc1:imagebutton id=btn_ModDest_P Runat="server" AlternateText="Modifica" ImageUrl="../images/proto/matita.gif" DisabledUrl="../images/proto/matita.gif" Tipologia="DO_OUT_DES_MODIFICA"></cc1:imagebutton></TD>-->
															<TD>
																<cc1:imagebutton id="btn_aggiungiDest_P" Width="18" Runat="server" AlternateText="Aggiungi" DisabledUrl="../images/proto/aggiungi.gif"
																	Tipologia="DO_OUT_DES_PIU" Height="17" ImageUrl="../images/proto/aggiungi.gif"></cc1:imagebutton></TD> <!-- <TD>
<cc1:imagebutton id=btn_storiaDest_P Runat="server" Width="18" AlternateText="Storia" ImageUrl="../images/proto/storia.gif" DisabledUrl="../images/proto/storia.gif" Tipologia="DO_OUT_DES_STORIA" Height="17" Enabled="False"></cc1:imagebutton></TD>-->
															<TD>
																<cc1:imagebutton id="btn_StoriaDest" Width="18" Runat="server" AlternateText="Storia" DisabledUrl="../images/proto/storia.gif"
																	Tipologia="DO_PROT_OG_STORIA" Height="17" ImageUrl="../images/proto/storia.gif"></cc1:imagebutton></TD>
														</TR>
													</TABLE>
												</TD>
											</TR>
                                            <ut8:RicercaVeloce id="rubrica_veloce_destinatario" runat="server" Visible="false" OnEnabledChanged = "RubricaVeloce_OnEnabledChanged"></ut8:RicercaVeloce>
											<TR>
												<TD style="HEIGHT: 19px" colSpan="2">&nbsp;
													<asp:textbox id="txt_CodDest_P" AutoPostBack="True" Width="75px" Runat="server" CssClass="testo_grigio"></asp:textbox>
														<asp:textbox id="txt_DescDest_P" Width="287px" Runat="server" CssClass="testo_grigio" MaxLength="128"></asp:textbox>
                                                    <asp:Panel id="pnl_destinatario_veloce" runat="server" Visible="false">
                                                    <asp:HiddenField ID="hiddenDest" runat="server" Value="0" />
                                                    <script type="text/javascript">
                                                        function ItemSelectedDestinatario(sender, e) {
                                                            var value = e.get_value();

                                                            if (!value) {

                                                                if (e._item.parentElement && e._item.parentElement.tagName == "LI")

                                                                    value = e._item.parentElement.attributes["_value"].value;

                                                                else if (e._item.parentElement && e._item.parentElement.parentElement.tagName == "LI")

                                                                    value = e._item.parentElement.parentElement.attributes["_value"].value;

                                                                else if (e._item.parentNode && e._item.parentNode.tagName == "LI")

                                                                    value = e._item.parentNode._value;

                                                                else if (e._item.parentNode && e._item.parentNode.parentNode.tagName == "LI")

                                                                    value = e._item.parentNode.parentNode._value;

                                                                else value = "";

                                                            }

                                                            var searchText = $get('<%=txt_DescDest_P.ClientID %>').value;
                                                            searchText = searchText.replace('null', '');
                                                            var testo = value;
                                                            var indiceFineCodice = testo.lastIndexOf(')');
                                                            document.getElementById("<%=this.txt_DescDest_P.ClientID%>").focus();
                                                            document.getElementById("<%=this.txt_DescDest_P.ClientID%>").value = "";
                                                            var indiceDescrizione = testo.lastIndexOf('(');
                                                            var descrizione = testo.substr(0, indiceDescrizione - 1);
                                                            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
                                                            document.getElementById('txt_CodDest_P').value = codice;
                                                            document.getElementById('txt_DescDest_P').value = descrizione;
                                                            if (testo.indexOf('[RC]') > 0) {
                                                                document.getElementById('hiddenDest').value = '2';
                                                            }
                                                            else {
                                                                var inizioCodiceRubrica = testo.lastIndexOf('[') + 1;
                                                                var fineCodiceRubrica = testo.lastIndexOf(']') - 2;
                                                                var codiceRubrica = testo.substr(inizioCodiceRubrica, fineCodiceRubrica - inizioCodiceRubrica + 2);
                                                                document.getElementById('hiddenDest').value = codiceRubrica;
                                                            }
                                                            document.all("btn_aggiungiDest_P").click();
                                                        }

                                                        function acePopulatedDest(sender, e) {
                                                            var behavior = $find('AutoCompleteExDestinatari');
                                                            var target = behavior.get_completionList();
                                                            if (behavior._currentPrefix != null) {
                                                                var prefix = behavior._currentPrefix.toLowerCase();
                                                                var i;
                                                                for (i = 0; i < target.childNodes.length; i++) {
                                                                    var sValue = target.childNodes[i].innerHTML.toLowerCase();
                                                                    if (sValue.indexOf(prefix) != -1) {
                                                                        var fstr = target.childNodes[i].innerHTML.substring(0, sValue.indexOf(prefix));

                                                                        var pstr = target.childNodes[i].innerHTML.substring(fstr.length, fstr.length + prefix.length);

                                                                        var estr = target.childNodes[i].innerHTML.substring(fstr.length + prefix.length, target.childNodes[i].innerHTML.length);

                                                                        target.childNodes[i].innerHTML = fstr + '<span class="selectedWord">' + pstr + '</span>' + estr;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        </script>
                                                        <cc8:AutoCompleteExtender runat="server" ID="destinatario_veloce" TargetControlID="txt_DescDest_P"
                                                            CompletionListCssClass="autocomplete_completionListElementbis" CompletionListItemCssClass="single_itembis"
                                                            CompletionListHighlightedItemCssClass="single_item_hoverbis" ServiceMethod="GetListaCorrispondentiVeloce"
                                                            MinimumPrefixLength="2" CompletionInterval="500" EnableCaching="true" CompletionSetCount="20"
                                                            DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                            UseContextKey="true" OnClientItemSelected="ItemSelectedDestinatario" BehaviorID="AutoCompleteExDestinatari" OnClientPopulated="acePopulatedDest">
                                                        </cc8:AutoCompleteExtender>
                                                    </asp:Panel>
                                                    
                                               </TD>
											</TR>
											<TR> <!-- listBox	Destinatari-->
												<TD class="titolo_scheda" vAlign="middle">&nbsp;&nbsp;Destinatari *</TD>
												<TD vAlign="middle" align="right">
													<TABLE>
														<TR>															    
															<TD>
																<cc1:imagebutton id="btn_tipo_sped" Width="18px" Runat="server" AlternateText="Spedizione" DisabledUrl="../images/proto/spedisceEmail.gif"
																	Tipologia="DO_OUT_DES_TIPO_INVIO" Height="17px" ImageUrl="../images/proto/spedisceEmail.gif"></cc1:imagebutton></TD>
															<TD>
																<cc1:imagebutton id="btn_notifica_sped" Width="18px" Runat="server" AlternateText="Ricevute Spedizione"
																	DisabledUrl="../images/proto/notificheEmail.gif" Tipologia="" Height="17px" ImageUrl="../images/proto/notificheEmail.gif"></cc1:imagebutton></TD>
															<td><cc1:imagebutton ID="imgListaDest" runat="server" Width="19px" AlternateText="Destinatari principali"
																DisabledUrl="../images/rubrica/l_exp_o_grigia.gif"  Height="17"
																ImageUrl="../images/rubrica/l_exp_o_grigia.gif" OnClick="imgListaDest_Click" Enabled=false></cc1:imagebutton></td>
															<TD>
																<cc1:imagebutton id="btn_dettDest" Width="18px" Runat="server" AlternateText="Dettagli" DisabledUrl="../images/proto/dettaglio.gif"
																	Tipologia="DO_OUT_DES_DETTAGLI" Height="17px" ImageUrl="../images/proto/dettaglio.gif"></cc1:imagebutton></TD>
															<TD>
																<cc1:imagebutton id="btn_cancDest" Width="19" Runat="server" AlternateText="Cancella" DisabledUrl="../images/proto/cancella.gif"
																	Tipologia="DO_OUT_DES_ELIMINA" Height="17px" ImageUrl="../images/proto/cancella.gif"></cc1:imagebutton></TD>
														</TR>
													</TABLE>
												</TD>
											</TR>
											<TR>
												<TD vAlign="top" colSpan="2">&nbsp;
													<asp:ListBox id="lbx_dest" runat="server" Width="365px" CssClass="testo_grigio" Rows="2"></asp:ListBox></TD>
											</TR>
											<TR>
												<TD width="55%">
													<TABLE cellSpacing="0" cellPadding="0" width="100%">
														<TR>
														
															<TD class="titolo_scheda" vAlign="middle" width="50%">&nbsp;&nbsp;Destinatari CC</TD>
															<TD vAlign="middle" align="right">
																<cc1:ImageButton id="btn_insDestCC" runat="server" AlternateText="Inserisci tra i destinatari per Conoscenza"
																	DisabledUrl="../images/proto/ico_freccia_giu.gif" ImageUrl="../images/proto/ico_freccia_giu.gif"></cc1:ImageButton>&nbsp;
																<cc1:ImageButton id="btn_insDest" runat="server" AlternateText="Inserisci tra i destinatari" DisabledUrl="../images/proto/ico_freccia_su.gif"
																	ImageUrl="../images/proto/ico_freccia_su.gif"></cc1:ImageButton></TD>
														</TR>
													</TABLE>
												</TD>
												<TD vAlign="middle" align="right">
													<TABLE>
														<TR>
															<TD>
																<cc1:imagebutton id="btn_tipo_spedCC" Width="18px" Runat="server" AlternateText="Spedizione" Visible="false"
																	DisabledUrl="../images/proto/spedisceEmail.gif" Tipologia="DO_OUT_DES_TIPO_INVIO" Height="17px" ImageUrl="../images/proto/spedisceEmail.gif"></cc1:imagebutton></TD>
															<TD>
																<cc1:imagebutton id="btn_notifica_sped_CC" Width="18px" Runat="server" AlternateText="Ricevute Spedizione"
																	DisabledUrl="../images/proto/notificheEmail.gif" Tipologia="" Height="17px" ImageUrl="../images/proto/notificheEmail.gif"></cc1:imagebutton></TD>
															<td><cc1:imagebutton ID="imgListaDestCC" runat="server" Width="19px" AlternateText="Destinatari in conoscenza"
																DisabledUrl="../images/rubrica/l_exp_o_grigia.gif"  Height="17"
																ImageUrl="../images/rubrica/l_exp_o_grigia.gif" OnClick="imgListaDestCC_Click" Enabled=false></cc1:imagebutton></td>
															<TD>
																<cc1:imagebutton id="btn_dettDestCC" Width="18px" Runat="server" AlternateText="Dettagli" DisabledUrl="../images/proto/dettaglio.gif"
																	Tipologia="DO_OUT_DES_DETTAGLI" Height="17px" ImageUrl="../images/proto/dettaglio.gif"></cc1:imagebutton></TD>
											    			<TD>
																<cc1:imagebutton id="btn_cancDestCC" Width="19" Runat="server" AlternateText="Cancella" DisabledUrl="../images/proto/cancella.gif" 	Tipologia="DO_OUT_DES_ELIMINA" Height="17px" ImageUrl="../images/proto/cancella.gif"></cc1:imagebutton></TD>
														</TR>
													</TABLE>
												</TD>
											</TR>
											<TR>
												<TD vAlign="top" colSpan="2" height="35">&nbsp;
													<asp:ListBox id="lbx_destCC" runat="server" CssClass="testo_grigio" width="365px" Rows="2"></asp:ListBox></TD>
											</TR>
										</TABLE>
									</TD>
								</TR>
								<TR><TD height="2"></TD></TR>
								<TR>
									<TD>
										<TABLE class="info_grigio" id="tbl_evidenza_new" cellSpacing="0" cellPadding="0" width="95%"
											align="center" border="0">
											<TR>
												<TD width="10"></TD>
												<TD class="titolo_scheda" vAlign="middle">
													<asp:Panel id="pnl_evidenza" Runat="server">
														<asp:CheckBox id="chkEvidenza" Width="136px" Runat="server" CssClass="titolo_scheda" Checked="False"
															TextAlign="Left" Text="In evidenza&#9;"></asp:CheckBox>
													</asp:Panel></TD>
											</TR>
										</TABLE>
									</TD>
								</TR>
								</asp:panel>
								<!-- INIZIO CAMPO NOTE -->
								<tr><td height="2"></td></tr>
								<tr>
									<td>
									    <TABLE id="tbl_note" runat = "server" class="info_grigio" cellSpacing="0" cellPadding="0" width="95%" align="center" border="0">
										    <tr>
											    <TD class="titolo_scheda" vAlign="middle" align = "center" >
											        <uc2:DettaglioNota ID="dettaglioNota" runat="server" width="95%" 
                                                        TipoOggetto = "Documento" Rows="3" TextMode="MultiLine" OnEnabledChanged = "DettaglioNota_OnEnabledChanged" PAGINA_CHIAMANTE="docProtocollo" />
											    </TD>											
										    </tr>
									    </TABLE>
									</td>
								</tr>
								<!-- FINE CAMPO NOTE -->
									 <!-- pannello tipologia documento -->
					            <asp:Panel id="Panel_TipoAtto" Runat="server" Visible="false">
								<tr><td height="2"></td></tr>
								<TR>
									<TD>
										<TABLE class="info_grigio" id="tbl_evidenza" cellSpacing="0" cellPadding="0" width="95%"
											align="center" border="0">
											<TR>
												<TD class="titolo_scheda" style="WIDTH: 185px" vAlign="middle" width="185" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
												Tipologia documento</TD>
												<TD style="WIDTH: 63px" vAlign="middle" align="right">&nbsp;
													<cc1:imagebutton id="btn_addTipoAtto" Width="18" Runat="server" AlternateText="Aggiungi" DisabledUrl="../images/proto/aggiungi.gif"
														Height="17" ImageUrl="../images/proto/aggiungi.gif"></cc1:imagebutton></TD>
												<TD class="testo_grigio" style="WIDTH: 46px" height="25"><IMG height="1" src="../images/proto/spaziatore.gif" width="5" border="0">
												</TD>
											</TR>
											<TR>
												<TD class="testo_grigio" style="WIDTH: 263px" colSpan="3" height="25"><IMG height="1" src="../images/proto/spaziatore.gif" width="5" border="0">&nbsp;</TD>
												<TD style="WIDTH: 185px" vAlign="middle" align="center">&nbsp;
													<asp:checkbox id="chkEvidenza1" Runat="server" CssClass="titolo_scheda" Checked="False" TextAlign="Left"
														Text="In evidenza&#9;"></asp:checkbox></TD>
											</TR>
										</TABLE>
									</TD>
								</TR>
								</asp:Panel>		
					            <asp:panel id="Panel_ProfilazioneDinamica" Runat="server" Visible="true">
								<tr><td height="2"></td></tr>
								<tr>
									<td><!-- INIZIO TABELLA TIPO_ATTO -->
										<table class="info_grigio" id="tbl_tipoAtto" cellSpacing="0" cellPadding="0" width="95%" align="center" border="0">
											<tr>
												<td class="titolo_scheda" style="HEIGHT: 20px" valign="middle" height="20">
												    <img height="1" src="../images/proto/spaziatore.gif" width="8" border="0" />
												    <asp:Label ID="lbl_atto" runat="server" CssClass="titolo_scheda"></asp:Label>
												</td>
												<td style="HEIGHT: 20px" vAlign="middle" align="right"></td>
												<td style="HEIGHT: 20px"><img height="1" src="../images/proto/spaziatore.gif" width="8" border="0"></td>
											</tr>
											<tr>
												<td class="testo_grigio" align="left" height="25"><IMG height="1" src="../images/proto/spaziatore.gif" width="5" border="0">
												<!-- <div class="holder"> -->
													<asp:dropdownlist id="ddl_tipoAtto" runat="server" AutoPostBack="True" Width="336px" CssClass="testo_grigio"></asp:dropdownlist>
												<!-- </div> -->
												</td>
												<td valign="middle" align="right">
                                                <cc1:ImageButton class="ImgHand" ID="btn_delTipologyDoc" runat="server" AlternateText="Elimina la tipologia del documento"
                                                        DisabledUrl="../images/scan/rimuovi.gif" Tipologia="ELIMINA_TIPOLOGIA_DOC"
                                                        ImageUrl="../images/scan/rimuovi.gif" ToolTip="Elimina la tipologia del documento" Visible="false" style="text-align:right"/>
													<asp:ImageButton id="btn_CampiPersonalizzati" runat="server" ImageUrl="../images/proto/ico_oggettario.gif"></asp:ImageButton></td>
												<td><img height="1" src="../images/proto/spaziatore.gif" width="8" border="0" /></td>
											</tr>
										</table>
									</td>
								</tr>
								</asp:panel>
                                <asp:panel id="rispProtoPanelUscita" Runat="server" Visible="true">
								<tr><td height="2"></td></tr>
								<TR>
									<TD><!-- risposta al protocollo -->
										<TABLE class="info_grigio" id="tbl_rispostaprotoUscita" cellSpacing="0" cellPadding="0"
											width="95%" align="center" border="0">
											<TR>
												<TD class="titolo_scheda" style="HEIGHT: 23px" vAlign="middle" align="center" colSpan="2">
													<TABLE style="WIDTH: 136px; HEIGHT: 30px" align="left">
														<TR>
															<TD class="titolo_scheda">&nbsp;&nbsp;Risposta al protocollo</TD>
														</TR>
													</TABLE>
													<TABLE class="titolo_scheda" align="right">
														<TR>
															<TD align="right" width="26" height="17">
																<cc1:imagebutton id="btn_risp_sx" runat="server" DisabledUrl="../images/proto/RispProtocollo/freccina_sx.gif"
																	ImageUrl="../images/proto/RispProtocollo/freccina_sx.gif"></cc1:imagebutton><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
															<TD>
																<cc1:imagebutton id="btn_in_rispota_a" Width="26px" Runat="server" AlternateText="Ricerca i documenti a cui poter rispondere"
																	DisabledUrl="../images/proto/RispProtocollo/lentePreview.gif" ImageUrl="../images/proto/RispProtocollo/lentePreview.gif"></cc1:imagebutton>
																<cc1:imagebutton id="btn_Risp" Runat="server" AlternateText="Crea protocollo in risposta" DisabledUrl="../images/proto/RispProtocollo/catena.gif"
																	ImageUrl="../images/proto/RispProtocollo/catena.gif"></cc1:imagebutton>
                                                                    	<cc1:imagebutton id="btn_risp_grigio" Runat="server" AlternateText="Crea documento non protocollato in risposta" DisabledUrl="../images/proto/RispProtocollo/catena_grigio.gif"
																	ImageUrl="../images/proto/RispProtocollo/catena_grigio.gif" Visible="false"></cc1:imagebutton>    
                                                                    </TD>
															<TD align="center" width="25" height="17">
																<cc1:imagebutton id="btn_risp_dx" runat="server" AlternateText=" visualizza elenco documenti in risposta "
																	DisabledUrl="../images/proto/RispProtocollo/freccina_dx.gif" ImageUrl="../images/proto/RispProtocollo/freccina_dx.gif"></cc1:imagebutton></TD>
														</TR>
													</TABLE>
												</TD>
											</TR>
											<asp:panel id="pnl_text_risposta" Runat="server" Visible="False">
											<tr>
												<TD align="center">
													<asp:textbox id="txt_RispProtSegn_P" Width="367px" Runat="server" CssClass="testo_grigio" ReadOnly="True"></asp:textbox></TD>
											</tr>
											</asp:panel>
										</TABLE>
									</TD>
								</TR>
								</asp:panel>
                                
                  <!-- pannello UFFICIO REFERENTE -->
                                <asp:Panel ID="pnl_ufficioRef" runat="server" Visible="False">
 								<tr><td height="2"></td></tr>
                                <tr>
                                    <td>
                                       <table class="info_grigio" id="tbl_ufficioRef" cellspacing="0" cellpadding="0" width="95%"
												align="center" border="0">
											<TR>
												<TD vAlign="middle" colSpan="2">
													<TABLE align="left">
														<TR>
															<TD class="titolo_scheda" style="WIDTH: 351px">&nbsp;&nbsp;Ufficio Referente *</TD>
															<TD align="right">
																<TABLE id="tbl_ref">
																	<TR>
																		<TD>
																			<cc1:imagebutton id="btn_Rubrica_ref" Width="29" Runat="server" AlternateText="Seleziona una Uo dalla rubrica"
																				DisabledUrl="../images/proto/rubrica.gif" Height="19" ImageUrl="../images/proto/rubrica.gif"
																				Enabled="False" visible="false"></cc1:imagebutton></TD>
																	</TR>
																</TABLE>
															</TD>
														</TR>
													</TABLE>
												</TD>
											</TR>
											<TR>
												<TD colSpan="2" height="25">&nbsp;
													<asp:textbox id="txt_cod_uffRef" AutoPostBack="True" Width="75px" Runat="server" CssClass="testo_grigio"
														ReadOnly="True" BackColor="WhiteSmoke"></asp:textbox>
													<asp:textbox id="txt_desc_uffRef" Width="286px" Runat="server" CssClass="testo_grigio" ReadOnly="True"
														BackColor="WhiteSmoke" MaxLength="128"></asp:textbox></TD>
											</TR>
									    </TABLE>
									</TD>
								</TR>
								</asp:panel>
								
										<asp:panel id="panel_Annul" runat="server" Visible="False">
                                        <tr><td height="2"></td></tr>
								<tr>
									<td>
										<!-- Annullamento	-->
										<TABLE class="info_grigio" id="tbl_annullamento" cellSpacing="0" cellPadding="0" width="95%"
											align="center" border="0">
											<TR>
												<TD class="titolo_scheda_red" vAlign="middle">&nbsp;&nbsp;Annullato</TD>
												<TD vAlign="middle" align="right">&nbsp;</TD>
											</TR>
											<TR>
												<TD colSpan="2" height="25">&nbsp;
													<asp:textbox id="txt_dataAnnul_P" Width="75px" Runat="server" CssClass="testo_red" ReadOnly="True"></asp:textbox>
													<asp:textbox id="txt_numAnnul_P" Width="287px" Runat="server" CssClass="testo_red" ReadOnly="true"></asp:textbox></TD>
											</TR>
										</TABLE>	
                                        </td>
								</tr>
										</asp:panel>
								
								<tr><td height="2"></td></tr>
								<TR>
									<TD><!-- Fascicolazione / Trasmissione Rapida -->
										<table class="info_grigio" id="tbl_fasc_rapida" cellSpacing="0" cellPadding="0" width="95%" align="center" border="0">
											<asp:panel id="pnl_fasc_rapida" Runat="server" Visible="True">
												<tr>
													<td class="titolo_scheda" valign="top">&nbsp;&nbsp;<asp:label id="labelFascRapid" runat="server" width="80%"></asp:label></td>
													<td align="right">														
														<cc1:imagebutton id="btn_titolario" Runat="server" ImageUrl="../images/proto/ico_titolario_noattivo.gif" AlternateText="Titolario" Tipologia="DO_CLA_TITOLARIO" DisabledUrl="../images/proto/ico_titolario_noattivo.gif" OnClick="btn_titolario_Click"></cc1:imagebutton>
														
													
														<cc1:imagebutton class="ImgHand" id="imgFasc" Runat="server" AlternateText="Ricerca Fascicoli" DisabledUrl="../images/proto/ico_fascicolo_noattivo.gif"
															Tipologia="DO_CLA_VIS_PROC" ImageUrl="../images/proto/ico_fascicolo_noattivo.gif"></cc1:imagebutton>
                                                            <cc1:imagebutton Visible="false" class="ImgHand" id="imgNewFasc" Runat="server" AlternateText="Nuovo Fascicolo" DisabledUrl="../images/fasc/fasc_direct.gif"
															Tipologia="DO_CLA_VIS_PROC" ImageUrl="../images/fasc/fasc_direct.gif"></cc1:imagebutton><img height="1" src="../images/proto/spaziatore.gif" width="4" border="0" />
                                                            </td>
                                                           
                                                     
												    </tr>
												<asp:Panel ID="pnl_fasc_Primaria" runat="server" Visible="false" >
                                                    <tr>
                                                        <td colspan="2">
                                                            <table cellpadding="3" border=0><tr><td>
                                                            <asp:Label ID="lbl_fasc_Primaria" runat="server" CssClass="titolo_scheda"></asp:Label>
                                                            </td></tr></table>
                                                        </td>
                                                    </tr>
                                                </asp:Panel>
                                               
												<tr>
													<td colspan="2" height="25" valign="top">&nbsp;
														<asp:textbox id="txt_CodFascicolo" AutoPostBack="True" Width="75px" Runat="server" CssClass="testo_grigio" ReadOnly="False"></asp:textbox>
														<asp:textbox id="txt_DescFascicolo" Width="250px" Runat="server" CssClass="testo_grigio" ReadOnly="True"></asp:textbox></td>
												</tr>
											</asp:panel>
											<asp:panel id="pnl_trasm_rapida" Runat="server" Visible="True">
												<TR>
													<TD class="titolo_scheda" vAlign="top">&nbsp;&nbsp;Trasmissione Rapida</TD>
													<TD vAlign="top" align="right">&nbsp;</TD>
												</TR>
												<TR>
													<TD colSpan="2" height="25" valign="top">
													    <table width="100%" border="0">
													        <tr>
													            <td><asp:TextBox ID="txt_codModello" runat="server" Width="75px" AutoPostBack="true" CssClass="testo_grigio" Visible="false"></asp:TextBox></td>
													            <td><div class="holder"><asp:DropDownList CssClass="testo_grigio" id="ddl_tmpl" tabIndex="420" runat="server" AutoPostBack="True" Width="350px"></asp:DropDownList></div></td>
													        </tr>
													    </table>
													</TD>
												</TR>
											</asp:panel>
										</TABLE>
									</TD>
								</TR>
								<tr><td height="2"></td></tr>
								<tr>
									<td>
										<!-- PROTO EMERGENZA-->
										<!-- Annullamento	--><asp:panel id="pnl_protoEme" runat="server" Visible="False">
											<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="95%" align="center" border="0">
												<TR>
													<TD class="titolo_scheda" vAlign="middle">&nbsp;&nbsp;Protocollo emergenza&nbsp;</TD>
												</TR>
												<TR>
													<TD height="40">&nbsp;
                                                        <asp:textbox id="txt_ProtoEme" Width="290px" Runat="server" CssClass="testo_grigio" ReadOnly="true"></asp:textbox><br />
                                                
                                                    </TD>
                                                </TR>
                                                <TR>
                                                        <TD class="titolo_scheda" vAlign="middle">&nbsp;&nbsp;In Data&nbsp;&nbsp; <asp:textbox id="txt_dta_protoEme" Width="240px" Runat="server" CssClass="testo_grigio" ReadOnly="True"></asp:textbox><br />&nbsp;</TD>
                                                </TR>
											</TABLE>
										</asp:panel></td>
								</tr>
								
								<tr>
									<td  width="1" height="1">
									    <cc2:Messagebox Width="1" Height="1" CssClass="testo_grigio"  id="msg_TrasmettiRapida" runat="server"></cc2:Messagebox>
									    <cc2:Messagebox  Width="1" Height="1" CssClass="testo_grigio" id="msg_TrasmettiProto" runat="server"></cc2:Messagebox>
										<cc2:messagebox id="msg_copiaDoc" runat="server"></cc2:messagebox>
                                        <cc2:MessageBox ID="msg_rimuoviTipologia" runat="server"></cc2:MessageBox>	
										<cc2:Messagebox Width="1" Height="1" CssClass="testo_grigio"  id="msg_SpedizioneAutomatica" runat="server"></cc2:Messagebox>
										<uc1:ShellWrapper ID="shellWrapper" runat="server" />
                                        <uc2:AdoStreamWrapper ID="adoStreamWrapper" runat="server" />
                                        <uc3:FsoWrapper ID="fsoWrapper" runat="server" />
								    </td>
								</tr>
				                <TR height="1">
					                <TD><iframe id="ifrPrintPen" src="<%=StampaSegnatura%>" width="0" height="0"></iframe></TD>
				                </TR>
							</table>
						</td>
					</tr>
					<tr>
						<td>
							<!-- BOTTONIERA -->
							<TABLE id="tbl_bottoniera" cellSpacing="0" cellPadding="0" align="center" border="0">
								<TR>
									<TD vAlign="top" height="5" colspan="5"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
								</TR>
								<TR>
									<TD>
									    <cc1:imagebutton id="btn_salva_P" Runat="server" AlternateText="Salva" DisabledUrl="../images/bottoniera/btn_salva_nonAttivo.gif" Tipologia="DO_PROT_SALVA" Thema="btn_" SkinID="salva_Attivo" Enabled="False"></cc1:imagebutton>
<%--                                        <asp:Button ID="btnShowFilters" Text="Filtro" CssClass="pulsante79" runat="server" ToolTip="Filtra trasmissioni" />
--%>									</TD>
									<TD>
									    <cc1:imagebutton id="btn_protocolla_P" ondblclick="return false;" Runat="server" AlternateText="Protocolla"	DisabledUrl="../App_Themes/ImgComuni/btn_protocolla_nonAttivo.gif" Tipologia="DO_PROT_PROTOCOLLA" Thema="btn_" SkinID="protocolla_attivo"></cc1:imagebutton>
									    <cc1:imagebutton id="btn_protocollaDisabled" ondblclick="return false;" Runat="server" AlternateText="Protocolla" DisabledUrl="../App_Themes/ImgComuni/btn_protocolla_nonAttivo.gif" ImageUrl="../App_Themes/ImgComuni/btn_protocolla_nonAttivo.gif" Enabled="False" ></cc1:imagebutton>
									    <cc1:imagebutton id="btn_protocollaGiallo_P" ondblclick="return false;" Runat="server" AlternateText="Protocolla in Giallo" DisabledUrl="../App_Themes/ImgComuni/btn_protocolla_nonAttivo.gif" Tipologia="DO_PROT_PROTOCOLLAG" Thema="btn_" SkinID="protocolla_attivo" Visible="False"></cc1:imagebutton>
									</TD>
									<TD>
                                        <cc1:imagebutton id="btn_trasmetti" Runat="server"  Visible="False" OnClientClick="return ShowDialogSpedizioneDocumento();" Tipologia="DO_TRA_TRASMETTI"></cc1:imagebutton><!--Thema="btn_" SkinID="spedisci_attivo"-->
									    <cc1:imagebutton id="btn_spedisci_P" Runat="server" Tipologia="DO_OUT_SPEDISCI"  Visible="False" OnClientClick="return ShowDialogSpedizioneDocumento();"></cc1:imagebutton><!--Thema="btn_" SkinID="spedisci_attivo"-->
									    <cc1:imagebutton id="btn_spedisciDisabled" ondblclick="return false;" Runat="server" AlternateText="Spedisci" DisabledUrl="../App_Themes/ImgComuni/btn_spedisci_nonattivo.gif" Tipologia="DO_OUT_SPEDISCI" ImageUrl="../App_Themes/ImgComuni/btn_spedisci_nonattivo.gif" Enabled="False"></cc1:imagebutton>
									</TD>
									<TD>
									    <cc1:imagebutton id="btn_riproponiDati_P" Runat="server" AlternateText="Riproponi dati" DisabledUrl="../App_Themes/ImgComuni/btn_riproponi_nonattivo.gif" Tipologia="DO_PROT_RIPROPONI" Thema="btn_" SkinID="riproponi_Attivo"></cc1:imagebutton>
									</TD>
									<TD>
									    <cc1:imagebutton id="btn_annulla_P" Runat="server" AlternateText="Annulla" DisabledUrl="../App_Themes/ImgComuni/btn_annulla_nonAttivo.gif" Tipologia="DO_PROT_ANNULLA" Thema="btn_" SkinID="annulla_attivo"></cc1:imagebutton>
									    <cc1:imagebutton id="btn_annullaPred" Runat="server" AlternateText="Annulla Predisponi" DisabledUrl="../App_Themes/ImgComuni/btn_annulla_nonAttivo.gif" Tipologia="DO_PRED_ANNULLA" Thema="btn_" SkinID="annulla_attivo" Visible="false" OnClientClick="return confirm('Confermi annullamento predisposizione?')"></cc1:imagebutton>
									</TD>
								</TR>
							</TABLE>
							<!--FINE	BOTTONIERA -->
						</td>
					</tr>
				</table>
			</div>
			<DIV id="WAIT" style="BORDER-RIGHT: #840000 1px solid; BORDER-TOP: #840000 1px solid; VISIBILITY: hidden; BORDER-LEFT: #840000 1px solid; BORDER-BOTTOM: #840000 1px solid; POSITION: absolute; BACKGROUND-COLOR: #ffc37d">
				<table height="100%" cellSpacing="0" cellPadding="0" width="100%" border="0">
					<tr>
						<td style="FONT-SIZE: 10pt; FONT-FAMILY: Verdana" vAlign="middle" align="center">Stampa ricevuta. Attendere prego...
											</tr>
				</table>
			</DIV>
    		<script language="javascript">
					var h_scroll, h_body;
					h_scroll = document.body.scrollHeight;
					h_body = document.body.offsetHeight;
					if (h_scroll > h_body)
					{
						document.getElementById("tbl_contenitore").style.width=398;
					}
					else
					{
						document.getElementById("tbl_contenitore").style.width=415;
					}
			</script>
		</form>
		<!--</TR></TBODY></TABLE>-->
	</body>
</HTML>
