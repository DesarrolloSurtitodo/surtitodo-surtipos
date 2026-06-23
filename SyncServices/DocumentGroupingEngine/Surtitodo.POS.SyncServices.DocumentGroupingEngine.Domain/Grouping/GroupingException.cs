namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Domain.Grouping
{
    public class GroupingException : Exception
    {
        public string NumAtCard { get; }
        public IEnumerable<int> MemberKeys { get; }
        public string Bocodi { get; }
        public string Cacodi { get; }
        public string Tipdoc { get; }

        public GroupingException(
            string message,
            string numAtCard,
            IEnumerable<int> memberKeys,
            string bocodi,
            string cacodi,
            string tipdoc,
            Exception inner)
            : base(message, inner)
        {
            NumAtCard = numAtCard;
            MemberKeys = memberKeys;
            Bocodi = bocodi;
            Cacodi = cacodi;
            Tipdoc = tipdoc;
        }
    }
}
