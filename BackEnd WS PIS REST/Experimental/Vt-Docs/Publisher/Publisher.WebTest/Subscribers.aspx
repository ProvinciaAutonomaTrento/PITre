
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Subscribers.aspx.cs" MaintainScrollPositionOnPostback="true" Inherits="Publisher.WebTest.Subscribers" %>

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
        <asp:Panel ID="pnlInstances" runat="server" ScrollBars="Auto" Height="300px" BorderStyle="Solid">
            <asp:Label ID="lblInstances" runat="server" Font-Size="Large" Font-Bold="true" Text="Istanze di pubblicazione:"></asp:Label>
            <br />
            <br />
            <asp:LinkButton ID="btnInsertInstance" runat="server" Text="Nuova istanza" OnClick="btnInsertInstance_Click"></asp:LinkButton>
            <asp:DataGrid ID="grdInstances" runat="server" AutoGenerateColumns="False" 
                onitemcommand="grdInstances_ItemCommand" Width="100%" CellPadding="4" Font-Size="Smaller"
                ForeColor="#333333" GridLines="None">
                <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                <Columns>
                    <asp:ButtonColumn CommandName="Select" Text="Seleziona"></asp:ButtonColumn>
                    <asp:EditCommandColumn CancelText="Annulla" UpdateText="Salva"></asp:EditCommandColumn>
                    <asp:ButtonColumn CommandName="Delete" Text="Rimuovi"></asp:ButtonColumn>

                    <asp:TemplateColumn HeaderText="Id" Visible="false">
                        <ItemTemplate>
                            <asp:Label ID="lblId" runat="server" Text="<%#((Subscriber.Proxy.ChannelInfo) Container.DataItem).Id%>"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>

                    <asp:TemplateColumn HeaderText="Nome" HeaderStyle-Width="10%">
                        <ItemTemplate>
                            <asp:Label ID="lblName" runat="server" 
                            Text="<%#((Subscriber.Proxy.ChannelInfo) Container.DataItem).Name%>"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtName" runat="server"
                            Text="<%#((Subscriber.Proxy.ChannelInfo) Container.DataItem).Name%>"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateColumn>

                    <asp:TemplateColumn HeaderText="Descrizione" HeaderStyle-Width="30%">
                        <ItemTemplate>
                            <asp:Label ID="lblDescription" runat="server" 
                            Text="<%#((Subscriber.Proxy.ChannelInfo) Container.DataItem).Description%>"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtDescription" runat="server"
                            Text="<%#((Subscriber.Proxy.ChannelInfo) Container.DataItem).Description%>"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateColumn>

                    <asp:TemplateColumn HeaderText="Host Smtp" HeaderStyle-Width="10%">
                        <ItemTemplate>
                            <asp:Label ID="lblSmtpHost" runat="server" 
                            Text="<%#((Subscriber.Proxy.ChannelInfo) Container.DataItem).SmtpHost%>"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtSmtpHost" runat="server"
                            Text="<%#((Subscriber.Proxy.ChannelInfo) Container.DataItem).SmtpHost%>"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateColumn>

                    <asp:TemplateColumn HeaderText="Porta Smtp" HeaderStyle-Width="10%">
                        <ItemTemplate>
                            <asp:Label ID="lblSmtpPort" runat="server" 
                            Text="<%#((Subscriber.Proxy.ChannelInfo) Container.DataItem).SmtpPort%>"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtSmtpPort" runat="server"
                            Text="<%#((Subscriber.Proxy.ChannelInfo) Container.DataItem).SmtpPort%>"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateColumn>

                    <asp:TemplateColumn HeaderText="Smtp in SSL" HeaderStyle-Width="10%">
                        <ItemTemplate>
                            <asp:Label ID="lblSmtpSsl" runat="server" 
                            Text='<%#(((Subscriber.Proxy.ChannelInfo) Container.DataItem).SmtpSsl ? "Sì" : "No")%>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:CheckBox ID="txtSmtpSsl" runat="server"
                            Checked="<%#((Subscriber.Proxy.ChannelInfo) Container.DataItem).SmtpSsl%>"></asp:CheckBox>
                        </EditItemTemplate>
                    </asp:TemplateColumn>

                    <asp:TemplateColumn HeaderText="UserName Smtp" HeaderStyle-Width="10%">
                        <ItemTemplate>
                            <asp:Label ID="lblSmtpUserName" runat="server" 
                            Text="<%#((Subscriber.Proxy.ChannelInfo) Container.DataItem).SmtpUserName%>"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtSmtpUserName" runat="server"
                            Text="<%#((Subscriber.Proxy.ChannelInfo) Container.DataItem).SmtpUserName%>"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateColumn>

                    <asp:TemplateColumn HeaderText="Password Smtp" HeaderStyle-Width="10%">
                        <ItemTemplate>
                            <asp:Label ID="lblSmtpPassword" runat="server" 
                            Text="--"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtSmtpPassword" runat="server" TextMode="Password"
                            Text="<%#((Subscriber.Proxy.ChannelInfo) Container.DataItem).SmtpPassword%>"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateColumn>

                    <asp:TemplateColumn HeaderText="Mail Smtp" HeaderStyle-Width="10%">
                        <ItemTemplate>
                            <asp:Label ID="lblSmtpMail" runat="server" 
                            Text="<%#((Subscriber.Proxy.ChannelInfo) Container.DataItem).SmtpMail%>"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtSmtpMail" runat="server"
                            Text="<%#((Subscriber.Proxy.ChannelInfo) Container.DataItem).SmtpMail%>"></asp:TextBox>
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
        <asp:Panel ID="pnlRules" runat="server" ScrollBars="Auto" Height="200px" BorderStyle="Solid">
            <asp:Label ID="lblRules" runat="server" Font-Size="Large" Font-Bold="true" Text="Regole di pubblicazione dell'istanza:"></asp:Label>
            <br />
            <br />
            <asp:LinkButton ID="btnInsertRule" runat="server" Text="Nuova regola" OnClick="btnInsertRule_Click"></asp:LinkButton>
            <asp:DataGrid ID="grdRules" runat="server" AutoGenerateColumns="False"  Font-Size="Smaller"
                onitemcommand="grdRules_ItemCommand" Width="100%" 
                CellPadding="4" ForeColor="#333333" GridLines="None" >
                <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                <Columns>
                    
                    <asp:ButtonColumn CommandName="Select" Text="Seleziona"></asp:ButtonColumn>

                    <asp:EditCommandColumn CancelText="Annulla" UpdateText="Salva">
                    </asp:EditCommandColumn>
                    <asp:ButtonColumn CommandName="Delete" Text="Rimuovi"></asp:ButtonColumn>

                    <asp:TemplateColumn HeaderText="Id" Visible="false">
                        <ItemTemplate>
                            <asp:Label ID="lblId" runat="server" 
                            Text="<%#((Subscriber.Proxy.RuleInfo) Container.DataItem).Id%>"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>

                    <asp:TemplateColumn HeaderText="IdInstance" Visible="false">
                        <ItemTemplate>
                            <asp:Label ID="lblIdInstance" runat="server" 
                            Text="<%#((Subscriber.Proxy.RuleInfo) Container.DataItem).IdInstance%>"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>

                    <asp:TemplateColumn HeaderText="Nome" HeaderStyle-Width="30%">
                        <ItemTemplate>
                            <asp:Label ID="lblRuleName" runat="server" 
                            Text="<%#((Subscriber.Proxy.RuleInfo) Container.DataItem).RuleName%>"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtRuleName" runat="server"
                            Text="<%#((Subscriber.Proxy.RuleInfo) Container.DataItem).RuleName%>"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateColumn>

                    <asp:TemplateColumn HeaderText="Descrizione" HeaderStyle-Width="40%">
                        <ItemTemplate>
                            <asp:Label ID="lblRuleDescription" runat="server" 
                            Text="<%#((Subscriber.Proxy.RuleInfo) Container.DataItem).RuleDescription%>"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtRuleDescription" runat="server"
                            Text="<%#((Subscriber.Proxy.RuleInfo) Container.DataItem).RuleDescription%>"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateColumn>

                    <asp:TemplateColumn HeaderText="Abilitata" HeaderStyle-Width="10%">
                        <ItemTemplate>
                            <asp:Label ID="lblRuleEnabled" runat="server" 
                            Text='<%#(((Subscriber.Proxy.RuleInfo) Container.DataItem).Enabled ? "Sì" : "No")%>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:CheckBox ID="chkRuleEnabled" runat="server"
                            Checked="<%#((Subscriber.Proxy.RuleInfo) Container.DataItem).Enabled%>"></asp:CheckBox>
                        </EditItemTemplate>
                    </asp:TemplateColumn>

                    <asp:TemplateColumn HeaderText="Ordinal" Visible="false">
                        <ItemTemplate>
                            <asp:Label ID="lblOrdinal" runat="server" 
                            Text="<%#((Subscriber.Proxy.RuleInfo) Container.DataItem).Ordinal%>"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>

                    <asp:TemplateColumn HeaderText="Classe" HeaderStyle-Width="20%">
                        <ItemTemplate>
                            <asp:Label ID="lblRuleClassFullName" runat="server" 
                            Text="<%#((Subscriber.Proxy.RuleInfo) Container.DataItem).RuleClassFullName%>"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtRuleClassFullName" runat="server"
                            Text="<%#((Subscriber.Proxy.RuleInfo) Container.DataItem).RuleClassFullName%>"></asp:TextBox>
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
        <asp:Panel ID="pblSubRules" runat="server" ScrollBars="Auto" Height="350px" BorderStyle="Solid">
            <asp:Label ID="lblSubRules" runat="server" Font-Size="Large" Font-Bold="true" Text="Sotto regole di pubblicazione:"></asp:Label>
            <br />
            <br />
            <asp:LinkButton ID="btnInsertSubRule" runat="server" Text="Nuova sotto regola" OnClick="btnInsertSubRule_Click"></asp:LinkButton>
            <asp:DataGrid ID="grdSubRules" runat="server" AutoGenerateColumns="False" 
                onitemcommand="grdSubRules_ItemCommand" Width="100%" CellPadding="4" Font-Size="Smaller"
                ForeColor="#333333" GridLines="None">
                <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                <Columns>
                    
                    <asp:ButtonColumn CommandName="Select" Text="Seleziona"></asp:ButtonColumn>

                    <asp:EditCommandColumn CancelText="Annulla" UpdateText="Salva">
                    </asp:EditCommandColumn>
                    <asp:ButtonColumn CommandName="Delete" Text="Rimuovi"></asp:ButtonColumn>

                    <asp:TemplateColumn HeaderText="Id" Visible="false">
                        <ItemTemplate>
                            <asp:Label ID="lblId" runat="server" 
                            Text="<%#((Subscriber.Proxy.SubRuleInfo) Container.DataItem).Id%>"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>

                    <asp:TemplateColumn HeaderText="IdParentRule" Visible="false">
                        <ItemTemplate>
                            <asp:Label ID="lblIdParentRule" runat="server" 
                            Text="<%#((Subscriber.Proxy.SubRuleInfo) Container.DataItem).IdParentRule%>"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>

                    <asp:TemplateColumn HeaderText="Nome" HeaderStyle-Width="30%">
                        <ItemTemplate>
                            <asp:Label ID="lblSubRuleName" runat="server" 
                            Text="<%#((Subscriber.Proxy.SubRuleInfo) Container.DataItem).SubRuleName%>"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtSubRuleName" runat="server"
                            Text="<%#((Subscriber.Proxy.SubRuleInfo) Container.DataItem).SubRuleName%>"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateColumn>

                    <asp:TemplateColumn HeaderText="Descrizione" HeaderStyle-Width="50%">
                        <ItemTemplate>
                            <asp:Label ID="lblRuleDescription" runat="server" 
                            Text="<%#((Subscriber.Proxy.SubRuleInfo) Container.DataItem).RuleDescription%>"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtRuleDescription" runat="server"
                            Text="<%#((Subscriber.Proxy.SubRuleInfo) Container.DataItem).RuleDescription%>"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateColumn>

                    <asp:TemplateColumn HeaderText="Abilitata" HeaderStyle-Width="20%">
                        <ItemTemplate>
                            <asp:Label ID="lblRuleEnabled" runat="server" 
                            Text='<%#(((Subscriber.Proxy.SubRuleInfo) Container.DataItem).Enabled ? "Sì" : "No")%>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:CheckBox ID="chkRuleEnabled" runat="server"
                            Checked="<%#((Subscriber.Proxy.SubRuleInfo) Container.DataItem).Enabled%>"></asp:CheckBox>
                        </EditItemTemplate>
                    </asp:TemplateColumn>

                    <asp:TemplateColumn HeaderText="Ordinal" Visible="false">
                        <ItemTemplate>
                            <asp:Label ID="lblOrdinal" runat="server" 
                            Text="<%#((Subscriber.Proxy.SubRuleInfo) Container.DataItem).Ordinal%>"></asp:Label>
                        </ItemTemplate>
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
        <asp:Panel ID="pnlRuleOptions" runat="server" ScrollBars="Auto" Height="200px" BorderStyle="Solid">
            <asp:Label ID="Label1" runat="server" Font-Size="Large" Font-Bold="true" Text="Opzioni della regola di pubblicazione:"></asp:Label>
            <br />
            <br />
            <asp:HiddenField ID="hdParentRuleId" runat="server" />
            <asp:HiddenField ID="hdIsSubRule" runat="server" />
            <asp:LinkButton ID="btnInsertRuleOption" runat="server" Text="Nuova opzione" OnClick="btnInsertRuleOption_Click"></asp:LinkButton>
            <asp:DataGrid ID="grdRuleOptions" runat="server" AutoGenerateColumns="False"  Font-Size="Smaller"
                onitemcommand="grdRuleOptions_ItemCommand" Width="100%" 
                CellPadding="4" ForeColor="#333333" GridLines="None">
                <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                <Columns>
                    <asp:EditCommandColumn UpdateText="Salva"> </asp:EditCommandColumn>
                    <asp:ButtonColumn CommandName="Delete" Text="Rimuovi"></asp:ButtonColumn>

                    <asp:TemplateColumn HeaderText="Nome" HeaderStyle-Width="50%">
                        <ItemTemplate>
                            <asp:Label ID="lblName" runat="server" 
                            Text="<%#((Subscriber.Proxy.NameValuePair) Container.DataItem).Name%>"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtName" runat="server"
                            Text="<%#((Subscriber.Proxy.NameValuePair) Container.DataItem).Name%>"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateColumn>

                    <asp:TemplateColumn HeaderText="Valore" HeaderStyle-Width="50%">
                        <ItemTemplate>
                            <asp:Label ID="lblValue" runat="server" 
                            Text="<%#((Subscriber.Proxy.NameValuePair) Container.DataItem).Value%>"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtValue" runat="server"
                            Text="<%#((Subscriber.Proxy.NameValuePair) Container.DataItem).Value%>"></asp:TextBox>
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
        <asp:Panel ID="pnlRulesHistory" runat="server" ScrollBars="Auto" Height="200px" BorderStyle="Solid">
            <asp:Label ID="lblRulesHistory" runat="server" Font-Size="Large" Font-Bold="true" Text="Pubblicazioni effettuate per la regola:"></asp:Label>
            <br />
            <br />
            <asp:DataGrid ID="grdRulesHistory" runat="server" AutoGenerateColumns="False" 
                Width="100%" onitemcommand="grdRulesHistory_ItemCommand" CellPadding="4" Font-Size="Smaller"
                ForeColor="#333333" GridLines="None">
                <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                <Columns>
                    
                    <asp:BoundColumn HeaderText="Id" DataField="Id" Visible="false"></asp:BoundColumn>
                    <asp:BoundColumn HeaderText="IdRule" DataField="IdRule" Visible="false"></asp:BoundColumn>

                    <asp:ButtonColumn CommandName="Select" Text="Seleziona"></asp:ButtonColumn>                    

                    <asp:TemplateColumn HeaderText="Id oggetto" HeaderStyle-Width="20%">
                        <ItemTemplate>
                            <asp:Label ID="lblIdObject" runat="server" 
                            Text="<%#((Subscriber.Proxy.RuleHistoryInfo) Container.DataItem).ObjectSnapshot.IdObject%>"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>

                    <asp:TemplateColumn HeaderText="Descrizione oggetto" HeaderStyle-Width="40%">
                        <ItemTemplate>
                            <asp:Label ID="lblObjectDescription" runat="server" 
                            Text="<%#((Subscriber.Proxy.RuleHistoryInfo) Container.DataItem).ObjectSnapshot.Description%>"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>

                    <asp:TemplateColumn HeaderText="Nome profilo" HeaderStyle-Width="20%">
                        <ItemTemplate>
                            <asp:Label ID="lblObjectTemplateName" runat="server" 
                            Text="<%#((Subscriber.Proxy.RuleHistoryInfo) Container.DataItem).ObjectSnapshot.TemplateName%>"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>

                    <asp:TemplateColumn HeaderText="Tipo oggetto" HeaderStyle-Width="20%">
                        <ItemTemplate>
                            <asp:Label ID="lblObjectType" runat="server" 
                            Text="<%#((Subscriber.Proxy.RuleHistoryInfo) Container.DataItem).ObjectSnapshot.ObjectType%>"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                                        
                    <asp:TemplateColumn HeaderText="Pubblicato">
                        <ItemTemplate>
                            <asp:Label ID="lblPublished" runat="server" 
                            Text='<%#(((Subscriber.Proxy.RuleHistoryInfo) Container.DataItem).Published ? "Sì" : "No")%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>

                    <asp:BoundColumn HeaderText="Data pubblicazione" DataField="PublishDate"></asp:BoundColumn>

                    

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
        <asp:Panel ID="pubPublishedObject" runat="server" ScrollBars="Auto" Height="200px" BorderStyle="Solid">
            <asp:Label ID="lblPublishedObject" runat="server" Font-Size="Large" Font-Bold="true" Text="Dettagli oggetto pubblicato:"></asp:Label>
            <br />
            <br />
            <asp:DataGrid ID="grdPublishedObject" runat="server" 
                AutoGenerateColumns="False" Width="100%" Font-Size="Smaller"
                CellPadding="4" ForeColor="#333333" GridLines="None" >
                <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                <Columns>
                    <asp:TemplateColumn HeaderText="Nome" HeaderStyle-Width="50%">
                        <ItemTemplate>
                            <asp:Label ID="lblName" runat="server" 
                            Text="<%#((Subscriber.Proxy.Property) Container.DataItem).Name%>"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Valore" HeaderStyle-Width="50%">
                        <ItemTemplate>
                            <asp:Label ID="lblValue" runat="server" 
                            Text="<%#((Subscriber.Proxy.Property) Container.DataItem).Value.ToString()%>"></asp:Label>
                        </ItemTemplate>
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
        <asp:Panel ID="Panel1" runat="server"  Height="200px" BorderStyle="Solid">
            <asp:Label ID="lblAppointmentAsText" runat="server" Font-Size="Large" Font-Bold="true" Text="Messaggio iCalendar:"></asp:Label>
            <br />
            <br />
            <asp:TextBox ID="txtAppointmentAsText" runat="server" ReadOnly="true" BorderStyle="None" TextMode="MultiLine" Rows="10" Width="100%"></asp:TextBox>
        </asp:Panel>
    </div>
    </form>
</body>
</html>
