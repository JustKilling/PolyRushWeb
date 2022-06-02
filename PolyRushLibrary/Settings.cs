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
        //get api url by the api type
        public static string GetAPIBaseURL(ApiType type)
        {
            return type switch
            {
                ApiType.Cloud => CloudURI + "/api",
                ApiType.Local => LocalURI + "/api",
                _ => CloudURI + "/api"
            };
        }
        //gat domain url by type
        public static string GetBaseURL(ApiType type)
        {
            return type switch
            {
                ApiType.Cloud => CloudURI,
                ApiType.Local => LocalURI,
                _ => CloudURI
            };
        }


    }
}