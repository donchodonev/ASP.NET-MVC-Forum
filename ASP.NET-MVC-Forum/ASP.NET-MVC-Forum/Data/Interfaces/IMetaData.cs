namespace ASP.NET_MVC_Forum.Data.Interfaces
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
