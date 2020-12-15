<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Corrispondente.ascx.cs" Inherits="ConservazioneWA.UserControl.Corrispondente" %>


<%--<script type="text/javascript">
    function apriRubrica(calltype)
    {
	    var r = new Rubrica();
	    switch(calltype)
	    {
	        case "CALLTYPE_CORR_INT":
	            r.CallType = r.CALLTYPE_CORR_INT;
	            var res = r.Apri(); 
	        break;
	        
	        case "CALLTYPE_CORR_EST":
	            r.CallType = r.CALLTYPE_CORR_EST;
	            var res = r.Apri(); 
	        break;
	        
	        case "CALLTYPE_CORR_INT_EST":
	            r.CallType = r.CALLTYPE_CORR_INT_EST;
	            var res = r.Apri(); 
	        break;
	        
	        case "CALLTYPE_CORR_NO_FILTRI":
	            r.CallType = r.CALLTYPE_CORR_NO_FILTRI;
	            var res = r.Apri();
	        break;

	        case "CALLTYPE_CORR_INT_NO_UO":
	            r.CallType = r.CALLTYPE_CORR_INT_NO_UO;
	            var res = r.Apri();
	            break;       
	    }
	}
</script>--%>

<asp:Panel ID="pnl_Corrispondente" Width="100%" runat="server">
    <input id="txt_SystemId" type="hidden" runat="server" />
    <asp:TextBox ID="txt_Codice" Width="20%" runat="server" AutoPostBack="True" OnTextChanged="txt_Codice_TextChanged"></asp:TextBox>
    <asp:TextBox ID="txt_Descrizione" Width="55%" runat="server" ReadOnly="true"></asp:TextBox>
    <asp:ImageButton ID="btn_Rubrica" Width="30px" Height="20px" runat="server" ImageUrl="~/Img/rubrica.gif" OnClick="btn_Rubrica_Click" AlternateText="Seleziona un corrispondente dalla rubrica" ToolTip="Seleziona un corrispondente dalla rubrica" Visible="false" />
</asp:Panel>
