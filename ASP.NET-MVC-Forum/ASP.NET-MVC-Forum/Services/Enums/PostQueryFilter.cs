namespace ASP.NET_MVC_Forum.Services.Enums
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
        WithIdentityUser
    }
}
