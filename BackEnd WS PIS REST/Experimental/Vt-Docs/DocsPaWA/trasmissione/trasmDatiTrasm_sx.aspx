<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register TagPrefix="cc2" Namespace="Utilities" Assembly="MessageBox" %>
<%@ Page language="c#" Codebehind="trasmDatiTrasm_sx.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.trasmissione.trasmDatiTrasm_sx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title>trasmDatiTrasm</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<script language="javascript" src="../LIBRERIE/ProgressMsg.js"></script>
		<script language="javascript" src="../LIBRERIE/rubrica.js"></script>
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<script language="javascript">
		    function showWait() {
		        try {
		            if (top.principale.iFrame_dx.document.getElementById('please_wait') != undefined) {
		                var divWait = top.principale.iFrame_dx.document.getElementById('please_wait');
		                var height, width;
		                divWait.style.display = '';
		                height = top.principale.iFrame_dx.document.body.offsetHeight / 2 - 90 / 2;
		                width = top.principale.iFrame_dx.document.body.offsetWidth / 2 - 350 / 2;
		                divWait.style.top = height;
		                divWait.style.left = width;
		            }
		        } catch (e) {

		        }


		    }
		
			function pageLoad()
			{
			    /*
				_id = "trasmDatiTrasm";
				InitOperationMessage(true);
				_textOperation = new Array(1);
				_textOperation["btn_SalvaCompTrasm"] = new progressMsg(_id,"Operazione in corso...","Trasmissione documento");
				*/
			}
		
		
//			function calcTesto(f,l) {
// 
//			  // riconosce il tipo di browser collegato
//			  
//			  if (document.all) {
//			    var IE = true;
//			  }
//			  if (document.layers) {
//			    var NS = true;
//			  }
//			  
//			  // versione per NetScape
//			
//			  if (NS) {
//			    StrLen = f.value.length
//			    if (StrLen > l ) {
//			      f.value = f.value.substring(0,l)
//			      CharsLeft = 0
//			      window.alert("Lunghezza Testo eccessiva: " + StrLen + " caratteri, max " + l);
//			    } else {
//			      CharsLeft = l - StrLen
//			    }
//			    document.trasmDatiTrasm_sx.clTesto.value = CharsLeft
//			  }
//			  
//			  // versione per Internet Explorer
//			  
//			  if (IE) {
//			    var maxLength = l
//			    var strLen = document.trasmDatiTrasm_sx.txt_Note.value.length
//			    if (document.trasmDatiTrasm_sx.txt_Note.value.length > maxLength) {
//			      document.trasmDatiTrasm_sx.txt_Note.value = document.trasmDatiTrasm_sx.txt_Note.value.substring(0,maxLength)
//			      cleft = 0
//			      window.alert("Lunghezza NOTE GENERALI eccessiva: " + strLen + " caratteri, max " + maxLength);
//			    } else {
//			      cleft = maxLength - strLen
//			    }
//			    document.trasmDatiTrasm_sx.clTesto.value = cleft
//			  }
//			}
			
		function ApriRubricaTrasm (ragione, tipo_oggetto)
		{
			var tipoDest = "<%=gerarchia_trasm%>";
			var r = new Rubrica();
			r.CorrType = r.Interni;
			switch (tipoDest) {
				case 'T':
					r.CallType = r.CALLTYPE_TRASM_ALL;
					break;
					
				case 'I':
					r.CallType = r.CALLTYPE_TRASM_INF;
					break;
					
				case 'S':
					r.CallType = r.CALLTYPE_TRASM_SUP;
					break;		
					
				case 'P':
					r.CallType = r.CALLTYPE_TRASM_PARILIVELLO;
					break;														
			}
			if (tipo_oggetto != null)
				r.MoreParams = "objtype=" + tipo_oggetto;
				
			var res = r.Apri(); 
		}	
		
		function ShowDialogNomeModello()
			{
				
				
				var opts = "dialogHeight:200px;dialogWidth:300px;center:yes;help:no;resizable:no;scroll:no;status:no;unadorned:yes";
				
				
				var url="../popup/nomeModello.aspx";
				var res=window.showModalDialog("../popup/nomeModello.aspx",window,"dialogHeight:200px;dialogWidth:300px;center:yes;help:no;resizable:no;scroll:no;status:no;unadorned:yes");	
		//document.getElementById("hd_nomeModello").innerText		
			}			
		</script>
		<script language="javascript" id="ibtnMoveToA_click" event="onclick()" for="ibtnMoveToA">
			window.document.body.style.cursor='wait';			
		</script>
	<!--		<script language="javascript" id="Script" event="onclick()" for="btn_rubricaDest">
			alert(id);
            alert(document.getElementById(id).value);		
		</script>-->
	</HEAD>
	<body onload="javascript:pageLoad()" leftMargin="1" topMargin="1">
		<form id="trasmDatiTrasm_sx" method="post" runat="server">
		    <input id="azione" type=hidden name="azione" runat=server /> 
		    <cc2:messagebox id="msg_Trasmetti" runat="server"></cc2:messagebox>
			<TABLE id="tbl_1" height="100%" cellSpacing="0" cellPadding="0" width="99%" border="0">
				<tr height="5%">
					<td vAlign="top" align="center" width="100%">
						<TABLE class="info" cellSpacing="0" cellPadding="0" width="100%" align="center" border="0">
							<TR height="30">
								<TD class="testo_grigio_scuro"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Ruolo&nbsp;&nbsp;</TD>
								<TD class="testo_grigio" width="100%" height="20"><asp:label id="lbl_ruolo" runat="server" CssClass="testo_grigio"></asp:label></TD>
							</TR>
						</TABLE>
					</td>
				</tr>
				<tr>
					<td height="5"></td>
				</tr>
				<tr vAlign="top" height="100%">
					<td>
						<TABLE class="contenitore" cellSpacing="0" cellPadding="0" width="100%" align="center" border="0">
							<tr vAlign="top">
								<td>&nbsp; 
									<!--infoDocumento-->
									<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="95%" align="center" border="0">
										<TR>
											<TD class="titolo_scheda" vAlign="middle" align="center" height="30"><asp:label id="lbl_titolo_doc" runat="server" CssClass="menu_1_rosso" Width="350px"></asp:label></TD>
										</TR>
										<TR>
											<td>
												<table cellSpacing="0" cellPadding="0" width="100%">
													<tr>
														<TD class="titolo_scheda" style="WIDTH: 353px" vAlign="middle"><IMG height="1" src="../images/proto/spaziatore.gif" width="6" border="0">
															<asp:label id="lbl_dataDoc" runat="server" CssClass="titolo_scheda" Width="147px">Data</asp:label></TD>
													</tr>
												</table>
											</td>
										</TR>
										<TR>
											<TD>&nbsp;
												<asp:label id="lbl_dataDocumento" CssClass="testo_grigio" width="75px" Runat="server"></asp:label><asp:label id="lbl_segnDocumento" CssClass="testo_grigio" width="260px" Runat="server"></asp:label></TD>
										</TR>
										<tr>
											<td height="5"></td>
										</tr>
										<TR>
											<TD class="titolo_scheda" vAlign="middle"><IMG height="1" src="../images/proto/spaziatore.gif" width="6" border="0">
												<asp:label id="lbl_oggettoDoc" runat="server" CssClass="titolo_scheda" Width="306px">Oggetto</asp:label></TD>
										</TR>
										<TR vAlign="top" height="40">
											<TD>&nbsp; 
												<!--<asp:Label ID="lbl_oggettoDocumento" CssClass="testo_grigio" Runat="server" width="392px"></asp:Label>--><asp:textbox id="txt_oggettoDocumento" runat="server" CssClass="testo_grigio" Width="365" ReadOnly="True"
													TextMode="MultiLine" Rows="4" Height="49px"></asp:textbox></TD>
										</TR>
                                        <tr>
		<td colspan="2" align="right" class="testo_grigio">
			caratteri disponibili:&nbsp;<input type="text" id="clTestoOgg" runat="server" name="clTestoOgg"  size="4" class="testo_grigio" readonly="readonly" />&nbsp;</td>
	</tr>
                                                
										<TR height="3">
											<TD background="../images/proto/spaziatore.gif" colSpan="2"><IMG height="1" src="../images/proto/spaziatore.gif" width="6" border="0"></TD>
										</TR>
										<tr>
											<td height="3"></td>
										</tr>
									</TABLE>
								</td>
							</tr>
							<tr>
								<td height="10"></td>
							</tr>
							<tr>
								<td bgColor="#810d06" height="1"><IMG height="1" src="../images/proto/spaziatore.gif" width="6" border="0"></td>
							</tr>
							<tr>
								<td height="5"></td>
							</tr>
							<tr>
								<td>
									<!-- INIZIO TABELLA MITTENTE-->
									<TABLE class="info_grigio" height="30" cellSpacing="0" cellPadding="0" width="95%" align="center"
										border="0">
										<tr>
											<TD class="titolo_scheda" vAlign="middle" width="90%" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Mittente 
												trasmissione</TD>
											<td vAlign="middle" align="center"><cc1:imagebutton id="btn_modMit" Width="18" AlternateText="ModificaMit" ImageUrl="../images/proto/matita.gif"
													Runat="server" Height="17" DisabledUrl="../images/proto/matita.gif" Tipologia="DO_MODIFICA_MIT_TRASM"></cc1:imagebutton></td>
										</tr>
										<TR>
											<TD class="titolo_scheda" colSpan="2" height="25"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
												<asp:label id="lbl_ruol" runat="server" Width="50px">Ruolo</asp:label><asp:textbox id="txt_ruoloTrasm" CssClass="testo_grigio" width="300px" Runat="server" ReadOnly="True"	BackColor="White"></asp:textbox></TD>
										</TR>
										<TR>
											<TD class="titolo_scheda" colSpan="2" height="25"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
												<asp:label id="lbl_ut" runat="server" Width="50px">Utente</asp:label><asp:textbox id="txt_utenteTrasm" CssClass="testo_grigio" width="300px" Runat="server" BackColor="White"></asp:textbox></TD>
										</TR>
										<asp:Panel ID="pnl_cediDiritti" runat="server" Visible="false" CssClass="titolo_scheda">
										    <TR>
										        <TD class="titolo_scheda" colSpan="2" height="25"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
										             <asp:Label runat="server" ID="lbl_chk_cediDiritti">Cedi i diritti</asp:Label>&nbsp;<asp:checkbox id="cbx_cediDiritti" name="cbx_cediDiritti" Runat="server" ToolTip="Selezionando questa opzione si perderanno i diritti di visibilità sul documento" AutoPostBack="true" OnCheckedChanged="cbx_cediDiritti_CheckedChanged"></asp:checkbox>
										        </TD>
										    </TR>
										</asp:Panel>
									</TABLE>
								</td>
							</tr>
							<tr>
								<td height="5"></td>
							</tr>
							<tr>
								<td>
									<!-- ragione di trasmissione -->
									<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="95%" align="center" border="0">
										<tr>
											<td class="titolo_scheda" vAlign="middle" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Ragione</td>
										</tr>
										<tr>
											<td><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
											    <asp:dropdownlist id="ddl_ragioni" runat="server" CssClass="testo_grigio" Width="224px" AutoPostBack="True">
											        <asp:ListItem Selected=True Text="Seleziona..." Value="-1"></asp:ListItem>
											    </asp:dropdownlist>
										    </td>
										</tr>
										<tr>
											<td height="5"></td>
										</tr>
										<tr>
											<td align="left">
												<table cellSpacing="0" cellPadding="0" width="100%" border="0">
													<tr>
														<td class="titolo_scheda" vAlign="middle" width="38%"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Destinatari 
															per codice</td>
														<td vAlign="middle" width="15%"><asp:textbox id="txt_codDest" CssClass="testo_grigio" Width="95px" Runat="server"></asp:textbox></td>
														<td vAlign="middle" align="right" width="7%"><asp:imagebutton id="ibtnMoveToA" runat="server" AlternateText="Selezione per codice inserito" ImageUrl="../images/rubrica/b_arrow_right.gif"></asp:imagebutton></td>
														<td vAlign="middle" align="right" width="20%">
															<!--<IMG id="btn_rubricaDest" onmouseover="ChangeCursorT('hand','btn_rubricaDest');" 
																	onclick="ApriRubrica('trasm','trasm');"
																height="16" alt="Seleziona i destinatari della trasmissione" src="../images/proto/rubrica.gif" width="29">-->
                                                            <cc1:imagebutton id="btn_rubricaDest" Width="30px" 
                                                                AlternateText="Seleziona da Rubrica" ImageUrl="../images/proto/rubrica.gif"
																Runat="server" Height="20px" DisabledUrl="../images/proto/rubrica.gif"></cc1:imagebutton></td>
														<td vAlign="middle" align="center" width="20%"><cc1:imagebutton id="btn_rispostaA" Width="18" AlternateText="In risposta a" ImageUrl="../images/proto/ico_risposta.gif"
																Runat="server" Height="17" DisabledUrl="../images/proto/ico_risposta.gif"></cc1:imagebutton></td>
													</tr>
												</table>
											</td>
										</tr>
									</TABLE>
								</td>
							</tr>
							<tr>
								<td height="5"></td>
							</tr>
							<tr>
								<td>
									<!-- inizio tabella descrizione -->
									<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="95%" align="center" border="0">
										<TR>
											<TD class="titolo_scheda" vAlign="middle" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Descrizione</TD>
										</TR>
										<TR>
											<TD height="25">&nbsp;
												<asp:textbox id="txt_descrizione" runat="server" CssClass="testo_grigio" Width="365" ReadOnly="True"
													TextMode="MultiLine" Rows="4" Height="49px"></asp:textbox></TD>
										</TR>
                                           <tr>
		<td colspan="2" align="right" class="testo_grigio">
			caratteri disponibili:&nbsp;<input type="text" id="clTestoDesc" runat="server" name="clTestoDesc"  size="4" class="testo_grigio" readonly="readonly" />&nbsp;</td>
	</tr>
    <tr></tr><tr>
    <td colspan="2" align="right" class="testo_grigio"></td></tr>
	</tr>
									</TABLE>
								</td>
							</tr>
							<tr>
								<td height="5"></td>
							</tr>
							<tr>
								<td>
									<!-- note -->
									<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="95%" align="center" border="0">
										<TR>
											<TD class="titolo_scheda" vAlign="middle" height="19"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Note 
												generali</TD>
										</TR>
										<TR>
											<TD height="25">&nbsp;
												<asp:textbox id="txt_Note"  runat="server" CssClass="testo_grigio"
													Width="365" TextMode="MultiLine" Rows="4" Height="60px" MaxLength="250"></asp:textbox></TD>
										</TR>
										
										 <tr>
		<td colspan="2" align="right" class="testo_grigio">
			caratteri disponibili:&nbsp;<input type="text" id="clTestoNote" runat="server" name="clTestoNote"  size="4" class="testo_grigio" readonly="readonly" />&nbsp;</td>
	</tr>
									</TABLE>
								</td>
							</tr>
							<tr>
								<td height="3"></td>
							</tr>							
						</TABLE>
						<INPUT id="hd_nomeModello" type="hidden" name="hd_nomeModello" runat="server">
					</td>
				</tr>
				<tr>
					<td>
						<!--1-->
						<!-- BOTTONIERA -->
						<TABLE id="tbl_bottoniera" cellSpacing="0" cellPadding="0" align="center" border="0">							
							<TR>
								<TD vAlign="top" height="5"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
							</TR>							
							<TR>							   
				                <TD><cc1:imagebutton id="btn_NuovaDaTempl" AlternateText="Nuova trasmissione da Modello" Thema="btn_" SkinID="modelli"
							        Runat="server" DisabledUrl="../App_Themes/ImgComuni/btn_modelli_NoAttivo.gif" Tipologia="TRAS_NUOVA_DA_TEMPL"></cc1:imagebutton></TD>					          
					            <TD><cc1:imagebutton id="btn_SalvaTemplate" AlternateText="Salva Modello" Thema="btn_" SkinID="salva_modello"
							        Runat="server" DisabledUrl="../App_Themes/ImgComuni/btn_salva_modello_NoAttivo.gif" Tipologia="TRAS_SALVA_TEMPL"></cc1:imagebutton></TD>							   
							    <TD><cc1:imagebutton id="btn_SalvaTram" AlternateText="Salva" Thema="btn_" SkinID="salva"
							        Runat="server" DisabledUrl="../App_Themes/ImgComuni/btn_salva_NoAttivo.gif"></cc1:imagebutton></TD>	
							    <TD><cc1:ImageButton ID="btn_EliminaTrasmSalvata" 
                                        AlternateText="Rimuovi la trasmissione salvata" Thema="btn_" SkinID="rimuovi_Attivo"
				                    Runat="server" DisabledUrl="../App_Themes/ImgComuni/btn_rimuovi_nonAttivo.gif" 
                                        Visible="false" onclick="btn_EliminaTrasmSalvata_Click" /></TD>							  
							    <TD><cc1:imagebutton id="btn_SalvaCompTrasm" AlternateText="Trasmetti" Thema="btn_" SkinID="trasmetti" OnClientClick="showWait();"
							        Runat="server" DisabledUrl="../App_Themes/ImgComuni/btn_trasmetti_NoAttivo.gif"></cc1:imagebutton></TD>			              				                							           
							</TR>
						</TABLE>
						<!--FINE BOTTONIERA -->
					</td>
				</tr>
			</TABLE>
		</form>
	</body>
</HTML>
