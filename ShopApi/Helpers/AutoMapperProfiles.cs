
using AutoMapper;
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
            
    }
}
