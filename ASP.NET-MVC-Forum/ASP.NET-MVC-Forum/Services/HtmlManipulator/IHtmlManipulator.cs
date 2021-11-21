using Ganss.XSS;

namespace ASP.NET_MVC_Forum.Services.HtmlManipulator
{
    public interface IHtmlManipulator
    {
        public string Sanitize(string html);

        public string Escape(string content);

        public string Decode(string html);
    }
}
