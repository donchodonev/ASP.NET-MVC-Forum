namespace ASP.NET_MVC_Forum.Web.Models.Chat
{
    public class ChatMessageResponseData
    {
        public ChatMessageResponseData()
        {

        }

        public ChatMessageResponseData(string senderUsername, string time, string text)
        {
            SenderUsername = senderUsername;
            Time = time;
            Text = text;
        }

        public string SenderUsername { get; set; }

        public string Time { get; set; }

        public string Text { get; set; }
    }
}
