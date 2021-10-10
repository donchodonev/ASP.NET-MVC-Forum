﻿namespace ASP.NET_MVC_Forum.Services.Category.Models
{
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Data.Models;
    using ASP.NET_MVC_Forum.Services.Category.Contracts;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext db;

        public CategoryService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<IQueryable<Category>> AllAsync()
        {
            return await Task.Run(() => db.Categories.Where(x => x.IsDeleted == false));
        }
    }
}
