<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ajaxpopup2.ascx.cs" Inherits="AjaxPopupControl.UserControls.ajaxpopup21" %>
<span class="retval<%=Id%>"><asp:HiddenField ID="retval" runat="server"></asp:HiddenField></span>
<span class="already<%=Id%>"><asp:HiddenField ID="already" runat="server"></asp:HiddenField></span>
<script type="text/javascript">
    function ajaxModalPopup<%=Id%>() {
//        if ($('#<%=Id+"_panel"%>').is(':data(dialog)') || $('.already<%=Id%> input').get(0).value=='true') {
//            $('#<%=Id+"_panel"%>').dialog('open');
//        }
//        else {
            $('.already<%=Id%> input').get(0).value = 'true';
            var isFullScreen = <%=IsFullScreen.ToString().ToLower() %>;
            var permitScroll = 'auto';
            if (!<%=PermitScroll.ToString().ToLower() %>) permitScroll = 'no';
            var width = <%=Width %>;
            var height = <%=Height %>;
            if ($(window).width() - 10 < width || isFullScreen) width = $(window).width() - 10;
            if ($(window).height() - 20 < height || isFullScreen) height = $(window).height() - 20;
            var showLoading = <%=ShowLoading.ToString().ToLower() %>;
            $('#<%=Id+"_panel"%>').empty();
            var d = $('#<%=Id+"_panel"%>').html($('<iframe id="ifrm2_<%=Id%>" frameborder="0" src="<%=Page.ResolveClientUrl("~/Popup/loading.htm")%>" scrolling="no" width="99%" height="99%" /><iframe id="ifrm_<%=Id%>" frameborder="0" />'));
            if (!showLoading) d = $('#<%=Id+"_panel"%>').html($('<iframe id="ifrm_<%=Id%>" frameborder="0" src="<%=Url%>" scrolling="'+permitScroll+'" width="99%" height="99%" marginwidth="0" marginheight="0" />'));
            d.dialog({
                <asp:PlaceHolder id="dontShowClose" runat="server" visible="false">open: function (event, ui) { $('.ui-dialog-titlebar-close').hide(); },</asp:PlaceHolder>
                autoOpen: false,
                closeOnEscape: false,
                position: ['<%=PosH%>', '<%=PosV%>'],
                resizable: false,
                draggable: true,
                modal: true,
                show: 'puff',
                hide:  'clip',
                stack: false,
                title: '<%=NttDataWA.Utils.utils.FormatJs(Title) %>',
                <%=CloseFunction%>
                width: width,
                height: height
            });
            if (showLoading) {
                $('#<%=Id+"_panel"%>>#ifrm_<%=Id%>').hide();
                setTimeout('$("#<%=Id+"_panel"%>>#ifrm_<%=Id%>").attr({ src: \'<%=Url %>\', width: \'99%\', height: \'99%\', marginwidth: \'0\', marginheight: \'0\', scrolling: \''+permitScroll+'\' });', 500);
            }
            $('#<%=Id+"_panel"%>').dialog('open');
//        }
        
        // fix per INC000000385597
        // nascondo il contenuto del frame (visualizzatore) SOLO se si sta utilizzando IE
        var browser = '<%=Request.Browser.Browser.Trim().ToUpperInvariant() %>';
        if(browser == 'IE' || browser == 'INTERNETEXPLORER') {
            $('#frame').css("visibility", "hidden");
            //$('#frame').hide();
        }

        //$('#frame').hide();

        return false;
    }
        function ajaxModalPopupTitle<%=Id%>(titleI) {
//        if ($('#<%=Id+"_panel"%>').is(':data(dialog)') || $('.already<%=Id%> input').get(0).value=='true') {
//            $('#<%=Id+"_panel"%>').dialog('open');
//        }
//        else {
            $('.already<%=Id%> input').get(0).value = 'true';
            var isFullScreen = <%=IsFullScreen.ToString().ToLower() %>;
            var permitScroll = 'auto';
            if (!<%=PermitScroll.ToString().ToLower() %>) permitScroll = 'no';
            var width = <%=Width %>;
            var height = <%=Height %>;
            if ($(window).width() - 10 < width || isFullScreen) width = $(window).width() - 10;
            if ($(window).height() - 20 < height || isFullScreen) height = $(window).height() - 20;
            var showLoading = <%=ShowLoading.ToString().ToLower() %>;
            $('#<%=Id+"_panel"%>').empty();
            var d = $('#<%=Id+"_panel"%>').html($('<iframe id="ifrm2_<%=Id%>" frameborder="0" src="<%=Page.ResolveClientUrl("~/Popup/loading.htm")%>" scrolling="no" width="99%" height="99%" /><iframe id="ifrm_<%=Id%>" frameborder="0" />'));
            if (!showLoading) d = $('#<%=Id+"_panel"%>').html($('<iframe id="ifrm_<%=Id%>" frameborder="0" src="<%=Url%>" scrolling="'+permitScroll+'" width="99%" height="99%" marginwidth="0" marginheight="0" />'));
            d.dialog({
                <asp:PlaceHolder id="dontShowClose1" runat="server" visible="false">open: function (event, ui) { $('.ui-dialog-titlebar-close').hide(); },</asp:PlaceHolder>
                autoOpen: false,
                closeOnEscape: false,
                position: ['<%=PosH%>', '<%=PosV%>'],
                resizable: false,
                draggable: true,
                modal: true,
                show: 'puff',
                hide:  'clip',
                stack: false,
                title: titleI,
                <%=CloseFunction%>
                width: width,
                height: height
            });
            if (showLoading) {
                $('#<%=Id+"_panel"%>>#ifrm_<%=Id%>').hide();
                setTimeout('$("#<%=Id+"_panel"%>>#ifrm_<%=Id%>").attr({ src: \'<%=Url %>\', width: \'99%\', height: \'99%\', marginwidth: \'0\', marginheight: \'0\', scrolling: \''+permitScroll+'\' });', 500);
            }
            $('#<%=Id+"_panel"%>').dialog('open');
//        }
        
        // fix per INC000000385597
        // nascondo il contenuto del frame (visualizzatore) SOLO se si sta utilizzando IE
        var browser = '<%=Request.Browser.Browser.Trim().ToUpperInvariant() %>';
        if(browser == 'IE' || browser == 'INTERNETEXPLORER') {
            $('#frame').css("visibility", "hidden");
            //$('#frame').hide();
        }

        //$('#frame').hide();

        return false;
    }
</script>
<asp:Panel ID="pnl" runat="server"></asp:Panel>
