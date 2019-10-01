<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ajaxConfirmPopUpModal.ascx.cs" Inherits="NttDataWA.UserControls.ajaxConfirmPopUpModal" %>
  <!-- Popup modal confirm -->
    <script type="text/javascript">
        function ajaxConfirmPopUpModal(msg, hiddenToValorize) {
            var titleW = null;
            var input = null;
            var popupWidth = 400;
            var popupHeight = 300;
            var closeFunction = null;
            if (arguments.length > 2 && arguments[2] != null) titleW = arguments[2];
            if (arguments.length > 3 && arguments[3] != null) input = arguments[3];
            if (arguments.length > 4 && arguments[4] != null) popupWidth = arguments[4];
            if (arguments.length > 5 && arguments[5] != null) popupHeight = arguments[5];
            if (arguments.length > 6 && arguments[6] != null) closeFunction = arguments[6];
            if (titleW == null || titleW == '') titleW = '<asp:Literal id="litConfirm" runat="server" />';

            $('#confirmpopup_modal').empty();
            var d = $('#confirmpopup_modal').html($('<iframe id="ifrm_confirmpopup" frameborder="0" />'));
            d.dialog({
                open: function (event, ui) { $('.ui-dialog-titlebar-close').hide(); },
                close: function (event, ui) { if (closeFunction != null) eval(closeFunction); },
                position: { my: "center", at: "center", of: window },
                resizable: false,
                draggable: true,
                modal: true,
                show: 'puff',
                hide: 'clip',
                stack: true,
                title: titleW,
                width: popupWidth,
                height: popupHeight
            });
            $("#confirmpopup_modal #ifrm_confirmpopup").attr({ src: '<%=Page.ResolveClientUrl("~/Popup/ConfirmPopUp.aspx") %>?hidden=' + hiddenToValorize + '&msg=' + msg + '&input=' + input, width: '99%', height: '99%', marginwidth: '0', marginheight: '0', scrolling: 'auto' });
        };
    </script>
    <div id="confirmpopup_modal">
    </div>