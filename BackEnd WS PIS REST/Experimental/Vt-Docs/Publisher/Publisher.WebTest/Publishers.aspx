<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Publishers.aspx.cs" Inherits="Publisher.WebTest.Publishers" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:LinkButton ID="btnBack" runat="server" Text="<< Indietro" OnClick="btnBack_OnClick"></asp:LinkButton>
        <br />
        <br />
        <asp:Label ID="lblAdminList" runat="server" Text="Amministrazione:"></asp:Label>
        <asp:DropDownList ID="cboAdminList" runat="server" Width="300px" AutoPostBack="true" OnSelectedIndexChanged="cboAdminList_OnSelectedIndexChanged"></asp:DropDownList>
        <br />
        <br />
        <asp:Panel ID="pnlInstances" runat="server" ScrollBars="Auto" Height="300px" BorderStyle="Solid">
        <asp:Label ID="lblInstances" runat="server" Font-Size="Large" Font-Bold="true" Text="Istanze di pubblicazione:"></asp:Label>
            <br />
            <br />
            <asp:LinkButton ID="btnNewInstance" runat="server" Text="Nuova istanza" OnClick="btnNewInstance_Click" />
            <asp:LinkButton ID="btnRefresh" runat="server" Text="Aggiorna" OnClick="btnRefresh_Click" />
            <br />        

            <asp:DataGrid ID="grdInstances" runat="server" AutoGenerateColumns="False" 
                Width="100%" CellPadding="4" ForeColor="#333333" GridLines="None" Font-Size="Smaller"
                onitemcommand="grdInstances_ItemCommand">
                <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                <Columns>
                    <asp:ButtonColumn CommandName="Select" Text="Seleziona" HeaderStyle-Width="5%" ItemStyle-VerticalAlign="Top"> </asp:ButtonColumn>
                    <asp:EditCommandColumn CancelText="Annulla" UpdateText="Salva" HeaderStyle-Width="5%" ItemStyle-VerticalAlign="Top"></asp:EditCommandColumn>
                    <asp:ButtonColumn CommandName="Delete" Text="Rimuovi" HeaderStyle-Width="5%" ItemStyle-VerticalAlign="Top"></asp:ButtonColumn>

                    <asp:TemplateColumn HeaderText="Id" Visible="false">
                        <ItemTemplate>
                            <asp:Label ID="lblId" runat="server" Text="<%#((Publisher.Proxy.ChannelRefInfo) Container.DataItem).Id%>"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>

                    <asp:TemplateColumn HeaderText="Sottorscrittore" HeaderStyle-Width="40%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Top">
                        <ItemTemplate>
                            <asp:Label ID="lblSubscriber" runat="server" Text="<%#((Publisher.Proxy.ChannelRefInfo) Container.DataItem).ChannelName%>"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:HiddenField ID="hdSubscriber" runat="server" value="<%#((Publisher.Proxy.ChannelRefInfo) Container.DataItem).ChannelName%>"></asp:HiddenField>
                            <asp:DropDownList ID="cboSubscribers" runat="server"></asp:DropDownList>
                        </EditItemTemplate>
                    </asp:TemplateColumn>

                    <asp:TemplateColumn HeaderText="Intervallo di escuzione (sec)" HeaderStyle-Width="10%" ItemStyle-VerticalAlign="Top">
                        <ItemTemplate>
                            <asp:Label ID="lblExecutionInterval" runat="server" 
                            Text="<%#this.GetExecutionInterval((Publisher.Proxy.ChannelRefInfo) Container.DataItem)%>" ></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtExecutionInterval" runat="server"
                            Text="<%#this.GetExecutionInterval((Publisher.Proxy.ChannelRefInfo) Container.DataItem)%>" Width="20px"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateColumn>

                    <asp:TemplateColumn HeaderText="Stato servizio" HeaderStyle-Width="5%" ItemStyle-VerticalAlign="Top">
                        <ItemTemplate>
                            <asp:Label ID="lblState" runat="server" 
                            Text="<%#this.GetServiceState((Publisher.Proxy.ChannelRefInfo) Container.DataItem)%>"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>

                    <asp:TemplateColumn HeaderText="Data di avvio" HeaderStyle-Width="5%" ItemStyle-VerticalAlign="Top">
                        <ItemTemplate>
                            <asp:Label ID="lblStartDate" runat="server" 
                            Text="<%#this.GetServiceStartDate((Publisher.Proxy.ChannelRefInfo) Container.DataItem)%>"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>

                    <asp:TemplateColumn  HeaderStyle-Width="10%" ItemStyle-VerticalAlign="Top">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnStart" runat="server" CommandName="Start" Text="Avvia" 
                                Visible="<%#((Publisher.Proxy.ChannelRefInfo) Container.DataItem).State == Publisher.Proxy.ChannelStateEnum.Stopped%>" />
                            
                            <asp:LinkButton ID="btnStop" runat="server" CommandName="Stop" Text="Ferma"
                                Visible="<%#((Publisher.Proxy.ChannelRefInfo) Container.DataItem).State == Publisher.Proxy.ChannelStateEnum.Started%>"/>

                        </ItemTemplate>
                    </asp:TemplateColumn>

                    <asp:TemplateColumn HeaderText="Pubbl. effettuate" HeaderStyle-Width="5%" ItemStyle-VerticalAlign="Top">
                        <ItemTemplate>
                            <asp:Label ID="lblExecutionCount" runat="server" 
                            Text="<%#((Publisher.Proxy.ChannelRefInfo) Container.DataItem).ExecutionCount%>"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>

                     <asp:TemplateColumn HeaderText="Oggetti pubbl." HeaderStyle-Width="5%" ItemStyle-VerticalAlign="Top">
                        <ItemTemplate>
                            <asp:Label ID="lblPublishedObjects" runat="server" 
                            Text="<%#((Publisher.Proxy.ChannelRefInfo) Container.DataItem).PublishedObjects%>"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>

                     <asp:TemplateColumn HeaderText="Data ultima pubbl. " HeaderStyle-Width="5%" ItemStyle-VerticalAlign="Top">
                        <ItemTemplate>
                            <asp:Label ID="lblLastExecutionDate" runat="server" 
                            Text="<%#((Publisher.Proxy.ChannelRefInfo) Container.DataItem).LastExecutionDate%>"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>

                     <asp:TemplateColumn HeaderText="Calcola dal" HeaderStyle-Width="5%" ItemStyle-VerticalAlign="Top">
                        <ItemTemplate>
                            Data: <asp:Label ID="lblStartLogDate" runat="server" Text="<%#((Publisher.Proxy.ChannelRefInfo) Container.DataItem).StartLogDate%>"></asp:Label>
                            <br />
                            Id: <asp:Label ID="lblLastLogId" runat="server" Text="<%#((Publisher.Proxy.ChannelRefInfo) Container.DataItem).LastLogId%>"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtStartLogDate" runat="server" 
                            Text="<%#((Publisher.Proxy.ChannelRefInfo) Container.DataItem).StartLogDate%>"></asp:TextBox>                        
                        </EditItemTemplate>
                    </asp:TemplateColumn>

                    
                </Columns>
                <EditItemStyle BackColor="#999999" />
                <FooterStyle BackColor="#5D7B9D" ForeColor="White" Font-Bold="True" />
                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            </asp:DataGrid>
        </asp:Panel>
        <br />     
        <br />
        <asp:Panel ID="pnlEvents" runat="server" ScrollBars="Auto" Height="200px"  BorderStyle="Solid">
            <asp:Label ID="Label1" runat="server" Font-Size="Large" Font-Bold="true" Text="Eventi monitorati:"></asp:Label>
            <br />
            <br />
            <asp:LinkButton ID="btnNewEvent" runat="server" Text="Nuovo evento" OnClick="btnNewEvent_Click"></asp:LinkButton>
            <asp:LinkButton ID="btnRefreshEvents" runat="server" Text="Aggiorna" OnClick="btnRefreshEvents_Click" />
            <br />

            <asp:DataGrid ID="grdEvents" runat="server" AutoGenerateColumns="False" 
                Width="100%" CellPadding="4" ForeColor="#333333" GridLines="None" Font-Size="Smaller"
                onitemcommand="grdEvents_ItemCommand">
                <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                <Columns>
                    <asp:ButtonColumn CommandName="Select" Text="Seleziona" HeaderStyle-Width="10%"></asp:ButtonColumn>
                    <asp:EditCommandColumn CancelText="Annulla" UpdateText="Salva" HeaderStyle-Width="10%"></asp:EditCommandColumn>
                    <asp:ButtonColumn CommandName="Delete" Text="Rimuovi" HeaderStyle-Width="10%"></asp:ButtonColumn>

                    <asp:TemplateColumn HeaderText="Id" Visible="false">
                        <ItemTemplate>
                            <asp:Label ID="lblId" runat="server" Text="<%#((Publisher.Proxy.EventInfo) Container.DataItem).Id%>"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>

                    <asp:TemplateColumn HeaderText="IdInstance" Visible="false">
                        <ItemTemplate>
                            <asp:Label ID="lblIdInstance" runat="server" Text="<%#((Publisher.Proxy.EventInfo) Container.DataItem).IdChannel%>"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>

                    <asp:TemplateColumn HeaderText="Tipo oggetto" HeaderStyle-Width="25%">
                        <ItemTemplate>
                            <asp:Label ID="lblObjectType" runat="server" 
                            Text="<%#((Publisher.Proxy.EventInfo) Container.DataItem).ObjectType%>"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtObjectType" runat="server"
                            Text="<%#((Publisher.Proxy.EventInfo) Container.DataItem).ObjectType%>"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateColumn>

                    <asp:TemplateColumn HeaderText="Nome profilo" HeaderStyle-Width="25%">
                        <ItemTemplate>
                            <asp:Label ID="lblObjectTemplateName" runat="server" 
                            Text="<%#((Publisher.Proxy.EventInfo) Container.DataItem).ObjectTemplateName%>"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtObjectTemplateName" runat="server"
                            Text="<%#((Publisher.Proxy.EventInfo) Container.DataItem).ObjectTemplateName%>"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateColumn>

                    <asp:TemplateColumn HeaderText="Evento" HeaderStyle-Width="25%">
                        <ItemTemplate>
                            <asp:Label ID="lblEventName" runat="server" 
                            Text="<%#((Publisher.Proxy.EventInfo) Container.DataItem).EventName%>"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtEventName" runat="server"
                            Text="<%#((Publisher.Proxy.EventInfo) Container.DataItem).EventName%>"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateColumn>

                    <asp:TemplateColumn HeaderText="Carica file" HeaderStyle-Width="5%" Visible="false">
                        <ItemTemplate>
                            <asp:Label ID="lblLoadFile" runat="server" 
                            Text="<%#((Publisher.Proxy.EventInfo) Container.DataItem).LoadFileIfDocumentType%>"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:CheckBox ID="chkLoadFile" runat="server"
                            Checked="<%#((Publisher.Proxy.EventInfo) Container.DataItem).LoadFileIfDocumentType%>"></asp:CheckBox>
                        </EditItemTemplate>
                    </asp:TemplateColumn>

                    <asp:TemplateColumn HeaderText="DataMapper" HeaderStyle-Width="20%" Visible="false">
                        <ItemTemplate>
                            <asp:Label ID="lblDataMapper" runat="server" 
                            Text="<%#((Publisher.Proxy.EventInfo) Container.DataItem).DataMapperFullClass%>"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtDataMapper" runat="server"
                            Text="<%#((Publisher.Proxy.EventInfo) Container.DataItem).DataMapperFullClass%>"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateColumn>
                    
                </Columns>
                <EditItemStyle BackColor="#999999" />
                <FooterStyle BackColor="#5D7B9D" ForeColor="White" Font-Bold="True" />
                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            </asp:DataGrid>        
        </asp:Panel> 
    </div>
    </form>
</body>
</html>