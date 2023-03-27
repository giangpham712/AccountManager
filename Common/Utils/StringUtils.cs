using System;

namespace AccountManager.Common.Utils
{
    public static class StringUtils
    {
        public static string GeneratePassword(int length)
        {
            // Create a string of characters, numbers, special characters that allowed in the password  
            var validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();

            // Select one random character at a time from the string  
            // and create an array of chars  
            var chars = new char[length];
            for (var i = 0; i < length; i++) chars[i] = validChars[random.Next(0, validChars.Length)];
            return new string(chars);
        }
    }
}