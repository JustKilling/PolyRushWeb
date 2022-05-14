namespace PolyRushLibrary
{
    public enum ApiType
    {
        Local,
        Cloud
    }
    public class Settings
    {
        public static string LocalURI => @"https://localhost:5001";
        public static string CloudURI => @"https://polyrushapi.azurewebsites.net";
        public static string GetAPIBaseURL(ApiType type)
        {
            switch (type)
            {
                case ApiType.Cloud:
                    return CloudURI + "/api";
                case ApiType.Local:
                    return LocalURI + "/api";
                default:
                    return CloudURI + "/api";
            }
            
        }
        public static string GetBaseURL(ApiType type)
        {
            switch (type)
            {
                case ApiType.Cloud:
                    return CloudURI;
                case ApiType.Local:
                    return LocalURI;
                default:
                    return CloudURI;
            }

        }


    }
}