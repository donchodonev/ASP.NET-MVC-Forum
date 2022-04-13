namespace ASP.NET_MVC_Forum.Domain.Models.Votes
{
    public class VoteResponseModel
    {
        public VoteResponseModel(int voteSum)
        {
            VoteSum = voteSum;
        }

        public int VoteSum { get; set; }
    }
}