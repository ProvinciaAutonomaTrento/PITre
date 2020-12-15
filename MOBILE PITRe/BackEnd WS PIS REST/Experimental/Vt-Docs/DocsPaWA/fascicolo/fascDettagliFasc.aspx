<%@ Page language="c#" Codebehind="fascDettagliFasc.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.fascicolo.fascDettagliFasc" %>
<%@ Register TagPrefix="mytree" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls" %>
<%@ import namespace="Microsoft.Web.UI.WebControls" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<META HTTP-EQUIV="Pragma" CONTENT="no-cache">
		<LINK href="../CSS/DocsPA_30.css" type="text/css" rel="stylesheet">
		
		<script language="javascript">
		function ApriDettagliProfilazione()
		{
		    window.showModalDialog('../documento/AnteprimaProfDinModal.aspx?Chiamante=AnteprimaProfDinamicaFasc.aspx','','dialogWidth:510px;dialogHeight:400px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;');
		}
		
		
		function ApriRicercaSottoFascicoli(idfascicolo,desc)
		{
    		    var newLeft=(screen.availWidth-615);
			    var newTop=(screen.availHeight-710);	
    	        // apertura della ModalDialog
			    rtnValue = window.showModalDialog('../popup/RicercaSottoFascicoli.aspx?idfascicolo=' + idfascicolo + '&desc=' + desc,"","dialogWidth:615px;dialogHeight:440px;status:no;resizable:no;scroll:no;dialogLeft:"+newLeft+";dialogTop:"+newTop+";center:no;help:no;"); 
			   
			    if (rtnValue == "Y")
			    {
			       document.getElementById("hd_returnValueModal").value = rtnValue;
			       window.document.Form1.submit()
			    }
		   
		}
		</script>
		<base target=_self>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Dati Fascicolo" />
			<table id="tbl_contenitore" height="100%" cellSpacing="0" cellPadding="0" width="100%"
				border="0">
				<tr>
					<td vAlign="bottom">
						<table class="info" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<tr>
								<td height="5"></td>
							</tr>
							<tr vAlign="top">
								<td vAlign="top" align="center">
									<!--dati fascicolo-->
									<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="95%" align="center" border="0">
										<TR>
											<TD class="titolo_scheda" style="FONT-SIZE: 11px" vAlign="middle" align="center" colSpan="3"
												height="19">Dati fascicolo</TD>
										</TR>
										<TR>
											<TD width="55%" height="23">&nbsp;
												<asp:label id="Label1" runat="server" Width="107px" CssClass="testo_grigio">Classifica</asp:label>
												<asp:textbox id="txt_ClassFasc" runat="server" Width="128px" CssClass="testo_grigio" ReadOnly="True"
													Height="18px"></asp:textbox></TD>
											<td width="19%"><asp:label id="Label2" runat="server" Width="54px" CssClass="testo_grigio">Codice</asp:label></td>
											<td><asp:textbox id="txt_fascdesc" runat="server" Width="127px" CssClass="testo_grigio" ReadOnly="True"
													Height="18px"></asp:textbox></td>
										</TR>
										<TR>
											<TD width="55%" height="23">&nbsp;
												<asp:label id="lbl_dataAp" runat="server" Width="107px" CssClass="testo_grigio">Data apertura</asp:label><asp:textbox id="txt_fascApertura" runat="server" Width="128px" CssClass="testo_grigio" ReadOnly="True"
													Height="18px"></asp:textbox></TD>
											<td><asp:label id="lbl_dataC" runat="server" Width="100px" CssClass="testo_grigio">Data chiusura</asp:label></td>
											<td><asp:textbox id="txt_FascChiusura" runat="server" Width="127px" CssClass="testo_grigio" ReadOnly="True"
													Height="18px"></asp:textbox></td>
										</TR>
										<TR>
											<TD width="55%" height="23">&nbsp;
												<asp:label id="Label5" runat="server" Width="107px" CssClass="testo_grigio">Tipo</asp:label><asp:textbox id="txt_Fasctipo" runat="server" Width="128px" CssClass="testo_grigio" ReadOnly="True"
													Height="18px"></asp:textbox></TD>
											<td><asp:label id="Label6" runat="server" Width="35px" CssClass="testo_grigio">Stato</asp:label></td>
											<td><asp:textbox id="txt_fascStato" runat="server" Width="127px" CssClass="testo_grigio" ReadOnly="True"
													Height="18px"></asp:textbox></td>
										</TR>
										<tr>
											<TD colSpan="3" height="23">&nbsp;
												<asp:label id="Label3" runat="server" Width="106px" CssClass="testo_grigio">Descrizione</asp:label><asp:textbox id="txt_descrizione" runat="server" Width="393px" CssClass="testo_grigio" ReadOnly="True"
													Height="18px"></asp:textbox></TD>
										</tr>
										<tr>
											<TD colSpan="3" height="23">&nbsp;
												<asp:label id="Label8" runat="server" Width="106px" CssClass="testo_grigio">Note</asp:label><asp:textbox id="txt_fascnote" runat="server" Width="393px" CssClass="testo_grigio" ReadOnly="True"
													Height="18px"></asp:textbox></TD>
										</tr>
										<tr>
											<TD height="23">&nbsp;
												<asp:Label ID="lblFascicoloCartaceo" runat="server" CssClass="testo_grigio" Text="Cartaceo" Width="100px"></asp:Label>
												<asp:CheckBox ID="chkFascicoloCartaceo" runat="server" CssClass="testo_grigio" Enabled="False" />
											</TD>
											
											<TD height="23">
											    <asp:Label ID="lblFascicoloPrivato" runat="server" CssClass="testo_grigio" Text="Privato" ></asp:Label></td>
											<td>
												<asp:CheckBox ID="chkFascicoloPrivato" runat="server" CssClass="testo_grigio" Enabled="False"/>
											</TD>
											
										</tr>
										<TR>
											<TD colSpan="3" height="20">&nbsp;
												<asp:label id="lbl_lf" runat="server" Width="103px" CssClass="testo_grigio">Collocazione fisica</asp:label>&nbsp;<asp:textbox id="txt_cod_lf" runat="server" Width="80px" CssClass="testo_grigio" ReadOnly="True"
													Height="18px"></asp:textbox>&nbsp;<asp:textbox id="txt_desc_lf" runat="server" Width="165px" CssClass="testo_grigio" ReadOnly="True"
													Height="18px"></asp:textbox><asp:label id="Label9" runat="server" Width="50px" CssClass="testo_grigio">&nbsp;in data</asp:label><asp:textbox id="txt_dta_lf" runat="server" Width="93px" CssClass="testo_grigio" ReadOnly="True"
													Height="18px"></asp:textbox></TD>
										</TR>

										<asp:panel id="pnl_uffRef" Visible="False" Runat="server">
											<TR>
												<TD colSpan="3" height="20">&nbsp;
													<asp:label id="Label4" runat="server" Width="103px" CssClass="testo_grigio">Ufficio Referente</asp:label>
													<asp:textbox id="txt_cod_uff_ref" runat="server" Width="80px" CssClass="testo_grigio" ReadOnly="True"
														Height="18px"></asp:textbox>
													<asp:textbox id="txt_desc_uff_ref" runat="server" Width="165px" CssClass="testo_grigio" ReadOnly="True"
														Height="18px"></asp:textbox></TD>
											</TR>
										</asp:panel>
										
										<asp:Panel id="pnl_profilazione" Visible="false" runat="server">
										    <tr>
										        <td colSpan="3" height="20">&nbsp;
										            <asp:label id="lbl_tipoFasc" runat="server" Width="103px" CssClass="testo_grigio">Tipologia fascicolo</asp:label>
										            <asp:textbox id="txt_tipoFasc" runat="server" Width="250px" CssClass="testo_grigio" ReadOnly="True" Height="18px"></asp:textbox>&nbsp;
										            <asp:ImageButton id="img_dettagliProfilazioneFasc" AlternateText="Visualizza dettagli" Runat="server" ImageUrl="../images/proto/ico_oggettario.gif" Width="18px" Height="17px" />
												</td>
										    </tr>
										</asp:Panel>
									</TABLE>
								</td>
							</tr>
							
							<tr>
								<td height="5"></td>
							</tr>
							
							<tr>
								<td align="center">
									<table cellSpacing="0" cellPadding="0"  border="0">
										<tr>
											<TD class="item_editbox">
												<p class="boxform_item">
												<table width="98%">
												<tr>
												   <td align="center"><asp:label id="Label7" runat="server">Sotto fascicoli</asp:label></td>
												   <td style="width:20px"><asp:ImageButton id="img_cercaSottoFasc" 
                                       AlternateText="Ricerca sottofascicoli" Runat="server" 
                                       ImageUrl="../images/proto/ico_fascicolo_noattivo.gif" Width="18px" 
                                       Height="17px" onclick="img_cercaSottoFasc_Click"></asp:ImageButton></td>
												</tr>
												</table>
												
                                       </p>
											</TD>
										</tr>
										<tr>
										<td class="testo_grigio_scuro" vAlign="left" ><DIV style="OVERFLOW: auto; WIDTH: 510px;height:100px"><mytree:treeview id="Folders" runat="server" CssClass="testo_grigio" Height="100%" BorderStyle="Solid"
													ImageUrl="../images/alberi/folders/folder_chiusa.gif" SelectedImageUrl="../images/alberi/folders/folder_piena.gif" ExpandedImageUrl="../images/alberi/folders/folder_aperta.gif"
													HoverStyle="font-weight:normal;font-size:10px;color:#ffffff;text-indent:0px;font-family:Verdana, Arial, Helvetica, sans-serif;background-color:#c08682;"
													DefaultStyle="font-weight:normal;font-size:10px;color:#666666;text-indent:0px;font-family:Verdana, Arial, Helvetica, sans-serif;background-color: #d9d9d9;"
													SelectedStyle="font-weight:normal;font-size:10px;color:#ffffff;text-indent:0px;font-family:Verdana, Arial, Helvetica, sans-serif;background-color:#c08682;"
													BorderWidth="0px" NAME="Treeview1" SystemImagesPath="../images/alberi/left/" width="100%" SelectExpands="True" AutoPostBack="True"></mytree:treeview></td>
									
																					</div></tr>
																					
										<tr>
										    <td align="center"><cc1:imagebutton id="btn_aggiungi" Runat="server" Thema="btn_" SkinID="aggiungi_attivo"
										Tipologia="FASC_NEW_FOLDER" DisabledUrl="../App_Themes/ImgComuni/btn_aggiungi_nonattivo.gif" AlternateText="Aggiungi sotto fascicolo"
										autopostback="false"></cc1:imagebutton>
										    &nbsp;
										    <cc1:imagebutton id="btn_rimuovi" Runat="server" Thema="btn_" SkinID="rimuovi_attivo"
										Tipologia="FASC_DEL_FOLDER" DisabledUrl="../App_Themes/ImgComuni/btn_rimuovi_nonattivo.gif" AlternateText="Rimuovi sotto fascicolo"></cc1:imagebutton>
										    </td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<TR>
					<TD>
						<!-- BOTTONIERA -->
						<TABLE id="tbl_bottoniera" cellSpacing="0" cellPadding="0" align="center" border="0">
							<TR>
								<TD vAlign="top" height="5"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
							</TR>
							<TR>
								<!--<TD><cc1:imagebutton id="btn_insDoc" Runat="server" ImageUrl="../images/bottoniera/btnFascicola_attivo.gif"
										Tipologia="FASC_INS_DOC" DisabledUrl="../images/bottoniera/btnFascicola_nonattivo.gif" AlternateText="Inserisci documento nel sottofascicolo selezionato"></cc1:imagebutton></TD>
								<TD></TD>-->
								<TD></TD>
								<TD></TD>
							</TR>
						</TABLE>
						<!--FINE	BOTTONIERA --></TD>
				</TR>
			</table>
			<input type="hidden" ID="hd_returnValueModal" runat="server" />
		</form>
	</body>
</HTML>
