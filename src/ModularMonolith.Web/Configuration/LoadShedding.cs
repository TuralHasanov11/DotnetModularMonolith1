using Farfetch.LoadShedding.Events.Args;

namespace ModularMonolith.Web.Configuration;


public static class LoadShedding
{
    public static void SubscribeToItemEnqueued(ItemEnqueuedEventArgs eventArgs)
    {
        Console.WriteLine($"QueueLimit: {eventArgs.QueueLimit}, QueueCount: {eventArgs.QueueCount}");
    }

    public static void SubscribeToItemDequeued(ItemDequeuedEventArgs eventArgs)
    {
        Console.WriteLine($"QueueLimit: {eventArgs.QueueLimit}, QueueCount: {eventArgs.QueueCount}");
    }

    public static void SubscribeToItemProcessing(ItemProcessingEventArgs eventArgs)
    {
        Console.WriteLine($"ConcurrencyLimit: {eventArgs.ConcurrencyLimit}, ConcurrencyItems: {eventArgs.ConcurrencyCount}");
    }

    public static void SubscribeToItemProcessed(ItemProcessedEventArgs eventArgs)
    {
        Console.WriteLine($"ConcurrencyLimit: {eventArgs.ConcurrencyLimit}, ConcurrencyItems: {eventArgs.ConcurrencyCount}");
    }

    public static void SubscribeToRejected(ItemRejectedEventArgs eventArgs)
    {
        Console.Error.WriteLine($"Item rejected with Priority: {eventArgs.Priority}");
    }
}
