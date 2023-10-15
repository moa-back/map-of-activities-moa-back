namespace MapOfActivitiesAPI.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TypeId { get; set; }
        public Type Type { get; set; }
        public string Image { get; set; }
        public DateTime Time { get; set; }
        public string Description { get; set; }
    }
}
