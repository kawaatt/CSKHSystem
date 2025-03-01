using API.Data;
using API.Models.CSKHAuto;
using API.Models.DTO;
using API.Services.SignalR;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using System;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace API.Controllers
{
    [ApiController]
    [Route("api/BOAccount")]
    //[Authorize]
    public class BOAccountController : ControllerBase
    {
        private readonly IHubContext<SignalRHub> _contextHub;
        private readonly AppDbContext _dbContext;
        private IMapper _mapper;
        private ResponseDTO _responseDTO;

        public BOAccountController(AppDbContext dbContext, IMapper mapper, IHubContext<SignalRHub> contextHub)
        {
            _contextHub = contextHub;
            _dbContext = dbContext;
            _mapper = mapper;
            _responseDTO = new ResponseDTO();
        }

        [HttpPost]
        [Route("F168CheckAccount")]
        public async Task<ResponseDTO> F168CheckAccount()
        {
            try
            {
                // Read the request body as a string
                using (var reader = new StreamReader(Request.Body))
                {
                    var body = await reader.ReadToEndAsync();

                    // Deserialize the string into a DTO object
                    BOAccountDTO accountCheckDTO = JsonConvert.DeserializeObject<BOAccountDTO>(body);

                    // Use the accountCheckDTO object to access the payload data


                    Console.WriteLine(accountCheckDTO.Account);
                    // Add your logic here to process the payload data

                    List<TicketRequestDTO> ticketRequestDTOs = await _dbContext.TicketRequests
                                        .Where(x=>x.Account==accountCheckDTO.Account)
                                        .Select(o => new TicketRequestDTO
                                        {
                                            ID = o.ID,
                                            Account = o.Account,
                                            TicketContent = o.TicketContent,
                                            System = o.System,
                                            ImageURL = o.ImageURL,
                                            RequestDate = o.RequestDate,
                                            TicketCategory = new TicketCategoryDTO
                                            {
                                                ID = o.TicketCategory.ID,
                                                CategoryName = o.TicketCategory.CategoryName
                                            },
                                            TicketHistories = o.TicketHistories
                                                .Select(x => new TicketHistoryDTO
                                                {
                                                    UpdateTime = x.UpdateTime,
                                                    TicketStatusTitle = x.TicketStatusTitle,
                                                    TicketStatusDescription = x.TicketStatusDescription,
                                                    TicketStatusValue = x.TicketStatusValue
                                                }).OrderByDescending(x => x.UpdateTime).ToList()
                                        })
                                        .ToListAsync();
                    _responseDTO.Result = ticketRequestDTOs;
                }
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = ex.Message;
            }
            return _responseDTO;
        }

        
    }
}
