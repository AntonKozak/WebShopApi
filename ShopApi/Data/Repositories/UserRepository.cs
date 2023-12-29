
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApi.DTOs;
using ShopApi.Interfaces;

namespace ShopApi.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DataContext _context;
        private readonly IMapper _mapper;
    public UserRepository(DataContext context, IMapper mapper)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<MemberDto> GetMemberAsync(string username)
    {
        //without automapper // and below is the same as the one with automapper
        // return await _context.Users
        // .Where(x => x.UserName == username)
        // .Select(user => new MemberDto
        // {
        //     Id = user.Id,
        //     UserName = user.UserName,
        //     Email = user.Email,
        //     FirstName = user.FirstName,
        //     LastName = user.LastName,
        //     MainPhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain.HasValue && x.IsMain.Value).Url,
        //     Photos = user.Photos.Select(photo => new UsersPhotoDto
        //     {
        //         Id = photo.Id,
        //         Url = photo.Url,
        //         IsMain = photo.IsMain
        //     }).ToList()
        // }).SingleOrDefaultAsync();

        return await _context.Users
        .Where(x => x.UserName == username)
        .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
        .SingleOrDefaultAsync();
        
    }

    public async Task<IEnumerable<MemberDto>> GetMembersAsync()
    {
        return await _context.Users
        .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
        .ToListAsync();
    }

    public async Task<UserModel> GetUserByIdAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        return user!;
    }

    public async Task<UserModel> GetUserByUsernameAsync(string username)
    {
        var user = await _context.Users
        .Include(p => p.Photos)
        .SingleOrDefaultAsync(x => x.UserName == username);
        return user!;
    }

    public async Task<IEnumerable<UserModel>> GetUsersAsync()
    {
        return await _context.Users
                .Include(p => p.Photos)
                .ToListAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public void Update(UserModel user)
    {
        _context.Entry(user).State = EntityState.Modified;
    }

}
