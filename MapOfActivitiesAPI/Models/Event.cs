namespace MapOfActivitiesAPI.Models
{

    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TypeId { get; set; }
        public Type Type { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime Time { get; set; }
        public string Description { get; set; }
        public string Coordinates { get; set; }
    }


}
