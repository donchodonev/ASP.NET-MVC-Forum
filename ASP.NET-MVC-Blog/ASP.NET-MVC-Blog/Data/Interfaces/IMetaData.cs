namespace ASP.NET_MVC_Blog.Data.Interfaces
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public interface IMetaData
    {
        [Required]
        public DateTime CreatedOn { get; init; }

        public DateTime? ModifiedOn { get; set; }
    }
}
