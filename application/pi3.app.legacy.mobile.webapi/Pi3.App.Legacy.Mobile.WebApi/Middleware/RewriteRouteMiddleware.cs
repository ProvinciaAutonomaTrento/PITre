// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.Legacy.Mobile.Shared.Helpers;

namespace Pi3.App.Legacy.Mobile.WebApi.Middleware;

public class RewriteRouteMiddleware( 
    RequestDelegate next,
    PathResolutionService pathResolutionService )
{
    readonly RequestDelegate _next = next;
    readonly PathResolutionService _pathResolutionService = pathResolutionService;

    public async Task InvokeAsync( HttpContext context )
    {
        string? url = context.Request.Path.Value?.ToLowerInvariant();

        switch ( url )
        {
            case "/docinfo":
                //context.Request.Path = this.ResolvePath("Documents", "GetDocInfo") ?? url;
                context.Request.Path = "/api/Documents/Details";
                break;

            case "/fascinfo":
                //context.Request.Path = this.ResolvePath("Documents", "GetFascicoloInfo") ?? url;
                context.Request.Path = "/api/Documents/GetFascicoloInfo";
                break;

            case "/login":
                //context.Request.Path = this.ResolvePath("Authenticate", "Login") ?? url;
                context.Request.Path = "/api/Mobile/Login"; //this.ResolvePath("Mobile", "Login") ?? url;
                break;

            case "/rimuovinotifica":
                context.Request.Path = this.ResolvePath("Notifications", "Remove") ?? url;
                break;

            case "/listaistanze":
                //context.Request.Path = this.ResolvePath("AppSettings", "GetInstanceList") ?? url;
                context.Request.Path = "/api/AppSettings/Instance";
                //context.Request.Path = "/api/Mobile/ListaIstanze";
                break;

            case "/deleghe":
                //context.Request.Path = this.ResolvePath("Deleghe", "GetDeleghe") ?? url;
                context.Request.Path = "/api/Deleghe/Deleghe";
                break;

            case "/esercitadelega":
                //context.Request.Path = this.ResolvePath("Deleghe", "CreateDelega") ?? url;
                context.Request.Path = "/api/Mobile/EsercitaDelega";
                break;

            case "/delega":
                //context.Request.Path = this.ResolvePath("Deleghe", "CreateDelega") ?? url;
                context.Request.Path = "/api/Deleghe/Delega";
                break;

            case "/modellidelega":
                context.Request.Path = this.ResolvePath("Deleghe", "GetModelliDeleghe") ?? url;
                break;

            case "/revocadelega":
                //context.Request.Path = this.ResolvePath("Deleghe", "RevocaDeleghe") ?? url;
                context.Request.Path = "/api/Deleghe/RevocaDelega";
                break;

            case "/terminadelega":
                //context.Request.Path = this.ResolvePath("Deleghe", "RevocaDeleghe") ?? url;
                context.Request.Path = "/api/Mobile/TerminaDelega";
                break;

            case "/recuperapasswordotp":
                context.Request.Path = "/api/Mobile/RecuperaPasswordOTP";
                break;

            case "/update":
                context.Request.Path = "/api/Mobile/Update";
                break;

            case "/cambioruolo":
                context.Request.Path = "/api/Mobile/CambioRuolo";
                break;

            case "/logout":
                context.Request.Path = "/api/Mobile/LogOut";
                break;

            case "/delegacheckattiva":
                context.Request.Path = "/api/Mobile/DelegaCheckAttiva";
                break;

            case "/todolist":
                context.Request.Path = "/api/Mobile/TodoList";
                break;

            case "/listaammbyuser":
                context.Request.Path = "/api/Mobile/ListaAmmByUser";
                break;

            case "/librofirmaelements":
                context.Request.Path = "/api/LibroFirma/LibroFirmaElements";
                break;

            case "/ricerca":
                context.Request.Path = "/api/Ricerca/Ricerca";
                break;

            case "/accettarifiutatrasm":
                context.Request.Path = "/api/Trasmissioni/AccettaRifiutaTrasm";
                break;

            case "/adlaction":
                context.Request.Path = "/api/Documents/AdlAction";
                break;

            case "/smista":
                context.Request.Path = "/api/Trasmissioni/Smista";
                break;

            case "/listaragioni":
                context.Request.Path = "/api/Trasmissioni/ListaRagioni";
                break;

            case "/listamodellitrasmissione":
                context.Request.Path = "/api/Trasmissioni/ListaModelliTrasmissione";
                break;

            case "/listacorrispondenti":
                context.Request.Path = "/api/Trasmissioni/ListaCorrispondenti";
                break;

            case "/ricercautenti":
                context.Request.Path = "/api/Deleghe/RicercaUtenti";
                break;

            case "/file":
                context.Request.Path = "/api/Documents/File";
                break;

            case "/listapreferiti":
                context.Request.Path = "/api/Trasmissioni/ListaPreferiti";
                break;

            case "/preferito":
                context.Request.Path = "/api/Trasmissioni/Preferito";
                break;

            case "/condividi":
                context.Request.Path = "/api/Documents/Condividi";
                break;

            case "/fascricerca":
                context.Request.Path = "/api/Ricerca/FascRicerca";
                break;

            case "/documentocondiviso":
                context.Request.Path = "/api/Documents/DocumentoCondiviso";
                break;

            case "/listaruoliutente":
                context.Request.Path = "/api/Trasmissioni/ListaRuoliUtente";
                break;

            case "/richiestaotp":
                context.Request.Path = "/api/FirmaHSM/RichiestaOTP";
                break;

            case "/firmahsm":
                context.Request.Path = "/api/FirmaHSM/FirmaHSM";
                break;

            case "/trasmissionevista":
                context.Request.Path = "/api/Trasmissioni/TrasmissioneVista";
                break;

            case "/respingielementi":
                context.Request.Path = "/api/LibroFirma/RespingiElementi";
                break;

            case "/firmaelementi":
                context.Request.Path = "/api/LibroFirma/FirmaElementi";
                break;

            case "/cambiastatoelementimultipli":
                context.Request.Path = "/api/LibroFirma/CambiaStatoElementiMultipli";
                break;

            case "/firmaelementihsm":
                context.Request.Path = "/api/LibroFirma/FirmaElementiHSM";
                break;
        }

        await _next(context);
    }

    private string? ResolvePath(string controller, string action, string version = "v1.0" )
    {
        string? newPath = this._pathResolutionService.ResolvePath(controller, action);
        if ( !String.IsNullOrEmpty(newPath) )
        {
            /**
             * newPath =
             * */
            newPath = newPath.Replace("v{version:apiVersion}", version);
            newPath = $"/{newPath}";
        }
        return newPath;
    }
}
