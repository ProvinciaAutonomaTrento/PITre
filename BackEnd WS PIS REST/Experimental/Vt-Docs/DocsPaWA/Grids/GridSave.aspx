<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GridSave.aspx.cs" Inherits="DocsPAWA.Grids.GridSave" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<%@ Register TagPrefix="cc1" Namespace="Utilities" Assembly="MessageBox" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="head" runat="server">
    <title>Salvataggio o modifica una griglia</title>
    <link href="../CSS/DocsPA_30.css" type="text/css" rel="stylesheet" />
    <base target="_self" />
    <script type="text/javascript" language="javascript">
        function close_and_save() {
            var ret = document.getElementById("hid_tab_est").value;
            window.returnValue = ret;
            window.close();
        }
//        function setFocus() {
//            if (document.getElementById("btn_salva") != null) 
//            {
//                document.getElementById("btn_salva").focus();
//            }
//        }
    </script>
    <style type="text/css">
        body
        {
            font-family: Verdana;
        }
        #container
        {
            float: left;
            width: 100%;
            background-color: #fafafa;
            height: 650px;
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
        }
        #content_field
        {
            width: 100%;
            float: left;
            background-color: #ffffff;
            height: 350px;
        }
        .contenitore_box
        {
            text-align: left;
            font-family: Tahoma, Arial, sans-serif;
            margin: 5px;
            margin-bottom: 10px;
        }
        
        .contenitore_box fieldset
        {
            border: 1px solid #eaeaea;
            margin: 5px;
            padding: 0px;
        }
        
        .contenitore_box legend
        {
            font-size: 10px;
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
            margin-bottom: 10px;
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
    </style>
</head>
<body>
    <form id="GridSave" method="post" runat="server">
    <asp:HiddenField ID="hid_tab_est" runat="server" />
    <asp:ScriptManager ID="ScriptManager" runat="server" AsyncPostBackTimeout="360000">
    </asp:ScriptManager>
    <div id="container">
        <div id="content">
            <asp:Label runat="server" ID="titlePage" Text="Salvataggio o modifica di una griglia"
                CssClass="title"></asp:Label>
            <div id="content_field">
                <asp:UpdatePanel ID="upTemplates" runat="server" UpdateMode="Conditional">
                    <contenttemplate>
                        <div class="contenitore_box">
                            <fieldset>
                                <legend>Azioni disponibili</legend>
                                <asp:RadioButtonList ID="rbl_save" runat="server" CssClass="testo_grigio" Width="368px"
                                    AutoPostBack="true" OnSelectedIndexChanged="ChangeMode">
                                     <asp:ListItem Value="new" Selected="True">Crea una nuova griglia</asp:ListItem>
                                    <asp:ListItem Value="mod">Modifica una griglia</asp:ListItem>
                                </asp:RadioButtonList>
                            </fieldset>
                        </div>
                    </contenttemplate>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="upChangeGrid" runat="server" UpdateMode="Conditional">
                    <contenttemplate>
                        <div class="contenitore_box_due">
                            <asp:Panel ID="pnl_scelta" runat="server" Visible="false">
                                <fieldset>
                                    <legend>Scegli la griglia da modificare</legend>
                                    <asp:DropDownList ID="ddl_ric_griglie" runat="server" CssClass="testo_grigio" Width="100%" OnSelectedIndexChanged="ChangeSelectedGrid" AutoPostBack="true">
                                    </asp:DropDownList>
                                </fieldset>
                            </asp:Panel>
                            <asp:Panel ID="nuova_g" runat="server" Visible="false">
                                <fieldset>
                                    <legend>Inserisci il nome della nuova griglia*</legend>
                                    <asp:TextBox ID="txt_titolo" runat="server" CssClass="testo_grigio" Width="98%"
                                        MaxLength="30"></asp:TextBox>
                                </fieldset>
                            </asp:Panel>
                        </div>
                    </contenttemplate>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="upChangeGridName" runat="server" UpdateMode="Conditional">
                    <contenttemplate>
                        <div class="contenitore_box_due">
                            <asp:Panel ID="modify_g" runat="server" Visible="false">
                                <fieldset>
                                    <legend>Nome della griglia da modificare*</legend>
                                    <asp:TextBox ID="txt_name_mod" runat="server" CssClass="testo_grigio" Width="98%"
                                        MaxLength="30"></asp:TextBox>
                                </fieldset>
                            </asp:Panel>
                        </div>
                    </contenttemplate>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="pnl_visibility" runat="server" UpdateMode="Conditional">
                    <contenttemplate>
                        <div class="contenitore_box">
                            <fieldset>
                                <legend>Rendi disponibile</legend>
                                <asp:RadioButtonList ID="rblVisibilita" runat="server" CssClass="testo_grigio" Width="398px">
                                    <asp:ListItem Value="user" Selected="True">Utente (@usr@)</asp:ListItem>
                                    <asp:ListItem Value="role">Ruolo (@grp@)</asp:ListItem>
                                </asp:RadioButtonList>
                            </fieldset>
                        </div>
                    </contenttemplate>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                    <contenttemplate>
                        <div id="imposta">
                            <asp:CheckBox runat="server" ID="chk_pref" TextAlign="right" Text="Imposta la griglia come predefinita" CssClass="titolo_scheda"  />
                        </div>
                    </contenttemplate>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="UpdatePanelConfirm" runat="server" UpdateMode="Conditional">
                    <contenttemplate>
                <table id="Table3" align="center">
                    <tr>
                        <td align="center">
                            <asp:Button ID="btn_salva" runat="server" CssClass="pulsante_mini_3" Text="Salva"
                                OnClick="btn_salva_Click"></asp:Button>

                             <cc2:ConfirmButtonExtender ID="bDeleteExtender" runat="server" TargetControlID="btn_salva"
                                    ConfirmText="Attenzione! Questa griglia è visibile a tutto il ruolo. Sei sicuro di volerla modificare?"/>
                        </td>
                        <td>
                            <asp:Button ID="btn_annulla" runat="server" CssClass="pulsante_mini_3" Text="Chiudi"
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
