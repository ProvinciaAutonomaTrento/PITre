<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ajaxpopup_old.ascx.cs" Inherits="AjaxPopupControl.WebUserControl1" %>
<script type="text/javascript">
    $(function () {
        $('#<%=PopupButtonId%>').click(function() {
            $('#<%=PopupLayerId%>').load('<%=Url%>', function() {
                $('#<%=PopupLayerId%>').dialog({
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
        });
    });
</script>
<asp:PlaceHolder ID="plc" runat="server"></asp:PlaceHolder>