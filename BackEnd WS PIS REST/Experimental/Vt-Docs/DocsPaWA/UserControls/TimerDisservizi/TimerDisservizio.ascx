<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimerDisservizio.ascx.cs" Inherits="DocsPAWA.UserControls.TimerDisservizi.TimerDisservizio" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
 
    
    <style>
        .modalPopup 
        {
	        background-color:#DDDDDD;
	        border-width:2px;
	        border-style:solid;
	        border-color:Gray;
	        padding:3px;
	        width:350px;
        }
        .modalBackground
        {
            background-color: Gray;
            filter: alpha(opacity=50);
            opacity: 0.5;
        }
        
        .tabAlert
        {
            margin: 0px;
            padding: 0px;
            border-collapse: collapse;
            border-top: 1px solid #666666;
            border-bottom: 1px solid #666666;
            text-align: center;
            background-color: #ffffff;
        }
    </style>  
    <script type="text/javascript" language="javascript">

        function btnOK_OnClientClick() {
            $find("myBehavior1").hide();

            var http = new ActiveXObject("MSXML2.XMLHTTP");
            var url = "<%=this.AccettaDisservizioPageUrl%>";
            http.Open("POST", url, false);
            http.send();
            
            return false;
        }

    </script>
    <link href="../../CSS/docspa_30.css" type="text/css" rel="stylesheet" />  
    <asp:ScriptManagerProxy ID="ScriptManagerProxy" runat="server" />
    <asp:Timer runat="server" id="UpdateTimer" interval="60000" ontick="UpdateTimer_Tick" />
     

    <asp:UpdatePanel runat="server" id="TimedPanel" updatemode="Conditional">
        <Triggers>
            <asp:AsyncPostBackTrigger controlid="UpdateTimer" eventname="Tick" />
        </Triggers>
    </asp:UpdatePanel>


 <asp:UpdatePanel ID="udpMsj" runat="server" UpdateMode="Conditional" RenderMode="Inline" >
    <ContentTemplate>
        <asp:Button ID="btnD" runat="server" Text="" Style="display: none" Width="0" Height="0" />
        <asp:Panel id="divMsg" runat="server" CssClass="modalPopup" style="display: none; font-weight: bold; font-family: Arial;" >
            <asp:Panel ID="pnlMsgHD" runat="server" style=" text-align:center; color:Red; text-decoration:underline;"  >
                &nbsp;Avviso
            </asp:Panel>
            <br />
            <table class="tabAlert" width="100%">
            
            <tr>
                <td align="justify">
                    <asp:UpdatePanel ID="upMessage" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Literal ID="ltlMessage" runat="server">Messaggio</asp:Literal>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td style="height: 3px;">
                </td>
            </tr>
            <tr>
                <td style="height: 3px;">
                </td>
            </tr>
            <tr>
                <td align="center">
                    <%--<asp:Button ID="btnOK" runat="server" Text="OK" Width="60px" OnClientClick="return btnOK_OnClientClick();" />--%>
                </td>
            </tr>
        </table>
        <br />
        <div style="text-align:center;">
        <asp:Button ID="btnOK" runat="server" CssClass="pulsante"  Text="OK" Width="60px" OnClientClick="return btnOK_OnClientClick();" />
        </div>
        </asp:Panel>
        
        <cc1:ModalPopupExtender ID="mpeMsg" runat="server" TargetControlID="btnD"
        BehaviorID="myBehavior1"
            PopupControlID="divMsg" PopupDragHandleControlID="pnlMsgHD" BackgroundCssClass="modalBackground" 
            DropShadow="true" OkControlID="btnOK" >
        </cc1:ModalPopupExtender>
    </ContentTemplate>
</asp:UpdatePanel>
  
  

