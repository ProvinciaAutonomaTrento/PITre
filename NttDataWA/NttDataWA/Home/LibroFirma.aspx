<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LibroFirma.aspx.cs" Inherits="NttDataWA.Home.LibroFirma"
    MasterPageFile="~/MasterPages/Base.Master" %>

<%@ Register Src="../UserControls/HomeTabs.ascx" TagPrefix="uc1" TagName="HomeTabs" %>
<%@ Register Src="../UserControls/HeaderHome.ascx" TagPrefix="uc2" TagName="HeaderHome" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .gridViewResult
        {
            min-width: 100%;
            overflow: auto;
        }
        .gridViewResult th
        {
            text-align: center;
            
        }
        .gridViewResult td
        {
            text-align: center;
            padding: 5px;
        }
        #gridViewResult tr.selectedrow
        {
            background: #f3edc6;
            color: #333333;
        }
        
        .tbl_rounded tr.Borderrow
        {
            border-top: 2px solid #FFFFFF;
        }
        
        .tbl_rounded td
        {
            background: #fff;
            min-height: 1em;
            border: 1px solid #A8A8A8;
            border-top: 0;
            text-align: center;
        }
        
        .margin-left
        {
            padding-left: 5px;
        }
        .tbl_rounded tr.Borderrow:hover td
        {
            background-color: #b2d7f8;
        }
        
        .proponente
        {
            padding-top: 4px;
            cursor: pointer;
            white-space: nowrap;
            text-overflow: ellipsis;
            overflow: hidden;
        }
        .proponente:hover
        {
            cursor: pointer;
            white-space: nowrap;
            text-overflow: ellipsis;
            overflow: hidden;
            text-decoration: none;
            color: #333333;
        }
        #containerNotificationCenterAdlHome
        {
            padding-left: 5px;
            padding-right: 5px;
            padding-top: 5px;
            width: 99%;
            height: 99%;
            overflow: hidden;
        }
        
        .truncated
        {
            white-space: nowrap;
            cursor: pointer;
            overflow: hidden;
            text-overflow: ellipsis;
            width: 200px;
        }
        .dropDownTipoFirma
        {
            color: #666;
            background: transparent !important;
            border: 0 !important;
            font-family: sans-serif;
            outline: 0;
            -webkit-box-shadow: none;
            -moz-box-shadow: none;
            box-shadow: none;
            text-align: left;
        }
        
        .colTipoFirma
        {
            float: left;
            margin: 0px 25px 0px 0px;
            text-align: left;
            border-bottom: 1px solid black;
        }
        
        .col_5_personal
        {
            float: left;
            margin: 0px 25px 0px 0px;
            text-align: left;
        }
        .imgTipoFirma
        {
            margin: 0px 0px 0px 0px;
            padding: 0px 0px 0px 0px;
            width: 14px;
            height: 11px;
        }
        .error
        {
            width: 100%;
            text-align: center;
            background: #FF0000;
            color: #FFFFFF;
            font-weight: bold;
            margin-bottom: 5px;
        }
        
        .FixedHeader
        {
            
            position: fixed;
            width:95%;
        }
    </style>
    <script type="text/javascript">

        function EsaminaUnoAUnoSingoloElemento() {
            $('#btnEsaminaUnoAUnoSingoloElemento').click();
        }
    </script>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <uc:ajaxpopup2 Id="AddFilterLibroFirma" runat="server" Url="../popup/AddFilterLibroFirma.aspx"
        Width="600" Height="600" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="EsaminaLibroFirmaSingolo" runat="server" Url="../popup/EsaminaLibroFirma.aspx?caller=s"
        IsFullScreen="true" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('upPnlButtons', 'CLOSE_ESAMINA_UNO_A_UNO');}" />
    <uc:ajaxpopup2 Id="EsaminaLibroFirma" runat="server" Url="../popup/EsaminaLibroFirma.aspx"
        IsFullScreen="true" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('upPnlButtons', 'CLOSE_ESAMINA_UNO_A_UNO');}" />
    <uc:ajaxpopup2 Id="AddressBook" runat="server" Url="../Popup/AddressBook.aspx" PermitClose="false"
        PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui){__doPostBack('upPnlButtons', 'closePopupAddressBook');}" />
    <uc:ajaxpopup2 Id="Object" runat="server" Url="../Popup/Object.aspx" PermitClose="false"
        PermitScroll="false" Width="800" Height="1000" IsFullScreen="false" CloseFunction="function (event, ui){__doPostBack('upPnlButtons', 'closePopupObject');}" />
    <uc:ajaxpopup2 Id="SignatureSelectedItems" runat="server" Url="../popup/SignatureSelectedItems.aspx"
        Width="700" Height="500" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('upPnlButtons', 'closePopupSignatureSelectedItems');}" />
    <uc:ajaxpopup2 Id="DetailsLFAutomaticMode" runat="server" Url="../popup/DetailsLFAutomaticMode.aspx?caller=lf"
        Width="750" Height="500" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('upPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="MassiveReport" runat="server" Url="../popup/MassiveReport_iframe.aspx"
        IsFullScreen="true" PermitClose="false" PermitScroll="true" />
    <div id="containerTop">
        <div id="containerTabHome">
            <asp:UpdatePanel runat="server" ID="UpHeaderHome" UpdateMode="Conditional" ClientIDMode="static">
                <ContentTemplate>
                    <div id="containerStandardTop">
                        <div id="containerStandardTopCxHome">
                            <img src="../Images/Common/griff.png" alt="" title="" />
                        </div>
                        <div id="containerHomeHeader">
                            <div id="containerHeaderHomeSx">
                                <strong>
                                    <asp:Label runat="server" ID="headerHomeLblRole"></asp:Label>
                                </strong>
                            </div>
                            <div id="containerHeaderHomeDx">
                                <div class="styled-select_full">
                                    <asp:DropDownList ID="ddlRolesUser" runat="server" CssClass="chzn-select-deselect"
                                        AutoPostBack="true" Width="700px" OnSelectedIndexChanged="ddlRolesUser_SelectedIndexChange">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel runat="server" ID="UpContainerProjectTab" UpdateMode="Conditional"
                ClientIDMode="Static">
                <ContentTemplate>
                    <div id="containerTabIndex">
                        <asp:UpdatePanel runat="server" ID="UpnlTabHeader" UpdateMode="Conditional" ClientIDMode="Static">
                            <ContentTemplate>
                                <uc1:HomeTabs runat="server" PageCaller="LIBRO_FIRMA" ID="HomeTabs"></uc1:HomeTabs>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <div id="containerNotificationCenter">
                <div id="containerNotificationCenterAdlHome">
                    <div id="containerHeaderButton">
                        <asp:UpdatePanel ID="UpnlButtonTop" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="col_5_personal">
                                    <cc1:CustomImageButton runat="server" ID="LibroFirmaImgRefresh" ImageUrl="../Images/Icons/home_refresh.png"
                                        OnMouseOutImage="../Images/Icons/home_refresh.png" OnMouseOverImage="../Images/Icons/home_refresh_hover.png"
                                        CssClass="clickableRight" ImageUrlDisabled="../Images/Icons/home_refresh_disabled.png"
                                        OnClick="LibroFirmaImgRefresh_OnClick" />
                                    <cc1:CustomImageButton runat="server" ID="LibroFirmaImgAddFilter" ImageUrl="../Images/Icons/home_add_filters.png"
                                        OnMouseOutImage="../Images/Icons/home_add_filters.png" OnMouseOverImage="../Images/Icons/home_add_filters_hover.png"
                                        CssClass="clickableRight" ImageUrlDisabled="../Images/Icons/home_add_filters_disabled.png"
                                        OnClientClick="return ajaxModalPopupAddFilterLibroFirma();" />
                                    <cc1:CustomImageButton runat="server" ID="LibroFirmaImgRemoveFilter" ImageUrl="../Images/Icons/home_delete_filters.png"
                                        OnMouseOutImage="../Images/Icons/home_delete_filters.png" OnMouseOverImage="../Images/Icons/home_delete_filters_hover.png"
                                        CssClass="clickable" ImageUrlDisabled="../Images/Icons/home_delete_filters_disabled.png"
                                        Enabled="false" OnClick="LibroFirmaImgRemoveFilter_Click" />
                                    <cc1:CustomImageButton runat="server" ID="SelezionaPerFirma" ImageUrl="../Images/Icons/LibroFirma/Sel_Firma.png"
                                        OnMouseOutImage="../Images/Icons/LibroFirma/Sel_Firma.png" OnMouseOverImage="../Images/Icons/LibroFirma/Sel_Firma_hover.png"
                                        CssClass="clickableRight margin-left" ImageUrlDisabled="../Images/Icons/LibroFirma/Sel_Firma_disabled.png"
                                        OnClick="SelezionaPerFirma_Click" Height="22px" />
                                    <cc1:CustomImageButton runat="server" ID="SelezionaPerRespingi" ImageUrl="../Images/Icons/LibroFirma/Sel_Rifiuta.png"
                                        OnMouseOutImage="../Images/Icons/LibroFirma/Sel_Rifiuta.png" OnMouseOverImage="../Images/Icons/LibroFirma/Sel_Rifiuta_hover.png"
                                        CssClass="clickable" ImageUrlDisabled="../Images/Icons/LibroFirma/Sel_Rifiuta_disabled.png"
                                        OnClick="SelezionaPerRespingi_Click" Height="22px" />
                                    <cc1:CustomImageButton runat="server" ID="Deseleziona" ImageUrl="../Images/Icons/LibroFirma/Sel_None.png"
                                        OnMouseOutImage="../Images/Icons/LibroFirma/Sel_None.png" OnMouseOverImage="../Images/Icons/LibroFirma/Sel_None_hover.png"
                                        CssClass="clickable" ImageUrlDisabled="../Images/Icons/LibroFirma/Sel_None_disabled.png"
                                        OnClick="RimuoviSelezione_Click" Height="22px" />
                                    <cc1:CustomImageButton runat="server" ID="LibroFirmaImgEsamina" ImageUrl="../Images/Icons/LibroFirma/EsaminaUnoAUno.png"
                                        OnMouseOutImage="../Images/Icons/LibroFirma/EsaminaUnoAUno.png" OnMouseOverImage="../Images/Icons/LibroFirma/EsaminaUnoAUno_hover.png"
                                        CssClass="clickable margin-left" ImageUrlDisabled="../Images/Icons/LibroFirma/EsaminaUnoAUno_disabled.png"
                                        Width="20" OnClick="EsaminaUnoAUno_Click" />
                                </div>
                                <div class="colTipoFirma">
                                    <img src="../Images/Icons/LibroFirma/LF_SignType.png" alt="" title="" class="imgTipoFirma" />
                                    <asp:DropDownList runat="server" ID="TipoFirmaDigitale_People" CssClass="clickable dropDownTipoFirma"
                                        OnSelectedIndexChanged="TipoFirmaDigitale_People_SelectedIndexChanged" AutoPostBack="true">
                                    </asp:DropDownList>
                                </div>
                                <div style="padding-top: 5px">
                                    <asp:UpdatePanel ID="upPnlNumElementi" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <div class="col">
                                                <asp:Label ID="lblNumElementi" runat="server"></asp:Label>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div id="containerGridView" class="row" style="height: 100%; overflow: auto">
                        <asp:UpdatePanel runat="server" ID="UpnlGrid" UpdateMode="Conditional" class="UpnlGrid">
                            <ContentTemplate>
                                <asp:GridView ID="gridViewResult" runat="server" Width="100%" AllowPaging="true"
                                    PageSize="10" AutoGenerateColumns="false" CssClass="gridViewResult tbl_rounded round_onlyextreme"
                                    HorizontalAlign="Center" ShowHeader="true" ShowHeaderWhenEmpty="true" OnRowDataBound="gridViewResult_RowDataBound"
                                    OnPreRender="gridViewResult_PreRender" OnPageIndexChanging="gridViewResult_PageIndexChanging"
                                    OnRowCreated="gridViewResult_ItemCreated" OnRowCommand="gridViewResult_RowCommand"
                                     Style="cursor: pointer;">
                                    <RowStyle CssClass="NormalRow" />
                                    <PagerStyle CssClass="recordNavigator2" />
                                    <Columns>
                                        <asp:TemplateField Visible="false">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="systemIdElemento" Text='<%# Bind("IdElemento") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText='<%$ localizeByText:LibroFirmaIdNumProto%>' HeaderStyle-Width="3%"
                                            ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <cc1:CustomImageButton runat="server" ID="BtnDocument" Width="20px" CssClass="clickableRight"
                                                    CommandName="viewLinkObject" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <%--                                <asp:TemplateField HeaderText='<%$ localizeByText:LibroFirmaIdNumProto%>' HeaderStyle-Width="8%"
                                        ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <span class="noLink">
                                                <asp:LinkButton runat="server" ID="idDocumento" Text='<%#this.GetDocnumber((NttDataWA.DocsPaWR.ElementoInLibroFirma) Container.DataItem) %>'
                                                    CommandName="viewLinkObject" CssClass="clickable"></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText='<%$ localizeByText:LibroFirmaFile%>' HeaderStyle-Width="4%"
                                        ItemStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="file" Text='<%#this.GetFile((NttDataWA.DocsPaWR.ElementoInLibroFirma) Container.DataItem) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>
                                        <asp:TemplateField Visible="false">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="idProfile" Text='<%# Bind("InfoDocumento.Docnumber") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText='<%$ localizeByText:LibroFirmaOggetto%>' HeaderStyle-Width="25%">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="oggetto" Text='<%#this.GetObject((NttDataWA.DocsPaWR.ElementoInLibroFirma) Container.DataItem) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText='<%$ localizeByText:LibroFirmaTipologia%>' Visible="false">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="tipologia" Text='<%# Bind("InfoDocumento.TipologiaDocumento") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText='<%$ localizeByText:LibroFirmaDestinatario%>' Visible="false">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="destinatario" Text='<%# Bind("InfoDocumento.Destinatario") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText='<%$ localizeByText:LibroFirmaProponente%>' ItemStyle-Width="8%"
                                            HeaderStyle-Width="8%">
                                            <ItemTemplate>
                                                <div class="truncated">
                                                    <asp:Label runat="server" ID="proponente" Text='<%#this.GetProponente((NttDataWA.DocsPaWR.ElementoInLibroFirma) Container.DataItem) %>'
                                                        ToolTip='<%#this.GetProponente((NttDataWA.DocsPaWR.ElementoInLibroFirma) Container.DataItem) %>'
                                                        CssClass="clickable" />
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText='<%$ localizeByText:LibroFirmaDataInserimento%>' HeaderStyle-Width="5%"
                                            HeaderStyle-Wrap="false">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="dataInserimento" Text='<%#this.GetDataInserimento((NttDataWA.DocsPaWR.ElementoInLibroFirma) Container.DataItem) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText='<%$ localizeByText:LibroFirmaTipo%>' HeaderStyle-Width="3%">
                                            <ItemTemplate>
                                                <asp:Image runat="server" ID="iconTypeSignature" Width="20px" CssClass="clickable" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText='<%$ localizeByText:LibroFirmaNote%>' HeaderStyle-Width="20%">
                                            <ItemTemplate>
                                                <%--<div class="truncated">--%>
                                                <asp:Label runat="server" ID="note" Text='<%# Bind("Note") %>' ToolTip='<%# Bind("Note") %>'
                                                    CssClass="clickable" />
                                                <%--</div>--%>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText='<%$ localizeByText:LibroFirmaModalita%>' HeaderStyle-Width="3%">
                                            <ItemTemplate>
                                                <cc1:CustomImageButton runat="server" ID="imgModalita" Width="20px" CssClass="clickable" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-Width="2%">
                                            <ItemTemplate>
                                                <cc1:CustomImageButton runat="server" ID="EsaminaUnoAUnoSingoloElemento" Width="20px"
                                                    CssClass="clickableLeft" OnClick="EsaminaUnoAUnoSingoloElemento_Click" ImageUrl="../Images/Icons/LibroFirma/EsaminaSingolo.png"
                                                    OnMouseOutImage="../Images/Icons/LibroFirma/EsaminaSingolo.png" OnMouseOverImage="../Images/Icons/LibroFirma/EsaminaSingolo_hover.png"
                                                    ImageUrlDisabled="../Images/Icons/LibroFirma/EsaminaSingolo_disabled.png" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText='<%$ localizeByText:LibroFirmaStato%>' HeaderStyle-Width="2%">
                                            <ItemTemplate>
                                                <div id="errorSign" class="error" runat="server">
                                                    <asp:Label runat="server" ID="lblErrorSign" Text='<%$ localizeByText:LibroFirmaLblErrorSign%>'
                                                        ToolTip='<%#this.GetErrorSign((NttDataWA.DocsPaWR.ElementoInLibroFirma) Container.DataItem) %>'
                                                        CssClass="clickableLeft" />
                                                </div>
                                                <cc1:CustomImageButton runat="server" ID="BtnStato" Width="20px" CssClass="clickableLeft"
                                                    OnClick="BtnStato_Click" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                                <asp:PlaceHolder ID="plcNavigator" runat="server" />
                                <asp:UpdatePanel ID="upPnlGridIndexes" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:HiddenField ID="grid_rowindex" runat="server" ClientIDMode="Static" />
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel ID="upPnlButtons" UpdateMode="Conditional" runat="server" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="LibroFirmaFirmaSelezionati" runat="server" CssClass="btnEnable clickableSW"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClientClick="return ajaxModalPopupSignatureSelectedItems();" />
            <cc1:CustomButton ID="LibroFirmaFirmaSelezionatiAdl" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="LibroFirmaFirmaSelezionatiAdl_Click" />
            <cc1:CustomButton ID="LibroFirmaRespingiSelezionati" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="LibroFirmaRespingiSelezionati_Click" />
            <cc1:CustomButton ID="LibroFirmaEliminaSelezionati" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" Visible="false" />
            <asp:HiddenField ID="HiddenItemsChecked" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="HiddenItemsUnchecked" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="HiddenItemsAll" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="HiddenSelectForSignature" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="HiddenSelectForReject" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="HiddenRemoveSelect" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="HiddenEsaminaTitle" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="HiddenRespingiSelezionati" runat="server" ClientIDMode="Static" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        //alert($(".chzn-select-deselect").tipsy);
        $(".chzn-select-deselect").tipsy({ gravity: 's', fade: false, opacity: 1, delayIn: 0, delayOut: 0 });
        $(".chzn-select").tipsy({ gravity: 's', fade: false, opacity: 1, delayIn: 0, delayOut: 0 });
        $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
        $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
    </script>
</asp:Content>
