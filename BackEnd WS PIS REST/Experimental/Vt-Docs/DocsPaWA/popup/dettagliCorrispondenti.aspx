<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Page language="c#" Codebehind="dettagliCorrispondenti.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.dettagliCorrispondenti" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD runat = "server">
    <meta content="Microsoft Visual Studio 7.0" name=GENERATOR>
    <meta content=C# name=CODE_LANGUAGE>
    <meta content=JavaScript name=vs_defaultClientScript>
    <meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><LINK href="../CSS/docspa_30.css" type=text/css rel=stylesheet >
  </HEAD>
<body bottomMargin="0" leftMargin="5" topMargin="5" rightMargin="5" MS_POSITIONING="GridLayout">
<form id=dettagliCorrispondenti method=post runat="server">
<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Dettagli corrispondenti" />
<TABLE class=contenitore id="tbl_dettagliCorrispondenti" height="570" width="432" align=center border=0>
  <TR vAlign=middle height=20>
        <td class=menu_1_rosso align=center><asp:label id=Label1 runat="server">Dettagli corrispondente</asp:label></td>
  </TR>
  <tr vAlign=top>
    <td>
      <table class=info_grigio height=520 cellSpacing=0 cellPadding=0 width="99%" align=center border=0>
        <TR vAlign=middle height=70>
          <TD vAlign=middle align=center colSpan=2 height=70px>
            <div id=descCorr style="LEFT: 20px; OVERFLOW: auto; POSITION: absolute; TOP: 45px; height:70px" >
            <table cellSpacing=0 cellPadding=0 width="95%" align=center border=0>
            <tr height=6px><td></td></tr>
              <tr>
                <TD><IMG height=1 src="../images/proto/spaziatore.gif" width=3 border=0 ></TD>
                <td><cc1:imagebutton id=btn_ModMit Runat="server" DisabledUrl="../images/proto/matita.gif" Tipologia="DO_IN_MIT_MODIFICA" ImageUrl="../images/proto/matita.gif" AlternateText="Modifica"></cc1:imagebutton></td>
                <TD><IMG height=1 src="../images/proto/spaziatore.gif" width=3 border=0 ></TD>
                <td valign=top><asp:label id=lbl_nomeCorr runat="server" CssClass="testo_grigio" Width="362px" Font-Bold="True"></asp:label></td>
              </tr>
              <tr>
                <TD><IMG height=1 src="../images/proto/spaziatore.gif" width=3 border=0 ></TD>
                <td></td>
                <TD><IMG height=1 src="../images/proto/spaziatore.gif" width=3 border=0 ></TD>
                <td><asp:label id=lbl_codRubr_corr runat="server" CssClass="testo_grigio" Width="362px" Font-Bold="True" ForeColor="DarkRed"></asp:label></td>
              </tr>
              </table>
              </div>
            </TD>
          </TR>
          <tr>
            <td>
              <hr style="BORDER-TOP-STYLE: solid; BORDER-RIGHT-STYLE: solid; BORDER-LEFT-STYLE: solid; BORDER-BOTTOM-STYLE: solid" 
            width=390 color=#7d7d7d SIZE=1>
            </td>
          </tr>
          <asp:panel id="pnl_notcommon" runat="server" Visible="true">
          <TR>
            <TD colSpan=2 height=90px>
              <TABLE>
                <TR>
                  <TD><IMG src="../images/proto/spaziatore.gif" width=4 border=0> 
<asp:label id=lbl_indirizzo runat="server" Width="89px" CssClass="titolo_scheda">Indirizzo</asp:label></TD>
                  <TD>
<asp:textbox id=txt_indirizzo runat="server" Width="250px" CssClass="testo_grigio"></asp:textbox></TD>
                </TR>
                <TR>
                  <TD><IMG src="../images/proto/spaziatore.gif" width=4 border=0> 
<asp:label id=lbl_cap runat="server" Width="89px" CssClass="titolo_scheda">Cap</asp:label></TD>
                <TD>
<asp:textbox id=txt_cap runat="server" Width="168px" CssClass="testo_grigio" MaxLength="5"></asp:textbox></TD>
                </TR>
                <TR>
                  <TD><IMG src="../images/proto/spaziatore.gif" width=4 border=0> 
<asp:label id=lbl_citta runat="server" Width="89px" CssClass="titolo_scheda">Città</asp:label></TD>
                  <TD>
<asp:textbox id=txt_citta runat="server" Width="168px" CssClass="testo_grigio"></asp:textbox>&nbsp; 
<asp:label id=lbl_provincia runat="server" Width="32px" CssClass="titolo_scheda">Prov</asp:label>
<asp:textbox id=txt_provincia runat="server" Width="38px" CssClass="testo_grigio" MaxLength="2"></asp:textbox></TD>
                </TR>
                <TR>
                  <TD><IMG src="../images/proto/spaziatore.gif" width=4 border=0> 
<asp:label id="lbl_local" runat="server" Width="87px" CssClass="titolo_scheda">Localita</asp:label></TD>
                  <TD>
<asp:textbox id="txt_local" runat="server" Width="168px" CssClass="testo_grigio" MaxLength="128"></asp:textbox></TD>
                </TR>
                <TR>
                  <TD><IMG src="../images/proto/spaziatore.gif" width=4 border=0> 
<asp:label id=lbl_nazione runat="server" Width="87px" CssClass="titolo_scheda">Nazione</asp:label></TD>
                  <TD>
<asp:textbox id=txt_nazione runat="server" Width="168px" CssClass="testo_grigio"></asp:textbox></TD>
                </TR>
                <TR>
                  <TD><IMG src="../images/proto/spaziatore.gif" width=4 border=0> 
<asp:label id=lbl_telefono runat="server" Width="87px" CssClass="titolo_scheda">Telefono princ.</asp:label></TD>
                  <TD>
<asp:textbox id=txt_telefono runat="server" Width="168px" CssClass="testo_grigio"></asp:textbox></TD>
                </TR>
                <TR>
                  <TD><IMG src="../images/proto/spaziatore.gif" width=4 border=0> 
<asp:label id=lbl_telefono2 runat="server" Width="87px" CssClass="titolo_scheda">Telefono sec</asp:label></TD>
                  <TD>
<asp:textbox id=txt_telefono2 runat="server" Width="168px" CssClass="testo_grigio"></asp:textbox></TD>
                </TR>
                <TR>
                  <TD width=80><IMG src="../images/proto/spaziatore.gif" width=4 border=0> 
<asp:label id=lbl_fax runat="server" CssClass="titolo_scheda">Fax</asp:label></TD>
                  <TD>
<asp:textbox id=txt_fax runat="server" Width="168px" CssClass="testo_grigio"></asp:textbox></TD>
                </TR>
                <TR>
                  <TD width=145><IMG height=1 src="../images/proto/spaziatore.gif" width=4 border=0> 
<asp:label id=lbl_codfisc runat="server" Width="120px" CssClass="titolo_scheda">Cod. fiscale</asp:label></TD>
                  <TD>
<asp:textbox id=txt_codfisc runat="server" Width="250px" CssClass="testo_grigio" MaxLength="16"></asp:textbox></TD>
                </TR>

                <TR>
                  <TD width=145><IMG height=1 src="../images/proto/spaziatore.gif" width=4 border=0> 
<asp:label id=lbl_partita_iva runat="server" Width="120px" CssClass="titolo_scheda">P.Iva</asp:label></TD>
                  <TD>
<asp:textbox id=txt_partita_iva runat="server" Width="250px" CssClass="testo_grigio" MaxLength="11"></asp:textbox></TD>
                </TR>

              </TABLE>
            </TD>
          </TR>
          </asp:panel>
 		  <asp:panel id="PanelListaCorrispondenti" runat="server" Visible="false">
		  <TR vAlign="top">
			<TD colspan="2">
			  <DIV style="OVERFLOW: auto; HEIGHT: 270px">
			  <asp:DataGrid id="dg_listCorr" runat="server" SkinID="datagrid" Width="100%" Font-Bold="True" AutoGenerateColumns="False">
			  <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
			  <ItemStyle CssClass="bg_grigioN" Height="20px"></ItemStyle>
			  <HeaderStyle HorizontalAlign="Center" CssClass="menu_1_bianco_dg"></HeaderStyle>
			  <Columns>
			    <asp:BoundColumn DataField="CODICE" HeaderText="Codice">
				  <HeaderStyle Width="30%" Height="15px"></HeaderStyle>
				</asp:BoundColumn>
				<asp:BoundColumn DataField="DESCRIZIONE" HeaderText="Descrizione">
				  <HeaderStyle Width="70%"></HeaderStyle>
				</asp:BoundColumn>
		      </Columns>
			  </asp:DataGrid>
			  </DIV>
			</TD>
	      </TR>
		</asp:panel>
		<asp:Panel ID="pnl_email" Visible="true" runat="server">
         <TR vAlign=top>
            <TD colSpan=2>
              <TABLE border="0">
				<tr>
					<td class="titolo_scheda" style="WIDTH: 145px"><img src="../../images/proto/spaziatore.gif" width="4" border="0"><asp:label id="lbl_email" runat="server" CssClass="titolo_scheda" Width="87px">Email</asp:label>&nbsp;&nbsp;</td>
					<td>
                        <asp:textbox id="txt_email" runat="server" CssClass="testo_grigio" Width="250px"></asp:textbox>
                    </td>
                </tr>
                <tr> 
                    <td colspan="2">
                        <div id="divGridViewCaselle" runat="server">
                            <asp:GridView  ID="gvCaselle" runat="server" Width="250px" AutoGenerateColumns="False" 
                                CellPadding="1" BorderWidth="1px" BorderColor="Gray"
                                style="overflow-y:scroll;overflow-x:hidden;max-height:90px" visible="false">
                                <SelectedRowStyle CssClass="bg_grigioS"></SelectedRowStyle>
                                <AlternatingRowStyle CssClass="bg_grigioA"></AlternatingRowStyle>
                                <RowStyle CssClass="bg_grigioN"></RowStyle>
                                <HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>                                                                                      
                                <Columns>
                                    <asp:TemplateField HeaderText="SystemId" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID ="lblSystemId" Text ='<%# ((DocsPAWA.DocsPaWR.MailCorrispondente)Container.DataItem).systemId %>'></asp:Label>
                                            </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Email" ShowHeader="true" >
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="68%"></ItemStyle>
                                            <ItemTemplate>
                                                <asp:TextBox AutoPostBack="true" Width="250px" CssClass="testo_grigio" style="margin:1px;" ID="txtEmailCorr" runat="server" ToolTip='<%# ((DocsPAWA.DocsPaWR.MailCorrispondente)Container.DataItem).Email %>' Text='<%# ((DocsPAWA.DocsPaWR.MailCorrispondente)Container.DataItem).Email %>'></asp:TextBox>
                                            </ItemTemplate>
                                    </asp:TemplateField> 
                                    <asp:TemplateField HeaderText="Note E-mail" ShowHeader="true">
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                            <ItemStyle  HorizontalAlign="Center" VerticalAlign="Middle" Width="28%"></ItemStyle>
                                            <ItemTemplate>
                                                <asp:TextBox AutoPostBack="true"  Width="120px" MaxLength="20" CssClass="testo_grigio" style="margin:1px;" ID="txtNoteMailCorr" runat="server" ToolTip='<%# ((DocsPAWA.DocsPaWR.MailCorrispondente)Container.DataItem).Note %>' Text='<%# ((DocsPAWA.DocsPaWR.MailCorrispondente)Container.DataItem).Note %>'></asp:TextBox>
                                            </ItemTemplate>
                                    </asp:TemplateField> 
                                    <asp:TemplateField HeaderText="*" ShowHeader="true" >
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="4%"></ItemStyle>
                                            <ItemTemplate>
                                                <asp:RadioButton ID="rdbPrincipale" runat="server" Enabled="false" Checked='<%# TypeMailCorrEsterno(((DocsPAWA.DocsPaWR.MailCorrispondente)Container.DataItem).Principale) %>' />
                                            </ItemTemplate>
                                    </asp:TemplateField> 
                                    <asp:TemplateField HeaderText="" ShowHeader="false" Visible="false">
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="2%"></ItemStyle>
                                            <ItemTemplate>
                                                <asp:ImageButton ID="imgEliminaCasella" runat="server"  ImageUrl="~/images/proto/cancella.gif" />
                                            </ItemTemplate>
                                    </asp:TemplateField> 
                                    </Columns>
                            </asp:GridView>
                        </div>
                    </td>
				</tr>
                <TR>
                  <TD><IMG height=1 src="../images/proto/spaziatore.gif" width=4 border=0 > <asp:label id=lbl_codAOO runat="server" CssClass="titolo_scheda">Codice AOO</asp:label></TD>
                  <TD><asp:textbox id=txt_codAOO runat="server" CssClass="testo_grigio" Width="250px"></asp:textbox></TD>
                </TR>
                <TR>
                  <TD><IMG height=1 src="../images/proto/spaziatore.gif" width=4 border=0 > <asp:label id=lbl_codAmm runat="server" CssClass="titolo_scheda" Width="126px">Cod. amministrazione</asp:label></TD>
                  <TD><asp:textbox id=txt_codAmm runat="server" CssClass="testo_grigio" Width="250px"></asp:textbox></TD>
                </TR>
                <TR>
                  <TD><IMG height=1 src="../images/proto/spaziatore.gif" width=4 border=0 > <asp:label id=lbl_note runat="server" CssClass="titolo_scheda" Width="87px">Note</asp:label></TD>
                  <TD><asp:textbox id=txt_note runat="server" CssClass="testo_grigio" Width="250px"></asp:textbox></TD>
                </TR>
                <TR height=50 valign=baseline>
                  <TD align=center colSpan=2>
                  <DIV style="OVERFLOW: auto;  HEIGHT=120px; POSITION: absolute; TOP: 515px; LEFT:40px" ><asp:label id=lbl_DescIntOp runat="server" CssClass="testo_grigio" Width="300px" Font-Bold="True" BorderColor="Gainsboro"></asp:label></DIV>
                  </TD>
                </TR>
              </TABLE>
            </TD>
          </TR>
          </asp:Panel>
        </table>
      </td>
    </tr>
    <tr vAlign=baseline height=20>
      <td colSpan=2>
        <table cellSpacing=0 cellPadding=0 width="100%" border=0>
          <TR width="95%">
            <TD align=center height=20><input class=PULSANTE onclick=window.close(); type=button value=CHIUDI name=Chiudi> 
                <asp:button id=btn_CreaCorDoc ToolTip="Crea Occasionale con i dettagli Specificati" runat="server" CssClass="PULSANTE" Visible="False" Text="OCCASIONALE"></asp:button></TD>
          </TR>
        </table>
      </td>
    </tr>
  </TABLE>
</form>
</body>
</HTML>
