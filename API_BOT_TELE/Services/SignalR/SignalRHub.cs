using Microsoft.AspNetCore.SignalR;

namespace TELEBOT_CSKH.Services.SignalR
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
