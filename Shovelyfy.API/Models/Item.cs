using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Swap.API.Models
{
    public class Item
    {
        public int ItemId { get; set; }
        public string Description { get; set; }
        public ItemValueLevel Value { get; set; }
        public ItemDealType DealType { get; set; }


        public virtual User User { get; set; }


        public enum ItemValueLevel
        {
            One = 1,
            Two = 2,
            Three = 3,
            Four = 4,
        }

        public enum ItemDealType
        {
            Default,
            Supermatch,
        }
    }
}