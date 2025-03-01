using API.Constant;
using API.Data;
using API.Models.CSKHAuto;
using API.Models.DTO;
using API.Services.MediaServices;
using API.Services.SignalR;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace API.Controllers
{
    [ApiController]
    [Route("api/TicketRequest")]
    //[Authorize]
    public class TicketRequestController : ControllerBase
    {
        private readonly IHubContext<SignalRHub> _contextHub;
        private readonly AppDbContext _dbContext;
        private readonly IImageUploadService _imageUpdateServices;

        private IMapper _mapper;
        private ResponseDTO _responseDTO;

        public TicketRequestController(AppDbContext dbContext, IMapper mapper, IImageUploadService imageUpdateServices, IHubContext<SignalRHub> contextHub)
        {
            _contextHub = contextHub;
            _dbContext = dbContext;
            _mapper = mapper;
            _imageUpdateServices = imageUpdateServices;
            _responseDTO = new ResponseDTO();
        }

        [HttpGet]
        public async Task<ResponseDTO> GetTicketList([FromQuery] QueryParameterDTO ParamDTO)
        {
            try
            {
                var MainQuery = _dbContext.TicketRequests.Where(x => x.System == ParamDTO.System).AsQueryable();
                switch (ParamDTO.Status)
                {
                    case 0:
                        MainQuery = MainQuery.Where(x => x.System == ParamDTO.System && x.TicketHistories.Count == 0);
                        break;
                    case 1:
                        MainQuery = MainQuery.Where(x => x.System == ParamDTO.System && x.TicketHistories.Count >0 && !x.TicketHistories.Any(x => x.TicketStatusValue == 3));
                        break;
                    case 2:
                        MainQuery = MainQuery.Where(x => x.System == ParamDTO.System && x.TicketHistories.Any(x => x.TicketStatusValue == 3));
                        break;
                    default:
                        break;
                }

                if (!string.IsNullOrEmpty(ParamDTO.SearchText))
                {
                    MainQuery = MainQuery.Where(w => w.Account.Contains(ParamDTO.SearchText));
                }

                if (ParamDTO.StartTime != null)
                {
                    MainQuery = MainQuery.Where(w => w.RequestDate >= ParamDTO.StartTime);
                }

                if (ParamDTO.EndTime != null)
                {
                    MainQuery = MainQuery.Where(w => w.RequestDate <= ParamDTO.EndTime);
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
                        case "requestDate":
                            if (isOrderAsc)
                            {
                                MainQuery = MainQuery.OrderBy(w => w.RequestDate);
                            }
                            else
                            {
                                MainQuery = MainQuery.OrderByDescending(w => w.RequestDate);
                            }
                            break;
                        default:
                            break;
                    }
                }
                
                List<TicketRequestDTO> ListTicketRequestData = await MainQuery
                                        .Skip((ParamDTO.PageIndex - 1) * ParamDTO.PageSize)
                                        .Take(ParamDTO.PageSize)
                                        .Select(o => new TicketRequestDTO
                                        {
                                            ID = o.ID,
                                            Account = o.Account,
                                            TicketContent = o.TicketContent,
                                            System = o.System,
                                            ImageURL =  $"{SD.DomainImageURL}/{o.ImageURL}",
                                            RequestDate = o.RequestDate,
                                            TicketCategory = new TicketCategoryDTO
                                            {
                                                ID = o.TicketCategory.ID,
                                                CategoryName = o.TicketCategory.CategoryName
                                            },
                                            TicketHistories = o.TicketHistories
                                                .Select(x => new TicketHistoryDTO
                                                {
                                                    UpdatedByUser = x.UpdatedByUser,
                                                    UpdateTime = x.UpdateTime,
                                                    TicketStatusTitle = x.TicketStatusTitle,
                                                    TicketStatusDescription = x.TicketStatusDescription,
                                                    TicketStatusValue = x.TicketStatusValue
                                                }).OrderBy(x => x.UpdateTime).ToList()
                                        })
                                        .ToListAsync();

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
        [Route("GetTicketByID")]
        public async Task<ResponseDTO> GetTicketByID()
        {
            try
            {
                using (var reader = new StreamReader(Request.Body))
                {
                    var body = await reader.ReadToEndAsync();

                    // Deserialize the string into a DTO object
                    dynamic payloadObject = JsonConvert.DeserializeObject<dynamic>(body);

                    Guid _myTicketID = Guid.Parse(payloadObject.TicketID.ToString());
                    string System = payloadObject.System;

                    TicketRequestDTO _ticketRequest = await _dbContext.TicketRequests
                                        .Where(x => x.ID == _myTicketID && x.System == System)
                                        .Select(o => new TicketRequestDTO
                                        {
                                            ID = o.ID,
                                            Account = o.Account,
                                            TicketContent = o.TicketContent,
                                            System = o.System,
                                            ImageURL = $"{SD.DomainImageURL}/{o.ImageURL}",
                                            RequestDate = o.RequestDate,
                                            TicketCategory = new TicketCategoryDTO
                                            {
                                                ID = o.TicketCategory.ID,
                                                CategoryName = o.TicketCategory.CategoryName
                                            },
                                            TicketHistories = o.TicketHistories
                                                .Select(x => new TicketHistoryDTO
                                                {
                                                    UpdatedByUser = x.UpdatedByUser,
                                                    UpdateTime = x.UpdateTime,
                                                    TicketStatusTitle = x.TicketStatusTitle,
                                                    TicketStatusDescription = x.TicketStatusDescription,
                                                    TicketStatusValue = x.TicketStatusValue
                                                }).OrderBy(x => x.UpdateTime).ToList()
                                        })
                                        .FirstOrDefaultAsync();
                    Console.WriteLine(JsonConvert.SerializeObject(_ticketRequest));
                    if (_ticketRequest != null)
                    {
                        _responseDTO.Result = _ticketRequest;
                    }
                    else
                    {
                        _responseDTO.Message = "NO DATA FOUND";
                        _responseDTO.IsSuccess = false;
                    }
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
        [Route("AcceptTicketProcess")]
        public async Task<ResponseDTO> AcceptTicketProcess([FromQuery] string TicketID, [FromQuery] string System, [FromQuery] string Username)
        {
            try
            {
                TicketRequest _ticketRequest = await _dbContext.TicketRequests
                                        .Include(x => x.TicketCategory)
                                        .Include(x => x.TicketHistories)
                                        .Where(x => x.ID == Guid.Parse(TicketID) && x.System == System)
                                        .FirstOrDefaultAsync();

                if (_ticketRequest != null)
                {
                    TicketHistory _ticketHistory = _ticketRequest.TicketHistories.FirstOrDefault(x => x.TicketStatusValue == 1);
                    if (_ticketHistory != null)
                    {
                        _responseDTO.IsSuccess = false;
                        _responseDTO.Message = $"Ticket đã được xử lý - {_ticketHistory.UpdatedByUser}";
                        return _responseDTO;
                    }

                    TicketHistory _tempTicketHistory = new TicketHistory
                    {
                        ID = Guid.NewGuid(),
                        TicketRequestID = _ticketRequest.ID,
                        System = System.ToUpper(),
                        UpdatedByUser = Username,
                        UpdateTime = DateTime.Now,
                        TicketStatusValue = 1,
                        TicketStatusTitle = "Bộ phận CSKH đang xử lý",
                        TicketStatusDescription = $"Yêu cầu kiểm tra của quý khách đã được bộ phận CSKH tiếp nhận xử lý. Quý khách vui lòng theo dõi và kiểm tra trạng thái kết quả sau ít phút."
                    };

                    _dbContext.TicketHistories.Add(_tempTicketHistory);
                    await _dbContext.SaveChangesAsync();
                    await _contextHub.Clients.All.SendAsync("UpdateTicketHistory", 99);
                    _ticketRequest.TicketHistories.Add(_tempTicketHistory);
                    _responseDTO.Result = _mapper.Map<TicketRequestDTO>(_ticketRequest);
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
        [Route("UpdateTicketProcess")]
        public async Task<ResponseDTO> UpdateTicketProcess([FromBody] TicketHistoryDTO Data)
        {
            try
            {

                TicketRequest _ticketRequest = await _dbContext.TicketRequests
                                        .Include(x => x.TicketHistories)
                                        .Include(x=>x.TicketCategory)
                                        .Where(x => x.ID == Data.TicketRequestID && x.System == Data.System)
                                        .FirstOrDefaultAsync();

                if (_ticketRequest != null)
                {
                    TicketHistory _tempTicketHistory = new TicketHistory
                    {
                        ID = Guid.NewGuid(),
                        TicketRequestID = _ticketRequest.ID,
                        System = Data.System.ToUpper(),
                        UpdatedByUser = Data.UpdatedByUser,
                        UpdateTime = DateTime.Now,
                        TicketStatusValue = 2,
                        TicketStatusTitle = "Cập nhật xử lý",
                        TicketStatusDescription = Data.TicketStatusDescription
                    };

                    _dbContext.TicketHistories.Add(_tempTicketHistory);
                    await _dbContext.SaveChangesAsync();
                    await _contextHub.Clients.All.SendAsync("UpdateTicketHistory", 99);
                    _ticketRequest.TicketHistories.Add(_tempTicketHistory);
                    _responseDTO.Result = _mapper.Map<TicketRequestDTO>(_ticketRequest);
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
        [Route("FinishTicketProcess")]
        public async Task<ResponseDTO> FinishTicketProcess([FromBody] TicketHistory Data)
        {
            try
            {
                Console.WriteLine("FinishTicketProcess");
                TicketRequest _ticketRequest = await _dbContext.TicketRequests
                                        .Include(x => x.TicketHistories)
                                        .Include(x => x.TicketCategory)
                                        .Where(x => x.ID == Data.TicketRequestID && x.System == Data.System)
                                        .FirstOrDefaultAsync();
                if (_ticketRequest != null)
                {
                    TicketHistory _tempTicketHistory = new TicketHistory
                    {
                        ID = Guid.NewGuid(),
                        TicketRequestID = _ticketRequest.ID,
                        System = Data.System.ToUpper(),
                        UpdatedByUser = Data.UpdatedByUser,
                        UpdateTime = DateTime.Now,
                        TicketStatusValue = 3,
                        TicketStatusTitle = "Hoàn thành hỗ trợ",
                        TicketStatusDescription = Data.TicketStatusDescription
                    };

                    _dbContext.TicketHistories.Add(_tempTicketHistory);
                    await _dbContext.SaveChangesAsync();
                    await _contextHub.Clients.All.SendAsync("UpdateTicketHistory", 99);
                    _ticketRequest.TicketHistories.Add(_tempTicketHistory);
                    _responseDTO.Result = _mapper.Map<TicketRequestDTO>(_ticketRequest);
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
        [Route("SendTicketRequest")]
        public async Task<ResponseDTO> SendTicketRequest([FromBody] TicketRequestDTO TicketRequestDTO)
        {
            string fileLocalPath = "";
            try
            {
                BOAccount _boAccount = await _dbContext.CheckAccountFilters.Where(x=>x.Account == TicketRequestDTO.Account 
                                                                        && x.System == TicketRequestDTO.System 
                                                                        && x.CardHolder == TicketRequestDTO.CardHolder).FirstOrDefaultAsync();

                if (_boAccount==null)
                {
                    _responseDTO.IsSuccess = false;
                    _responseDTO.Message = "Xác thực tài khoản thất bại. Quý khách vui lòng xác thực lại";
                    return _responseDTO;
                }

                //Select All TicketRequest by Account and System
                List<TicketRequest> _ticketRequest = await _dbContext.TicketRequests
                                            .Include(x => x.TicketCategory) 
                                            .Include(x => x.TicketHistories)
                                            .Where(x => x.Account == TicketRequestDTO.Account && x.System == TicketRequestDTO.System)
                                            .OrderByDescending(x => x.RequestDate)
                                            .ToListAsync();
                bool fAddNewTicket = false;
                if (_ticketRequest == null || _ticketRequest.Count == 0)
                {
                    fAddNewTicket = true;
                }
                else
                {
                    //Find the total pending ticket request
                    List<TicketRequest> _ticketCheckedDTOList = _ticketRequest.Where(x => !x.TicketHistories.Any(y => y.TicketStatusValue == 2)).ToList();
                    if (_ticketCheckedDTOList.Count >= 3)
                    {
                        _responseDTO.IsSuccess = false;
                        _responseDTO.Message = "Tài khoản của quý khách đang tồn tại nhiều lượt yêu cầu kiểm tra đang được xử lý. Quý khách vui lòng chờ bộ phận CSKH xử lý hoàn thành các yêu cầu cũ trước khi gửi mới";
                        return _responseDTO;
                    }
                    else
                    {
                        fAddNewTicket = true;
                    }
                }

                if (fAddNewTicket == true)
                {
                    string uploadImageURL = "";
                    if (!string.IsNullOrEmpty(TicketRequestDTO.ImageBase64))
                    {
                        uploadImageURL = await _imageUpdateServices.UploadImageBase64Async(TicketRequestDTO.ImageBase64, TicketRequestDTO.System);
                        if (string.IsNullOrEmpty(uploadImageURL))
                        {
                            _responseDTO.IsSuccess = false;
                            _responseDTO.Message = "Quá trình tải ảnh thất bại. Quý khách vui lòng thực hiện lại";
                            return _responseDTO;
                        }
                    }

                    TicketRequest _newTicketRequest = new TicketRequest
                    {
                        ID = new Guid(),
                        Account = TicketRequestDTO.Account,
                        TicketContent = TicketRequestDTO.TicketContent,
                        System = TicketRequestDTO.System,
                        ImageURL = uploadImageURL,
                        RequestDate = DateTime.Now,
                        TicketCategoryID = TicketRequestDTO.TicketCategoryID
                    };
                    Console.WriteLine(JsonConvert.SerializeObject(_newTicketRequest));
                    _dbContext.TicketRequests.Add(_newTicketRequest);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = "#error - Lỗi hệ thống. "+ ex.Message;
            }

            return _responseDTO;
        }

        
    }
}
