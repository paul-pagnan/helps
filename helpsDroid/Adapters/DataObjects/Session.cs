namespace helps.Droid.Adapters.DataObjects
{
    public class Session : MyList
    { 
        public string Name { get; set; }
        public int WorkshopSet { get; set; }
        public string WorkshopSetName { get; set; }
        public string Time { get; set; }
        public string DateHumanFriendly { get; set; }
        public string Location { get; set; }
        public int FilledPlaces { get; set; }
        public int TotalPlaces { get; set; }
    }
}