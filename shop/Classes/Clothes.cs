using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shop.Classes
{
    public class Clothes
    {
        public string Photo { get; set; }
        public string Name { get; set; }
        public int Cost { get; set; }
        public int Discount { get; set; }
        public double Rating { get; set; }
        public int Count { get; set; }
        public double End { get; set; }
    }

    class Category
    {
       public string Photo { get; set; }
       public string Name { get; set; }
    }
}
