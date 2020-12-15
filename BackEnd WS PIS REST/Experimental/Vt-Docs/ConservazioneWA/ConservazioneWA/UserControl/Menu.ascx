<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Menu.ascx.cs" Inherits="ConservazioneWA.UserControl.Menu" %>
<div id="menutop" class="horizontalcssmenu">
    <asp:Panel runat="server" ID="MenuHome" Visible="false">
        <ul id="cssmenu1">
            <li class="sonoqui">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="HOME" Text="Home" runat="server"
                    ToolTip="Home"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="ISTANZE" Text="Istanze" runat="server"
                    ToolTip="Istanze"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="SUPPORTI" Text="Supporti"
                    runat="server" ToolTip="Supporti"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="DOCUMENTI" Text="Documenti"
                    runat="server" ToolTip="Documenti"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="FASCICOLI" Text="Fascicoli"
                    runat="server" ToolTip="Fascicoli"></asp:LinkButton></li>
            <li class="altro"><a href="#">Registro</a>
                <ul class="visibility">
                    <li class="visibility">
                        <asp:LinkButton OnClick="LinkAction_Click" CommandName="STAMPE" Text="Stampe" runat="server"
                            ToolTip="Stampe"></asp:LinkButton></li>
                    <li class="visibility">
                        <asp:LinkButton OnClick="LinkAction_Click" CommandName="REPORT" Text="Report" runat="server"
                            ToolTip="Report"></asp:LinkButton></li>
                </ul>
            </li>
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="LOG" Text="Log" runat="server"
                    ToolTip="Log"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="NOTIFICHE" Text="Notifiche"
                    runat="server" ToolTip="Notifiche"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton53" OnClick="LinkAction_Click" CommandName="ESIBIZIONE" Text="Esibizione"
                    runat="server" ToolTip="Esibizione"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton Text="Esci" runat="server" OnClick="Exit" ToolTip="Esci"></asp:LinkButton></li>
        </ul>
    </asp:Panel>
    <asp:Panel runat="server" ID="MenuIstanze" Visible="false">
        <ul id="cssmenu2">
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="HOME" Text="Home" runat="server"
                    ToolTip="Home"></asp:LinkButton></li>
            <li class="sonoqui">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="ISTANZE" Text="Istanze" runat="server"
                    ToolTip="Istanze"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="SUPPORTI" Text="Supporti"
                    runat="server" ToolTip="Supporti"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="DOCUMENTI" Text="Documenti"
                    runat="server" ToolTip="Documenti"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="FASCICOLI" Text="Fascicoli"
                    runat="server" ToolTip="Fascicoli"></asp:LinkButton></li>
            <li class="altro"><a href="#">Registro</a>
                <ul class="visibility">
                    <li class="visibility">
                        <asp:LinkButton ID="LinkButton1" OnClick="LinkAction_Click" CommandName="STAMPE"
                            Text="Stampe" runat="server" ToolTip="Stampe"></asp:LinkButton></li>
                    <li class="visibility">
                        <asp:LinkButton ID="LinkButton2" OnClick="LinkAction_Click" CommandName="REPORT"
                            Text="Report" runat="server" ToolTip="Report"></asp:LinkButton></li>
                </ul>
            </li>
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="LOG" Text="Log" runat="server"
                    ToolTip="Log"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="NOTIFICHE" Text="Notifiche"
                    runat="server" ToolTip="Notifiche"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton54" OnClick="LinkAction_Click" CommandName="ESIBIZIONE" Text="Esibizione"
                    runat="server" ToolTip="Esibizione"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton Text="Esci" runat="server" OnClick="Exit" ToolTip="Esci"></asp:LinkButton></li>
        </ul>
    </asp:Panel>
    <asp:Panel runat="server" ID="MenuSupporti" Visible="false">
        <ul id="cssmenu3">
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="HOME" Text="Home" runat="server"
                    ToolTip="Home"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="ISTANZE" Text="Istanze" runat="server"
                    ToolTip="Istanze"></asp:LinkButton></li>
            <li class="sonoqui">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="SUPPORTI" Text="Supporti"
                    runat="server" ToolTip="Supporti"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="DOCUMENTI" Text="Documenti"
                    runat="server" ToolTip="Documenti"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="FASCICOLI" Text="Fascicoli"
                    runat="server" ToolTip="Fascicoli"></asp:LinkButton></li>
            <li class="altro"><a href="#">Registro</a>
                <ul class="visibility">
                    <li class="visibility">
                        <asp:LinkButton ID="LinkButton3" OnClick="LinkAction_Click" CommandName="STAMPE"
                            Text="Stampe" runat="server" ToolTip="Stampe"></asp:LinkButton></li>
                    <li class="visibility">
                        <asp:LinkButton ID="LinkButton4" OnClick="LinkAction_Click" CommandName="REPORT"
                            Text="Report" runat="server" ToolTip="Report"></asp:LinkButton></li>
                </ul>
            </li>
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="LOG" Text="Log" runat="server"
                    ToolTip="Log"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="NOTIFICHE" Text="Notifiche"
                    runat="server" ToolTip="Notifiche"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton55" OnClick="LinkAction_Click" CommandName="ESIBIZIONE" Text="Esibizione"
                    runat="server" ToolTip="Esibizione"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton Text="Esci" runat="server" OnClick="Exit" ToolTip="Esci"></asp:LinkButton></li>
        </ul>
    </asp:Panel>
    <asp:Panel runat="server" ID="MenuDocumenti" Visible="false">
        <ul id="cssmenu4">
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="HOME" Text="Home" runat="server"
                    ToolTip="Home"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="ISTANZE" Text="Istanze" runat="server"
                    ToolTip="Istanze"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="SUPPORTI" Text="Supporti"
                    runat="server" ToolTip="Supporti"></asp:LinkButton></li>
            <li class="sonoqui">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="DOCUMENTI" Text="Documenti"
                    runat="server" ToolTip="Documenti"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="FASCICOLI" Text="Fascicoli"
                    runat="server" ToolTip="Fascicoli"></asp:LinkButton></li>
            <li class="altro"><a href="#">Registro</a>
                <ul class="visibility">
                    <li class="visibility">
                        <asp:LinkButton ID="LinkButton5" OnClick="LinkAction_Click" CommandName="STAMPE"
                            Text="Stampe" runat="server" ToolTip="Stampe"></asp:LinkButton></li>
                    <li class="visibility">
                        <asp:LinkButton ID="LinkButton6" OnClick="LinkAction_Click" CommandName="REPORT"
                            Text="Report" runat="server" ToolTip="Report"></asp:LinkButton></li>
                </ul>
            </li>
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="LOG" Text="Log" runat="server"
                    ToolTip="Log"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="NOTIFICHE" Text="Notifiche"
                    runat="server" ToolTip="Notifiche"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton56" OnClick="LinkAction_Click" CommandName="ESIBIZIONE" Text="Esibizione"
                    runat="server" ToolTip="Esibizione"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton Text="Esci" runat="server" OnClick="Exit" ToolTip="Esci"></asp:LinkButton></li>
        </ul>
    </asp:Panel>
    <asp:Panel runat="server" ID="MenuFascicoli" Visible="false">
        <ul id="cssmenu5">
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="HOME" Text="Home" runat="server"
                    ToolTip="Home"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="ISTANZE" Text="Istanze" runat="server"
                    ToolTip="Istanze"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="SUPPORTI" Text="Supporti"
                    runat="server" ToolTip="Supporti"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="DOCUMENTI" Text="Documenti"
                    runat="server" ToolTip="Documenti"></asp:LinkButton></li>
            <li class="sonoqui">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="FASCICOLI" Text="Fascicoli"
                    runat="server" ToolTip="Fascicoli"></asp:LinkButton></li>
            <li class="altro"><a href="#">Registro</a>
                <ul class="visibility">
                    <li class="visibility">
                        <asp:LinkButton ID="LinkButton7" OnClick="LinkAction_Click" CommandName="STAMPE"
                            Text="Stampe" runat="server" ToolTip="Stampe"></asp:LinkButton></li>
                    <li class="visibility">
                        <asp:LinkButton ID="LinkButton8" OnClick="LinkAction_Click" CommandName="REPORT"
                            Text="Report" runat="server" ToolTip="Report"></asp:LinkButton></li>
                </ul>
            </li>
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="LOG" Text="Log" runat="server"
                    ToolTip="Log"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="NOTIFICHE" Text="Notifiche"
                    runat="server" ToolTip="Notifiche"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton57" OnClick="LinkAction_Click" CommandName="ESIBIZIONE" Text="Esibizione"
                    runat="server" ToolTip="Esibizione"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton Text="Esci" runat="server" OnClick="Exit" ToolTip="Esci"></asp:LinkButton></li>
        </ul>
    </asp:Panel>
    <asp:Panel runat="server" ID="MenuRegistro" Visible="false">
        <ul id="cssmenu6">
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="HOME" Text="Home" runat="server"
                    ToolTip="Home"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="ISTANZE" Text="Istanze" runat="server"
                    ToolTip="Istanze"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="SUPPORTI" Text="Supporti"
                    runat="server" ToolTip="Supporti"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="DOCUMENTI" Text="Documenti"
                    runat="server" ToolTip="Documenti"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="FASCICOLI" Text="Fascicoli"
                    runat="server" ToolTip="Fascicoli"></asp:LinkButton></li>
            <li class="sonoqui"><a href="#">Registro</a>
                <ul class="visibility">
                    <li class="visibility">
                        <asp:LinkButton ID="LinkButton9" OnClick="LinkAction_Click" CommandName="STAMPE"
                            Text="Stampe" runat="server" ToolTip="Stampe"></asp:LinkButton></li>
                    <li class="visibility">
                        <asp:LinkButton ID="LinkButton10" OnClick="LinkAction_Click" CommandName="REPORT"
                            Text="Report" runat="server" ToolTip="Report"></asp:LinkButton></li>
                </ul>
            </li>
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="LOG" Text="Log" runat="server"
                    ToolTip="Log"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="NOTIFICHE" Text="Notifiche"
                    runat="server" ToolTip="Notifiche"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton58" OnClick="LinkAction_Click" CommandName="ESIBIZIONE" Text="Esibizione"
                    runat="server" ToolTip="Esibizione"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton Text="Esci" runat="server" OnClick="Exit" ToolTip="Esci"></asp:LinkButton></li>
        </ul>
    </asp:Panel>
    <asp:Panel runat="server" ID="MenuLog" Visible="false">
        <ul id="cssmenu7">
            <li class="altro">
                <asp:LinkButton ID="LinkButton13" OnClick="LinkAction_Click" CommandName="HOME" Text="Home"
                    runat="server" ToolTip="Home"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton14" OnClick="LinkAction_Click" CommandName="ISTANZE"
                    Text="Istanze" runat="server" ToolTip="Istanze"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton15" OnClick="LinkAction_Click" CommandName="SUPPORTI"
                    Text="Supporti" runat="server" ToolTip="Supporti"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton16" OnClick="LinkAction_Click" CommandName="DOCUMENTI"
                    Text="Documenti" runat="server" ToolTip="Documenti"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton17" OnClick="LinkAction_Click" CommandName="FASCICOLI"
                    Text="Fascicoli" runat="server" ToolTip="Fascicoli"></asp:LinkButton></li>
            <li class="altro"><a href="#">Registro</a>
                <ul class="visibility">
                    <li class="visibility">
                        <asp:LinkButton ID="LinkButton18" OnClick="LinkAction_Click" CommandName="STAMPE"
                            Text="Stampe" runat="server" ToolTip="Stampe"></asp:LinkButton></li>
                    <li class="visibility">
                        <asp:LinkButton ID="LinkButton19" OnClick="LinkAction_Click" CommandName="REPORT"
                            Text="Report" runat="server" ToolTip="Report"></asp:LinkButton></li>
                </ul>
            </li>
            <li class="sonoqui">
                <asp:LinkButton ID="LinkButton20" OnClick="LinkAction_Click" CommandName="LOG" Text="Log"
                    runat="server" ToolTip="Log"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton21" OnClick="LinkAction_Click" CommandName="NOTIFICHE"
                    Text="Notifiche" runat="server" ToolTip="Notifiche"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton59" OnClick="LinkAction_Click" CommandName="ESIBIZIONE" Text="Esibizione"
                    runat="server" ToolTip="Esibizione"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton22" Text="Esci" runat="server" OnClick="Exit" ToolTip="Esci"></asp:LinkButton></li>
        </ul>
    </asp:Panel>
    <asp:Panel runat="server" ID="MenuNotifiche" Visible="false">
        <ul id="cssmenu8">
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="HOME" Text="Home" runat="server"
                    ToolTip="Home"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="ISTANZE" Text="Istanze" runat="server"
                    ToolTip="Istanze"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="SUPPORTI" Text="Supporti"
                    runat="server" ToolTip="Supporti"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="DOCUMENTI" Text="Documenti"
                    runat="server" ToolTip="Documenti"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="FASCICOLI" Text="Fascicoli"
                    runat="server" ToolTip="Fascicoli"></asp:LinkButton></li>
            <li class="altro"><a href="#">Registro</a>
                <ul class="visibility">
                    <li class="visibility">
                        <asp:LinkButton ID="LinkButton11" OnClick="LinkAction_Click" CommandName="STAMPE"
                            Text="Stampe" runat="server" ToolTip="Stampe"></asp:LinkButton></li>
                    <li class="visibility">
                        <asp:LinkButton ID="LinkButton12" OnClick="LinkAction_Click" CommandName="REPORT"
                            Text="Report" runat="server" ToolTip="Report"></asp:LinkButton></li>
                </ul>
            </li>
            <li class="altro">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="LOG" Text="Log" runat="server"
                    ToolTip="Log"></asp:LinkButton></li>
            <li class="sonoqui">
                <asp:LinkButton OnClick="LinkAction_Click" CommandName="NOTIFICHE" Text="Notifiche"
                    runat="server" ToolTip="Notifiche"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton60" OnClick="LinkAction_Click" CommandName="ESIBIZIONE" Text="Esibizione"
                    runat="server" ToolTip="Esibizione"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton Text="Esci" runat="server" OnClick="Exit" ToolTip="Esci"></asp:LinkButton></li>
        </ul>
    </asp:Panel>
        <asp:Panel runat="server" ID="MenuEsibizione" Visible="false">
        <ul id="cssmenu9">
            <li class="altro">
                <asp:LinkButton ID="LinkButton61" OnClick="LinkAction_Click" CommandName="HOME" Text="Home" runat="server"
                    ToolTip="Home"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton62" OnClick="LinkAction_Click" CommandName="ISTANZE" Text="Istanze" runat="server"
                    ToolTip="Istanze"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton63" OnClick="LinkAction_Click" CommandName="SUPPORTI" Text="Supporti"
                    runat="server" ToolTip="Supporti"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton64" OnClick="LinkAction_Click" CommandName="DOCUMENTI" Text="Documenti"
                    runat="server" ToolTip="Documenti"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton65" OnClick="LinkAction_Click" CommandName="FASCICOLI" Text="Fascicoli"
                    runat="server" ToolTip="Fascicoli"></asp:LinkButton></li>
            <li class="altro"><a href="#">Registro</a>
                <ul class="visibility">
                    <li class="visibility">
                        <asp:LinkButton ID="LinkButton66" OnClick="LinkAction_Click" CommandName="STAMPE"
                            Text="Stampe" runat="server" ToolTip="Stampe"></asp:LinkButton></li>
                    <li class="visibility">
                        <asp:LinkButton ID="LinkButton67" OnClick="LinkAction_Click" CommandName="REPORT"
                            Text="Report" runat="server" ToolTip="Report"></asp:LinkButton></li>
                </ul>
            </li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton68" OnClick="LinkAction_Click" CommandName="LOG" Text="Log" runat="server"
                    ToolTip="Log"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton69" OnClick="LinkAction_Click" CommandName="NOTIFICHE" Text="Notifiche"
                    runat="server" ToolTip="Notifiche"></asp:LinkButton></li>
            <li class="sonoqui">
                <asp:LinkButton ID="LinkButton70" OnClick="LinkAction_Click" CommandName="ESIBIZIONE" Text="Esibizione"
                    runat="server" ToolTip="Esibizione"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton71" Text="Esci" runat="server" OnClick="Exit" ToolTip="Esci"></asp:LinkButton></li>
        </ul>
    </asp:Panel>

    <!-- Prova Voce di Menù Esibizione - sono tanti panel visibili o meno a seconda del caso-->
    <asp:Panel runat="server" ID="PnlHomeEsibizione" Visible="false">
        <ul id="Ul1">
            <li class="sonoqui">
                <asp:LinkButton ID="LinkButton23" OnClick="LinkAction_Click" CommandName="HOME_ESIBIZIONE" Text="Home" runat="server"
                    ToolTip="Home"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton31" OnClick="LinkAction_Click" CommandName="DOCUMENTI_ESIBIZIONE"
                    Text="Documenti" runat="server" ToolTip="Ricerca Documenti Esibizione"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton42" OnClick="LinkAction_Click" CommandName="FASCICOLI_ESIBIZIONE"
                    Text="Fascicoli" runat="server" ToolTip="Ricerca Fascicoli Esibizione"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton33" OnClick="LinkAction_Click" CommandName="ISTANZE_ESIBIZIONE"
                    Text="Istanze" runat="server" ToolTip="Istanze di Esibizione"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton43" OnClick="LinkAction_Click" CommandName="ESIBIZIONE"
                    Text="Esibizione" runat="server" ToolTip="Gestione Esibizione"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton32" Text="Esci" runat="server" OnClick="Exit" ToolTip="Esci"></asp:LinkButton></li>
        </ul>
    </asp:Panel>
    <asp:Panel runat="server" ID="PnlRicercaDocumentiEsibizione" Visible="false">
        <ul id="Ul3">
            <li class="altro">
                <asp:LinkButton ID="LinkButton27" OnClick="LinkAction_Click" CommandName="HOME_ESIBIZIONE" Text="Home" runat="server"
                    ToolTip="Home"></asp:LinkButton></li>
            <li class="sonoqui">
                <asp:LinkButton ID="LinkButton30" OnClick="LinkAction_Click" CommandName="DOCUMENTI_ESIBIZIONE"
                    Text="Documenti" runat="server" ToolTip="Ricerca Documenti Esibizione"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton41" OnClick="LinkAction_Click" CommandName="FASCICOLI_ESIBIZIONE"
                    Text="Fascicoli" runat="server" ToolTip="Ricerca Fascicoli Esibizione"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton28" OnClick="LinkAction_Click" CommandName="ISTANZE_ESIBIZIONE"
                    Text="Istanze" runat="server" ToolTip="Istanze di Esibizione"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton44" OnClick="LinkAction_Click" CommandName="ESIBIZIONE"
                    Text="Esibizione" runat="server" ToolTip="Gestione Esibizione"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton29" Text="Esci" runat="server" OnClick="Exit" ToolTip="Esci"></asp:LinkButton></li>
        </ul>
    </asp:Panel>
    <asp:Panel runat="server" ID="PnlRicercaFascicoliEsibizione" Visible="false">
        <ul id="Ul4">
            <li class="altro">
                <asp:LinkButton ID="LinkButton35" OnClick="LinkAction_Click" CommandName="HOME_ESIBIZIONE" Text="Home" runat="server"
                    ToolTip="Home"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton36" OnClick="LinkAction_Click" CommandName="DOCUMENTI_ESIBIZIONE"
                    Text="Documenti" runat="server" ToolTip="Ricerca Documenti Esibizione"></asp:LinkButton></li>
            <li class="sonoqui">
                <asp:LinkButton ID="LinkButton39" OnClick="LinkAction_Click" CommandName="FASCICOLI_ESIBIZIONE"
                    Text="Fascicoli" runat="server" ToolTip="Ricerca Fascicoli Esibizione"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton37" OnClick="LinkAction_Click" CommandName="ISTANZE_ESIBIZIONE"
                    Text="Istanze" runat="server" ToolTip="Istanze di Esibizione"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton45" OnClick="LinkAction_Click" CommandName="ESIBIZIONE"
                    Text="Esibizione" runat="server" ToolTip="Gestione Esibizione"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton38" Text="Esci" runat="server" OnClick="Exit" ToolTip="Esci"></asp:LinkButton></li>
        </ul>
    </asp:Panel>
    <asp:Panel runat="server" ID="PnlRicercaIstanzeEsibizione" Visible="false">
        <ul id="Ul2">
            <li class="altro">
                <asp:LinkButton ID="LinkButton24" OnClick="LinkAction_Click" CommandName="HOME_ESIBIZIONE" Text="Home" runat="server"
                    ToolTip="Home"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton34" OnClick="LinkAction_Click" CommandName="DOCUMENTI_ESIBIZIONE"
                    Text="Documenti" runat="server" ToolTip="Ricerca Documenti Esibizione"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton40" OnClick="LinkAction_Click" CommandName="FASCICOLI_ESIBIZIONE"
                    Text="Fascicoli" runat="server" ToolTip="Ricerca Fascicoli Esibizione"></asp:LinkButton></li>
            <li class="sonoqui">
                <asp:LinkButton ID="LinkButton25" OnClick="LinkAction_Click" CommandName="ISTANZE_ESIBIZIONE"
                    Text="Istanze" runat="server" ToolTip="Istanze di Esibizione"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton46" OnClick="LinkAction_Click" CommandName="ESIBIZIONE"
                    Text="Esibizione" runat="server" ToolTip="Gestione Esibizione"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton26" Text="Esci" runat="server" OnClick="Exit" ToolTip="Esci"></asp:LinkButton></li>
        </ul>
    </asp:Panel>
        <asp:Panel runat="server" ID="PnlGestioneEsibizione" Visible="false">
        <ul id="Ul5">
            <li class="altro">
                <asp:LinkButton ID="LinkButton47" OnClick="LinkAction_Click" CommandName="HOME_ESIBIZIONE" Text="Home" runat="server"
                    ToolTip="Home"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton48" OnClick="LinkAction_Click" CommandName="DOCUMENTI_ESIBIZIONE"
                    Text="Documenti" runat="server" ToolTip="Ricerca Documenti Esibizione"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton49" OnClick="LinkAction_Click" CommandName="FASCICOLI_ESIBIZIONE"
                    Text="Fascicoli" runat="server" ToolTip="Ricerca Fascicoli Esibizione"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton50" OnClick="LinkAction_Click" CommandName="ISTANZE_ESIBIZIONE"
                    Text="Istanze" runat="server" ToolTip="Istanze di Esibizione"></asp:LinkButton></li>
            <li class="sonoqui">
                <asp:LinkButton ID="LinkButton51" OnClick="LinkAction_Click" CommandName="ESIBIZIONE"
                    Text="Esibizione" runat="server" ToolTip="Gestione Esibizione"></asp:LinkButton></li>
            <li class="altro">
                <asp:LinkButton ID="LinkButton52" Text="Esci" runat="server" OnClick="Exit" ToolTip="Esci"></asp:LinkButton></li>
        </ul>
    </asp:Panel>
    <!-- End Prova Voce di Menù -->
</div>
