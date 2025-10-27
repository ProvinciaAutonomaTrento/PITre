// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System.Collections.Concurrent;

namespace Signing.Api;

/// <summary>
/// Manages the mapping between users and their SignalR connection IDs
/// </summary>
public interface IUserConnectionManager
{
    /// <summary>
    /// Adds a connection for a user
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="connectionId">The SignalR connection ID</param>
    void AddConnection(string userId, string connectionId);

    /// <summary>
    /// Removes a specific connection
    /// </summary>
    /// <param name="connectionId">The SignalR connection ID to remove</param>
    void RemoveConnection(string connectionId);

    /// <summary>
    /// Gets all connection IDs for a specific user
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <returns>A collection of connection IDs</returns>
    IEnumerable<string> GetConnections(string userId);

    /// <summary>
    /// Gets the user ID associated with a connection ID
    /// </summary>
    /// <param name="connectionId">The connection ID to check</param>
    /// <returns>The user ID or null if not found</returns>
    string GetUserForConnection(string connectionId);

    /// <summary>
    /// Stores metadata for a user connection
    /// </summary>
    /// <param name="connectionId">The connection ID</param>
    /// <param name="metadata">The metadata object</param>
    void StoreConnectionMetadata(string connectionId, object metadata);

    /// <summary>
    /// Gets metadata for a connection
    /// </summary>
    /// <param name="connectionId">The connection ID</param>
    /// <returns>The metadata or null if not found</returns>
    T GetConnectionMetadata<T>(string connectionId) where T : class;
}

/// <summary>
/// Manages the mapping between users and their SignalR connection IDs
/// </summary>
public class UserConnectionManager : IUserConnectionManager
{
    private readonly ConcurrentDictionary<string, HashSet<string>> _userConnectionMap = new();
    private readonly ConcurrentDictionary<string, string> _connectionUserMap = new();
    private readonly ConcurrentDictionary<string, object> _connectionMetadata = new();

    /// <summary>
    /// Adds a connection for a user
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="connectionId">The SignalR connection ID</param>
    public void AddConnection(string userId, string connectionId)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(connectionId))
            return;

        // Add to user->connections mapping
        _userConnectionMap.AddOrUpdate(userId,
            // If the key doesn't exist, create a new HashSet with this connection
            new HashSet<string> { connectionId },
            // If the key exists, add to the existing HashSet
            (_, connections) =>
            {
                lock (connections)
                {
                    connections.Add(connectionId);
                    return connections;
                }
            });

        // Add to connection->user mapping
        _connectionUserMap[connectionId] = userId;
    }

    /// <summary>
    /// Removes a specific connection
    /// </summary>
    /// <param name="connectionId">The SignalR connection ID to remove</param>
    public void RemoveConnection(string connectionId)
    {
        if (string.IsNullOrEmpty(connectionId))
            return;

        // First, find which user owns this connection
        if (_connectionUserMap.TryRemove(connectionId, out var userId))
        {
            // Then remove the connection from the user's collection
            if (_userConnectionMap.TryGetValue(userId, out var connections))
            {
                lock (connections)
                {
                    connections.Remove(connectionId);

                    // If the user has no more connections, remove the user entry
                    if (connections.Count == 0)
                    {
                        _userConnectionMap.TryRemove(userId, out _);
                    }
                }
            }
        }

        // Remove any metadata for this connection
        _connectionMetadata.TryRemove(connectionId, out _);
    }

    /// <summary>
    /// Gets all connection IDs for a specific user
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <returns>A collection of connection IDs</returns>
    public IEnumerable<string> GetConnections(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return Enumerable.Empty<string>();

        if (_userConnectionMap.TryGetValue(userId, out var connections))
        {
            lock (connections)
            {
                // Return a copy to avoid modification issues
                return connections.ToList();
            }
        }

        return Enumerable.Empty<string>();
    }

    /// <summary>
    /// Gets the user ID associated with a connection ID
    /// </summary>
    /// <param name="connectionId">The connection ID to check</param>
    /// <returns>The user ID or null if not found</returns>
    public string GetUserForConnection(string connectionId)
    {
        if (string.IsNullOrEmpty(connectionId))
            return null;

        _connectionUserMap.TryGetValue(connectionId, out var userId);
        return userId;
    }

    /// <summary>
    /// Stores metadata for a user connection
    /// </summary>
    /// <param name="connectionId">The connection ID</param>
    /// <param name="metadata">The metadata object</param>
    public void StoreConnectionMetadata(string connectionId, object metadata)
    {
        if (string.IsNullOrEmpty(connectionId) || metadata == null)
            return;

        _connectionMetadata[connectionId] = metadata;
    }

    /// <summary>
    /// Gets metadata for a connection
    /// </summary>
    /// <param name="connectionId">The connection ID</param>
    /// <returns>The metadata or null if not found</returns>
    public T GetConnectionMetadata<T>(string connectionId) where T : class
    {
        if (string.IsNullOrEmpty(connectionId))
            return null;

        if (_connectionMetadata.TryGetValue(connectionId, out var metadata) && metadata is T typedMetadata)
        {
            return typedMetadata;
        }

        return null;
    }
}