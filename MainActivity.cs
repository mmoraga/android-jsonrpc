using System.Net;
using System.Net.Sockets;
using System.Text;
using Android.Util;
using StreamJsonRpc;

namespace android_jsonrpc;

[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : Activity
{
    private const string tag = "android_jsonrpc";
    
    static async Task RpcConnectionAsync()
    {
        try
        {
            var tcpServer = new TcpListener(IPAddress.Loopback, 29000);

            Log.Info(tag, "Starting server.");
            tcpServer.Start();
            var tcpClient = new TcpClient("localhost", 29000);

            var clientConnection = tcpServer.AcceptTcpClient();
            var clientConnectionStream = clientConnection.GetStream();
            var serverFormatter = new JsonMessageFormatter(Encoding.UTF8);
            var clientFormatter = new JsonMessageFormatter(Encoding.UTF8);

            var serverHandler = new LengthHeaderMessageHandler(clientConnectionStream, clientConnectionStream, serverFormatter);
            using var jsonRpc = new JsonRpc(serverHandler, new GreeterServer());
            jsonRpc.StartListening();
            Log.Info(tag, "start listening to rpc stuff");

            var stream = tcpClient.GetStream();
            var jsonMessageHandler = new LengthHeaderMessageHandler(stream, stream, clientFormatter);

            var greeterClient = new JsonRpc(jsonMessageHandler);
            greeterClient.StartListening();
            Log.Info(tag, "Try sending message.");
            var helloReply = await greeterClient.InvokeAsync<string>("SayHelloAsync", "Little Bobby Tables");
            Log.Info(tag, helloReply);

            await jsonRpc.Completion;
        }
        catch (Exception e)
        {
            Log.Info(tag, $"Error in rpc communication: {e}");
        }
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Set our view from the "main" layout resource
        SetContentView(Resource.Layout.activity_main);
        _ = Task.Factory.StartNew(RpcConnectionAsync, CancellationToken.None,
            TaskCreationOptions.DenyChildAttach | TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }
}