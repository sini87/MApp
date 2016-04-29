using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MApp.Web.CustomLibraries
{
    /// <summary>
    /// http://stackoverflow.com/questions/8226514/best-place-to-set-currentculture-for-multilingual-asp-net-mvc-web-applications
    /// </summary>
    public class CultureAwareControllerActivator : IControllerActivator
    {
        public IController Create(RequestContext requestContext, Type controllerType)
        {
            //Get the {language} parameter in the RouteData
            string language = requestContext.RouteData.Values["language"] == null ?
                "tr" : requestContext.RouteData.Values["language"].ToString();

            //Get the culture info of the language code
            CultureInfo culture = CultureInfo.GetCultureInfo(language);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            return DependencyResolver.Current.GetService(controllerType) as IController;
        }
    }
}