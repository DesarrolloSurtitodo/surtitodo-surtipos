namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Models
{
    public sealed class SapLoginResponse
    {
        public string SessionId { get; set; } = string.Empty;

        public string Version { get; set; } = string.Empty;

        public int SessionTimeout { get; set; }
    }
}
