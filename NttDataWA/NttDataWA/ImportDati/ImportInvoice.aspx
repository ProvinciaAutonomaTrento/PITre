<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Base.Master" AutoEventWireup="true" CodeBehind="ImportInvoice.aspx.cs" Inherits="NttDataWA.ImportDati.ImportInvoice" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
                                    <p>
                                        <asp:Literal ID="LitSearchProject" runat="server"></asp:Literal>
                                    </p>
                                </div>
                                <div id="containerStandardTopDx">
                                </div>
                            </div>
                            <div id="containerStandardBottom">
                                <div id="containerProjectCxBottom"></div>
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
                                    <li class="searchIAmSearch" id="LiSearchProject" runat="server"></li>
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
            <div id="containerStandard2">
                <div id="content">
                    <%--parte sinistra--%>
                    <div id="contentSx">
                        <div class="box_inside">
                            <div class="row">
                                <fieldset>
                                    <asp:UpdatePanel ID="UpPnlSearchInvoice" UpdateMode="Conditional" runat="server">
                                        <ContentTemplate>
                                            <div class="row">
                                                <div class="col">
                                                    <p>
                                                        <span class="weight">
                                                            <asp:Literal runat="server" ID="LitImportInvoiceSearch"></asp:Literal>*
                                                        </span>
                                                    </p>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-full">
                                                    <cc1:CustomTextArea runat="server" ID="TxtImportInvoiceSearch" CssClass="txt_input_full"
                                                     CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                </div>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </fieldset>
                            </div>
                            <asp:UpdatePanel ID="UpPnlOptionalFields" UpdateMode="Conditional" runat="server">
                                <ContentTemplate>
                                    <asp:PlaceHolder runat="server" ID="PnlOptionalFields" Visible="false">
                                    <asp:Literal runat="server" ID="Literal1" ></asp:Literal>
                                        <div class="row">
                                            <fieldset>
                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Literal runat="server" ID="LitTxtRifAmm"></asp:Literal>
                                                                </span>
                                                            </p>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-full">
                                                            <cc1:CustomTextArea runat="server" ID="TxtRifAmm" CssClass="txt_input_full" 
                                                            CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Literal runat="server" ID="LitTxtIdDoc"></asp:Literal>
                                                                </span>
                                                            </p>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-full">
                                                            <cc1:CustomTextArea runat="server" ID="TxtIdDoc" CssClass="txt_input_full" 
                                                            CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Literal runat="server" ID="LitTxtCIG"></asp:Literal>
                                                                </span>
                                                            </p>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-full">
                                                            <cc1:CustomTextArea runat="server" ID="TxtCIG" CssClass="txt_input_full" 
                                                            CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                    </div>
                                                   
                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Literal runat="server" ID="LitTxtPosFin"></asp:Literal>
                                                                </span>
                                                            </p>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-full">
                                                            <cc1:CustomTextArea runat="server" ID="TxtPosFin" CssClass="txt_input_full" 
                                                            CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                    </div>
                                                    <!--OPTIONAL -->
                                                    <asp:Panel runat="server" ID="PnlTxtOptional1">
                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Literal runat="server" ID="LitTxtOptional1"></asp:Literal>
                                                                </span>
                                                            </p>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-full">
                                                            <cc1:CustomTextArea runat="server" ID="TxtOptional1" CssClass="txt_input_full" 
                                                            CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                    </div>
                                                    </asp:Panel>
                                                    <asp:Panel runat="server" ID="PnlTxtOptional2">
                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Literal runat="server" ID="LitTxtOptional2"></asp:Literal>
                                                                </span>
                                                            </p>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-full">
                                                            <cc1:CustomTextArea runat="server" ID="TxtOptional2" CssClass="txt_input_full" 
                                                            CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                    </div>
                                                    </asp:Panel>
                                                    <asp:Panel runat="server" ID="pnlDataInizioFine" Visible="false">
                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Literal runat="server" ID="LitTxtOptional3"></asp:Literal>
                                                                </span>
                                                            </p>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-full">
                                                            <cc1:CustomTextArea runat="server" ID="TxtOptional3" CssClass="txt_input_full"
                                                            CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Literal runat="server" ID="LitTxtOptional4"></asp:Literal>
                                                                </span>
                                                            </p>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-full">
                                                            <cc1:CustomTextArea runat="server" ID="TxtOptional4" CssClass="txt_input_full" 
                                                            CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                    </div>
                                                    </asp:Panel>
                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Literal runat="server" ID="LitTxtOptional5"></asp:Literal>
                                                                </span>
                                                            </p>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-full">
                                                            <cc1:CustomTextArea runat="server" ID="TxtOptional5" CssClass="txt_input_full" 
                                                            CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                    </div>

                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Literal runat="server" ID="LitTxtOptional6"></asp:Literal>
                                                                </span>
                                                            </p>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-full">
                                                            <cc1:CustomTextArea runat="server" ID="TxtOptional6" CssClass="txt_input_full" 
                                                            CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                    </div>
                                                    
                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Literal runat="server" ID="LitTxtOptional10"></asp:Literal>
                                                                </span>
                                                            </p>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-full">
                                                            <cc1:CustomTextArea runat="server" ID="TxtOptional10" CssClass="txt_input_full" 
                                                            CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                    </div>

                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Literal runat="server" ID="LitTxtOptional7"></asp:Literal>
                                                                </span>
                                                            </p>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-full">
                                                            <cc1:CustomTextArea runat="server" ID="TxtOptional7" CssClass="txt_input_full" 
                                                            CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                    </div>

                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Literal runat="server" ID="LitTxtOptional8"></asp:Literal>
                                                                </span>
                                                            </p>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-full">
                                                            <cc1:CustomTextArea runat="server" ID="TxtOptional8" CssClass="txt_input_full" 
                                                            CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                    </div>

                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Literal runat="server" ID="LitTxtOptional9"></asp:Literal>
                                                                </span>
                                                            </p>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-full">
                                                            <cc1:CustomTextArea runat="server" ID="TxtOptional9" CssClass="txt_input_full" 
                                                            CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                    </div>

                                            </fieldset>
                                        </div>
                                    </asp:PlaceHolder>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <div class="col-right">
                                <asp:UpdatePanel ID="UplAddLine" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Panel ID="PnlBtnAddLine" runat="server" Visible="false">
                                            <asp:Literal runat="server" ID="litAddLine" ></asp:Literal>
                                            <cc1:CustomImageButton ID="btnAddLine" runat="server" CssClass="clickableRight" Visible="true"
                                                OnClick="BtnAddLine_Click" ImageUrl="../Images/Icons/add_version.png" OnMouseOutImage="../Images/Icons/add_version.png"
                                                OnMouseOverImage="../Images/Icons/add_version_hover.png" ImageUrlDisabled="../Images/Icons/add_version_disabled.png" />
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                            <asp:UpdatePanel ID="UpnNewLine" UpdateMode="Conditional" runat="server">
                                <ContentTemplate>
                                    <asp:PlaceHolder runat="server" ID="PnlNewLine" Visible="false">
                                        <div class="row">
                                            <fieldset>
                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Literal runat="server" ID="litDescrizione"></asp:Literal>
                                                                </span>
                                                            </p>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-full">
                                                            <cc1:CustomTextArea runat="server" ID="txtDescrizione" CssClass="txt_input_full" 
                                                            CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Literal runat="server" ID="litQuantita"></asp:Literal>
                                                                </span>
                                                            </p>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-full">
                                                            <cc1:CustomTextArea runat="server" ID="txtQuantita" CssClass="txt_input_full onlynumbers" 
                                                            CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Literal runat="server" ID="litPrezzoUnitario"></asp:Literal>
                                                                </span>
                                                            </p>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-full">
                                                            <cc1:CustomTextArea runat="server" ID="txtPrezzoUnitario" CssClass="txt_input_full" 
                                                            CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                    </div>
                                                   
                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Literal runat="server" ID="litPrezzoTotale"></asp:Literal>
                                                                </span>
                                                            </p>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-full">
                                                            <cc1:CustomTextArea runat="server" ID="txtPrezzoTotale" CssClass="txt_input_full" 
                                                            CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                    </div>

                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Literal runat="server" ID="litAliquota"></asp:Literal>
                                                                </span>
                                                            </p>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-full">
                                                            <cc1:CustomTextArea runat="server" ID="txtAliquota" CssClass="txt_input_full onlynumbers" 
                                                            CssClassReadOnly="txt_input_full_disabled"></cc1:CustomTextArea>
                                                        </div>
                                                    </div>
                                            </fieldset>
                                        </div>
                                    </asp:PlaceHolder>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                    <%--parte destra--%>
                    <div id="contentDx">
                        <asp:UpdatePanel runat="server" ID="UpPnlContentDxSx" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div id="contentDxSxProspetti" runat="server" clientidmode="Static">
                                    <asp:UpdatePanel ID="UpPnlDocumentData" runat="server" UpdateMode="Conditional" ClientIDMode="Static"
                                        Visible="false">
                                        <ContentTemplate>
                                            <fieldset>
                                                <iframe width="100%" height="99%" frameborder="0" marginheight="0" marginwidth="0"
                                                    id="frame" runat="server" clientidmode="Static" style="z-index: 999999999;">
                                                </iframe>
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
    <asp:UpdatePanel ID="upPnlButtons" UpdateMode="Conditional" runat="server" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="BtnImportInvoiceSearch" runat="server" CssClass="btnEnable defaultAction" CssClassDisabled="btnDisable"
            OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnImportInvoiceSearch_Click" />
            <cc1:CustomButton ID="BtnImportInvoiceUpdate" runat="server" CssClass="btnEnable defaultAction" CssClassDisabled="btnDisable"
            OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnImportInvoiceUpdate_Click" />
            <cc1:CustomButton ID="BtnImportInvoiceConfirm" runat="server" CssClass="btnEnable defaultAction" CssClassDisabled="btnDisable"
            OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnImportInvoiceConfirm_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
