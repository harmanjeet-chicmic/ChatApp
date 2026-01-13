using ChatApp.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces.Repositories
{
    public interface IGroupRepository
    {
        Task AddAsync(Group group);
        Task<Group?> GetByIdAsync(Guid groupId);
    }
}
