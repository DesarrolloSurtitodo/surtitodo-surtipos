namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Configuration
{
    public class GroupingSettings
    {
        public const string SectionName = "GroupingSettings";

        public int CandidatesTopLimit { get; init; } = 50;
        public decimal MaxGroupAmount { get; init; } = 1_000_000m;
    }
}
