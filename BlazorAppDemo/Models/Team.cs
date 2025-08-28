namespace BlazorAppDemo.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int Points { get; set; }
        public string Country { get; set; } // Country 
    }
}
