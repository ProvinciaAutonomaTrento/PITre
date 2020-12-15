<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InterruptionSignatureProcess.aspx.cs"
    Inherits="NttDataWA.Popup.InterruptionSignatureProcess" MasterPageFile="~/MasterPages/Popup.Master" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        $(function () {
            function charsLeft() {
                var maxLength = <%=maxLength%>;
                var actLength = $("#TxtTextAbortRecord").val().length;
                if (actLength>maxLength) {
                    $("#TxtTextAbortRecord").val($("#TxtTextAbortRecord").val().substring(0, maxLength-1));
                    actLength = maxLength;
                }
                $("#ltrTextAbortRecord_chars").text(maxLength - actLength);
            }
            
            $("#TxtTextAbortRecord").keyup(charsLeft);
            $("#TxtTextAbortRecord").change(charsLeft);
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
                <p>
                    <asp:Label runat="server" ID="lblNoteInterruption" CssClass="NormalBold"></asp:Label></p>
                <div class="row" style="padding: 5px">
                    <cc1:CustomTextArea runat="server" ID="TxtTextAbortRecord" Columns="50" Rows="4"
                        CssClass="txt_textarea" CssClassReadOnly="txt_textarea_disabled" TextMode="MultiLine"
                        ClientIDMode="Static"></cc1:CustomTextArea>
                </div>
                <div class="row">
                    <div class="col-right-no-margin">
                        <span class="charactersAvailable">
                            <asp:Literal ID="ltrTextAbortRecord" runat="server" ClientIDMode="Static"> </asp:Literal>
                            <span id="ltrTextAbortRecord_chars" clientidmode="Static" runat="server"></span>
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
            <cc1:CustomButton ID="InterruptionSignatureProcessOk" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClientClick="disallowOp('Content2');"
                OnClick="InterruptionSignatureProcessOk_Click" />
            <cc1:CustomButton ID="InterruptionSignatureProcessClose" runat="server" CssClass="btnEnable" OnClientClick="disallowOp('Content2');"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="InterruptionSignatureProcessClose_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
