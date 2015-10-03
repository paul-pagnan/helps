using System.Collections.Generic;

namespace helps.Shared.DataObjects
{ 
    public class WorkshopDetail
    { 
        public int Id { get; set; }
        public string Title { get; set; }
        public string Room { get; set; }
        public string Time { get; set; }
        public string DateHumanFriendly { get; set; }
        public string TargetGroup { get; set; }
        public string Description { get; set; }
        public int FilledPlaces { get; set; }
        public int TotalPlaces { get; set; }
        public int? ProgramId { get; set; }
        public List<SessionPreview> Sessions { get; set; }
    }
}