#nullable disable

namespace MovieBoxBot.Models
{
    internal class PhotoMessageModel
    {
        public int Pages { get; set; }
        public List<PhotoMessage> PhotoMessages { get; set; }

        public PhotoMessageModel()
        {
            PhotoMessages = new List<PhotoMessage>();
        }
    }
}
