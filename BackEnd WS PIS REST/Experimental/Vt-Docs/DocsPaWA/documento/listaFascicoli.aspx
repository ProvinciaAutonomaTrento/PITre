<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Page language="c#" Codebehind="listaFascicoli.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.documento.listaFascicoliDiClassifica" %>
<%@ Register TagPrefix="uc1" TagName="DataGridPagingWait" Src="../waiting/DataGridPagingWait.ascx" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD runat="server">
		<title></title>
<meta content="Microsoft Visual Studio 7.0" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><LINK href="../CSS/docspA_30.css" type=text/css rel=stylesheet >
<script type=text/javascript>
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

            function WaitDataGridCallback(eventTarget,eventArgument)
			{
				document.body.style.cursor="wait";				
				document.getElementById("lbl_tipoLista").innerText="Ricerca in corso...";
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
</HEAD>
<body text="#660066" vLink="#ff3366" aLink="#cc0066" link="#660066" 
MS_POSITIONING="GridLayout">
<form id="tabRisultatiRicFasc" method="post" runat="server">
<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Lista Fascicoli" />
<table height="100%" cellSpacing="0" cellPadding="0" width="100%" align="center" border="0">
    <tr>
	    <td height="1"><uc1:datagridpagingwait id="DataGridPagingWait1" runat="server"></uc1:datagridpagingwait></td>
    </tr>
    <tr>
        <td>
      <table cellSpacing="0" cellPadding="0" width="100%" border="0">
        <TR>
          <TD height="2"></TD>
        </TR>
        <tr vAlign="middle">
        <asp:panel id="pnl_title" Visible="True" Runat="server">
          <TD class="infoDT" id="TD1" vAlign="middle" align="center" height="20" runat="server">
            <asp:label id="lbl_tipoLista" runat="server" Width="80%" CssClass="titolo_rosso">Fascicoli di classifica</asp:label>
            <asp:imagebutton id="btn_deleteADL" Visible="False" Runat="server" ImageUrl="../images/proto/cancella_area_lavoro.gif" AlternateText="Elimina Fascicolo selezionato da Area Lavoro"></asp:imagebutton>
            <asp:imagebutton id="btn_insertAll" Visible="False" Runat="server" ImageUrl="../images/classificaADL.gif" AlternateText="Classifica Documento nei Fascicoli selezionati"></asp:imagebutton>
          </TD>
        </asp:panel>
        </tr>
        <tr>
			<td><p align="left"><asp:checkbox id="chkSelectDeselectAll" runat="server" AutoPostBack="True" CssClass="testo_grigio_scuro" Text="Seleziona / deseleziona tutti" Checked="False" Visible="True"></asp:checkbox></p>
			</td>
        </tr>

        <TR vAlign=top>
          <TD class="testo_grigio_scuro" id="Td5" style="HEIGHT: 12px" align="center" 
          height="12" runat="server">&nbsp; <asp:label id="LabelMsg" runat="server" Visible="False">Label</asp:label></TD></TR>
        <tr vAlign=top>
          <td vAlign=middle>
            <DIV style="OVERFLOW: auto; HEIGHT: 240px"><asp:datagrid id="DataGrid1" runat="server" SkinID="datagrid" Width="100%" AllowSorting="True" BorderStyle="Inset" AutoGenerateColumns="False" CellPadding="1" BorderWidth="1px" BorderColor="Gray" AllowPaging="True" AllowCustomPaging="true" DataMember="element1" PageSize="7" OnItemCreated="DataGrid1_ItemCreated" >
											<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
											<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
											<ItemStyle HorizontalAlign="Left" Height="20px" CssClass="bg_grigioN" VerticalAlign="Middle"></ItemStyle>
											<HeaderStyle Wrap="False" Height="20px" ForeColor="White" CssClass="menu_1_bianco_dg"></HeaderStyle>
											<Columns>
											    
											    <asp:TemplateColumn>
											        <HeaderStyle Width="5%" />
											        <ItemStyle HorizontalAlign="Center" />
											        <ItemTemplate>
											            <asp:CheckBox runat="server" ID="checkFasc" Checked="false" AutoPostBack="true" OnCheckedChanged = "checkFasc_OnCheckedChanged"   />
                                                        <asp:HiddenField ID="hfFascId" runat="server" Value='<%# DataBinder.Eval(Container, "DataItem.Id") %>' />
											        </ItemTemplate>
											    </asp:TemplateColumn>
											
												<asp:TemplateColumn HeaderText="Stato">
													<HeaderStyle Width="5%"></HeaderStyle>
													<ItemStyle HorizontalAlign="Center"></ItemStyle>
													<ItemTemplate>
														<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Stato") %>' ID="Label1">
														</asp:Label>
													</ItemTemplate>
													<EditItemTemplate>
														<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Stato") %>' ID="Textbox1">
														</asp:TextBox>
													</EditItemTemplate>
												</asp:TemplateColumn>
												<asp:TemplateColumn HeaderText="Tipo">
													<HeaderStyle Width="5%"></HeaderStyle>
													<ItemStyle HorizontalAlign="Center"></ItemStyle>
													<ItemTemplate>
														<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Tipo") %>' ID="Label2">
														</asp:Label>
													</ItemTemplate>
													<EditItemTemplate>
														<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Tipo") %>' ID="Textbox2">
														</asp:TextBox>
													</EditItemTemplate>
												</asp:TemplateColumn>
												<asp:TemplateColumn HeaderText="Cod Class.">
													<HeaderStyle Width="10%"></HeaderStyle>
													<ItemTemplate>
														<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.CodClass") %>' ID="Label3">
														</asp:Label>
													</ItemTemplate>
													<EditItemTemplate>
														<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.CodClass") %>' ID="Textbox3">
														</asp:TextBox>
													</EditItemTemplate>
												</asp:TemplateColumn>
												<asp:TemplateColumn HeaderText="Codice">
													<HeaderStyle Width="15%"></HeaderStyle>
													<ItemTemplate>
														<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Codice") %>' ID="Label8">
														</asp:Label>
													</ItemTemplate>
													<EditItemTemplate>
														<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Codice") %>' ID="Textbox8">
														</asp:TextBox>
													</EditItemTemplate>
												</asp:TemplateColumn>
												<asp:TemplateColumn HeaderText="Descrizione">
													<HeaderStyle Width="60%"></HeaderStyle>
													<ItemTemplate>
														<asp:Label runat="server" Text='<%# DocsPAWA.Utils.TruncateString(DataBinder.Eval(Container, "DataItem.Descrizione")) %>' ID="Label4">
														</asp:Label>
													</ItemTemplate>
													<EditItemTemplate>
														<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Descrizione") %>' ID="Textbox4">
														</asp:TextBox>
													</EditItemTemplate>
												</asp:TemplateColumn>
												<asp:TemplateColumn Visible="False" HeaderText="Apertura">
													<ItemTemplate>
														<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Apertura") %>' ID="Label5">
														</asp:Label>
													</ItemTemplate>
													<EditItemTemplate>
														<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Apertura") %>' ID="Textbox5">
														</asp:TextBox>
													</EditItemTemplate>
												</asp:TemplateColumn>
												<asp:TemplateColumn Visible="False" HeaderText="Chiusura">
													<ItemTemplate>
														<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Chiusura") %>' ID="Label6">
														</asp:Label>
													</ItemTemplate>
													<EditItemTemplate>
														<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Chiusura") %>' ID="Textbox6">
														</asp:TextBox>
													</EditItemTemplate>
												</asp:TemplateColumn>
												<asp:TemplateColumn Visible="False" HeaderText="Chiave">
													<ItemTemplate>
														<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Chiave") %>' ID="Label7">
														</asp:Label>
													</ItemTemplate>
													<EditItemTemplate>
														<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Chiave") %>' ID="Textbox7">
														</asp:TextBox>
													</EditItemTemplate>
												</asp:TemplateColumn>
												<asp:TemplateColumn Visible="False" HeaderText="SystemId">
													<ItemTemplate>
														<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.IdLegislatura") %>' ID="Label9">
														</asp:Label>
													</ItemTemplate>
													<EditItemTemplate>
														<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.IdLegislatura") %>' ID="Textbox9">
														</asp:TextBox>
													</EditItemTemplate>
												</asp:TemplateColumn>
												<asp:TemplateColumn HeaderText="Dett.">
													<HeaderStyle Width="10%"></HeaderStyle>
													<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
													<ItemTemplate>
														<asp:ImageButton id="ImageButton1" runat="server" BorderWidth="1px" BorderColor="#404040" ImageUrl="../images/proto/dettaglio.gif" CommandName="Select" AlternateText='Dettagli del fascicolo'></asp:ImageButton>
													</ItemTemplate>
												</asp:TemplateColumn>
												<asp:TemplateColumn Visible="False" HeaderText="Id">
													<ItemTemplate>
														<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Id") %>' ID="Label10">
														</asp:Label>
													</ItemTemplate>
													<EditItemTemplate>
														<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Id") %>' ID="Textbox10">
														</asp:TextBox>
													</EditItemTemplate>
												</asp:TemplateColumn>
											</Columns>
											<PagerStyle HorizontalAlign="Center" BackColor="#C2C2C2" CssClass="menu_pager_grigio" Mode="NumericPages" Position="TopAndBottom"></PagerStyle>
										</asp:datagrid></DIV></td></tr>
							<!--<tr vAlign="top" height="10">
									<td id="TD2" vAlign="top" height="10" runat="server"></td>
								</tr>--></table></td></tr>
  <TR vAlign=bottom>
    <TD vAlign=bottom align=center width="100%" height="100%" 
    ><cc1:iframewebcontrol id=iFrame_cn runat="server" BorderColor="Transparent" Frameborder="0" Scrolling="auto" Marginwidth="0" Marginheight="0" iHeight="100%" iWidth="100%"></cc1:iframewebcontrol></TD></TR></table></form>
	</body>
</HTML>
