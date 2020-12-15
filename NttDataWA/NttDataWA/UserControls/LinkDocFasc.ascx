<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LinkDocFasc.ascx.cs"
    Inherits="NttDataWA.UserControls.LinkDocFasc" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:HiddenField ID="hf_SelectedObject" runat="server" Value="0" />
<asp:HiddenField ID="hf_Reset" runat="server" Value="0" />
<asp:HiddenField ID="hf_IdValue" runat="server" />
<asp:UpdatePanel runat="server" ID="UpPnlLinkCustom" UpdateMode="Conditional">
    <ContentTemplate>
        <triggers>
            <asp:AsyncPostBackTrigger ControlID="LinkDocFascBtn_Cerca" />
             <asp:AsyncPostBackTrigger ControlID="LinkDocFascBtn_Reset" />
        </triggers>
        <div class="row">
            <div class="col">
                <p>
                    <span class="weight">
                        <asp:Literal ID="TxtEtiLinkDocOrFascValue" runat="server"></asp:Literal></span></p>
            </div>
        </div>
        <asp:UpdatePanel runat="server" ID="UpdatePanel1">
            <ContentTemplate>
                <triggers>
                <asp:AsyncPostBackTrigger ControlID="hpl_Link" />
                </triggers>
                <asp:PlaceHolder ID="pnlLink_Link" runat="server" Visible="false">
                    <div class="row">
                        <div class="col">
                            <asp:LinkButton ID="hpl_Link" runat="server" OnClick="hpl_Link_Click" />
                        </div>
                    </div>
                </asp:PlaceHolder>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:PlaceHolder ID="pnlLink_InsertModify" runat="server" Visible="false">
            <div class="row">
                <div class="col">
                    <p>
                        <span class="weight">
                            <asp:Literal ID="TxtLinkDocFasc" runat="server"></asp:Literal></span></p>
                </div>
            </div>
            <div class="row">
                <div class="col_full">
                    <cc1:CustomTextArea ID="txt_MascheraValue" runat="server" CssClass="txt_input_full"
                        CssClassReadOnly="txt_input_full_disabled">
                    </cc1:CustomTextArea>
                </div>
            </div>
            <asp:PlaceHolder runat="server" ID="tr_interno">
                <div class="row">
                    <div class="col">
                        <p>
                            <span class="weight">
                                <asp:Literal ID="lbl_oggetto_link_Interno" runat="server"></asp:Literal></span></p>
                    </div>
                    <div class="col-right-no-margin">
                        <cc1:CustomImageButton runat="server" ID="LinkDocFascBtn_Cerca" ImageUrl="../Images/Icons/search_response_documents.png"
                            OnMouseOutImage="../Images/Icons/search_response_documents.png" OnMouseOverImage="../Images/Icons/search_response_documents_hover.png"
                            CssClass="clickable" ImageUrlDisabled="../Images/Icons/search_response_documents_disabled.png"
                            EnableViewState="true" OnClick="LinkDocFascBtn_Cerca_Click" />
                        <cc1:CustomImageButton runat="server" ID="LinkDocFascBtn_Reset" ImageUrl="../Images/Icons/clean_field_custom.png"
                            OnMouseOutImage="../Images/Icons/clean_field_custom.png" OnMouseOverImage="../Images/Icons/clean_field_custom_hover.png"
                            CssClass="clickable" ImageUrlDisabled="../Images/Icons/clean_field_custom_disabled.png"
                            EnableViewState="true" OnClick="LinkDocFascBtn_Reset_Click" />
                    </div>
                </div>
                <div class="row">
                    <div class="col_full">
                        <cc1:CustomTextArea ID="txt_NomeObjValue" runat="server" CssClass="txt_input_full"
                            CssClassReadOnly="txt_input_full_disabled" ReadOnly="true">
                        </cc1:CustomTextArea>
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder runat="server" ID="tr_esterno">
                <div class="row">
                    <div class="col">
                        <p>
                            <span class="weight">
                                <asp:Literal ID="TxtLinkDocFasc2" runat="server"></asp:Literal></span></p>
                    </div>
                </div>
                <div class="row">
                    <div class="col_full">
                        <cc1:CustomTextArea ID="txt_Link" runat="server" CssClass="txt_input_full" CssClassReadOnly="txt_input_full_disabled">
                        </cc1:CustomTextArea>
                    </div>
                </div>
            </asp:PlaceHolder>
        </asp:PlaceHolder>
    </ContentTemplate>
</asp:UpdatePanel>
