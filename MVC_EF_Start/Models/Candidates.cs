using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MVC_EF_Start.Models
{

    public class Candidates
    {
        public string api_version { get; set; }
        public Candidate_API[] results { get; set; }
        public Pagination pagination { get; set; }
    }

    public class Pagination
    {
        public int per_page { get; set; }
        public int pages { get; set; }
        public int page { get; set; }
        public int count { get; set; }
    }

    public class Candidate_API
    {

        public string party { get; set; }
        public string party_full { get; set; }
        public bool has_raised_funds { get; set; }
        public bool federal_funds_flag { get; set; }
        public string candidate_id { get; set; }
        public string name { get; set; }
        public string first_file_date { get; set; }
        public string last_file_date { get; set; }
        public int active_through { get; set; }
        public string incumbent_challenge_full { get; set; }
        public string candidate_status { get; set; }
        public string district { get; set; }
        public string state { get; set; }
        public string office_full { get; set; }
        public string last_f2_date { get; set; }
    }

    public class Candidate
    {
        [Key]
        public string candidate_id { get; set; }
        public string name { get; set; }
        public string party_code { get; set; }
        public string incumbent_challenge_full { get; set; }
        public string committee_id { get; set; }
        public string zip { get; set; }
        public List<Funding> Fundings { get; set; }

    }




}