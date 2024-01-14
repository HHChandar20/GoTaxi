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
        private int destinationId;
        private string? name;
        private int? locationId;
        private Location? location;

        public int DestinationId
        {
            get { return destinationId; }
            set { destinationId = value; }
        }

        public string? Name
        {
            get { return name; }
            set { name = value; }
        }

        public int? LocationId
        {
            get { return locationId; }
            set { locationId = value; }
        }

        [ForeignKey("LocationId")]
        public Location? Location
        {
            get { return location; }
            set { location = value; }
        }

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