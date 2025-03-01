using AutoMapper;
using TELEBOT_CSKH.Models.TELEGRAM_BOT;

namespace TELEBOT_CSKH
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<TelegramAccount, TelegramAccountDTO>().ReverseMap();
                config.CreateMap<TelegramResponse, TelegramResponseDTO>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
