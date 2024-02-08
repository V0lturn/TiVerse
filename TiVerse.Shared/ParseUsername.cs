namespace TiVerse.Shared
{
    public static class ParseUsername
    {
        public static string? GetName(string login)
        {
            if (login != null && login.Contains('@'))
            {
                var parts = login.Split('@');
                return parts[0];
            }

            return login;
        }
    }
}