<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CorrespondentCustom.ascx.cs"
    Inherits="NttDataWA.UserControls.CorrespondentCustom" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<div class="row">
    <div class="col">
        <p>
            <span class="weight">
                <asp:Literal ID="TxtEtiCustomCorrespondentValue" runat="server"></asp:Literal></span></p>
    </div>
    <div class="col-right-no-margin">
        <asp:UpdatePanel runat="server" ID="UpPnlCustomCorrespondent" UpdateMode="Conditional">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="DocumentImgCustomCorrespondentAddressBookCustom" />
            </Triggers>
            <ContentTemplate>
                <cc1:CustomImageButton runat="server" ID="DocumentImgCustomCorrespondentAddressBookCustom"
                    ImageUrl="../Images/Icons/address_book.png" OnMouseOutImage="../Images/Icons/address_book.png"
                    OnMouseOverImage="../Images/Icons/address_book_hover.png" CssClass="clickableLeft"
                    ImageUrlDisabled="../Images/Icons/address_book_disabled.png" EnableViewState="true"
                    OnClick="DocumentImgCustomAddressBook_Click" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>
<div class="row">
    <div class="col_full">
        <asp:UpdatePanel runat="server" ID="UpPnlCorrespondentCustom" UpdateMode="Conditional">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="TxtCodeCorrespondentCustomValue" />
            </Triggers>
            <ContentTemplate>
                <asp:HiddenField ID="IdCorrespondentCustomHidden" runat="server" />
                <div class="colHalf">
                    <cc1:CustomTextArea ID="TxtCodeCorrespondentCustomValue" runat="server" CssClass="txt_addressBookLeft"
                        CssClassReadOnly="txt_addressBookLeft_disabled" OnTextChanged="TxtCodeCorrespondentCustom_OnTextChanged"
                        AutoPostBack="true"></cc1:CustomTextArea>
                </div>
                <div class="colHalf2">
                    <div class="colHalf3">
                        <cc1:CustomTextArea ID="TxtDescriptionCorrespondentCustomValue" runat="server" CssClass="txt_addressBookRight"
                            CssClassReadOnly="txt_addressBookRight_disabled"></cc1:CustomTextArea>
                    </div>
                </div>
                  <uc1:AutoCompleteExtender runat="server" ID="RapidCorrespondentCustom" TargetControlID="TxtDescriptionCorrespondentCustomValue"
                    CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                    CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                    MinimumPrefixLength="3" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                    DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                    UseContextKey="true">
                </uc1:AutoCompleteExtender>
                 <asp:CheckBox runat="server" ID="ChkStoryCustomCorrespondent" Checked="true" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>
