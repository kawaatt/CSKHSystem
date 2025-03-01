using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TELEBOT_CSKH.Data;
using TELEBOT_CSKH.Models.DTO;
using TELEBOT_CSKH.Models.TELEGRAM_BOT;
using TELEBOT_CSKH.Services.SignalR;

namespace TELEBOT_CSKH.Controllers
{
    public class TelegramCustomerController
    {
        public ResponseDTO _responseDTO;
        public readonly AppDbContext _dbContext;
        public readonly IMapper _mapper;
        private readonly IHubContext<SignalRHub> _contextHub;

        public TelegramCustomerController(AppDbContext dbContext, IMapper mapper, IHubContext<SignalRHub> contextHub)
        {
            _responseDTO = new ResponseDTO();
            _dbContext = dbContext;
            _mapper = mapper;
            _contextHub = contextHub;
        }

        [HttpGet]
        public async Task<ResponseDTO> GetTelegramCustomerList([FromQuery] QueryParameterDTO ParamDTO)
        {
            try
            {
                if (ParamDTO.System.IsNullOrEmpty())
                {
                    _responseDTO.IsSuccess = false;
                    _responseDTO.Message = "#1 QUERY ERROR";
                    return _responseDTO;
                }

                var MainQuery = _dbContext.TelegramCustomers.Where(x => x.System == ParamDTO.System).AsQueryable();
                
                if (!string.IsNullOrEmpty(ParamDTO.SearchText))
                {
                    MainQuery = MainQuery.Where(w => w.UserName.Contains(ParamDTO.SearchText)
                                    || w.UserName.ToString().Contains(ParamDTO.SearchText));
                }

                if (ParamDTO.StartTime != null)
                {
                    MainQuery = MainQuery.Where(w => w.CreateDate >= ParamDTO.StartTime);
                }

                if (ParamDTO.EndTime != null)
                {
                    MainQuery = MainQuery.Where(w => w.CreateDate <= ParamDTO.EndTime);
                }

                bool isOrderAsc = true;
                if (ParamDTO.SortDirection == "desc")
                {
                    isOrderAsc = false;
                }

                if (!string.IsNullOrEmpty(ParamDTO.SortBy))
                {
                    switch (ParamDTO.SortBy)
                    {
                        case "createDate":
                            if (isOrderAsc)
                            {
                                MainQuery = MainQuery.OrderBy(w => w.CreateDate);
                            }
                            else
                            {
                                MainQuery = MainQuery.OrderByDescending(w => w.CreateDate);
                            }
                            break;
                        default:
                            break;
                    }
                }

                List<TelegramCustomerDTO> ListTicketRequestData = await MainQuery
                                        .Skip((ParamDTO.PageIndex - 1) * ParamDTO.PageSize)
                                        .Take(ParamDTO.PageSize)
                                        .Select(o => new TelegramCustomerDTO
                                        {
                                            ID = o.ID,
                                            TelegramID = o.TelegramID,
                                            UserName = o.UserName,
                                            Name = o.Name,
                                            iPremium = o.iPremium,
                                            CreateDate = o.CreateDate,
                                            System = o.System
                                        }).ToListAsync();

                _responseDTO.TotalCount = ListTicketRequestData.Count();

                if (ListTicketRequestData.Count > 0)
                {
                    _responseDTO.Result = ListTicketRequestData;
                }
                else
                {
                    _responseDTO.Message = "NO DATA FOUND";
                    _responseDTO.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = ex.Message;
            }
            return _responseDTO;
        }

        [HttpPost]
        public async Task<ResponseDTO> PostAsync(TelegramCustomerDTO telegramCustomerDTO)
        {
            try
            {
                TelegramCustomer _telegramCustomer = await _dbContext.TelegramCustomers.Where(x => x.TelegramID== telegramCustomerDTO.TelegramID && x.System == telegramCustomerDTO.System).FirstOrDefaultAsync();
                if (_telegramCustomer != null)
                {
                    _responseDTO.IsSuccess = false;
                    _responseDTO.Message = "DATA EXISTED";
                    return _responseDTO;
                }
                _responseDTO.Result = _mapper.Map<TelegramCustomerDTO>(_telegramCustomer);
            }
            catch (Exception ex)
            {
                _responseDTO.Message = ex.Message;
                _responseDTO.IsSuccess = false;
            }
            return _responseDTO;
        }

        [HttpDelete]
        public async Task<ResponseDTO> DeleteAsync(Guid ID, string System)
        {
            try
            {
                TelegramCustomer _telegramCustomer = await _dbContext.TelegramCustomers.Where(x => x.ID == ID && x.System == System).FirstOrDefaultAsync();
                if (_telegramCustomer == null)
                {
                    _responseDTO.IsSuccess = false;
                    _responseDTO.Message = "DATA NOT FOUND";
                    return _responseDTO;
                }

                _dbContext.TelegramCustomers.Remove(_telegramCustomer);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _responseDTO.Message = ex.Message;
                _responseDTO.IsSuccess = false;
            }
            return _responseDTO;
        }
    }
}
