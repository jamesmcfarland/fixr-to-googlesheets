public static class SanitiseHelper
{
    public static string Sanitise(this object str)
    {
        return ((string)str).ToString().ToLower().Trim();
    }

    public static bool IsNullOrEmpty(this string str)
    {
        bool whitespace = true;
        foreach (char c in str.ToCharArray())
        {
            if (c != ' ')
            {
                whitespace = false;
                break;
            }
        }
        return str == null || str.Length == 0 || whitespace;
    }
}