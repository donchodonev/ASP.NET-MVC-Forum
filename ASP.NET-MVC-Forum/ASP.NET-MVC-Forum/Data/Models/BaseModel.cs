﻿namespace ASP.NET_MVC_Forum.Web.Data.Models
{
    using ASP.NET_MVC_Forum.Web.Data.Interfaces;
    using System;

    public abstract class BaseModel : IDeletable, IMetaData
    {
        public BaseModel()
        {
            CreatedOn = DateTime.UtcNow;
        }

        public bool IsDeleted {get;set; }

        public DateTime CreatedOn { get; init; }

        public DateTime? ModifiedOn { get; set; }
    }
}
