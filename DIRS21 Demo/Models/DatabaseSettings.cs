namespace DIRS21_Demo.Models
{
    public class DatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }

        public string BookingsCollectionName { get; set; }
        public string ServicesCollectionName { get; set; }
        public string ImagesCollectionName { get; set; }
    }
}