namespace Surtitodo.POS.Shared.SharedProcessMonitor;

public enum WorkerEventType { Info, Success, Error, Warning, Started, Paused, Stopped }

public record WorkerEvent(
    WorkerEventType Type,
    string Message,
    DateTime Timestamp,
    string? NumAtCard = null);