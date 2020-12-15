<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="HSM_Signature.aspx.cs" Inherits="ConservazioneWA.HSM_Signature" %>

<%@ Register Assembly="ConservazioneWA" Namespace="ConservazioneWA.UserControl" TagPrefix="cc1" %>
<head id="Head1" runat="server">
    <title></title>
    <link runat="server" type="text/css" rel="stylesheet" id="CssLayout" />
    <link href="CSS/Conservazione.css" type="text/css" rel="stylesheet" />
    <style type="text/css">
    .cbtn
        {
            font-family: Verdana;
            font-size: 10px;
            color: White;
            background-image: url('Img/bg_button.jpg');
        }
        
    .cbtnHover
        {
            font-family: Verdana;
            font-size: 10px;
            color: White;
            background-image: url('Img/bg_button_hover.jpg');
        }
    .cbtnDisabled
        {
             background-color:#f2f2f2;
             border:1px solid #dcdcdc;
             font-family: Verdana;
             font-size: 10px;
             margin: 0px;
             padding: 0px;
             padding: 2px;
             width: 130px;
             height: 25px;
             color: #dcdcdc;
             font-weight: bold;
             margin: 5px;
        }
    </style>
    <script type="text/javascript">
        $(document).click(function (event) {
            if (event.target.id != "showHistory") {
                $('#divHistory:visible').hide();
            }
            else {
                $('.bullet').toggle();
            }
        });

        $(function () {
            Tipsy();
            reallowOp();
        });

        function sessionend() {
            var frs = parent.frames.fra_sessionend;
            if (frs.document.getElementById('user_id') != null)
                frs.document.getElementById('user_id').value = '';
        }

        function Tipsy() {
            $(".tipsy").remove();
            $('.tooltip').tipsy();
            $('.clickable').tipsy({ gravity: 's', fade: false, opacity: 1, delayIn: 0, delayOut: 0 });
            $('.clickableLeft').tipsy({ gravity: 'e', fade: false, opacity: 1, delayIn: 0, delayOut: 0 });
            $('.redStrike').tipsy({ gravity: 's', fade: false, opacity: 1, delayIn: 0, delayOut: 0 });
            $('.clickableUnderline').tipsy({ gravity: 's', fade: false, opacity: 1, delayIn: 0, delayOut: 0 });
            $('.referenceCode').tipsy({ className: 'reference-tip', gravity: 'n', fade: false, opacity: 1, delayIn: 0, delayOut: 0 });
            $('.repSpedLongTxt').tipsy({ className: 'repsped_longtxt', gravity: 'n', fade: false, opacity: 1, delayIn: 0, delayOut: 0 });
            $('.clickableLeftN').tipsy({ gravity: 'e', fade: false, opacity: 1, delayIn: 0, delayOut: 0 });
            $('.clickableRight').tipsy({ gravity: 'w', fade: false, opacity: 1, delayIn: 0, delayOut: 0, html: true });
            $('.clickableNE').tipsy({ gravity: 'ne', fade: false, opacity: 1, delayIn: 0, delayOut: 0 });

            var isIEmin9 = false;
            if ($.browser.msie && $.browser.version < 10) isIEmin9 = true;
            if (!isIEmin9) {
                $('.tooltip-no-ie').tipsy();
                $('.clickable-no-ie').tipsy({ gravity: 's', fade: false, opacity: 1, delayIn: 0, delayOut: 0 });
                $('.clickableLeft-no-ie').tipsy({ gravity: 'e', fade: false, opacity: 1, delayIn: 0, delayOut: 0 });
                $('.redStrike-no-ie').tipsy({ gravity: 's', fade: false, opacity: 1, delayIn: 0, delayOut: 0 });
            }
        }

        function signHSM() {
            var txtHsmAlias = document.getElementById('TxtHsmAlias').value;
            var txtHsmDomain = document.getElementById('TxtHsmDomain').value;
            var txtHsmPin = document.getElementById('TxtHsmPin').value;
            var txtHsmLitOtp = document.getElementById('TxtHsmLitOtp').value;

            if (txtHsmAlias != '' && txtHsmDomain != '' && txtHsmPin != '' && txtHsmLitOtp != '') {
                return true;
            }
            else {
                alert('Inserire tutti i campi obbligatori.');
                return false;
            }
        }
    </script>
</head>
<body id="IdMasterBody" runat="server">
    <form id="FrmHSM" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="3600"
            EnablePageMethods="true" ScriptMode="Release">
            <Scripts>
                <asp:ScriptReference Path="~/Scripts/webkit.js" />
            </Scripts>
        </asp:ScriptManager>
        <div class="box_cerca">
                <div align="center">
                    <fieldset>
                        <legend>Firma HSM</legend>
                        <div>
                            <p style="margin-left: 20px; font-weight: bold;">
                                <asp:Label ID="HsmLitAlias" runat="server"></asp:Label></p>
                            <p style="margin-left: 20px;">
                                <cc1:CustomTextArea runat="server" ID="TxtHsmAlias" CssClass="txt_hsmtxt"></cc1:CustomTextArea></p>
                        </div>
                        <div>
                            <p style="margin-left: 20px; font-weight: bold;">
                                <asp:Label ID="HsmLitDomain" runat="server"></asp:Label></p>
                            <p style="margin-left: 20px;">
                                <cc1:CustomTextArea runat="server" ID="TxtHsmDomain" CssClass="txt_hsmtxt"></cc1:CustomTextArea></p>
                        </div>
                        <div>
                           <p style="margin-left: 20px; font-weight: bold;">
                                <asp:Label ID="HsmLitPin" runat="server"></asp:Label></p>
                            <p style="margin-left: 20px;">
                                <cc1:CustomTextArea runat="server" ID="TxtHsmPin" CssClass="txt_hsmtxt" TextMode="Password"></cc1:CustomTextArea></p>
                        </div>
                        <div>
                            <p style="margin-left: 20px; font-weight: bold;">
                                <asp:Label ID="HsmLitOtp" runat="server"></asp:Label></p>
                            <p style="margin-left: 20px;">
                                <cc1:CustomTextArea runat="server" ID="TxtHsmLitOtp" CssClass="txt_hsmtxt" TextMode="Password"></cc1:CustomTextArea></p>
                        </div>
                    </fieldset>
                </div>
        </div>
        <div align="center">
            <asp:UpdatePanel ID="UpPnlButtons" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Button ID="BtnRequestOTP" runat="server" ClientIDMode="Static" OnClick="BtnRequestOTP_Click" />
                    <asp:Button ID="BtnSign" runat="server" ClientIDMode="Static" OnClick="BtnSign_Click" />
                    <asp:Button ID="BtnClose" runat="server" ClientIDMode="Static" OnClick="BtnClose_Click" OnClientClick="window.close();" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </form>
 </body>