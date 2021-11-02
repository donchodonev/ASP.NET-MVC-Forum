namespace ASP.NET_MVC_Forum.Data.Interfaces
{
    using System.Linq;

    public interface IReport
    {
        public int Id { get; set; }

        public string Reason { get; set; }
    }
}
