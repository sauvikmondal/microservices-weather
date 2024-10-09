using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudWeather.Dataloader.Models
{
    internal class TemparatureModel
    {
        public DateTime CreatedOn { get; set; }
        public Decimal TempHighF { get; set; }
        public Decimal TempLowF { get; set; }
        public string ZipCode { get; set; }
    }
}
