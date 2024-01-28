
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using ShopApi.DTOs;
using ShopApi.Entities;
using ShopApi.Helpers;
using ShopApi.Helpers.FilterParams;
using ShopApi.Interfaces;

namespace ShopApi.Data.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public MessageRepository(DataContext context, IMapper mapper)
    {
        _mapper = mapper;
        _context = context;
    }

    public void AddMessage(Message message)
    {
        _context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        _context.Messages.Remove(message);
    }

    public async Task<Message> GetMessage(int id)
    {
        return await _context.Messages.FindAsync(id);
    }

    public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
    {


        var query = _context.Messages
        .OrderByDescending(m => m.MessageSent)
        .AsQueryable();

        query = messageParams.Container switch
        {
            "Inbox" => query.Where(u => u.Recipient.UserName == messageParams.Username),
            "Outbox" => query.Where(u => u.Sender.UserName == messageParams.Username),
            _ => query.Where(u => u.Recipient.UserName == messageParams.Username && u.RecipientDeleted == false && u.DateRead == DateTime.MinValue)
        };

        var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

        return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
    }

    public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName)
{
    var messages = await _context.Messages
        .Include(u => u.Sender).ThenInclude(p => p.Photos)
        .Include(u => u.Recipient).ThenInclude(p => p.Photos)
        .Where(m => (m.Recipient.UserName == currentUserName && m.Sender.UserName == recipientUserName)
                 || (m.Recipient.UserName == recipientUserName && m.Sender.UserName == currentUserName))
        .OrderBy(m => m.MessageSent)
        .ToListAsync();

    var unreadMessages = messages.Where(m => m.DateRead == DateTime.MinValue && m.Recipient.UserName == currentUserName).ToList();

    if (unreadMessages.Any())
    {
        foreach (var message in unreadMessages)
        {
            message.DateRead = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

    // Now, map the messages to MessageDto
    var messageDtos = _mapper.Map<IEnumerable<MessageDto>>(messages);
    return messageDtos;
}

    public async Task<bool> SaveAllAsync()
    {
        try
        {
            // Output information about entities in the change tracker
            foreach (var entry in _context.ChangeTracker.Entries())
            {
                Console.WriteLine($"Entity: {entry.Entity.GetType().Name}, State: {entry.State}");
            }

            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            // Log the exception (you might want to use a logging library or log to a file)
            Console.WriteLine($"Error saving changes: {ex.Message}");
            Console.WriteLine(ex.InnerException?.Message);

            // Return false to indicate that changes were not successfully saved
            return false;
        }
    }
}
