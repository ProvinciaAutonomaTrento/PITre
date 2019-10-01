<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportAccessi.aspx.cs" Inherits="NttDataWA.Management.ReportAccessi" MasterPageFile="~/MasterPages/Base.Master"  %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>

<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div id="containerTop">
        <div id="containerDocumentTop">
            <div id="containerStandardTop">
                <div id="containerStandardTopSx"></div>
                <div id="containerStandardTopCx">
                    <p><asp:Literal ID="ReportAccessiTitle" runat="server"></asp:Literal></p>
                </div>
            </div>
            <div id="containerStandardBottom">
                <div id="containerProjectCxBottom">
                </div>
            </div>
        </div>
        <div id="containerDocumentTab" class="containerStandardTab">
            <div id="containerDocumentTabOrangeInternalSpace">
                <div id="containerDocumentTabOrangeSx">
                </div>
                <div id="containerDocumentTabOrangeDx">
                </div>
            </div>
            <div id="containerStandardTabDxBorder">
            </div>
        </div>
    </div>
    <div id="containerStandard2" runat="server" clientidmode="Static">
        <div id="content">
            <div id="contentSx">
                <fieldset class="basic">
                    <asp:UpdatePanel runat="server" ID="UpPnlFilter" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="row">
                                <div class="col">
                                    <p>
                                        <span class="weight"><asp:Label runat="server" ID="lblFormato" Font-Bold="True"></asp:Label></span>
                                    </p>
                                </div>
                            </div>
                            <!-- FORMATO FILE EXPORT -->
                            <div class="row">
                                <div class="col">
                                    <asp:DropDownList ID="ddlFormatoPreReport" runat="server" Width="200px" AutoPostBack="true"
                                        CssClass="chzn-select-deselect">
                                        <asp:ListItem id="xlsItem" Value="XLS"></asp:ListItem>
                                        <asp:ListItem id="odtItem" Value="ODS"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col">
                                    <p>
                                        <span class="weight">
                                            <asp:Literal runat="server" ID="ReportAccessiLitData"></asp:Literal>
                                        </span>
                                    </p>
                                </div>
                            </div>
                            <!-- DATA CREAZIONE FASCICOLO -->
                            <div class="row">
                                <div class="col2">
                                    <asp:DropDownList ID="ddl_dataCreazione_E" runat="server" AutoPostBack="true" Width="140px"
                                        CssClass="chzn-select-deselect" OnSelectedIndexChanged="ddl_dataCreazione_E_SelectedIndexChanged">
                                            <asp:ListItem id="ddlDateItemSingleValue" Value="0"></asp:ListItem>
                                            <asp:ListItem id="ddlDateItemRange" Value="1"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="col2">
                                    <asp:Literal runat="server" ID="LtlDaDataCreazione" Visible="false"></asp:Literal>
                                </div>
                                <div class="col4">
                                    <cc1:CustomTextArea ID="txt_initDataCreazione_E" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                            CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                </div>
                                <div class="col2">
                                    <asp:Literal runat="server" ID="LtlADataCreazione" Visible="false"></asp:Literal>
                                </div>
                                <div class="col4">
                                    <cc1:CustomTextArea ID="txt_finedataCreazione_E" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                            CssClassReadOnly="txt_textdata_disabled" Visible="false"></cc1:CustomTextArea>
                                </div>
                            </div>
                            <!-- STATO DEL FASCICOLO -->
                            <div class="row">
                                <div class="col">
                                    <p>
                                        <span class="weight">
                                            <asp:Literal runat="server" ID="LtlStatoFascicolo" Visible="false"></asp:Literal>
                                        </span>
                                    </p>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col">
                                    <asp:DropDownList ID="ddl_statoFascicolo" runat="server" Width="140px" CssClass="chzn-select-deselect" Visible="false">
                                        <asp:ListItem id="itemAll" Value="A"></asp:ListItem>
                                        <asp:ListItem id="itemOpen" Value="O"></asp:ListItem>
                                        <asp:ListItem id="itemClosed" Value="C"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:UpdatePanel runat="server" ID="UpPnlHidden" UpdateMode="Conditional" ClientIDMode="Static">
                        <ContentTemplate>
                            <asp:HiddenField ID="HiddenConfirmExport" runat="server" ClientIDMode="Static" />
                            <asp:HiddenField ID="HiddenConfirmPublish" runat="server" ClientIDMode="Static" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </fieldset>
                
            </div>
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

</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="upPnlButtons" UpdateMode="Conditional" runat="server" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="BtnExport" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="BtnExport_Click" Visible="True" Enabled="True" />
            <cc1:CustomButton ID="BtnPublish" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="BtnPublish_Click" Visible="true" Enabled="true" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

