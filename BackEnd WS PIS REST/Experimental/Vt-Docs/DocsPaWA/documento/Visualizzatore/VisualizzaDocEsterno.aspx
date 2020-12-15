<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VisualizzaDocEsterno.aspx.cs" Inherits="DocsPAWA.documento.Visualizzatore.VisualizzaDocEsterno" %>

<%@ Register Src="../../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<%--<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">--%>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="../../CSS/ProfilazioneDinamica.css" type="text/css" rel="stylesheet" />
    <link href="../../CSS/docspa_30.css" type="text/css" rel="stylesheet" />
    <base target="_self" />

    <script type="text/javascript">

        var myclose = false;

        function ConfirmClose() {
            //alert('X:' + event.clientX);
            //alert('Y:' + event.clientY);
            if (event.clientY < 0 && event.clientX >= document.body.clientWidth - 35) {
                setTimeout('myclose=false', 100);
                myclose = true;
            }
        }

        function HandleClose() {
            if (myclose == true) {
                //alert('X:' + event.clientX);
                //alert('Y:' + event.clientY);
                //alert('Entrato');
                PageMethods.AbandonSession();
            }
        }
    </script>

</head>
<body onbeforeunload="ConfirmClose();" onunload="HandleClose();">
    <form id="docVisualizzaFrame" runat="server">
    <asp:scriptmanager id="ScriptManager1" runat="server" enablepagemethods="true" />
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" />
    <table id="tblVisUnificata" cellpadding="0" style="height: 100%; width: 100%;">
        <tr>
            <td style="width: 95%;">
                <iframe id="iFrameVisUnificata" style="width: 100%; height: 100%;" scrolling="auto"
                    frameborder="0" visible="true" enableviewstate="false" runat="server"></iframe>
            </td>
            <td style="width: 5%; vertical-align: top; text-align: center;">
                <asp:Panel ID="pnlButton" runat="server" Height="100%" BorderStyle="Solid" BorderColor="#810d06"
                    BorderWidth="1px" ScrollBars="Auto" Visible="true">
                    <asp:DataGrid ID="grdButtons" runat="server" AutoGenerateColumns="False" ShowHeader="False"
                        GridLines="None" OnItemCommand="grdButtons_ItemCommand">
                        <Columns>
                            <asp:BoundColumn DataField="VersionId" Visible="false"></asp:BoundColumn>
                            <asp:TemplateColumn>
                                <ItemStyle HorizontalAlign="Right" />
                                <ItemTemplate>
                                    <asp:ImageButton ID="btnVisualizza" runat="server" Enabled="<%#this.IsAcquired((DocsPAWA.DocsPaWR.BaseInfoDoc) Container.DataItem)%>"
                                        ImageUrl="<%#this.GetVersionImage((DocsPAWA.DocsPaWR.BaseInfoDoc) Container.DataItem)%>"
                                        CommandName="ShowVersion" ImageAlign="Middle" ToolTip="<%# this.GetLabelTooltip((DocsPAWA.DocsPaWR.BaseInfoDoc) Container.DataItem) %>" />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn>
                                <ItemStyle HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <asp:Label ID="lblDescrizione" runat="server" CssClass="lbl_indiceAllegato" Text="<%# this.GetLabel((DocsPAWA.DocsPaWR.BaseInfoDoc) Container.DataItem) %>">
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn>
                                <ItemStyle HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <asp:ImageButton ID="btnVersioneStampabile" runat="server" Visible="<%#this.IsAcquired((DocsPAWA.DocsPaWR.BaseInfoDoc) Container.DataItem)%>"
                                        ImageUrl="<%#this.GetPrintableVersionImage()%>" ImageAlign="Middle" ToolTip="Apri versione stampabile"
                                        CommandName="ShowPrintableVersion" />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn Visible="false">
                                <ItemTemplate>
                                    <asp:Literal ID="ltlIsProto" runat="server" Text="<%#this.GetIsProto((DocsPAWA.DocsPaWR.BaseInfoDoc) Container.DataItem)%>" />
                                    <asp:Literal ID="ltlDocName" runat="server" Text="<%#this.GetName((DocsPAWA.DocsPaWR.BaseInfoDoc) Container.DataItem)%>" />
                                    <asp:Literal ID="ltlDocNumber" runat="server" Text="<%#this.GetDocNumber((DocsPAWA.DocsPaWR.BaseInfoDoc) Container.DataItem)%>" />
                                    <asp:Literal ID="ltlVersionId" runat="server" Text="<%#this.GetVersionId((DocsPAWA.DocsPaWR.BaseInfoDoc) Container.DataItem)%>" />
                                    <asp:Literal ID="ltlVersionNumber" runat="server" Text="<%#this.GetVersionNumber((DocsPAWA.DocsPaWR.BaseInfoDoc) Container.DataItem)%>" />
                                    <asp:Literal ID="ltlPath" runat="server" Text="<%#this.GetPath((DocsPAWA.DocsPaWR.BaseInfoDoc) Container.DataItem)%>" />
                                    <asp:Literal ID="ltlFileName" runat="server" Text="<%#this.GetFileName((DocsPAWA.DocsPaWR.BaseInfoDoc) Container.DataItem)%>" />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                    </asp:DataGrid>
                </asp:Panel>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
