namespace BlazorAppDemo.Models
{
    public class Country
    {
        public string Name { get; set; } = string.Empty;
        public int Id { get; set; }
        public IEnumerable<Team> Teams { get; set; } = [];
    }
}
