namespace szpont.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalStudents { get; set; }
        public int TotalTopics { get; set; }
        public int EngineeringTopics { get; set; }
        public int MastersTopics { get; set; }
        public int TotalUsers { get; set; }
        public int TotalAdmins { get; set; }
        public int TotalPromotors { get; set; }
        public int TotalDziekans { get; set; }
        public int TotalKierowniks { get; set; }
        public Dictionary<string, int> TopicsByType { get; set; } = new();
        public Dictionary<string, int> UsersByRole { get; set; } = new();
    }
}

