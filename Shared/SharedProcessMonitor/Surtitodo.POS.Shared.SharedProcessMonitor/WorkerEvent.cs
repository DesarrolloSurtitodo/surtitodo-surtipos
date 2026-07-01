namespace Surtitodo.POS.Shared.SharedProcessMonitor;

public enum WorkerEventType { Info, Success, Error, Warning, Started, Paused, Stopped, Metrics }

public record WorkerEvent(
    WorkerEventType Type,
    string Message,
    DateTime Timestamp,
    string? NumAtCard = null,
    int? Procesados = null,
    int? Correctos = null,
    int? Errores = null,
    int? Pendientes = null);