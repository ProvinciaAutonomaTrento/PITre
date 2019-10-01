<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CompleteTask.aspx.cs" Inherits="NttDataWA.Popup.CompleteTask"
    MasterPageFile="~/MasterPages/Popup.Master" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%@ register src="~/UserControls/Calendar.ascx" tagprefix="uc6" tagname="Calendar" %>
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        $(function () {
            function charsLeft() {
                var maxLength = <%=maxLength%>;
                var actLength = $("#TxtCompleteTask").val().length;
                if (actLength>maxLength) {
                    $("#TxtCompleteTask").val($("#TxtCompleteTask").val().substring(0, maxLength-1));
                    actLength = maxLength;
                }
                $("#ltrTextCompleteTask_chars").text(maxLength - actLength);
            }
            
            $("#TxtCompleteTask").keyup(charsLeft);
            $("#TxtCompleteTask").change(charsLeft);
            charsLeft();
        });
    </script>
    <style type="text/css">
        #container
        {
            position: fixed;
            top: 1px;
            left: 0px;
            bottom: 71px;
            right: 0px;
            overflow: auto;
            background: #ffffff;
            text-align: left;
            padding: 10px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div class="container">
        <asp:UpdatePanel runat="server" ID="UpPnlDescription" UpdateMode="Conditional" ClientIDMode="Static">
            <ContentTemplate>
                <asp:Panel ID="pnlDataScadenza" runat="server" Visible="false">
                    <p>
                        <asp:Label runat="server" ID="LblDataScadenza" CssClass="NormalBold"></asp:Label></p>
                    <div class="row" style=" padding-left: 5px">
                        <cc1:CustomTextArea ID="txt_dataScadenza" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                            CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                    </div>
                </asp:Panel>
                <p>
                    <asp:Label runat="server" ID="lblNoteCompleteTask" CssClass="NormalBold"></asp:Label></p>
                <div class="row" style="padding-left: 5px">
                    <cc1:CustomTextArea runat="server" ID="TxtCompleteTask" Columns="50" Rows="4" CssClass="txt_textarea"
                        CssClassReadOnly="txt_textarea_disabled" TextMode="MultiLine" ClientIDMode="Static" MaxLength="2000"></cc1:CustomTextArea>
                </div>
                <div class="row">
                    <div class="col-right-no-margin">
                        <span class="charactersAvailable">
                            <asp:Literal ID="ltrTextCompleteTask" runat="server" ClientIDMode="Static"> </asp:Literal>
                            <span id="ltrTextCompleteTask_chars" clientidmode="Static" runat="server"></span>
                        </span>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="CompleteTaskOk" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClientClick="disallowOp('Content2');"
                OnClick="CompleteTaskOk_Click" />
            <cc1:CustomButton ID="CompleteTaskClose" runat="server" CssClass="btnEnable" OnClientClick="disallowOp('Content2');"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="CompleteTaskClose_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
