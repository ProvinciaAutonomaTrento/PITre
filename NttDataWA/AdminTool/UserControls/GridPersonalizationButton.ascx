<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GridPersonalizationButton.ascx.cs"
    Inherits="SAAdminTool.UserControls.GridPersonalizationButton" %>
<%@ Register Src="~/UserControls/DocumentConsolidation.ascx" TagName="DocumentConsolidation"
    TagPrefix="uc2" %>
<script type="text/javascript" language="javascript">

    // L'URL della finestra per la personalizzazione delle griglie
    var _grids = '<%=UrlToGridManagement %>';
    //L'URL della finestra per ritornare al default delle griglie
    var _gridsDefault = '<%=UrlToDefaultGrid %>';
    //L'URL della finestra per le grigle preferite
    var _gridspreferred = '<%=UrlPreferredGrid %>';
    //L'URL della finestra per salvare le griglie
    var _gridsave = '<%=UrlSaveGrid %>';

    function OpenGrids(gridType, gridId, templateId, ricADL) {
        var newLeft = (screen.availWidth / 2 - 400);
        var newTop = (screen.availHeight / 2 - 300);

        var adlPar = '';
        if (ricADL != undefined && ricADL == 'ricADL')
            adlPar = "&ricADL=1";

        window.showModalDialog(
            _grids + "?gridType=" + gridType + "&gridId=" + gridId + "&templateId=" + templateId + adlPar,
            'GridCustomization',
            'dialogWidth:920px;dialogHeight:900px;status:no;resizable:no;scroll:no;dialogLeft:' + newLeft + ';dialogTop:' + newTop + ';center:no;help:no;', '');
    }

    function ReturnDefaultGrid(gridType, ricADL) {
        var newLeft = (screen.availWidth / 2 - 225);
        var newTop = (screen.availHeight / 2 - 75);

        var adlPar = '';
        if (ricADL != undefined && ricADL == 'ricADL')
            adlPar = "&ricADL=1";

        window.showModalDialog(
            _gridsDefault + "?gridType=" + gridType + adlPar,
            'GridDefault',
            'dialogWidth:400px;dialogHeight:200px;status:no;resizable:no;scroll:no;dialogLeft:' + newLeft + ';dialogTop:' + newTop + ';center:no;help:no;',
            '');
    }

    function OpenPreferredGrids(gridType, gridId, templateId, ricADL) {
        var newLeft = (screen.availWidth / 2 - 400);
        var newTop = (screen.availHeight / 2 - 300);

        var adlPar = '';
        if (ricADL != undefined && ricADL == 'ricADL')
            adlPar = "&ricADL=1";

        window.showModalDialog(
            _gridspreferred + "?gridType=" + gridType + "&gridId=" + gridId + "&templateId=" + templateId + adlPar,
            'GridPreferred',
            'dialogWidth:500px;dialogHeight:300px;status:no;resizable:no;scroll:no;dialogLeft:' + newLeft + ';dialogTop:' + newTop + ';center:no;help:no;', '');
    }

    function OpenSaveGrid(gridType, gridId, templateId, ricADL) {
        var newLeft = (screen.availWidth / 2 - 400);
        var newTop = (screen.availHeight / 2 - 300);

        var adlPar = '';
        if (ricADL != undefined && ricADL == 'ricADL')
            adlPar = "&ricADL=1";

        window.showModalDialog(
            _gridsave + "?gridType=" + gridType + "&gridId=" + gridId + "&templateId=" + templateId + adlPar,
            'GridPreferred',
            'dialogWidth:500px;dialogHeight:400px;status:no;resizable:no;scroll:no;dialogLeft:' + newLeft + ';dialogTop:' + newTop + ';center:no;help:no;', '');
    }
</script>
<asp:ImageButton ID="btnSalvaGrid" runat="server" Visible="true" AlternateText="Salva o modifica la griglia" ToolTip="Salva o modifica la griglia" ImageUrl="~/images/ricerca/ico_salva_griglia.gif" DisabledUrl="~/images/ricerca/ico_salva_griglia_disable.gif" />
<asp:ImageButton ID="btnCustomizeGrid" runat="server" Visible="true" AlternateText="Personalizza la griglia di ricerca" ToolTip="Personalizza la griglia di ricerca" ImageUrl="~/images/ricerca/ico_doc2.gif" />
<asp:ImageButton ID="btnPreferredGrid" runat="server" Visible="true" AlternateText="Mie griglie preferite" ToolTip="Mie griglie preferite" ImageUrl="~/images/ricerca/ico_griglie_preferite.gif" />
