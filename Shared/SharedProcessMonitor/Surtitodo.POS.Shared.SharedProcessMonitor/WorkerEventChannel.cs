using System.Threading.Channels;

namespace Surtitodo.POS.Shared.SharedProcessMonitor;

public sealed class WorkerEventChannel
{
    private readonly Channel<WorkerEvent> _channel =
        Channel.CreateUnbounded<WorkerEvent>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });

    public ChannelWriter<WorkerEvent> Writer => _channel.Writer;
    public ChannelReader<WorkerEvent> Reader => _channel.Reader;
}