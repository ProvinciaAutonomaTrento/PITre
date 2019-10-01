<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ajaxpopup.ascx.cs" Inherits="AjaxPopupControl.UserControls.ajaxpopup2" %>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Button ID="button" runat="server" Text="Button" />
        <asp:Panel ID="panel" runat="server" />
        <asp:HiddenField ID="hidden" runat="server" />
        <script type="text/javascript">
            function ajaxModalPopup<%=Id%>() {
                $('#<%=panel.ClientID%>').load('<%=Url%>', function() {
                    $('#<%=panel.ClientID%>').dialog({
                        closeOnEscape: false,
                        resizable: false,
                        draggable: false,
                        modal: true,
                        show: 'puff',
                        hide:  'clip',
                        stack: false,
                        title: '<%=Title.Replace("'", "\'") %>',
                        width: <%=Width %>,
                        height: <%=Height %>
                    });
                });
            }
        </script>
    </ContentTemplate>
</asp:UpdatePanel>