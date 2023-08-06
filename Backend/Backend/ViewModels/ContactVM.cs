using Backend.Models;

namespace Backend.ViewModels
{
    public class ContactVM
    {
        public Contact? Contact { get; set; }
        public Dictionary<string, string>? Settings { get; set; }
    }
}
