using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MVC_EF_Start.Models
{
    public class Location
    {
        [Key]
        public string zip { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public List<Candidate> Candidates { get; set; }
        public List<CommitteeDT> Committees { get; set; }
    }
}
