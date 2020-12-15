<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExportDettagli.aspx.cs" Inherits="DocsPAWA.AdminTool.Gestione_Funzioni.ExportDettagli" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="head" runat="server">
    <title>Esporta dettagli</title>
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet" />
    <base target="_self" />

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
            width: 100%;
            background-color: #ffffff;
            height: 780px;
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
            height: 810px;
        }
        .contenitore_box
        {
            text-align: left;
            font-family: Tahoma, Arial, sans-serif;
            margin: 5px;
            margin-bottom: 5px;
            background-color: #ffffff;
            padding-bottom: 5px;
            padding-left: 5px;
            padding-right: 5px;
            font-size: 12px;
        }
        .contenitore_box fieldset
        {
            border: 1px dashed #eaeaea;
            margin: 0px;
            padding: 5px;
        }
        
        .contenitore_box legend
        {
            font-size: 10px;
            font-weight: normal;
            color: #4b4b4b;
            font-family: Verdana;
            margin-left: 15px;
            font-weight: bold;
            padding-bottom: 5px;
        }
        
        .contenitore_box_due
        {
            text-align: left;
            font-family: Tahoma, Arial, sans-serif;
            margin: 5px;
            margin-bottom: 5px;
            background-color: #ffffff;
            border: 1px solid #cccccc;
            padding-bottom: 5px;
            padding-left: 5px;
            padding-right: 5px;
        }
        
        .contenitore_box_due fieldset
        {
            border: 1px dashed #eaeaea;
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
        .testo_grigio2
        {
            font-size: 11px;
            font-family: Verdana;
            color: #333333;
            margin-top: 5px;
        }
        .tab_period
        {
            border-collapse: collapse;
            margin: 0px;
            padding: 0px;
            width: 100%;
        }
        .tab_period td
        {
            padding: 5px;
            background-color: #ffffff;
        }
        .t1
        {
            width: 5%;
            background-color: #fbfbfb;
            border: 1px solid #cccccc;
        }
        .t2
        {
            width: 95%;
            background-color: #ffffff;
            border: 1px solid #cccccc;
        }
        .inp1
        {
            width: 20px;
            font-family: Tahoma, Arial, sans-serif;
            font-size: 11px;
            color: #333333;
        }
        .inp2
        {
            width: 50px;
            font-family: Tahoma, Arial, sans-serif;
            font-size: 11px;
            color: #333333;
        }
        .contenitore_box_due p
        {
            margin: 0px;
            font-size: 11px;
            margin-top: 5px;
            padding: 0px;
            margin-left: 8px;
            color: #333333;
        }
        .contenitore_box p
        {
            margin: 0px;
            font-size: 11px;
            margin-top: 5px;
            padding: 0px;
            margin-left: 8px;
            color: #333333;
        }
        .ddl_d
        {
            margin-left: 10px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager" runat="server" AsyncPostBackTimeout="360000">
        </asp:ScriptManager>
        <div id="container">
            <asp:Label runat="server" ID="titlePage" Text="Esporta dettagli" CssClass="title"></asp:Label>
            <div id="content">
                <div id="content_field">
                    <asp:Panel runat="server" CssClass="contenitore_box_due" ID="pnl_tipo">
                        <fieldset>
                            <legend>Report</legend>
                            <asp:Label runat="server" ID="lbl_type_descr" CssClass="testo_grigio"></asp:Label>
                            <asp:Label runat="server" ID="lbl_type" Visible="false"></asp:Label>
                        </fieldset>
                    </asp:Panel>
                    <asp:Panel runat="server" CssClass="contenitore_box_due" ID="pnl_funzione">
                        <fieldset>
                            <legend>
                                <asp:Label runat="server" ID="lbl_funzione" ></asp:Label>
                            </legend>
                                <asp:Label runat="server" ID="lbl_description" CssClass="testo_grigio"></asp:Label>
                                <asp:Label runat="server" ID="lbl_cod" Visible="false"></asp:Label>
                        </fieldset>
                    </asp:Panel>
                    <asp:Panel runat="server" CssClass="contenitore_box_due" id="pnl_format">
                        <fieldset>
                            <legend>Formato di esportazione</legend>
                            <asp:DropDownList runat="server" ID="ddl_format" CssClass="testo_grigio" Width="100%" AutoPostBack="true">
                                <asp:ListItem Value="XLS" Text="Excel" runat="server"></asp:ListItem>
                                <asp:ListItem Value="ODS" Text="Open Office Calc" runat="server"></asp:ListItem>
                            </asp:DropDownList>
                        </fieldset>
                    </asp:Panel>
                    <asp:Panel runat="server" ID="pnl_confirm">
                        <table id="Table3" align="center">
                            <tr>
                                <td align="center">
                                    <asp:Button ID="btn_conferma" runat="server" CssClass="cbtn" Text="Esporta" OnClick="btn_conferma_Click"></asp:Button>
                                </td>
                                <td>
                                    <asp:Button ID="btn_annulla" runat="server" CssClass="cbtn" Text="Chiudi"
                                    OnClientClick="window.close();"></asp:Button>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </div>
            </div>  
        </div>
    </form>
</body>
</html>
