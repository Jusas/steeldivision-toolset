using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataExtractor.DataModels
{
    public class DivisionModel
    {
        public int Id { get; set; }
        public string DivisionName { get; set; }
        public string DivisionNickName { get; set; }
        public int[] PhaseIncome { get; set; }
        public int Nationality { get; set; }
    }
}
