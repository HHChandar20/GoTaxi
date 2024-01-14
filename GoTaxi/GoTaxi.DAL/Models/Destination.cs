using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoTaxi.DAL.Models
{
    public class Destination
    {
        public int DestinationId { get; set; }
        public string? Name { get; set; }
        public int? LocationId { get; set; }

        [ForeignKey("LocationId")]
        public Location? Location { get; set; }

        public Destination()
        {
            Name = " ";
            Location = new Location(90, 90);
        }

        public Destination(string name, double longitude, double latitude)
        {
            Name = name;
            Location = new Location(longitude, latitude);
        }
    }
}