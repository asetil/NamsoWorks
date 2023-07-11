using System;
using Aware.Util;
using Nest;
using Aware.Util.Enums;

namespace Aware.Search.ElasticSearch.Model
{
    [ElasticsearchType(Name = "item", IdProperty = "ID")]
    public class ElasticItem
    {
        [Number(NumberType.Integer)]
        public int ID { get; set; }

        [Number(NumberType.Integer)]
        public int StoreID { get; set; }

        [Number(NumberType.Integer)]
        public int ProductID { get; set; }

        [Number(NumberType.Double)]
        public decimal ListPrice { get; set; }

        [Number(NumberType.Double)]
        public decimal SalesPrice { get; set; }

        [Number(NumberType.Double)]
        public decimal Stock { get; set; }

        [Boolean]
        public bool IsForSale { get; set; }

        [Boolean]
        public bool HasVariant { get; set; }

        [Date]
        public DateTime DateModified { get; set; }

        [Number(NumberType.Byte)]
        public Statuses Status { get; set; }
    }
}