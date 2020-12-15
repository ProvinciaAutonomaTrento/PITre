<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DocumentiFascicolazione.ascx.cs" Inherits="DocsPAWA.FascicolazioneCartacea.DocumentiFascicolazione" %>
<%@ Register Src="../waiting/WaitingPanel.ascx" TagName="WaitingPanel" TagPrefix="uc2" %>
<%@ Register TagPrefix="uc1" TagName="DataGridPagingWait" Src="../waiting/DataGridPagingWait.ascx" %> 
<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet" />
<script type="text/javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
<script type="text/javascript">
        // Verifica presenza del documento richiesto in altri fascicoli
    	function CheckFascicoli(versionId)
    	{
    	    ShowWaitCursor();
    	    
    	    var args=new Object;
			args.window=window;

			window.showModalDialog('VerificaFascicoli.aspx?versionId=' + versionId,
					args,
					'dialogWidth:425px;dialogHeight:275px;status:no;resizable:yes;scroll:no;center:yes;help:no;close:no');								
					
            ReleaseWaitCursor();					
    	}
    	
    	// Visualizzazione del file con l'applicazione proprietaria
	    function ShowFile(versionId)
	    {	
	        var windowDim = "top=0, left=0, width=" + window.screen.availWidth + ", height=" + window.screen.availHeight;

	        window.open("ShowFilePage.aspx?versionId=" + versionId, "", "fullscreen=no,toolbar=no," + windowDim + ",directories=no,menubar=yes,resizable=yes,scrollbars=yes");
	        
	        return false;
	    }
    	    
    	// Stampa lista
	    function PrintList()
		{				
			var args=new Object;
			args.window=window;
			
			window.showModalDialog("../exportDati/exportDatiSelection.aspx?export=archivioCartaceo",
									args,
									"dialogWidth:450px;dialogHeight:250px;status:no;resizable:no;scroll:no;center:yes;help:no;");
									
            return false;
		}			
		
		// Handler evento click button "btnDiscardDocument"
		function OnClickScartaDocumento()
		{ 
		    if (confirm("Attenzione:\nsi stà scegliendo di scartare il documento dalla gestione dell'archivio cartaceo.\nIn tal modo non sarà più visualizzato in lista.\nContinuare?"))
		    {
                ShowWaitPanel("Scarta documento in corso...");
		        ShowWaitCursor();
		        return true;
		    }
		    else
		        return false;
		}
		
    	// Handler evento click checkbox "chkDocumentoCartaceo"
    	function OnClickDocumentoCartaceo(sender, chkInsFascCartaceo)
    	{
    	    var checkBoxSender = document.getElementById(sender);
    	    var checkbox = document.getElementById(chkInsFascCartaceo);

            if (checkBoxSender.checked)
                checkbox.disabled = false;
            else
            {
                checkbox.disabled = true;
                checkbox.checked = false;
            }
    	}
    	
    	function SelectAllCheckCartaceo(sender)
    	{
            var chk = document.getElementById(sender);
    	    if (chk!=null)
    	        CheckUncheckAll("chkDocumentoCartaceo", chk);
    	}
    	
    	function SelectAllCheckInserisci(sender)
    	{
            var chk = document.getElementById(sender);
    	    if (chk!=null)
    	        CheckUncheckAll("chkInsFascCartaceo", chk);
    	}
    	
    	function CheckUncheckAll(targetCheckId, senderCheck) 
	    {
		    for(i = 0; i < document.forms[0].elements.length; i++) 
		    {
			    var elm = document.forms[0].elements[i]

			    if (elm.type == 'checkbox') 
			    {	
			        if (elm.name.indexOf(targetCheckId) > -1)
				    {	
				        if (!elm.disabled)
                            elm.checked = senderCheck.checked;
				    }
			    }
			}
		}
		
		function SaveDocuments()
		{
		    ShowWaitPanel("Salva documenti in corso...");
		    
		    ShowWaitCursor();
		}
		
		// Visualizzazione clessidra
		function ShowWaitCursor()
		{
			window.document.body.style.cursor="wait";
        }

		function ReleaseWaitCursor()
		{
		    window.document.body.style.cursor="none";
        }
	
		function WaitDataGridCallback(eventTarget,eventArgument)
		{
			ShowWaitPanel("Ricerca in corso...");
			
			ShowWaitCursor();
		}

</script>
<div id="divButtons" style="WIDTH: 100%;" runat="server">
    <uc1:datagridpagingwait id="DataGridPagingWait1" runat="server"></uc1:datagridpagingwait>
    <table border="0" cellpadding="0" cellspacing="0" width="100%">
		<tr id="trHeader"  runat="server">
		    <td vAlign="middle" class="pulsanti" align="left" style="width: 100%;">
		        <asp:Label CssClass="testo_grigio" ID="lblMessage" runat="server"></asp:Label>
		    </td>
		</tr>
    </table>
</div>
<asp:datagrid id="grdDocumentiFascicolazione" SkinID="datagrid" runat="server" Width="100%" BorderWidth="1px" AllowCustomPaging="True"
    HorizontalAlign="Center" BorderColor="Gray" CellPadding="1"
    AutoGenerateColumns="False" BorderStyle="Inset" OnItemCreated="grdDocumentiFascicolazione_ItemCreated" OnItemDataBound="grdDocumentiFascicolazione_ItemDataBound" OnPreRender="grdDocumentiFascicolazione_PreRender" AllowPaging="True" OnPageIndexChanged="grdDocumentiFascicolazione_PageIndexChanged" OnItemCommand="grdDocumentiFascicolazione_ItemCommand">
    <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
    <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
    <ItemStyle Height="20px" CssClass="bg_grigioN"></ItemStyle>			
    <HeaderStyle Wrap="False" Font-Bold="True" Height="20px" ForeColor="White" CssClass="menu_1_bianco_dg"></HeaderStyle>
    <Columns>
        <asp:BoundColumn DataField="Index" Visible="False"></asp:BoundColumn>
        <asp:BoundColumn DataField="Documento" HeaderText="Documento">                                
            <HeaderStyle Width="15%" Font-Bold="True" Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Center" />
            <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                Font-Underline="False" HorizontalAlign="Center" />
        </asp:BoundColumn>
        <asp:BoundColumn DataField="TipoDocumento" HeaderText="Tipo">
            <HeaderStyle Font-Bold="True" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                Font-Underline="False" HorizontalAlign="Center" Width="5%" />
            <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                Font-Underline="False" HorizontalAlign="Center" />
        </asp:BoundColumn>
        <asp:BoundColumn DataField="Versione" HeaderText="Versione">
            <HeaderStyle Width="5%" Font-Bold="True" Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Center" />
            <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                Font-Underline="False" HorizontalAlign="Center" />
        </asp:BoundColumn>
        <asp:BoundColumn DataField="Registro" HeaderText="Registro">
            <HeaderStyle Width="15%" Font-Bold="True" Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Center" />
        </asp:BoundColumn>
        <asp:BoundColumn DataField="Fascicolo" HeaderText="Fascicolo">
            <HeaderStyle Width="40%" Font-Bold="True" Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Center" />
        </asp:BoundColumn>
        <asp:TemplateColumn HeaderText="Altri fascicoli">
            <ItemTemplate>
                <asp:ImageButton ID="btnCheckFascicoli" runat="server" ImageUrl="../images/fasc/ico_mini_cartella.gif" ToolTip="Verifica se presente in altro fascicolo cartaceo" />
            </ItemTemplate>
            <HeaderStyle Width="5%" Font-Bold="True" Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Center" />
            <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                Font-Underline="False" HorizontalAlign="Center" />
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Visualizza">
            <ItemTemplate>
                <asp:ImageButton ID="btnShowFile" runat="server" ImageUrl="../images/proto/dett_lente_doc.gif" ToolTip="Visualizza documento" />
            </ItemTemplate>
            <HeaderStyle Width="5%" Font-Bold="True" Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Center" />
            <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                Font-Underline="False" HorizontalAlign="Center" />
        </asp:TemplateColumn>
        <asp:TemplateColumn >
            <HeaderTemplate>
                Cartaceo&nbsp;<br />
            </HeaderTemplate>
            <ItemTemplate>
                <input id="chkDocumentoCartaceo" runat="server" checked="false" type="checkbox" alt="Imposta il documento come cartaceo" />
                <asp:ImageButton ID="btnDiscardDocument" runat="server" ImageUrl="../images/proto/RispProtocollo/cestino.gif" ToolTip="Scarta documento se non cartaceo" CommandName="DISCARD_DOCUMENT" />
            </ItemTemplate>
            <HeaderStyle Width="5%" Font-Bold="True" Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Center" />
            <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                Font-Underline="False" HorizontalAlign="Center" />
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Ins. in cartaceo">
            <HeaderStyle Width="5%" Font-Bold="True" Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False" VerticalAlign="Bottom" HorizontalAlign="Center" />
            <HeaderTemplate>
                Ins. in cartaceo
                    <br />
                    <asp:CheckBox ID="chkSelectAllInsFascCartaceo" runat="server" ToolTip="Seleziona / deseleziona tutti" />
                </HeaderTemplate>
                <ItemTemplate>
                    <input id="chkInsFascCartaceo" runat="server" checked="false" type="checkbox" alt="Inserisci il documento nel corrispondente fascicolo cartaceo" />
                </ItemTemplate>
                <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                    Font-Underline="False" HorizontalAlign="Center" />
            </asp:TemplateColumn>
        </Columns>
    <PagerStyle HorizontalAlign="Center" Mode="NumericPages" BackColor="#C2C2C2" CssClass="menu_pager_grigio" Position="TopAndBottom" />
</asp:datagrid>
<br />
<table border="0" cellpadding="0" cellspacing="0" width="100%">
	<tr align="center">		
	    <td>
	        <asp:ImageButton ImageAlign="middle" ID="btnSave" runat="server" ToolTip="Salva i documenti selezionati" SkinID="salva" OnClick="btnSave_Click" />
	        <asp:ImageButton ImageAlign="middle" ID="btnPrint" runat="server" SkinID="stampa_Attivo" ToolTip="Esporta il risultato della ricerca" />
	    </td>
    </tr>
</table>
<uc2:WaitingPanel ID="WaitingPanel1" runat="server" />