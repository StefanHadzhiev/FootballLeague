namespace FootballLeague.Shared.Helpers
{
    public static class GuidHelper
    {
        public static Guid ValidateGuid(string guid, string entity)
        {
            if (!Guid.TryParse(guid, out Guid res))
            {
                throw new ArgumentException($"Provied {entity} ID was not a valid GUID.");
            }

            return res;
        }
    }
}
