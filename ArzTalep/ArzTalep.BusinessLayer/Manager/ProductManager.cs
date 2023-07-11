using ArzTalep.BL.Model;
using Aware.Data;
using Aware.Manager;
using Aware.Util.Log;
using Aware.BL.Model;

namespace ArzTalep.BL.Manager
{
    public class ProductManager : BaseManager<Product>, IProductManager
    {
        public ProductManager(IRepository<Product> repository, IAwareLogger logger) : base(repository, logger)
        {
        }

        protected override OperationResult<Product> OnBeforeUpdate(ref Product existing, Product model)
        {
            throw new System.NotImplementedException();
        }
    }
}
