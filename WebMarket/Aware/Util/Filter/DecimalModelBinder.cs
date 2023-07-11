using System;
using System.Configuration;
using System.Globalization;
using System.Web.Mvc;

namespace Aware.Util.Filter
{
    public class DecimalModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var modelState = new ModelState { Value = valueResult };
            object actualValue = null;
            
            try
            {
                if (!string.IsNullOrEmpty(valueResult.AttemptedValue))
                {
                    //<add key="DecimalCulture" value="tr-TR" />
                    string decimalCulture = Config.Value("DecimalCulture","tr-TR");
                    actualValue = Convert.ToDecimal(valueResult.AttemptedValue, CultureInfo.GetCultureInfo(decimalCulture));
                }
            }
            catch (FormatException e)
            {
                modelState.Errors.Add(e);
            }

            bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
            return actualValue;
        }
    }

    //public class DecimalModelBinder : DefaultModelBinder
    //{
    //    public override object BindModel(ControllerContext controllerContext,
    //                                     ModelBindingContext bindingContext)
    //    {
    //        object result = null;

    //        // Don't do this here!
    //        // It might do bindingContext.ModelState.AddModelError
    //        // and there is no RemoveModelError!
    //        // 
    //        // result = base.BindModel(controllerContext, bindingContext);

    //        string modelName = bindingContext.ModelName;
    //        string attemptedValue =
    //            bindingContext.ValueProvider.GetValue(modelName).AttemptedValue;

    //        // Depending on CultureInfo, the NumberDecimalSeparator can be "," or "."
    //        // Both "." and "," should be accepted, but aren't.
    //        string wantedSeperator = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
    //        string alternateSeperator = (wantedSeperator == "," ? "." : ",");

    //        if (attemptedValue.IndexOf(wantedSeperator) == -1 && attemptedValue.IndexOf(alternateSeperator) != -1)
    //        {
    //            attemptedValue = attemptedValue.Replace(alternateSeperator, wantedSeperator);
    //        }

    //        try
    //        {
    //            if (bindingContext.ModelMetadata.IsNullableValueType  && string.IsNullOrWhiteSpace(attemptedValue))
    //            {
    //                return null;
    //            }
    //            result = decimal.Parse(attemptedValue, NumberStyles.Currency);
    //        }
    //        catch (FormatException e)
    //        {
    //            bindingContext.ModelState.AddModelError(modelName, e);
    //        }

    //        return result;
    //    }
    //}
}
