using System;
using System.Collections.Generic;
using Aware.Util;
using Nest;
using Aware.ECommerce.Enums;
using Aware.Util.Enums;

namespace Aware.Search.ElasticSearch.Model
{
    [ElasticsearchType(Name = "product", IdProperty = "ID")]
    public class ElasticProduct
    {
        [Number(NumberType.Integer)]
        public int ID { get; set; }

        [String(Analyzer = "turkish_custom")]
        public string Name { get; set; }

        [String(Analyzer = "turkish_custom")]
        public string ShortDescription { get; set; }

        [String(Analyzer = "turkish_custom")]
        public string Description { get; set; }

        [String(Analyzer = "turkish_custom")]
        public string Brand { get; set; }

        [Number(NumberType.Integer)]
        public int CategoryID { get; set; }

        [String(Analyzer = "turkish_custom")]
        public string PropertyInfo { get; set; } //JSON

        [String]
        public string ImageInfo { get; set; } //JSON

        [Number(NumberType.Byte)]
        public MeasureUnits Unit { get; set; }

        [String]
        public string Barcode { get; set; }

        [Nested]
        public List<ElasticItem> Items { get; set; }

        [Nested]
        public List<IDNamePair> Properties { get; set; }

        [Date]
        public DateTime DateModified { get; set; }

        [Number(NumberType.Double)]
        public decimal Rating { get; set; }

        [Number(NumberType.Integer)]
        public int CommentCount { get; set; }

        [Number(NumberType.Byte)]
        public Statuses Status { get; set; }
        
    }
}
