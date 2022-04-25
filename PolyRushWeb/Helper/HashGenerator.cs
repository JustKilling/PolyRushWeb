using System;
using System.Security.Cryptography;
using PolyRushLibrary;

// static public class HashGenerator
// {
//     static public byte[] GetHash(string inputString)
//     {
//         using (HashAlgorithm algorithm = SHA256.Create())
//             return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
//     }
//
//     static public string GetHashString(string inputString)
//     {
//         StringBuilder sb = new StringBuilder();
//         foreach (byte b in GetHash(inputString))
//             sb.Append(b.ToString("X2"));
//
//         return sb.ToString();
//     }
// }
//
//
// static public class SaltGenerator
// {
//     static public string CreateRandomSalt()
//     {
//         byte[] saltBytes = new byte[16];
//
//         var rng = RandomNumberGenerator.Create();
//         rng.GetNonZeroBytes(saltBytes);
//         var saltText = BitConverter.ToString(saltBytes);
//
//         return saltText;
//     }
// }
//
public static class HashAndSaltHelper
{
    //extention method to provide the user with a salt and a hash.
    public static void ProvideSaltAndHash(this User user)
    {
        byte[] salt = GenerateSalt();
        user.Salt = Convert.ToBase64String(salt);
        user.Password = ComputeHash(user.Password, user.Salt);
    }

    //method to create a random salt
    private static byte[] GenerateSalt()
    {
        RandomNumberGenerator rng = RandomNumberGenerator.Create();
        byte[] salt = new byte[24];
        rng.GetBytes(salt);
        return salt;
    }

    //method for creating the hashed password
    public static string ComputeHash(string strPassword, string strSalt)
    {
        byte[] salt = Convert.FromBase64String(strSalt);
        using Rfc2898DeriveBytes hg = new(strPassword, salt);
        hg.IterationCount = 14010;
        byte[] bytes = hg.GetBytes(24);
        return Convert.ToBase64String(bytes);
    }
}