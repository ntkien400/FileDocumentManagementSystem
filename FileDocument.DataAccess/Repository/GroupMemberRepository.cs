using FileDocument.DataAccess.IRepository;
using FileDocument.Models.Dtos;
using FileDocument.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDocument.DataAccess.Repository
{
    public class GroupMemberRepository : GenericRepository<GroupMember>, IGroupMemberRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public GroupMemberRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

    }
}
