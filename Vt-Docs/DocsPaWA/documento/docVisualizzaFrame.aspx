<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="docVisualizzaFrame.aspx.cs" Inherits="DocsPAWA.documento.docVisualizzaFrame" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >

<html>
<head id="Head1" runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<meta http-equiv="Pragma" content="no-cache">
		<LINK href="../CSS/ProfilazioneDinamica.css" type="text/css" rel="stylesheet">
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		 <base target = "_self" />
</head>
<body MS_POSITIONING="GridLayout">
    <form id="docVisualizzaFrame" runat="server">
        <asp:Panel ID="visUnificata" Visible="true" runat="server" >
            <table id="tablePadre" cellpadding="0" style="height:99%; width:100%;">
            <tr>
                <td style="width:95%">
                    <iframe id="iframeVisUnificata" style="width:100%; height:100%;" scrolling="auto" frameborder="0" runat="server" visible="true"></iframe>      
                </td>
                <td style="width:5%; vertical-align:top; text-align:center;">
                    <asp:Panel ID="pnlButton" runat="server" Height="100%" BorderStyle="Solid" BorderColor="#810d06" BorderWidth="1px" ScrollBars="Auto" Visible="true">
                        <asp:DataGrid ID="grdButtons" runat="server" AutoGenerateColumns="False" 
                            ShowHeader="False"
                            onitemcommand="grdButtons_ItemCommand" GridLines="None">
                            <Columns>
                                <asp:BoundColumn DataField="VersionId" Visible="false"></asp:BoundColumn>
                                <asp:TemplateColumn>
                                    <ItemStyle HorizontalAlign="Right" />
                                    <ItemTemplate>
                                        <asp:ImageButton ID="btnVisualizza" runat="server" 
                                            Enabled = "<%#this.IsAcquired((DocsPAWA.DocsPaWR.FileRequest) Container.DataItem)%>"
                                            ImageUrl = "<%#this.GetVersionImage((DocsPAWA.DocsPaWR.FileRequest) Container.DataItem)%>"
                                            CommandName = "ShowVersion"
                                            ImageAlign = "Middle"
                                            ToolTip = "<%# this.GetLabelTooltip((DocsPAWA.DocsPaWR.FileRequest) Container.DataItem) %>" />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn>
                                    <ItemStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblDescrizione" runat="server" 
                                                CssClass="lbl_indiceAllegato"
                                                Text = "<%# this.GetLabel((DocsPAWA.DocsPaWR.FileRequest) Container.DataItem) %>"
                                                ToolTip ="<%# this.GetNomeOriginale((DocsPAWA.DocsPaWR.FileRequest) Container.DataItem) %>"
                                                >
                                        </asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn>
                                    <ItemStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:ImageButton ID="btnVersioneStampabile" runat="server" 
                                                Visible = "<%#this.IsAcquired((DocsPAWA.DocsPaWR.FileRequest) Container.DataItem)%>" 
                                                ImageUrl = "<%#this.GetPrintableVersionImage((DocsPAWA.DocsPaWR.FileRequest) Container.DataItem)%>"
                                                ImageAlign = "Middle" ToolTip="Apri versione stampabile"
                                                CommandName = "ShowPrintableVersion"/>
                                    </ItemTemplate>                                
                                </asp:TemplateColumn>
                            </Columns>
                        </asp:DataGrid>
                    </asp:Panel>
                </td>
            </tr>        
            </table> 
        </asp:Panel>
    </form>
</body>
</html>