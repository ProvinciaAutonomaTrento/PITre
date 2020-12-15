<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DettaglioNota.ascx.cs"
    Inherits="DocsPAWA.Note.DettaglioNota" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register Assembly="AjaxControlToolkit, Version=3.0.30930.28736, Culture=neutral, PublicKeyToken=28f01b0e84b6d53e"
    Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
    <script type="text/javascript"  language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
    <script type="text/javascript"  language="javascript" src="../LIBRERIE/jquery-1.8.3.js"></script>

<script type="text/javascript">
    function dettaglionota_setfocus(controlId) { try { document.getElementById(controlId).focus(); } catch (e) {} }
    
    function ShowDialogListaNote()
    {
		var pageHeight=530;
		var pageWidth=650;
        return (window.showModalDialog('<%=ListaNoteModalDialogUrl %>', null, 'dialogWidth:' + pageWidth + 'px;dialogHeight:' + pageHeight + 'px;status:no;resizable:yes;scroll:yes;center:yes;help:no;close:no') == 'true');
    }   
    
    function ItemSelected(sender, args)
    {
        var testo = args.get_text();
        var codiceRF = testo.substring(testo.lastIndexOf(" ["));
        codiceRF = codiceRF.substring(2, codiceRF.lastIndexOf("]"))
        testo = testo.substring(0, testo.lastIndexOf(" ["));

        var nota;
        document.getElementById("<%=this.txtAutoComplete.ClientID%>").focus();
        document.getElementById("<%=this.txtAutoComplete.ClientID%>").value = "";

        nota = document.getElementById("<%=this.txtNote.ClientID%>").value;
        if (nota != "")
            document.getElementById("<%=this.txtNote.ClientID%>").value += "\n" + testo;
        else
            document.getElementById("<%=this.txtNote.ClientID%>").value = testo;

        document.getElementById("<%=this.isTutti.ClientID%>").value = "";
        if (codiceRF == "TUTTI") {
            //document.getElementById("<%=this.isTutti.ClientID%>").value = "1";
            //$('span.rblTipiVisibilita input')[3].checked = true;
            $('span.rblTipiVisibilita input')[$('span.rblTipiVisibilita input').length - 1].checked = true;
            __doPostBack('<%=this.rblTipiVisibilita.ClientID%>', '');
        }
        else
            $('span.rblTipiVisibilita input')[$('span.rblTipiVisibilita input').length - 2].checked = true;
    }   


</script>

<table id="tblNote" border="0" cellspacing="0" cellpadding="0" width="<%=this.Width%>" >
    <tr>
        <td colspan="3" height="5px">
        </td>
    </tr>
    <tr>
        <td colspan="3" height="5px" align="left">
            <asp:Label ID="lblNoteDisponibili" runat="server" CssClass="titolo_scheda"></asp:Label>
        </td>
    </tr>
    <tr>
        <td colspan="3">
        <table width="100%"><tr>
        <td class="titolo_scheda" valign="middle" width="90%">
            <asp:Label ID="lblAutoreNote" runat="server" CssClass="titolo_scheda"></asp:Label>
        </td>
        <td class="testo_grigio" valign="middle" align="rigth" width="10%" colspan="2">
            <cc1:ImageButton ID="btnSbloccaNote" runat="server" ImageAlign="Middle" ImageUrl="../images/proto/matita.gif"
                DisabledUrl="../images/proto/matita.gif" OnClick="btnSbloccaNote_Click" ToolTip="Abilita la modifica delle note" />&nbsp;
            <cc1:ImageButton ID="btnListaNote" runat="server" ImageAlign="Middle" ImageUrl="../images/rubrica/l_exp_o_grigia.gif"
                DisabledUrl="../images/rubrica/l_exp_o_grigia.gif" OnClick="btnListaNote_Click"
                OnClientClick="return ShowDialogListaNote()" ToolTip="Consulta le note" />
        </td>
        </tr></table></td>
    </tr>
    <tr>
        <td class="titolo_scheda" valign="middle" colspan="3" align="left">
            <asp:RadioButtonList ID="rblTipiVisibilita" runat="server" RepeatDirection="Horizontal" name="rblTipiVisibilita"
                RepeatLayout="Flow" CssClass="titolo_scheda rblTipiVisibilita" AutoPostBack="true" OnSelectedIndexChanged="DataChanged">
                <asp:ListItem Value="Personale">Personale</asp:ListItem>
                <asp:ListItem Value="Ruolo">Ruolo</asp:ListItem>
                <asp:ListItem Value="RF">RF</asp:ListItem>
                <asp:ListItem Value="Tutti" Selected="True">Tutti</asp:ListItem>
            </asp:RadioButtonList>
        &nbsp;
            <asp:DropDownList Visible="false" ID="ddl_regRF" runat="server" 
                CssClass="testo_grigio" Width="132px" AutoPostBack="true" OnSelectedIndexChanged="RFChanged">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="titolo_scheda" valign="middle" colspan="3" align="left">
            <table cellpadding="0" cellspacing="0" border="0">
                <tr>
                    <td>
                        <asp:TextBox ID="txtAutoComplete" Visible="false" Width="350" CssClass="testo_grigio"
                            ToolTip="Cerca in elenco" runat="server" valign="middle" ></asp:TextBox>
                        <asp:HiddenField ID="isTutti" runat="server" Value=""/>
                        <cc2:AutoCompleteExtender runat="server" ID="autoComplete1" TargetControlID="txtAutoComplete"
                            CompletionListCssClass="Completer_completionListElement FS-NBK" CompletionListItemCssClass="Completer_listItem"
                            CompletionListHighlightedItemCssClass="Completer_highlightedListItem" ServiceMethod="GetListaNote"
                            MinimumPrefixLength="2" CompletionInterval="500" EnableCaching="true" CompletionSetCount="20"
                            DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                            UseContextKey="true" OnClientItemSelected="ItemSelected">
                        </cc2:AutoCompleteExtender>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td colspan="3" class="testo_grigio" valign="top" align="left">
            <asp:TextBox ID="txtNote" runat="server" Width="360" CssClass="testo_grigio" TextMode="MultiLine"
                OnTextChanged="DataChanged"></asp:TextBox>
        </td>
    </tr>
     <tr>
		<td colspan="2" align="right" class="testo_grigio">
			caratteri disponibili:&nbsp;<input type="text" id="clTesto" runat="server" name="clTesto"  size="4" class="testo_grigio" readonly="readonly" />&nbsp;</td>
	</tr>
    <tr>
        <td colspan="3" height="5px">
            &nbsp;
        </td>
    </tr>
</table>
