using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MVC_EF_Start.Models
{
    public class Committees
    {
        public string api_version { get; set; }
        public PaginationCommittee pagination { get; set; }
        public Committee[] results { get; set; }
    }

    public class PaginationCommittee
    {
        public int per_page { get; set; }
        public int count { get; set; }
        public int pages { get; set; }
        public int page { get; set; }
    }

    public class Committee
    {
        public string last_f1_date { get; set; }
        public string filing_frequency { get; set; }
        public string party { get; set; }
        public string name { get; set; }
        public string party_full { get; set; }
        public string designation_full { get; set; }
        public string treasurer_name { get; set; }
        public string organization_type { get; set; }
        public string committee_type { get; set; }
        public string organization_type_full { get; set; }
        public string committee_id { get; set; }
        public string affiliated_committee_name { get; set; }
        public string designation { get; set; }
        public string state { get; set; }
        public string committee_type_full { get; set; }
        //public List<Candidate> Candidates { get; set; }
        //public string[] candidate_ids { get; set; }
        //public string[] candidate_ids { get; set; }
        public string last_file_date { get; set; }
        public string first_file_date { get; set; }
    }

    public class Party
    {
        [Key]
        public string party_code { get; set; }
        public string party_name { get; set; }
        public List <CommitteeDT> Committees { get; set; }
        public List<Candidate> Candidates { get; set; }
    }
    public class CommitteeDT
    {
        [Key]
        public string committee_id { get; set; }
        public string name { get; set; }
        public string zip { get; set; }
        public string design { get; set; }
        public string type { get; set; }
        public string party_code { get; set; }
        public List<Candidate> Candidates { get; set; }
    }




}
