<%@ Page language="c#" Codebehind="Segn_Fasc.aspx.cs" AutoEventWireup="false" Inherits="Amministrazione.Gestione_Homepage.Segn_Fasc" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>DOCSPA - AMMINISTRAZIONE</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
		<script language="javascript">
		<!--
			function TrasmDati(cod,tipo) {			
				if(tipo == "segn")
				{
					window.opener.document.Form1.txt_segnatura.value+=cod;
				}				
				else
				{
					window.opener.document.Form1.txt_fascicolatura.value+=cod;
				}
			}
		-->
		</script>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="frm" method="post" runat="server">
			<table cellSpacing="0" cellPadding="0" align="center" border="0">
				<tr>
					<td class="testo_grigio_scuro" align="right">|&nbsp;&nbsp;&nbsp;<A href="javascript:self.close()">Chiudi</A>&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;</td>
				</tr>
				<tr>
					<td height="20"></td>
				</tr>
				<tr>
					<td class="testo_grigio_scuro" align="center" height="20"><asp:label id="lbl_testa" runat="server"></asp:label></td>
				</tr>				
				<tr>
					<td align="center">
						<table cellSpacing="0" cellPadding="3" border="1">
							<tr bgcolor="#e0e0e0">
								<td>&nbsp;</td>
								<td class="titolo" align="center">Codice</td>
								<td class="titolo" align="center">Descrizione</td>
							</tr>
							<tr id="COD_AMM" runat="server">
								<td bgcolor="#e0e0e0"><input onclick="TrasmDati('COD_AMM', document.frm.txt_tipo.value)" type="image" alt="Aggiungi"
										src="../Imagesaggiungi.gif" name="image1">
								</td>
								<td class="testo_grigio_scuro">COD_AMM</td>
								<td class="testo_grigio_scuro">Codice dell'Amm.ne</td>
							</tr>
							<tr id="COD_REG" runat="server">
								<td bgcolor="#e0e0e0"><input onclick="TrasmDati('COD_REG', document.frm.txt_tipo.value)" type="image" alt="Aggiungi"
										src="../Images/aggiungi.gif" name="image2">
								</td>
								<td class="testo_grigio_scuro">COD_REG</td>
								<td class="testo_grigio_scuro">Codice del Registro</td>
							</tr>
							<tr id="COD_TITOLO" runat="server">
								<td bgcolor="#e0e0e0"><input onclick="TrasmDati('COD_TITOLO', document.frm.txt_tipo.value)" type="image" alt="Aggiungi"
										src="../Imagesaggiungi.gif" name="image3">
								</td>
								<td class="testo_grigio_scuro">COD_TITOLO</td>
								<td class="testo_grigio_scuro">Codice e Titolo</td>
							</tr>
							<tr id="COD_UO" runat="server">
								<td bgcolor="#e0e0e0"><input onclick="TrasmDati('COD_UO', document.frm.txt_tipo.value)" type="image" alt="Aggiungi"
										src="../Images/aggiungi.gif" name="image4">
								</td>
								<td class="testo_grigio_scuro">COD_UO</td>
								<td class="testo_grigio_scuro">Codice dell'Unità Org.va</td>
							</tr>
							<tr id="DATA_ANNO" runat="server">
								<td bgcolor="#e0e0e0"><input onclick="TrasmDati('DATA_ANNO', document.frm.txt_tipo.value)" type="image" alt="Aggiungi"
										src="../Imagesaggiungi.gif" name="image5">
								</td>
								<td class="testo_grigio_scuro">DATA_ANNO</td>
								<td class="testo_grigio_scuro">Anno</td>
							</tr>
							<tr id="DATA_COMP" runat="server">
								<td bgcolor="#e0e0e0"><input onclick="TrasmDati('DATA_COMP', document.frm.txt_tipo.value)" type="image" alt="Aggiungi"
										src="../Images/aggiungi.gif" name="image6">
								</td>
								<td class="testo_grigio_scuro">DATA_COMP</td>
								<td class="testo_grigio_scuro">Data della protoc.ne</td>
							</tr>
							<tr id="IN_OUT" runat="server">
								<td bgcolor="#e0e0e0"><input onclick="TrasmDati('IN_OUT', document.frm.txt_tipo.value)" type="image" alt="Aggiungi"
										src="../Imagesaggiungi.gif" name="image7">
								</td>
								<td class="testo_grigio_scuro">IN_OUT</td>
								<td class="testo_grigio_scuro">Ingresso / Uscita</td>
							</tr>
							<tr id="NUM_PROG" runat="server">
								<td bgcolor="#e0e0e0"><input onclick="TrasmDati('NUM_PROG', document.frm.txt_tipo.value)" type="image" alt="Aggiungi"
										src="../Images/aggiungi.gif" name="image8">
								</td>
								<td class="testo_grigio_scuro">NUM_PROG</td>
								<td class="testo_grigio_scuro">Numero del fascicolo</td>
							</tr>
							<tr id="NUM_PROTO" runat="server">
								<td bgcolor="#e0e0e0"><input onclick="TrasmDati('NUM_PROTO', document.frm.txt_tipo.value)" type="image" alt="Aggiungi"
										src="../Images/aggiungi.gif" name="image9">
								</td>
								<td class="testo_grigio_scuro">NUM_PROTO</td>
								<td class="testo_grigio_scuro">Numero di protocollo</td>
							</tr>
						</table>
						<br>
						<table cellSpacing="0" cellPadding="3" border="1">
							<tr bgcolor="#e0e0e0">
								<td colspan="3" class="titolo" align="center">Separatori</td>
							</tr>
							<tr>
								<td bgcolor="#e0e0e0">
									<input onclick="TrasmDati('|', document.frm.txt_tipo.value)" type="image" alt="Aggiungi"
										src="../Images/aggiungi.gif" name="image10">
								</td>
								<td class="testo_grigio_scuro" align="center" width="20">|</td>
								<td class="testo_grigio_scuro" align="center">Pipe</td>
							</tr>
							<tr>
								<td bgcolor="#e0e0e0">
									<input onclick="TrasmDati('.', document.frm.txt_tipo.value)" type="image" alt="Aggiungi"
										src="../Images/aggiungi.gif" name="image11">
								</td>
								<td class="testo_grigio_scuro" align="center" width="20">.</td>
								<td class="testo_grigio_scuro" align="center">Punto</td>
							</tr>
							<tr>
								<td bgcolor="#e0e0e0">
									<input onclick="TrasmDati('-', document.frm.txt_tipo.value)" type="image" alt="Aggiungi"
										src="../Images/aggiungi.gif" name="image12">
								</td>
								<td class="testo_grigio_scuro" align="center" width="20">-</td>
								<td class="testo_grigio_scuro" align="center">Meno</td>
							</tr>
							<tr>
								<td bgcolor="#e0e0e0">
									<input onclick="TrasmDati('\', document.frm.txt_tipo.value)" type="image" alt="Aggiungi"
										src="../Images/aggiungi.gif" name="image13">
								</td>
								<td class="testo_grigio_scuro" align="center" width="20">\</td>
								<td class="testo_grigio_scuro" align="center">Backslash</td>
							</tr>							
							<tr>
								<td bgcolor="#e0e0e0">
									<input onclick="TrasmDati('_', document.frm.txt_tipo.value)" type="image" alt="Aggiungi"
										src="../Images/aggiungi.gif" name="image15">
								</td>
								<td class="testo_grigio_scuro" align="center" width="20">_</td>
								<td class="testo_grigio_scuro" align="center">Underscore</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
			<br>
			<br>
			<br>
			<br>
			<br>
			<br>
			<br>
			<br>
			<br>
			<br>
			<br>
			<asp:textbox id="txt_tipo" runat="server" Visible="True"></asp:textbox>
		</form>
	</body>
</HTML>
