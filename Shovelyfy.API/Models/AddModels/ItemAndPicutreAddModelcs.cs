using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Swap.API.Models.Item;

namespace Swap.API.Models.AddModels
{
    public class ItemAndPictureAddModel
    {
        public string Description { get; set; }
        public int Value { get; set; }
        public int DealType { get; set; }
        public string PictureData { get; set; }
        public string PictureExtention { get; set; }
        public string UserId { get; set; }
    }

    public class ItemWithPictures
    {
        public int ItemId { get; set; }
        public string Description { get; set; }
        public int Value { get; set; }
        public int DealType { get; set; }
        public string PictureData { get; set; }
        public int MatchCount { get; set; }
    }
}