using dotnet_etcd;
using Etcdserverpb;
using Google.Protobuf;
using Grpc.Core;

namespace POCTemplate;

internal static class Program
{
    public static async Task Main()
    {
        var client = new EtcdClient(
            "http://localhost:2379",
            configureChannelOptions: options => options.Credentials = ChannelCredentials.Insecure
        );

        Console.WriteLine("=== etcd PoC with .NET ===");
        Console.WriteLine();

        await DemoBasicCrud(client);
        await DemoWatch(client);
        await DemoTransaction(client);

        Console.WriteLine("Done.");
    }

    private static async Task DemoBasicCrud(EtcdClient client)
    {
        Console.WriteLine("--- Basic CRUD ---");

        await client.PutAsync("name", "Guilherme");
        Console.WriteLine("PUT name = Guilherme");

        var value = await client.GetValAsync("name");
        Console.WriteLine($"GET name = {value}");

        await client.PutAsync("name", "Updated");
        var updated = await client.GetValAsync("name");
        Console.WriteLine($"GET name (after update) = {updated}");

        await client.DeleteAsync("name");
        var deleted = await client.GetValAsync("name");
        Console.WriteLine(
            $"GET name (after delete) = \"{deleted}\" (empty string means key not found)"
        );

        Console.WriteLine();
    }

    private static async Task DemoWatch(EtcdClient client)
    {
        Console.WriteLine("--- Watch ---");

        _ = client.WatchAsync(
            "dog",
            (WatchEvent[] events) =>
            {
                if (events.Length == 0)
                    return;
                foreach (var e in events)
                    Console.WriteLine($"  Watch event: {e.Key} -> \"{e.Value}\" ({e.Type})");
            }
        );

        await client.PutAsync("dog", "sits");
        await client.PutAsync("dog", "runs");
        await client.DeleteAsync("dog");

        await Task.Delay(200);
        Console.WriteLine();
    }

    private static async Task DemoTransaction(EtcdClient client)
    {
        Console.WriteLine("--- Transaction (atomic multi-key write) ---");

        var watchRequest = new WatchRequest
        {
            CreateRequest = new WatchCreateRequest
            {
                Key = ByteString.CopyFromUtf8("animals/"),
                RangeEnd = ByteString.CopyFromUtf8("animals0"),
            },
        };

        _ = client.WatchAsync(
            watchRequest,
            (WatchEvent[] events) =>
            {
                if (events.Length == 0)
                    return;
                Console.WriteLine($"  Watch received {events.Length} event(s) in one response:");
                foreach (var e in events)
                    Console.WriteLine($"    {e.Key} -> \"{e.Value}\" ({e.Type})");
            }
        );

        var txn = new TxnRequest();
        txn.Success.AddRange(
            new[]
            {
                new RequestOp
                {
                    RequestPut = new PutRequest
                    {
                        Key = ByteString.CopyFromUtf8("animals/cow"),
                        Value = ByteString.CopyFromUtf8("moo"),
                    },
                },
                new RequestOp
                {
                    RequestPut = new PutRequest
                    {
                        Key = ByteString.CopyFromUtf8("animals/chicken"),
                        Value = ByteString.CopyFromUtf8("cluck"),
                    },
                },
            }
        );

        await client.TransactionAsync(txn);

        var cow = await client.GetValAsync("animals/cow");
        var chicken = await client.GetValAsync("animals/chicken");
        Console.WriteLine($"GET animals/cow     = {cow}");
        Console.WriteLine($"GET animals/chicken = {chicken}");

        await Task.Delay(200);
        Console.WriteLine();
    }
}
