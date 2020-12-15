<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="NttDataWA.Options.ChangePassword" MasterPageFile="~/MasterPages/Base.Master"  %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
        <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        $(function () {
            $('.defaultAction').keypress(function (e) {
                if (e.which == 13) {
                    e.preventDefault();
                    $('#BtnChangePasswordConfirm').click();
                }
            });
        });
    </script>

</asp:Content>

<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div id="containerTop">
        <div id="containerDocumentTop">
            <div id="containerStandardTop">
                <div id="containerStandardTopSx">
                </div>
                <div id="containerStandardTopCx">
                    <p>
                        <asp:Label ID="ChangePasswordLbl" runat="server"/>
                    </p>
                </div>
                <div id="containerStandardTopDx">
                </div>
            </div>
            <div id="containerStandardBottom">
                <div id="containerProjectCxBottom">
                </div>
            </div>
        </div>
    </div>
    <div id="containerStandard" runat="server" clientidmode="Static">
        <div id="content">
            <div id="box_inside" style=" padding-left:10px; padding-top:5px">
                <asp:HiddenField ID="HiddenChangePassword" runat="server" ClientIDMode="Static" />
                <asp:HiddenField ID="HiddenChangePasswordMultiAministrator" runat="server" ClientIDMode="Static" />
                <asp:HiddenField ID="hdnMultiAdmNewPass" runat="server" ClientIDMode="Static" />
                <%-- ************** OLD PASSWORD ******************** --%>
                <div class="row3">
                    <asp:UpdatePanel ID="UpPnlOldPassword" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="colHalf9">
                                <strong>
                                    <asp:Literal ID="lt_oldPassword" runat="server"></asp:Literal>
                                </strong>
                            </div>
                            <div class="colHalf">
                                <cc1:CustomTextArea ID="TxtOldPassword" Width="100%" runat="server" CssClass="txt_input_full defaultAction"
                                    CssClassReadOnly="txt_input_full_disabled" TextMode="Password" MaxLength="30" ClientIDMode="Static"></cc1:CustomTextArea>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>

                <%-- ************** NEW PASSWORD ******************** --%>
                <div class="row3">
                    <asp:UpdatePanel ID="UpPnlNewPassword" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="colHalf9">
                                <strong>
                                    <asp:Literal ID="lt_NewPassword" runat="server" />
                                </strong>
                            </div>
                            <div class="colHalf">
                                <cc1:CustomTextArea ID="TxtNewPassword" Width="100%" runat="server" CssClass="txt_input_full defaultAction"
                                    CssClassReadOnly="txt_input_full_disabled" TextMode="Password" MaxLength="30" ClientIDMode="Static"></cc1:CustomTextArea>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <%-- ************** CONFIRM PASSWORD ******************** --%>
                <div class="row3">
                    <asp:UpdatePanel ID="UpPnlConfirmPassword" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="colHalf9">
                                <strong>
                                    <asp:Literal ID="lt_ConfirmPassword" runat="server"></asp:Literal>
                                </strong>
                            </div>
                            <div class="colHalf">
                                <cc1:CustomTextArea ID="TxtConfirmPassword" Width="100%" runat="server" CssClass="txt_input_full defaultAction"
                                    CssClassReadOnly="txt_input_full_disabled" TextMode="Password" MaxLength="30" ClientIDMode="Static"></cc1:CustomTextArea>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" UpdateMode="Conditional" runat="server" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="BtnChangePasswordConfirm" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                    OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnChangePasswordConfirm_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>