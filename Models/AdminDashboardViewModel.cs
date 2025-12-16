namespace szpont.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalStudents { get; set; }
        public int TotalTopics { get; set; }
        public int TotalUsers { get; set; }
        public int TotalAdmins { get; set; }
        public int TotalPromotors { get; set; }
        public int TotalDziekans { get; set; }
        public int TotalKierowniks { get; set; }
        public Dictionary<string, int> TopicsByType { get; set; } = new();
        public Dictionary<string, (int Value, string CssClass)> MainStats { get; set; } = new();
        public Dictionary<string, int> UserRoleStats { get; set; } = new();
    }
}

