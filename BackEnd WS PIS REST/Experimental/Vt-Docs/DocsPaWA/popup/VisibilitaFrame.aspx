<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VisibilitaFrame.aspx.cs"
    Inherits="DocsPAWA.popup.VisibilitaFrame" EnableEventValidation="false" %>

<%@ Register Src="../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<%@ Register Assembly="DocsPaWebCtrlLibrary" Namespace="DocsPaWebCtrlLibrary" TagPrefix="cc1" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title></title>
    <link href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
    <base target="_self" />
    <script language="javascript">
        function checkField(tbx_anno, tbx_numProto, tbxDoc) {
            var value_tbx_anno = document.getElementById(tbx_anno);
            var value_tbx_numProto = document.getElementById(tbx_numProto);
            var value_tbxDoc = document.getElementById(tbxDoc);

            if ((value_tbx_anno != null) && (value_tbx_numProto != null)) {
                if ((value_tbx_anno.value != '') && (value_tbx_numProto.value != '')) {
                    if (isNaN(value_tbx_anno.value) && isNaN(value_tbx_numProto.value)) {
                        alert('Attenzione sono ammessi solo valori numerici');
                        return false;
                    }
                    else {
                        if (isNaN(value_tbx_anno.value)) {
                            alert('Attenzione sono ammessi solo valori numerici per Anno');
                            document.form_filter.tbx_anno.select();
                            return false;
                        }

                        if (isNaN(value_tbx_numProto.value)) {
                            alert('Attenzione sono ammessi solo valori numerici per Nr. Protocollo');
                            document.form_filter.tbx_numProto.select();
                            return false;
                        }
                    }
                }
            }

            if (value_tbxDoc != null) {
                if (value_tbxDoc.value != '') {
                    if (isNaN(value_tbxDoc.value)) {
                        alert('Attenzione sono ammessi solo valori numerici per ID Documento');
                        document.form_filter.tbxDoc.select();
                        return false;
                    }
                }
                else {
                    alert('Attenzione inserire i campi richiesti');
                    return false;
                }
            }
        }

        function CloseIt() {
            window.close();
            return false;
        }

        function OpenHelp(from) {
            var pageHeight = 600;
            var pageWidth = 800;
            var posTop = (screen.availHeight - pageHeight) / 2;
            var posLeft = (screen.availWidth - pageWidth) / 2;

            var newwin = window.showModalDialog('../Help/Manuale.aspx?from=' + from,
							    '',
							    'dialogWidth:' + pageWidth + 'px;dialogHeight:' + pageHeight + 'px;status:no;resizable:no;scroll:yes;dialogLeft:' + posLeft + ';dialogTop:' + posTop + ';center:yes;help:no');
        }							
    </script>
</head>
<body topmargin="4">
    <form id="form_filter" runat="server" defaultbutton="btn_cerca">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="Ricerca Visibilità Documento" />
    <table id="contenitore" align="center" width="600">
        <tr>
            <td>
                <table border="0" cellpadding="0" cellspacing="0" width="100%" align="center">
                    <tr>
                        <td align="center" class="titolo_scheda">
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;VISIBILITA'
                            DOCUMENTI
                        </td>
                        <td align="right">
                            <asp:ImageButton ID="help" runat="server" OnClientClick="OpenHelp('RicercaVis')"
                                AlternateText="Aiuto?" SkinID="btnHelp" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <table id="table_filtri" width="100%" border="0">
                    <tr id="filtriGrigiProt">
                        <td class="info">
                            <asp:RadioButtonList ID="rblTipo" runat="server" CssClass="testo_grigio_scuro" RepeatDirection="Horizontal"
                                OnSelectedIndexChanged="rblTipo_SelectedIndexChanged" AutoPostBack="True">
                                <asp:ListItem Selected="True" Value="P">Protocollati</asp:ListItem>
                                <asp:ListItem Value="NP">Non Protocollati</asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                    <asp:Panel ID="pnlProt" runat="server">
                        <tr id="filtri" class="info">
                            <td>
                                <table border="0" cellpadding="0" cellspacing="0" width="90%">
                                    <tr>
                                        <td>
                                            &nbsp;<asp:Label ID="lbl_registri" runat="server" Text="Registro:" CssClass="testo_grigio_scuro"></asp:Label>
                                            &nbsp;
                                            <asp:DropDownList ID="ddl_registri" runat="server" CssClass="testo_grigio" Width="150px"
                                                OnSelectedIndexChanged="ddl_registri_SelectedIndexChanged">
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:Label ID="lbl_anno" runat="server" Text="Anno:" CssClass="testo_grigio_scuro"></asp:Label>
                                            &nbsp;
                                            <asp:TextBox ID="tbx_anno" runat="server" CssClass="testo_grigio" Width="50px"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Label ID="lbl_numProto" runat="server" Text="Nr. Protocollo:" CssClass="testo_grigio_scuro"></asp:Label>
                                            &nbsp;
                                            <asp:TextBox ID="tbx_numProto" runat="server" CssClass="testo_grigio" Width="50px"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </asp:Panel>
                    <asp:Panel ID="pnlDOC" runat="server" Visible="false">
                        <tr id="Documenti" class="info">
                            <td>
                                &nbsp;<asp:Label ID="lblDOc" runat="server" Text="ID Documento:" CssClass="testo_grigio_scuro"></asp:Label>
                                &nbsp;
                                <asp:TextBox ID="tbxDoc" runat="server" CssClass="testo_grigio" Width="60px"></asp:TextBox>
                        </tr>
                    </asp:Panel>
                    <asp:Panel ID="pnlTipologia" runat="server" Visible="false">
                        <tr>
                            <td class="titolo_scheda" valign="middle" height="25">
                                &nbsp;&nbsp;Tipologia documento &nbsp;<asp:DropDownList ID="ddl_tipologiaDoc" runat="server"
                                    Width="200px" CssClass="testo_grigio" AutoPostBack="True" OnSelectedIndexChanged="ddl_tipologiaDoc_SelectedIndexChanged">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table border="0" cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td align="left" valign="top">
                                            <asp:Panel ID="panel_Contenuto" runat="server">
                                            </asp:Panel>
                                            &nbsp;
                                        </td>
                                        <td valign="top">
                                            <asp:Panel ID="pnl_RFAOO" runat="server">
                                                <asp:Label ID="lblAooRF" CssClass="titolo_scheda" Text="" runat="server"></asp:Label>
                                                <asp:DropDownList ID="ddlAooRF" runat="server" CssClass="testo_grigio" AutoPostBack="False"
                                                    Visible="false">
                                                </asp:DropDownList>
                                            </asp:Panel>
                                        </td>
                                        <td class="titolo_scheda" valign="top">
                                            <asp:Panel ID="pnlAnno" runat="server">
                                                &nbsp;&nbsp;<asp:Label ID="lblAnno" CssClass="titolo_scheda" Text="Anno *" runat="server"></asp:Label>
                                                <asp:TextBox ID="TxtAnno" runat="server" CssClass="testo_grigio" Width="40" MaxLength="4"></asp:TextBox>
                                            </asp:Panel>
                                        </td>
                                        <td class="titolo_scheda" valign="top">
                                            <asp:Panel ID="pnlNumero" runat="server">
                                                &nbsp;&nbsp;<asp:Label ID="lblNumero" CssClass="titolo_scheda" Text="Numero *" runat="server"></asp:Label>
                                                <asp:TextBox ID="TxtNumero" runat="server" CssClass="testo_grigio" Width="40" MaxLength="4"></asp:TextBox>
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </asp:Panel>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center" height="26" valign="middle" class="pulsanti">
                <asp:Button ID="btn_cerca" runat="server" CssClass="pulsante" Width="65px" Height="19px"
                    Text="CERCA" OnClientClick="return checkField('tbx_anno','tbx_numProto')" OnClick="btn_cerca_Click">
                </asp:Button>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btn_chiudi" runat="server" CssClass="pulsante" Width="65px" Height="19px"
                    Text="CHIUDI" OnClientClick="CloseIt()"></asp:Button>
            </td>
        </tr>
    </table>
    <table width="590" border="0" align="center">
        <tr>
            <td>
                <label id="lblDocInDeposito" runat="server" visible="false" style="color: Red">
                </label>
                <div style="height: 380px;" align="center">
                    <cc1:IFrameWebControl ID="IF_VisDoc" runat="server" Align="center" OnNavigate="IF_VisDoc_Navigate"
                        iWidth="100%" iHeight="100%" />
                </div>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
