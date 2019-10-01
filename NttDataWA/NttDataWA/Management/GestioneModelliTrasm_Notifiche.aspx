<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GestioneModelliTrasm_Notifiche.aspx.cs"
    Inherits="NttDataWA.Management.GestioneModelliTrasm_Notifiche" MasterPageFile="~/MasterPages/Popup.Master" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <script language="C#" runat="server">
        public bool getCheckBox(object abilita)
        {
            string abil = abilita.ToString();
            if (abil == "true")
            {
                return true;
            }
            else
            {
                return false;
            }
        }            
    </script>
    <link href="../CSS/docspa_30.css" type="text/css" rel="stylesheet" />
    <script language="javascript">
        function SingleSelect(regex, current) {
            re = new RegExp(regex);
            for (i = 0; i < document.forms[0].elements.length; i++) {
                elm = document.forms[0].elements[i];

                if (elm.type == 'checkbox' && elm != current && re.test(elm.name)) {
                    elm.checked = false;
                }
            }
        }
    </script>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <asp:UpdatePanel ID="UpContainer" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <div id="containerStandard">
                <asp:HiddenField ID="hd_mode" runat="server" />
                <table cellspacing="0" cellpadding="0" width="95%" align="center" border="0">
                    <tr>
                        <td align="center">
                            <asp:Label ID="lbl_avviso" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:Label ID="lbl_nota" runat="server" CssClass="testo_grigio"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <div id="DivDGList">
                                <asp:GridView ID="dg_Notifiche" CssClass="tbl_rounded round_onlyextreme" runat="server"
                                    AutoGenerateColumns="False" CellPadding="1" Width="100%" 
                                    OnItemCreated="dg_Notifiche_ItemCreated" DataKeyNames="idPeople,idRuolo,tipo,idGroup"
                                    OnDataBound="dg_Notifiche_DataBound">
                                    <RowStyle CssClass="NormalRow" Height="50" />
                                    <AlternatingRowStyle CssClass="AltRow" />
                                    <PagerStyle CssClass="recordNavigator2" />
                                    <Columns>
                                        <asp:BoundField DataField="descrizione" ReadOnly="True" HeaderText="Descrizione"
                                            HtmlEncode="False">
                                            <HeaderStyle Width="80%"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:BoundField>
                                        <asp:TemplateField HeaderText="Notifica">
                                            <HeaderStyle HorizontalAlign="Center" Width="10%"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="Chk" Checked='<%# getCheckBox(DataBinder.Eval(Container, "DataItem.attivo")) %>'
                                                    Enabled='<%# getCheckBox(DataBinder.Eval(Container, "DataItem.disabled")) %>'
                                                    runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField Visible="False" DataField="idPeople" HeaderText="idPeople"></asp:BoundField>
                                        <asp:BoundField Visible="False" DataField="idRuolo" HeaderText="idRuolo"></asp:BoundField>
                                        <asp:BoundField Visible="False" DataField="tipo" HeaderText="tipo"></asp:BoundField>
                                        <asp:BoundField Visible="False" DataField="idGroup" HeaderText="idRuolo"></asp:BoundField>
                                        <asp:TemplateField HeaderText="Cessione">
                                            <HeaderStyle HorizontalAlign="Center" Width="10%"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="Chk_C" Checked='<%# getCheckBox(DataBinder.Eval(Container, "DataItem.attivoC")) %>'
                                                    runat="server" Enabled='<%# getCheckBox(DataBinder.Eval(Container, "DataItem.enabledC")) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel ID="UpdatePanelButtons" runat="server" UpdateMode="Conditional"
        ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="btn_ok" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Salva"  OnClientClick=" disallowOp('ContentPlaceHolderContent'); return __doPostBack('UpdatePanelButtons');"
                OnClick="btn_ok_Click"/>
            <cc1:CustomButton ID="btn_annulla" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Chiudi"  OnClientClick="disallowOp('ContentPlaceHolderContent'); return __doPostBack('UpdatePanelButtons');"
                OnClick="btn_annulla_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
