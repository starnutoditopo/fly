using System;

namespace Fly.Services;

public static class StringArraySerializationHelper
{
    private const char CSV_SEPARATOR = ';';
    public static string[] FromCsv(string csv)
    {
        if (string.IsNullOrWhiteSpace(csv))
        {
            return [];
        }
        string[] result = csv.Split(CSV_SEPARATOR, StringSplitOptions.RemoveEmptyEntries);
        return result;
    }

    public static string ToCsv(string[] items)
    {
        if (items == null)
        {
            return string.Empty;
        }
        string csv = string.Join(CSV_SEPARATOR, items);
        return csv;
    }
}
