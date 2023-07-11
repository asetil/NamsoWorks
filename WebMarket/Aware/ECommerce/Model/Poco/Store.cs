using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Aware.Regional.Model;
using Aware.Util;
using Aware.Util.Enums;

namespace Aware.ECommerce.Model
{
    public class Store : IEntity
    {
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string Description { get; set; }
        public virtual decimal MinOrderAmount { get; set; }
        public virtual string WorkTimeInfo { get; set; }
        public virtual string ImageInfo { get; set; }
        public virtual string RegionInfo { get; set; } // ',23,5,83,' şeklinde olacak ve ',83,' şeklinde arama için kullanılacak...
        public virtual int ParentID { get; set; } // zincir mağaza ve alt mağazalar vs için
        public virtual int CustomerID { get; set; }
        public virtual string Guid { get; set; } //senkronizasyon amaçlı kullanılacak
        public virtual bool AllowSocialShare { get; set; }
        public virtual Statuses Status { get; set; }


        [NotMapped]
        public virtual StoreStatisticModel Statistic { get; set; }

        [NotMapped]
        public virtual List<Region> ServiceRegions { get; set; }

        public virtual bool IsOpen
        {
            get
            {
                try
                {
                    if (string.IsNullOrEmpty(WorkTimeInfo)) return false;
                    var dayIndex = (int)DateTime.Now.DayOfWeek;
                    var info = WorkTimeInfo.Replace("[", "").Replace("]", "").Split(';')[dayIndex - 1];

                    var start = info.Split(':')[0].Int();
                    var finish = info.Split(':')[1].Int();
                    if (finish - start > 45)
                    {
                        return true;
                    }
                }
                catch (Exception)
                {

                }
                return false;
            }
        }
    }
}