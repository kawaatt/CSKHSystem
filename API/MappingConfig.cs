using API.Models.CSKHAuto;
using AutoMapper;

namespace API
{
    public class MappingConfig : Profile
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<TicketCategory, TicketCategoryDTO>().ReverseMap();
                config.CreateMap<TicketHistory, TicketHistoryDTO>().ReverseMap();

                config.CreateMap<TicketRequest, TicketRequestDTO>()
                    .ForMember(dest => dest.TicketHistories, opt => opt.MapFrom(src => src.TicketHistories)).ReverseMap();
            });
            return mappingConfig;
        }
    }
}
