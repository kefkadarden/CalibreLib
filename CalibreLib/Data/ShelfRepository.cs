using CalibreLib.Areas.Identity.Data;
using Azure.Core;
using CalibreLib.Models;
using CalibreLib.Models.Metadata;
using CalibreLib.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CalibreLib.Data
{
    public class ShelfRepository : IRepositoryBase<Shelf>
    {
        private bool disposedValue = false;
        private CalibreLibContext context;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShelfRepository(CalibreLibContext context, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            _env = env;
            _userManager = userManager;
            _contextAccessor = httpContextAccessor;
        }

        public void Delete(Shelf item)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Shelf>> GetAllAsync()
        {
            var user = await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
            return user?.Shelves;
        }

        public async Task<Shelf> GetByIDAsync(int id)
        {
            var user = await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
            return user?.Shelves.FirstOrDefault(x => x.Id == id);
        }

        public void Insert(Shelf item)
        {
            throw new NotImplementedException();
        }

        public void SaveAsync()
        {
            throw new NotImplementedException();
        }

        public void Update(Shelf item)
        {
            throw new NotImplementedException();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    context.Dispose();
                }

                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~ShelfRepository()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
