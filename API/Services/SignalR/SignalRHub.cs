using API.Models.CSKHAuto;
using Microsoft.AspNetCore.SignalR;

namespace API.Services.SignalR
{
    public class SignalRHub : Hub
    {
        //public async Task UpdateTicketHistory(string TicketHistory)
        //{
        //    await Clients.All.SendAsync("UpdateTicketHistory", TicketHistory);
        //}

        public async Task UpdateTicketHistory(int ID)
        {
            await Clients.All.SendAsync("UpdateTicketHistory", ID);
        }
    }
}
