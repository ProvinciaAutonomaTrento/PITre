<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GridPersonalization.aspx.cs"
    Inherits="DocsPAWA.Grids.GridPersonalization" %>

<%@ Register Src="../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<meta http-equiv="Pragma" content="no-cache" />
<meta http-equiv="Expires" content="-1" />
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="../UserControls/AjaxMessageBox.ascx" TagName="AjaxMessageBox" TagPrefix="uc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .tabAlert
        {
            margin: 0px;
            padding: 0px;
            border-collapse: collapse;
            border-left: 3px solid #666666;
            border-right: 3px solid #666666;
            border-bottom: 3px solid #666666;
            background-color: #ffffff;
        }
        .tabAlert td
        {
            padding: 2px;
        }
        .tabAlert caption
        {
            font-size: 14px;
            text-align: center;
            background-color: #3399FF;
            border-left: 3px solid #666666;
            border-right: 3px solid #666666;
            border-top: 3px solid #666666;
            padding: 2px;
            border-bottom: 1px solid #666666;
        }
        
        li
        {
            list-style-type: square;
        }
        
        #top_div
        {
            margin-top: 10px;
            margin-left: 5px;
            background-color: #eaeaea;
            border: 1px solid #000000;
            margin-right: 5px;
            float: left;
            padding-bottom: 5px;
        }
        .tab_cont
        {
            background-color: #fafafa;
            margin-left: 5px;
            margin-top: 5px;
            padding: 5px;
            border: 1px solid #cccccc;
            clear: both;
            float: left;
            width: 865px;
        }
        
        .tab_griglia
        {
            background-color: #fafafa;
            clear: both;
            margin-left: 5px;
            margin-top: 5px;
            float: left;
            width: 865px;
            padding: 5px;
            border: 1px solid #cccccc;
        }
        
        #button_center
        {
            background-color: #fafafa;
            clear: both;
            margin-left: 5px;
            margin-top: 5px;
            float: left;
            width: 865px;
            margin-right: 5px;
            padding: 5px;
            border: 1px solid #cccccc;
            text-align: center;
        }
    </style>
    <script type="text/javascript" language="javascript">
        // Ogni volta che viene cambiato il colore di sfondo, viene riportato
        // il codice HEX del colore nel campo nascosto
        function setBackColor(sender, args) {
            $get('hfBackColor').value = sender.get_selectedColor();
        }



        function close_and_save() {
            var ret = document.getElementById("hid_tab_est").value;
            window.returnValue = ret;
            window.close();
        }
    </script>
</head>
<body>
    <form id="frmGridPersonalization" runat="server">
    <asp:HiddenField ID="hid_tab_est" runat="server"/>
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="Personalizza Griglia" />
    <asp:ScriptManager ID="ScriptManager" runat="server" AsyncPostBackTimeout="360000">
    </asp:ScriptManager>
    
    <div id="top_div">
        <asp:Panel ID="pnlTemplateManagement" runat="server">
            <div class="tab_cont">
                <div style="float: left; width: 15%;">
                    <span class="testo_grigio_scuro">Tipologia</span>
                </div>
                <div style="float: left; width: 50%">
                    <asp:UpdatePanel ID="upVisibleTemplates" runat="server">
                        <ContentTemplate>
                            <asp:DropDownList ID="ddlVisibleTemplates" CssClass="testo_grigio_scuro" Width="100%"
                                runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlVisibleTemplates_SelectedIndexChanged"
                                OnPreRender="ddlVisibleTemplates_PreRender" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div style="float: left; width: 10%; text-align: center;">
                    <asp:UpdatePanel ID="upAddTemplate" runat="server">
                        <ContentTemplate>
                            <asp:Button CssClass="pulsante_mini_3" ID="btnAddTemplateFields" Text="Aggiungi"
                                runat="server" Enabled="false" Visible="true" OnClick="btnAddTemplateFields_Click"
                                Width="85%" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
            <div class="tab_cont">
                <div style="float: left; width: 15%; padding-top: 20px;">
                    <span class="testo_grigio_scuro">Tipologia inserita</span>
                </div>
                <div style="float: left; width: 50%">
                    <asp:UpdatePanel ID="upAddedTemplates" runat="server">
                        <ContentTemplate>
                            <asp:ListBox ID="lstTemplates" runat="server" CssClass="testo_grigio_scuro" Width="100%"
                                OnSelectedIndexChanged="lstTemplates_SelectedIndexChanged" AutoPostBack="true"
                                OnPreRender="lstTemplates_PreRender"></asp:ListBox>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div style="float: left; width: 10%; text-align: center; margin-top: 15px;">
                    <asp:UpdatePanel ID="upRemoveTemplate" runat="server">
                        <ContentTemplate>
                            <asp:Button CssClass="pulsante_mini_3" ID="btnRemoveTemplateFields" Visible="true"
                                Text="Rimuovi" runat="server" OnClick="btnRemoveTemplateFields_Click" Enabled="false"
                                Width="85%" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlFields" runat="server" Height="300px" ScrollBars="Vertical" CssClass="tab_griglia">
            <asp:UpdatePanel ID="upFieldList" runat="server">
                <ContentTemplate>
                    <asp:GridView ID="gvFields" runat="server" AllowPaging="false" AllowSorting="false"
                        AutoGenerateColumns="false" Width="100%" SkinID="gridview">
                        <AlternatingRowStyle BackColor="White" />
                        <RowStyle BackColor="#d9d9d9" ForeColor="#333333" Font-Size="Small" />
                        <Columns>
                            <asp:TemplateField ItemStyle-Width="80%">
                                <ItemTemplate>
                                    <asp:Label ID="lblText" CssClass="testo_grigio_scuro" runat="server" Text="<%# this.GetText((DocsPAWA.DocsPaWR.Field)Container.DataItem) %>"
                                        ForeColor="<%# this.GetForeColor((DocsPAWA.DocsPaWR.Field)Container.DataItem) %>" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="20%">
                                <ItemTemplate>
                                    <asp:Button ID="lbProperties" runat="server" Text="Proprietà" CommandArgument="<%# this.GetFieldId((DocsPAWA.DocsPaWR.Field)Container.DataItem) %>"
                                        OnClick="lbProperties_Click" CssClass="pulsante_mini_2" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </ContentTemplate>
            </asp:UpdatePanel>
        </asp:Panel>
        <div class="tab_cont">
            <div style="float: left; width: 15%;">
                <span class="testo_grigio_scuro">Ordina per:</span>
            </div>
            <div style="float: left; width: 50%">
                <asp:UpdatePanel ID="upOrder" runat="server">
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlOrder" CssClass="testo_grigio_scuro" runat="server" DataTextField="Text"
                            DataValueField="Id" AppendDataBoundItems="true" Width="100%">
                            <asp:ListItem />
                        </asp:DropDownList>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div style="float: left;">
                <asp:DropDownList ID="ddlAscDesc" runat="server" CssClass="testo_grigio_scuro">
                    <asp:ListItem Text="Crescente" Value="Asc" />
                    <asp:ListItem Text="Decrescente" Value="Desc" />
                </asp:DropDownList>
            </div>
        </div>
        <div class="tab_cont">
            <div style="float: left; padding-top: 3px;">
                <span class="testo_grigio_scuro">Colore sfondo per campi non appartenenti a modello:&nbsp;</span>
            </div>
            <div style="float: left;">
                <asp:TextBox ID="txtColor" CssClass="testo_grigio_scuro" runat="server" ReadOnly="true" />&nbsp;
            </div>
            <div style="float: left; padding: 3px;">
                <asp:Panel ID="pnlSample" CssClass="testo_grigio_scuro" runat="server" Width="15px"
                    Height="15px" />
                <cc1:ColorPickerExtender ID="cpeColor" runat="server" TargetControlID="txtColor"
                    OnClientColorSelectionChanged="setBackColor" SampleControlID="pnlSample" />
            </div>
            <asp:UpdatePanel ID="upBackColor" runat="server">
                <ContentTemplate>
                    <asp:HiddenField ID="hfBackColor" runat="server" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <asp:UpdatePanel ID="upFieldProperties" runat="server">
            <ContentTemplate>
                <asp:Panel ID="pnlFieldProperties" runat="server" Width="100%" Visible="false">
                    <div class="tab_cont">
                        <table width="99%">
                            <tr>
                                <td colspan="2" id="tr1" runat="server" align="center">
                                    <asp:Label ID="lblTitle" CssClass="testo_grigio_scuro" runat="server">Proprietà del campo</asp:Label>
                                    <asp:HiddenField ID="hfFieldId" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblLabel" runat="server" CssClass="testo_grigio_scuro">Etichetta</asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtLabel" runat="server" CssClass="testo_grigio_scuro"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lbl1" runat="server" CssClass="testo_grigio_scuro">Numero massimo di caratteri:</asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtLength" CssClass="testo_grigio_scuro" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lbl2" runat="server" CssClass="testo_grigio_scuro">Larghezza colonna:</asp:Label>
                                </td>
                                <td>
                                    <asp:DropDownList CssClass="testo_grigio_scuro" ID="ddlWidth" runat="server">
                                        <asp:ListItem Selected="True">100</asp:ListItem>
                                        <asp:ListItem>200</asp:ListItem>
                                        <asp:ListItem>300</asp:ListItem>
                                        <asp:ListItem>400</asp:ListItem>
                                        <asp:ListItem>600</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td class="testo_grigio_scuro">
                                    <asp:CheckBox runat="server" CssClass="testo_grigio_scuro" Text="Visibile" ID="chkVisible" />
                                </td>
                                <td>
                                    <asp:Label ID="lbl_ordinamento" runat="server" CssClass="testo_grigio_scuro">Ordinamento: </asp:Label>
                                    <asp:ImageButton ID="btnUp" runat="server" AlternateText="Sposta giù" OnClick="btnUp_Click"
                                        ImageUrl="../AdminTool/Images/up.bmp" />
                                    &nbsp;
                                    <asp:ImageButton ID="btnDown" runat="server" AlternateText="Sposta su" OnClick="btnDown_Click"
                                        ImageUrl="../AdminTool/Images/down.bmp" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div id="button_center">
            <asp:UpdatePanel ID="upButtons" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Button CssClass="pulsante_mini_4" ID="btnSaveGridSettings" runat="server" Text="Salva Impostazione"
                        OnClick="btnSaveGridSettings_Click" OnClientClick="close_and_save();" />
                    &nbsp;
                    <asp:Button CssClass="pulsante_mini_3" ID="btnClose" runat="server" Text="Chiudi"
                        OnClientClick="window.close();" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <uc1:AjaxMessageBox ID="ambMessageBox" runat="server" />
    </div>
    </form>
</body>
</html>
