﻿using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PPKS_Parking_BE_API.Data;

namespace PPKS_Parking_BE_API.WebSockets
{
    public static class ParkingWebSocketHandler
    {
        public static async Task HandleAsync(WebSocket webSocket, ApplicationDbContext context)
        {
            var buffer = new byte[1024 * 8];
            var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));

            var receiveTask = Task.Run(async () =>
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                        break;
                    }
                }
            });

            while (webSocket.State == WebSocketState.Open && await timer.WaitForNextTickAsync())
            {
                var parkings = await context.Parkings
                    .Include(p => p.Blocks)
                        .ThenInclude(b => b.ParkingSpots)
                    .ToListAsync();

                var data = parkings.Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.FreeSpotsCount
                });

                var json = JsonSerializer.Serialize(data);
                var bytes = Encoding.UTF8.GetBytes(json);
                var segment = new ArraySegment<byte>(bytes);

                await webSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
            }

            await receiveTask;
        }

    }
}
