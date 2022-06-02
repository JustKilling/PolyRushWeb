using System;

namespace PolyRushLibrary
{
    //request to login
    [Serializable]
    public class AuthenticationRequest
    {
        public string Password;
        public string Email;
    }

    //request to register
    [Serializable]
    public class AuthenticationRequestRegister : AuthenticationRequest
    {
        public string Avatar;
        public string Username;
        public string Firstname;
        public string Lastname;
    }
}