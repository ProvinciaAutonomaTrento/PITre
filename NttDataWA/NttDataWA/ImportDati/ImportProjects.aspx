<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Base.Master" AutoEventWireup="true" CodeBehind="ImportProjects.aspx.cs" Inherits="NttDataWA.ImportDati.ImportProjects" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .container
        {
            width: 95%;
            margin: 0 auto;
        }        
        
        fieldset {min-height: 70px;}
        
        #tabs {background: #fff; border: 0; width: 98%;}
        #tabs ul {background: #fff; border-top: 0; border-left: 0; border-right: 0;}
        #tabs div {background: #fff;}
    </style>
    <script type="text/javascript">
        function get_estensione(path) {
            var posizione_punto = path.lastIndexOf(".");
            var lunghezza_stringa = path.length;
            var estensione = path.substring(posizione_punto + 1, lunghezza_stringa);
            return estensione;
        }

        function controlla_estensione(el) {
            if (get_estensione(el.value) != "xls") {
                alert("Il file deve avere estensione .xls");
                sostituisciInputFile(el);
            }
        }

        function sostituisciInputFile(el) {
            return el.parentNode.replaceChild(el.cloneNode(true), el);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div id="containerTop">
        <asp:UpdatePanel runat="server" ID="UpHeaderProject" UpdateMode="Conditional">
            <ContentTemplate>
                <div id="containerDocumentTop">
                    <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div id="containerStandardTop">
                                <div id="containerStandardTopSx">
                                </div>
                                <div id="containerStandardTopCx">
                                    <p><asp:Label ID="pageTitle" runat="server" /></p>
                                </div>
                                <div id="containerStandardTopDx">
                                </div>
                            </div>
                            <div id="containerStandardBottom">
                                <div id="containerProjectCxBottom">
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

    <asp:UpdatePanel ID="UpContainer" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <div id="containerStandard">
                <div id="content">
                    <div id="contentStandard1Column">
                        <asp:UpdatePanel ID="box_upload" runat="server">
                            <ContentTemplate>
                                <fieldset class="basic4">
                                    <div class="col-right"><img src="../Images/Icons/small_xls.png" alt="" /> <asp:HyperLink ID="lnkTemplate" runat="server" NavigateUrl="ImportFascicoli.xls" Target="_blank" /></div>
                                    <div class="col"><asp:Literal ID="lblFilename" runat="server" /></div>
                                    <div class="col"><input type="file" id="fileUpload" runat="server" onchange="controlla_estensione(this);"  /></div>
                                </fieldset>
                                <asp:Button ID="BtnUploadHidden" runat="server" CssClass="hidden" ClientIDMode="Static" OnClick="BtnUploadHidden_Click" OnClientClick="disallowOp('Content2');" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:PostBackTrigger ControlID="BtnUploadHidden" />
                            </Triggers>
                        </asp:UpdatePanel>
                        <asp:UpdatePanel ID="upReport" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:PlaceHolder id="plcReport" runat="server" Visible="false">
                                    <div id="tabs">
                                        <ul>
                                            <li><a href="#tabs-1"><asp:Literal ID="lblTabGeneral" runat="server"></asp:Literal></a></li>
                                            <li><a href="#tabs-2"><asp:Literal ID="lblTabReportPdf" runat="server"></asp:Literal></a></li>
                                        </ul>
                                        <div id="tabs-1">
                                            <!-- generale -->
                                            <asp:GridView ID="grdGenerale" runat="server" ShowHeader="true" AutoGenerateColumns="False" CssClass="tbl_rounded round_onlyextreme">
                                                <RowStyle CssClass="NormalRow" />
                                                <AlternatingRowStyle CssClass="AltRow" />
                                                <Columns>
                                                    <asp:TemplateField>
                                                        <ItemStyle HorizontalAlign="Center" Width="3%" />
                                                        <ItemTemplate>
                                                            <asp:Literal ID="ltlOrdinalNumber" runat="server" Text="<%#this.GetOrdinalNumber((NttDataWA.DocsPaWR.ImportResult) Container.DataItem)%>" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="Message" ItemStyle-Width="45%" />
                                                    <asp:TemplateField>
                                                        <ItemStyle HorizontalAlign="Center" Width="7%" />
                                                        <ItemTemplate>
                                                            <asp:Literal ID="ltlResult" runat="server" Text="<%#this.GetResult((NttDataWA.DocsPaWR.ImportResult) Container.DataItem)%>" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField ItemStyle-Width="45%">
                                                        <ItemTemplate>
                                                            <asp:Literal ID="ltlDetails" runat="server" Text="<%#this.GetDetails((NttDataWA.DocsPaWR.ImportResult) Container.DataItem)%>" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                        <div id="tabs-2">
                                            <!-- report pdf -->
                                            <iframe src="../Popup/MassiveReport.aspx" id="frame" width="100%" frameborder="0"></iframe>
                                            <script type="text/javascript">
                                                function resizePrintIframe() {
                                                    var height = document.documentElement.clientHeight;
                                                    height -= 90; /* whatever you set your body bottom margin/padding to be */
                                                    document.getElementById('frame').style.height = height + "px";
                                                };

                                                $(function () {
                                                    resizePrintIframe();
                                                });
                                            </script>
                                        </div>
                                    </div>
                                </asp:PlaceHolder>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="BtnImport" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClientClick="$('#BtnUploadHidden').click();" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
