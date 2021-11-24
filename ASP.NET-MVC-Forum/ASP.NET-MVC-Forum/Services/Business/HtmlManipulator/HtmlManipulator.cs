namespace ASP.NET_MVC_Forum.Services.Business.HtmlManipulator
{
    using Ganss.XSS;
    using System.Text.RegularExpressions;
    using System.Web;
    public class HtmlManipulator : IHtmlManipulator
    {
        private readonly IHtmlSanitizer sanitizer;

        public HtmlManipulator(IHtmlSanitizer sanitizer)
        {
            this.sanitizer = sanitizer;
        }
        public string Escape(string html)
        {
            var pattern = @"<.*?>";

            return Regex.Replace(html, pattern, string.Empty);
        }

        public string Sanitize(string html)
        {
            return sanitizer.Sanitize(html);
        }

        public string Decode(string html)
        {
            return HttpUtility.HtmlDecode(html);
        }
    }
}


