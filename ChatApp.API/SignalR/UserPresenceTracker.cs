using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatApp.API.SignalR
{
    /// <summary>
    /// Tracks all active SignalR connections for users in memory.
    /// A user is considered online if at least one valid connection
    /// is actively sending heartbeat updates.
    /// </summary>
    public class UserPresenceTracker
    {
        /// <summary>
        /// Stores all active connections grouped by user.
        /// Each connection is associated with the last time it was seen alive.
        /// </summary>
        private static readonly Dictionary<Guid, Dictionary<string, DateTime>> OnlineUsers = new();

        /// <summary>
        /// Synchronization lock to ensure thread-safe access to the online user map.
        /// </summary>
        private static readonly object Lock = new();

        /// <summary>
        /// Maximum time a connection can remain inactive before it is considered offline.
        /// </summary>
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(15);

        // ============================================================
        // CONNECTION MANAGEMENT
        // ============================================================

        /// <summary>
        /// Registers a new SignalR connection for a user.
        /// If this is the first active connection for the user,
        /// the method returns true indicating the user has just come online.
        /// </summary>
        /// <param name="userId">The user who established the connection.</param>
        /// <param name="connectionId">The SignalR connection identifier.</param>
        /// <returns>
        /// True if this is the user's first active connection (user came online),
        /// otherwise false.
        /// </returns>
        public bool UserConnected(Guid userId, string connectionId)
        {
            lock (Lock)
            {
                if (!OnlineUsers.ContainsKey(userId))
                    OnlineUsers[userId] = new Dictionary<string, DateTime>();

                OnlineUsers[userId][connectionId] = DateTime.UtcNow;

                return OnlineUsers[userId].Count == 1;
            }
        }

        /// <summary>
        /// Removes a SignalR connection for a user.
        /// If this was the user's last active connection,
        /// the method returns true indicating the user has gone offline.
        /// </summary>
        /// <param name="userId">The user who disconnected.</param>
        /// <param name="connectionId">The SignalR connection identifier.</param>
        /// <returns>
        /// True if this was the user's last active connection (user went offline),
        /// otherwise false.
        /// </returns>
        public bool UserDisconnected(Guid userId, string connectionId)
        {
            lock (Lock)
            {
                if (!OnlineUsers.ContainsKey(userId))
                    return false;

                OnlineUsers[userId].Remove(connectionId);

                if (OnlineUsers[userId].Count == 0)
                {
                    OnlineUsers.Remove(userId);
                    return true;
                }

                return false;
            }
        }

        // ============================================================
        // HEARTBEAT MANAGEMENT
        // ============================================================

        /// <summary>
        /// Updates the last-seen timestamp of a specific connection.
        /// This is used by the heartbeat mechanism to prevent users
        /// from being marked offline due to temporary network delays
        /// or browser tab throttling.
        /// </summary>
        /// <param name="userId">The user sending the heartbeat.</param>
        /// <param name="connectionId">The active SignalR connection.</param>
        public void Refresh(Guid userId, string connectionId)
        {
            lock (Lock)
            {
                if (OnlineUsers.ContainsKey(userId) &&
                    OnlineUsers[userId].ContainsKey(connectionId))
                {
                    OnlineUsers[userId][connectionId] = DateTime.UtcNow;
                }
            }
        }

        // ============================================================
        // CONNECTION LOOKUP
        // ============================================================

        /// <summary>
        /// Returns all currently active SignalR connections for a user.
        /// Expired or inactive connections are automatically removed
        /// before the list is returned.
        /// </summary>
        /// <param name="userId">The user whose connections should be retrieved.</param>
        /// <returns>A list of active SignalR connection IDs.</returns>
        public IReadOnlyList<string> GetConnections(Guid userId)
        {
            CleanupExpiredConnections();

            lock (Lock)
            {
                if (!OnlineUsers.ContainsKey(userId))
                    return Array.Empty<string>();

                return OnlineUsers[userId].Keys.ToList();
            }
        }

        // ============================================================
        // INTERNAL CLEANUP
        // ============================================================

        /// <summary>
        /// Removes expired SignalR connections that have not sent a heartbeat
        /// within the configured timeout window. This ensures that users
        /// are marked offline even if their browser or network failed to
        /// properly close the connection.
        /// </summary>
        private void CleanupExpiredConnections()
        {
            lock (Lock)
            {
                var now = DateTime.UtcNow;

                foreach (var userId in OnlineUsers.Keys.ToList())
                {
                    var activeConnections =
                        OnlineUsers[userId]
                            .Where(c => now - c.Value < Timeout)
                            .ToDictionary(x => x.Key, x => x.Value);

                    if (activeConnections.Count == 0)
                        OnlineUsers.Remove(userId);
                    else
                        OnlineUsers[userId] = activeConnections;
                }
            }
        }
    }
}
