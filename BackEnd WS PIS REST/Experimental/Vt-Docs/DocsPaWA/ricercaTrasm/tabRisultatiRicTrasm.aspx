<%@ Register TagPrefix="uc1" TagName="DataGridPagingWait" Src="../waiting/DataGridPagingWait.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Page language="c#" Codebehind="tabRisultatiRicTrasm.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.ricercaTrasm.tabRisultatiRicTrasm" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<script language="javascript" id="btn_SetTxUtAsViste_Click" event="onclick()" for="btn_SetTxUtAsViste">
			if(window.confirm('Attenzione,\nle trasmissioni ricevute che non necessitano accettazione o rifiuto\nsaranno eliminate ed impostate come già viste.\n\nConfermi?'))		
			{
				window.document.forms[0].hd1.value='1';
				window.document.body.style.cursor='wait';
			}
			else
			{
				window.document.forms[0].hd1.value='0';
			}
		</script>
		<!--
		<script type="text/javascript">
			var iframeids=["iFrame_cn"]
			var iframehide="yes"

			function resizeCaller() 
			{
				var dyniframe=new Array()
				for (i=0; i<iframeids.length; i++)
				{
				if (document.getElementById)
					resizeIframe(iframeids[i])
					//reveal iframe for lower end browsers? (see var above):
					if ((document.all || document.getElementById) && iframehide=="no")
					{
						var tempobj=document.all? document.all[iframeids[i]] : document.getElementById(iframeids[i])
						tempobj.style.display="block"
					}
				}
			}

			function resizeIframe(frameid)
			{
				if(frameid != "")
				{
				var currentfr=document.getElementById(frameid)
				if (currentfr && !window.opera)
				{
					currentfr.style.display="block"
					if (currentfr.contentDocument && currentfr.contentDocument.body.offsetHeight) //ns6 syntax
						currentfr.height = currentfr.contentDocument.body.offsetHeight; 
					else if (currentfr.Document && currentfr.Document.body.scrollHeight) //ie5+ syntax
						currentfr.height = currentfr.Document.body.scrollHeight;
					if (currentfr.addEventListener)
						currentfr.addEventListener("load", readjustIframe, false)
					else if (currentfr.attachEvent)
					{
						currentfr.detachEvent("onload", readjustIframe) // Bug fix line
						currentfr.attachEvent("onload", readjustIframe)
					}
				}
				}
			}

			function readjustIframe(loadevt) 
			{
				var crossevt=(window.event)? event : loadevt
				var iframeroot=(crossevt.currentTarget)? crossevt.currentTarget : crossevt.srcElement;
				if (iframeroot)
				{
					resizeIframe(iframeroot.id);
				}
			}

			function loadintoIframe(iframeid, url)
			{
				if (document.getElementById)
				document.getElementById(iframeid).src=url
			}

			if (window.addEventListener)
				window.addEventListener("load", resizeCaller, false)
			else if (window.attachEvent)
					window.attachEvent("onload", resizeCaller)
				else
					window.onload=resizeCaller
		</script>
		-->
		<script language="javascript">
			function ApriSmistamento() 
			{
				var args = new Object;
				args.window = window;
				
				var retValue = window.showModalDialog("../SmistaDoc/SmistaDoc_Container.aspx",
				                      args,
				                      "dialogWidth:1024px;dialogHeight:768px;status:no;resizable:no;scroll:no;dialogLeft:0;dialogTop:0;center:yes;help:no;"); 				
				
				
				top.principale.document.location.reload()
			}
			
			function StampaRisultatoRicerca()
			{				
				var args=new Object;
				args.window=window;
				
				var retValue = window.showModalDialog("../exportDati/exportDatiSelection.aspx?export=trasm",
										args,
										"dialogWidth:450px;dialogHeight:420px;status:no;resizable:no;scroll:no;center:yes;help:no;");						
                //top.principale.document.location.reload();										
			}				
			
			function svuotaTDLPage(noticeDays,oldTxt,tipoObjTrasm,isFunctionEnabled,datePost)
			{
			    var args=new Object;
				args.window=window;
				
				var retValue = window.showModalDialog("../TodoList/svuotaTDLPage.aspx?noticeDays="+noticeDays+"&oldTxt="+oldTxt+"&tipoObjTrasm="+tipoObjTrasm+"&isFunctionEnabled="+isFunctionEnabled+"&datePost="+datePost,
										args,
										"dialogWidth:500px;dialogHeight:340px;status:no;resizable:no;scroll:no;center:yes;help:no;");				
                if(retValue=="Y")
                    top.principale.document.location.reload();         
			}
			
			function WaitDataGridCallback(eventTarget,eventArgument)
			{
				document.body.style.cursor="wait";				
				document.getElementById("titolo").innerText="Ricerca in corso...";
			}
			
			function dettaglioTrasm(path)
			{
			    var args=new Object;
				args.window=window;
				
				var retValue = window.showModalDialog(path,
										args,
										"dialogWidth:580px;dialogHeight:400px;status:no;resizable:no;scroll:yes;center:yes;help:no;");				                      
				if(retValue=="Y")
				{		
				    //document.location.reload();
				    alert(path);
				    window.top.principale.iFrame_dx.document.location=path;
				    //window.top.principale.iFrame_dx.document.location=window.top.principale.iFrame_dx.document.location.href;
                }                
			}
		</script>
	</HEAD>
	<body>
		<form id="tabRisultatiRicTrasm" method="post" runat="server">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Risultati Ricerca Trasmissioni" />
			<input id="hd1" type="hidden" name="hd1" runat="server" />					
			<uc1:datagridpagingwait id="DataGridPagingWait1" runat="server"></uc1:datagridpagingwait>
			<TABLE cellSpacing="0" cellPadding="0" width="99%" align="center" border="0">												
				<asp:panel id="pnl_titolo" Runat="server">
					<TR>
						<TD class="pulsanti">
							<TABLE width="100%">
								<TR id="tr1" runat="server">
									<TD id="TD1" align="left" height="90%" runat="server">
										<asp:label id="titolo" Runat="server" CssClass="titolo_real">Elenco trasmissioni</asp:label></TD>
									<TD class="testo_grigio_scuro" vAlign="middle" align="right" height="10%">									    
								        <cc1:ImageButton id="btn_smista" runat="server" AlternateText="Smista documenti" Tipologia="DO_SMISTA"
						                        DisabledUrl="../images/smistamento/smista_disattivo.gif" ImageUrl="../images/smistamento/smista_attivo.gif"></cc1:ImageButton>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
								        <asp:ImageButton ID="btn_rimuoviTDL" Visible=true runat=server AlternateText="Rimuovi trasmissioni da COSE DA FARE"
								            ImageUrl="../images/proto/rimuoviTDL.gif" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;								    
										<asp:ImageButton ID="btn_stampa" Visible="False" Runat="server" AlternateText="Esporta il risultato della ricerca"
											ImageUrl="../images/proto/export.gif"></asp:ImageButton>&nbsp;&nbsp;
									</TD>
								</TR>
							</TABLE>
						</TD>
					</TR>
				</asp:panel>
				<asp:panel id="pnl_dt_Eff" Runat="server">
					<TR>
						<TD>
							<asp:datagrid id="dt_Eff" SkinID="datagrid" runat="server" Width="100%" OnItemCommand="ShowInfo" BorderColor="Gray"
								AutoGenerateColumns="False" BorderWidth="1px" PageSize='<%# this.GetPageSize() %>' AllowPaging="True" CellPadding="1"
								AllowCustomPaging="True" OnItemCreated="Grid_OnItemCreated">
								<SelectedItemStyle CssClass="bg_grigioSP"></SelectedItemStyle>
								<AlternatingItemStyle CssClass="bg_grigioAP"></AlternatingItemStyle>
								<ItemStyle CssClass="bg_grigioNP"></ItemStyle>
								<HeaderStyle ForeColor="White" CssClass="menu_1_bianco_dg"></HeaderStyle>
								<Columns>
									<asp:TemplateColumn Visible="False" HeaderText="Chiave">
										<ItemTemplate>
											<asp:Label id="Label6" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Chiave") %>'>
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox id="TextBox6" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Chiave") %>'>
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Trasm. il">
										<HeaderStyle Width="10%"></HeaderStyle>
										<ItemTemplate>
											<asp:Label id="Label1" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Data") %>'>
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox id="TextBox1" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Data") %>'>
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Mittente<br/>(Ruolo)">
										<HeaderStyle Width="20%"></HeaderStyle>
										<ItemTemplate>
											<asp:Label id="Label2" runat="server" Text='<%#this.GetDetails((System.Data.DataRowView)Container.DataItem)%>'>
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox id="TextBox2" runat="server" Text='<%#this.GetDetails((System.Data.DataRowView)Container.DataItem)%>'>
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Destinatario">
										<HeaderStyle Width="20%"></HeaderStyle>
										<ItemTemplate>
											<asp:Label id="Label3" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Destinatario") %>'>
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox id=TextBox3 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Destinatario") %>'>
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Scadenza">
										<HeaderStyle Width="5%"></HeaderStyle>
										<ItemTemplate>
											<asp:Label id="lblScadenza" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.DataScad") %>'>
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox id="txtScadenza" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.DataScad") %>'>
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Doc.">
										<HeaderStyle Width="10%"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center"></ItemStyle>
										<ItemTemplate>
											<asp:Label id="Label13" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.id") %>'>
											</asp:Label>
										</ItemTemplate>
									</asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Oggetto&lt;br&gt;&#160;-------&lt;br&gt;&#160;Mittente">
										<HeaderStyle Width="30%"></HeaderStyle>
										<ItemTemplate>
											<asp:Label id="Label41" runat="server" Text='<%# DocsPAWA.Utils.TruncateString(DataBinder.Eval(Container, "DataItem.InfoOggTrasm")) %>'>
                                            </asp:Label>
                                            <asp:label id="lbl_righe1" runat="server" Text='<%# ShowSeparator(DataBinder.Eval(Container, "DataItem.MittDoc")) %>'>
                                            </asp:label>
											<asp:Label id="Label191" runat="server" Text='<%# DocsPAWA.Utils.TruncateString_MittDest(DataBinder.Eval(Container, "DataItem.MittDoc")) %>'>
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox id="Textbox41" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.InfoOggTrasm") %>'>
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Oggetto" Visible="false">
										<HeaderStyle Width="30%"></HeaderStyle>
										<ItemTemplate>
											<asp:Label id="Label4" runat="server" Text='<%# DocsPAWA.Utils.TruncateString(DataBinder.Eval(Container, "DataItem.InfoOggTrasm")) %>'>
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox id="Textbox4" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.InfoOggTrasm") %>'>
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Scheda">
										<HeaderStyle Width="5%"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
										<ItemTemplate>
                                            <asp:ImageButton id="ImageButton5" runat="server" ImageUrl="../images/proto/fulmine.gif" BorderColor="DimGray" BorderWidth="0px" CommandName="Select" ToolTip="Vai al dettaglio della trasmissione">
											</asp:ImageButton>
											<asp:ImageButton id="Imagebutton4" runat="server" ImageUrl="../images/proto/dettaglio.gif" BorderColor="Gray" BorderWidth="1px" CommandName="ShowInfo" BorderStyle="Solid" AlternateText='<%# DataBinder.Eval(Container, "DataItem.segnData") %>'>
											</asp:ImageButton>
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn>
										<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
										<ItemTemplate>
											<asp:Label id=Label16 runat="server" Text=' <%# GetCheckBoxLabel((int) DataBinder.Eval(Container, "DataItem.Chiave"),(string)" " )%> '>
											</asp:Label>
										</ItemTemplate>
									</asp:TemplateColumn>
                                    <%-- column segnatura di repertorio --%>
                                    <asp:TemplateColumn HeaderText="Repertorio">
										<ItemStyle Font-Bold="true" />
                                        <ItemTemplate>
											<asp:Label id="lblRepertorioEff" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Repertorio") %>'>
											</asp:Label>
										</ItemTemplate>
									</asp:TemplateColumn>
								</Columns>
								<PagerStyle VerticalAlign="Middle" HorizontalAlign="Center" Position="TopAndBottom" BackColor="#C2C2C2"
									CssClass="menu_pager_grigio" Mode="NumericPages"></PagerStyle>
							</asp:datagrid></TD>
					</TR>
				</asp:panel><asp:panel id="pnl_dt_ric" Runat="server">
					<TR>
						<TD>
							<asp:datagrid id="dt_Ric" SkinID="datagrid" runat="server" Width="100%" BorderColor="Gray" AutoGenerateColumns="False"
								BorderWidth="1px" PageSize='<%# this.GetPageSize() %>' AllowPaging="True" CellPadding="1" AllowCustomPaging="True" OnItemCreated="Grid_OnItemCreated">
								<SelectedItemStyle CssClass="bg_grigioSP"></SelectedItemStyle>
								<AlternatingItemStyle CssClass="bg_grigioAP"></AlternatingItemStyle>
								<ItemStyle CssClass="bg_grigioNP"></ItemStyle>
								<HeaderStyle ForeColor="White" CssClass="menu_1_bianco_dg"></HeaderStyle>
								<Columns>
									<asp:TemplateColumn Visible="False" HeaderText="Chiave">
										<HeaderStyle Width="0px"></HeaderStyle>
										<ItemTemplate>
											<asp:Label id="Label5" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Chiave") %>'>
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox id="Textbox5" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Chiave") %>'>
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Trasm. il">
										<HeaderStyle Width="5%"></HeaderStyle>
										<ItemTemplate>
											<asp:Label id="Label7" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Data") %>'>
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox id="Textbox7" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Data") %>'>
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Mittente Trasm.&lt;br&gt;&#160;(Ruolo)">
										<HeaderStyle Width="150px"></HeaderStyle>
										<ItemTemplate>
											<asp:Label id="Label18" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Utente") %>'>
											</asp:Label>
											<br>
											(
											<asp:Label id="Label17" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Ruolo") %>'>
											</asp:Label>)
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox id="Textbox8" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Utente") %>'>
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn Visible="False" HeaderText="Ruolo">
										<HeaderStyle Width="10%"></HeaderStyle>
										<ItemTemplate>
											<asp:Label id="Label9" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Ruolo") %>'>
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox id="Textbox9" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Ruolo") %>'>
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Ragione">
										<HeaderStyle Width="10%"></HeaderStyle>
										<ItemTemplate>
											<asp:Label id="Label10" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Ragione") %>'>
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox id="Textbox10" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Ragione") %>'>
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Note">
										<HeaderStyle Width="20%"></HeaderStyle>
										<ItemTemplate>
											<asp:Label id="Label15" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.noteGenerali") %>'>
											</asp:Label>
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Scadenza">
										<HeaderStyle Width="5%"></HeaderStyle>
										<ItemTemplate>
											<asp:Label id="Label11" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.DataScad") %>'>
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox id="Textbox11" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.DataScad") %>'>
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Doc.">
										<HeaderStyle Width="5%"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center"></ItemStyle>
										<ItemTemplate>
											<asp:Label id="Label14" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.id") %>' >
											</asp:Label>
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Oggetto&lt;br&gt;&#160;-------&lt;br&gt;&#160;Mittente">
										<HeaderStyle Width="80px"></HeaderStyle>
										<ItemTemplate>
											<asp:Label id="Label12" runat="server" Text='<%# DocsPAWA.Utils.TruncateString(DataBinder.Eval(Container, "DataItem.InfoOggTrasm")) %>'>
											</asp:Label>
											<asp:label id="lbl_righe" runat="server" Text='<%# ShowSeparator(DataBinder.Eval(Container, "DataItem.MittDoc")) %>'></asp:label>
											<asp:Label id="Label19" runat="server" Text='<%# DocsPAWA.Utils.TruncateString_MittDest(DataBinder.Eval(Container, "DataItem.MittDoc")) %>'>
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox id="Textbox12" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.InfoOggTrasm") %>'>
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn Visible="False" HeaderText="Oggetto">
										<HeaderStyle Width="80px"></HeaderStyle>
										<ItemTemplate>
											<asp:Label id="Label8" runat="server" Text='<%# DocsPAWA.Utils.TruncateString(DataBinder.Eval(Container, "DataItem.InfoOggTrasm")) %>'>
											</asp:Label>
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Scheda">
										<HeaderStyle Width="5%"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
										<ItemTemplate>
                                            <asp:ImageButton id="ImageButton1" runat="server" ImageUrl="../images/proto/fulmine.gif" BorderColor="DimGray" BorderWidth="0px" CommandName="Select" ToolTip="Vai al dettaglio della trasmissione">
											</asp:ImageButton>
											<asp:ImageButton id="ImageButton3" runat="server" ImageUrl="../images/proto/dettaglio.gif" AlternateText='<%# DataBinder.Eval(Container, "DataItem.segnData") %>' BorderWidth="1px" BorderColor="Gray" BorderStyle="Solid" CommandName="ShowInfo">
											</asp:ImageButton>
										</ItemTemplate>
									</asp:TemplateColumn>
                                    <%-- column segnatura di repertorio --%>
                                    <asp:TemplateColumn HeaderText="Repertorio">
                                        <ItemStyle Font-Bold="true" />
										<ItemTemplate>
											<asp:Label id="lblRepertorioRic" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Repertorio") %>'>
											</asp:Label>
										</ItemTemplate>
									</asp:TemplateColumn>
								</Columns>
								<PagerStyle VerticalAlign="Middle" HorizontalAlign="Center" Position="TopAndBottom" BackColor="#C2C2C2"
									CssClass="menu_pager_grigio" Mode="NumericPages"></PagerStyle>
							</asp:datagrid></TD>
					</TR>
				</asp:panel>
				<TR id="tr2" runat="server">
					<TD class="testo_grigio_scuro" id="TD2" vAlign="middle" align="left" height="20" runat="server">&nbsp;
						<asp:label id="LabelMsg" runat="server" Visible="False">Label</asp:label></TD>
				</TR>
				<!-- -->
				<TR>
					<TD class="menu_1_bianco" height="10"></TD>
				</TR>
				<TR>
					<TD class="menu_1_bianco" style="HEIGHT: 16px" align="center" height="16"><asp:panel id="pnl_sollecito" Runat="server" Visible="False">
							<TABLE cellSpacing="0" cellPadding="0" border="0">
								<TR>
									<TD>
										<asp:Button id="sollecita_tutti" runat="server" CssClass="pulsante" Text="Sollecita tutti"></asp:Button>&nbsp;&nbsp;&nbsp;&nbsp;
									</TD>
									<TD>&nbsp;
										<asp:Button id="sollecita_sel" runat="server" CssClass="pulsante" Text="Sollecita selezionati"></asp:Button></TD>
								</TR>
							</TABLE>
						</asp:panel></TD>
				</TR>				
			</TABLE>
		</form>
	</body>
</HTML>
