using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Aoxe.Utf8Json;
using IAliceServices;
using IBobServices;
using ICarolServices;

namespace BobServices
{
    public class BobService : IBobService
    {
        private readonly IAliceService _aliceService;
        private readonly ICarolService _carolService;

        public BobService(IAliceService aliceService, ICarolService carolService)
        {
            _aliceService = aliceService;
            _carolService = carolService;
        }

        public string Hello() => "Hello,I am Bob.";

        public async Task<string> HelloAsyncTest()
        {
            var hello = $"Hi,I am Bob.{DateTime.Now}";
            var ms = new MemoryStream();
            var bytes = Encoding.UTF8.GetBytes(hello);
            await ms.WriteAsync(bytes.AsMemory(0, bytes.Length));
            ms.Position = 0;
            var buffer = new byte[bytes.Length];
            var i = await ms.ReadAsync(buffer.AsMemory(0, buffer.Length));
            var result = Encoding.UTF8.GetString(buffer);
            return result;
        }

        public string SayHelloToAlice() => $"Hi,I am Bob.\r\n{_aliceService.Hello()}";

        public async Task<string> SayHelloToAliceAsyncTest() =>
            $"Hi,I am Bob.\r\n{await _aliceService.HelloAsyncTest()}";

        public string SayHelloToCarol() => $"Hi,I am Bob.\r\n{_carolService.Hello()}";

        public Exception ThrowException() =>
            throw new Exception("This exception was thrown by Bob.");

        public async Task<string> PassAppleToAliceAsync(string appleName)
        {
            var apple = new Apple
            {
                Id = 1,
                Name = appleName,
                Message = $"Bob has passed the apple to Alice on {DateTimeOffset.Now}."
            };
            return (await _aliceService.PassBackAppleAsync(apple)).ToJson();
        }
    }
}
