using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Domain.Source;

namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Domain.Grouping
{
    /// <summary>
    /// Representa un grupo de documentos cuya suma de TITOT no supera 1.000.000.
    /// </summary>
    public class DocumentGroup
    {
        public string BOCODI { get; init; } = default!;
        public string CACODI { get; init; } = default!;
        public string TIPDOC { get; init; } = default!;

        /// <summary>Documentos miembro del grupo, en el orden en que fueron acumulados.</summary>
        public IReadOnlyList<Documents> Members { get; init; } = [];

        /// <summary>Suma total de TITOT de los miembros.</summary>
        public decimal TotalAmount => Members.Sum(d => d.TITOT);

        /// <summary>Lista de TICODI para usar en el IN de la query de líneas.</summary>
        public IEnumerable<int> MemberKeys => Members.Select(d => d.TICODI);
    }
}