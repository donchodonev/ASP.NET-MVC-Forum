﻿namespace ASP.NET_MVC_Forum.Services.Data.Category
{
    using ASP.NET_MVC_Forum.Data.Models;
    using ASP.NET_MVC_Forum.Models.Post;
    using System.Collections.Generic;
    using System.Linq;

    public interface ICategoryDataService
    {
        public IQueryable<Category> All(bool withPostsIncluded = false);
        public List<string> GetCategoryNames();

        public CategoryIdAndNameViewModel[] GetCategoryIdAndNameCombinations();
    }
}