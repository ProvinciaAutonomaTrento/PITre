<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PannelloRicercaModelliTrasmissione.ascx.cs"
    Inherits="NttDataWA.UserControls.PannelloRicercaModelliTrasmissione" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<script type="text/javascript" language="javascript">
    /*
    * Cancellazione della descrizione di un corrispondente se il codice è vuoto
    */
    function clearCorrData(codTxt, descrTxtId) {
        if (codTxt.value == '')
            document.getElementById(descrTxtId).value = '';

    }
</script>
<asp:UpdatePanel runat="server" UpdateMode="Conditional" ID="UpPnlSearchTransmission"  ClientIDMode="static">
    <ContentTemplate>
        <div id="principale">
            <asp:Panel ID="pnlNoResult" runat="server" CssClass="content">
                <div class="filter" style="float: left; width: 97%; text-align: right;">
                    <asp:Label ID="lblNoResult" runat="server" ForeColor="Red" />
                </div>
            </asp:Panel>
            <br />
            <div class="content">
                <asp:Panel ID="pnlCampi" runat="server" CssClass="filter" Width="97%">
                    <div class="filter" style="float: left; width: 17%; text-align: left;">
                        <span class="weight">Codice</span><br />
                        <cc1:CustomTextArea ID="txtCodice" runat="server" CssClass="txt_input_full" CssClassReadOnly="txt_input_full_disabled" />
                    </div>
                    <div class="filter" style="float: left; width: 40%; text-align: left;">
                        <span class="weight">Modello</span><br />
                        <cc1:CustomTextArea ID="txtModello" runat="server" CssClass="txt_input_full" CssClassReadOnly="txt_input_full_disabled" />
                    </div>
                    <div class="filter" style="float: left; width: 40%; text-align: left;">
                        <span class="weight">Note</span><br />
                        <cc1:CustomTextArea ID="txtNote" runat="server" CssClass="txt_input_full" CssClassReadOnly="txt_input_full_disabled" />
                    </div>
                </asp:Panel>
            </div>
            <br />
            <div class="content">
                <asp:Panel ID="pnlCampi2" runat="server" CssClass="filter" Width="97%">
                    <div class="filter" style="float: left; width: 17%; text-align: left;">
                        <span class="weight">Tipo Trasmissione</span><br />
                        <asp:DropDownList ID="ddlTipoTrasmissione" runat="server" Width="99%" CssClass="chzn-select-deselect">
                            <asp:ListItem />
                            <asp:ListItem Text="Documento" Value="D" />
                            <asp:ListItem Text="Fascicolo" Value="F" />
                        </asp:DropDownList>
                    </div>
                    <div style="float: left; width: 40%; text-align: left;" class="filter">
                        <span class="weight">Registro</span><br />
                        <asp:DropDownList ID="ddlRegistri" runat="server" Width="99%" CssClass="chzn-select-deselect" />
                    </div>
                    <div style="float: left; width: 40%; text-align: left;" class="filter">
                        <span class="weight">Ragione Trasmissione</span><br />
                        <asp:DropDownList ID="ddlRagioniTrasmissione" runat="server" Width="99%" CssClass="chzn-select-deselect" />
                    </div>
                </asp:Panel>
            </div>
            <br />
            <div class="content">
                <asp:Panel runat="server" ID="pnlVisibilita" CssClass="filter" Visible="false" Width="97%">
                    <span class="weight">Visibilità</span>&nbsp;
                    <asp:DropDownList ID="ddlRegistry" runat="server" CssClass="chzn-select-deselect">
                    </asp:DropDownList>
                    <cc1:CustomTextArea ID="txtVisibilitaCodice" runat="server" Width="15%" />
                    &nbsp;<cc1:CustomImageButton runat="server" ID="imgSrcVisibilita" ImageUrl="../Images/Icons/view_answer_documents.png"
                        OnMouseOutImage="../Images/Icons/view_answer_documents.png" OnMouseOverImage="../Images/Icons/view_answer_documents_hover.png"
                        ImageUrlDisabled="../Images/Icons/view_answer_documents_disabled.png" CssClass="clickableLeft"
                        OnClick="imgSrcVisibilita_Click" />
                    &nbsp;&nbsp;<cc1:CustomTextArea ID="txtVisibilitaDescrizione" CssClass="testo" runat="server"
                        ReadOnly="true" Width="60%" />
                    &nbsp;&nbsp;<cc1:CustomImageButton runat="server" ID="imgRubricaVisibilita" ImageUrl="../Images/Icons/address_book.png"
                        OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                        CssClass="clickable" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                        OnClick="imgRubricaVisibilita_Click" Visible="True" />
                </asp:Panel>
            </div>
            <br />
            <div class="content">
                <asp:Panel ID="pnlDestinatari" runat="server" CssClass="filter" Width="97%">
                    <div class="row">
                        <div class="col">
                            <p>
                                <span style="text-align: left;" class="weight">Destinatari</span></p>
                        </div>
                    </div>
                    <div class="row">
                        <div class="filter" style="float: left; width: 17%; text-align: left;">
                            <cc1:CustomTextArea ID="txtDestinatariCodice" runat="server" CssClass="txt_input_full"
                                CssClassReadOnly="txt_input_full_disabled" AutoComplete="off" />
                        </div>
                        <div class="filter" style="float: left; width: 4%; text-align: left; padding-left: 25px;">
                            <cc1:CustomImageButton runat="server" ID="imgSrcDestinatari" ImageUrl="../Images/Icons/view_response_documents.png"
                                OnMouseOutImage="../Images/Icons/view_response_documents.png" OnMouseOverImage="../Images/Icons/view_response_documents_hover.png"
                                CssClass="clickable" OnClick="imgSrcDestinatari_Click" />
                        </div>
                        <asp:HiddenField ID="IdDestinatario" runat="server" />
                        <div class="filter" style="float: left; width: 70%; text-align: left;">
                            <cc1:CustomTextArea ID="txtDestinatariDescrizione" runat="server" ReadOnly="true"
                                CssClass="txt_input_full" CssClassReadOnly="txt_input_full_disabled" />
                        </div>
                        <div class="filter" style="float: left; width: 5%; text-align: left; margin-left: 5px;">
                            <cc1:CustomImageButton runat="server" ID="imgRubricaDestinatari" ImageUrl="../Images/Icons/address_book.png"
                                OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                CssClass="clickable" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                OnClick="imgRubricaDestinatari_Click" Visible="True" />
                        </div>
                    </div>
                </asp:Panel>
            </div>
            <br />
            <br />
            <div class="content">
                <div class="filter" style="float: left; width: 97%; text-align: left;">
                    <asp:CheckBox ID="chkRuoloDestDisabilitato" runat="server" Text="Ruolo destinatario disabilitato" />&nbsp;
                    <asp:CheckBox ID="chkRuoloDestInibito" runat="server" Text="Ruolo destinatario inibito ricezione trasmissioni" />
                </div>
            </div>
            <br />
            <br />
            <asp:Panel runat="server" ID="pnlRange" CssClass="content" Visible="false">
                <div class="filter" style="float: left; width: 97%">
                    <span class="testo_piccoloB">Cerca:</span><br />
                    <asp:RadioButtonList ID="rblSearchScope" runat="server" RepeatDirection="Horizontal"
                        CssClass="testo_piccoloB">
                        <asp:ListItem Text="Fra i modelli creati utente" Value="OnlyUser" />
                        <asp:ListItem Text="Fra i modelli creati dall'amministratore" Value="OnlyAdmin" />
                        <asp:ListItem Text="Ovunque" Value="Everywhere" Selected="True" />
                    </asp:RadioButtonList>
                </div>
            </asp:Panel>
          
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
