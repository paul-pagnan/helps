using System;

namespace helps.Shared.DataObjects
{ 
    public class WorkshopPreview 
    { 
        public int Id { get; set; }
        public string Name { get; set; }
        public int WorkshopSetId { get; set; }
        public string Type { get; set; }
        public string Time { get; set; }
        public DateTime Date { get; set; }
        public string DateHumanFriendly { get; set; }
        public string Location { get; set; }
        public int FilledPlaces { get; set; }
        public int TotalPlaces { get; set; }
    }
}