using System;

namespace PolyRushLibrary
{
    [Serializable]
    public class AuthenticationRequest
    {
        public string Password;
        public string Email;
    }

    [Serializable]
    public class AuthenticationRequestRegister : AuthenticationRequest
    {
        public string Avatar;
        public string Username;
        public string Firstname;
        public string Lastname;
    }

    [Serializable]
    public class RefreshRequest
    {
        public string RefreshToken;
    }
}