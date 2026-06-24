using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Domain.Source;

namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Domain.Grouping
{
    public static class DocumentGroupingEngine
    {
        /// <summary>
        /// Recibe los documentos YA ordenados (TIDATA, CACODI, TICODI) y construye
        /// el primer grupo válido. Se detiene en cuanto el siguiente doc superaría
        /// el límite, por eso retorna como máximo UN grupo por llamada.
        /// </summary>
        /// <param name="orderedDocuments">200 docs con AGROUP_STATUS_CODE = 'P', pre-ordenados.</param>
        /// <returns>El grupo formado, o null si ningún documento individual cabe.</returns>
        public static DocumentGroup? BuildNextGroup(IEnumerable<Documents> orderedDocuments, decimal maxGroupAmount)
        {
            var members = new List<Documents>();
            decimal accumulated = 0m;

            // El molde se fija con el primer documento acumulado
            string? mBocodi = null;
            string? mCacodi = null;
            string? mTipdoc = null;

            foreach (var doc in orderedDocuments)
            {
                // Si aún no hay molde, este doc lo define
                if (members.Count == 0)
                {
                    // Caso excepcional: el primer doc ya supera el límite → grupo de uno
                    if (doc.TITOT > maxGroupAmount)
                    {
                        members.Add(doc);
                        break;
                    }

                    mBocodi = doc.BOCODI;
                    mCacodi = doc.CACODI;
                    mTipdoc = doc.TIPDOC;

                    members.Add(doc);
                    accumulated += doc.TITOT;
                    continue;
                }

                // Solo candidatos que coincidan con el molde del grupo
                if (doc.BOCODI != mBocodi || doc.CACODI != mCacodi || doc.TIPDOC != mTipdoc)
                    continue;

                if (accumulated + doc.TITOT > maxGroupAmount)
                    break;

                members.Add(doc);
                accumulated += doc.TITOT;
            }

            if (members.Count == 0)
                return null;

            var first = members[0];
            return new DocumentGroup
            {
                BOCODI = first.BOCODI,
                CACODI = first.CACODI,
                TIPDOC = first.TIPDOC,
                Members = members.AsReadOnly()
            };
        }
    }
}
