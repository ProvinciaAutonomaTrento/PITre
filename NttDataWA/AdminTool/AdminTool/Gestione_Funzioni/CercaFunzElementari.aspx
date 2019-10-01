<%@ Page language="c#" Codebehind="CercaFunzElementari.aspx.cs" AutoEventWireup="false" Inherits="Amministrazione.Gestione_Funzioni.CercaFunzElementari" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>DOCSPA - AMMINISTRAZIONE > Trova.............................</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
		<script language="JavaScript">
		var volte = 0;
		var trovato = false;
		var base='';
		
		function inizia() 
		{
			base = opener.document.body.createTextRange();
		}

		function trova() 
		{
			if (document.MioForm.ddl_funz.value == '') 
			{
				alert('Nulla da cercare!');
				return;
			}
			if (volte==0) 
			{
				inizia()
			}
			
			trovato=base.findText(document.MioForm.ddl_funz.value)
			
			if (trovato) 
			{
				base.findText(document.MioForm.ddl_funz.value);
				base.select();
				base.scrollIntoView();
				volte++;
				base.moveStart("character", 1);  
				base.moveEnd("textedit");
			}
			else 
			{
				if (volte == '0')
				{
					alert('"' + document.MioForm.ddl_funz.value +'" non è stato trovato!');
				}
				else 
				{
					alert('"' + document.MioForm.ddl_funz.value+'"  è  stato trovato  '+ volte+' volte.');
					volte=0;
				}
			}
		}
		</script>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="MioForm" method="post" runat="server">
			<table border="0" cellpadding="0" cellspacing="0" width="95%" align="center">												
				<tr>
					<td colspan="2" class="testo_grigio_scuro">Funzione elementare da ricercare:
					</td>
				</tr>
				<tr>
					<td class="testo">
						<asp:DropDownList id="ddl_funz" runat="server" CssClass="testo" Width="200px"></asp:DropDownList></td>
					<td>
						<input type="button" value="Trova" onClick="trova()" class="testo_btn"></td>
				</tr>
				<tr>
					<td colspan="2" class="testo_grigio_scuro">&nbsp;</td>
				</tr>
				<tr>
					<td colspan="2" class="testo_grigio_scuro">&nbsp;</td>
				</tr>
				<tr>
					<td colspan="2" height="10" align="center" class="testo_grigio_scuro">|&nbsp;&nbsp;&nbsp;<a href="javascript:self.close()">Chiudi</a>&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
