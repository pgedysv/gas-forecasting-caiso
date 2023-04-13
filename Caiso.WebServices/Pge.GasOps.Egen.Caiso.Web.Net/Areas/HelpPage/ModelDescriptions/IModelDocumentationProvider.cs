using System;
using System.Reflection;

namespace Pge.GasOps.Egen.Caiso.Web.Net.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}