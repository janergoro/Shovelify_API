using Swap.API.Models.AddModels;

namespace Swap.API.Controllers
{
    internal class ItemWithMatchedItemsViewModel : ItemWithPictures
    {
        public ItemWithPictures[] Matches { get; set; }
    }
}