<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportGenerator.aspx.cs"
    Inherits="DocsPAWA.popup.ReportGenerator" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="../../UserControls/AjaxMessageBox.ascx" TagName="AjaxMessageBox"
    TagPrefix="uc1" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .title
        {
            background-color: #fafafa;
            border: 1px solid #cccccc;
            width: 99%;
            text-align: center;
            font: Tahoma;
            font-size: medium;
            margin-left: 5px;
            margin-top: 5px;
            padding: 5px;
            font-family: Verdana;
            font-size: 12px;
            font-variant: small-caps;
        }
        
        .content
        {
            background-color: #fafafa;
            margin-left: 5px;
            margin-top: 5px;
            padding: 5px;
            border: 1px solid #cccccc;
            width: 97%;
        }
        
        .content_fields
        {
            background-color: #fafafa;
            margin-left: 5px;
            margin-top: 5px;
            padding: 5px;
            border: 1px solid #cccccc;
            width: 97%;
            text-align: center;
        }
        
        .datagrid
        {
            text-align: center;
            width: 70%;
            background-color: White;
            border-color: #DEDFDE;
            border-style: none;
            border-width: 1px;
            color: Black;
        }
        
        .datagrid_alternate
        {
            background-color: White;
            font-size: 10px;
            font-family: Verdana;
            text-align: center;
        }
        
        .datagrid_footer
        {
            background-color: #CCCC99;
        }
        
        .datagrid_header
        {
            background-color: #6B696B;
            color: White;
            font-weight: bold;
            font-size: 10px;
            font-family: Verdana;
            text-align: center;
        }
        
        .item
        {
            background-color: #F7F7DE;
            font-size: 10px;
            font-family: Verdana;
            text-align: center;
        }
        
        .selected_item
        {
            background-color: #CE5D5A;
            color: White;
        }
        
        .buttons
        {
            background-color: #fafafa;
            margin-left: 5px;
            margin-top: 5px;
            padding: 5px;
            border: 1px solid #cccccc;
            width: 97%;
            text-align: center;
        }
        
        .doc_content
        {
            width: 0px;
            height: 0px;
        }
        
        .drop_down_list
        {
            padding-left: 3pt;
            padding-right: 10pt;
            font-size: 12px;
            color: #666666;
            font-family: Verdana;
        }
        
        .text_box_title
        {
            width: 91%;
        }
        
        .text_box_subtitle
        {
            width: 87%;
        }
        
        .label
        {
            font-weight: bold;
            font-size: 10px;
            font-family: Verdana;
        }
        
        .field_set
        {
            width: 90%;
            text-align:left;
        }
        
        .watermark
        {
            font-style: italic;
            width: 87%;
        }
        
        .watermark_title
        {
            font-style: italic;
            width: 91%;
        }
        
        .wait_window
        {
            display: none;
            font-weight: bold;
            font-size: xx-large;
            font-family: Arial;
            text-align: center;
        }
        
        .wait_window_text
        {
            font-family: Arial;
            font-size: small;
            text-align: center;
        }
    </style>
    <script type="text/javascript" language="javascript">
        // Chiusura finestra
        function close_and_clean() {
            window.close();

        }

        // Apertura "finestra" di attesa
        function show_wait(sender, args) {
            this._popup = $find('mdlPopupWait');
            this._popup.show();
            return true;
        }

    </script>
</head>
<body>
    <form id="form1" runat="server" style="text-align:center;">
    <asp:ScriptManager runat="server" ID="ScriptManager" AsyncPostBackTimeout="360000">
    </asp:ScriptManager>
    <fieldset class="field_set">
        <legend><span class="title">Generazione report</span></legend>
        <div class="content">
            <asp:UpdatePanel runat="server" ID="upReportSelector" runat="server">
                <ContentTemplate>
                    <span class="label">Report da generare:</span>&nbsp;
                    <asp:DropDownList ID="ddlReport" runat="server" DataTextField="ReportName" DataValueField="ReportKey"
                        CssClass="drop_down_list" AutoPostBack="true" OnSelectedIndexChanged="ddlReport_SelectedIndexChanged" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div class="content">
            <span class="label">Titolo:</span>&nbsp;
            <asp:TextBox ID="txtReportTitle" runat="server" CssClass="text_box_title" AutoCompleteType="Disabled" />
            <cc1:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" runat="server" WatermarkCssClass="watermark_title"
                TargetControlID="txtReportTitle" WatermarkText="Titolo report">
            </cc1:TextBoxWatermarkExtender>
        </div>
        <div class="content">
            <span class="label">Sottotitolo:</span>&nbsp;
            <asp:TextBox ID="txtReportSubtitle" runat="server" CssClass="text_box_subtitle" AutoCompleteType="Disabled" />
            <cc1:TextBoxWatermarkExtender ID="tbwSubtitle" runat="server" WatermarkCssClass="watermark"
                TargetControlID="txtReportSubtitle">
            </cc1:TextBoxWatermarkExtender>
        </div>
        <div class="content">
            <span class="label">Formato di esportazione:</span>&nbsp;
            <asp:DropDownList ID="ddlExportFormat" runat="server" CssClass="drop_down_list">
                <asp:ListItem Text="PDF" />
                <asp:ListItem Text="Excel" />
                <asp:ListItem Text="Open Office Calc" Value="ODS" />
            </asp:DropDownList>
        </div>
        <asp:UpdatePanel runat="server" ID="upFieldsSelection" runat="server">
            <ContentTemplate>
                <asp:Panel runat="server" ID="pnlFieldsSelection" CssClass="content_fields" Visible="false">
                    <span class="label">Campi da esportare:</span>
                    <br />
                    <br />
                    <asp:DataGrid ID="dgFields" runat="server" AutoGenerateColumns="False" CellPadding="4"
                        GridLines="Vertical" CssClass="datagrid">
                        <AlternatingItemStyle CssClass="datagrid_alternate" />
                        <Columns>
                            <asp:TemplateColumn HeaderText="Nome campo">
                                <ItemTemplate>
                                    <asp:Literal ID="ltlFieldName" runat="server" Text="<%# ((DocsPAWA.DocsPaWR.HeaderProperty)Container.DataItem).ColumnName %>" />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Esporta">
                                <ItemTemplate>
                                    <asp:HiddenField ID="hfFieldName" runat="server" Value="<%# ((DocsPAWA.DocsPaWR.HeaderProperty)Container.DataItem).OriginalName %>" />
                                    <asp:CheckBox OnCheckedChanged="chkSelected_CheckedChanged" ID="chkSelected" runat="server"
                                        Checked="<%# ((DocsPAWA.DocsPaWR.HeaderProperty)Container.DataItem).Export %>" />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                        <FooterStyle CssClass="datagrid_footer" />
                        <HeaderStyle CssClass="datagrid_header" />
                        <ItemStyle CssClass="item" />
                        <SelectedItemStyle CssClass="selected_item" />
                    </asp:DataGrid>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div class="buttons">
            <asp:UpdatePanel runat="server">
                <ContentTemplate>
                    <asp:Button ID="btnExport" runat="server" Text="Esporta" OnClick="btnExport_Click" />&nbsp;
                    <asp:Button ID="btnClose" runat="server" OnClientClick="close_and_clean();" Text="Chiudi"
                        OnClick="btnClose_Click" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <br />
        <asp:UpdatePanel runat="server">
            <ContentTemplate>
                <iframe runat="server" id="reportContent" style="width: 0px; height: 0px;" />
            </ContentTemplate>
        </asp:UpdatePanel>
        <uc1:AjaxMessageBox ID="messageBox" runat="server" />
    </fieldset>
    </form>
</body>
</html>
