using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Infrastructure.Shared
{
    public class BaseRepository<TEntity,TEntityId> : IRepository<TEntity,TEntityId>
    where TEntity : Entity<TEntityId>
    where TEntityId : EntityId
    {
        protected readonly DbSet<TEntity> Objs;
        
        public BaseRepository(DbSet<TEntity> objs)
        {
            this.Objs = objs ?? throw new ArgumentNullException(nameof(objs));
        
        }

        public async Task<List<TEntity>> GetAllAsync()
        {
            return await this.Objs.ToListAsync();
        }
        
        public async Task<TEntity> GetByIdAsync(TEntityId id)
        {
            //return await this._context.Categories.FindAsync(id);
            return await this.Objs
                .Where(x => id.Equals(x.Id)).FirstOrDefaultAsync();
        }
        public async Task<List<TEntity>> GetByIdsAsync(List<TEntityId> ids)
        {
            return await this.Objs
                .Where(x => ids.Contains(x.Id)).ToListAsync();
        }
        public async Task<TEntity> AddAsync(TEntity obj)
        {
            var ret = await this.Objs.AddAsync(obj);
            return ret.Entity;
        }

        public void Remove(TEntity obj)
        {
            this.Objs.Remove(obj);
        }
    }
}