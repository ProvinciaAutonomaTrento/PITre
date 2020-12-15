<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DocumentConsolidation.ascx.cs" Inherits="DocsPAWA.UserControls.DocumentConsolidation" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary"%>
<script type="text/javascript">
    function confirmConsolidateStep1() {
        return confirm("<%=this.ConsolidateMessage1%>");

//        return confirm("Attenzione:\n" +
//                "Si sta per consolidare il documento, non sarà più possibile acquisire o modificare versioni.\n" +
//                "L'operazione ha carattere di irreversibilità. Si desidera continuare?");
    }

    function confirmConsolidateStep2() {
        return confirm("<%=this.ConsolidateMessage2%>");
        /*
        return confirm("Attenzione:\n" +
                "Si sta per consolidare il documento nei suoi metadati fondamentali. Inoltre non sarà più possibile acquisire o modificare versioni.\n" +
                "L'operazione ha carattere di irreversibilità. Si desidera continuare?");
        */
    }

    function showConsolidationSummary() {
        var pageHeight = 500;
        var pageWidth = 700;

        return window.showModalDialog('<%=DocsPAWA.Utils.getHttpFullPath()%>/MassiveOperation/DocumentConsolidationSummary.aspx',
            null, 
            'dialogWidth:' + pageWidth + 'px;dialogHeight:' + pageHeight + 'px;status:no;resizable:yes;scroll:yes;center:yes;help:no;close:no');
    }

    function openConsolidation(context) {
        var newLeft = (screen.availWidth / 2 - 85);
        var newTop = (screen.availHeight / 2 - 60);
        var dialogArgs = new Object();
        dialogArgs.window = window;
        dialogArgs.title = 'Consolidamento documenti';
        window.showModalDialog("<%=ConsolidationMassivePage%>?context=" + context, dialogArgs, 'dialogWidth:420px;dialogHeight:350px;status:no;resizable:no;scroll:no;center:yes;help:no;');
    }
</script>
<cc1:imagebutton id="btnConsolidateStep1" Runat="server" Tipologia="DO_CONSOLIDAMENTO"  ImageAlign="Middle" Height="16" ImageUrl="../images/ConsolidationStep1.gif" AlternateText="Consolida documento" ToolTip="Consolida documento" OnClick="btnConsolidateStep1_OnClick"></cc1:imagebutton>													
<cc1:imagebutton id="btnConsolidateStep2" Runat="server" Tipologia="DO_CONSOLIDAMENTO_METADATI" ImageAlign="Middle" Height="16" ImageUrl="../images/ConsolidationStep2.gif" AlternateText="Consolida documento e metadati" ToolTip="Consolida documento e metadati" OnClick="btnConsolidateStep2_OnClick"></cc1:imagebutton>													
