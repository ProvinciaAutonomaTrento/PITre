<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FascicolazioneMassiva.aspx.cs"
    Inherits="DocsPAWA.MassiveOperation.FascicolazioneMassiva" MasterPageFile="~/MassiveOperation/MassiveMasterPage.Master" %>

<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register TagPrefix="cc2" Namespace="Utilities" Assembly="MessageBox" %>
<%@ Register TagPrefix="cc3" Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" %>

<asp:Content ID="Head" ContentPlaceHolderID="Head" runat="server">
    <script language="javascript" type="text/javascript">
        // Funzione per l'apertura del popup di ricerca fascicolo
        function ApriRicercaFascicoli(codiceClassifica, NodoMultiReg) {
            // L'url della finestra
            var newUrl;

            // Creazione dell'url in cui è possibile reperire la pagina di ricerca nodo
            newUrl = '<%= GetPath %>' + '?codClassifica=' + codiceClassifica + '&caller=profilo&NodoMultiReg=' +
                NodoMultiReg;
            // Calcolo della posizione orizzontale e verticale in modo che la finestra di
            // ricerca fascicolo risulti centrata nello schermo
            var newLeft = (screen.availWidth - 615);
            var newTop = (screen.availHeight - 704);

            // Apertura finestra
            rtnValue = window.showModalDialog(newUrl, "", "dialogWidth:615px;dialogHeight:750px;status:no;resizable:no;scroll:no;dialogLeft:" + newLeft + ";dialogTop:" + newTop + ";center:no;help:no;");

            // Se la fiestra ritorna N, vengono cancellati i contenuti delle
            // due caselle di testo, altrimenti viene provocato il submit
            if (rtnValue == "N") {
                window.document.getElementById('<%= Page.Form.ClientID %>').txtCodFascicolo.value = "";
                window.document.getElementById('<%= Page.Form.ClientID %>').txtDescFascicolo.value = "";
            }
            if (rtnValue == "Y") {
                window.document.getElementById('<%= Page.Form.ClientID %>').submit();
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Form" ContentPlaceHolderID="Form" runat="server">
    <table style="width:99%;">  
        <tr align="center">
            <td valign="top">
                <table class="info_grigio" id="tbl_fasc_rapida" cellspacing="0" cellpadding="0" width="100%"
                    align="center" border="0">
                    <tr>
                        <td class="titolo_scheda" valign="middle" align="left">
                            &nbsp;&nbsp;Codice fascicolo Generale/Procedimentale
                        </td>
                        <td valign="middle" align="right">
                            <cc1:ImageButton class="ImgHand" ID="imgFasc" runat="server" AlternateText="Ricerca Fascicoli"
                                DisabledUrl="../images/proto/ico_fascicolo_noattivo.gif" Tipologia="DO_CLA_VIS_PROC"
                                ImageUrl="../images/proto/ico_fascicolo_noattivo.gif" OnClick="imgFasc_Click">
                            </cc1:ImageButton>
                            &nbsp;&nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" height="50">
                            <font size="1">&nbsp;</font>
                            <asp:TextBox ID="txtCodFascicolo" Width="75px" CssClass="testo_grigio" AutoPostBack="True"
                                runat="server" ReadOnly="False" OnTextChanged="txtCodFascicolo_TextChanged"></asp:TextBox>&nbsp;
                            <asp:TextBox ID="txtDescFascicolo" Width="287px" CssClass="testo_grigio" runat="server"
                                ReadOnly="True"></asp:TextBox><font size="1"></font>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
   </table>
</asp:Content>
