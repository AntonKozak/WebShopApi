
using AutoMapper.Execution;
using ShopApi.DTOs;
using ShopApi.Helpers;

namespace ShopApi.Interfaces;

public interface IUserRepository
{
    void Update(UserModel user);
    Task<IEnumerable<UserModel>> GetUsersAsync();
    Task<UserModel> GetUserByIdAsync(int id);
    Task<UserModel> GetUserByUsernameAsync(string username);
    Task<PagedList<MemberDto>> GetMembersAsync(UserParams filteringsParams);
    Task<MemberDto> GetMemberAsync(string username);
}

