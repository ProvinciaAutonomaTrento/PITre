using System;
class NttDataWAException : Exception
{

    public NttDataWAException()
    {
        try
        {
            base.GetBaseException();
        }
        catch (System.Exception ex)
        {
            NttDataWA.UIManager.AdministrationManager.DiagnosticError(ex);
        }
    }

    public NttDataWAException(string auxMessage, Exception inner, string source)
        : base(String.Format(auxMessage), inner)
    {
        try
        {
            this.Source = source;
        }
        catch (System.Exception ex)
        {
            NttDataWA.UIManager.AdministrationManager.DiagnosticError(ex);            
        }
    }

    public NttDataWAException(string auxMessage, string source)
        : base(String.Format(auxMessage))
    {
        try
        {
            this.Source = source;
        }
        catch (System.Exception ex)
        {
            NttDataWA.UIManager.AdministrationManager.DiagnosticError(ex);
        }
    }
}