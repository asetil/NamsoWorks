using System;
using System.Configuration;
using System.Globalization;
using System.Web.Mvc;

namespace Aware.Util.Filter
{
    public class DateTimeModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var modelState = new ModelState { Value = valueResult };
            DateTime? actualValue = null;

            try
            {
                //<add key="DateTimeCulture" value="tr-TR" />
                var dateTimeCulture = Config.Value("DateTimeCulture", "tr-TR");
                actualValue = Convert.ToDateTime(valueResult.AttemptedValue, CultureInfo.GetCultureInfo(dateTimeCulture));
            }
            catch (FormatException e)
            {
                modelState.Errors.Add(e);
            }

            bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
            return actualValue;
        }
    }
}