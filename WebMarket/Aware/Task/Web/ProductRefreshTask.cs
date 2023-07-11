using System;
using System.Linq;
using Aware.ECommerce.Interface;
using Aware.Util;
using Aware.Util.Model;
using Aware.ECommerce.Enums;

namespace Aware.Task.Web
{
    public class ProductRefreshTask : BaseTask
    {
        private readonly IPropertyService _productInfoService;
        public ProductRefreshTask(IPropertyService productInfoService)
        {
            _productInfoService = productInfoService;
        }

        public override Result Execute()
        {
            try
            {
                int executionParam = 0;
                if (ExecutionParams != null && ExecutionParams.Any())
                {
                    executionParam = ExecutionParams.FirstOrDefault().Int();
                }

                var success = true;
                if (executionParam == 1)
                {
                    success = _productInfoService.RefreshProductImages();
                }
                else if (executionParam == 2)
                {
                    success = _productInfoService.RefreshProductProperties();
                }

                var result = success ? Result.Success(null,Resource.General_Success) : Result.Error(Resource.General_Error); 
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("ProductRefreshTask > Execute - Failed", ex);
                return Result.Error(ex.Message);
            }
        }

        public override TaskType Type
        {
            get { return TaskType.ProductRefresh; }
        }
    }
}
