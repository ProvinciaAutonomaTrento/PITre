<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ajaxpopup2.ascx.cs" Inherits="AjaxPopupControl.UserControls.ajaxpopup21" %>
<asp:PlaceHolder ID="plc" runat="server" />
<span class="retval"><asp:HiddenField ID="retval" runat="server"></asp:HiddenField></span>
<script type="text/javascript">
    function ajaxModalPopup<%=Id%>() {
        var isFullScreen = <%=IsFullScreen.ToString().ToLower() %>;
        var permitScroll = 'auto';
        if (!<%=PermitScroll.ToString().ToLower() %>) permitScroll = 'no';
        var width = <%=Width %>;
        var height = <%=Height %>;
        if ($(window).width() - 100 < width || isFullScreen) width = $(window).width() - 100;
        if ($(window).height() - 100 < height || isFullScreen) height = $(window).height() - 100;

        $('#<%=Id+"_panel"%>').empty();
        var d = $('#<%=Id+"_panel"%>').html($('<iframe id="ifrm" frameborder="0" />'));
        d.dialog({
            <asp:Literal id="dontShowClose" runat="server" visible="false">open: function (event, ui) { $('.ui-dialog-titlebar-close').hide(); },</asp:Literal>
            closeOnEscape: false,
            position: ['<%=PosH%>', '<%=PosV%>'],
            resizable: false,
            draggable: false,
            modal: true,
            show: 'puff',
            hide:  'clip',
            stack: false,
            title: '<%=Title.Replace("'", "\'") %>',
            width: width,
            height: height
        });
        $("#<%=Id+"_panel"%>>#ifrm").attr({ src: '<%=Url%>', width: '99%', height: '99%', marginwidth: '0', marginheight: '0', scrolling: permitScroll });

        return false;
    }
</script>

<asp:Panel ID="pnl" runat="server"></asp:Panel>