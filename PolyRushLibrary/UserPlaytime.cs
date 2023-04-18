using PolyRushLibrary;

namespace PolyRushWeb.Models
{
    [Serializable]
    public class UserPlaytime
    {
        public UserDTO User;
        public TimeSpan Playtime;
    }
}
