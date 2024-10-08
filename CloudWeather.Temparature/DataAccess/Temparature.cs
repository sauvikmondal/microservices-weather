namespace CloudWeather.Temparature.DataAccess
{
    public class Temparature
    {
        public Guid Id { get; set; }    
        public DateTime CreatedOn { get; set; }
        public Decimal TempHighF { get; set;}
        public Decimal TempLowF { get; set;}
        public string ZipCode { get; set;}



    }
}
