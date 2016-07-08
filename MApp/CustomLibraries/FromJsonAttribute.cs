using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace MApp.Web.CustomLibraries
{
    /// <summary>
    /// this CustomModelBinderAttribute class is used to bind the posted knockout-json from the view
    /// to the regarding object
    /// http://www.c-sharpcorner.com/UploadFile/5ff76e/posting-data-to-mvc-action-using-knockoutjs/
    /// </summary>
    public class FromJsonAttribute : CustomModelBinderAttribute
    {
        private readonly static JavaScriptSerializer serialier = new JavaScriptSerializer();
        public override IModelBinder GetBinder()
        {
            return new JsonModelBinder();
        }

        private class JsonModelBinder : IModelBinder
        {
            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                var stringfield = controllerContext.HttpContext.Request[char.ToUpper(bindingContext.ModelName[0]) + bindingContext.ModelName.Substring(1)];
                //var stringfield = controllerContext.HttpContext.Request[bindingContext.ModelName];
                if (string.IsNullOrEmpty(stringfield))
                    return null;
                object ret = null;
                try
                {
                    ret = serialier.Deserialize(stringfield, bindingContext.ModelType);
                }
                catch (System.Reflection.AmbiguousMatchException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine(ex);
                }catch (System.FormatException ex)
                {
                    Console.WriteLine(ex);
                }
                return ret;
            }
        }
    }
}