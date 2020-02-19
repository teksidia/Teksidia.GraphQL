namespace GraphQLApi.Models
{
    public class Result<TEntity>
    {
        public int TotalCount { get; set; }
        public TEntity[] Items { get; set; }
        public PageInfo PageInfo { get; set; }
    }
}
