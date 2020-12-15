using System.Web.Compilation;
using System;
using System.CodeDom;
using NttDataWA.Utils;

public class LocalizationExpressionBuilder : ExpressionBuilder
{
    public override CodeExpression GetCodeExpression(System.Web.UI.BoundPropertyEntry entry, object parsedData, ExpressionBuilderContext context)
    {
        try
        {
            CodeExpression[] inputParams = new CodeExpression[] { new CodePrimitiveExpression(entry.Expression.Trim()), 
                                                    new CodeTypeOfExpression(entry.DeclaringType), 
                                                    new CodePrimitiveExpression(entry.PropertyInfo.Name) };

            // Return a CodeMethodInvokeExpression that will invoke the GetRequestedValue method using the specified input parameters 
            return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(this.GetType()),
                                        "GetRequestedValue",
                                        inputParams);
        }
        catch (System.Exception ex)
        {
            NttDataWA.UIManager.AdministrationManager.DiagnosticError(ex);
            return null;
        }
    }

    public static object GetRequestedValue(string key, Type targetType, string propertyName)
    {
        try
        {
            // If we reach here, no type mismatch - return the value 
            return GetByText(key);
        }
        catch (System.Exception ex)
        {
            NttDataWA.UIManager.AdministrationManager.DiagnosticError(ex);
            return null;
        }
    }

    //Place holder until database is build
    public static string GetByText(string text)
    {
        try
        {
            string language = NttDataWA.UIManager.UserManager.GetUserLanguage();
            return Languages.GetLabelFromCode(text, language);
        }
        catch (System.Exception ex)
        {
            NttDataWA.UIManager.AdministrationManager.DiagnosticError(ex);
            return null;
        }

    }
}