<%@ Page Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="attachment.aspx.cs" Inherits="NttDataWA.Popup.attachment" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .container {width: 95%; margin: 0 auto;}
        p {text-align: left;}
        .col-right {float: right; font-size: 0.8em;}
        #AttachmentDescription {width: 100%;}
    </style>
    <script type="text/javascript">
        $(function () {
            function charsLeft() {
                var maxLength = <%=maxLength%>;
                var actLength = $("#AttachmentDescription").val().length;
                if (actLength>maxLength) {
                    $("#AttachmentDescription").val($("#AttachmentDescription").val().substring(0, maxLength-1));
                    actLength = maxLength;
                }
                $("#AttachmentDescription_chars").text(maxLength - actLength);
            }
            
            $("#AttachmentDescription").keyup(charsLeft);
            $("#AttachmentDescription").change(charsLeft);
            charsLeft();

            $("#AttachmentPagesCount").keydown(function (event) {
                // Allow: backspace, delete, tab, escape, and enter
                if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 27 || event.keyCode == 13 ||
                // Allow: Ctrl+A
                (event.keyCode == 65 && event.ctrlKey === true) ||
                // Allow: home, end, left, right
                (event.keyCode >= 35 && event.keyCode <= 39)) {
                    // let it happen, don't do anything
                    return;
                }
                else {
                    // Ensure that it is a number and stop the keypress
                    if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)) {
                        event.preventDefault();
                    }
                }
            });

            $("#AttachmentBtn").click(function () {
                $("#AttachmentDescription_err").remove();
                if ($("#AttachmentDescription").val().length == 0) {
                    $("#AttachmentDescription").focus();
                    $("#AttachmentDescription").before('<span id="AttachmentDescription_err" class="red">Campo obbligatorio</span>');
                    return false;
                }
            });

            $("#AttachmentDescription").focus();
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
            <div class="container">
                <div id="rowMessage" runat="server" />
                <p id="rowCode" runat="server">
                    <strong>Codice <asp:Literal ID="AttachmentCode" runat="server" /></strong><br />
                </p>
                <p id="rowDescription" runat="server">
                    <strong><asp:Literal ID="AttachmentLitDescription" runat="server"></asp:Literal></strong><asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="<br />Campo obbligatorio" ControlToValidate="AttachmentDescription" EnableClientScript="false" ForeColor="Red"></asp:RequiredFieldValidator><br />
                    <cc1:CustomTextArea ID="AttachmentDescription" runat="server" Columns="50" Rows="4" TextMode="MultiLine" ClientIDMode="Static" CssClass="txt_textarea"></cc1:CustomTextArea><br />
                    <span class="col-right"><asp:Literal ID="AttachmentsLitAvChars" runat="server"></asp:Literal><span id="AttachmentDescription_chars"></span></span>
                </p>
                <p id="rowPagesCount" runat="server">
                    <strong><asp:Literal ID="AttachmentLitPagesNr" runat="server"></asp:Literal></strong><br />
                    <cc1:CustomTextArea ID="AttachmentPagesCount" MaxLength="5" runat="server" ClientIDMode="Static" Columns="5" CssClass="txt_textdata" ></cc1:CustomTextArea><br />
                </p>
            </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
            <cc1:CustomButton ID="AttachmentBtn" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Salva" onclick="AttachmentBtn_Click" OnClientClick="disallowOp('Content2');" />
            <cc1:CustomButton ID="AttachmentBtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Chiudi" onclick="AttachmentBtnClose_Click" />
</asp:Content>
