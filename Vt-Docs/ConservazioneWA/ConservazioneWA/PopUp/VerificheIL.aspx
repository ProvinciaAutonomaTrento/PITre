<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VerificheIL.aspx.cs" Inherits="ConservazioneWA.PopUp.VerificheIL" %>

<%@ Register Src="~/UserControl/Calendar.ascx" TagName="Calendario" TagPrefix="uc1" %>
<%@ Register Src="~/ActivexWrappers/ShellWrapper.ascx" TagName="ShellWrapper" TagPrefix="uc2" %>
<%@ Register Src="~/ActivexWrappers/FsoWrapper.ascx" TagName="FsoWrapper" TagPrefix="uc3" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Verifiche</title>
    <link href="../CSS/Conservazione.css" type="text/css" rel="stylesheet" />
   
   <base target="_self" />

   <style type="text/css">
        #txt_note
        {
            height: 101px;
            width: 272px;
        }
        .style1
        {
            font-weight: bold;
            font-size: 10px;
            color: #666666;
            font-family: Verdana;
            height: 76px;
        }
        #TextArea1
        {
            height: 80px;
            width: 200px;
        }
        .testolabel
        {
            font-size:12px;
        }
        .cbtn
        {
            background-image: url('../Img/bg_button.jpg');
        }
        
        .cbtnHover
        {
            background-image: url('../Img/bg_button_hover.jpg');
        }
        
        .tab_istanze_header
        {
            background-image: url('../Img/bg_tab_header.jpg');
            background-repeat: repeat-x;
        }
        
        #content
        {
            background-image: url('../Img/bg_content.jpg');
        }
        
        .menu_pager_grigio
        {
            background-image: url('../Img/bg_pager_table.jpg');
            background-repeat: repeat-x;
        }
        
        #testoNote
        {
             background-image: url('../Img/bg_tab_header.jpg');
             background-repeat: repeat-x;
        }
       
    </style>
    <script language="javascript" type="text/javascript">
        function chk_1() {
            document.getElementById('lbl_file_da_aprire').enabled = false;

            document.getElementById('lbl_numero_file').enabled = true;
            document.getElementById('tb_num_file').enabled = true;
        }
        function chk_2() {
            document.getElementById('lbl_file_da_aprire').enabled = true;

            document.getElementById('lbl_numero_file').enabled = false;
            document.getElementById('tb_num_file').enabled = false;
        }
        function showVerificaLeggibilita(idConservazione, file, numero, idSupporto, note, dataProx) {
            var returnvalue = window.showModalDialog("VerificaLeggibilita.aspx?idCons=" + idConservazione + "&file=" + file + "&num=" + numero+"&idSupporto="+idSupporto+"&note="+note+"&dataProx="+dataProx, "", "dialogWidth:800px;dialogHeight:700px;status:no;resizable:no;scroll:no;center:yes;help:no");
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    
        <div align="center">
            <div id="testoNote">
                <asp:Label runat="server" ID="lb_intestazione" Text="Verifica i documenti"></asp:Label>
            </div>
        </div>
        <div>
           <p><asp:Label ID="lbl_intro" runat="server"  CssClass="testolabel" 
                   
                   Text="Per effettuare una verifica, selezionare una delle seguenti opzioni."></asp:Label></p> 
        </div>
        <asp:HiddenField ID="hd_idIstanza" runat="server" />
        <asp:HiddenField ID="hd_totDocs" runat="server" />
        <div align="left" style="margin:0 100px;">
        <table width="100%">
        <tr id="trDataProssimaVerifica" runat="server">
                    <td style="width: 40%; text-align: right">
                        <asp:Label runat="server" ID="lblDataProssimaVerifica" Text="Prossima verifica di integrità il: *" CssClass="testo_grigio3"></asp:Label>
                    </td>
                    <td style="width: 60%; text-align: left">
                        <uc1:Calendario runat="server" ID="txtDataProssimaVerifica" PAGINA_CHIAMANTE="VerificaSupporto" />
                    </td>
        </tr>
        <tr id="trDataProssimaVerificaLegg" runat="server">
            <td style="width: 40%; text-align: right">
                <asp:Label runat="server" ID="lblDataProssimaVerificaLegg" Text="Prossima verifica di leggibilità il: *" CssClass="testo_grigio3"></asp:Label>
            </td>
            <td style="width: 60; text-align: left">
                <uc1:Calendario runat="server" ID="txtDataProssimaVerificaLegg" PAGINA_CHIAMANTE="VerificaSupporto" />
            </td>
        </tr>
                <tr id="trNoteVerifica" runat="server">
                    <td style="width: 40%; text-align: right">
                        <asp:Label runat="server" ID="lblNoteDiVerifica" Text="Note di verifica:" CssClass="testo_grigio3"></asp:Label>
                    </td>
                    <td style="width: 60%; text-align: left">
                        <asp:TextBox runat="server" ID="txtNoteDiVerifica" CssClass="testo_grigio3" Width="70%"></asp:TextBox>
                    </td>
                </tr></table>
                <br />
        <table style="width:100%;">
            <tr>
            <td><asp:RadioButton ID="rbtn_numero_file" runat="server"  GroupName="radio1" 
                    CssClass="testolabel"  /> 
             </td>
             <td>
    <asp:Label ID="lbl_numero_file" runat="server" 
                     Text="Scegliere il numero di documenti da controllare:" 
                     CssClass="testolabel"></asp:Label>
             </td>
             <td>  
                    <asp:TextBox ID="tb_num_file" runat="server" Width="90px"></asp:TextBox>
                </td> </tr>
                <tr>
                <td><asp:RadioButton ID="rbtn_percent_file" runat="server" GroupName="radio1" CssClass="testolabel"/>
                </td>
                <td>
                    <asp:Label ID="lbl_percent_file" runat="server" CssClass="testolabel" 
                        Text="Scegliere la percentuale dei documenti da controllare:"></asp:Label>
                    </td>
                <td>
                    <asp:TextBox ID="tb_percent_file" runat="server" Width="58px"></asp:TextBox> <asp:Label ID="Label1" runat="server" CssClass="testolabel" >%</asp:Label>
                </td></tr>
            
                <tr><td>
                <asp:RadioButton ID="rbtn_file_da_aprire" runat="server" GroupName="radio1" 
                        CssClass="testolabel" 
                        /> </td>
                        <td>
                    <asp:Label ID="lbl_file_da_aprire" runat="server" 
                                Text="Scegliere i documenti da controllare:" CssClass="testolabel"></asp:Label>
                        </td>
                <td> <select id="sel_file_da_aprire" runat="server" visible="False">
                <option></option>
            </select></td></tr>
            
        </table>
        <br />
        
        </div>
        <div align="center" style="margin:25px 0;">
            <asp:Panel ID="Panel1" runat="server" ScrollBars="Vertical" Height="150px">
            
<asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
                EnableModelValidation="True" Width="590px">
    <Columns>
        <asp:BoundField HeaderText="ID Documento" DataField="docnumber" />
        <asp:BoundField HeaderText="Oggetto" DataField="desc" />
        <asp:BoundField HeaderText="Tipo File" DataField="tipo" />
        <asp:TemplateField>        
            <ItemTemplate>
<asp:CheckBox id="chk_doc" runat="server">
            </asp:CheckBox>
            </ItemTemplate> 
            
        
        </asp:TemplateField>
    </Columns>
            <HeaderStyle CssClass="tab_istanze_header" Font-Size="12px" 
        ForeColor="White" />
            </asp:GridView>
            </asp:Panel>
           
        </div>
        <div align="center">
        <asp:Button ID="btn_verifica_unificata" runat="server" Text="Verifica Unificata" 
                CssClass="cbtn" onclick="btn_verifica_unificata_Click" />
            <asp:Button ID="btn_verifica_integrita" runat="server"  Text="Verifica Integrità" 
                CssClass="cbtn" onclick="btn_verifica_integrita_Click"/>
            <asp:Button ID="btn_verifica" runat="server"  Text="Verifica Leggibilità" 
                CssClass="cbtn" onclick="btn_verifica_Click" />
            <asp:Button ID="btn_chiudi" runat="server" Text="Annulla" CssClass="cbtn" 
                onclick="btn_chiudi_Click"  />
        </div>
    
    </form>
</body>
</html>
