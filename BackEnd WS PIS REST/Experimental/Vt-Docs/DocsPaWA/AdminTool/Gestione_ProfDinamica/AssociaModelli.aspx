<%@ Page language="c#" Codebehind="AssociaModelli.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.AdminTool.Gestione_ProfDinamica.AssociaModelli" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
 
 <script language="javascript">
     var w = window.screen.width;
     var h = window.screen.height;
     var new_w = (w - 500) / 2;
     var new_h = (h - 400) / 2;  

    function VisualizzaModello() {
        window.open('ModelAnteprimaModelli.aspx', '', 'width=800px,height=600px,top=50,left=100,toolbar=no,directories=no,menubar=no,resizable=yes, scrollbars=no, center=yes');
        //window.showModalDialog('ModelAnteprimaModelli.aspx', '', 'dialogWidth:800px;dialogHeight:650px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:' + new_h + ';left:' + new_w);				
    }

    // Funzione per richiamare la pagina di creazione di un esempio di data source 
    // relativo al template utilizzato
    function generateSampleDataSource() {
        var pageURL = '<%=UrlToMainSourceGeneratorPage%>';

        var w_width = 100;
        var w_height = 100;
        var t = (screen.availHeight - w_height) / 2;
        var l = (screen.availWidth - w_width) / 2;

        window.open(pageURL, "dataSourceSample","width=1,height=1,top=" + t + ",left=" + l);
    }

 </script>
<HTML>
	<HEAD runat = "server">
        <title></title>	
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
            <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Associa modelli" />
			<table width="100%">
				<tr>
					<td class="titolo" align="center" bgColor="#e0e0e0" height="20">
						<asp:Label id="lbl_titolo" runat="server">Modelli</asp:Label></td>
					<td align="center" width="10%">
						<asp:Button id="btn_conferma" runat="server" Text="Conferma" CssClass="testo_btn_p"></asp:Button></td>
				</tr>
               
               
			</table>
			<BR>
			
            <table width="100%">
             <tr>
                <td align="right" >
                <asp:HyperLink id="linkTag" runat="server" Target="_blank" CssClass="testo_grigio_scuro" ForeColor="brown">Scarica L'elenco dei tag ammessi</asp:HyperLink> 
                </td>
             </tr>
             </table>

			<table width="100%" bgColor="#f6f4f4" style="BORDER-RIGHT: #810d06 1px solid; BORDER-TOP: #810d06 1px solid; BORDER-LEFT: #810d06 1px solid; BORDER-BOTTOM: #810d06 1px solid">
                <tr id="trGenerateMTextSourceModel" runat="server">
                    <td align="center" colspan="4">
                        <input type="button" id="btnGenerateModelSampleMain" value="Genera data source di esempio" 
                                class="testo_btn_p" onclick="generateSampleDataSource();" style="width:100%; margin: 5 5 2 5;" /> 
                        <br />
                    </td>
                </tr>
                <tr runat="server" id="trModelChoiceMainDocument" visible="false">
                    <td class="testo" colspan="3">
                        Tipo modello per Doc. Principale:
                        &nbsp;
                        <asp:DropDownList runat="server" ID="ddlModelTypeMain" AutoPostBack="true" OnSelectedIndexChanged="ddlModelTypeMain_SelectedIndexChanged" CssClass="testo">
                            <asp:ListItem>Word (RTF)</asp:ListItem>
                            <asp:ListItem>MText</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
					<td>
                        <asp:Panel ID="pnlModelTypeMain" runat="server" Visible="false">
                            <asp:DropDownList ID="ddlMTextModelMain" Width="210" AutoPostBack="true" runat="server" CssClass="testo"/>
                            <br />
                            <asp:TextBox ID="txtModelNameMain" runat="server" CssClass="testo" Width="210" />&nbsp;
                            <asp:Button ID="btnAddTemplateDoc" CssClass="testo_btn_p" runat="server" Text="Aggiungi..." OnClick="btnAddTemplateDoc_Click" />
                        </asp:Panel>
                        <input type="file" runat="server" class="testo" id="uploadPathUno" size="50" name="uploadPathUno" />
                    </td>
					<td align="center">
						<asp:CheckBox id="CheckBox1" runat="server" Text="Modello 1" Enabled="False" CssClass="testo"
							Font-Bold="True"></asp:CheckBox></td>
					<td align="center">
						<asp:ImageButton id="Modello1" runat="server" AlternateText="Visualizza Modello 1" ImageUrl="../Images/lentina.gif"></asp:ImageButton>
                    </td>
                    <td align="center">
						<asp:ImageButton id="ImageButton1" runat="server" ImageUrl="../Images/cestino.gif"></asp:ImageButton></td>
				</tr>
                <tr runat="server" id="trModelChoiceAtt" visible="false">
                    <td class="testo" colspan="3">
                        Tipo modello per Allegato:
                        &nbsp;
                        <asp:DropDownList runat="server" ID="ddlModelTypeAtt" AutoPostBack="true" OnSelectedIndexChanged="ddlModelTypeAtt_SelectedIndexChanged" CssClass="testo">
                            <asp:ListItem>Word (RTF)</asp:ListItem>
                            <asp:ListItem>MText</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
				<tr>
					<td>
                        <asp:Panel ID="pnlModelTypeAtt" runat="server" Visible="false">
                            <asp:DropDownList ID="ddlMTextModelAtt" AutoPostBack="true" Width="210" runat="server" CssClass="testo" />
                            <br />
                            <asp:TextBox ID="txtModelNameAtt" runat="server" CssClass="testo" Width="210" />&nbsp;
                            <asp:Button ID="btnAddTemplateAtt" CssClass="testo_btn_p" runat="server" Text="Aggiungi..." OnClick="btnAddTemplateAtt_Click" />
                        </asp:Panel>
                        <INPUT type="file" runat="server" class="testo" id="uploadPathDue" size="50" name="uploadPathDue" />
                    </td>
					<td align="center">
						<asp:CheckBox id="CheckBox2" runat="server" Text="Modello 2" Enabled="False" CssClass="testo"
							Font-Bold="True"></asp:CheckBox></td>
					<td align="center">
						<asp:ImageButton id="Modello2" runat="server" AlternateText="Visualizza Modello 2" ImageUrl="../Images/lentina.gif"></asp:ImageButton></td>
                    <td align="center">
						<asp:ImageButton id="ImageButton2" runat="server" ImageUrl="../Images/cestino.gif"></asp:ImageButton></td>
				</tr>
                <asp:Panel ID="PanelSU" runat="server">
                <tr id="TrSU" runat="server">
                   <td><INPUT type="file" runat="server" class="testo" id="uploadPathSU" size="50" name="uploadPathModelloSU"></td>
					<td align="center">
						<asp:CheckBox id="CheckBox4" runat="server" Text="Stampa Un.&nbsp;&nbsp;" Enabled="False" CssClass="testo"
							Font-Bold="True"></asp:CheckBox></td>
					<td align="center">
						<asp:ImageButton id="ModelloSU" runat="server" AlternateText="Visualizza Modello Stampa unione" ImageUrl="../Images/lentina.gif"></asp:ImageButton></td>
                    <td align="center">
                        <asp:ImageButton id="ImageButton4" runat="server" ImageUrl="../Images/cestino.gif"></asp:ImageButton></td>
                </tr>
                </asp:Panel>
				<tr>
					<td><INPUT type="file" runat="server" class="testo" id="uploadPathAllUno" size="50" name="uploadPathAllUno"></td>
					<td align="center">
						<asp:CheckBox id="CheckBox3" runat="server" Text="Allegato&nbsp;&nbsp;" Enabled="False" CssClass="testo"
							Font-Bold="True"></asp:CheckBox></td>
					<td align="center">
						<asp:ImageButton id="Allegato" runat="server" AlternateText="Visualizza Allegato" ImageUrl="../Images/lentina.gif"></asp:ImageButton></td>
                    <td align="center">
                        <asp:ImageButton id="ImageButton3" runat="server" ImageUrl="../Images/cestino.gif"></asp:ImageButton></td>
				</tr>
                <tr id="Tr2" runat="server">
                   <td><INPUT type="file" runat="server" class="testo" id="uploadPathModExc" size="50" name="uploadPathModExc"></td>
					<td align="center">
						<asp:CheckBox id="CheckBox5" runat="server" Text="Modello Export" Enabled="False" CssClass="testo"
							Font-Bold="True"></asp:CheckBox></td>
					<td align="center">
						<asp:ImageButton id="ModelloExc" runat="server" AlternateText="Visualizza Modello per export" ImageUrl="../Images/lentina.gif"></asp:ImageButton></td>
                    <td align="center">
                        <asp:ImageButton id="ImageButton6" runat="server" ImageUrl="../Images/cestino.gif"></asp:ImageButton></td>
                </tr>
			</table>
		</form>
	</body>
</HTML>
