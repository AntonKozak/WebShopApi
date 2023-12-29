
using AutoMapper.Execution;
using ShopApi.DTOs;

namespace ShopApi.Interfaces;

public interface IUserRepository
{
    void Update(UserModel user);
    Task<bool> SaveAllAsync();
    Task<IEnumerable<UserModel>> GetUsersAsync();
    Task<UserModel> GetUserByIdAsync(int id);
    Task<UserModel> GetUserByUsernameAsync(string username);
    Task<IEnumerable<MemberDto>> GetMembersAsync();
    Task<MemberDto> GetMemberAsync(string username);
}

