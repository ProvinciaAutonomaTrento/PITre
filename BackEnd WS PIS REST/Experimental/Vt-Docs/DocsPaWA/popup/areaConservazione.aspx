<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="areaConservazione.aspx.cs"
    Inherits="DocsPAWA.popup.areaConservazione" %>

<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register Src="../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc5" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../CSS/rubrica.css" type="text/css" rel="stylesheet">
    <link href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
    <link href="../CSS/StyleSheet.css" type="text/css" rel="Stylesheet" />
    <base target="_self" />
    <style type="text/css">
        #TextArea1
        {
            width: 316px;
            height: 56px;
        }
        .style3
        {
            font-weight: bold;
            font-size: 10px;
            color: #666666;
            font-family: Verdana;
            width: 410px;
        }
        .style4
        {
            font-weight: bold;
            font-size: 10px;
            color: #666666;
            font-family: Verdana;
            width: 397px;
        }
        .style5
        {
            height: 44px;
        }
        .modalBackground
        {
            background-color: #f0f0f0;
            filter: alpha(opacity=50);
            opacity: 0.5;
        }
        .modalPopup
        {
            background-color: #ffffdd;
            border-width: 3px;
            border-style: solid;
            border-color: Gray;
            padding: 3px;
            text-align: center;
        }
        .tab_policy
        {
            border-collapse: collapse;
            margin: 0px;
            width: 100%;
        }
        .tab_policy td
        {
            padding: 2px;
            margin: 0px;
        }
        .tab_policy_sx
        {
            width: 180px;
            vertical-align: middle;
        }
        .tab_policy_dx
        {
            width: 270px;
        }
        .tab_policy_dx1 img
        {
            border: 0px;
            float: left;
            width: 17px;
            height: 17px;
        }
        .tab_policy_dx1
        {
            width: 150px;
        }
        
        .tab_policy_cx
        {
            width: 150px;
            vertical-align: middle;
        }
        .non_valido
        {
            text-decoration: line-through;
            color: #990000;
        }
    </style>
    <script type="text/javascript" language="javascript">

        function beginRequest(sender, args) {
            $find("mdlWait").show();
        }

        function endRequest(sender, args) {
            $find("mdlWait").hide();
        }

    </script>
    <script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
    <script language="javascript" type="text/javascript">
        function showModalDialogEliminaIstanza() {
            alert('L\'istanza non può essere eliminata perché è già stata inviata per la conservazione');
            return false;
        }
        function showModalDialogEliminaDoc() {
            alert('Il documento non può essere eliminato perché è già stato inviato per la conservazione');
            return false;
        }

        function controlla() {
            if (document.forms[0].txt_descrizione.value == "") {
                alert('Inserire campo descrizione');
                return false;
            }
        }

        function apriModalDialogEliminaIstanza() {
            var val = window.confirm('Si vogliono eliminare tutti i documenti di questa conservazione?');
            if (val) {
                return true;
            }
            else {
                return false;
            }
        }
        function closeWindow() {
            self.close();
        }

        var abilita = 'false';

        function OpenHelp(from) {
            var pageHeight = 600;
            var pageWidth = 800;
            var posTop = (screen.availHeight - pageHeight) / 2;
            var posLeft = (screen.availWidth - pageWidth) / 2;

            var newwin = window.showModalDialog('../Help/Manuale.aspx?from=' + from,
							    '',
							    'dialogWidth:' + pageWidth + 'px;dialogHeight:' + pageHeight + 'px;status:no;resizable:no;scroll:yes;dialogLeft:' + posLeft + ';dialogTop:' + posTop + ';center:yes;help:no');
        }

        function showAreaConservazioneValidationMask(idCons, args) {
            var pageHeight = 500;
            var pageWidth = 850;

            var retValue = window.showModalDialog('areaConservazioneValidation.aspx?idCons=' + idCons + '&sessionParam=' + args,
            null, 'dialogWidth:' + pageWidth + 'px;dialogHeight:' + pageHeight + 'px;status:no;resizable:yes;scroll:yes;center:yes;help:no;close:no')


            if (retValue) {
                // Invio comunque istanza
                var hdForceSend = document.getElementById('<%=this.hdForceSend.ClientID%>');
                hdForceSend.value = 'true';

                form1.submit();
            }
        }

        function SelezionaIstanza(idIstanza) {
            if (idIstanza != null) {
                var hdIstanzaPreferred = document.getElementById('<%=this.hdIstanzaPreferred.ClientID%>');
                hdIstanzaPreferred.value = hdIstanzaPreferred;
            }
            else {
                var hdIstanzaPreferred = document.getElementById('<%=this.hdIstanzaPreferred.ClientID%>');
                hdIstanzaPreferred.value = null;
            }
        }

    </script>
</head>
<body style="overflow-y: scroll; overflow-x: hidden;">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server" AsyncPostBackTimeout="3600">
    </asp:ScriptManager>
    <script language="javascript" type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(beginRequest);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequest);
    </script>
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="Istanze di Conservazione" />
    <asp:HiddenField ID="hdForceSend" runat="server" />
    <asp:HiddenField ID="hdIstanzaPreferred" runat="server" />
    <asp:UpdatePanel ID="upGridPersonalization" runat="server">
        <contenttemplate>
    <div align="center">
        <table style="width: 96%;">
            <tr>
                <td class="pulsanti">
                    <asp:Label runat="server" CssClass="testo_grigio_scuro" ID="lbl_intestazione">Lista Istanze di Conservazione</asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Panel ID="panel1" runat="server" BorderWidth="1px" BorderColor="#810D06" BorderStyle="Solid"
                        BackColor="#f2f2f2">
                        <table id="tbl_filtro" runat="server" width="100%">
                            <tr>
                                <td align="left" valign="top" width="45%">
                                    <asp:CheckBox ID="cb_istanzeChiuse" runat="server" Text="mostra istanze chiuse" CssClass="testo_grigio_scuro" />
                                    <asp:CheckBox ID="cb_istanzeAutomatiche" runat="server" Text="mostra istanze automatiche" CssClass="testo_grigio_scuro" />
                                    <asp:CheckBox ID="cb_istanzeManuali" runat="server" Text="mostra istanze manuali" CssClass="testo_grigio_scuro" />
                                </td>
                                <td align="left" valign="top" width="20%">
                                    <asp:ImageButton ID="btn_filtra" runat="server" SkinID="btnCerca" ToolTip="Cerca"
                                        OnClick="btn_filtra_Click" />
                                </td>
                                <td align="right" valign="top">
                                    <asp:ImageButton ID="help" runat="server" OnClientClick="OpenHelp('GestioneAreaConserv')"
                                        AlternateText="Aiuto?" SkinID="btnHelp" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td>
                 <asp:UpdatePanel ID="upIstanze" runat="server" UpdateMode="Conditional">
                                            <contenttemplate>
                    <asp:Panel runat="server" ID="panelIstanzeCons">
                        <asp:GridView ID="gv_istanzeCons" runat="server" SkinID="gridview" Width="100%" BorderWidth="1px"
                            BorderColor="Gray" AllowPaging="True" PageSize="10" AutoGenerateColumns="False"
                            OnSelectedIndexChanged="gv_istanzeCons_SelectedIndexChanged" OnPageIndexChanging="gv_istanzeCons_PageIndexChanging"
                            OnRowDeleting="gv_istanzeCons_RowDeleting" OnPreRender="gv_istanzeCons_PreRender">
                            <HeaderStyle CssClass="menu_1_bianco_dg" />
                            <Columns>                              
                                <asp:TemplateField HeaderText="Id Istanza" ItemStyle-Width="10px">
                                    <ItemTemplate>
                                        <asp:Label ID="lbl_idIstanzaVis" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.idIstanza") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="stato" ItemStyle-Width="65px">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("stato") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lb_stato" runat="server" Text='<%# Bind("stato") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="supporto" ItemStyle-Width="60px" ItemStyle-HorizontalAlign="Center" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="supporto" runat="server" Text='<%# Bind("supporto") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField> 
                                <asp:TemplateField HeaderText="note" ItemStyle-Width="140px">
                                    <ItemTemplate>
                                        <asp:Label ID="note" runat="server" Text='<%# Bind("note") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField> 
                                 <asp:TemplateField HeaderText="descrizione" ItemStyle-Width="100px">
                                    <ItemTemplate>
                                        <asp:Label ID="descrizione" runat="server" Text='<%# Bind("descrizione") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>           
                                <asp:BoundField DataField="data_apertura" HeaderText="DATA APERTURA" ItemStyle-Width="10px" />
                                <asp:BoundField DataField="data_invio" HeaderText="DATA INVIO" ItemStyle-Width="10px" />
                                <asp:BoundField DataField="data_conservazione" HeaderText="DATA CONSERVAZIONE" ItemStyle-HorizontalAlign="Center"
                                    ItemStyle-Width="100px" />
                                 <asp:TemplateField HeaderText="Tipo Cons." ItemStyle-Width="10px">
                                    <ItemTemplate>
                                        <asp:Label ID="tipo_cons" runat="server" Text='<%# Bind("tipo_cons") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>      
                                <asp:BoundField DataField="note_rifiuto" HeaderText="Note Rifiuto" ItemStyle-Width="130px" />
                                <asp:BoundField DataField="automatica" HeaderText="A/M" ItemStyle-Width="35px" ItemStyle-HorizontalAlign="center" />
                                <asp:TemplateField HeaderText="dett." ShowHeader="False" ItemStyle-Width="35px" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ImageButton1" runat="server" CommandName="Select" ImageUrl="~/images/proto/dettaglio.gif"
                                            CausesValidation="false" ToolTip="Dettaglio" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="elimina" ShowHeader="False" ItemStyle-Width="50px"
                                    ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ImageButton2" runat="server" CausesValidation="False" CommandName="Delete"
                                            Text="Delete" ImageUrl="../images/proto/cancella.gif" OnClientClick="return apriModalDialogEliminaIstanza();"
                                            ToolTip="Elimina istanza di conservazione" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="consolida" ItemStyle-Width="100px" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="consolida" runat="server" Text='<%# Bind("consolida") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField> 
                                   <asp:TemplateField HeaderText="idpolicyvalidazione" ItemStyle-Width="100px" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="idpolicyvalidazione" runat="server" Text='<%# Bind("idpolicyvalidazione") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>      
                                  <asp:TemplateField HeaderText="lbl_preferita" ItemStyle-Width="100px" Visible="false">
                                    <ItemTemplate>
                                       <asp:Label ID="lbl_preferita" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.predefinita") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>    
                                 <asp:TemplateField HeaderText="Id Istanza" visible="false">
                                    <EditItemTemplate>
                                        <asp:Label ID="Label1" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.idIstanza") %>'></asp:Label>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lbl_idIstanza" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.idIstanza") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Tipo Cons." visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="tipo_cons2" runat="server" Text='<%# Bind("tipo_cons") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>  
                            </Columns>
                            <PagerStyle HorizontalAlign="Center" BackColor="#C2C2C2" CssClass="menu_pager_grigio" />
                            <AlternatingRowStyle CssClass="bg_grigioA" />
                            <RowStyle CssClass="bg_grigioN" />
                            <SelectedRowStyle CssClass="bg_grigioS" />
                        </asp:GridView>
                    </asp:Panel>
                     </contenttemplate>
        </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td align="center" valign="top">
                    <asp:Panel runat="server" ID="panelDettaglioIstanza" Visible="false" Width="100%">
                        <table id="tbl_dettaglio" runat="server" width="100%">
                            <tr>
                                <td class="testo_grigio_scuro" align="left">
                                    <asp:Panel ID="panel_parametri" runat="server" BorderWidth="1px" BorderColor="#810D06"
                                        BorderStyle="Solid" BackColor="#f2f2f2">
                                        <table id="tbl_supporto" runat="server" width="100%">
                                            <tr>
                                                <td valign="bottom" class="tab_policy_sx">
                                                    <asp:Label ID="lbl_descrizione" runat="server" Text="Inserisci descrizione *" Width="142px"></asp:Label>
                                                </td>
                                                
                                                <td class="tab_policy_dx">
                                                    <asp:TextBox ID="txt_descrizione" runat="server" Width="249px" CssClass="testo_grigio"
                                                        Height="18px" MaxLength="250"></asp:TextBox>
                                                </td>
                                                <td  valign="bottom" class="tab_policy_cx">
                                                    <asp:Label ID="Label3" runat="server" Text="Inserisci Note" Width="99px"></asp:Label>
                                                    </td>
                                                    <td>
                                                    <asp:TextBox ID="txt_note" runat="server" CssClass="testo_grigio" Width="249px" Height="18px"
                                                        MaxLength="500"></asp:TextBox>
                                                </td>
                                                <td align="right" valign="bottom">
                                                    <asp:Label ID="lb_total_size" runat="server" Text="Total size:" CssClass="testo_grigio_scuro"></asp:Label>
                                                    <asp:Label ID="lbl_size" runat="server" CssClass="testo_grigio_scuro"></asp:Label>
                                                    <asp:Label ID="lbl_byte" runat="server" Text="MegaByte" CssClass="testo_grigio_scuro"></asp:Label>
                                                </td>
                                            </tr>
                                            </table>
                                         <asp:UpdatePanel ID="upTipologie" runat="server" UpdateMode="Conditional">
                                            <contenttemplate>
                                                <table class="tab_policy">
                                                <tr>
                                                    <td class="tab_policy_sx" align="left" valign="bottom">
                                                        Tipologia Conservazione*
                                                    </td>
                                                    <td class="tab_policy_dx1">
                                                        <asp:DropDownList ID="ddl_tipoCons" runat="server" DataValueField="Codice" DataTextField="Descrizione" CssClass="testo_grigio" Width="256" OnSelectedIndexChanged="SelectTipoConservazione" AutoPostBack="true">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td align="left" style="padding-left:12px;">
                                                        <asp:CheckBox  Text="Consolida i documenti" runat="server" ID="chk_consolida"/>
                                                    </td> 
                                                     <td align="right" valign="bottom">
                                                    <asp:Label ID="lbl_numdocs_intro" runat="server" Text="Numero Documenti:" CssClass="testo_grigio_scuro"></asp:Label>
                                                    <asp:Label ID="lbl_numdocs" runat="server" CssClass="testo_grigio_scuro"></asp:Label>
                                                    </td>                                               
                                                </tr>               
                                            </table>
                                            </contenttemplate>
                                        </asp:UpdatePanel>
                                        <table class="tab_policy">
                                        <asp:UpdatePanel ID="upValidate" runat="server" UpdateMode="Conditional">
                                            <contenttemplate>
                                        <tr>
                                            <td class="tab_policy_sx">Valida l'istanza con una Policy</td>
                                            <td align="left">
                                                <asp:DropDownList ID="ddl_policy" runat="server" CssClass="testo_grigio" AutoPostBack="true" OnSelectedIndexChanged="ChangePolicy">
                                                </asp:DropDownList>
                                            </td>                                          
                                        </tr>
                                        </table>
                                        </contenttemplate>
    </asp:UpdatePanel>
    </asp:Panel> </td> </tr>
    <tr>
        <td>
            <asp:UpdatePanel ID="upDettaglio" runat="server" UpdateMode="Conditional">
                <contenttemplate>
            <asp:GridView ID="gv_dettaglioCons" SkinID="gridview" runat="server" AllowPaging="True"
                AutoGenerateColumns="False" OnDataBound="gv_dettaglioCons_DataBound" OnPageIndexChanging="gv_dettaglioCons_PageIndexChanging"
                OnPreRender="gv_dettaglioCons_PreRender" OnSelectedIndexChanged="gv_dettaglioCons_SelectedIndexChanged"
                Width="100%" OnRowCommand="gv_dettaglioCons_RowCommand" PageSize="7" EmptyDataText="L'istanza non contiene alcun documento"
                EmptyDataRowStyle-CssClass="testo_grigio">
                <HeaderStyle CssClass="menu_1_bianco_dg" />
                <Columns>
                    <asp:TemplateField HeaderText="ID DOCUMENTO" Visible="False">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox5" runat="server" Text='<%# Bind("docNumber") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lbl_docNumber" runat="server" Text='<%# Bind("docNumber") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Id profile" Visible="False">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox1" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.idProfile") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lbl_idProfile" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.idProfile") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="tipo_doc" HeaderText="TIPO DOC." />
                    <asp:BoundField DataField="oggetto" HeaderText="OGGETTO" />
                    <asp:BoundField DataField="codice_fasc" HeaderText="CODICE FASC." />
                    <asp:BoundField DataField="data_ins" HeaderText="DATA INSERIMENTO" />
                    <asp:TemplateField HeaderText="ID/SEGNATURA DATA">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox3" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.data_prot_or_crea") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lbl_data_prot_or_crea" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.data_prot_or_crea") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="size" HeaderText="KB" />
                    <asp:TemplateField HeaderText="Num Prot." Visible="False">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox2" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.num_prot") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lbl_numProt" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.num_prot") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="tipoFile" HeaderText="Tipo file" />
                    <asp:BoundField DataField="numAllegati" HeaderText="Numero Allegati" />
                    <asp:TemplateField HeaderText="VIS." ShowHeader="false">
                        <ItemTemplate>
                            <asp:ImageButton ID="img_vis" runat="server" CausesValidation="false" CommandName="VisualizzaDoc"
                                ImageUrl="~/images/proto/dett_lente_doc.gif" ToolTip="Visualizza immagine documento"
                                CommandArgument="<%# Container.DataItemIndex %>" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="elimina" ShowHeader="False">
                        <ItemTemplate>
                            <asp:ImageButton ID="img_elimina" runat="server" CausesValidation="False" CommandName="Select"
                                ImageUrl="../images/proto/cancella.gif" Text="Select" ToolTip="Elimina da area di conservazione" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="stato" Visible="False">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("stato") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lb_stato" runat="server" Text='<%# Bind("stato") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Id project" Visible="False">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox1" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.IdProject") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lbl_idProject" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.IdProject") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Immagine acquisita" Visible="False">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox6" runat="server" Text='<%# Bind("cha_img") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lbl_cha_img" runat="server" Text='<%# Bind("cha_img") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="System Id" Visible="False">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox1" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.systemId") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lbl_systemId" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.systemId") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Diritti documento" Visible="False">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox8" runat="server" Text='<%# Bind("diritti") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lbl_diritti" runat="server" Text='<%# Bind("diritti") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                     <asp:TemplateField HeaderText="policyValida" Visible="False">
                        <EditItemTemplate>
                            <asp:TextBox ID="txt_policyValida" runat="server" Text='<%# Bind("policyValida") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lbl_policyValida" runat="server" Text='<%# Bind("policyValida") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <AlternatingRowStyle CssClass="bg_grigioA" />
                <PagerStyle HorizontalAlign="Center" BackColor="#C2C2C2" CssClass="menu_pager_grigio" />
                <RowStyle CssClass="bg_grigioN" />
                <SelectedRowStyle CssClass="bg_grigioS" />
            </asp:GridView>
            </contenttemplate>
            </asp:UpdatePanel>
        </td>
    </tr>
    <tr>
        <td align="center" class="style5" valign="bottom">
            <asp:Button ID="btn_sottoscrivi" runat="server" Text="Invia per conservazione" CssClass="pulsante"
                OnClick="Button1_Click" OnClientClick="return controlla();" />
            &nbsp;&nbsp;
            <asp:Button ID="btn_riabilitaIstanza" runat="server" Text="Riabilita istanza rifiutata"
                CssClass="pulsante" OnClick="btn_riabilitaIstanza_Click" OnClientClick="abilita='true';" />
            &nbsp;&nbsp;
            <asp:Button ID="btn_eliminaTutti" runat="server" CssClass="pulsante" Text="Elimina Tutti"
                OnClick="btn_eliminaTutti_Click" />
            &nbsp;&nbsp;
            <asp:Button ID="btn_predefinita" runat="server" CssClass="pulsante" Text="Seleziona come predefinita"
                OnClick="btn_predefinita_Click" />
            &nbsp;&nbsp;
            <asp:Button ID="btn_elimina_policy" runat="server" CssClass="pulsante" Text="Elimina non conformi alla Policy"
                OnClick="btn_elimina_policy_Click" ToolTip="Elimina tutti i documenti non conformi alla Policy" />
        </td>
    </tr>
    </table> </asp:Panel>
    <tr>
        <td align="center" class="style5">
            <%--<asp:Button ID="btn_esci" runat="server" Text="ESCI" CssClass="pulsante" 
                onclientclick="closeWindow();" />--%>
            <asp:ImageButton ID="btn_esci" runat="server" SkinID="btnAnnulla" ToolTip="Esci"
                OnClientClick="closeWindow();" />
        </td>
    </tr>
    <tr>
        <td>
            <asp:HiddenField ID="idIstanza" runat="server" />
            <asp:HiddenField ID="idPeo" runat="server" />
            <asp:HiddenField ID="idRuoloUo" runat="server" />
            <asp:HiddenField ID="statoIstanza" runat="server" />
            <asp:HiddenField ID="isOpen" Value="0" runat="server" />
        </td>
    </tr>
    </table> </contenttemplate> </asp:UpdatePanel> </div>
    <!-- PopUp Wait-->
    <uc5:ModalPopupExtender ID="mdlPopupWait" runat="server" TargetControlID="Wait" PopupControlID="Wait"
        BackgroundCssClass="modalBackground" BehaviorID="mdlWait" />
    <div id="Wait" runat="server" style="display: none; font-weight: normal; font-size: 13px;
        font-family: Arial; text-align: center;">
        <asp:UpdatePanel ID="pnlUP" runat="server">
            <contenttemplate>
                <div class="modalPopup">
                    <asp:Label ID="lblInfo" runat="server">Attendere prego...</asp:Label>
                    <br />
                    <img id="imgLoading" src="../images/loading.gif" style="border-width: 0px;" alt="Attendere prego" />
                </div>
            </contenttemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
