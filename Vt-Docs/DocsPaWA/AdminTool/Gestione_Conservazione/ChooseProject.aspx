<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChooseProject.aspx.cs" Inherits="DocsPAWA.AdminTool.Gestione_Conservazione.ChooseProject" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<%@ Register TagPrefix="cc1" Namespace="Utilities" Assembly="MessageBox" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="head" runat="server">
    <title>Seleziona uno o più formati documento</title>
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet" />
    <base target="_self" />
    <script type="text/javascript" language="javascript">
        function close_and_save(ret) {
            window.returnValue = ret;
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
    <div id="container">
        <div id="content">
            <asp:Label runat="server" ID="titlePage" Text="Seleziona un fascicolo"
                CssClass="title"></asp:Label>
            <div id="content_field">
                <div align="center">
                    <asp:DataGrid ID="grvFileType" runat="server" AllowSorting="false" AutoGenerateColumns="false"
                        AllowPaging="False" CssClass="tab_format">
                        <AlternatingItemStyle BackColor="#fefefe" />
                        <ItemStyle BackColor="#f0f0f0" ForeColor="#333333" />
                        <Columns>
                            <asp:TemplateColumn Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="SYSTEM_ID" runat="server" Text='<%# this.GetProjectID((DocsPAWA.DocsPaWR.Fascicolo)Container.DataItem) %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn ItemStyle-Width="8%" HeaderStyle-HorizontalAlign="center" HeaderStyle-CssClass="head_tab">
                                <ItemTemplate>
                                     <input name="rbl_pref" type="radio" class="testo_grigio_scuro" value='<%# this.GetProjectID((DocsPAWA.DocsPaWR.Fascicolo)Container.DataItem) %>'>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn ItemStyle-Width="20%" HeaderText="CODICE" ItemStyle-CssClass="tab_sx2"
                                HeaderStyle-HorizontalAlign="left" HeaderStyle-CssClass="head_tab">
                                <ItemTemplate>
                                    <asp:Label ID="CODEPROJECT" runat="server" Text='<%# this.GetProjectCode((DocsPAWA.DocsPaWR.Fascicolo)Container.DataItem) %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn ItemStyle-Width="37%" HeaderText="DESCRIZIONE" ItemStyle-CssClass="tab_sx"
                                HeaderStyle-HorizontalAlign="left" HeaderStyle-CssClass="head_tab">
                                <ItemTemplate>
                                    <asp:Label ID="PROJECTDESCRIPTION" runat="server" Text='<%# this.GetProjectDescription((DocsPAWA.DocsPaWR.Fascicolo)Container.DataItem) %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn ItemStyle-Width="35%" HeaderText="TITOLARIO" ItemStyle-CssClass="tab_sx"
                                HeaderStyle-HorizontalAlign="left" HeaderStyle-CssClass="head_tab">
                                <ItemTemplate>
                                    <asp:Label ID="TITOLARIO" runat="server" Text='<%# this.GetProjectTitolario((DocsPAWA.DocsPaWR.Fascicolo)Container.DataItem) %>'></asp:Label>
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
                            <asp:Button ID="btn_salva" runat="server" CssClass="cbtn" Text="Seleziona" OnClick="BtnSaveDocumentFormat_Click"></asp:Button>
                        </td>
                        <td>
                            <asp:Button ID="btn_annulla" runat="server" CssClass="cbtn" Text="Chiudi"
                                OnClientClick="window.close();"></asp:Button>
                        </td>
                    </tr>
                </table>
                   </contenttemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    </form>
</body>
</html>

