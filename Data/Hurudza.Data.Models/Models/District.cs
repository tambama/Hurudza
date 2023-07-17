namespace Hurudza.Data.Models.Models
{
    public class District
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int ProvinceId { get; set; }
        
        public required Province Province { get; set; }
    }
}