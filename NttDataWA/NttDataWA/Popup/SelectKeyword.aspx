<%@ Page Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true"
    CodeBehind="SelectKeyword.aspx.cs" Inherits="NttDataWA.Popup.SelectKeyword" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <uc:ajaxpopup2 Id="AddKeyword" runat="server" Url="../popup/AddKeyword.aspx" Width="400"
        Height="175" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpdPnlListKeywords', '');}" />
    <div id="selectKeyword">
        <fieldset>
            <asp:UpdatePanel ID="UpdPnlListKeywords" runat="server" UpdateMode="Conditional"
                ClientIDMode="static">
                <ContentTemplate>
                    <div class="row">
                        <div class="col">
                            <span class="weight">
                                <asp:Label ID="SelectKeywordLbl" runat="server"></asp:Label></span>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-left-no-margin-key">
                           
                                <asp:ListBox ID="ListParoleChiave" runat="server" SelectionMode="Multiple" Height="250px"
                                    CssClass="txt_textarea" CssClassReadOnly="txt_textarea_disabled"></asp:ListBox>
                           
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </fieldset>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="uppnlButtons" runat="server">
        <ContentTemplate>
            <div style="float: left">
                <cc1:CustomButton ID="SelectKeywordBtnOk" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                    OnMouseOver="btnHover" OnClick="SelectKeywordBtnOk_Click" />
                <cc1:CustomButton ID="SelectKeywordBtnAdd" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                    OnMouseOver="btnHover" OnClick="SelectKeywordBtnAdd_Click" />
                <cc1:CustomButton ID="SelectKeywordBtnChiudi" runat="server" CssClass="btnEnable"
                    CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="SelectKeywordBtnChiudi_Click" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
        $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
    </script>
</asp:Content>
