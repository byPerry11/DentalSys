using System;

namespace UnitTest.Services
{
    public static class UtilsTestService
    {
        public static string RandomString(int length = 8)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            var random = new Random();
            var result = "";

            for (int i = 0; i < length; i++)
                result += chars[random.Next(chars.Length)];

            return result;
        }

        public static string RandomEmail()
        {
            return $"{RandomString(6)}@test.com";
        }

        public static string RandomPhone()
        {
            var random = new Random();
            return "55" + random.Next(10000000, 99999999);
        }
    }
}