using System.Security.Cryptography;
using System.Text;

namespace Hurudza.Common.Utils.Helpers;

public static class Codes
{
    public static string GetRegistrationToken()
    {
        var random = new Random();

        var token = random.Next(100000, 999999);

        return token.ToString();
    }

    public static string GetAccountNumber()
    {
        var random = new Random();

        var token = random.Next(10000000, 99999999);

        return token.ToString();
    }

    public static string GetTransactionReference12()
    {
        var random = new Random();

        var token = random.Next(1000, 9999);

        var date = DateTime.Now;

        var reference = $"DW{token}{date.Hour:00}{date.Minute:00}{date.Second:00}";

        return reference;
    }

    public static string GetTransactionReference16()
    {
        var random = new Random();

        var token = GetUniqueKey(10); //random.NextInt64(10000000000, 99999999999);
        var two = random.Next(10, 99);

        var reference = $"{two}-DW-{token}";

        return reference;
    }

    public static string GetTransactionReference24()
    {
        var token = GetUniqueKey(21);

        var reference = $"DW-{token}";
        return reference;
    }

    public static string GetPassword()
    {
        int maxSize = 6;
        char[] chars = new char[62];
        string a;
        a = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        chars = a.ToCharArray();
        int size = maxSize;
        byte[] data = new byte[1];
        RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
        crypto.GetNonZeroBytes(data);
        size = maxSize;
        data = new byte[size];
        crypto.GetNonZeroBytes(data);
        StringBuilder result = new StringBuilder(size);
        foreach (byte b in data)
        {
            result.Append(chars[b % (chars.Length - 1)]);
        }

        return result.ToString();
    }

    public static string GetUniqueKey(int length = 12)
    {
        int maxSize = length;
        char[] chars = new char[62];
        string a;
        a = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        chars = a.ToCharArray();
        int size = maxSize;
        byte[] data = new byte[1];
        RNGCryptoServiceProvider crypto = new();
        crypto.GetNonZeroBytes(data);
        data = new byte[size];
        crypto.GetNonZeroBytes(data);
        StringBuilder result = new(size);
        foreach (byte b in data)
        {
            result.Append(chars[b % (chars.Length - 1)]);
        }
        return result.ToString();
    }
}
