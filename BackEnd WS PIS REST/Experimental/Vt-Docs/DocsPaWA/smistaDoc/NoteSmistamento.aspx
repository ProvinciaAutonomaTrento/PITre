<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NoteSmistamento.aspx.cs" Inherits="DocsPAWA.smistaDoc.NoteSmistamento" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ import namespace="Microsoft.Web.UI.WebControls" %>
<%@ Register Src="../UserControls/Calendar.ascx" TagName="Calendario" TagPrefix="uc3" %>
<%@ Register TagPrefix="mytree" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Inserimento dati aggiuntivi smistamento</title>
    <script  type="text/javascript" language=javascript src="../LIBRERIE/DocsPA_Func.js"></script>
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<base target="_self">
		
		
</head>
<body bottomMargin="0" leftMargin="5" topMargin="5" rightMargin="5">
    <form id="formNoteTrasmissione" runat="server">   
        <table class="contenitore" cellSpacing="2" cellPadding="0"  bgColor="#ffffff" border="0" Width=400px>
           <tr><td colspan=2></td></tr>
           <tr><td class=testo_grigio colspan=2 >&nbsp;<asp:label id=lblCorr runat=server Width=390px></asp:label></td></tr>
           <tr><td colspan=2></td></tr>
           <tr><td colspan=2></td></tr>
           <tr><td>&nbsp;<asp:Label runat=server ID="lblNoteInd" CssClass="testo_grigio_scuro" Width="100px">Note individuali&nbsp;</asp:Label></td><td><asp:TextBox ID="txtNoteInd" runat="server" Width=280px MaxLength="250" TextMode="MultiLine" CssClass=testo_grigio Rows=3></asp:TextBox></td></tr>
           <tr><td colspan=2></td></tr>
           <tr><td>&nbsp;<asp:label id="lbl_dtaScadenza" runat="server" CssClass="testo_grigio_scuro" Width="100px">Data scadenza&nbsp;</asp:label></td>
           <td><uc3:Calendario id="txt_dtaScadenza" runat="server" Visible="true" /></td></tr>
           <asp:Panel ID="pnl_tipoTrasm" runat="server">
               <tr><td colspan=2></td></tr>
               <tr>
                    <td>&nbsp;<asp:Label ID="lbl_tipo" runat=server CssClass="testo_grigio_scuro" Width="100px">Tipo&nbsp;</asp:Label></td>
                    <td>
                        <asp:DropDownList ID="ddl_tipo" runat=server CssClass="testo_grigio_scuro">
                            <asp:ListItem Text="Uno" Value="S" Selected=True></asp:ListItem>
                            <asp:ListItem Text="Tutti" Value="T"></asp:ListItem>
                        </asp:DropDownList></td> 
                </tr>
            </asp:Panel>
            <tr><td colspan=2></td></tr>
            <tr><td colspan=2></td></tr>
            <tr><td colspan=2></td></tr>
            <tr><td colspan=2></td></tr>
            <tr><td align=center  class="titolo_scheda" colspan=2>
                <asp:button id="btnSave" runat="server" text="   Ok   " CssClass="pulsante" OnClick="btnSave_Click"/>&nbsp;&nbsp;
                <asp:button id="btnClose" runat="server" text="Annulla" CssClass="pulsante" OnClick="btnClose_Click"/>
                </td>
            </tr>
            <tr><td colspan=2></td></tr>
            <tr><td colspan=2></td></tr>
        </table>    
    </form>
</body>
</html>
