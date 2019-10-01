<%@ Page language="c#" Codebehind="Organigramma.aspx.cs" AutoEventWireup="false" Inherits="Amministrazione.Gestione_Organigramma.Organigramma" %>
<%@ Register TagPrefix="uc2" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Register src="../UserControl/CalculateAtipicitaOptions.ascx" tagname="CalculateAtipicitaOptions" tagprefix="uc2" %>
<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
        <title></title>
		<script language="C#" runat="server">
			public bool getCheckBox(object abilita)
			{			
				string abil = abilita.ToString();
				if(abil == "true")
				{
					return true;
				}
				else
				{
					return false;
				}
			}                   
		</script>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
		<script language="JavaScript" src="../CSS/caricamenujs.js"></script>
		<script language="javascript">

		    function disabledCb(componentEnabled, componentDisabled) {
		        if (document.getElementById(componentEnabled).checked) 
                {
                    document.getElementById(componentDisabled).checked = false;
                }
		    }

		    function verificaCF() {
		        var cf = document.getElementById('txt_uo_codice_fiscale').value;
		        if (cf.length == 16)
		            return confirm('Per un corrispondente di tipo UO stai inserendo/modificando il campo Codice fiscale con uno di tipo persona, sei sicuro di voler proseguire?');
		        return true;
		    }

		    function ApriOrdinamento(idUo,idLivello,idAmm,descUO)
		    {
		        var myUrl = "Ordinamento.aspx?idAmm="+idAmm+"&idUo="+idUo+"&idLivello="+idLivello+"&descUO="+descUO;			
				rtnValue = window.showModalDialog(myUrl,"","dialogWidth:600px;dialogHeight:500px;status:no;resizable:no;scroll:no;center:yes;help:no;");		
		    }
		    function ApriInvioRubricaComune()
		    {
		        var myUrl = "InvioUORubricaComune.aspx?IdElemento=<%=GetIdUOSelezionata()%>&TipoElemento=UO";
		        rtnValue = window.showModalDialog(myUrl,"","dialogWidth:500px;dialogHeight:150px;status:no;resizable:no;scroll:no;center:yes;help:no;");
		    }
		    function ApriVisibTitolario(idCorrGlobRuolo,idGruppo,idAmm,descRuolo,codRuolo) 
			{			
				var myUrl = "Gestione_Ruolo_Titolario/GestRuoloTitolario.aspx?from=OR&idAmm="+idAmm+"&idCorrGlobRuolo="+idCorrGlobRuolo+"&idGruppo="+idGruppo+"&descRuolo="+descRuolo+"&codRuolo="+codRuolo;			
				rtnValue = window.showModalDialog(myUrl,"","dialogWidth:820px;dialogHeight:650px;status:no;resizable:no;scroll:no;center:yes;help:no;"); 				
			}
			function ApriGestVisibilita(idCorrGlobUO,idAmm,idCorrGlobRuolo,idGruppo) 
			{			
				var myUrl = "GestVisibilita.aspx?from=OR&idCorrGlobUO="+idCorrGlobUO+"&idAmm="+idAmm+"&idCorrGlobRuolo="+idCorrGlobRuolo+"&idGruppo="+idGruppo;			
				rtnValue = window.showModalDialog(myUrl,"","dialogWidth:550px;dialogHeight:350px;status:no;resizable:no;scroll:no;center:yes;help:no;"); 				
			}
			function ApriGestioneUtenti(percorso,idGruppo,idAmm,idCorrGlobRuolo) 
			{			
				var myUrl = "GestUtenti_inRuoloUO.aspx?percorso="+percorso+"&idGruppo="+idGruppo+"&idAmm="+idAmm+"&idCorrGlobRuolo="+idCorrGlobRuolo;				
				rtnValue = window.showModalDialog(myUrl,"","dialogWidth:750px;dialogHeight:550px;status:no;resizable:no;scroll:no;center:yes;help:no;"); 				
			}
			function ApriSpostaUtente(idCorrGlobUtente, userid, nomeUtente, cognomeUtente, idAmm, countUtenti, idCorrGlobGruppo, idGruppo) {
			    var myUrl = "Sposta_Utente.aspx?idCorrGlobUtente=" + idCorrGlobUtente + "&userid=" + userid + "&idAmm=" + idAmm + "&nomeUtente=" + nomeUtente + "&cognomeUtente=" + cognomeUtente + "&countUtenti=" + countUtenti + "&idCorrGlobGruppo=" + idCorrGlobGruppo + "&idGruppo=" + idGruppo;
			    rtnValue = window.showModalDialog(myUrl, "", "dialogWidth:550px;dialogHeight:250px;status:no;resizable:no;scroll:no;center:yes;help:no;");
			    Form1.hd_returnValueModal.value = rtnValue;
			}
			function ApriSpostaRuolo(idCorrGlobRuolo,idGruppo,descRuolo,idAmm,tipoRuolo) 
			{			
				var myUrl = "Sposta_Ruolo.aspx?idCorrGlobRuoloDaSpostare="+idCorrGlobRuolo+"&idGruppoDaSpostare="+idGruppo+"&idAmm="+idAmm+"&descRuoloDaSpostare="+descRuolo+"&tipoRuoloDaSpostare="+tipoRuolo;				
				rtnValue = window.showModalDialog(myUrl,"","dialogWidth:550px;dialogHeight:350px;status:no;resizable:no;scroll:no;center:yes;help:no;"); 								
			}
			function ApriSpostaUO(idCorrGlobUODaSpostare,codUODaSpostare,descUODaSpostare,idAmm,livelloUO_DaSpostare) 
			{			
				var myUrl = "Sposta_UO.aspx?idCorrGlobUODaSpostare="+idCorrGlobUODaSpostare+"&codUODaSpostare="+codUODaSpostare+"&descUODaSpostare="+descUODaSpostare+"&idAmm="+idAmm+"&livelloUO_DaSpostare="+livelloUO_DaSpostare;				
				rtnValue = window.showModalDialog(myUrl,"","dialogWidth:550px;dialogHeight:350px;status:no;resizable:no;scroll:no;center:yes;help:no;"); 								
			}
			function ApriRisultRic(idAmm) 
			{										
				window.document.body.style.cursor='wait';

				if (document.Form1.txt_ricCod.value.length > 0 || document.Form1.txt_ricDesc.value.length > 0) {
				    var myUrl = "RisultatoRicercaOrg.aspx?idAmm=" + idAmm + "&tipo=" + document.Form1.ddl_ricTipo.value + "&cod=" + document.Form1.txt_ricCod.value + "&desc=" + document.Form1.txt_ricDesc.value + "&searchHistoricized=" + document.Form1.chkSearchHistoricized.checked;
					rtnValue = window.showModalDialog(myUrl,"","dialogWidth:750px;dialogHeight:400px;status:no;resizable:no;scroll:no;center:yes;help:no;"); 				
					Form1.hd_returnValueModal.value = rtnValue;
				}
			}
			function ApriOrganigramma(idAmm) {
			    var myUrl = "Navigazione_Organigramma.aspx?readonly=0&navigazione=1&selezione=1&subselezione=&idAmm=" + idAmm;
			    rtnValue = window.showModalDialog(myUrl, "", "dialogWidth:600px;dialogHeight:600px;status:no;resizable:yes;scroll:yes;center:yes;help:no;");
			    Form1.hdReturnValueSelectedRicUO.value = rtnValue;
			    window.document.Form1.submit();
			}		
            function ApriCopiaVisibilitaRuolo(idAmm, idCorrGlobUO, idCorrGlobRuolo, idGruppo, descRuolo, codRuolo, tipoRuolo) {
                var myUrl = "CopiaVisibilitaRuolo.aspx?idAmm=" + idAmm + "&idCorrGlobUO=" + idCorrGlobUO + "&idCorrGlobRuolo=" + idCorrGlobRuolo + "&idGruppo=" + idGruppo + "&descRuolo=" + descRuolo + "&codRuolo=" + codRuolo + "&tipoRuolo=" + tipoRuolo;
                rtnValue = window.showModalDialog(myUrl, "", "dialogWidth:700px;dialogHeight:370px;status:no;resizable:no;scroll:no;center:yes;help:no;");
            }
            function ApriGestioneQualifiche(idAmm, idUo, idGruppo, idPeople) {
                var myUrl = "GestQual_Utente.aspx?idAmm=" + idAmm + "&idUo=" + idUo + "&idGruppo=" + idGruppo + "&idPeople=" + idPeople;
                rtnValue = window.showModalDialog(myUrl, "", "dialogWidth:750px;dialogHeight:550px;status:no;resizable:no;scroll:no;center:yes;help:no;");
            }
            function ApriGestioneApplicazioni(idPeople, username) {
                var myUrl = "GestApps_Utente.aspx?idPeople=" + idPeople + "&IdApplicazione=0";
                rtnValue = window.showModalDialog(myUrl, "", "dialogWidth:750px;dialogHeight:550px;status:no;resizable:no;scroll:no;center:yes;help:no;");
            }
		</script>
		<SCRIPT language="JavaScript">
			
			var cambiapass;
			var hlp;
			var sput;
			var spru;
			var elut;
			var perm;
			
			function VediPermessi(codruolo) {
				perm = window.open('../Gestione_Funzioni/PermessiFunzioni.aspx?codruolo=' + codruolo,'','width=650,height=500,scrollbars=YES');
			}
			function apriPopup() {
				window.open('../help.aspx?from=OR','','width=450,height=500,scrollbars=YES');
			}				
			function cambiaPwd() {
				window.open('../CambiaPwd.aspx','','width=410,height=300,scrollbars=NO');
			}	
			function ClosePopUp()
			{	
				if(typeof(cambiapass) != 'undefined')
				{
					if(!cambiapass.closed)
						cambiapass.close();
				}
				if(typeof(hlp) != 'undefined')
				{
					if(!hlp.closed)
						hlp.close();
				}				
				if(typeof(sput) != 'undefined')
				{
					if(!sput.closed)
						sput.close();
				}
				if(typeof(spru) != 'undefined')
				{
					if(!spru.closed)
						spru.close();
				}
				if(typeof(elut) != 'undefined')
				{
					if(!elut.closed)
						elut.close();
				}
				if(typeof(perm) != 'undefined')
				{
					if(!perm.closed)
						perm.close();
				}
			}
			 
            /*        
            function gestioneClickReg(indice, idReg)
            { 
               if(document.getElementById('pnlRF')!=null)
               {
                   
			        //prendo le righe della tabella relativa agli RF
			        trs=document.getElementById('dg_rf').tBodies[0].rows;
			        //indice di riga cliccato si ottiene incrementando di uno quello corrente
			        var indNew = parseInt(indice) +1;
    			 
    			    //prendo il check del datagrid registri
    			    trsReg=document.getElementById('dg_registri').tBodies[0].rows;
    			    listaTdReg = trsReg[indNew].cells;
    			    checkReg = (listaTdReg[2].getElementsByTagName("input"))[0];
    			
    			    if(checkReg.checked)
    			    {
			            for(i=1;i<trs.length;i++)
			            {
			                listaTd=trs[i].cells;

			                idAooColl = ((listaTd[2].getElementsByTagName("input"))[1].value);
        			      
			                if(idAooColl==idReg)
		                    {  
  	                           //Abilito il check relativo al registro selezionato
		                       checkRF = (listaTd[2].getElementsByTagName("input"))[0];
    		                   
    		                   //abilito il check
		                       checkRF.disabled = false;
        		               
		                    }
			            }
			        }
			        else
			        {
			            for(i=1;i<trs.length;i++)
			                {
			                    listaTd=trs[i].cells;
            			        
        			            
			                    idAooColl = ((listaTd[2].getElementsByTagName("input"))[1].value);
            			      
			                    if(idAooColl==idReg)
		                        {  
  	                               //Abilito il check relativo al registro selezionato
		                           checkRF = (listaTd[2].getElementsByTagName("input"))[0];
        		                   
    		                       //disabilito il check
		                           checkRF.disabled = true;
		                            checkRF.checked = false;
            		               
		                        }
			                }
			            }
			       
			     }    
            }
            
            
          function  DisabledGridRF()
          {
                //prendo le righe della tabella relativa agli rf
			    righeRF=document.getElementById('dg_rf').tBodies[0].rows;
			    //indice di riga cliccato si ottiene incrementando di uno quello corrente
			
			    for(m=1;m<righeRF.length;m++)
			    {
			        listaTd=righeRF[m].cells;
			        
			        //idAooColl = (listaTd[1].getElementsByTagName("input"))[1].value;
			          idAooColl = ((listaTd[2].getElementsByTagName("input"))[1].value);
			      
			        if(idAooColl!="")
		            {  
		               
		               //Vedo se il registro associato all'RF è ceccato
		               checkRF = (listaTd[2].getElementsByTagName("input"))[0];
		             
		               checkRF.disabled = true;
		
		            }
			  
			    }    
          }
          
          function  GestioneLoadGridRegistriRF()
          {
                 if(document.getElementById('dg_registri')!=null)  
                 {
                    //prendo le righe della tabella relativa agli rf
		            trs=document.getElementById('dg_registri').tBodies[0].rows;
                 }       
               
		        //indice di riga cliccato si ottiene incrementando di uno quello corrente
		        if(document.getElementById('dg_rf')!=null)
		        {
		           //disabilito tutti gli RF
                    DisabledGridRF();
                    
		           //prendo le righe della tabella relativa agli rf
			        trsRF=document.getElementById('dg_rf').tBodies[0].rows;
			   
		            for(i=1;i<trs.length;i++)
		            {
		                listaTd=trs[i].cells;
		                idReg = ((listaTd[2].getElementsByTagName("input"))[1].value);
                        
                        //vedo se il check relativo al registro è fleggato
                        checkReg = (listaTd[2].getElementsByTagName("input"))[0];

                        if(checkReg.checked)
                        {
                           
                            //rendo abilitati i check relativi ai registri selezionati
                            for(l=1;l<trsRF.length;l++)
		                    {
		                         listaTdRF=trsRF[l].cells;
		                         idAooColl = ((listaTdRF[2].getElementsByTagName("input"))[1].value);
                                 
                                 //Vedo se il registro associato all'RF è ceccato
                                 checkRF = (listaTdRF[2].getElementsByTagName("input"))[0];
			                     if(idAooColl == idReg)
			                     {
                                    checkRF.disabled = false;
                                 }

	                        }
	                    }
		            }
		        }  
             }
             */

		    function confirmStoricizza()
		    {
			    var agree=confirm("Attenzione! la UO sarà storicizzata e tutti i suoi ruoli saranno spostati nella nuova UO creata. Sei sicuro di voler procedere?");
			    if (agree)
			    {
				    document.getElementById("txt_confirmSto").value = "si";
				    return true ;
			    }			
		    }

             function ApriImportaOrganigramma() {
                 var myUrl = "ImportaOrganigramma.aspx";
                 rtnValue = window.showModalDialog(myUrl, "", "dialogWidth:430px;dialogHeight:150px;status:no;resizable:no;scroll:no;center:yes;help:no;");
                 //rtnValue = window.open(myUrl,"","dialogWidth:430px;dialogHeight:180px;status:no;resizable:no;scroll:no;center:yes;help:no;");
                 Form1.submit();
             }

             /*
              * Funzione per mostrare una confirm che avvisi l'utente che l'operazione richiesta 
              * richiederà del tempo per il suo completamento.
              */
             function ShowConfirmBeforeStartModMove(buttonRef) {

                 // Messaggio da mostrare nella confirm
                 var messageToShow;
             
                 // Il bottone deve contenere il test 'Sposta' o 'Modifica'
                 if (buttonRef.value === 'Sposta' || buttonRef.value === 'Modifica') {
                     // Costruzione del testo da mostrare
                     if (document.getElementById('chkStoricizza').checked)
                         messageToShow = 'La conferma delle modifiche provocherà la disabilitazione del ruolo (ruolo storicizzato), ' +
                                         'la creazione di un nuovo ruolo (legato logicamente al ruolo storicizzato) e la ' +
                                         'storicizzazione di tutti gli oggetti (documenti, fascicoli, trasmissioni) associati al ' + 
                                         'ruolo storicizzato.';
                     else
                         messageToShow = 'La conferma delle modifiche avrà effetto immediato sul ruolo, quindi il ruolo corrente ' +
                                         'non sarà storicizzato e ' +
                                         'tutti gli oggetti (documenti, fascicoli, trasmissioni) associati al ' +
                                         'ruolo, non saranno storicizzati.';

                     // Se è stato deflaggato "Aggiorna modelli" viene mostrato un avviso in coda agli altri
                     if (!document.getElementById('chkUpdateModels').checked)
                         messageToShow += '\n\nAttenzione! Procedendo verranno cancellati i modelli visibili unicamente al ruolo';

                     messageToShow += '\n\nSi desidera continuare?';

                    return confirm(messageToShow);
                 }
                 else { 
                    // Si può continuare
                    return true;
                 }
             }
             
            
		</SCRIPT>
        <script type="text/javascript" language="javascript">
            function manageDropDownListRegistries(check) {
                try {
                    if (!document.getElementById(check).checked) {
                        document.getElementById('ddlRegistriInteropSemplificata').selectedIndex = 0;
                        document.getElementById('ddlRegistriInteropSemplificata').disabled = 'disabled';

                        document.getElementById('ddlRfInteropSemplificata').disabled = 'disabled';
                        document.getElementById('ddlRfInteropSemplificata').selectedIndex = 0;
                    }
                    else {
                        document.getElementById('ddlRegistriInteropSemplificata').disabled = '';
                        document.getElementById('ddlRfInteropSemplificata').disabled = '';
                    }
                        
                } catch (e) {

                }
            }
        </script>
	</HEAD>
	<!--<body bottomMargin="0" leftMargin="0" topMargin="0" rightMargin="0" onunload="ClosePopUp()" onload = "GestioneLoadGridRegistriRF()">-->
    <body bottomMargin="0" leftMargin="0" topMargin="0" rightMargin="0" onunload="ClosePopUp()">
		<form id="Form1" method="post" runat="server">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Organigramma" />
			<!-- Gestione del menu a tendina --><uc2:menutendina id="MenuTendina" runat="server"></uc2:menutendina>
            <input id="hd_returnValueModal" type="hidden" name="hd_returnValueModal" runat="server">
            <input id="hdReturnValueSelectedRicUO" type="hidden" name="hdReturnValueSelectedRicUO" runat="server" />
            <input id="hd_DisableRole" type="hidden" name="hd_DisableRole" runat="server">
            <asp:HiddenField ID="hfRetValModSposta" runat="server" />
			<!-- TESTATA CON MENU' -->
			<table height="100%" cellSpacing="1" cellPadding="0" width="100%" border="0">
				<tr>
					<td><uc1:testata id="Testata" runat="server"></uc1:testata></td>
				</tr>
				<tr>
					<td class="testo_grigio_scuro" bgColor="#c0c0c0" height="20"><asp:label id="lbl_position" runat="server"></asp:label></td>
				</tr>
				<tr>
					<!-- TITOLO PAGINA -->
					<td class="titolo" align="center" bgColor="#e0e0e0" height="20">Organigramma</td>
				</tr>
				<tr>
					<td vAlign="top" align="center" bgColor="#f6f4f4" height="100%">
						<!-- INIZIO: INSERIRE QUI IL CORPO CENTRALE -->
						<table cellSpacing="0" cellPadding="0" width="100%" border="0">
							<tr>
								<td align="left" colSpan="3"><asp:label id="lbl_avviso" CssClass="testo_rosso" Runat="server"></asp:label></td>
							</tr>
							<tr >
								<!-- LATO SX --------------------------- TREEVIEW ------------------------------- -->
								<td vAlign="top" width="40%"><asp:panel id="pnl_tv" Runat="server">
										<TABLE cellSpacing="5" cellPadding="5" width="40%" border="0">
											<TR>
												<TD class="pulsanti">
													<TABLE cellSpacing="2" cellPadding="0" width="40%" border="0">
														<TR>
															<TD class="testo_piccoloB">Ricerca tra:</TD>
															<TD class="testo_piccoloB">Codice:</TD>
															<TD class="testo_piccoloB" id="td_descRicerca" runat="server">Nome UO:</TD>
															<TD class="testo_piccoloB"></TD>
														</TR>
														<TR>
															<TD class="testo_piccoloB">
																<asp:DropDownList id="ddl_ricTipo" Runat="server" CssClass="testo_grigio_scuro" AutoPostBack="True">
																	<asp:ListItem Value="U" Selected="True">Unità Organizz.</asp:ListItem>
																	<asp:ListItem Value="R">Ruolo</asp:ListItem>
																	<asp:ListItem Value="PN">Nome</asp:ListItem>
																	<asp:ListItem Value="PC">Cognome</asp:ListItem>
																</asp:DropDownList></TD>
															<TD class="testo_piccoloB">
																<asp:TextBox id="txt_ricCod" tabIndex="1" Runat="server" CssClass="testo_grigio_scuro" Width="80"></asp:TextBox></TD>
															<TD class="testo_piccoloB">
																<asp:TextBox id="txt_ricDesc" tabIndex="2" Runat="server" CssClass="testo_grigio_scuro" Width="210"></asp:TextBox></TD>
                                                            <td></td>
														</TR>
                                                        <tr>
                                                            <td class="testo_piccoloB" colspan="4" style="text-align:right; padding-top:5px;">
                                                                <asp:CheckBox ID="chkSearchHistoricized" runat="server" Text="Storicizzati" ToolTip="Cerca ruolo storicizzato" Enabled="false" />
                                                                <asp:button id="btn_find" tabIndex="3" Runat="server" CssClass="testo_btn" Text="Cerca"></asp:button>&nbsp;
                                                                <asp:Button ID="btnGenerateSummary" runat="server" CssClass="testo_btn" Text="Report consistenza" ToolTip="Genera report consistenza" OnClick="btnGenerateSummary_Click" />
                                                            </td>
                                                        </tr>
													</TABLE>
												</TD>
											</TR>
											<TR>
												<TD vAlign="top">
													<IEWC:TREEVIEW id="treeViewUO" runat="server" AutoPostBack="True" height="480px" width="480px"
														font="verdana" bordercolor="maroon" borderstyle="solid" borderwidth="1px" backcolor="antiquewhite"
														DefaultStyle="font-weight:normal;font-size:10px;color:black;text-indent:0px;font-family:Verdana;"></IEWC:TREEVIEW></TD>
											</TR>
										</TABLE>
									</asp:panel></td>
								<!-- FINE LATO SX -->

                                <!-- SPAZIETTO CENTRALE -->
								<td width="2%" valign=top></td>

								<!-- LATO DX ----------------------------- DATI --------------------------------- -->
								<td vAlign="top" align="center" width="50%">
									<table cellSpacing="2" cellPadding="0" width="0" border="0">
										<tr>
											<td vAlign="top"><asp:label id="lbl_percorso" runat="server" CssClass="label_percorso"></asp:label></td>
										</tr>
										<!-- Unità Organizzativa -->
                                        <asp:panel id="pnl_uo" runat="server" Visible="true">
											<TR>
												<TD >
													<TABLE class="contenitore" width="100%">
														<TR>
															<TD colSpan="2">
																<TABLE cellSpacing="0" cellPadding="0" width="100%" border="0">
																	<TR>
																		<TD class="titolo_pnl" vAlign="middle" align="center" width="5%"><IMG src="../Images/uo.gif" border="0">
																		</TD>
																		<TD class="titolo_pnl" vAlign="middle" width="30%">Unità organizzativa:
																		</TD>
																		<TD class="titolo_pnl" vAlign="middle" align="right" width="65%"></TD>
																	</TR>
																</TABLE>
															</TD>
														</TR>
														<TR>
															<TD class="testo_grigio_scuro" width="20%">Codice *</TD>
															<TD align="left">
																<asp:TextBox id="txt_rubricaUO" tabIndex="4" runat="server" CssClass="testo" Width="350px" MaxLength="128" AutoPostBack="true"></asp:TextBox></TD>
														</TR>
														<TR>
															<TD class="testo_grigio_scuro" width="20%">Descrizione *</TD>
															<TD align="left">
																<asp:TextBox id="txt_descrizioneUO" tabIndex="5" runat="server" CssClass="testo" Width="350px"
																	MaxLength="128"></asp:TextBox></TD>
														</TR>
														<TR vAlign="middle">
															<TD class="testo_grigio_scuro" width="20%">Interoperante</TD>
															<TD align="left">
																<asp:CheckBox id="chk_interopUO" tabIndex="6" runat="server" CssClass="testo" AutoPostBack="True"
																	Checked="False"></asp:CheckBox>&nbsp;&nbsp;
																<asp:DropDownList id="ddl_aoo" tabIndex="7" runat="server" CssClass="testo" AutoPostBack="False" Width="318px">
																	<asp:ListItem Value="null" Selected="True">Seleziona AOO...</asp:ListItem>
																	<asp:ListItem Value="null">---------------------</asp:ListItem>
																</asp:DropDownList></TD>
														</TR>
														<TR>
															<TD colSpan="2" height="5"></TD>
														</TR>
                                                        <TR vAlign="middle" runat="server" id="trInteropSemplificata">
															<TD class="testo_grigio_scuro" width="20%">Interoperante PiTre</TD>
															<TD align="left">
                                                                <div style="float: left;">
                                                                    <div style="float: left;">
																        <asp:CheckBox id="chkInteroperante" runat="server" CssClass="testo" Checked="False"></asp:CheckBox>&nbsp;&nbsp;
                                                                    </div>
                                                                    <div style="float: inherit;">
																        <asp:DropDownList id="ddlRegistriInteropSemplificata" runat="server" CssClass="testo" Width="318px">
																	        <asp:ListItem Value="null" Selected="True">Seleziona AOO...</asp:ListItem>
																	        <asp:ListItem Value="null">---------------------</asp:ListItem>
																        </asp:DropDownList>
                                                                    </div>
                                                                    <div style="float: inherit;">
                                                                        <asp:DropDownList ID="ddlRfInteropSemplificata" runat="server" CssClass="testo" Width="318px">
                                                                            <asp:ListItem Value="null" Selected="True">Seleziona RF...</asp:ListItem>
																	        <asp:ListItem Value="null">---------------------</asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </div>
                                                                </div>
                                                                <div style="clear: both;"></div>
                                                                </TD>
                                                                
														</TR>
														<TR>
															<TD colSpan="2" height="5"></TD>
														</TR>
														<TR>
															<TD class="testo_grigio_scuro" width="20%">Indirizzo</TD>
															<TD align="left">
																<asp:TextBox id="txt_uo_indirizzo" tabIndex="8" runat="server" CssClass="testo" Width="350px"
																	MaxLength="128"></asp:TextBox></TD>
														</TR>
														<TR>
															<TD class="testo_grigio_scuro" width="20%">Città</TD>
															<TD align="left">
																<asp:TextBox id="txt_uo_citta" tabIndex="9" runat="server" CssClass="testo" Width="350px" MaxLength="64"></asp:TextBox></TD>
														</TR>
														<TR>
															<TD class="testo_grigio_scuro" width="20%">Cap</TD>
															<TD align="left">
																<asp:TextBox id="txt_uo_cap" tabIndex="10" runat="server" CssClass="testo" Width="50px" MaxLength="5"></asp:TextBox></TD>
														</TR>
														<TR>
															<TD class="testo_grigio_scuro" width="20%">Provincia</TD>
															<TD align="left">
																<asp:TextBox id="txt_uo_prov" tabIndex="11" runat="server" CssClass="testo" Width="50px" MaxLength="2"></asp:TextBox></TD>
														</TR>
														<TR>
															<TD class="testo_grigio_scuro" width="20%">Nazione</TD>
															<TD align="left">
																<asp:TextBox id="txt_uo_nazione" tabIndex="12" runat="server" CssClass="testo" Width="150px"
																	MaxLength="32"></asp:TextBox></TD>
														</TR>
														<TR>
															<TD class="testo_grigio_scuro" width="20%">Telefono #1</TD>
															<TD align="left">
																<asp:TextBox id="txt_uo_telefono1" tabIndex="13" runat="server" CssClass="testo" Width="150px"
																	MaxLength="16"></asp:TextBox></TD>
														</TR>
														<TR>
															<TD class="testo_grigio_scuro" style="HEIGHT: 21px" width="20%">Telefono #2</TD>
															<TD style="HEIGHT: 21px" align="left">
																<asp:TextBox id="txt_uo_telefono2" tabIndex="14" runat="server" CssClass="testo" Width="150px"
																	MaxLength="16"></asp:TextBox></TD>
														</TR>
														<TR>
															<TD class="testo_grigio_scuro" width="20%">Fax</TD>
															<TD align="left">
																<asp:TextBox id="txt_uo_fax" tabIndex="15" runat="server" CssClass="testo" Width="150px" MaxLength="16"></asp:TextBox></TD>
														</TR>
                                                        <TR>
															<TD class="testo_grigio_scuro" width="20%">Note</TD>
															<TD align="left">
																<asp:TextBox id="txt_uo_note" tabIndex="15" runat="server" CssClass="testo" Width="150px" MaxLength="16"></asp:TextBox></TD>
														</TR>
                                                        <TR>
                                                            <TD class="testo_grigio_scuro" width="20%">Codice Fiscale </TD>
                                                            <TD align="left">
                                                                <asp:TextBox id="txt_uo_codice_fiscale" tabIndex="15" runat="server" CssClass="testo" Width="150px" MaxLength="16"></asp:TextBox></TD> 
                                                        </TR>
                                                        <TR>
                                                            <TD class="testo_grigio_scuro" width="20%">Partita Iva </TD>
                                                            <TD align="left">
                                                                <asp:TextBox id="txt_uo_partita_iva" tabIndex="15" runat="server" CssClass="testo" Width="150px" MaxLength="11"></asp:TextBox></TD> 
                                                        </TR>
                                                        <TR>
															<TD class="titolo_pnl" colSpan="2">
																<TABLE cellPadding="0" align="right" border="0" width="100%">
																	<TR>
                                                                        <TD class="titolo_pnl">
                                                                            <asp:button id="btn_ordinamento" runat="server" CssClass="testo_btn_org" Text="Ordinamento" ToolTip="Ordinamento del ramo della UO" Visible=false></asp:button></TD>
																	    <TD class="titolo_pnl">
																			<asp:button id="btn_annulla_ins_uo" runat="server" CssClass="testo_btn_org" Text="Annulla" ToolTip="Annulla operazione di inserimento UO inferiore" Visible=false></asp:button></TD>
																	    <TD class="titolo_pnl">
																			<asp:button id="btn_clear_uo" runat="server" CssClass="testo_btn_org" Text="Pulisci campi" ToolTip="Ripulisce tutti i campi dei dettagli" Visible=false></asp:button></TD>
																	    <TD class="titolo_pnl">
																			<asp:button id="btn_elimina_uo" tabIndex="19" runat="server" CssClass="testo_btn_org" Text="Elimina"></asp:button></TD>
																		<TD class="titolo_pnl">
																		    <asp:button ID="btn_importaOrganigramma" runat="server" CssClass="testo_btn_org" Text="Importa" ToolTip="Importazione dell'organigramma" Visible="false" OnClick="btn_ImportaOrganigramma_Click"></asp:button></TD>
																		<TD class="titolo_pnl">
																			<asp:button id="btn_sposta_uo" runat="server" CssClass="testo_btn_org" Text="Sposta" ToolTip="Sposta l'unità organizzativa in un altro ramo dell'organigramma"></asp:button></TD>
																		<TD class="titolo_pnl">
																			<asp:button id="btn_ins_ruolo_uo" tabIndex="18" runat="server" CssClass="testo_btn_org" Text="Ins. Ruolo"></asp:button></TD>
																		<TD class="titolo_pnl">
																			<asp:button id="btn_ins_sotto_uo" tabIndex="17" runat="server" CssClass="testo_btn_org" Text="Ins. UO inferiore"></asp:button></TD>
																		<TD class="titolo_pnl">
																			<asp:button id="btn_mod_uo" tabIndex="16" runat="server" CssClass="testo_btn_org" Text="Modifica" ToolTip="Applica le modifiche senza la storicizzazione della UO" OnClientClick="return verificaCF()"></asp:button></TD>
																		<TD class="titolo_pnl">
																			<asp:button id="btn_storicizza" tabIndex="20" runat="server" CssClass="testo_btn_org" Text="Storicizza" ToolTip="Applica le modifiche con la storicizzazione della UO"></asp:button></TD>
																	</TR>
																</TABLE>
															</TD>
														</TR>
														<INPUT id="txt_confirmSto" type="hidden" name="txt_confirmSto" runat="server"></input>				
														<asp:Panel ID="pnlRubricaComune" runat="server">
										                    <tr>
										                        <td height=5></td>
										                    </tr>																								
														    <TR>
														        <td colspan = "2">
														            <table border=0 cellpadding=0 cellspacing=0 width=100% align=center>
														                <tr>
														                    <td class="titolo_pnl" align="left" colSpan="2">Rubrica comune:</td>
														                </tr>
														                <tr>
														                    <td height=5></td>
														                </tr>
                                                                        <tr>
															                <td class="testo_grigio_scuro" width="20%">
															                    Codice:
															                </td>
															                <td class="testo_grigio_scuro" width="350px">
															                    <asp:Label ID = "lblCodiceRC" runat = "server"></asp:Label>
                                                                            </td>
														                </tr>
														                <tr>
														                    <td height=5></td>
														                </tr>			
                                                                        <tr>
															                <td class="testo_grigio_scuro" width="20%">
															                    Descrizione:
															                </td>
															                <td class="testo" class = "testo_grigio" width = "350px">
															                    <asp:Label ID = "lblDescrizioneRC" runat = "server"></asp:Label>
                                                                            </td>
														                </tr>
														                <tr>
														                    <td height=5></td>
														                </tr>														                											                
														                <tr>
															                <td class="testo_grigio_scuro" width="20%">
															                    Creato il:
															                </td>
															                <td class="testo" class = "testo_grigio" width = "350px">
															                    <asp:Label ID = "lblDataCreazioneRC" runat = "server"></asp:Label>
                                                                            </td>
														                </tr>
														                <tr>
														                    <td height=5></td>
														                </tr>
														                <TR>
															                <TD class="testo_grigio_scuro" width="20%">
															                    Modificato il:
															                </TD>
															                <TD class="testo_grigio_scuro">
															                    <asp:Label ID="lblDataUltimaModificaRC" runat="server" width="350px"></asp:Label>
                                                                            </TD>
														                </TR>	
														                <tr>
														                    <td height=5></td>
														                </tr>
														            </table>
														        </td>
														    </TR>
                                                            <TR>
															    <TD class="titolo_pnl" colSpan="2">
																    <TABLE cellPadding="0" align="right" border="0">
																	    <TR>
																	        <TD class="titolo_pnl">&nbsp;
																			    <asp:button id="btn_invia_rc" tabIndex="19" runat="server"  OnClientClick = "return ApriInvioRubricaComune()" CssClass="testo_btn_org" Text="Invia a rubrica" OnClick="btn_invia_rc_OnClick"></asp:button></TD>
																	    </TR>
																    </TABLE>
															    </TD>
														    </TR>																    
														</asp:Panel>
													</TABLE>
												</TD>
											</TR>
										</asp:panel>										
										<!-- Ruoli -->
										<asp:panel id="pnl_ruoli" runat="server" Visible="true">
											<tr>
												<td>
													<TABLE class="contenitore" width="97%">
														<TR>
															<TD colSpan="2">
																<TABLE cellSpacing="0" cellPadding="0" width="100%" border="0">
																	<TR>
																		<TD class="titolo_pnl" vAlign="middle" align="center" width="5%"><IMG src="../Images/ruolo.gif" border="0">
																		</TD>
																		<TD class="titolo_pnl" vAlign="middle" width="15%">Ruolo:
																		</TD>
																		<TD class="titolo_pnl" vAlign="middle" align="right" width="80%"></TD>
																	</TR>
																</TABLE>
															</TD>
														</TR>
														<TR>
															<TD class="testo_grigio_scuro" style="HEIGHT: 23px" width="20%">Tipo ruolo *</TD>
															<TD style="HEIGHT: 23px" align="left">
																<asp:DropDownList id="ddl_tipo_ruolo" tabIndex="20" runat="server" CssClass="testo" AutoPostBack="True"
																	Width="350px">
																	<asp:ListItem Value="null" Selected="True">Seleziona...</asp:ListItem>
																	<asp:ListItem Value="null">---------------------</asp:ListItem>
																</asp:DropDownList></TD>
														</TR>
														<TR>
															<TD class="testo_grigio_scuro" width="20%">Codice *</TD>
															<TD align="left">
																<asp:TextBox id="txt_rubricaRuolo" tabIndex="21" runat="server" CssClass="testo" Width="175px"
																	MaxLength="20"></asp:TextBox></TD>
														</TR>
														<TR>
															<TD class="testo_grigio_scuro" width="20%">Descrizione *</TD>
															<TD align="left">
																<asp:TextBox id="txt_descrizioneRuolo" tabIndex="22" runat="server" CssClass="testo" Width="350px"
																	MaxLength="128"></asp:TextBox></TD>
														</TR>
                                                        <tr runat="server" id="trCodUO">
                                                            <td class="testo_grigio_scuro" style="width: 20%">
                                                                UO di app. *
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtCodUo" runat="server" CssClass="testo" Width="100"
                                                                    AutoPostBack="True" ></asp:TextBox>&nbsp;<asp:TextBox ID="txtDescUo" TabIndex="1" runat="server"
                                                                        CssClass="testo" Width="300" ReadOnly="True"></asp:TextBox>
                                                                <asp:HiddenField ID="hdIdCorrGlob" runat="server" />
                                                                <asp:ImageButton ID="btnOrg" runat="server" AlternateText="Ricerca in organigramma"
                                                                    ImageUrl="../../images/proto/ico_titolario.gif" TabIndex="2" ></asp:ImageButton>
                                                            </td>
                                                        </tr>
														<tr>
														   <td colspan ="2">
   														   <table border="0" cellpadding="0" cellspacing="0">
                                                           <TR>
                                                                <td class="testo_grigio_scuro" width="20%" colspan="3">Disabilitato ricezione trasmissioni</td>
                                                                <td align="left" colspan="3"><asp:CheckBox id="cb_disabled_trasm" tabIndex="23" runat="server" CssClass="testo"></asp:CheckBox></td>
                                                            </TR>
   														    <TR>
															    <TD class="testo_grigio_scuro" width="20%">Di riferimento
															    </TD>
															    <TD align="left">
																    <asp:CheckBox id="chk_rif" tabIndex="23" runat="server" CssClass="testo"></asp:CheckBox></TD>
															    <TD class="testo_grigio_scuro" width="20%">Responsabile
															    </TD>
															    <TD align="left">
																    <asp:CheckBox id="chk_resp" tabIndex="24" runat="server" CssClass="testo"></asp:CheckBox></TD>																																															
																    <TD class="testo_grigio_scuro" width="20%">Segretario
															    </TD>
															    <TD align="left">
																    <asp:CheckBox id="cb_segretario" tabIndex="24" runat="server" CssClass="testo"></asp:CheckBox></TD>																																
														    </TR>
                                                            <tr runat="server" id="trStoricizzaAndUpdateModels">
                                                                <td class="testo_grigio_scuro" width="20%">
                                                                    Storicizza ruolo
															    </td>
															    <td align="left">
																    <asp:CheckBox ID="chkStoricizza" Checked="true" runat="server" CssClass="testo" />
                                                                </td>
                                                                <td class="testo_grigio_scuro" width="20%">
                                                                    Aggiorna modelli di trasmissione
															    </td>
															    <td align="left">
																    <asp:CheckBox ID="chkUpdateModels" runat="server" CssClass="testo" />
                                                                </td>
                                                            </tr>
                                                            <tr id="trExtendVisibility" runat="server">
                                                                <td class="testo_grigio_scuro">
                                                                    Estensione visibilità: 
                                                                </td>
                                                                <td colspan="5">
                                                                    <asp:RadioButtonList ID="rblExtendVisibility" runat="server" CssClass="testo">
                                                                        <asp:ListItem Text="Non estendere" Value="N" Selected="True" />
                                                                        <asp:ListItem Text="Estendi visibilità ai superiori escludendo documenti e fascicoli a visibilità atipica" Value="E">Est. a sup. escludendo atipici</asp:ListItem>
                                                                        <asp:ListItem Text="Estendi visibilità ai superiori includendo documenti e fascicoli a visibilità atipica" Value="A">Est. a sup. includendo atipici</asp:ListItem>
                                                                    </asp:RadioButtonList>
                                                                </td>
                                                            </tr>
                                                            <tr runat="server" id="trCalculateAtipicita" visible="false" style="border:1px solid black;">
                                                                <td class="testo_grigio_scuro">
                                                                    Calcolo atipicità: 
                                                                </td>
                                                                <td colspan="5">
                                                                    <uc2:CalculateAtipicitaOptions ID="calculateAtipicitaOptions" runat="server" />
                                                                </td>
                                                            </tr>
   														   </table>
														   </td>
														</tr>
														
														<TR>
															<TD class="titolo_pnl" colSpan="2">
																<TABLE cellSpacing="0" cellPadding="0" align="right" border="0" width="100%">
																	<TR>
                                                                        <td class="titolo_pnl">
                                                                            <button ID="btnShowRoleHistory" runat="server" class="testo_btn_org" title="Mostra storia ruolo" type="button">Storia ruolo</button>
                                                                        </td>
                                                                        <td class="titolo_pnl">&nbsp;
                                                                            <asp:button id="btn_copiaVisRuolo" runat="server" CssClass="testo_btn_org" Text="Copia Vis."></asp:button></td>
																		<TD class="titolo_pnl">&nbsp;
																			<asp:button id="btn_elimina_ruolo" tabIndex="27" runat="server" CssClass="testo_btn_org" Text="Elimina"></asp:button></TD>
																		<TD class="titolo_pnl">&nbsp;
																			<asp:button id="btn_visib_titolario" tabIndex="26" runat="server" CssClass="testo_btn_org" Text="Visibilità Titolario"
																				ToolTip="Gestione della visibilità del ruolo sui nodi di titolario" Visible="false"></asp:button></TD>
																		<TD class="titolo_pnl">&nbsp;
																			<asp:button id="btn_sposta_ruolo" tabIndex="26" runat="server" CssClass="testo_btn_org" Text="Sposta"
																				ToolTip="Sposta il ruolo in un altra UO dell'organigramma" OnClientClick="return ShowConfirmBeforeStartModMove(this);"></asp:button></TD>
																		<TD class="titolo_pnl">&nbsp;
																			<asp:button id="btn_utenti_ruolo" tabIndex="25" runat="server" CssClass="testo_btn_org" Text="Gestione utenti"></asp:button></TD>
	                                                                    <TD class="titolo_pnl">&nbsp;
																			<asp:button id="btn_mod_ruolo" tabIndex="24" runat="server" CssClass="testo_btn_org" Text="Modifica" ToolTip="Modifica ruolo" OnClientClick="return ShowConfirmBeforeStartModMove(this);"></asp:button></TD>
																	</TR>
																</TABLE>
																
															</TD>
														</TR>
													</TABLE>
												</td>
											</tr>
										</asp:panel>

                                        <!-- REGISTRI/RF -->
                                        <tr>
                                            <td>
                                                <asp:panel id="pnl_registri" CssClass="contenitore" runat="server" Visible="true" Width="97%">
                                                    <asp:ScriptManager ID="scrManager" runat="server"></asp:ScriptManager>
													    <asp:Panel runat="server" ID="pnlReg">
                                                            <!-- intestazione Registro -->
                                                            <table style="width:99%;margin:3px;" cellspacing="0" cellpadding="0">
														        <tr>
															        <td class="titolo_pnl" vAlign="middle" align="center" width="5%"><IMG src="../Images/registri.gif" border="0">
															        </td>
															        <td class="titolo_pnl" vAlign="middle" width="30%">Registri
															        </td>
															        <td class="titolo_pnl" vAlign="middle" align="right" width="65%">
                                                                    </td>
														        </tr>
													        </table>
                                                            <!-- content registro -->
                                                            <div style="overflow:auto;height:89px;width:99%; padding:3px;">
                                                                <asp:UpdatePanel ID="UpdatePanelRegistro" runat="server" UpdateMode="Always">
                                                                    <ContentTemplate>                                                                    
                                                                        <asp:DataGrid Width="100%" ID="dg_registri" TabIndex="28" runat="server" Height="59px" AutoGenerateColumns="False" CellPadding="1" BorderWidth="1px" BorderColor="Gray" >
                                                                            <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
                                                                            <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
                                                                            <ItemStyle CssClass="bg_grigioN"></ItemStyle>
                                                                            <HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
                                                                            <Columns>
                                                                                <asp:BoundColumn Visible="False" DataField="IDRegistro" ReadOnly="True" HeaderText="ID"></asp:BoundColumn>
                                                                                <asp:BoundColumn DataField="Codice" ReadOnly="True" HeaderText="Codice">
                                                                                    <ItemStyle Wrap="true"></ItemStyle>
                                                                                </asp:BoundColumn>
                                                                                <asp:BoundColumn DataField="Descrizione" ReadOnly="True" HeaderText="Descrizione">
                                                                                    <ItemStyle Wrap="true"></ItemStyle>
                                                                                </asp:BoundColumn>
                                                                                <asp:BoundColumn DataField="EmailRegistro" ReadOnly="True" HeaderText="Email">
                                                                                    <ItemStyle Wrap="true"></ItemStyle>
                                                                                </asp:BoundColumn>
                                                                                <asp:BoundColumn Visible="False" DataField="IDAmministrazione" ReadOnly="True" HeaderText="IDAmm">
                                                                                </asp:BoundColumn>
                                                                                <asp:TemplateColumn HeaderText="Sel">
                                                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                                                                    <ItemStyle Wrap="true" HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                                                                    <ItemTemplate>
                                                                                        <asp:CheckBox ID="cbx_Sel" AutoPostBack="true" OnCheckedChanged="cbx_SelReg_CheckedChanged" Checked='<%# getCheckBox(DataBinder.Eval(Container, "DataItem.sel")) %>' runat="server" />
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateColumn>
                                                                                <asp:TemplateColumn HeaderText="Cons">
                                                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="true"></ItemStyle>
                                                                                    <ItemTemplate>
                                                                                        <asp:CheckBox ID="cbx_Consulta" Checked='<%# getCheckBox(DataBinder.Eval(Container, "DataItem.consulta")) %>' runat="server" />
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateColumn>
                                                                                <asp:TemplateColumn HeaderText="Notifica">
                                                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="true"></ItemStyle>
                                                                                    <ItemTemplate>
                                                                                        <asp:CheckBox ID="cbx_Notifica" Checked='<%# getCheckBox(DataBinder.Eval(Container, "DataItem.notifica")) %>' runat="server" />
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateColumn>
                                                                                <asp:TemplateColumn HeaderText="Spedisci">
                                                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="true"></ItemStyle>
                                                                                    <ItemTemplate>
                                                                                        <asp:CheckBox ID="cbx_Spedisci" Checked='<%# getCheckBox(DataBinder.Eval(Container, "DataItem.spedisci")) %>' runat="server" />
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateColumn>
                                                                                 <asp:BoundColumn Visible="False" DataField="Sospeso" ReadOnly="True" HeaderText="Sospeso">
                                                                                </asp:BoundColumn>
                                                                           </Columns>
                                                                        </asp:DataGrid>
                                                                    </ContentTemplate>
                                                                </asp:UpdatePanel>
                                                            </div>
                                                         </asp:Panel><br />
				                                        <asp:Panel runat="server" ID="pnlRF">
                                                            <!-- intestazione rf -->
														    <table style="width:99%;margin:3px;" cellspacing="0" cellpadding="0">
																	    <tr>
																		    <td class="titolo_pnl" vAlign="middle" align="center" width="5%"><IMG src="../Images/registri.gif" border="0">
																		    </td>
																		    <td class="titolo_pnl" vAlign="middle" width="30%">RF
																		    </td>
																		    <td class="titolo_pnl" vAlign="middle" align="right" width="65%"></td>
																	    </tr>
															</table>
                                                            <!-- content rf -->
															<div style="overflow:auto;height:89px;width:99%; padding:3px;">
                                                                        <asp:UpdatePanel ID="updPanelRF" runat="server" UpdateMode="Always">
                                                                            <ContentTemplate>
                                                                                <asp:DataGrid Width="100%" ID="dg_rf" TabIndex="28" runat="server" Height="59px" AutoGenerateColumns="False"
                                                                                    CellPadding="1" BorderWidth="1px" BorderColor="Gray" >
                                                                                    <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
                                                                                    <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
                                                                                    <ItemStyle CssClass="bg_grigioN"></ItemStyle>
                                                                                    <HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
                                                                                    <Columns>
                                                                                        <asp:BoundColumn Visible="False" DataField="IDRegistro" ReadOnly="True" HeaderText="ID"></asp:BoundColumn>
                                                                                        <asp:BoundColumn DataField="Codice" ReadOnly="True" HeaderText="Codice">
                                                                                            <ItemStyle Wrap="true"></ItemStyle>
                                                                                        </asp:BoundColumn>
                                                                                        <asp:BoundColumn DataField="Descrizione" ReadOnly="True" HeaderText="Descrizione">
                                                                                            <ItemStyle Wrap="true"></ItemStyle>
                                                                                        </asp:BoundColumn>
                                                                                        <asp:BoundColumn DataField="EmailRegistro" ReadOnly="True" HeaderText="Email">
                                                                                            <ItemStyle Wrap="true"/>
                                                                                        </asp:BoundColumn>
                                                                                        <asp:BoundColumn Visible="False" DataField="IDAmministrazione" ReadOnly="True" HeaderText="IDAmm">
                                                                                        </asp:BoundColumn>
                                                                                        <asp:TemplateColumn HeaderText="Sel">
                                                                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                                                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="true"></ItemStyle>
                                                                                            <ItemTemplate>
                                                                                                <asp:CheckBox ID="cbx_Sel" AutoPostBack="true" OnCheckedChanged="cbx_Sel_CheckedChanged" Checked='<%# getCheckBox(DataBinder.Eval(Container, "DataItem.sel")) %>' runat="server" />
                                                                                            </ItemTemplate>
                                                                                        </asp:TemplateColumn>
                                                                                        <asp:TemplateColumn HeaderText="Cons">
                                                                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                                                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="true"></ItemStyle>
                                                                                            <ItemTemplate>
                                                                                                <asp:CheckBox ID="cbx_Consulta" Checked='<%# getCheckBox(DataBinder.Eval(Container, "DataItem.consulta")) %>' runat="server" />
                                                                                            </ItemTemplate>
                                                                                        </asp:TemplateColumn>
                                                                                        <asp:TemplateColumn HeaderText="Notifica">
                                                                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                                                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="true"></ItemStyle>
                                                                                            <ItemTemplate>
                                                                                                <asp:CheckBox ID="cbx_Notifica" Checked='<%# getCheckBox(DataBinder.Eval(Container, "DataItem.notifica")) %>' runat="server" />
                                                                                            </ItemTemplate>
                                                                                        </asp:TemplateColumn>
                                                                                        <asp:TemplateColumn HeaderText="Spedisci">
                                                                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                                                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="true"></ItemStyle>
                                                                                            <ItemTemplate>
                                                                                                <asp:CheckBox ID="cbx_Spedisci" Checked='<%# getCheckBox(DataBinder.Eval(Container, "DataItem.spedisci")) %>' runat="server" />
                                                                                            </ItemTemplate>
                                                                                        </asp:TemplateColumn>
                                                                                        <asp:BoundColumn DataField="Disabled" ReadOnly="True" HeaderText="" Visible="False">
                                                                                            <ItemStyle></ItemStyle>
                                                                                        </asp:BoundColumn>
                                                                                            <asp:BoundColumn DataField="AooCollegata" ReadOnly="True" HeaderText="" Visible="False">
                                                                                            <ItemStyle></ItemStyle>
                                                                                        </asp:BoundColumn>                                                      
                                                                                            </Columns>
                                                                                </asp:DataGrid>
                                                                            </ContentTemplate>
                                                                        </asp:UpdatePanel>
                                                                    </div>
														</asp:Panel>
                                                        <asp:Panel ID="pnlTitolo" runat="server">
                                                            <div class="titolo_pnl" style="width:100%;margin:3px;">
                                                                <div style="float:right; padding-right:2px;">
                                                                    <asp:button id="btn_vis_reg" tabIndex="30" runat="server" CssClass="testo_btn_org" Text="Imposta visibilità registri"></asp:button>
                                                                    &nbsp;
                                                                    <asp:button id="btn_mod_registri" tabIndex="29" runat="server" CssClass="testo_btn_org" Text="Modifica"></asp:button>
                                                                </div>
                                                            </div>
                                                        </asp:Panel>
										        </asp:panel>
                                            </td>
                                        </tr>

										<!-- Funzioni -->
                                        <asp:panel id="pnl_funzioni" runat="server" Visible="true">
											<TR>
												<TD>
													<TABLE class="contenitore" width="97%">
														<TR>
															<TD colSpan="2">
																<TABLE cellSpacing="0" cellPadding="0" width="100%" border="0">
																	<TR>
																		<TD class="titolo_pnl" vAlign="middle" align="center" width="5%"><IMG src="../Images/funzioni.gif" border="0">
																		</TD>
																		<TD class="titolo_pnl" vAlign="middle" width="30%">Funzioni:
																		</TD>
																		<TD class="titolo_pnl" vAlign="middle" align="right" width="65%"></TD>
																	</TR>
																</TABLE>
															</TD>
														</TR>
														<TR>
															<TD>
																<DIV style="OVERFLOW: auto; HEIGHT: 88px;width:100%;">
																	<asp:DataGrid id="dg_funzioni" tabIndex="31" runat="server" Height="59px" AutoGenerateColumns="False"
																		CellPadding="1" BorderWidth="1px" BorderColor="Gray">
																		<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
																		<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
																		<ItemStyle CssClass="bg_grigioN"></ItemStyle>
																		<HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
																		<Columns>
																			<asp:BoundColumn Visible="False" DataField="IDTipoFunzione" ReadOnly="True" HeaderText="ID"></asp:BoundColumn>
																			<asp:BoundColumn DataField="codice" ReadOnly="True" HeaderText="Codice">
																				<ItemStyle Width="20%"></ItemStyle>
																			</asp:BoundColumn>
																			<asp:BoundColumn DataField="descrizione" ReadOnly="True" HeaderText="Descrizione">
																				<ItemStyle Width="80%"></ItemStyle>
																			</asp:BoundColumn>
																			<asp:BoundColumn Visible="False" DataField="IDAmministrazione" ReadOnly="True" HeaderText="IDAmm"></asp:BoundColumn>
																			<asp:TemplateColumn HeaderText="Sel">
																				<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
																				<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
																				<ItemTemplate>
																					<asp:CheckBox ID="Chk_funzioni" Checked='<%# getCheckBox(DataBinder.Eval(Container, "DataItem.sel")) %>' runat="server" />
																				</ItemTemplate>
																			</asp:TemplateColumn>
																		</Columns>
																	</asp:DataGrid></DIV>
															</TD>
														</TR>
														<TR>
															<TD class="titolo_pnl" colSpan="2">
																<TABLE cellSpacing="0" cellPadding="0" align="right" border="0" width="100%">
																	<TR>
																		<TD class="titolo_pnl">
                                                                            <div>
                                                                                <div style="float:right; padding-right:2px;">
                                                                                    <asp:button id="btn_mod_funzioni" tabIndex="32" runat="server" CssClass="testo_btn_org" Text="Modifica"></asp:button>
                                                                                </div>
                                                                            </div>
																	    </TD>
																	</TR>
																</TABLE>
															</TD>
														</TR>
													</TABLE>
												</TD>
											</TR>
										</asp:panel>
										<!-- Utente -->
                                        <asp:panel id="pnl_utente" runat="server" Visible="true">
											<TR>
												<TD>
													<TABLE class="contenitore" width="100%">
														<TR>
															<TD colSpan="2">
																<TABLE cellSpacing="0" cellPadding="0" width="100%" border="0">
																	<TR>
																		<TD class="titolo_pnl" vAlign="middle" align="center" width="5%"><IMG src="../Images/utente.gif" border="0">
																		</TD>
																		<TD class="titolo_pnl" vAlign="middle" width="30%">Utente:
																		</TD>
																		<TD class="titolo_pnl" vAlign="middle" align="right" width="65%"></TD>
																	</TR>
																</TABLE>
															</TD>
														</TR>
														<TR>
															<TD class="testo_grigio_scuro" width="20%">Abilitato</TD>
															<TD align="left">
																<asp:CheckBox id="cb_abilitato" Runat="server" CssClass="testo"></asp:CheckBox></TD>
														</TR>
														<TR>
															<TD class="testo_grigio_scuro" width="20%">UserId</TD>
															<TD align="left">
																<asp:TextBox id="txt_userIdUtente" runat="server" CssClass="testo" Width="350px" MaxLength="128"></asp:TextBox></TD>
														</TR>
														<TR>
															<TD class="testo_grigio_scuro" width="20%">Cognome</TD>
															<TD align="left">
																<asp:TextBox id="txt_cognomeUtente" runat="server" CssClass="testo" Width="350px" MaxLength="128"></asp:TextBox></TD>
														</TR>
														<TR>
															<TD class="testo_grigio_scuro" width="20%">Nome</TD>
															<TD align="left">
																<asp:TextBox id="txt_nomeUtente" runat="server" CssClass="testo" Width="350px" MaxLength="128"></asp:TextBox></TD>
														</TR>
														<TR>
															<TD class="testo_grigio_scuro" width="20%">Cod. rubrica</TD>
															<TD align="left">
																<asp:TextBox id="txt_rubicaUtente" runat="server" CssClass="testo" Width="350px" MaxLength="128"></asp:TextBox></TD>
														</TR>
														<TR>
															<TD class="testo_grigio_scuro" width="20%">Email</TD>
															<TD align="left">
																<asp:TextBox id="txt_emailUtente" runat="server" CssClass="testo" Width="350px" MaxLength="128"></asp:TextBox></TD>
														</TR>
														<TR>
															<TD class="testo_grigio_scuro" width="20%">Dominio</TD>
															<TD align="left">
																<asp:TextBox id="txt_dominioUtente" runat="server" CssClass="testo" Width="350px" MaxLength="128"></asp:TextBox></TD>
														</TR>
														<TR>
															<TD class="testo_grigio_scuro" width="20%">Sede</TD>
															<TD align="left">
																<asp:TextBox id="txt_sedeUtente" runat="server" CssClass="testo" Width="350px" MaxLength="128"></asp:TextBox></TD>
														</TR>
														<TR>
															<TD class="testo_grigio_scuro" style="HEIGHT: 12px" width="20%">Notifica</TD>
															<TD style="HEIGHT: 12px" align="left">
																<asp:dropdownlist id="ddl_notifica" runat="server" CssClass="testo" Width="308px">
																	<asp:ListItem Value="null">nessuna notifica email</asp:ListItem>
																	<asp:ListItem Value="E" Selected="True">tramite email (no allegati)</asp:ListItem>
																	<asp:ListItem Value="ED">tramite email con i documenti allegati</asp:ListItem>
																</asp:dropdownlist></TD>
														</TR>
														<TR>
															<TD class="testo_grigio_scuro" width="20%">Amministratore</TD>
															<TD align="left">
																<asp:CheckBox id="cb_amm" Runat="server" CssClass="testo"></asp:CheckBox></TD>
														</TR>
														<TR>
															<TD class="titolo_pnl" colSpan="2">
																<TABLE cellSpacing="0" cellPadding="0" align="right" border="0">
																	<TR>
                                                                        <TD class="titolo_pnl">&nbsp;
																			<asp:button id="btn_gest_qual" runat="server" CssClass="testo_btn_org" Text="Gestione qualifiche" ToolTip="Gestione delle qualifiche associate all'utente"></asp:button></TD>
                                                                        <TD class="titolo_pnl">&nbsp;
																			<asp:button id="btn_gest_app" runat="server" CssClass="testo_btn_org" Text="Gestione applicazioni" ToolTip="Gestione delle applicazioni associate all'utente"></asp:button></TD>
																		<TD class="titolo_pnl">&nbsp;
																			<asp:button id="btn_sposta_utente" runat="server" CssClass="testo_btn_org" Text="Sposta" ToolTip="Sposta l'utente in un altro ruolo dell'organigramma"></asp:button></TD>
																		<TD class="titolo_pnl">&nbsp;
																			<asp:button id="btn_mod_utente" tabIndex="11" runat="server" CssClass="testo_btn_org" Text="Modifica"
																				Visible="False"></asp:button></TD>
																	</TR>
																</TABLE>
															</TD>
														</TR>
													</TABLE>
												</TD>
											</TR>
										</asp:panel></table>
								</td>
								<!-- FINE LATO DX --></tr>
						</table>
						<!-- FINE: CORPO CENTRALE --></td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
