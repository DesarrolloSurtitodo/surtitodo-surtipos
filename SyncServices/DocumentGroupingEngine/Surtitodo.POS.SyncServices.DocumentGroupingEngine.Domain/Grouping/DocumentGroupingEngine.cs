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

            foreach (var doc in orderedDocuments)
            {
                // Si agregarlo rompería el límite...
                if (accumulated + doc.TITOT > maxGroupAmount)
                {
                    // ...pero el grupo aún está vacío, significa que este documento
                    // solo ya supera el límite → se agrupa solo como caso excepcional
                    if (members.Count == 0)
                    {
                        members.Add(doc);
                    }

                    // En ambos casos cerramos — sea grupo de uno o grupo acumulado
                    break;
                }

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
