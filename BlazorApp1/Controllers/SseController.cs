using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace BlazorApp1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SseController : ControllerBase
    {
        [HttpPost("stream")]
        public async Task StreamEvents()
        {
            Response.Headers.Append("Content-Type", "text/event-stream");
            Response.Headers.Append("Cache-Control", "no-cache");
            Response.Headers.Append("Connection", "keep-alive");
            Response.Headers.Append("Access-Control-Allow-Origin", "*");
            
            // Define the events to send
            var events = new[]
            {
                new { id = "1751073677430", data = @"{""jsonrpc"":""2.0"",""id"":1,""result"":{""kind"":""status-update"",""taskId"":""9fd47b2a-ac7a-4231-bd3b-864e7edcfc3b"",""contextId"":""c37b1a58-9075-4873-8b61-e2a14486d945"",""status"":{""state"":""working"",""message"":{""kind"":""message"",""role"":""agent"",""messageId"":""333980ed-56d0-45a7-8661-32d65536fbc6"",""parts"":[{""kind"":""text"",""text"":""Generating code...""}],""taskId"":""9fd47b2a-ac7a-4231-bd3b-864e7edcfc3b"",""contextId"":""c37b1a58-9075-4873-8b61-e2a14486d945""},""timestamp"":""2025-06-28T01:21:17.430Z""},""final"":false}}" },
                new { id = "1751073678245", data = @"{""jsonrpc"":""2.0"",""id"":1,""result"":{""kind"":""status-update"",""taskId"":""9fd47b2a-ac7a-4231-bd3b-864e7edcfc3b"",""contextId"":""c37b1a58-9075-4873-8b61-e2a14486d945"",""status"":{""state"":""completed"",""message"":{""kind"":""message"",""role"":""agent"",""messageId"":""2df2b9c6-fb51-4517-8628-55b5ef95bb08"",""parts"":[{""kind"":""text"",""text"":""Completed, but no files were generated.""}],""taskId"":""9fd47b2a-ac7a-4231-bd3b-864e7edcfc3b"",""contextId"":""c37b1a58-9075-4873-8b61-e2a14486d945""},""timestamp"":""2025-06-28T01:21:18.245Z""},""final"":true}}" }
            };

            // Send each event
            foreach (var evt in events)
            {
                var message = $"id: {evt.id}\ndata: {evt.data}\n\n";
                var bytes = Encoding.UTF8.GetBytes(message);
                await Response.Body.WriteAsync(bytes, 0, bytes.Length);
                await Response.Body.FlushAsync();
                
                // Add a small delay between events to simulate real-time streaming
                await Task.Delay(500);
            }
        }
    }
}