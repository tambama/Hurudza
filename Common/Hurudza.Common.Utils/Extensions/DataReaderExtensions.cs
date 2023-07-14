using System.Data;

namespace Hurudza.Common.Utils.Extensions
{
    public static class DataReaderExtensions
    {
        public static string? SafeGetString(this IDataRecord reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
                return reader.GetString(colIndex);
            return null;
        }

        public static int? SafeGetInt32(this IDataRecord reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
                return reader.GetInt32(colIndex);
            return null;
        }

        public static DateTime? SafeGetDateTime(this IDataRecord reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
                return reader.GetDateTime(colIndex);
            return null;
        }

        public static decimal? SafeGetDecimal(this IDataRecord reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
                return reader.GetDecimal(colIndex);
            return null;
        }
    }
}
