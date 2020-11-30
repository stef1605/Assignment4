using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MVC_EF_Start.Models
{
    public class Funding
    {
        [Key]
        public int trans_id { get; set; }
        public string candidate_id { get; set; }
        public float funding_received { get; set; }
        public float debts_owed { get; set; }
    }

    public class CandFunding
    {
        [Key]
        public string candidate_id { get; set; }
        public decimal funding_received { get; set; }
        public decimal debts_owed { get; set; }
    }

    public class StateFunding
    {
        public string State { get; set; }
        public decimal funding_received { get; set; }
        public decimal debts_owed { get; set; }
    }
}
