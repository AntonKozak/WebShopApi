
using AutoMapper;
using Humanizer;
using ShopApi.DTOs;
using ShopApi.Entities;

namespace ShopApi.Helpers;

public class AutoMapperProfiles: Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<UserModel, MemberDto>()
            .ForMember(dest => dest.MainPhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url));
        CreateMap<UsersPhoto, UsersPhotoDto>();
        CreateMap<MemberUpdateDto, UserModel>();
        CreateMap<RegisterDto, UserModel>();
        CreateMap<Message, MessageDto>()
            .ForMember(dest => dest.SenderPhotoUrl, opt => opt.MapFrom(src => src.Sender.Photos.FirstOrDefault(x => x.IsMain).Url))
            .ForMember(dest => dest.RecipientPhotoUrl, opt => opt.MapFrom(src => src.Recipient.Photos.FirstOrDefault(x => x.IsMain).Url));
        // Convert the UTC date time, which is stored in the database, to local time
        CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
        CreateMap<DateTime?, DateTime?>().ConvertUsing(d => d.HasValue ? DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : null);

    }
}
