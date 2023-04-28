

using StreamJsonRpc;

namespace android_jsonrpc
{
    public class HelloRequest
    {
        public string? Name { get; set; }
    }

    public class HelloReply
    {
        public string? Message { get; set; }
    }

    public interface IGreeter
    {
        Task<string> SayHelloAsync(string name);
        Task<HelloReply> SayHelloObjectAsync(HelloRequest request);
    }

    public class GreeterServer : IGreeter
    {
        public Task<string> SayHelloAsync(string name)
        {
            throw new RemoteInvocationException("Everything went unexpected.", 1889, errorData: null);
            return Task.FromResult($"Hello {name}");
        }

        public Task<HelloReply> SayHelloObjectAsync(HelloRequest request)
        {
            throw new RemoteInvocationException("Everything went unexpected.", 1889, errorData: null);
            return Task.FromResult(new HelloReply
            {
                Message = $"Hello {request.Name}"
            });
        }
    }
}

