<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Proceedings.aspx.cs" Inherits="NttDataWA.Management.Proceedings" MasterPageFile="~/MasterPages/Base.Master"  %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/jquery.jstree.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div id="containerTop" style="background-color: Green">
        <asp:UpdatePanel runat="server" ID="UpHeaderProject" UpdateMode="Conditional">
            <ContentTemplate>
                <div id="containerDocumentTop" style="background-color: Red">
                    <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div id="containerStandardTop">
                                <div id="containerStandardTopSx">
                                </div>
                                <div id="containerStandardTopCx">
                                    <p>
                                        <asp:Literal ID="LitProceedings" runat="server"></asp:Literal></p>
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
        <asp:UpdatePanel runat="server" ID="UpContainerProjectTab" UpdateMode="Conditional"
            ClientIDMode="Static">
            <ContentTemplate>
                <div id="containerDocumentTab" class="containerStandardTab">
                    <div id="containerDocumentTabOrangeInternalSpace">
                        <asp:UpdatePanel runat="server" ID="UpnlTabHeader" UpdateMode="Conditional" ClientIDMode="Static">
                            <ContentTemplate>
                                <div id="containerDocumentTabOrangeSx">
                                    <ul>
                                        <li class="searchIAmSearch" id="LiSearchProject" runat="server">
                                            <%--  <asp:HyperLink ID="LinkSearchVisibility" runat="server" NavigateUrl="~/Search/SearchVisibility.aspx"
                                                CssClass="clickable"></asp:HyperLink>--%></li>
                                    </ul>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div id="containerDocumentTabOrangeDx">
                        </div>
                    </div>
                    <div id="containerStandardTabDxBorder">
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <asp:UpdatePanel ID="UpContainer" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <div id="containerStandard2" runat="server" clientidmode="Static">
                <div id="content">
                    <!-- Parte sinistra-->
                    <div id="contentSx">
                        <div class="box_inside">
                            <div class="row">
                                <fieldset class="basic">
                                    <div class="row">
                                        <div class="col">
                                            <p>
                                                <span class="weight">
                                                    <asp:Label ID="lblReport" runat="server"></asp:Label>
                                                </span>
                                            </p>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-full">
                                            <asp:DropDownList ID="ddl_reportType" runat="server" CssClass="chzn-select-deselect" 
                                                AutoPostBack="true" RepeatLayout="UnorderedList" Width="97%">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </fieldset>
                            </div>
                        </div>
                        <div class="box_inside">
                            <!-- filtri -->
                            <div class="row">
                                <fieldset class="basic">
                                    <asp:PlaceHolder ID="pnl_single" runat="server">
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <span class="weight">
                                                        <asp:Label ID="lblProceedingType" runat="server"></asp:Label>
                                                    </span>
                                                </p>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-full">
                                                <asp:DropDownList ID="ddl_proceeding_type" runat="server" TabIndex="1" CssClass="chzn-select-deselect" 
                                                        AutoPostBack="true" Width="97%"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <span class="weight">
                                                        <asp:Label ID="lblProceedingsYear" runat="server"></asp:Label>
                                                    </span>
                                                </p>
                                            </div>
                                            <div class="col">
                                                <cc1:CustomTextArea ID="txt_anno" runat="server" CssClass="txt_textdata" CssClassReadOnly="txt_textdata_disabled" />
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>
                                </fieldset>
                            </div>
                        </div>
                    </div>
                    <!-- chiusura contentSx -->
                    <!-- Parte destra che conterrà la visualizzazione del PDF-->
                    <div id="contentDx">
                        <asp:UpdatePanel runat="server" ID="UpPnlContentDxSx" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div id="contentDxReport" runat="server" clientidmode="Static">
                                    <asp:UpdatePanel ID="UpPnlDocumentData" runat="server" UpdateMode="Conditional" ClientIDMode="Static" Visible="false">
                                        <ContentTemplate>
                                            <fieldset>
                                                <iframe width="100%" height="99%" frameborder="0" marginheight="0" marginwidth="0" id="frame"
                                                    runat="server" clientidmode="Static" style="z-index: 999999999;"></iframe>
                                            </fieldset>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="upPnlButtons" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="ProceedingsBtnPrint" runat="server" CssClass="btnEnable defaultAction" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="ProceedingsBtnPrint_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">        $(".chzn-select-deselect").chosen({
            allow_single_deselect: true, no_results_text: "Nessun risultato trovato"
        }); $(".chzn-select").chosen({
            no_results_text: "Nessun risultato trovato"
        }); </script>
</asp:Content>

