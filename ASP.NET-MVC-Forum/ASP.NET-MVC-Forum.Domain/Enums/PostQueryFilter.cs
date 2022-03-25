namespace ASP.NET_MVC_Forum.Domain.Enums
{
    public enum PostQueryFilter
    {
        WithoutDeleted,
        AsNoTracking,
        WithComments,
        WithCategory,
        WithUser,
        WithUserPosts,
        WithVotes,
        WithIdentityUser,
        WithReports,
        OrderByDescending
    }
}
