<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="CorrespondentDetails.aspx.cs" Inherits="NttDataWA.Popup.CorrespondentDetails" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Import namespace="NttDataWA.DocsPaWR" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .container {width: 95%; margin: 0 auto;}
        p {text-align: left;}
        em {font-style: normal; font-weight: bold; color: #9D9D9D;}
        .col-label {width: 100px; overflow: hidden; padding: 5px 0 0 0;}
        div div.col-label:last-child {color: #f00;}
        .txt_input, .txt_input_disabled, .txt_textarea, .txt_textarea_disabled {width: 472px; height: 1.5em;}
        .txt_textarea, .txt_textarea_disabled {height: 3em; vertical-align: top;}
        .txt_input_disabled {background-color: #ffffff;}
        .txt_input_small {width: 168px;}
        .tbl tr.NormalRow:hover td {background-color: #fff;}
        .tbl tr.AltRow:hover td {background-color: #e1e9f0;}
    </style>
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
<div class="container">
    <div class="row">
        <p class="center">
            <strong><asp:Literal ID="lbl_nomeCorr" runat="server" /></strong><br />
            <em><asp:Literal ID="lbl_codRubr_corr" runat="server" /></em>
        </p>
    </div>
    <asp:PlaceHolder ID="pnl_registro" runat="server">
        <div class="row">
            <div class="col col-label">
                <asp:Literal ID="lbl_registro" runat="server" /></div>
            <div class="col-no-margin">
                <asp:Literal ID="lit_registro" runat="server" />
            </div>
        </div>
    </asp:PlaceHolder>
    <div class="row">
        <div class="col col-label"><asp:Literal id="lbl_tipocorr" runat="server" /></div>
        <div class="col-no-margin">
                <asp:DropDownList ID="ddl_tipoCorr" runat="server" AutoPostBack="true" CssClass="chzn-select-deselect" Enabled="false" Width="200px">
                    <asp:ListItem Text="UO" Value="U" />  
                    <asp:ListItem Text="RUOLO" Value="R" />
                    <asp:ListItem Text="PERSONA" Value="P" />
                    <asp:ListItem Text="RAGGRUPPAMENTO FUNZIONALE" Value="F" />
                    <asp:ListItem Text="OCCASIONALE" Value="O" />
                </asp:DropDownList>
        </div>
    </div>
    <asp:PlaceHolder id="pnl_user" runat="server">
          <div class="row">
            <div class="col col-label"><asp:Literal id="lbl_titolo" runat="server" /></div>
            <div class="col-no-margin">
                    <asp:DropDownList ID="ddl_titolo" runat="server" CssClass="chzn-select-deselect" Width="200px">
                    </asp:DropDownList>
            </div>
        </div>
          <div class="row">
            <div class="col col-label"><asp:Literal id="lbl_nome" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="txt_nome" runat="server" CssClass="txt_input" Enabled="false" CssClassReadOnly="txt_input_disabled" />
            </div>
        </div>
        <div class="row">
            <div class="col col-label"><asp:Literal id="lbl_cognome" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="txt_cognome" runat="server" CssClass="txt_input" Enabled="false" CssClassReadOnly="txt_input_disabled" />
            </div>
        </div>
        <div class="row">
            <div class="col col-label"><asp:Literal id="lbl_luogoNasc" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="txt_luogoNasc" runat="server" CssClass="txt_input" CssClassReadOnly="txt_input_disabled" />
            </div>
        </div>
        <div class="row">
            <div class="col col-label"><asp:Literal id="lbl_dtnasc" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="txt_dtnasc" runat="server" CssClass="txt_textdata datepicker" CssClassReadOnly="txt_textdata_disabled" />
            </div>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder id="pnl_notcommon" runat="server">
        <asp:PlaceHolder id="pnl_ddCanPref" runat="server">
            <div class="row">
                <div class="col col-label"><asp:Literal id="lbl_canalePreferenziale" runat="server" /></div>
                <div class="col-no-margin">
                    <asp:DropDownList ID="dd_canpref" runat="server" AutoPostBack="true" Enabled="false" CssClass="chzn-select-deselect" Width="200px" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="row">
            <div class="col col-label"><asp:Literal id="lbl_indirizzo" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="txt_indirizzo" runat="server" CssClass="txt_input" CssClassReadOnly="txt_input_disabled" />
            </div>
        </div>
        <div class="row">
            <div class="col col-label"><asp:Literal id="lbl_citta" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="txt_citta" runat="server" CssClass="txt_input txt_input_small" CssClassReadOnly="txt_input_disabled txt_input_small" />
            </div>
            <div class="col col-label right"><asp:Literal id="lbl_provincia" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="txt_provincia" runat="server" CssClass="txt_input txt_input_small" CssClassReadOnly="txt_input_disabled txt_input_small" MaxLength="2" />
            </div>
        </div>
        <div class="row">
            <div class="col col-label"><asp:Literal id="lbl_local" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="txt_local" runat="server" CssClass="txt_input txt_input_small" CssClassReadOnly="txt_input_disabled txt_input_small" MaxLength="128" />
            </div>
            <div class="col col-label right"><asp:Literal id="lbl_nazione" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="txt_nazione" runat="server" CssClass="txt_input txt_input_small" CssClassReadOnly="txt_input_disabled txt_input_small" />
            </div>
        </div>
        <div class="row">
            <div class="col col-label"><asp:Literal id="lbl_telefono" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="txt_telefono" runat="server" CssClass="txt_input txt_input_small" CssClassReadOnly="txt_input_disabled txt_input_small" />
            </div>
            <div class="col col-label right"><asp:Literal id="lbl_telefono2" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="txt_telefono2" runat="server" CssClass="txt_input txt_input_small" CssClassReadOnly="txt_input_disabled txt_input_small" />
            </div>
        </div>
        <div class="row">
            <div class="col col-label"><asp:Literal id="lbl_fax" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="txt_fax" runat="server" CssClass="txt_input txt_input_small" CssClassReadOnly="txt_input_disabled txt_input_small" />
            </div>
            <div class="col col-label right"><asp:Literal id="lbl_cap" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="txt_cap" runat="server" CssClass="txt_input txt_input_small" MaxLength="5" CssClassReadOnly="txt_input_disabled txt_input_small" />
            </div>
        </div>
        <div class="row">
            <div class="col col-label"><asp:Literal id="lbl_codfisc" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="txt_codfisc" runat="server" CssClass="txt_input txt_input_small" CssClassReadOnly="txt_input_disabled txt_input_small" MaxLength="16" />
            </div>
            <div class="col col-label right"><asp:Literal id="lbl_partita_iva" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="txt_partita_iva" runat="server" CssClass="txt_input txt_input_small" CssClassReadOnly="txt_input_disabled txt_input_small" MaxLength="11" />
            </div>
        </div>
        <%--<div class="row">
            <div class="col col-label"><asp:Literal id="lbl_codice_ipa" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="txt_codice_ipa" runat="server" CssClass="txt_input txt_input_small" CssClassReadOnly="txt_input_disabled txt_input_small" MaxLength="11" />
            </div>
        </div>--%>
    </asp:PlaceHolder>
 	<asp:PlaceHolder id="PanelListaCorrispondenti" runat="server" Visible="false">
		<div class="row">
            <asp:GridView ID="dg_listCorr" runat="server" Width="600" AutoGenerateColumns="False"
                AllowPaging="False" ClientIDMode="Static" CssClass="tbl">
                <RowStyle CssClass="NormalRow" />
                <AlternatingRowStyle CssClass="AltRow" />
                <Columns>
                    <asp:BoundField DataField="CODICE" ItemStyle-Width="30%" />
                    <asp:BoundField DataField="DESCRIZIONE" ItemStyle-Width="70%" />
                </Columns>
            </asp:GridView>
        </div>
	</asp:PlaceHolder>
    <asp:PlaceHolder ID="pnl_email" runat="server">
        <asp:PlaceHolder ID="plcSingleMail" runat="server">
            <div class="row">
                <div class="col col-label"><asp:Literal id="lbl_email" runat="server" /></div>
                <div class="col-no-margin">
                    <cc1:CustomTextArea ID="txt_email" runat="server" CssClass="txt_input" CssClassReadOnly="txt_input_disabled" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="row">
            <div class="col col-label"><asp:Literal id="lbl_codAOO" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="txt_codAOO" runat="server" CssClass="txt_input" CssClassReadOnly="txt_input_disabled" />
            </div>
        </div>
        <div class="row">
            <div class="col col-label"><asp:Literal id="lbl_codAmm" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="txt_codAmm" runat="server" CssClass="txt_input" CssClassReadOnly="txt_input_disabled" />
            </div>
        </div>
        <div class="row">
            <div class="col col-label"><asp:Literal id="lbl_note" runat="server" /></div>
            <div class="col-no-margin">
                <cc1:CustomTextArea ID="txt_note" runat="server" CssClass="txt_textarea" CssClassReadOnly="txt_textarea_disabled" />
            </div>
        </div>
        <asp:PlaceHolder ID="plc_gvCaselle" runat="server" Visible="false">
            <div class="row" style="padding: 5px 0 0 0;">
                <asp:GridView  ID="gvCaselle" runat="server" AutoGenerateColumns="False" CssClass="tbl" style="overflow-y: scroll; overflow-x: hidden; max-height: 90px; width: 600px;">
                    <RowStyle CssClass="NormalRow" />
                    <AlternatingRowStyle CssClass="AltRow" />                                                                                   
                    <Columns>
                        <asp:TemplateField HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden">
                            <ItemTemplate>
                                <asp:Label runat="server" ID ="lblSystemId" Text ='<%# ((MailCorrispondente)Container.DataItem).systemId %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-Width="68%">
                            <ItemTemplate>
                                <cc1:CustomTextArea ID="txtEmailCorr" runat="server" Enabled="false" CssClass="txt_input txt_input_small" CssClassReadOnly="txt_input_disabled txt_input_small" width="385" Text='<%# ((MailCorrispondente)Container.DataItem).Email %>' />
                            </ItemTemplate>
                        </asp:TemplateField> 
                        <asp:TemplateField ItemStyle-Width="28%">
                            <ItemTemplate>
                                <cc1:CustomTextArea ID="txtNoteMailCorr" runat="server" Enabled="false"  CssClass="txt_input txt_input_small" CssClassReadOnly="txt_input_disabled txt_input_small" MaxLength="20" width="150" Text='<%# ((MailCorrispondente)Container.DataItem).Note %>' />
                            </ItemTemplate>
                        </asp:TemplateField> 
                        <asp:TemplateField ItemStyle-Width="4%">
                            <ItemTemplate>
                                <asp:RadioButton ID="rdbPrincipale" runat="server" Enabled="false" Checked='<%# TypeMailCorrEsterno(((MailCorrispondente)Container.DataItem).Principale) %>' />
                            </ItemTemplate>
                        </asp:TemplateField> 
                    </Columns>
                </asp:GridView>
            </div>
        </asp:PlaceHolder>
        <div class="row" style="padding: 5px 0 0 0;">
            <asp:PlaceHolder id="plc_DescIntOp" runat="server" />
        </div>
    </asp:PlaceHolder>
</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <cc1:CustomButton ID="BtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
        OnMouseOver="btnHover" ClientIDMode="Static"   OnClientClick="disallowOp('Content2');"
        onclick="BtnClose_Click" />
    <cc1:CustomButton ID="btn_CreaCorDoc" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
        OnMouseOver="btnHover" ClientIDMode="Static" visible="false"  OnClientClick="disallowOp('Content2');"
        onclick="btn_CreaCorDoc_Click" />
 <script type="text/javascript">     $(".chzn-select-deselect").chosen({ allow_single_deselect:
true, no_results_text: "Nessun risultato trovato"
     }); $(".chzn-select").chosen({
         no_results_text: "Nessun risultato trovato"
     }); </script>
</asp:Content>