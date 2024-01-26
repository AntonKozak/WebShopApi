
using ShopApi.DTOs;
using ShopApi.Entities;
using ShopApi.Helpers;

namespace ShopApi.Interfaces;

public interface IMessageRepository
{
    void AddMessage(Message message);
    void DeleteMessage(Message message);
    Task<Message> GetMessage(int id);
    Task<PagedList<MessageDto>> GetMessagesForUser();
    Task<IEnumerable<MessageDto>> GetMessageThread(int currentUserId, int recipientId);
    Task<bool> SaveAllAsync();

}
