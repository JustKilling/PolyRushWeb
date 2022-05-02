using PolyRushLibrary;

namespace PolyRushWeb.Models
{
    [Serializable]
    public class UserPlaytime
    {
        public UserDTO User { get; set; } = null!;
        public TimeSpan Playtime { get; set; }
    }
}
