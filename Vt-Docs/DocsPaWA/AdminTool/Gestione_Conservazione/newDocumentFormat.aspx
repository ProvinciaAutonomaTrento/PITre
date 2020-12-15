<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="newDocumentFormat.aspx.cs"
    Inherits="DocsPAWA.AdminTool.Gestione_Conservazione.newDocumentFormat" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<%@ Register TagPrefix="cc1" Namespace="Utilities" Assembly="MessageBox" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="head" runat="server">
    <title>Seleziona uno o più formati documento</title>
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet" />
    <base target="_self" />
    <script type="text/javascript" language="javascript">
        function close_and_save() {
            var ret = document.getElementById("hid_tab_est").value;
            window.returnValue = "Y";
            window.close();
        }
    </script>
    <style type="text/css">
        body
        {
            font-family: Verdana;
        }
        #container
        {
            float: left;
            width: 100%;
            height: 360px;
        }
        #content
        {
            border: 1px solid #cccccc;
            text-align: center;
            width: 98%;
            float: left;
            margin-left: 6px;
            margin-top: 5px;
        }
        .title
        {
            margin: 0px;
            padding: 0px;
            font-size: 11px;
            font-weight: bold;
            text-align: center;
            width: 100%;
            float: left;
            padding-top: 4px;
            padding-bottom: 4px;
            background-color: #810101;
            color: #ffffff;
        }
        #content_field
        {
            width: 100%;
            float: left;
            background-color: #fbfbfb;
            height: 305px;
            padding-top: 10px;
            overflow-y: scroll;
        }
        .contenitore_box
        {
            text-align: left;
            font-family: Tahoma, Arial, sans-serif;
            margin: 5px;
            margin-bottom: 10px;
        }
        .contenitore_box fieldset
        {
            border: 1px solid #eaeaea;
            margin: 5px;
            padding: 0px;
        }
        .contenitore_box legend
        {
            font-size: 10px;
            font-weight: normal;
            color: #4b4b4b;
            font-family: Verdana;
            margin-left: 15px;
            font-weight: bold;
        }
        .contenitore_box_due
        {
            text-align: left;
            font-family: Tahoma, Arial, sans-serif;
            margin: 5px;
            margin-bottom: 10px;
        }
        .contenitore_box_due fieldset
        {
            border: 1px solid #eaeaea;
            margin: 5px;
            padding: 5px;
        }
        .contenitore_box_due legend
        {
            font-size: 10px;
            font-weight: normal;
            color: #4b4b4b;
            font-family: Verdana;
            margin-left: 15px;
            font-weight: bold;
            padding-bottom: 5px;
        }
        .cbtn
        {
            font-family: Verdana;
            font-size: 11px;
            margin: 0px;
            padding: 0px;
            padding: 2px;
            width: 120px;
            height: 25px;
            color: #ffffff;
            border: 1px solid #ffffff;
            background-image: url('../images/bg_button.gif');
        }
        .titolo_scheda
        {
            font-size: 11px;
            color: #666666;
            font-weight: bold;
        }
        .testo_grigio
        {
            font-size: 11px;
            font-family: Verdana;
            color: #333333;
        }
        .tab_format
        {
            border-collapse: collapse;
            margin: 0px;
            padding: 0px;
            width: 95%;
        }
        .tab_format td
        {
            margin: 0px;
            padding: 0px;
            padding: 2px;
            font-size: 10px;
            border: 1px solid #cccccc;
        }
        .head_tab
        {
            background-color: #810101;
            color: #ffffff;
            font-size: 12px;
            text-align: center;
        }
        .tab_sx
        {
            color: #333333;
            text-align: left;
        }
        .tab_sx2
        {
            color: #333333;
            text-align: center;
        }
        #button
        {
            margin-top: 5px;
        }
    </style>
</head>
<body>
    <form id="GridSave" method="post" runat="server">
    <asp:HiddenField ID="hid_tab_est" runat="server" />
    <asp:ScriptManager ID="ScriptManager" runat="server" AsyncPostBackTimeout="360000">
    </asp:ScriptManager>
    <script type="text/javascript">
        var scrollTop;

        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequestHandler);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);

        function BeginRequestHandler(sender, args) {
            var elem = $get("content_field");
            scrollTop = elem.scrollTop;
        }

        function EndRequestHandler(sender, args) {
            var elem = $get("content_field");
            elem.scrollTop = scrollTop;

        }

    </script>
    <div id="container">
        <div id="content">
            <asp:Label runat="server" ID="titlePage" Text="Seleziona uno o più formati documento"
                CssClass="title"></asp:Label>
            <asp:UpdatePanel ID="updateDocument" runat="server">
                <contenttemplate>
         <asp:Panel ID="content_field" runat="server" ScrollBars="Auto" >
                <div align="center">
                    <asp:DataGrid ID="grvFileType" runat="server" AllowSorting="false" AutoGenerateColumns="false"
                        AllowPaging="False" CssClass="tab_format">
                        <AlternatingItemStyle BackColor="#fefefe" />
                        <ItemStyle BackColor="#f0f0f0" ForeColor="#333333" />
                        <Columns>
                            <asp:TemplateColumn Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="SYSTEM_ID" runat="server" Text='<%# this.GetTypeID((DocsPAWA.DocsPaWR.SupportedFileType)Container.DataItem) %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn ItemStyle-Width="8%" HeaderStyle-HorizontalAlign="center" HeaderStyle-CssClass="head_tab">
                                <ItemTemplate>
                                    <asp:CheckBox runat="server" ID="check_file" OnCheckedChanged="ChangeCheckClick"
                                        AutoPostBack="true" Checked='<%# this.GetChecked((DocsPAWA.DocsPaWR.SupportedFileType)Container.DataItem) %>' />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn ItemStyle-Width="20%" HeaderText="ESTENSIONE" ItemStyle-CssClass="tab_sx2"
                                HeaderStyle-HorizontalAlign="left" HeaderStyle-CssClass="head_tab">
                                <ItemTemplate>
                                    <asp:Label ID="FILENAME" runat="server" Text='<%# this.GetTypeName((DocsPAWA.DocsPaWR.SupportedFileType)Container.DataItem) %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn ItemStyle-Width="72%" HeaderText="DESCRIZIONE" ItemStyle-CssClass="tab_sx"
                                HeaderStyle-HorizontalAlign="left" HeaderStyle-CssClass="head_tab">
                                <ItemTemplate>
                                    <asp:Label ID="FILEDESCRIPTION" runat="server" Text='<%# this.GetTypeDescription((DocsPAWA.DocsPaWR.SupportedFileType)Container.DataItem) %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                    </asp:DataGrid>
                </div>
        </div>
        <asp:UpdatePanel ID="UpdatePanelConfirm" runat="server" UpdateMode="Conditional">
            <contenttemplate>
                <table id="Table3" align="center">
                    <tr>
                        <td align="center">
                            <asp:Button ID="btn_salva" runat="server" CssClass="cbtn" Text="Imposta" OnClick="BtnSaveDocumentFormat_Click"></asp:Button>
                        </td>
                        <td>
                            <asp:Button ID="btn_annulla" runat="server" CssClass="cbtn" Text="Chiudi" OnClick="BtnCloseDocument_Click"></asp:Button>
                        </td>
                    </tr>
                </table>
                   </contenttemplate>
        </asp:UpdatePanel>
        </asp:Panel> 
        </contenttemplate> 
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
