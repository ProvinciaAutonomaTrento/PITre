<%@ Page Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="version_add.aspx.cs" Inherits="NttDataWA.Popup.version_add" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        p {text-align: left;}
        .red {color: #f00;}
        .col-right {float: right;}
    </style>
    <script type="text/javascript">

        $(function () {
            function charsLeft() {
                var maxLength = <%=maxLength%>;
                var actLength = $("#VersionDescription").val().length;
                if (actLength>maxLength) {
                    $("#VersionDescription").val($("#VersionDescription").val().substring(0, maxLength-1));
                    actLength = maxLength;
                }
                $("#VersionDescription_chars").text(maxLength - actLength);
            }
            
            $("#VersionDescription").keyup(charsLeft);
            $("#VersionDescription").change(charsLeft);
            charsLeft();

            /*$("#VersionBtn").click(function () {
                $("#VersionDescription_err").remove();
                if ($("#VersionDescription").val().length == 0) {
                    $("#VersionDescription").focus();
                    $("#VersionDescription").before('<span id="VersionDescription_err" class="red">Campo obbligatorio</span>');
                    return false;
                }
            });*/

            $("#VersionDescription").focus();
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
            <div id="rowMessage" runat="server" />
            <p id="rowDescription" runat="server">
                <span class="weight"><asp:Literal ID="VersionLitDescription" runat="server"></asp:Literal></span><br />
                <cc1:CustomTextArea ID="VersionDescription" style="margin-left:5px; overflow:hidden;" runat="server" Columns="50" Rows="6" TextMode="MultiLine" MaxLength="200" ClientIDMode="Static"></cc1:CustomTextArea><br />
                <span class="col-right"><asp:Literal ID="VersionsLitChars" runat="server"></asp:Literal> <span id="VersionDescription_chars"></span></span>
            </p>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
            <cc1:CustomButton ID="VersionBtn" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Salva" ClientIDMode="Static" onclick="VersionBtn_Click" OnClientClick="disallowOp('Content1')"/>
            <cc1:CustomButton ID="VersionBtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Chiudi" ClientIDMode="Static" OnClientClick="disallowOp('Content1')" onclick="VersionBtnClose_Click" />
</asp:Content>
