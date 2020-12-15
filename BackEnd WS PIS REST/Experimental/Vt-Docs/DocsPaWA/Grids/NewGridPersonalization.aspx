<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NewGridPersonalization.aspx.cs"
    Inherits="DocsPAWA.Grids.NewGridPersonalization" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<meta http-equiv="Pragma" content="no-cache" />
<meta http-equiv="Expires" content="-1" />
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Personalizza la griglia di ricerca</title>
    <link href="../CSS/docspa_30.css" type="text/css" rel="stylesheet" />
    <script type="text/javascript" language="javascript">
        function close_and_save() {
            var ret = document.getElementById("hid_tab_est").value;
            window.returnValue = ret;
            window.close();
        }
        function setFocus() {
            document.getElementById("btn_imposta").focus();
        }
        function showWait(sender, args) {
            // Viene visualizzato il popup di wait    
            this._popup = $find('mdlPopupWait');
            this._popup.show();

        }
        function hideWait(sender, args) {
            this._popup = $find('mdlPopupWait');
            this._popup.hide();
        }
    </script>
    <style type="text/css">
        body
        {
            background-color: #eaeaea;
            font-family: Verdana;
        }
        #content
        {
            background-color: #fafafa;
            border: 1px solid #cccccc;
            margin-left: 5px;
            margin-right: 5px;
            margin-bottom: 5px;
            margin-top: 5px;
            float: left;
            overflow-y: scroll;
        }
        #order
        {
            margin: 0px;
            padding: 0px;
            border-bottom: 1px solid #cccccc;
            margin-left: 19px;
            float: left;
            width: 573px;
            margin-top: 20px;
            padding-bottom: 5px;
        }
        #content_center_dx h2
        {
            margin: 0px;
            padding: 0px;
            margin-bottom: 5px;
            margin-top: 5px;
            color: #666666;
            font-weight: bold;
            font-size: 11px;
        }
        #order p
        {
            margin: 0px;
            padding: 0px;
            color: #666666;
            font-weight: bold;
            font-size: 11px;
            padding-top: 2px;
        }
        #content_center
        {
            margin-left: 5px;
            float: left;
            width: 925px;
            margin-top: 10px;
        }
        #content_center h2
        {
            margin: 0px;
            padding: 0px;
            margin-bottom: 5px;
            margin-top: 5px;
            float: left;
            width: 100%;
            color: #666666;
            font-weight: bold;
            font-size: 11px;
        }
        #box_fields
        {
            height: 550px;
            float: left;
            width: 550px;
            background-color: #ffffff;
            border: 1px solid #cccccc;
            overflow-y: scroll;
        }
        #up_down_box
        {
            float: right;
            width: 325px;
            text-align: center;
        }
        #final_buttons
        {
            clear: both;
            text-align: center;
            margin-top: 10px;
            margin-bottom: 10px;
            padding-top: 10px;
        }
        .tab_check
        {
            margin: 0px;
            padding: 0px;
            border: collapse;
            border: 0px;
        }
        .tab_check td
        {
            border: 0px;
            margin: 0px;
            padding-left: 4px;
            padding-right: 4px;
            padding-top: 1px;
            padding-bottom: 1px;
            border-right: 1px dotted #cccccc;
            border-bottom: 1px dotted #cccccc;
        }
        .tab_check tr.RowOverFirst
        {
            cursor: pointer;
        }
        .tab_check tr.RowOverFirst a:link
        {
            color: #333333;
        }
        .tab_check tr.RowOverFirst a:visited
        {
            color: #333333;
        }
        
        .tab_check tr.RowOverSelected
        {
            background-color: #f0f0f0;
            color: #333333;
            cursor: pointer;
        }
        .link_field a:link
        {
            text-decoration: none;
            font-size: 11px;
            padding-top: 3px;
            padding-bottom: 3px;
            color: #333333;
        }
        .link_field a:visited
        {
            text-decoration: none;
            font-size: 11px;
            padding-top: 3px;
            padding-bottom: 3px;
            color: #333333;
        }
        .link_field a:hover
        {
            text-decoration: none;
            font-size: 11px;
            padding-top: 3px;
            padding-bottom: 3px;
            color: #333333;
        }
        .link_field
        {
            text-decoration: none;
            font-size: 11px;
            padding-top: 3px;
            padding-bottom: 3px;
            color: #333333;
        }
        #content_center_sx
        {
            float: left;
            width: 600px;
            margin-bottom: 10px;
        }
        #content_center_dx
        {
            float: right;
            width: 325px;
        }
        #value_fields
        {
            float: right;
            width: 325px;
            text-align: center;
            margin-top: 23px;
            margin-bottom: 5px;
        }
        #value_fields
        {
            background-color: #ffffff;
            border: 1px solid #cccccc;
        }
        .tab_sx_f
        {
            text-align: left;
            width: 140px;
        }
        .tab_dx_f
        {
            text-align: left;
        }
        #box_type_document
        {
            float: right;
            width: 325px;
            text-align: center;
            background-color: #ffffff;
            height: 454px;
            border: 1px solid #cccccc;
            overflow-y: scroll;
            margin-bottom: 10px;
        }
        #upButtons
        {
            float: left;
            width: 35px;
        }
        .selected_check
        {
            background-color: #f0f0f0;
            color: #ffffff;
        }
        
        .no_selected_check a:link
        {
            color: #333333;
        }
        .no_selected_check a:visited
        {
            color: #333333;
        }
        #chkAllPanel
        {
            float: left;
            width: 100%;
        }
        .hidden
        {
            display: none;
        }
        .tabella
        {
            width: 100%;
            border-collapse: collapse;
            border: 1px solid #800000;
        }
        .modalBackground
        {
            background-color: Gray;
            filter: alpha(opacity=50);
            opacity: 0.5;
        }
        .text
        {
            height: 8px;
        }
        .modalBackground
        {
            background-color: Gray;
            filter: alpha(opacity=70);
            opacity: 0.7;
        }
        .modalPopup
        {
            background-color: #ffffdd;
            border-width: 3px;
            border-style: solid;
            border-color: Gray;
            padding: 3px;
            text-align: center;
        }
        .tab_pro
        {
            height: 65px;
        }
    </style>
</head>
<body>
    <form id="frmGridPersonalization" runat="server">
    <asp:HiddenField ID="hid_tab_est" runat="server" />
    <asp:ScriptManager ID="ScriptManager" runat="server" AsyncPostBackTimeout="360000">
    </asp:ScriptManager>
    <div id="content">
        <div id="order">
            <div style="float: left; width: 80px;">
                <p>
                    Ordina per:</p>
            </div>
            <div style="float: left; width: 50%">
                <asp:UpdatePanel ID="upOrder" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlOrder" CssClass="testo_grigio_scuro" runat="server" DataTextField="Text"
                            DataValueField="Id" AppendDataBoundItems="true" Width="100%" OnSelectedIndexChanged="ChangeFieldOrder"
                            AutoPostBack="true">
                            <asp:ListItem />
                        </asp:DropDownList>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div style="float: left;">
                <asp:UpdatePanel ID="upOrder2" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlAscDesc" runat="server" CssClass="testo_grigio_scuro" OnSelectedIndexChanged="ChangeAscDescOrder"
                            AutoPostBack="true">
                            <asp:ListItem Text="Crescente" Value="Asc" />
                            <asp:ListItem Text="Descrescente" Value="Desc" />
                        </asp:DropDownList>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
        <div id="content_center">
            <div id="content_center_sx">
                <asp:UpdatePanel ID="chkAllPanel" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div style="float: left; width: 50%;">
                            <h2 style="margin-left: 35px;">
                                Scelta dei campi da visualizzare</h2>
                        </div>
                        <div style="float: right; text-align: right; margin-right: 9px;">
                            <asp:CheckBox ID="checkAll" TextAlign="left" Text="Seleziona tutti" CssClass="testo_grigio"
                                runat="server" AutoPostBack="true" OnCheckedChanged="SelectDeselectAll" onclick="showWait()" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="upButtons" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div style="margin-bottom: 10px; margin-top: 60px;">
                            <cc1:ImageButton ID="btn_up_field" runat="server" Text="Sposta su" OnClick="UpField"
                                ToolTip="Sposta su" Width="30" DisabledUrl="~/App_Themes/ImgComuni/up_grid_disabled.gif" />
                        </div>
                        <cc1:ImageButton ID="btn_down_field" runat="server" Text="Sposta giù" OnClick="DownField"
                            ToolTip="Sposta giù" Width="30" DisabledUrl="~/App_Themes/ImgComuni/down_grid_disabled.gif" />
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="box_fields" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:HiddenField ID="selectedFieldPosition" runat="server" />
                        <asp:DataGrid ID="gridField" runat="server" AllowPaging="false" CssClass="tab_check"
                            AutoGenerateColumns="false" ShowFooter="false" ShowHeader="true" Width="100%"
                            CellPadding="0" CellSpacing="0" OnItemCreated="DataGridFieldCreated">
                            <HeaderStyle HorizontalAlign="Left" Font-Size="11px" CssClass="headTab" Font-Bold="true" />
                            <ItemStyle CssClass="RowOverFirst" />
                            <Columns>
                                <asp:TemplateColumn Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="SYSTEM_ID" runat="server" Text='<%# this.GetFieldID((DocsPAWA.DocsPaWR.Field)Container.DataItem) %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn ItemStyle-Width="5%" ItemStyle-HorizontalAlign="center" HeaderText="Visibile"
                                    HeaderStyle-Height="15">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkSelected" runat="server" AutoPostBack="true" OnCheckedChanged="ChangeCheckClick"
                                            Checked='<%# this.GetChecked((DocsPAWA.DocsPaWR.Field)Container.DataItem) %>'
                                            Enabled='<%# this.GetEnable((DocsPAWA.DocsPaWR.Field)Container.DataItem) %>' />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn ItemStyle-Width="95%" ItemStyle-CssClass="link_field" HeaderText="Nome campo"
                                    HeaderStyle-Height="15">
                                    <ItemTemplate>
                                        <asp:LinkButton runat="server" Text='<%# this.GetText((DocsPAWA.DocsPaWR.Field)Container.DataItem) %>'
                                            ToolTip='<%# this.GetText((DocsPAWA.DocsPaWR.Field)Container.DataItem) %>' ID="linkField"
                                            OnClick="SelectRowField" Width="100%"></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                            </Columns>
                        </asp:DataGrid>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div id="content_center_dx">
                <asp:UpdatePanel ID="value_fields" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table width="100%" align="center" class="tab_pro">
                            <tr>
                                <td colspan="2" id="tr1" runat="server" align="center" style="height: 15px; font-size: 11px;
                                    font-weight: bold;">
                                    <asp:Label ID="lblTitle" runat="server">Proprietà del campo</asp:Label>
                                    <asp:HiddenField ID="hfFieldId" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tab_sx_f">
                                    <asp:Label ID="lblEtichetta" runat="server" CssClass="testo_grigio_scuro" Visible="false">Etichetta:</asp:Label>
                                </td>
                                <td class="tab_dx_f">
                                    <asp:TextBox ID="txtLabel" runat="server" CssClass="testo_grigio_scuro" Width="170"
                                        Visible="false"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="tab_sx_f">
                                    <asp:Label ID="lblLarghezza" runat="server" CssClass="testo_grigio_scuro" Visible="false">Larghezza colonna:</asp:Label>
                                </td>
                                <td class="tab_dx_f">
                                    <asp:DropDownList CssClass="testo_grigio_scuro" ID="ddlWidth" runat="server" Visible="false">
                                        <asp:ListItem Selected="True">100</asp:ListItem>
                                        <asp:ListItem>200</asp:ListItem>
                                        <asp:ListItem>300</asp:ListItem>
                                        <asp:ListItem>400</asp:ListItem>
                                        <asp:ListItem>600</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <h2>
                    <asp:Label ID="lbl_type_template" runat="server"></asp:Label></h2>
                <asp:UpdatePanel ID="box_type_document" runat="server">
                    <ContentTemplate>
                        <asp:DataGrid ID="gridTemplates" runat="server" AllowPaging="false" CssClass="tab_check"
                            AutoGenerateColumns="false" ShowFooter="false" ShowHeader="true" Width="100%"
                            CellPadding="0" CellSpacing="0">
                            <HeaderStyle HorizontalAlign="Left" Font-Size="11px" CssClass="headTab" Font-Bold="true" />
                            <Columns>
                                <asp:TemplateColumn Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="SYSTEM_ID_TEMPLATE" runat="server" Text='<%# this.GetTemplateID((DocsPAWA.DocsPaWR.Templates)Container.DataItem) %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn ItemStyle-Width="5%" ItemStyle-HorizontalAlign="center" HeaderText="Mostra"
                                    HeaderStyle-Height="15">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkSelectedTemplate" runat="server" OnCheckedChanged="ChangeCheckClickTemplate"
                                            Checked='<%# this.GetTemplateVisible((DocsPAWA.DocsPaWR.Templates)Container.DataItem) %>'
                                            AutoPostBack="true" onclick="showWait()" />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn ItemStyle-Width="95%" ItemStyle-CssClass="link_field" ItemStyle-HorizontalAlign="left"
                                    HeaderText="Tipologia" HeaderStyle-Height="15">
                                    <ItemTemplate>
                                        <asp:Label ID="Label1" runat="server" Text='<%# this.GetNameTemplate((DocsPAWA.DocsPaWR.Templates)Container.DataItem) %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                            </Columns>
                        </asp:DataGrid>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
        <div id="final_buttons">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Button CssClass="pulsante_mini_4" ID="btn_imposta" runat="server" Text="Imposta"
                        OnClick="btnSaveGridSettings_Click" />
                    &nbsp;
                    <asp:Button CssClass="pulsante_mini_4" ID="btn_annulla" runat="server" Text="Annulla"
                        OnClientClick="window.close();" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    <!-- PopUp Wait-->
    <cc2:ModalPopupExtender ID="mdlPopupWait" runat="server" TargetControlID="Wait" PopupControlID="Wait"
        BackgroundCssClass="modalBackground" />
    <div id="Wait" runat="server" style="display: none; font-weight: normal; font-size: 13px;
        font-family: Arial; text-align: center;">
        <asp:UpdatePanel ID="pnlUP" runat="server">
            <ContentTemplate>
                <div class="modalPopup">
                    <asp:Label ID="lblInfo" runat="server">Attendere prego...</asp:Label>
                    <br />
                    <img id="imgLoading" src="../images/loading.gif" style="border-width: 0px;" alt="Attendere prego" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
