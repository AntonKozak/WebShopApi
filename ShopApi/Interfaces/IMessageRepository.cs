
using ShopApi.DTOs;
using ShopApi.Entities;
using ShopApi.Helpers;
using ShopApi.Helpers.FilterParams;

namespace ShopApi.Interfaces;

public interface IMessageRepository
{
    void AddMessage(Message message);
    void DeleteMessage(Message message);
    Task<Message> GetMessage(int id);
    Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
    Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName);
    Task<bool> SaveAllAsync();

}
