<%@ Page language="c#" Codebehind="Inserimento.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.Inserisci" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register src="../../UserControls/Calendar.ascx" TagName="Calendario"  tagprefix="uc3" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD runat="server">
  	    <title></title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<LINK href="../../CSS/docspa.css" type="text/css" rel="stylesheet">
		<LINK href="../../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<base target="_self">
		<script language="javascript" id="btn_Insert_click" event="onclick()" for="btn_Insert">
//			document.getElementById('btn_Insert').style.display='none';
//			document.getElementById('btn_Insert_disabled').style.display='';
            var tipoCorrispondente = document.getElementById('ddl_tipoCorr').value;
            if(tipoCorrispondente == 'U' && document.getElementById('uo_codfisc').value.length == 16)
            {
               if(confirm('Per un corrispondente di tipo UO stai inserendo/modificando il campo Codice fiscale con uno di tipo persona, sei sicuro di voler proseguire?'))
               {
                    document.getElementById('btn_Insert').style.display='none';
			        document.getElementById('btn_Insert_disabled').style.display='';
                    return true;
               }
               else
                 return false;
            }
            else
            {
                document.getElementById('btn_Insert').style.display='none';
			    document.getElementById('btn_Insert_disabled').style.display='';
                return true;
            }
		</script>
		<script language="javascript">

			function nascondi()
			{
				document.getElementById('btn_Insert_disabled').style.display='none';
				
			}
			
			
	function eventoCombo()
    {
         var ddlReg = document.getElementById("ddl_registri");
         var lbl_registro = document.getElementById("lbl_registro");
         var inserisci = document.getElementById("btn_Insert");
         
         var rfPresente = false;
         
         if(ddlReg!=null)
       {
            for (li=0;li<(document.getElementById('ddl_registri').options.length);li++)
            {
                var valore = document.getElementById('ddl_registri').options[li].value;

                var arrayValori = valore.split("_");
               
                if(arrayValori != null && arrayValori.length > 1)
                {
                  if(arrayValori[1]!=null && arrayValori[1]!="")
                  {
                    rfPresente = true;
                     break;
                  }

                }
            }
           
            if(rfPresente)
            {          
                lbl_registro.innerHTML = 'Registro/RF';
            }
            else
            {
                lbl_registro.innerHTML = 'Registro';       
            }
       
            var indice_selezionato = document.getElementById('ddl_registri').selectedIndex;	
            
            var item= null;
            var valore= null;
            var arrayValori=null;
            
            if(indice_selezionato!=-1)
            {
               item = document.getElementById('ddl_registri').options[indice_selezionato];
               valore = item.value;
               arrayValori = valore.split("_");
            }            
           
            if(arrayValori != null && arrayValori.length > 1)
            {
              if(arrayValori[1]!=null && arrayValori[1]!="")
              {
               // item.style.color = 'Gray';  
              
                if(arrayValori[1]=="1")
                {
                   // lbl_registro.innerHTML = 'Rf disabilitato'; 
                    inserisci.disabled = true;
                  
                    inserisci.title = 'Non è possibile creare un nuovo corrispondente in un RF disabilitato'; 
                }
                else
                {
                   // lbl_registro.innerHTML = 'Rf'; 
                    inserisci.disabled = false;
                    inserisci.title = 'Inserisci'; 
                }
                     
              }
              else
              {
               // lbl_registro.innerHTML = 'Registro';
                inserisci.title = 'Inserisci'; 
                inserisci.disabled = false;
              }    
                             
           }
        }
    }
    
    
   /* function colorCombo()
    {
         var ddlReg = document.getElementById("ddl_registri");
         var lbl_registro = document.getElementById("lbl_registro");
       
         
         if(ddlReg!=null)
           {
                for (li=0;li<(document.getElementById('ddl_registri').options.length);li++)
                {
                    var item = document.getElementById('ddl_registri').options[li];
                    
                    var valore = item.value;

                    var arrayValori = valore.split("_");
                   
                    if(arrayValori != null && arrayValori.length > 1)
                    {
                      if(arrayValori[1]!=null && arrayValori[1]!="")
                      {
                        item.style.color = 'Gray';
                      }              
                    }
                    
                }
               
           }
    }
    */
    
		</script>
</HEAD>
	<body bottomMargin="2" leftMargin="2" topMargin="3" rightMargin="2" MS_POSITIONING="GridLayout" onload="eventoCombo()">
    <div align="center">
		<form id="InsDest" method="post" runat="server">
		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Inserimento corrispondenti" />
			<TABLE class="info" id="tbl_dettagliCorrispondenti" height="620" width="510" align="center"
				cellpadding="0" cellspacing="0" border="0">
				<TR>
					<td class="item_editbox" colSpan="2" valign="middle" height="25">
						<P class="boxform_item"><asp:label id="Label1" runat="server">Inserimento Corrispondente</asp:label></P>
					</td>
				</TR>
				<TR>
					<TD style="HEIGHT: 5px" vAlign="top" align="center" width="4" height="8"><IMG height="6" src="../../images/proto/spaziatore.gif" width="6" border="0"></TD>
					<TD style=" HEIGHT: 5px" vAlign="middle" align="left" colSpan="4" height="8"></TD>
				</TR>
				<asp:Panel runat="server">
					<tr valign="top" height="120">
						<td colspan="2">
                        <table border="0" cellpadding="0" cellspacing="0" width="100%">
								<TR height="23">
									<TD class="titolo_scheda"><IMG height="1" src="../../images/proto/spaziatore.gif" width="8" border="0">
									<asp:label id="lbl_registro" runat="server" Width="130px">Registro</asp:label></TD>
									<TD align="left"><asp:dropdownlist id="ddl_registri" runat="server" Width="315px" CssClass="testo_grigio"  onchange="eventoCombo()"></asp:dropdownlist></TD>
								</TR>
								<TR height="23">
									<td class="titolo_scheda"><IMG height="1" src="../../images/proto/spaziatore.gif" width="8" border="0"><asp:label id="TipoCorr" runat="server" Width="130px">Tipo Corrispondente</asp:label></td>
									<TD align="left"><asp:dropdownlist id="ddl_tipoCorr" runat="server" AutoPostBack="True" CssClass="testo_grigio" Width="315px">
											<asp:ListItem Value="U" Selected="True">UO</asp:ListItem>
											<asp:ListItem Value="R">RUOLO</asp:ListItem>
											<asp:ListItem Value="P">PERSONA</asp:ListItem>
										</asp:dropdownlist></TD>
								</TR>
								<TR height="23">
									<TD class="titolo_scheda"><IMG height="1" src="../../images/proto/spaziatore.gif" width="8" border="0"><asp:label id="codRubr" runat="server" Width="130px">Codice rubrica *</asp:label></TD>
									<TD align="left"><asp:textbox id="CodRubrica" runat="server" CssClass="testo_grigio" Width="315px" MaxLength="32"></asp:textbox></TD>
								</TR>
								<TR height="23">
									<TD class="titolo_scheda"><IMG height="1" src="../../images/proto/spaziatore.gif" width="8" border="0"><asp:label id="lbl_canpref" runat="server" Width="130px">Canale preferenziale</asp:label></TD>
									<TD align="left"><asp:dropdownlist id="dd_canpref" runat="server" AutoPostBack="True" CssClass="testo_grigio" Width="315px"></asp:dropdownlist></TD>
								</TR>
								<TR height="23">
									<TD class="titolo_scheda"><IMG height="1" src="../../images/proto/spaziatore.gif" width="8" border="0"><asp:label id="lbl_amm" runat="server" Width="70px">Codice AMM</asp:label><asp:label id="star_amm" Runat="server" Visible="false">*</asp:label></TD>
									<TD align="left"><asp:textbox id="CodAmm" runat="server" CssClass="testo_grigio" Width="315px" MaxLength="32"></asp:textbox></TD>
								</TR>
								<TR height="23">
									<TD class="titolo_scheda"><IMG height="1" src="../../images/proto/spaziatore.gif" width="8" border="0"><asp:label id="lbl_aoo" runat="server" Width="70px">Codice AOO</asp:label><asp:label id="star_aoo" Runat="server" Visible="false">*</asp:label></TD>
									<TD align="left"><asp:textbox id="CodAoo" runat="server" CssClass="testo_grigio" Width="315px" MaxLength="32"></asp:textbox></TD>
								</TR>
                                <asp:ScriptManager ID="scrManager" runat="server"></asp:ScriptManager>
								<tr height="23">
									<td class="titolo_scheda"><IMG height="1" src="../../images/proto/spaziatore.gif" width="8" border="0">
                                        <asp:label id="lbl_email" runat="server" Width="32px">Email</asp:label>
                                        <asp:label id="star" Runat="server">*</asp:label>
                                    </td>
                                    <td align="left">
                                        <asp:UpdatePanel ID="UpdatePanelMail" runat="server" UpdateMode="Always">
                                            <ContentTemplate>
                                                <asp:TextBox ID="txtCasella" CssClass="testo_grigio" runat="server" Width="315"></asp:TextBox>
                                                <asp:ImageButton AlternateText="Aggiungi casella di posta" ImageAlign="AbsMiddle" OnClick="imgAggiungiCasella_Click"
                                                    ID="imgAggiungiCasella" runat="server" ToolTip="Aggiungi casella di posta" ImageUrl="~/images/proto/aggiungi.gif" />
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </td>
                                </tr>
                                <tr height="23">
                                    <td class="titolo_scheda"><IMG height="1" src="../../images/proto/spaziatore.gif" width="8" border="0">
                                        <asp:label ID="lblNote" runat="server" Width="130px">Note E-mail</asp:label>&nbsp;
                                    </td>
                                    <td align="left">
                                        <asp:UpdatePanel ID="UpdatePanelNote" runat="server" UpdateMode="Always">
                                            <ContentTemplate>
                                                <asp:TextBox ID="txtNote" CssClass="testo_grigio" runat="server" Width="315" MaxLength="20"></asp:TextBox>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </td>
                                </tr>
                                <tr height="23">
                                    <td class="titolo_scheda">
                                        <IMG height="1" src="../../images/proto/spaziatore.gif" width="8" border="0">
                                    </td>
                                    <td align="left">
                                    <asp:UpdatePanel ID="PanelMail" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                           <%--  <div id="divGridViewCaselle" runat="server" style="width:315px;padding:0px; height:50px">--%>
                                                            <asp:GridView ID="gvCaselle" runat="server" AutoGenerateColumns="false"
                                                                CellPadding="1" BorderWidth="1px" BorderColor="Gray" Width="315px"> 
                                                                <SelectedRowStyle CssClass="bg_grigioS"></SelectedRowStyle>
                                                                <AlternatingRowStyle CssClass="bg_grigioA"></AlternatingRowStyle>
                                                                <RowStyle CssClass="bg_grigioN"> </RowStyle>
                                                                <HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>                                                                                      
                                                                <Columns>
                                                                    <asp:TemplateField HeaderText="SystemId" Visible="false">
                                                                            <ItemTemplate>
                                                                                <asp:Label runat="server" ID ="lblSystemId" Text ='<%# ((DocsPAWA.DocsPaWR.MailCorrispondente)Container.DataItem).systemId %>'></asp:Label>
                                                                            </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Email" ShowHeader="true">
                                                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                                                            <ItemTemplate>
                                                                                <asp:TextBox AutoPostBack="true" OnTextChanged="txtEmailCorr_TextChanged" CssClass="testo_grigio" ID="txtEmailCorr" runat="server" Text ='<%# ((DocsPAWA.DocsPaWR.MailCorrispondente)Container.DataItem).Email %>'></asp:TextBox>
                                                                            </ItemTemplate>
                                                                    </asp:TemplateField> 
                                                                    <asp:TemplateField HeaderText="Note E-mail" ShowHeader="true">
                                                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                                                            <ItemStyle  HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                                                            <ItemTemplate>
                                                                                <asp:TextBox AutoPostBack="true" CssClass="testo_grigio" style="margin:1px;" OnTextChanged="txtNoteMailCorr_TextChanged" MaxLength="20" ID="txtNoteMailCorr" runat="server" Text ='<%# ((DocsPAWA.DocsPaWR.MailCorrispondente)Container.DataItem).Note %>'></asp:TextBox>
                                                                            </ItemTemplate>
                                                                    </asp:TemplateField> 
                                                                    <asp:TemplateField HeaderText="*" ShowHeader="true">
                                                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                                                            <ItemTemplate> 
                                                                                <asp:RadioButton OnCheckedChanged="rdbPrincipale_ChekedChanged" ID="rdbPrincipale" runat="server" AutoPostBack="true" Checked='<%# TypeMailCorrEsterno(((DocsPAWA.DocsPaWR.MailCorrispondente)Container.DataItem).Principale) %>' />
                                                                            </ItemTemplate>
                                                                    </asp:TemplateField> 
                                                                    <asp:TemplateField HeaderText="" ShowHeader="true">
                                                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                                                            <ItemTemplate>
                                                                                <asp:ImageButton ID="imgEliminaCasella" runat="server"  OnClick="imgEliminaCasella_Click" AutoPostBack="true" ImageUrl="~/images/proto/cancella.gif" />
                                                                            </ItemTemplate>
                                                                    </asp:TemplateField> 
                                                                </Columns>
                                                            </asp:GridView>
                                            <%-- </div>--%>
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="imgAggiungiCasella" EventName="Click" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                    </td>
                                </tr>
							</table>
						</td>
					</tr>
				</asp:Panel>
				<asp:panel id="pnlUO" runat="server">
                <TR vAlign="top">
                <TD colSpan="2">
      <table border="0" cellpadding="0" cellspacing="0" width="100%"><!--PANNELLO DATI UO-->
        <TR  height="23">
            <TD  align="left" class=titolo_scheda><IMG height="1" src="../../images/proto/spaziatore.gif" width="6" border="0">
                <asp:label id=lbl_desc_nome runat="server" Width="130px">Descrizione *</asp:label>
            </TD>
            <TD align="left"><asp:textbox id=uo_descr  runat="server" Width="315px" CssClass="testo_grigio" MaxLength="256"></asp:textbox>
            </TD>
        </TR>
        <TR height="23">
          <TD class=titolo_scheda><IMG height=1 
            src="../../images/proto/spaziatore.gif" width=8 border=0><asp:label id=lbl_indirizzo runat="server" Width="130px">Indirizzo</asp:label></TD>
          <TD align="left">
            <asp:textbox id=uo_indirizzo runat="server" Width="315px" CssClass="testo_grigio" MaxLength="128"></asp:textbox></TD></TR>
        <TR height=23>
          <TD class=titolo_scheda><IMG height=1 
            src="../../images/proto/spaziatore.gif" width=8 border=0><asp:label id=lbl_cap runat="server" Width="130px">Cap</asp:label></TD>
          <TD align="left">
<asp:textbox id=uo_cap runat="server" Width="315px" CssClass="testo_grigio" MaxLength="5"></asp:textbox></TD></TR>
        <TR height=23>
          <TD class=titolo_scheda><IMG height=1 
            src="../../images/proto/spaziatore.gif" width=8 border=0><asp:label id=lbl_citta runat="server">Citta'</asp:label></TD>
          <TD align="left">
<asp:textbox id=uo_citta runat="server" Width="315px" CssClass="testo_grigio" MaxLength="64"></asp:textbox></TD></TR>
        <TR height=23>
          <TD class=titolo_scheda><IMG height=1 
            src="../../images/proto/spaziatore.gif" width=8><asp:label id=lbl_provincia runat="server">Provincia</asp:label></TD>
          <TD align="left">
<asp:textbox id=uo_prov runat="server" Width="315px" CssClass="testo_grigio" MaxLength="2"></asp:textbox></TD></TR>
        <TR height=23>
          <TD class=titolo_scheda><IMG height=1 
            src="../../images/proto/spaziatore.gif" width=8><asp:label id="lbl_localita" runat="server">Località</asp:label></TD>
          <TD align="left">
<asp:textbox id="uo_local" runat="server" Width="315px" CssClass="testo_grigio" MaxLength="128"></asp:textbox></TD></TR>
        <TR height=23>
          <TD class=titolo_scheda><IMG height=1 
            src="../../images/proto/spaziatore.gif" width=8 border=0><asp:label id=lbl_nazione runat="server">Nazione</asp:label></TD>
          <TD align="left">
<asp:textbox id=uo_nazione runat="server" Width="315px" CssClass="testo_grigio" MaxLength="32"></asp:textbox></TD></TR>
        <TR height=23>
          <TD class=titolo_scheda><IMG height=1 
            src="../../images/proto/spaziatore.gif" width=8 border=0><asp:label id=lbl_telefono1 runat="server">Telefono princ.</asp:label></TD>
          <TD align="left">
<asp:textbox id=uo_telefono1 runat="server" Width="315px" CssClass="testo_grigio" MaxLength="16"></asp:textbox></TD></TR>
        <TR height=23>
          <TD class=titolo_scheda><IMG height=1 
            src="../../images/proto/spaziatore.gif" width=8 border=0><asp:label id=lbl_telefono2 runat="server">Telefono sec.</asp:label></TD>
          <TD align="left">
<asp:textbox id=uo_telefono2 runat="server" Width="315px" CssClass="testo_grigio" MaxLength="16"></asp:textbox></TD></TR>
        <TR height=23>
          <TD class=titolo_scheda><IMG height=1 
            src="../../images/proto/spaziatore.gif" width=8 border=0><asp:label id=lbl_fax runat="server">Fax</asp:label></TD>
          <TD align="left">
<asp:textbox id=uo_fax runat="server" Width="315px" CssClass="testo_grigio" MaxLength="16"></asp:textbox></TD></TR>
        <TR height=23>
          <TD class=titolo_scheda><IMG height=1 
            src="../../images/proto/spaziatore.gif" width=8 border=0><asp:label id=lbl_codfisc_uo runat="server">C.F.</asp:label></TD>
          <TD align="left">
<asp:textbox id=uo_codfisc runat="server" Width="315px" CssClass="testo_grigio" MaxLength="16"></asp:textbox></TD></TR>
 <TR height=23>
          <TD class=titolo_scheda><IMG height=1 
            src="../../images/proto/spaziatore.gif" width=8 border=0><asp:label id=lbl_partitaiva_uo runat="server">P.IVA</asp:label></TD>
          <TD align="left">
<asp:textbox id=uo_partitaiva runat="server" Width="315px" CssClass="testo_grigio" MaxLength="11"></asp:textbox></TD></TR>
        <TR height=23>
          <TD class=titolo_scheda><IMG height=1 
            src="../../images/proto/spaziatore.gif" width=8 border=0><asp:label id=lbl_note runat="server">Note</asp:label></TD>
          <TD align="left">
<asp:textbox id=uo_note runat="server" Width="315px" CssClass="testo_grigio" MaxLength="250"></asp:textbox></TD></TR></TABLE></TD></TR>
				</asp:panel>
                    <asp:panel id="pnlRuolo" runat="server" Visible="False">
  <TR vAlign=top>
    <TD colSpan=2>
      <TABLE cellSpacing=0 cellPadding=0 width="100%" border=0>
        <TR height=23><TD class=titolo_scheda><IMG height=1 src="../../images/proto/spaziatore.gif" width=8 border=0><asp:label id=Label2 runat="server" Width="130px">Descrizione *</asp:label></TD>
          <TD align="left"><asp:textbox id=r_descr runat="server" Width="315px" CssClass="testo_grigio" MaxLength="256"></asp:textbox></TD></TR></TABLE></TD></TR>
</asp:panel>
                    <asp:panel id="pnlUtente" runat="server" Visible="False">
  <TR vAlign=top>
    <TD colSpan=2>
      <TABLE cellSpacing=0 cellPadding=0 width="100%" border=0><!--PANNELLO DATI UTENTE-->
        <TR height=23>
        <TD class=titolo_scheda><IMG height=1 src="../../images/proto/spaziatore.gif" width=8 border=0><asp:label id=Label6 runat="server" Width="130px">Titolo</asp:label></TD>
          <TD align="left">
			<asp:dropdownlist id="dd_titolo" runat="server" AutoPostBack="True" CssClass="testo_grigio" Width="240px"></asp:dropdownlist></TD></TR>
        <TR height=23><TD class=titolo_scheda><IMG height=1 src="../../images/proto/spaziatore.gif" width=8 border=0><asp:Label id=Label4 runat="server" Width="130px">Nome *</asp:Label></TD>
		<TD align="left"><asp:textbox id=p_nome runat="server" Width="315px" CssClass="testo_grigio" MaxLength="50"></asp:textbox></TD></TR>
        <TR height=23>
        <TD class=titolo_scheda><IMG height=1 src="../../images/proto/spaziatore.gif" width=8 border=0><asp:label id=Label13 runat="server" Width="130px">Cognome *</asp:label></TD>
          <TD align="left">
			<asp:textbox id=p_cognome runat="server" Width="315px" CssClass="testo_grigio" MaxLength="50"></asp:textbox></TD></TR>
             <TR height=23>
        <TD class=titolo_scheda><IMG height=1 src="../../images/proto/spaziatore.gif" width=8 border=0><asp:label id=Label5 runat="server" Width="130px">Data di nascita (gg/mm/aaaa)</asp:label></TD>
          <TD align="left">
			<asp:textbox id=p_dataNascita runat="server" Width="315px" CssClass="testo_grigio" MaxLength="50"></asp:textbox></TD></TR>
         <TR height=23>
        <TD class=titolo_scheda><IMG height=1 src="../../images/proto/spaziatore.gif" width=8 border=0><asp:label id=Label3 runat="server" Width="130px">Luogo di nascita</asp:label></TD>
          <TD align="left">
			<asp:textbox id=p_luogoNascita runat="server" Width="315px" CssClass="testo_grigio" MaxLength="128"></asp:textbox></TD></TR>
        <TR height=23>
        <TD class=titolo_scheda><IMG height=1 src="../../images/proto/spaziatore.gif" width=8 border=0><asp:label id=Label14 runat="server" Width="130px">Indirizzo</asp:label></TD>
          <TD align="left">
			<asp:textbox id=p_indirizzo runat="server" Width="315px" CssClass="testo_grigio" MaxLength="128"></asp:textbox></TD></TR>
        <TR height=23>
          <TD class=titolo_scheda><IMG height=1 src="../../images/proto/spaziatore.gif" width=8 border=0><asp:label id=Label15 runat="server" Width="130px">Cap</asp:label></TD>
          <TD align="left">
			<asp:textbox id=p_cap runat="server" Width="315px" CssClass="testo_grigio" MaxLength="5"></asp:textbox></TD></TR>
        <TR height=23>
          <TD class=titolo_scheda><IMG height=1 src="../../images/proto/spaziatore.gif" width=8 border=0><asp:label id=Label16 runat="server" Width="130px">Citta'</asp:label></TD>
          <TD align="left">
			<asp:textbox id=p_citta runat="server" Width="315px" CssClass="testo_grigio" MaxLength="64"></asp:textbox></TD></TR>
        <TR height=23>
          <TD class=titolo_scheda><IMG height=1 src="../../images/proto/spaziatore.gif" width=8 border=0><asp:label id=Label17 runat="server" Width="130px">Provincia</asp:label></TD>
          <TD align="left">
			<asp:textbox id=p_prov runat="server" Width="315px" CssClass="testo_grigio" MaxLength="2"></asp:textbox></TD></TR>
        <TR height=23>
          <TD class=titolo_scheda style="HEIGHT: 22px"><IMG height=1 src="../../images/proto/spaziatore.gif" width=8 border=0><asp:label id=Label18 runat="server" Width="130px">Nazione</asp:label></TD>
          <TD align="left"><asp:textbox id=p_nazione runat="server" Width="315px" CssClass="testo_grigio" MaxLength="32"></asp:textbox></TD></TR>
        <TR height=23>
          <TD class=titolo_scheda style="HEIGHT: 22px"><IMG height=1 src="../../images/proto/spaziatore.gif" width=8 border=0><asp:label id="lbl_local" runat="server" Width="130px">Località</asp:label></TD>
          <TD align="left"><asp:textbox id="p_local" runat="server" Width="315px" CssClass="testo_grigio" MaxLength="32"></asp:textbox></TD></TR>
        <TR height=23>
          <TD class=titolo_scheda><IMG height=1 src="../../images/proto/spaziatore.gif" width=8 border=0><asp:label id=Label19 runat="server" Width="130px">Telefono princ.</asp:label></TD>
          <TD align="left">
			<asp:textbox id=p_telefono1 runat="server" Width="315px" CssClass="testo_grigio" MaxLength="16"></asp:textbox></TD></TR>
        <TR height=23>
          <TD class=titolo_scheda><IMG height=1 src="../../images/proto/spaziatore.gif" width=8 border=0><asp:label id=Label20 runat="server" Width="130px">Telefono sec.</asp:label></TD>
          <TD align="left">
			<asp:textbox id=p_telefono2 runat="server" Width="315px" CssClass="testo_grigio" MaxLength="16"></asp:textbox></TD></TR>
        <TR height=23>
          <TD class=titolo_scheda><IMG height=1 src="../../images/proto/spaziatore.gif" width=8 border=0><asp:label id=Label21 runat="server" Width="130px">Fax</asp:label></TD>
          <TD align="left">
			<asp:textbox id=p_fax runat="server" Width="315px" CssClass="testo_grigio" MaxLength="16"></asp:textbox></TD></TR>
        <TR height=23>
          <TD class=titolo_scheda><IMG height=1 
            src="../../images/proto/spaziatore.gif" width=8 border=0><asp:label id=lbl_codfiscale_p runat="server">C.F.</asp:label></TD>
          <TD align="left">
            <asp:textbox id=p_codfisc runat="server"  Width="315px" CssClass="testo_grigio" MaxLength="16"></asp:textbox></TD></TR>
        <TR height=23>
          <TD class=titolo_scheda><IMG height=1 
            src="../../images/proto/spaziatore.gif" width=8 border=0><asp:label id=lbl_partitaiva_p runat="server">P.IVA</asp:label></TD>
          <TD align="left">
            <asp:textbox id=p_partitaiva runat="server" Width="315px" CssClass="testo_grigio" MaxLength="11"></asp:textbox></TD></TR>
        <TR height=23>
          <TD class=titolo_scheda><IMG height=1 src="../../images/proto/spaziatore.gif" width=8 border=0><asp:label id=Label23 runat="server" Width="130px">Note</asp:label></TD>
          <TD align="left">
			<asp:textbox id=p_note runat="server" Width="315px" CssClass="testo_grigio" MaxLength="250"></asp:textbox></TD></TR></TABLE></TD></TR>
		</asp:panel>
				    <TR height="5%">
					<TD align="left" colSpan="2">
						<table id="tbl_bottoniera" align="center" border="0">
							<tr>
								<td width="40"><asp:button id="btn_Insert" runat="server" CssClass="pulsante_hand" Text="INSERISCI"></asp:button><asp:button id="btn_Insert_disabled" runat="server" CssClass="pulsante_hand" Text="INSERISCI"
										Enabled="False" EnableViewState="False"></asp:button>
								</td>
								<td><asp:button id="btn_chiudi" CssClass="pulsante" Runat="server" Text="CHIUDI"></asp:button></td>
							</tr>
						</table>
					</TD>
				</TR>
			</TABLE>
		</form>
    </div>
	</body>
</HTML>
