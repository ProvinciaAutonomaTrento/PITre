<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="newPolicyStampe.aspx.cs" Inherits="DocsPAWA.AdminTool.Gestione_Conservazione.newPolicyStampe" %>

<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<%@ Register Src="../../UserControls/Calendar.ascx" TagName="Calendario" TagPrefix="uc4" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="head" runat="server">
    <title>Crea una nuova Policy per le Stampe</title>
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet" />
    <base target="_self" />
    <script type="text/javascript" language="javascript">
        function close_and_save(ret) {
            window.returnValue = ret;
            window.close();
        }

    </script>
    <script src="../../LIBRERIE/rubrica.js" type="text/javascript"></script>
    <style type="text/css">
        body
        {
            font-family: Verdana;
            overflow-x: hidden;
            overflow-y: scroll;
        }
        #container
        {
            float: left;
            width: 99%;
            background-color: #ffffff;
            height: 400px;
        }
        #content
        {
            border: 1px solid #cccccc;
            text-align: center;
            width: 97%;
            float: left;
            margin-left: 6px;
            margin-top: 5px;
        }
        .title
        {
            margin: 0px;
            padding: 0px;
            font-size: 11px;
            font-weight: bold;
            text-align: center;
            width: 100%;
            float: left;
            padding-top: 4px;
            padding-bottom: 4px;
            background-color: #810101;
            color: #ffffff;
        }
        #content_field
        {
            width: 100%;
            float: left;
            background-color: #fbfbfb;
           height: 405px;
        }
        .contenitore_box
        {
            text-align: left;
            font-family: Tahoma, Arial, sans-serif;
            margin: 5px;
        }
        .contenitore_box fieldset
        {
            border: 1px solid #eaeaea;
            margin: 5px;
            padding: 0px;
        }
        
        .contenitore_box legend
        {
            font-size: 12px;
            font-weight: normal;
            color: #4b4b4b;
            font-family: Verdana;
            margin-left: 15px;
            font-weight: bold;
        }
        
        .contenitore_box_due
        {
            text-align: left;
            font-family: Tahoma, Arial, sans-serif;
            margin: 5px;
        }
        
        .contenitore_box_due fieldset
        {
            border: 1px solid #eaeaea;
            margin: 5px;
            padding: 5px;
        }
        
        .contenitore_box_due legend
        {
            font-size: 10px;
            font-weight: normal;
            color: #4b4b4b;
            font-family: Verdana;
            margin-left: 15px;
            font-weight: bold;
            padding-bottom: 5px;
        }
        
        #imposta
        {
            float: left;
            width: 100%;
            clear: both;
            margin-left: 10px;
            text-align: left;
            margin-bottom: 15px;
        }
        .cbtn
        {
            font-family: Verdana;
            font-size: 11px;
            margin: 0px;
            padding: 0px;
            padding: 2px;
            width: 120px;
            height: 25px;
            color: #ffffff;
            border: 1px solid #ffffff;
            background-image: url('../images/bg_button.gif');
        }
        .titolo_scheda
        {
            font-size: 11px;
            color: #666666;
            font-weight: bold;
        }
        .testo_grigio
        {
            font-size: 11px;
            font-family: Verdana;
            color: #333333;
        }
        .testo_chk
        {
            font-size: 9px;
            font-weight: normal;
            color: #4b4b4b;
            font-family: Verdana;
            font-weight: normal;
        }
    </style>
</head>
<body>
    <form id="Form" method="post" runat="server">
    <asp:HiddenField ID="hid_tab_est" runat="server" />
    <asp:ScriptManager ID="ScriptManager" runat="server" AsyncPostBackTimeout="360000">
    </asp:ScriptManager>
    <div id="container">
        <div id="content">
            <asp:Label runat="server" ID="titlePage" Text="Crea una nuova Policy" CssClass="title"></asp:Label>
            <div id="content_field">
                <asp:UpdatePanel ID="upNamePolicy" runat="server" UpdateMode="Conditional">
                    <contenttemplate>
                    <asp:Panel runat="server" CssClass="contenitore_box_due" id="pnl_name">
                            <fieldset>
                                <legend>Nome della Policy*</legend>
                                    <asp:TextBox id="txt_nome" runat="server" Width="99%" CssClass="testo_grigio" MaxLength="100"></asp:TextBox>
                            </fieldset>
                    </asp:Panel>
                    </contenttemplate>
                </asp:UpdatePanel>   
                  <asp:UpdatePanel ID="upTipoStampe" runat="server" UpdateMode="Conditional">
                    <contenttemplate>
                        <div class="contenitore_box_due">
                                <fieldset>
                                    <legend>Tipologia di stampe</legend>
                                    <asp:RadioButtonList runat="server" ID="rbl_tipo" CssClass="testo_grigio" TextAlign="Left" RepeatDirection="Horizontal" OnSelectedIndexChanged="StampeChange_Click" AutoPostBack="true">
                                    <asp:ListItem Text="Stampe Repertori" Selected="true" Value="C"></asp:ListItem>
                                    <asp:ListItem Text="Stampe Registro" Value="R"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </fieldset>
                        </div>
                    </contenttemplate>
                </asp:UpdatePanel>  
                  <asp:UpdatePanel ID="upTipoRepertori" runat="server" UpdateMode="Conditional">
                    <contenttemplate>
                    <asp:Panel runat="server" CssClass="contenitore_box_due" id="pnl_profilazione">
                            <fieldset>
                                <legend>Registri di repertorio</legend>
                                    <asp:DropDownList ID="ddl_type_repertorio" runat="server" CssClass="testo_grigio" Width="100%" OnSelectedIndexChanged="RepertoriChange_Click" AutoPostBack="true" >
                                    </asp:DropDownList>                      
                            </fieldset>
                    </asp:Panel>
                    </contenttemplate>
                </asp:UpdatePanel>   
                  <asp:UpdatePanel ID="upRfAOO" runat="server" UpdateMode="Conditional">
                    <contenttemplate>
                    <asp:Panel runat="server" CssClass="contenitore_box_due" id="pnl_rf_aoo">
                            <fieldset>
                                <legend>RF/AOO</legend>
                                    <asp:DropDownList ID="ddl_rf_aoo" runat="server" CssClass="testo_grigio" Width="100%" >
                                    </asp:DropDownList>                      
                            </fieldset>
                    </asp:Panel>
                    </contenttemplate>
                </asp:UpdatePanel>   
                <asp:UpdatePanel ID="upCreationDate" runat="server" UpdateMode="Conditional">
                    <contenttemplate>
                        <div class="contenitore_box_due">
                                <fieldset>
                                    <legend>Data di creazione</legend>
                                    <asp:dropdownlist id="ddl_dataCreazione_E" runat="server" AutoPostBack="true" CssClass="testo_grigio" Width="150px" OnSelectedIndexChanged="ddl_dataCreazione_E_SelectedIndexChanged">
												<asp:ListItem Value="0">Valore Singolo</asp:ListItem>
												<asp:ListItem Value="1">Intervallo</asp:ListItem>
									            <asp:ListItem Value="2">Oggi</asp:ListItem>
								                <asp:ListItem Value="3">Settimana Corrente</asp:ListItem>
								                <asp:ListItem Value="4">Mese Corrente</asp:ListItem>
                                                <asp:ListItem Value="5">Anno Corrente</asp:ListItem>
											</asp:dropdownlist>
                                         <asp:label id="lblDa" runat="server" CssClass="testo_grigio" Width="18px">Da</asp:label>
                                         <uc4:Calendario id="lbl_dataCreazioneDa" runat="server" Visible="true" PaginaChiamante="esportalog" CSS="testo" onkeypress='#MaxCaratteri(this, 0);'/>
                                         <asp:label id="lblA" runat="server" CssClass="testo_grigio" Width="18px">A</asp:label>
                                         <uc4:Calendario id="lbl_dataCreazioneA" runat="server" Visible="true" PaginaChiamante="esportalog" CSS="testo" onkeypress="return MaxCaratteri(this, 0);"/>
                                </fieldset>
                        </div>
                    </contenttemplate>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="UpdatePanelConfirm" runat="server" UpdateMode="Conditional">
                    <contenttemplate>
                <table id="Table3" align="center">
                    <tr>
                        <td align="center">
                            <asp:Button ID="btn_salva" runat="server" CssClass="cbtn" Text="Salva" OnClick="BtnSaveDocument_Click"></asp:Button>
                        </td>
                        <td>
                            <asp:Button ID="btn_annulla" runat="server" CssClass="cbtn" Text="Chiudi"
                                OnClientClick="window.close();"></asp:Button>
                        </td>
                    </tr>
                </table>
                   </contenttemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>
    </form>
</body>
</html>

