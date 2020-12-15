<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImportPregressiDetails.aspx.cs"
    Inherits="DocsPAWA.AdminTool.Gestione_Import.ImportDetails" %>

<%@ Register Src="../../ActivexWrappers/FsoWrapper.ascx" TagName="FsoWrapper" TagPrefix="uc3" %>
<%@ Register Src="../../ActivexWrappers/AdoStreamWrapper.ascx" TagName="AdoStreamWrapper"
    TagPrefix="uc2" %>
<%@ Register Src="../../ActivexWrappers/ShellWrapper.ascx" TagName="ShellWrapper"
    TagPrefix="uc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="head" runat="server">
    <title>Report Import Pregresso</title>
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet" />
    <base target="_self" />
    <script type="text/javascript" language="javascript">
        function close_and_save(ret) {
            window.returnValue = ret;
            window.close();
        }

        function OpenFile() {
            var filePath;
            var exportUrl;
            var http;
            var applName;
            var fso;
            var folder;
            var path;

            try {
                fso = FsoWrapper_CreateFsoObject();
                path = fso.GetSpecialFolder(2).Path;
                //Modificato per richiesta cliente: exportPitre
                //filePath = path + "\\exportDocspa.xls";
                filePath = path + "\\exportPitre.xls";
                applName = "Microsoft Excel";
                exportUrl = "ExportDatiPage.aspx";
                http = CreateObject("MSXML2.XMLHTTP");
                http.Open("POST", exportUrl, false);
                http.send();
                var content = http.responseBody;

                if (content != null) {
                    AdoStreamWrapper_SaveBinaryData(filePath, content);

                    ShellWrappers_Execute(filePath);

                    self.close();
                }
            }
            catch (ex) {
                alert(ex.message.toString());
            }
        }

        // Creazione oggetto activex con gestione errore
        function CreateObject(objectType) {
            try { return new ActiveXObject(objectType); }
            catch (ex) { alert("Oggetto '" + objectType + "' non istanziato"); }
        }
    </script>
    <script src="../../LIBRERIE/rubrica.js" type="text/javascript"></script>
    <style type="text/css">
        body
        {
            font-family: Verdana;
            overflow-x: hidden;
            overflow-y: auto;
        }
        #container
        {
            float: left;
            width: 100%;
            background-color: #ffffff;
            height: 550px;
        }
        #content
        {
            border: 1px solid #cccccc;
            text-align: center;
            width: 97%;
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
            height: 450px;
            overflow-x: scroll;
            overflow-y: scroll;
        }
        .contenitore_box
        {
            text-align: left;
            font-family: Tahoma, Arial, sans-serif;
            margin: 5px;
            margin-bottom: 5px;
            background-color: #ffffff;
            padding-bottom: 5px;
            padding-left: 5px;
            padding-right: 5px;
            font-size: 12px;
        }
        .contenitore_box fieldset
        {
            border: 1px dashed #eaeaea;
            margin: 0px;
            padding: 5px;
        }
        
        .contenitore_box legend
        {
            font-size: 10px;
            font-weight: normal;
            color: #4b4b4b;
            font-family: Verdana;
            margin-left: 15px;
            font-weight: bold;
            padding-bottom: 5px;
        }
        
        .contenitore_box_due
        {
            text-align: left;
            font-family: Tahoma, Arial, sans-serif;
            margin: 5px;
            margin-bottom: 5px;
            background-color: #ffffff;
            border: 1px solid #cccccc;
            padding-bottom: 5px;
            padding-left: 5px;
            padding-right: 5px;
        }
        
        .contenitore_box_due fieldset
        {
            border: 1px dashed #eaeaea;
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
        
        #imposta
        {
            float: left;
            width: 100%;
            clear: both;
            margin-left: 10px;
            text-align: left;
            margin-bottom: 15px;
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
        .testo_grigio2
        {
            font-size: 11px;
            font-family: Verdana;
            color: #333333;
            margin-top: 5px;
        }
        .tab_period
        {
            border-collapse: collapse;
            margin: 0px;
            padding: 0px;
            width: 100%;
        }
        .tab_period td
        {
            padding: 5px;
            background-color: #ffffff;
        }
        .t1
        {
            width: 5%;
            background-color: #fbfbfb;
            border: 1px solid #cccccc;
        }
        .t2
        {
            width: 95%;
            background-color: #ffffff;
            border: 1px solid #cccccc;
        }
        .inp1
        {
            width: 20px;
            font-family: Tahoma, Arial, sans-serif;
            font-size: 11px;
            color: #333333;
        }
        .inp2
        {
            width: 50px;
            font-family: Tahoma, Arial, sans-serif;
            font-size: 11px;
            color: #333333;
        }
        .contenitore_box_due p
        {
            margin: 0px;
            font-size: 11px;
            margin-top: 5px;
            padding: 0px;
            margin-left: 8px;
            color: #333333;
        }
        .contenitore_box p
        {
            margin: 0px;
            font-size: 11px;
            margin-top: 5px;
            padding: 0px;
            margin-left: 8px;
            color: #333333;
        }
        .ddl_d
        {
            margin-left: 10px;
        }
        
        .head_tab
        {
            font-size: 10px;
            text-align: center;
            padding: 2px;
        }
        
        .tab_sx3
        {
            font-size: 11px;
            text-align: center;
            padding: 2px;
        }
        .tab_format
        {
            width: 100%;
        }
                .tab_sx3 ul
        {
            margin: 0px;
            padding: 0px;
            text-align:left;
        }
        
        .tab_sx3 li
        {
            margin: 0px;
            padding: 0px;
            list-style-type: none;
        }
    </style>
    <script type="text/javascript" language="javascript">

        function beginRequest(sender, args) {
            $find("mdlWait").show();
        }

        function endRequest(sender, args) {
            $find("mdlWait").hide();
        }
    </script>
</head>
<body>
    <form id="Form" method="post" runat="server">
    <asp:HiddenField ID="hid_tab_est" runat="server" />
    <asp:ScriptManager ID="ScriptManager" runat="server" AsyncPostBackTimeout="360000">
    </asp:ScriptManager>
    <script language="javascript" type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(beginRequest);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequest);
    </script>
    <div id="container">
        <div id="content">
            <asp:Label runat="server" ID="titlePage" Text="Report import pregresso" CssClass="title"></asp:Label>
            <div id="content_field">
                <asp:DataGrid ID="grvItems" runat="server" AllowSorting="false" AutoGenerateColumns="false"
                    AllowPaging="False" CssClass="tab_format" OnItemCreated="DataGrid_ItemCreated"
                    OnItemDataBound="ImageCreatedRender">
                    <AlternatingItemStyle BackColor="#f2f2f2" />
                    <ItemStyle BackColor="#d9d9d9" ForeColor="#333333" />
                    <Columns>
                        <asp:TemplateColumn Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="SYSTEM_ID" runat="server" Text='<%# this.GetItemID((DocsPAWA.DocsPaWR.ItemReportPregressi)Container.DataItem) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderStyle-HorizontalAlign="center" HeaderStyle-CssClass="head_tab"
                            ItemStyle-CssClass="tab_sx3" HeaderText="DATA" ItemStyle-Width="15%">
                            <ItemTemplate>
                                <asp:Label ID="DATA" runat="server" Text='<%# this.GetData((DocsPAWA.DocsPaWR.ItemReportPregressi)Container.DataItem) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="ESITO" ItemStyle-CssClass="tab_sx3" HeaderStyle-HorizontalAlign="left"
                            HeaderStyle-CssClass="head_tab" ItemStyle-Width="15%">
                            <ItemTemplate>
                                <asp:Label ID="ESITO" runat="server" Text='<%# this.GetEsito((DocsPAWA.DocsPaWR.ItemReportPregressi)Container.DataItem) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderStyle-HorizontalAlign="center" HeaderStyle-CssClass="head_tab"
                            ItemStyle-CssClass="tab_sx3" HeaderText="ERRORE" ItemStyle-Width="15%">
                            <ItemTemplate>
                                <asp:Label ID="ERR" runat="server" Text='<%# this.GetErrore((DocsPAWA.DocsPaWR.ItemReportPregressi)Container.DataItem) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderStyle-HorizontalAlign="center" HeaderStyle-CssClass="head_tab"
                            ItemStyle-CssClass="tab_sx3" HeaderText="ID DOCUMENTO" ItemStyle-Width="15%">
                            <ItemTemplate>
                                <asp:Label ID="NDOC" runat="server" Text='<%# this.GetDocumento((DocsPAWA.DocsPaWR.ItemReportPregressi)Container.DataItem) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderStyle-HorizontalAlign="center" HeaderStyle-CssClass="head_tab"
                            ItemStyle-CssClass="tab_sx3" HeaderText="NUM PROTO/ID VECCHIO DOC" ItemStyle-Width="15%">
                            <ItemTemplate>
                                <asp:Label ID="NUMPROTO" runat="server" Text='<%# this.GetNumProtoExcel((DocsPAWA.DocsPaWR.ItemReportPregressi)Container.DataItem) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderStyle-HorizontalAlign="center" HeaderStyle-CssClass="head_tab"
                            ItemStyle-CssClass="tab_sx3" HeaderText="REGISTRO" ItemStyle-Width="10%">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="REG" Text='<%# this.GetRegistro((DocsPAWA.DocsPaWR.ItemReportPregressi)Container.DataItem) %>' />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderStyle-HorizontalAlign="center" HeaderStyle-CssClass="head_tab"
                            ItemStyle-CssClass="tab_sx3" HeaderText="PROPRIETARIO" ItemStyle-Width="15%">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="Ruolo" Text='<%# this.GetUtenteRuolo((DocsPAWA.DocsPaWR.ItemReportPregressi)Container.DataItem) %>' />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderStyle-HorizontalAlign="center" HeaderStyle-CssClass="head_tab"
                            ItemStyle-CssClass="tab_sx3" HeaderText="TIPO OPERAZIONE" ItemStyle-Width="5%">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="TIPO" Text='<%# this.GetTipoOperazione((DocsPAWA.DocsPaWR.ItemReportPregressi)Container.DataItem) %>' />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderStyle-HorizontalAlign="center" HeaderStyle-CssClass="head_tab"
                            ItemStyle-CssClass="tab_sx3" HeaderText="N° ALLEGATI" ItemStyle-Width="5%">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="TIPO" Text='<%# this.GetNumeroAllegati((DocsPAWA.DocsPaWR.ItemReportPregressi)Container.DataItem) %>' />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
                </asp:DataGrid>
            </div>
        </div>
        <asp:UpdatePanel ID="up" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <table id="Table3" align="center">
                    <tr>
                        <td align="center">
                            <asp:Button ID="btn_esporta" runat="server" CssClass="cbtn" Text="Esporta" OnClick="BtnEsporta_Click">
                            </asp:Button>
                        </td>
                        <td>
                            <asp:Button ID="btn_annulla" runat="server" CssClass="cbtn" Text="Chiudi" OnClientClick="window.close();">
                            </asp:Button>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <!-- PopUp Wait-->
    <cc2:ModalPopupExtender ID="mdlPopupWait" runat="server" TargetControlID="Wait" PopupControlID="Wait"
        BackgroundCssClass="modalBackground" BehaviorID="mdlWait" />
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
    <uc1:ShellWrapper ID="shellWrapper" runat="server" />
    <uc2:AdoStreamWrapper ID="adoStreamWrapper" runat="server" />
    <uc3:FsoWrapper ID="fsoWrapper" runat="server" />
    </form>
</body>
</html>
