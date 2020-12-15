<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="SmistaUO.ascx.cs" Inherits="ProtocollazioneIngresso.Smistamento.SmistaUO" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<LINK href="../../CSS/docspa_30.css" type="text/css" rel="stylesheet">
<script language="javascript">

	// Verifica se almeno un checkbox di tipo "optComp" o "optCC" è impostato
	function AlmostOneRadioChecked()
	{	
		var retValue=false;
		
		var checkID="";	
		
		for(i = 0; i < document.frmProtIngresso.elements.length; i++) 
		{
			var elm = document.frmProtIngresso.elements[i]
			
			if (elm.type == 'radio')
			{	
				checkID=elm.id;
				
				if (checkID.indexOf('optComp') > -1 || 
					checkID.indexOf('optCC') > -1)
				{				
					retValue=elm.checked;
					
					if (retValue)
					{
						break;
					}
				}
			} 
		}
		
		return retValue;
	}
	
	function ResetRadioButtons(aspRadioID) 
	{
		//var firstRadio=null;
		
		var re = new RegExp(aspRadioID + '$')  
		for(i = 0; i < document.forms[0].elements.length; i++) 
		{
			var elm = document.forms[0].elements[i]
							
			if (elm.type == 'radio') 
			{					
				
				if (re.test(elm.name)) 
				{							
					elm.value = 'optNull'
				}
			}
		}
		
		// Impostazione focus primo radio button abilitato disponibile
		//SetControlFocus(firstRadio.id);
		
		try
		{
			// Aggiornamento pulsante "Protocolla"
			RefreshButtonProtocollaEnabled();
		}
		catch (ex)
		{
		
		}
	}

	// Impostazione del focus su un controllo
	function SetControlFocus(controlID)
	{	
		try
		{
			var control=document.getElementById(controlID);
			
			if (control!=null)
			{
				control.focus();
			}
		}
		catch (e)
		{
		
		}
	}

</script>
<asp:datagrid id="grdUOApp" AutoGenerateColumns="False" BorderColor="Gray" CellPadding="1" BorderWidth="1px"
	Width="100%" runat="server" SkinID="datagrid">
	<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
	<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
	<ItemStyle CssClass="bg_grigioN"></ItemStyle>
	<HeaderStyle HorizontalAlign="Center" CssClass="testo_biancoN" BackColor="#810D06"></HeaderStyle>
	<Columns>
		<asp:BoundColumn Visible="False" DataField="ID" HeaderText="ID"></asp:BoundColumn>
		<asp:BoundColumn Visible="False" DataField="TYPE" HeaderText="TYPE"></asp:BoundColumn>
		<asp:BoundColumn DataField="DESCRIPTION" HeaderText="Smista a:">
			<HeaderStyle Width="85%"></HeaderStyle>
			<ItemStyle Font-Names="verdana" VerticalAlign="Middle"></ItemStyle>
		</asp:BoundColumn>
		<asp:TemplateColumn HeaderText="COMP">
			<HeaderStyle Width="5%"></HeaderStyle>
			<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
			<ItemTemplate>
				<asp:RadioButton id="optComp" runat="server" GroupName="SELEZIONE" TextAlign="Right"></asp:RadioButton>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn HeaderText="CC">
			<HeaderStyle Width="5%"></HeaderStyle>
			<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
			<ItemTemplate>
				<asp:RadioButton id="optCC" runat="server" GroupName="SELEZIONE" TextAlign="Right"></asp:RadioButton>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn HeaderText="CANC">
			<HeaderStyle Width="5%"></HeaderStyle>
			<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
			<HeaderTemplate>
				<cc1:imagebutton id="btnClearOptions" Width="20px" height="18px" ImageUrl="Images/clearFlag.gif"
					CommandName="CLEAR_OPTIONS" AlternateText="Elimina tutte le selezioni" Tipologia="" DisabledUrl="Images/clearFlag.gif"
					Runat="server"></cc1:imagebutton>
			</HeaderTemplate>
			<ItemTemplate>
				<asp:RadioButton id="optNull" runat="server" GroupName="SELEZIONE" Checked="True"></asp:RadioButton>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:BoundColumn Visible="False" DataField="HAS_RUOLI_RIF"></asp:BoundColumn>
		<asp:BoundColumn Visible="False" DataField="LIST_RUOLI_RIF"></asp:BoundColumn>
	</Columns>
</asp:datagrid>
