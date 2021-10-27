namespace DIRS21_Demo.Models
{
    // Single class, too few settings to justify doing it otherwise
    public class DatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }

        public string BookingsCollectionName { get; set; }
        public string ServicesCollectionName { get; set; }
        public string ImagesCollectionName { get; set; }
    }
}