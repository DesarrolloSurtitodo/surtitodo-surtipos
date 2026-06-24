namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Common.Extensions
{
    public static class ExceptionExtensions
    {
        public static string GetRootMessage(this Exception ex)
        {
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }

            return ex.Message;
        }

        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            return value.Length <= maxLength
                ? value
                : value[..maxLength];
        }
    }
}