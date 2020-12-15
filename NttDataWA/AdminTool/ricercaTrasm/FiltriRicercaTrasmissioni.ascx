<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FiltriRicercaTrasmissioni.ascx.cs"
    Inherits="SAAdminTool.ricercaTrasm.FiltriRicercaTrasmissioni" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register Src="../UserControls/Calendar.ascx" TagName="Calendario" TagPrefix="uc3" %>

<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>

<script language="javascript" type="text/javascript">

	// Gestione visualizzazione maschera rubrica
	function ShowDialogRubrica(txtTipoCorrispondente)
	{
		var w_width = screen.availWidth - 40;
		var w_height = screen.availHeight - 35;
		
		var navapp = navigator.appVersion.toUpperCase();
		if ((navapp .indexOf("WIN") != -1) && (navapp .indexOf("NT 5.1") != -1))
			w_height = w_height + 20;
		
		var opts = "dialogHeight:" + w_height + "px;dialogWidth:" + w_width + "px;center:yes;help:no;resizable:no;scroll:no;status:no;unadorned:yes";
		
		var params="calltype=" +  Rubrica.prototype.CALLTYPE_RICERCA_TRASM_TODOLIST + "&tipo_corr=" + document.getElementById(txtTipoCorrispondente).value;
		
		var urlRubrica="../popup/rubrica/Rubrica.aspx";
		var res=window.showModalDialog (urlRubrica + "?" + params,window,opts);				
	}	
		
</script>

<input id="txtTipoCorrispondente" type="hidden" runat="server" />
<table class="info_grigio" id="tblContainer" cellspacing="3" cellpadding="0" width="95%"
    align="center" border="0" runat="server">
    <tr>
        <td style="width: 747px">
            <div id="pnlContainer" style="overflow: auto; width: 99.95%; height: 380px" runat="server">
                <table class="testo_grigio" id="tblElemNonLetti"  cellspacing="0" cellpadding="0"
                    width="95%" align="center" border="0" runat="server">
                    <tr height="25">
                         <td class="titolo_scheda">
                            <asp:CheckBox ID="chkElemNonLetti" runat="server" Text="Elementi non letti" AutoPostBack="True" OnCheckedChanged="chkElemNonLetti_CheckedChanged"></asp:CheckBox>
                        </td>
                    </tr>
                     <tr height="25">
                         <td class="titolo_scheda">
                            <asp:CheckBox ID="chkFasciscoli" runat="server" Text="Fascicoli" AutoPostBack="True" OnCheckedChanged="chkFasciscoli_CheckedChanged"></asp:CheckBox>
                        </td>
                    </tr>
                </table>
                <table class="testo_grigio" id="tblTipoDocumento" height="25" cellspacing="0" cellpadding="0"
                    width="95%" align="center" border="0" runat="server">
                    <tr>
                        <td class="titolo_scheda" width="20%">
                            <asp:CheckBox ID="chkDocumenti" runat="server" Text="Documenti" AutoPostBack="True" OnCheckedChanged="chkDocumenti_CheckedChanged"></asp:CheckBox>
                        </td>
                        <td class="testo_grigio" width="80%">
                            <asp:RadioButtonList ID="rbListTipoDocumento" runat="server" Width="100%" AutoPostBack="True"
                                RepeatDirection="Horizontal" CssClass="titolo_scheda" CellPadding="0" CellSpacing="0"
                                OnSelectedIndexChanged="rbListTipoDocumento_SelectedIndexChanged">
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                </table>
                <table class="testo_grigio" id="tblDocAcqFirm" height="25" cellspacing="0" cellpadding="0"
                    width="95%" align="center" border="0" runat="server">
                    <tr>
                    <td width="19%"></td>
                        <td class="titolo_scheda" width="25%"><asp:CheckBox ID="chkAcquisiti" runat="server" Text="Documenti acquisiti"></asp:CheckBox></td>
                        <td class="titolo_scheda" width="25%"><asp:CheckBox ID="chkFirmati" runat="server" Text="Documenti firmati"></asp:CheckBox></td>
                        <td class="titolo_scheda" width="25%"><asp:CheckBox ID="cb_nonFirmato" runat="server" Text="Documenti non firmati"></asp:CheckBox></td>
                    </tr>
                </table>  
               <table class="testo_grigio" id="Table1" height="25" cellspacing="0"
                    cellpadding="0" width="95%" align="center" border="0" runat="server">
                    <tr>
                        <td class="titolo_scheda" width="20%">
                            <asp:Label ID="Label2" runat="server">In attesa di acc.:</asp:Label>
                        </td>
                        <td class="titolo_scheda" width="80%">
                            <asp:CheckBox ID="chkAccettazione" runat="server" AutoPostBack="true" OnCheckedChanged="chkAccettazione_CheckedChanged"></asp:CheckBox>
                        </td>
                    </tr>
                </table> 
                <table class="testo_grigio" id="tblRagioneTrasmissione" height="25" cellspacing="0"
                    cellpadding="0" width="95%" align="center" border="0" runat="server">
                    <tr>
                        <td class="titolo_scheda" width="20%">
                            <asp:Label ID="lblRagioniTrasmissione" runat="server">Ragione trasmissione:</asp:Label>
                        </td>
                        <td class="titolo_scheda" width="80%">
                            <asp:DropDownList ID="cboRagioniTrasmissione" runat="server" Width="50%" AutoPostBack="False"
                                CssClass="testo_grigio">
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table class="testo_grigio" id="tblDataTrasmissioneDocumento" height="25" cellspacing="0"
                    cellpadding="0" width="95%" align="center" border="0" runat="server">
                    <tr>
                        <td class="titolo_scheda" width="20%">
                            <asp:Label ID="lblDataTrasmissione" runat="server">Data trasmissione:</asp:Label>
                        </td>
                        <td class="titolo_scheda" width="20%">
                            <asp:DropDownList ID="cboTypeDataTrasmissione" runat="server" Width="100%" AutoPostBack="True"
                                CssClass="testo_grigio" OnSelectedIndexChanged="cboFilterType_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td class="testo_grigio" align="center" width="5%">
                            <asp:Label ID="lblInitDataTrasmissione" runat="server" CssClass="titolo_scheda">Da:</asp:Label>
                        </td>
                        <td class="testo_grigio" align="left" width="20%">
                            <uc3:Calendario ID="txtInitDataTrasmissione" runat="server" />
                        </td>
                        <td class="testo_grigio" align="center" width="5%">
                            <asp:Label ID="lblEndDataTrasmissione" runat="server" CssClass="titolo_scheda">a:</asp:Label>
                        </td>
                        <td class="testo_grigio" align="left" width="20%">
                            <uc3:Calendario ID="txtEndDataTrasmissione" runat="server" />
                        </td>
                        <td class="titolo_scheda" align="right" width="10%">
                        </td>
                    </tr>
                </table>
                <table class="testo_grigio" id="tblDataScadenzaTrasmissione" height="25" cellspacing="0"
                    cellpadding="0" width="95%" align="center" border="0" runat="server">
                    <tr>
                        <td class="titolo_scheda" width="20%">
                            <asp:Label ID="lblDataScadenzaTrasmissione" runat="server">Data scadenza:</asp:Label>
                        </td>
                        <td class="titolo_scheda" width="20%">
                            <asp:DropDownList ID="cboTypeDataScadenzaTrasmissione" runat="server" Width="100%"
                                AutoPostBack="True" CssClass="testo_grigio" OnSelectedIndexChanged="cboFilterType_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td class="testo_grigio" align="center" width="5%">
                            <asp:Label ID="lblInitDataScadenzaTrasmissione" runat="server" CssClass="titolo_scheda">Da:</asp:Label>
                        </td>
                        <td class="testo_grigio" align="left" width="20%">
                            <uc3:Calendario ID="txtInitDataScadenzaTrasmissione" runat="server" />
                        </td>
                        <td class="testo_grigio" align="center" width="5%">
                            <asp:Label ID="lblEndDataScadenzaTrasmissione" runat="server" CssClass="titolo_scheda">a:</asp:Label>
                        </td>
                        <td class="testo_grigio" align="left" width="20%">
                            <uc3:Calendario ID="txtEndDataScadenzaTrasmissione" runat="server" />
                        </td>
                        <td class="titolo_scheda" align="right" width="10%">
                        </td>
                    </tr>
                </table>
                <table class="testo_grigio" id="tblUtenteMittente" height="25" cellspacing="0" cellpadding="0"
                    width="95%" align="center" border="0" runat="server">
                    <tr>
                        <td class="titolo_scheda" width="20%" style="height: 25px">
                            <asp:Label ID="lblUtenteMittente" runat="server">Mittente trasmissione:</asp:Label>
                        </td>
                        <td class="titolo_scheda" width="80%" style="height: 25px">
                            <asp:RadioButtonList ID="optListTipiMittente" runat="server" CssClass="titolo_scheda"
                                RepeatLayout="Flow" RepeatDirection="Horizontal" AutoPostBack="True" OnSelectedIndexChanged="optListTipiMittente_SelectedIndexChanged">
                                <asp:ListItem Value="U">UO</asp:ListItem>
                                <asp:ListItem Value="R" Selected="True">Ruolo</asp:ListItem>
                                <asp:ListItem Value="P">Persona</asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                    <tr>
                        <td class="titolo_scheda" width="20%" style="height: 25px">
                        </td>
                        <td class="titolo_scheda" width="80%" style="height: 25px">
                            <input id="txtSystemIdUtenteMittente" type="hidden" runat="server" />
                            <asp:TextBox ID="txtCodiceUtenteMittente" runat="server" Width="20%" CssClass="testo_grigio"
                                AutoPostBack="True" OnTextChanged="txtCodiceUtenteMittente_TextChanged"></asp:TextBox>
                            <asp:TextBox ID="txtDescrizioneUtenteMittente" runat="server" Width="72%" CssClass="testo_grigio"></asp:TextBox>
                            <asp:ImageButton ID="btnShowRubrica" runat="server" ImageUrl="../images/proto/rubrica.gif"
                                Height="20px" Width="20px" OnClick="btnShowRubrica_Click" />
                        </td>
                    </tr>
                </table>
                <table class="testo_grigio" id="tblOggetto" cellspacing="0" cellpadding="0" width="95%"
                    align="center" border="0" runat="server">
                    <tr>
                        <td class="titolo_scheda" valign="top" width="20%" style="height: 42px">
                            <asp:Label ID="lblOggetto" runat="server">Oggetto:</asp:Label>
                        </td>
                        <td class="testo_grigio" width="80%" style="height: 42px">
                            <asp:TextBox ID="txtOggetto" runat="server" Width="100%" CssClass="testo_grigio"
                                TextMode="MultiLine"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <table class="testo_grigio" id="Table2" cellspacing="0" cellpadding="0" width="95%"
                    align="center" border="0" runat="server">
                    <tr>
                        <td class="titolo_scheda" valign="top" width="20%" style="height: 42px">
                            <asp:Label ID="Label1" runat="server">Ordinamento:</asp:Label>
                        </td>
                        <td class="testo_grigio" width="80%" style="height: 42px">
                            <asp:DropDownList ID="ddl_ordinamento" runat="server" CssClass="testo_grigio" Width="250px">
                                <asp:ListItem Selected="True" Value="DTA_INVIO_DESC">DATA INVIO DECRESCENTE</asp:ListItem>
                                <asp:ListItem Value="DTA_INVIO_ASC">DATA INVIO CRESCENTE</asp:ListItem>
                                <asp:ListItem Value="DTA_SCAD_DESC">DATA SCADENZA DECRESCENTE</asp:ListItem>
                                <asp:ListItem Value="DTA_SCAD_ASC">DATA SCADENZA CRESCENTE</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table class="testo_grigio" cellspacing="0" cellPadding="0" width="95%" align="center" border="0" runat="server">
		            <tr>
		                <td class="titolo_scheda" valign="top" width="20%" style="height:19">
		                    <asp:Label runat="server" ID="lbl_fileAcquisiti">Tipo file acquisito</asp:Label> 
		                </td>		                
		                <td class="testo_grigio" width="80%" style="height: 42px">
		                    <asp:DropDownList ID="ddl_tipoFileAcquisiti" runat="server" CssClass="testo_grigio" Width="25%" >
		                        <asp:ListItem></asp:ListItem>
		                    </asp:DropDownList>
		                    </td>
		            </tr>
                </table>
            </div>
        </td>
    </tr>
</table>
