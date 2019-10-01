<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="GestioneModelliTrasm.aspx.cs"
    Inherits="NttDataWA.Management.GestioneModelliTrasm" MasterPageFile="~/MasterPages/Base.Master" %>

<%@ Register TagPrefix="cc2" Src="~/UserControls/PannelloRicercaModelliTrasmissione.ascx"
    TagName="PannelloRicercaModelliTrasmissione" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .corr_grey
        {
            color: #ccc;
        }
        #divDetails
        {
            text-align: left;
            margin: 5px 0 0 0;
            padding: 5px 0 0 0;
            font-size: 0.9em;
            border-top: 1px solid #ccc;
        }
        #divDetails ul
        {
            list-style-type: disc;
            margin: 0;
            padding: 0;
        }
        #divDetails ul li
        {
            margin: 0 0 0 15px;
        }
        .tbl_rounded td
        {
            vertical-align: top;
        }
        .tbl_rounded tr.nopointer td
        {
            cursor: default;
        }
        .tbl_rounded tr
        {
            cursor: pointer;
        }
        .tbl_rounded th
        {
            white-space: nowrap;
        }
        
        .recordNavigator, .recordNavigator table, .recordNavigator tr, .recordNavigator td
        {
            background-color: #EEEEEE;
        }
        .recordNavigator td
        {
            border: 0;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <uc:ajaxpopup2 Id="AddressBook" runat="server" Url="../popup/AddressBook.aspx" PermitClose="false"
        PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui)  {$('#btnAddressBookPostback').click();}" />
    <uc:ajaxpopup2 Id="GestioneNotifiche" runat="server" Url="../Management/GestioneModelliTrasm_Notifiche.aspx"
        PermitClose="false" PermitScroll="false" IsFullScreen="false" Width="745" Height="450"
        CloseFunction="function (event, ui)  {__doPostBack('UpPnlButtons','');}" />
    <uc:ajaxpopup2 Id="FindAndReplace" runat="server" Url="../popup/FindAndReplaceModelliTrasmissione.aspx"
        PermitClose="false" PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui)  {__doPostBack('UpPnlButtons','');}" />
    <uc:ajaxpopup2 Id="ReportGenerator" runat="server" Url="../popup/ReportGenerator.aspx"
        PermitClose="false" PermitScroll="false" IsFullScreen="false" Width="745" Height="400"
        CloseFunction="function (event, ui)  {__doPostBack('UpPnlButtons','');}" />
    <div id="containerTop">
        <asp:UpdatePanel runat="server" ID="UpHeaderProject" UpdateMode="Conditional">
            <ContentTemplate>
                <div id="containerDocumentTop">
                    <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div id="containerStandardTop">
                                <div id="containerStandardTopSx">
                                </div>
                                <div id="containerStandardTopCx">
                                    <p>
                                        <asp:Label ID="GestioneModelliTrasmissione" runat="server" />
                                    </p>
                                </div>
                                <div id="containerStandardTopDx">
                                </div>
                            </div>
                            <div id="containerStandardBottom">
                                <div id="containerProjectCxBottom">
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <asp:UpdatePanel ID="UpContainer" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <div id="containerStandard">
                <div id="content">
                    <div id="contentStandard1Column">
                        <table cellspacing="0" cellpadding="0" width="90%" align="center" border="0">
                            <tr>
                                <td>
                                    <table cellspacing="0" cellpadding="0" width="100%">
                                        <!-- Nuovo modello -->
                                        <tr>
                                            <td class="pulsanti" align="center" height="20">
                                                <asp:Label ID="lbl_titolo" runat="server" CssClass="weight"></asp:Label>
                                            </td>
                                            <td class="pulsanti" align="right" height="20" valign="bottom">
                                            </td>
                                        </tr>
                                        <!-- Ricerca modello -->
                                        <tr>
                                            <td colspan="2">
                                                <table width="100%">
                                                    <tr>
                                                        <td colspan="2">
                                                            <asp:UpdatePanel runat="server" ID="UpPnlSearchModel" UpdateMode="Conditional">
                                                                <ContentTemplate>
                                                                    <cc2:PannelloRicercaModelliTrasmissione ID="prmtRicerca" runat="server" Search="btn_ricerca_Click"
                                                                        SearchContext="ModelliTrasmissioneUtente" UserType="User" />
                                                                    <div class="content">
                                                                        <div class="filter" style="float: left; width: 97%; text-align: right;">
                                                                            <cc1:CustomButton ID="btnFindAndReplace" runat="server" CssClass="buttonAbortLarge"
                                                                                CssClassDisabled="buttonAbortLargeDisable" OnMouseOver="buttonAbortLargeHover"
                                                                                ClientIDMode="Static" Visible="true" OnClick="btnFindAndReplace_Click" />
                                                                            <cc1:CustomButton ID="btnFind" runat="server" CssClass="buttonAbort" CssClassDisabled="buttonAbortDisable"
                                                                                OnMouseOver="buttonAbortHover" ClientIDMode="Static" Visible="true" OnClick="btn_ricerca_Click" />
                                                                            <cc1:CustomButton ID="btnExport" runat="server" CssClass="buttonAbort" CssClassDisabled="buttonAbortDisable"
                                                                                OnMouseOver="buttonAbortHover" ClientIDMode="Static" Visible="true" OnClick="btnExport_Click" />
                                                                        </div>
                                                                    </div>
                                                                </ContentTemplate>
                                                            </asp:UpdatePanel>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <div id="Div2" style="width: 100%; margin-bottom: 20px;" align="center" runat="server">
                                                    <!-- INIZIO : PANNELLO NUOVO TEMPLATE -->
                                                    <asp:UpdatePanel runat="server" ID="UpPnlNuovoModello" UpdateMode="Conditional" ClientIDMode="Static" >
                                                        <ContentTemplate>
                                                            <asp:HiddenField ID="deleteDest" runat="server" ClientIDMode="Static" />
                                                            <asp:Panel ID="Panel_NuovoModello" runat="server" Visible="False">
                                                                <fieldset>
                                                                    <table width="99%" border="0">
                                                                        <tbody>
                                                                            <tr>
                                                                                <td class="weight" align="center" colspan="4" height="25">
                                                                                    <asp:Label ID="lbl_stato" runat="server" CssClass="weight" />
                                                                                    Modello di Trasmissione
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="testo_grigio_scuro" style="height: 23px" align="left" width="120">
                                                                                    Nome *
                                                                                </td>
                                                                                <td style="width: 265px; height: 23px" align="left" width="265">
                                                                                    <cc1:CustomTextArea ID="txt_nomeModello" runat="server" CssClass="txt_input_full"
                                                                                        CssClassReadOnly="txt_input_full_disabled" Width="300px"></cc1:CustomTextArea>
                                                                                </td>
                                                                                <td class="testo_grigio_scuro" style="height: 23px" align="left" width="6%" rowspan="1">
                                                                                    Note
                                                                                </td>
                                                                                <td align="left" width="400" rowspan="2">
                                                                                    <cc1:CustomTextArea ID="txt_noteGenerali" runat="server" TextMode="MultiLine"
                                                                                        CssClass="txt_textarea" CssClassReadOnly="txt_textarea_disabled" ClientIDMode="Static" Width="300px" Rows="3"></cc1:CustomTextArea>
                                                                                    <%--<cc1:CustomTextArea ID="txt_noteGenerali" runat="server" CssClass="txt_textarea"
                                                                                        CssClassReadOnly="txt_textarea_disabled" Width="300px" Rows="3" TextMode="MultiLine"></cc1:CustomTextArea>--%>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td rowspan="2" colspan="4">
                                                                                    <div class="col-right-no-margin">
                                                                                        <span class="charactersAvailable">
                                                                                            <asp:Literal ID="Ltrtxt_noteGenerali" runat="server" ClientIDMode="Static"> </asp:Literal>
                                                                                            <span id="txt_noteGenerali_chars" clientidmode="Static" runat="server"></span>
                                                                                        </span>
                                                                                    </div>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td style="height: 23px" align="left" width="120">
                                                                                    <asp:Label ID="lbl_codice" runat="server" Visible="true" Text="Codice" CssClass="testo_grigio_scuro"></asp:Label>
                                                                                </td>
                                                                                <td style="width: 265px; height: 23px" align="left" width="265">
                                                                                    <cc1:CustomTextArea ID="txt_codModello" runat="server" CssClass="txt_input_full"
                                                                                        CssClassReadOnly="txt_input_full_disabled" Width="80px"></cc1:CustomTextArea>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="testo_grigio_scuro" align="left">
                                                                                    Tipo Trasmissione *
                                                                                </td>
                                                                                <td align="left" colspan="2" class="testo_grigio_scuro">
                                                                                    <asp:DropDownList ID="ddl_tipoTrasmissione" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddl_tipoTrasmissione_OnSelectedIndexChanged"
                                                                                        CssClass="chzn-select-deselect" Width="190">
                                                                                        <asp:ListItem Value="D">Documento</asp:ListItem>
                                                                                        <asp:ListItem Value="F">Fascicolo</asp:ListItem>
                                                                                    </asp:DropDownList>
                                                                                    &nbsp;
                                                                                    <asp:Label ID="lbl_registro_obb" runat="server" CssClass="testo_grigio_scuro" Visible="true"
                                                                                        Text="Registro*"></asp:Label>
                                                                                    &nbsp;
                                                                                    <asp:DropDownList ID="ddl_registri" CssClass="chzn-select-deselect" AutoPostBack="True"
                                                                                        runat="server">
                                                                                    </asp:DropDownList>
                                                                                    <asp:Label ID="lbl_registri" runat="server" CssClass="testo_grigio" Visible="false"></asp:Label>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="testo_grigio_scuro" align="left">
                                                                                    Rendi disponibile
                                                                                </td>
                                                                                <td colspan="3">
                                                                                    <asp:RadioButtonList ID="rbl_share" TabIndex="3" runat="server" CssClass="testo_grigio"
                                                                                        Width="250px" BackColor="Transparent" RepeatDirection="Horizontal">
                                                                                        <asp:ListItem Value="usr" Selected="True">solo a me stesso</asp:ListItem>
                                                                                        <asp:ListItem Value="grp">a tutto il ruolo</asp:ListItem>
                                                                                    </asp:RadioButtonList>
                                                                                </td>
                                                                            </tr>
                                                                    </table>
                                                                </fieldset>
                                                            </asp:Panel>
                                                            <br>
                                                            <asp:Panel ID="Panel_dest" runat="server" Visible="False" BorderWidth="1px" BorderStyle="None">
                                                                <table cellspacing="1" cellpadding="0" width="100%">
                                                                    <tr>
                                                                        <td class="testo_grigio_scuro" align="left">
                                                                            Seleziona la Ragione Trasmissione<asp:Label ID="lbl_ragione" runat="server" Text=" *"></asp:Label>&nbsp;&nbsp;
                                                                            <asp:DropDownList ID="ddl_ragioni" CssClass="chzn-select-deselect" runat="server"
                                                                                AutoPostBack="True" Width="300">
                                                                            </asp:DropDownList>
                                                                        </td>
                                                                        <td class="testo_grigio_scuro" style="height: 14px" align="left">
                                                                            Seleziona i Destinatari *&nbsp;&nbsp;<cc1:CustomTextArea ID="txt_codDest" CssClass="txt_input_full"
                                                                                CssClassReadOnly="txt_input_full_disabled" Width="95px" runat="server" AutoComplete="off"></cc1:CustomTextArea>
                                                                            &nbsp;&nbsp;<cc1:CustomImageButton runat="server" ID="ibtnMoveToA" ImageUrl="../Images/Icons/view_response_documents.png"
                                                                                OnMouseOutImage="../Images/Icons/view_response_documents.png" OnMouseOverImage="../Images/Icons/view_response_documents_hover.png"
                                                                                CssClass="clickable" OnClick="ibtnMoveToA_Click" />
                                                                            &nbsp;&nbsp;&nbsp;<cc1:CustomImageButton runat="server" ID="btn_Rubrica_dest" ImageUrl="../Images/Icons/address_book.png"
                                                                                OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                                                                CssClass="clickable" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                                                                OnClick="btn_Rubrica_dest_Click" />
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td colspan="2">
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </asp:Panel>
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                    <!-- FINE : PANNELLO NUOVO TEMPLATE -->
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <div id="Div1" style="width: 100%;" align="center" runat="server">
                                                    <asp:UpdatePanel runat="server" ID="UpPnlDest" UpdateMode="Conditional">
                                                        <ContentTemplate>
                                                            <asp:GridView ID="dt_dest" CssClass="tbl_rounded_custom round_onlyextreme" runat="server"
                                                                Visible="False" DataKeyNames="ID_RAGIONE,VAR_COD_RUBRICA" AutoGenerateColumns="False"
                                                                Width="100%" PageSize="1" onitemcreated="dt_dest_ItemCreated" OnRowDeleting="dt_dest_RowDeleting"
                                                                EnableViewState="true" BorderWidth="0px">
                                                                <RowStyle CssClass="NormalRow" Height="50" />
                                                                <AlternatingRowStyle CssClass="AltRow" />
                                                                <PagerStyle CssClass="recordNavigator2" />
                                                                <Columns>
                                                                    <asp:BoundField Visible="False" DataField="SYSTEM_ID" HeaderText="SystemIdMittDest">
                                                                    </asp:BoundField>
                                                                    <asp:BoundField DataField="RAGIONE" HeaderText="Ragione">
                                                                        <HeaderStyle Width="10%"></HeaderStyle>
                                                                    </asp:BoundField>
                                                                    <asp:TemplateField>
                                                                        <HeaderStyle Width="2%"></HeaderStyle>
                                                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                                                        <ItemTemplate>
                                                                            <asp:ImageButton ID="Img_tipo_urp" runat="server"></asp:ImageButton>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:BoundField DataField="VAR_COD_RUBRICA" HeaderText="Codice" HtmlEncode="false">
                                                                        <HeaderStyle Width="10%"></HeaderStyle>
                                                                    </asp:BoundField>
                                                                    <asp:BoundField DataField="VAR_DESC_CORR" HeaderText="Descrizione" HtmlEncode="false">
                                                                        <HeaderStyle Width="35%"></HeaderStyle>
                                                                    </asp:BoundField>
                                                                    <asp:TemplateField HeaderText="Tipo">
                                                                        <HeaderStyle Width="60px"></HeaderStyle>
                                                                        <ItemTemplate>
                                                                            <asp:DropDownList ID="DropDownList1" runat="server" CssClass="chzn-select-deselect"
                                                                                Width="150">
                                                                                <asp:ListItem Value="S">Uno</asp:ListItem>
                                                                                <asp:ListItem Value="T">Tutti</asp:ListItem>
                                                                            </asp:DropDownList>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Note">
                                                                        <HeaderStyle Width="30%"></HeaderStyle>
                                                                        <ItemTemplate>
                                                                            <asp:TextBox ID="TextBox1" runat="server" CssClass="testo_grigio" Width="97%"></asp:TextBox>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="SCADENZA">
                                                                        <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                                            Font-Underline="False" HorizontalAlign="Center" />
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="Label1" runat="server" Font-Bold="True" Text="gg" Width="20px"></asp:Label>
                                                                            <asp:TextBox ID="txt_scadenza" runat="server" CssClass="testo_grigio" MaxLength="3"
                                                                                Width="30px"></asp:TextBox>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle Font-Bold="True" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                                            Font-Underline="False" HorizontalAlign="Center" Width="70px" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Nasc. vers." HeaderStyle-HorizontalAlign="Center">
                                                                        <ItemStyle Width="5px" HorizontalAlign="Center" />
                                                                        <ItemTemplate>
                                                                            <asp:CheckBox ID="chkNascondiVersioniPrecedentiDocumento" runat="server" ToolTip="Ai destinatari della trasmissione saranno nascoste le versioni precedenti a quella corrente dei documenti consolidati"
                                                                                CssClass="testo" Checked='<%#DataBinder.Eval(Container.DataItem, "NASCONDI_VERSIONI_PRECEDENTI")%>' />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:ButtonField Text="&lt;img src=../Images/Icons/delete2.png border=0 alt='Elimina'&gt;"
                                                                        HeaderText="Elimina" CommandName="Delete">
                                                                        <HeaderStyle Width="3%"></HeaderStyle>
                                                                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                                    </asp:ButtonField>
                                                                    <asp:BoundField Visible="False" DataField="ID_RAGIONE" HeaderText="id_ragione"></asp:BoundField>
                                                                </Columns>
                                                                <PagerStyle HorizontalAlign="Center" BackColor="#EAEAEA" CssClass="bg_grigioN"></PagerStyle>
                                                            </asp:GridView>
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center" colspan="2" height="5">
                                                <!-- INIZIO : PANNELLO LISTA MODELLI -->
                                            
                                                <asp:UpdatePanel runat="server" ID="UpPnlListaModelli" UpdateMode="Conditional">
                                                    <ContentTemplate>
                                                        <asp:HiddenField ID="deleteMod" runat="server" ClientIDMode="Static" />
                                                        <asp:Panel ID="Panel_ListaModelli" runat="server" Visible="true">
                                                            <div id="DivDGListaTemplates" align="center" runat="server">
                                                                <asp:GridView ID="dt_listaModelli" CssClass="tbl_rounded_custom round_onlyextreme"
                                                                    runat="server" AutoGenerateColumns="False" Width="100%" BorderWidth="0px" AllowPaging="True"
                                                                    DataKeyNames="SYSTEM_ID" OnItemCreated="Grid_OnItemCreated" OnRowDeleting="dt_listaModelli_RowDeleting"
                                                                    OnPageIndexChanged="dt_listaModelli_PageIndexChanged" OnPageIndexChanging="dt_listaModelli_PageIndexChanging" OnRowCommand="dt_dest_RowCommand">
                                                                    <RowStyle CssClass="NormalRow" />
                                                                    <AlternatingRowStyle CssClass="AltRow" />
                                                                    <PagerStyle CssClass="recordNavigator" />
                                                                    <Columns>
                                                                        <asp:BoundField Visible="False" DataField="SYSTEM_ID" HeaderText="SystemId"></asp:BoundField>
                                                                        <asp:BoundField DataField="CODICE" HeaderText="Codice" HtmlEncode="false">
                                                                            <HeaderStyle Width="8%"></HeaderStyle>
                                                                        </asp:BoundField>
                                                                        <asp:BoundField DataField="MODELLO" HeaderText="Modello" HtmlEncode="false">
                                                                            <HeaderStyle Width="24%"></HeaderStyle>
                                                                        </asp:BoundField>
                                                                        <asp:BoundField DataField="REGISTRO" HeaderText="REGISTRO">
                                                                            <HeaderStyle Width="24%"></HeaderStyle>
                                                                        </asp:BoundField>
                                                                        <asp:BoundField DataField="TIPO DI TRASM." HeaderText="TIPO DI TRASM.">
                                                                            <HeaderStyle Width="18%"></HeaderStyle>
                                                                        </asp:BoundField>
                                                                        <asp:BoundField DataField="VISIBILITA'" HeaderText="VISIBILITA'">
                                                                            <HeaderStyle Width="20%"></HeaderStyle>
                                                                        </asp:BoundField>
                                                                        <asp:ButtonField Text="&lt;img src=../Images/Icons/view_doc_grid.png border=0 alt='Selezione'&gt;"
                                                                            HeaderText="Seleziona" CommandName="Select">
                                                                            <HeaderStyle Width="3%"></HeaderStyle>
                                                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                                        </asp:ButtonField>
                                                                        <asp:ButtonField Text="&lt;img src=../Images/Icons/delete2.png border=0 alt='Elimina'&gt;"
                                                                            HeaderText="Elimina" CommandName="Delete">
                                                                            <HeaderStyle Width="3%"></HeaderStyle>
                                                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                                        </asp:ButtonField>
                                                                    </Columns>
                                                                </asp:GridView>
                                                            </div>
                                                        </asp:Panel>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                                <!-- FINE : PANNELLO LISTA MODELLI -->
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" UpdateMode="Conditional" runat="server" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="btn_lista_modelli" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" Visible="false" />
            <cc1:CustomButton ID="btn_salvaModello" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" Visible="false" />
            <cc1:CustomButton ID="btn_nuovoModello" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="btn_nuovoModello_Click" />
            <cc1:CustomButton ID="btn_pp_notifica" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" Visible="false" />
            <input id="hd_returnValueModal" type="hidden" name="hd_returnValueModal" runat="server" />
            <asp:Button ID="btnAddressBookPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnAddressBookPostback_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">        $(".chzn-select-deselect").chosen({
            allow_single_deselect: true, no_results_text: "Nessun risultato trovato"
        }); $(".chzn-select").chosen({
            no_results_text: "Nessun risultato trovato"
        }); </script>
</asp:Content>
