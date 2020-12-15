<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="IconeRicerca.ascx.cs"
    Inherits="DocsPAWA.UserControls.IconeRicerca" %>

<script language="javascript" type="text/javascript">

function OpenNewViewer(path)
{
    window.open(path, '', "width=800px,height=600px,top=50,left=100,toolbar=no,directories=no,menubar=no,resizable=yes, scrollbars=no, center=yes");
}

function ApriModalDialog()
			{					
				var val;
				val=window.confirm("Vuoi inserire il documento nell'area di Lavoro ?");
				
				if(val)
				{
				    return true;								
				}
				else
				{
				    return false;
				}							
			}
			
     function ApriModalDialogNewADL()
			{					
				var val;
				val=window.confirm("Vuoi eliminare il documento dall'area di Lavoro ?");
				
				if(val)
				{					
					return true;
				}
				else
				{
					return false;
				}			
			}
			
			 function ApriModalDialogEliminaCons()
			{					
				var val;
				val=window.confirm("Vuoi eliminare il documento dall'area di Conservazione ?");
				
				if(val)
				{						
					return true;
				}
				else
				{
					return false;
				}			
			}
			
			
		function ShowMaskDettagliFirma()
			{
				var height=screen.availHeight;
				var width=screen.availWidth;
				
				height=(height * 90) / 100;
				width=(width * 90) / 100;
				
				window.showModalDialog('../popup/dettagliFirmaDoc.aspx',
										'',
										'dialogHeight: ' + height + 'px; dialogWidth: ' + width + 'px; resizable: yes;status:no;scroll:yes;help:no;close:no');
			}
			
			function showModalDialogEliminaDocInFasc()
			{
			        var val;
				    val=window.confirm("Il documento verrà rimosso dal fascicolo. Continuare?");
				    if(val)
				    {
				        return true;
				    }else
				        {
				            return false;
				        }   
			
			}
			
			function showModalDialogNonEliminareFascicolo()
			{
			            var agree=alert("Il documento non può essere rimosso dal fascicolo.");
                        return true;
            
			}

</script>

<script src="../LIBRERIE/DocsPA_Func.js" type="text/javascript"></script>

<table style="width: 100%;">
    <input id="txt_confirmDel" type="hidden" name="txt_confirmDel" runat="server" />
    <input id="txt_numero_fascicoli" type="hidden" name="txt_numero_fascicoli" runat="server" />
    <tr>
        <td>
            <asp:ImageButton BorderColor="#404040" ID="btn_dettaglio" runat="server" ImageUrl="~/images/proto/dettaglio.gif"
                ToolTip="Vai alla scheda del documento" CommandName="Select" />
        </td>
        <td>
            <asp:ImageButton ID="btn_adl" runat="server" ImageUrl="~/images/proto/ins_area.gif"
                ToolTip="Inserisci questo documento in Area di lavoro" CommandName="inserisciAdl"
                OnClick="btn_adl_Click" OnClientClick="return ApriModalDialog();" />
            <asp:ImageButton ID="btn_scheda" runat="server" ImageUrl="~/images/proto/dettaglio.gif"
                CommandName="scheda" BorderWidth="0px" BorderColor="Gray" BorderStyle="Solid"
                Visible="false" />
        </td>
    </tr>
    <tr>
        <td>
            <asp:ImageButton ID="btn_visualizza" runat="server" ToolTip="Visualizza immagine documento"
                OnClick="btn_visualizza_Click" />
        </td>
        <td>
            <asp:ImageButton ID="btn_firmato" runat="server" ToolTip="Dettaglio firma" OnClick="btn_firmato_Click"
                ImageUrl="~/images/tabDocImages/icon_p7m.gif" />
        </td>
    </tr>
    <tr>
        <td>
            <asp:ImageButton ID="btn_eliminaDocInFasc" runat="server" ImageUrl="../images/proto/cancella.gif"
                OnClick="btn_eliminaDocInFasc_Click" />
        </td>
        <td>
            <asp:ImageButton ID="btn_conservazione" runat="server" ImageUrl="~/images/proto/conservazione_d.gif"
                ToolTip="Inserisci questo documento in Area conservazione" CommandName="AreaConservazione"
                OnClick="btn_conservazione_Click" />
        </td>
    </tr>
</table>
