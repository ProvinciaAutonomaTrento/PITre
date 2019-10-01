<%@ Page language="c#" Codebehind="PermessiFunzioni.aspx.cs" AutoEventWireup="false" Inherits="Amministrazione.Gestione_Funzioni.PermessiFunzioni" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>DOCSPA - AMMINISTRAZIONE > Permessi..............................................................................................</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
		<script language="javascript">
			function body_onLoad()
			{
				var newLeft=0;
				var newTop=0;
				window.moveTo(newLeft,newTop);
			}	
			
			var volte = 0;
			var trovato = false;
			var base='';
			
			function inizia() 
			{
				base = document.body.createTextRange();
			}

			function trova(codice) 
			{		
				if (codice == '') 
				{
					alert('Nulla da cercare!');
					return;
				}
				if (volte==0) 
				{
					inizia()
				}
				
				trovato=base.findText(codice)
				
				if (trovato) 
				{
					base.findText(codice);
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
						alert('"' + codice +'" non trovato!');
					}
					else 
					{
						//alert('"' + codice +'"  è  stato trovato  '+ volte +' volte.');
						volte=0;
					}
				}
			}		
		</script>
	</HEAD>
	<body leftmargin="5" topmargin="5" MS_POSITIONING="GridLayout" onload="body_onLoad();">
		<form id="MioForm" method="post" runat="server">
			<table border="0" cellpadding="0" cellspacing="0" width="95%" align="center">
				<tr>
					<td height="10" align="right" class="testo_grigio_scuro">|&nbsp;&nbsp;&nbsp;Ricerca: Ctrl + F&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;<a href="javascript:self.close()">Chiudi</a>&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;</td>
				</tr>
				<tr>
					<td height="48" align="left"><img src="../Images/logo.gif" border="0" height="48" width="218"></td>
				</tr>
				<tr>
					<td class="testo_grigio_scuro" align="right">
						<asp:Label id="lbl_testa" runat="server"></asp:Label></td>
				</tr>
				<tr>
					<td height="15"></td>
				</tr>
				<tr>
					<td id="textArea" class="testo" height="100%" width="100%" runat="server">
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
