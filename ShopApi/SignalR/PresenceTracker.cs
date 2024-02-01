
namespace ShopApi.SignalR;

public class PresenceTracker
{
    //string = username, List<string> = connectionIds of users, can be more than one connectionId for same user
    private static readonly Dictionary<string, List<string>> OnlineUsers = 
    new Dictionary<string, List<string>>();

    public Task<bool> UserConnected(string username, string connectionId)
    {
        bool isOnline = false;
        // if two or more users connect at the same time, we don't want to have two or more threads accessing the same dictionary at the same time
        lock (OnlineUsers)
        {
            if (OnlineUsers.ContainsKey(username))
            {
                OnlineUsers[username].Add(connectionId);
            }
            else
            {
                OnlineUsers.Add(username, new List<string>{connectionId});
                isOnline = true;
            }
        }

        return Task.FromResult(isOnline);
    }

    public Task<bool> UserDisconnected(string username, string connectionId)
    {
        bool isOffline = false;
        lock (OnlineUsers)
        {
            if (!OnlineUsers.ContainsKey(username)) return Task.FromResult(isOffline);

            OnlineUsers[username].Remove(connectionId);

            if (OnlineUsers[username].Count == 0)
            {
                OnlineUsers.Remove(username);
                isOffline = true;
            }
        }

        return Task.FromResult(isOffline);
    }

    public Task<string[]> GetOnlineUsers()
    {
        string[] onlineUsers;
        lock (OnlineUsers)
        {
            onlineUsers = OnlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
        }

        return Task.FromResult(onlineUsers);
    }

    public static Task<List<string>> GetConnectionForUser(string username)
    {
        List<string> connectionIds;
        // make collection asynchonous to avoid blocking the thread
        // Ensure thread-safe access to the OnlineUsers collection
    lock (OnlineUsers)
    {
        // Retrieve the list of connection IDs associated with the given username
        // Note: GetValueOrDefault returns null if the username is not found,
        // so connectionIds could be null if the username is not present
        connectionIds = OnlineUsers.GetValueOrDefault(username);
    }

    return Task.FromResult(connectionIds);
    }
}
