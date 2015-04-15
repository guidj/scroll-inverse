using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScrollInverse.Core
{
    public class Device
    {
        public Device() { }

        public Device(string ID, string Name)
        {
            this.ID = ID;
            this.Name = Name;
        }

        public string ID { get; set; }

        public string Name { get; set; }
    }
}
