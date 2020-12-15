<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="IntegrationAdapter.ascx.cs" Inherits="DocsPaWA.UserControls.IntegrationAdapter" %>
<asp:MultiView ID="Views" runat="server">
  <!--VIEW DI AMMINISTRAZIONE-->
  <asp:View ID="AdminView" runat="server">
    <table width="100%" cellspacing="5">
     <tr>
       <td class="testo_grigio_scuro">Servizio: 
       <asp:DropDownList ID="ddl_Adapter" CssClass="testo" runat="server" AutoPostBack="True">
       </asp:DropDownList>
       </td>
     </tr>
     <tr id="tr_AdapterDescr" runat="server" visible="false">
      <td>
      <table width="100%">
        <tr>
          <td><asp:Image id="img_Adapter" runat="server"  Width="100" Height="100" /></td>
          <td><asp:Label class="testo_grigio_scuro" ID="lbl_AdapterDescr" runat="server" /></td>
        </tr>
      </table>
      </td>
     </tr>
     <tr>
         <td>
                    <asp:DataGrid ID="dg_Config" runat="server" SkinID="datagrid" BorderColor="Gray"
                        AutoGenerateColumns="false" BorderWidth="1px" CellPadding="1" Width="100%">
                        <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
						<ItemStyle CssClass="bg_grigioN"></ItemStyle>
						<HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
                        <Columns>
                            <asp:BoundColumn Visible="false" />
                            <asp:BoundColumn DataField="Name" HeaderText="nome" ItemStyle-Width="30%"
                                ReadOnly="true" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundColumn DataField="Type" HeaderText="tipo" ItemStyle-Width="10%"
                                ReadOnly="true" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundColumn HeaderText="valore"/>
                        </Columns>
                    </asp:DataGrid>
       </td>
     </tr>
    </table>
  </asp:View>
  <!--FINE VIEW DI AMMINISTRAZIONE-->
  <!--VIEW DI INSERTMODIFY-->
  <asp:View ID="InsertModifyView" runat="server">
     <script language="javascript">
        function _<%=ClientID%>_apriRicerca() {
			var newUrl;
			url = "<%=RicercaLink%>";
			var newLeft=(screen.availWidth-615);
			var newTop=(screen.availHeight-704);	
			rtnValue = window.showModalDialog(url, "", "dialogWidth:630px;dialogHeight:450px;status:no;resizable:no;scroll:yes;dialogLeft:" + newLeft + ";dialogTop:" + newTop + ";center:no;help:no;");
            if (rtnValue == "Y") {
			    window.document.getElementById('<%= hf_selectedObject.ClientID %>').value = '1';
			    window.document.getElementById('<%= Page.Form.ClientID %>').submit();
			}
            if (rtnValue == "D") {
			    window.document.getElementById('<%= hf_disservizio.ClientID %>').value = '1';
			    window.document.getElementById('<%= Page.Form.ClientID %>').submit();
			}
        }

        function _<%=ClientID %>_reset(){
            window.document.getElementById('<%= hf_reset.ClientID %>').value = '1';
            window.document.getElementById('<%= Page.Form.ClientID %>').submit();
        }
     </script>
     <asp:HiddenField ID="hf_selectedObject" runat="server" Value="0" />
     <asp:HiddenField ID="hf_reset" runat="server" Value="0" />
     <asp:HiddenField ID="hf_disservizio" runat="server" Value="0" />
     <asp:HiddenField ID="hf_codice" runat="server" />
     <asp:HiddenField ID="hf_descrizione" runat="server" />
     <table width="100%">
       <tr>
         <td width="75px"><asp:Label ID="lbl_codice" runat="server" CssClass="titolo_scheda"/></td>
         <td>
            <asp:TextBox ID="txt_codice" runat="server"/>
            <asp:ImageButton ID="btn_cercaCodice" ToolTip="Esegui ricerca puntuale" runat="server" ImageUrl="~/images/rubrica/b_arrow_right.gif" />
         </td>
         <td rowspan="2">
            <asp:Image ID="img_disservizio" runat="server" ImageUrl="~/images/semaforo-rosso.gif" Height="30px" Visible="false"/>
         </td>
       </tr>
       <tr>
         <td width="75px"><asp:Label ID="lbl_descrizione" runat="server" CssClass="titolo_scheda" Width="25px"/></td>
         <td>
             <table>
                 <tr>
                     <td>
                       <asp:TextBox ID="txt_descrizione" runat="server" ReadOnly="true" Columns="40"/>
                     </td>
                     <td>
                       <asp:ImageButton ID="btn_cerca" runat="server" BorderWidth="0" Width="26px" Height="20px" ImageAlign="AbsBottom" ImageUrl="~/images/proto/RispProtocollo/lentePreview.gif" ToolTip="Esegui ricerca" />
                     </td>
                     <td>
                        <asp:ImageButton ID="btn_reset" ToolTip="Cancella selezione" BorderWidth="0" Width="26px" ImageAlign="AbsBottom" Height="20px" runat="server" ImageUrl="~/images/simpleClearFlag.gif"/>
                     </td>

                  </tr>
             </table>
         </td>
       </tr>
     </table>
  </asp:View>
  <!--FINE VIEW DI INSERTMODIFY-->
  <!--VIEW DI READONLY-->
  <asp:View ID="ReadOnlyView" runat="server">
     <table width="100%">
       <tr>
         <td width="75px"><asp:Label ID="lbl_ro_codice" runat="server" CssClass="titolo_scheda"/></td>
         <td>
            <asp:Label ID="lbl_ro_codice_value" runat="server"/>
         </td>
       </tr>
       <tr>
         <td width="75px"><asp:Label ID="lbl_ro_descrizione" runat="server" CssClass="titolo_scheda" /></td>
         <td>
             <asp:Label ID="lbl_ro_descrizione_value" runat="server" />
         </td>
       </tr>
     </table>
  </asp:View>
  <!--FINE VIEW DI READONLY-->
  <!--VIEW DI RICERCA-->
  <asp:View ID="RicercaView" runat="server">
    <table width="100%">
       <tr>
         <td width="75px"><asp:Label ID="lbl_ric_codice" runat="server" CssClass="titolo_scheda"/></td>
         <td>
            <asp:TextBox ID="txt_ric_codice" runat="server"/>
         </td>
       </tr>
       <tr>
         <td width="75px"><asp:Label ID="lbl_ric_descrizione" runat="server" CssClass="titolo_scheda"/></td>
         <td>
             <asp:TextBox ID="txt_ric_descrizione" runat="server" Columns="40"/>
         </td>
       </tr>
     </table>
  </asp:View>
  <!--FINE VIEW DI RICERCA-->
</asp:MultiView>
