<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="docClassifica.aspx.cs"
    Inherits="DocsPAWA.documento.docClassifica" %>

<%@ Register TagPrefix="uc1" TagName="TestataDocumento" Src="TestataDocumento.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register TagPrefix="cc2" Namespace="Utilities" Assembly="MessageBox" %>
<%@ Register TagPrefix="uc2" TagName="AclDocumento" Src="AclDocumento.ascx" %>
<%@ Register Src="../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">

<head id="Head1" runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR" />
    <meta content="C#" name="CODE_LANGUAGE" />
    <meta content="JavaScript" name="vs_defaultClientScript" />
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
    <link href="../CSS/docspa_30.css" type="text/css" rel="stylesheet" />
    <script type="text/javascript" language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
    <script type="text/javascript" language="javascript" id="btn_insInFascicolo_click"
        event="onclick()" for="btn_insInFascicolo">
		window.document.body.style.cursor='wait';
			document.getElementById('btn_insInFascicolo').style.display='none';
			document.getElementById('btn_insInFascicoloDisabled').style.display='';
		
    </script>
    <script type="text/javascript" language="javascript" id="click_proponiClassifica"
        event="onclick()" for="proponiClassifica">
					//window.document.body.style.cursor='wait';
					AttendiClassifica();
    </script>
    <script type="text/javascript" language="javascript" id="btn_fascProcedim_click"
        event="onclick()" for="btn_fascProcedim">
					window.document.body.style.cursor='wait';			
					WndWait();
    </script>
    <script type="text/javascript">
        function OnClickNewFascicolo(codiceClassifica, profilazioneDinamica, idTitolario) {
            top.principale.iFrame_dx.document.location = 'tabDoc.aspx';

            var returnValue = true;

            var retValue = ApriFinestraNewFascNewTit(codiceClassifica, 'docClassifica', 'fascNewFascicolo.aspx', profilazioneDinamica, idTitolario);

            if (retValue != null && retValue) {
                if (document.getElementById("h_codFasc") != null) {
                    document.getElementById("h_codFasc").value = unescape(retValue);
                }
                if (document.getElementById("txt_codFasc") != null) {
                    document.getElementById("txt_codFasc").value = unescape(retValue);
                }
            }
            else {
                returnValue = false;
            }

            return returnValue;
        }
    </script>
    <script type="text/javascript" language="javascript">
			var w = window.screen.width;
		    var h = window.screen.height;
		    var new_w = (w-100)/2;
		    var new_h = (h-400)/2;
		    
			<!--	
			function btn_titolario_onClick(queryString)
			{		
				var retValue=true;
				//if (document.docClassifica.txt_codClass.value=="")
				//{
				//	retValue=confermaCaricamentoTitolario();
				//}

				if (retValue)
				{
					ApriTitolario(queryString,"gestClass");
				}
				
				return retValue;
			}

			function proponiclassifica_onClick()
			{
				ApriClassifica();
			}

            function nascondi()
		    {
			    document.getElementById('btn_insInFascicoloDisabled').style.display='none';
		    }
		    
			function ApriRicercaFascicoli(cod_class, index_titolario) 
			{
				//Federica per poter passare il codice classifica codificato
				//var queryString="CodClass="+ document.docClassifica.txt_codClass.value;
				var queryString="CodClass=" + cod_class;
				if(index_titolario != null && index_titolario != "")
				    queryString += "&IndexTitolario=" + index_titolario;
				var newLeft=(screen.availWidth-615);
				var newTop=(screen.availHeight-654);	
			
				rtnValue=window.showModalDialog("../popup/FiltriRicFasc.aspx?"+queryString,"Fascicoli","dialogWidth:615px;dialogHeight:550px;status:no;resizable:no;scroll:no;dialogLeft:"+newLeft+";dialogTop:"+newTop+";center:no;help:no;"); 
				if(rtnValue)
				{
					top.principale.iFrame_dx.document.location="../waitingPage.htm";
				}
				window.document.docClassifica.submit();
				return true;	
			}
			//-->
	

        function SingleSelect(regex,current) 
        { 
            re = new RegExp(regex);
            for(i = 0; i < document.forms[0].elements.length;i++)
            {
                elm = document.forms[0].elements[i];
                if (elm.type == 'radio' && elm != current && re.test(elm.name))
                {
                    if (elm.disabled)
                        alert('Non è possibile cambiare la fascicolazione primaria su un fascicolo di cui non si possiedono i diritti')
                    else
                        elm.checked = false; 
                } 
            } 
        }

		function confirmDel()
		{
			
            var obbligatorio = document.getElementById("txt_confirmDel").value;			
			if(obbligatorio != "attivo")
			{
			   
			    var agree=confirm("Il documento verrà rimosso dal fascicolo. Continuare?");
			    if (agree)
			    {
				    document.getElementById("txt_confirmDel").value = "si";
				    return true ;
			    }
			}
			else
            {
               
                var agree=alert("Il documento non può essere rimosso dal fascicolo.");
                return true;
            }						
		}
		
		function ApriSceltaTitolario(codClassifica)
		{   
	    	//window.open('../popup/sceltaTitolari.aspx?CodClassifica='+codClassifica,'','top = '+ new_h +' left = '+new_w+' width=500,height=420,scrollbars=YES');
			window.showModalDialog('../popup/sceltaTitolari.aspx?CodClassifica='+codClassifica,'','dialogWidth:500px;dialogHeight:300px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:'+ new_h +';left:'+new_w);  
			docClassifica.submit();
		}	
		
		function ApriSceltaNodo(queryString)
		{   
	    	//window.open('../popup/sceltaNodoTitolario.aspx?'+queryString,'','top = '+ new_h +' left = '+new_w+' width=500,height=420,scrollbars=YES');
			window.showModalDialog('../popup/sceltaNodoTitolario.aspx?'+queryString,'','dialogWidth:500px;dialogHeight:300px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:'+ new_h +';left:'+new_w);
			docClassifica.submit();
		}		
    </script>
</head>
<body>
    <form id="docClassifica" method="post" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="Classifica" />
    <input id="h_codFasc" type="hidden" name="h_codFasc" runat="server" onserverchange="h_codFasc_ServerChange" />
    <input id="txt_confirmDel" type="hidden" name="txt_confirmDel" runat="server" />
    <input id="txt_numero_fascicoli" type="hidden" name="txt_confirmDel" runat="server" />
    <uc2:AclDocumento ID="aclDocumento" runat="server"></uc2:AclDocumento>
    <table id="tbl_contenitore" cellspacing="0" cellpadding="0" width="398px" align="center"
        border="0">
        <tr valign="top">
            <td>
                <uc1:TestataDocumento ID="TestataDocumento" runat="server"></uc1:TestataDocumento>
            </td>
        </tr>
        <tr valign="top">
            <td valign="top" align="left">
                <table class="contenitore" height="100%" id="tbl_contenitore2" cellspacing="0" cellpadding="0"
                    width="100%" border="0">
                    <tr valign="top">
                        <td>
                            <asp:ImageButton ID="enterKeySimulator" runat="server" ImageUrl="../images/spacer.gif"
                                OnClick="btn_insInFascicolo_Click"></asp:ImageButton>
                            <img height="1" src="../images/proto/spaziatore.gif" width="1" border="0">
                        </td>
                    </tr>
                    <tr valign="top">
                        <td class="testo_msg_grigio" align="center" height="19">
                            fascicoli contenenti il documento
                        </td>
                    </tr>
                    <tr valign="top">
                        <td align="center">
                            <div style="overflow: auto; height: 195px">
                                <asp:DataGrid ID="Datagrid2" SkinID="datagrid" runat="server" AllowSorting="True"
                                    BorderStyle="Inset" AutoGenerateColumns="False" CellPadding="1" BorderWidth="1px"
                                    BorderColor="Gray" AllowPaging="True" PageSize="3" Width="100%" OnItemCommand="Datagrid2_ItemCommand"
                                    OnPageIndexChanged="Datagrid2_PageIndexChanged" OnItemCreated="Datagrid2_ItemCreated"
                                    OnItemDataBound="Datagrid2_DataBound">
                                    <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
                                    <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
                                    <ItemStyle HorizontalAlign="Center" Height="20px" CssClass="bg_grigioN" VerticalAlign="Middle">
                                    </ItemStyle>
                                    <HeaderStyle Wrap="False" Height="20px" ForeColor="White" CssClass="menu_1_bianco_dg">
                                    </HeaderStyle>
                                    <Columns>
                                        <asp:TemplateColumn HeaderText="codice">
                                            <HeaderStyle Width="20%"></HeaderStyle>
                                            <ItemTemplate>
                                                <asp:LinkButton ID="LinkButton1" runat="server" CssClass="linkon" Text='<%# DataBinder.Eval(Container, "DataItem.codice") %>'
                                                    CommandArgument='<%# DataBinder.Eval(Container, "DataItem.codice", "{0}") %>'
                                                    ToolTip='<%# DataBinder.Eval(Container, "DataItem.descrizioneFolders") %>' CommandName="viewInfoFascicolo"
                                                    Visible="true">
                                                </asp:LinkButton>
                                                <asp:Label ID="lbl_codice" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.codice") %>'
                                                    Visible="false"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderText="descrizione">
                                            <HeaderStyle Width="55%"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Left"></ItemStyle>
                                            <ItemTemplate>
                                                <asp:Label ID="Label3" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.descrizione") %>'>
                                                </asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="Textbox2" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.descrizione") %>'>
                                                </asp:TextBox>
                                            </EditItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn Visible="False" HeaderText="cod Class">
                                            <ItemTemplate>
                                                <asp:Label ID="Label4" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.vociClassificazione") %>'>
                                                </asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="Textbox3" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.vociClassificazione") %>'>
                                                </asp:TextBox>
                                            </EditItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn Visible="False" HeaderText="chiave">
                                            <ItemTemplate>
                                                <asp:Label ID="Label5" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.chiave") %>'>
                                                </asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="Textbox4" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.chiave") %>'>
                                                </asp:TextBox>
                                            </EditItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderText="Registro">
                                            <HeaderStyle Width="15%"></HeaderStyle>
                                            <ItemTemplate>
                                                <asp:Label ID="lbl_descRegistro" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.descRegistro") %>'>
                                                </asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txt_descRegistro" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.descRegistro") %>'>
                                                </asp:TextBox>
                                            </EditItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderText="PRINC" ItemStyle-Width="5%">
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                            <ItemTemplate>
                                                <asp:RadioButton ID="rbSel" runat="server" AutoPostBack="true" Visible="true" onclick="SingleSelect('rbS',this);"
                                                    OnCheckedChanged="rbSel_CheckedChanged" />
                                            </ItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderText="">
                                            <HeaderStyle Width="10%"></HeaderStyle>
                                            <ItemTemplate>
                                                <asp:LinkButton ID="img_cancella" runat="server" Text="&lt;img src=../images/proto/cancella.gif border=0 onclick=confirmDel(); alt='Rimuove il documento dal fascicolo'&gt;"
                                                    Visible="false" CommandName="update" CommandArgument='<%# DataBinder.Eval(Container, "DataItem.codice", "{0}") %>'>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn Visible="False" HeaderText="stato">
                                            <ItemTemplate>
                                                <asp:Label ID="stato" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.stato") %>'>
                                                </asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.stato") %>'>
                                                </asp:TextBox>
                                            </EditItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn Visible="False" HeaderText="IdRegistro">
                                            <ItemTemplate>
                                                <asp:Label ID="Label1" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.idRegistro") %>'>
                                                </asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="Textbox5" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.idRegistro") %>'>
                                                </asp:TextBox>
                                            </EditItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn Visible="False" HeaderText="AccessRights">
                                            <ItemTemplate>
                                                <asp:Label ID="Label2" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.accessRights") %>'>
                                                </asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="Textbox6" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.accessRights") %>'>
                                                </asp:TextBox>
                                            </EditItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn Visible="False" HeaderText="IdTitolario">
                                            <ItemTemplate>
                                                <asp:Label ID="Label7" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.idTitolario") %>'>
                                                </asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="Textbox7" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.idTitolario") %>'>
                                                </asp:TextBox>
                                            </EditItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderText="systemId" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label ID="lbl_systemId" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.systemId") %>'>
                                                </asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txt_systemId" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.systemId") %>'>
                                                </asp:TextBox>
                                            </EditItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderText="fascPrimaria" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label ID="lbl_fascPrima" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.fascPrimaria") %>'>
                                                </asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txt_fascPrima" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.fascPrimaria") %>'>
                                                </asp:TextBox>
                                            </EditItemTemplate>
                                        </asp:TemplateColumn>
                                    </Columns>
                                    <PagerStyle HorizontalAlign="Center" BackColor="#C2C2C2" CssClass="menu_pager_grigio"
                                        Mode="NumericPages"></PagerStyle>
                                </asp:DataGrid>
                            </div>
                        </td>
                    </tr>
                    <tr valign="top">
                        <td valign="middle" style="padding-bottom: 2px">
                            <table class="info_grigio" cellspacing="0" cellpadding="0" align="center" border="0" width="98%">
                                <tr>
                                    <td>
                                        <table cellspacing="0" cellpadding="0" border="0" width="100%">
                                            <tr>
                                                <td class="titolo_scheda" valign="middle">
                                                    &nbsp;Classifica
                                                </td>
                                                <td align="center">
                                                    <asp:RadioButtonList ID="OptLst" runat="server" CellPadding="0" RepeatDirection="Horizontal"
                                                        AutoPostBack="True" CssClass="testo_grigio_scuro" Width="168px" OnSelectedIndexChanged="OptLst_SelectedIndexChanged">
                                                        <asp:ListItem Value="Cod" Selected="True">Codice</asp:ListItem>
                                                        <asp:ListItem Value="Liv">Livello</asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </td>
                                                <td valign="middle" align="center">
                                                    <asp:ImageButton ID="buttonGiornaleRiscontri" runat="server" Visible="false" OnClick="buttonGiornaleRiscontri_Click">
                                                    </asp:ImageButton>
                                                    <cc1:ImageButton ID="Imagebutton1" runat="server" ImageUrl="../images/spacer.gif">
                                                    </cc1:ImageButton>
                                                    <asp:ImageButton ID="proponiClassifica" runat="server" ImageUrl="../images/proto/aggiungi.gif"
                                                        Visible="False"></asp:ImageButton>
                                                </td>
                                                <td valign="middle" align="center">
                                                    <cc1:ImageButton ID="btn_titolario" runat="server" ImageUrl="../images/proto/ico_titolario_noattivo.gif"
                                                        AlternateText="Titolario" ToolTip="Titolario" Tipologia="DO_CLA_TITOLARIO" DisabledUrl="../images/proto/ico_titolario_noattivo.gif"
                                                        OnClick="btn_titolario_Click"></cc1:ImageButton>
                                                </td>
                                                <td valign="middle" align="center">
                                                    <cc1:ImageButton ID="btn_fascInAreaDiLav" runat="server" ImageUrl="../images/proto/area_new.gif"
                                                        AlternateText="Visualizza fascicoli in area di lavoro" ToolTip="Visualizza fascicoli in area di lavoro"
                                                        Tipologia="DO_CLA_GET_ADL" DisabledUrl="../images/proto/area_new.gif" OnClick="btn_fascInAreaDiLav_Click">
                                                    </cc1:ImageButton>
                                                </td>
                                                <td valign="middle" align="center">
                                                    <cc1:ImageButton ID="btn_fascProcedim" runat="server" ImageUrl="../images/proto/zoom.gif"
                                                        AlternateText="Visualizza tutti i fascicoli con la classifica selezionata" ToolTip="Visualizza tutti i fascicoli con la classifica selezionata"
                                                        Tipologia="DO_CLA_VIS_PROC" DisabledUrl="../images/proto/zoom.gif" OnClick="btn_fascProcedim_Click">
                                                    </cc1:ImageButton>
                                                </td>
                                                <td valign="middle" align="center">
                                                    <cc1:ImageButton class="ImgHand" ID="imgFasc" runat="server" ImageUrl="../images/proto/ico_fascicolo_noattivo.gif"
                                                        AlternateText="Ricerca Fascicoli" ToolTip="Ricerca Fascicoli" Tipologia="DO_CLA_VIS_PROC"
                                                        DisabledUrl="../images/proto/ico_fascicolo_noattivo.gif" OnClick="imgFasc_Click">
                                                    </cc1:ImageButton>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="5" style="height: 25px;">
                                        <table cellspacing="0" cellpadding="0" border="0" style="width: 100%;">
                                            <tr>
                                                <td>
                                                    <asp:Panel ID="pnl_registri" runat="server" Visible="false">
                                                        <asp:Label ID="lbl_registro" runat="server" Width="60px" CssClass="testo_grigio"
                                                            Style="padding-left: 3px">Registro</asp:Label>
                                                        <asp:DropDownList ID="ddl_registri" runat="server" Width="80%" CssClass="testo_grigio"
                                                            AutoPostBack="True" OnSelectedIndexChanged="ddl_registri_SelectedIndexChanged">
                                                        </asp:DropDownList>
                                                    </asp:Panel>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="4" style="height: 25px;">
                                        <table cellspacing="0" cellpadding="0" border="0" style="width: 100%;">
                                            <tr valign="top">
                                                <td>
                                                    <asp:Label ID="lbl_codClass" runat="server" CssClass="testo_grigio" Width="60px"
                                                        Style="padding-left: 3px">Classifica</asp:Label>
                                                    <asp:TextBox ID="txt_codClass" runat="server" AutoPostBack="True" CssClass="testo_grigio"
                                                        Width="103px" OnTextChanged="txt_codClass_TextChanged"></asp:TextBox>
                                                </td>
                                                <td align="right" style="padding-right: 16px;">
                                                    <asp:Label ID="lbl_codFasc" runat="server" CssClass="testo_grigio" Width="60px">Fascicolo</asp:Label>
                                                    <asp:TextBox ID="txt_codFasc" runat="server" AutoComplete="off" CssClass="testo_grigio"
                                                        Width="103px" OnTextChanged="txt_codFasc_TextChanged"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="4" style="height: 25px;">
                                        <table cellspacing="0" cellpadding="0" border="0" style="width: 390px;">
                                            <tr>
                                                <td>
                                                    <asp:Panel ID="pnl_indiceSis" runat="server" Visible="false">
                                                        <asp:Label ID="lbl_indiceSis" runat="server" Width="60px" CssClass="testo_grigio"
                                                            Style="padding-left: 3px">Ind. Sis.</asp:Label>
                                                        <asp:TextBox ID="txt_indiceSis" runat="server" CssClass="testo_grigio" Width="103px"></asp:TextBox>
                                                        <cc1:ImageButton ID="img_indiceSis" runat="server" ImageUrl="../images/rubrica/b_arrow_right.gif"
                                                            OnClick="img_indiceSis_Click" />
                                                    </asp:Panel>
                                                </td>
                                                <asp:Panel ID="pnl_PrototolloTitolario" runat="server" Visible="false">
                                                    <td align="right" style="padding-right: 16px;">
                                                        <asp:Label ID="lbl_protoTitolario" runat="server" CssClass="testo_grigio" Width="60px"></asp:Label>
                                                        <asp:TextBox ID="txt_protoPratica" runat="server" Width="103px" MaxLength="5" CssClass="testo_grigio"
                                                            AutoPostBack="True" OnTextChanged="txt_protoPratica_TextChanged"></asp:TextBox>
                                                    </td>
                                                </asp:Panel>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="4" style="height: 25px;">
                                        <asp:Label ID="lbl_Titolari" runat="server" CssClass="testo_grigio" Width="21%" Style="padding-left: 3px">Titolario</asp:Label><asp:DropDownList
                                            ID="ddl_titolari" runat="server" AutoPostBack="True" CssClass="testo_grigio"
                                            Width="76%" EnableViewState="True" OnSelectedIndexChanged="ddl_titolari_SelectedIndexChanged">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="4">
                                        <asp:Label ID="lbl_livello1" runat="server" CssClass="testo_grigio" Width="21%" Style="padding-left: 3px">Livello1</asp:Label><asp:DropDownList
                                            ID="ddl_livello1" runat="server" AutoPostBack="True" CssClass="testo_grigio"
                                            Width="76%" EnableViewState="True" OnSelectedIndexChanged="ddl_livelloControl_SelectedIndexChanged">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="4">
                                        <asp:Label ID="lbl_livello2" runat="server" CssClass="testo_grigio" Width="21%" Style="padding-left: 3px">Livello2</asp:Label><asp:DropDownList
                                            ID="ddl_livello2" runat="server" AutoPostBack="True" CssClass="testo_grigio"
                                            Width="76%" EnableViewState="True" OnSelectedIndexChanged="ddl_livelloControl_SelectedIndexChanged">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="4">
                                        <asp:Label ID="lbl_livello3" runat="server" CssClass="testo_grigio" Width="21%" Style="padding-left: 3px">Livello3</asp:Label><asp:DropDownList
                                            ID="ddl_livello3" runat="server" AutoPostBack="True" CssClass="testo_grigio"
                                            Width="76%" EnableViewState="True" OnSelectedIndexChanged="ddl_livelloControl_SelectedIndexChanged">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="4">
                                        <asp:Label ID="lbl_livello4" runat="server" CssClass="testo_grigio" Width="21%" Style="padding-left: 3px">Livello4</asp:Label><asp:DropDownList
                                            ID="ddl_livello4" runat="server" AutoPostBack="True" CssClass="testo_grigio"
                                            Width="76%" EnableViewState="True" OnSelectedIndexChanged="ddl_livelloControl_SelectedIndexChanged">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="4">
                                        <asp:Label ID="lbl_livello5" runat="server" CssClass="testo_grigio" Width="21%" Style="padding-left: 3px">Livello5</asp:Label><asp:DropDownList
                                            ID="ddl_livello5" runat="server" AutoPostBack="True" CssClass="testo_grigio"
                                            Width="76%" EnableViewState="True" OnSelectedIndexChanged="ddl_livelloControl_SelectedIndexChanged">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="4">
                                        <asp:Label ID="lbl_livello6" runat="server" CssClass="testo_grigio" Width="21%" Style="padding-left: 3px">Livello6</asp:Label><asp:DropDownList
                                            ID="ddl_livello6" runat="server" AutoPostBack="True" CssClass="testo_grigio"
                                            Width="76%" EnableViewState="True" OnSelectedIndexChanged="ddl_livelloControl_SelectedIndexChanged">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <cc2:MessageBox ID="msg_inserisciDoc" runat="server"></cc2:MessageBox>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <!-- bottoniera-->
                <table id="tbl_bottoniera" cellspacing="0" cellpadding="5" align="center" border="0">
                    <tr>
                        <td>
                            <cc1:ImageButton ID="btn_aggAreaDiLavoro" runat="server" Thema="btn_" SkinID="area_attivo"
                                AlternateText="Aggiungi ad area di lavoro" Tipologia="DO_ADD_ADL" DisabledUrl="../images/bottoniera/btn_area_nonattivo.gif"
                                OnClick="btn_aggAreaDiLavoro_Click"></cc1:ImageButton>
                        </td>
                        <td>
                            <cc1:ImageButton ID="btn_insInFascicolo" runat="server" Thema="btn_" SkinID="classifica_attivo"
                                AlternateText="Inserisci documento in classifica" Tipologia="DO_CLA_INSERISCI"
                                DisabledUrl="../images/bottoniera/btn_classifica_nonAttivo.gif" OnClick="btn_insInFascicolo_Click">
                            </cc1:ImageButton><cc1:ImageButton ID="btn_insInFascicoloDisabled" ondblclick="return false;"
                                runat="server" AlternateText="Protocolla" DisabledUrl="../images/bottoniera/btn_classifica_nonAttivo.gif"
                                ImageUrl="../images/bottoniera/btn_classifica_nonAttivo.gif" Enabled="False"
                                ImageAlign="Top"></cc1:ImageButton>
                        </td>
                        <td>
                            <cc1:ImageButton ID="btn_new" AutoPostBack="False" runat="server" Thema="btn_" SkinID="nuovo_Attivo"
                                AlternateText="Nuovo fascicolo" DisabledUrl="../images/bottoniera/btn_nuovo_nonattivo.gif"
                                Enabled="False" OnClick="btn_new_Click"></cc1:ImageButton>
                        </td>
                        <td>
                            <asp:Button ID="btn_new_tit" runat="server" Enabled="False" CssClass="pulsante92"
                                Text="Nodo Tit" ToolTip="Nuovo nodo di titolario" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <cc2:MessageBox ID="msgEliminaUltimoDoc" runat="server" />
    </form>
</body>
</html>

