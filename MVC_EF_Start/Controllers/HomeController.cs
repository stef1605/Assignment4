using MVC_EF_Start.DataAccess;
using MVC_EF_Start.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace MVC_EF_Start.Controllers
{
    public class HomeController : Controller
    {
        HttpClient httpClient;
        public ApplicationDbContext dbContext;

        public HomeController(ApplicationDbContext context)
        {
            dbContext = context;
        }
        
        static string BASE_URL = "https://api.open.fec.gov/v1/";
        static string API_KEY = "d1OdWVY6fL8IOGhIgjhyMPMYvXs5K3inmCAtg3Wc"; //Add your API key here inside ""

        public IActionResult Index1()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Add("X-Api-Key", API_KEY);
            httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            string ELECTION_API_PATH = BASE_URL + "candidates/?api_key=d1OdWVY6fL8IOGhIgjhyMPMYvXs5K3inmCAtg3Wc";
            string candidatesData = "";

            Candidates candidates = null;

            httpClient.BaseAddress = new Uri(ELECTION_API_PATH);


            try
            {
                HttpResponseMessage response = httpClient.GetAsync(ELECTION_API_PATH).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    candidatesData = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    async Task<ViewResult> DatabaseOperations()
                    {
                        foreach (var cand in candidates.results)
                        {
                            Candidate cand1 = new Candidate
                            {
                                name = cand.name,
                                candidate_id = cand.candidate_id
                            };
                            dbContext.Candidate.Add(cand1);
                            dbContext.SaveChanges();
                        }

                        return View();
                    }
                }

                if (!candidatesData.Equals(""))
                {
                    // JsonConvert is part of the NewtonSoft.Json Nuget package
                    candidates = JsonConvert.DeserializeObject<Candidates>(candidatesData);
                }
               
            }
            catch (Exception e)
            {
                // This is a useful place to insert a breakpoint and observe the error message
                Console.WriteLine(e.Message);
            }

            return View(candidates);
        }
        
        public IActionResult Index()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Add("X-Api-Key", API_KEY);
            httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            string ELECTION_API_PATH = BASE_URL + "committees/?api_key=d1OdWVY6fL8IOGhIgjhyMPMYvXs5K3inmCAtg3Wc";
            string committeesData = "";

            Committees committees = null;

            httpClient.BaseAddress = new Uri(ELECTION_API_PATH);


            try
            {
                HttpResponseMessage response = httpClient.GetAsync(ELECTION_API_PATH).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    committeesData = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                }

                if (!committeesData.Equals(""))
                {
                    // JsonConvert is part of the NewtonSoft.Json Nuget package
                    committees = JsonConvert.DeserializeObject<Committees>(committeesData);
                    async Task<ViewResult> DatabaseOperations()
                    {
                        foreach (var comm in committees.results)
                        {
                            CommitteeDT com = new CommitteeDT
                            {
                                name = comm.name
                            };

                            Party p = new Party
                            {
                                party_code = comm.party,
                                party_name = comm.party_full
                            };
                            dbContext.CommitteeDT.Add(com);
                            dbContext.Party.Add(p);
                            dbContext.SaveChanges();
                        }

                        return View();
                    }
                }

            }
            catch (Exception e)
            {
                // This is a useful place to insert a breakpoint and observe the error message
                Console.WriteLine(e.Message);
            }

            return View(committees);
        }
        
        public ActionResult CandidateData(string Instate,string CityIn, string PartyIn, string CommIn, string CandIn)
        {
            

            List<Location> loc = dbContext.Location.ToList();
            List<Candidate> cand = dbContext.Candidate.ToList();
            List<CommitteeDT> comm = dbContext.CommitteeDT.ToList();
            List<Party> party = dbContext.Party.ToList();
            List<Funding> fund = dbContext.Funding.ToList();

            var totfund = dbContext.Funding.AsEnumerable()
                .Select(x =>
                   new {
                       candid = x.candidate_id,
                       funding = x.funding_received,
                       debts = x.debts_owed
                   }
                 )
                 .GroupBy(s => new { s.candid })
                 .Select(g =>
                       new CandFunding
                        {
                           candidate_id = g.Key.candid,
                           funding_received = g.Sum(x => Math.Round(Convert.ToDecimal(x.funding), 2)),
                           debts_owed = g.Sum(x => Math.Round(Convert.ToDecimal(x.debts), 2)),
                       }
                 );

            List<CandFunding> cd = totfund.ToList();

            var SC_List = (from c in cand
                           join l in loc on c.zip equals l.zip
                           join co in comm on c.committee_id equals co.committee_id
                           join p in party on c.party_code equals p.party_code
                           join f in cd on c.candidate_id equals f.candidate_id
                           //group fund by c.name into g
                           //where l.state == Instate
                           //orderby l.state, l.city
                           select new StateCandidates()
                           {
                               name = c.name,
                               comname = co.name,
                               pname = p.party_name,
                               city = l.city,
                               state = l.state,
                               debts = f.debts_owed,
                               funding = f.funding_received
                           });
            if (!String.IsNullOrEmpty(Instate))
            {
                SC_List = SC_List.Where(s => s.state.Contains(Instate));
            }
            if (!String.IsNullOrEmpty(CityIn))
            {
                SC_List = SC_List.Where(s => s.city.Contains(CityIn));
            }
            if (!String.IsNullOrEmpty(PartyIn))
            {
                SC_List = SC_List.Where(s => s.pname.Contains(PartyIn));
            }
            if (!String.IsNullOrEmpty(CommIn))
            {
                SC_List = SC_List.Where(s => s.comname.Contains(CommIn));
            }
            if (!String.IsNullOrEmpty(CandIn))
            {
                SC_List = SC_List.Where(s => s.name.Contains(CandIn));
            }

            return View(SC_List);

                    }

        
        public IActionResult ChartData()
        {
            List<Location> loc = dbContext.Location.ToList();
            List<Candidate> cand = dbContext.Candidate.ToList();
            List<CommitteeDT> comm = dbContext.CommitteeDT.ToList();
            List<Party> party = dbContext.Party.ToList();
            List<Funding> fund = dbContext.Funding.ToList();

            var totfund = dbContext.Funding.AsEnumerable()
                .Select(x =>
                   new {
                       candid = x.candidate_id,
                       funding = x.funding_received,
                       debts = x.debts_owed
                   }
                 )
                 .GroupBy(s => new { s.candid })
                 .Select(g =>
                       new CandFunding
                       {
                           candidate_id = g.Key.candid,
                           funding_received = g.Sum(x => Math.Round(Convert.ToDecimal(x.funding), 2)),
                           debts_owed = g.Sum(x => Math.Round(Convert.ToDecimal(x.debts), 2)),
                       }
                 );

            List<CandFunding> cd = totfund.ToList();

            var SC_List = (from c in cand
                           join l in loc on c.zip equals l.zip
                           join co in comm on c.committee_id equals co.committee_id
                           join p in party on c.party_code equals p.party_code
                           join f in cd on c.candidate_id equals f.candidate_id
                           select new StateCandidates()
                           {
                               name = c.name,
                               comname = co.name,
                               pname = p.party_name,
                               city = l.city,
                               state = l.state,
                               debts = f.debts_owed,
                               funding = f.funding_received
                           });

            List<StateCandidates> sc = SC_List.ToList();

            var sf = sc.Select
                (x =>
                   new {
                       state = x.state,
                       funding = x.funding,
                       debts = x.debts
                   }
                 )
                 .GroupBy(s => new { s.state })
                 .Select(g =>
                       new StateFunding
                       {
                           State = g.Key.state,
                           funding_received = g.Sum(x => Math.Round(Convert.ToDecimal(x.funding), 2)),
                           debts_owed = g.Sum(x => Math.Round(Convert.ToDecimal(x.debts), 2)),
                       }
                 );

            return View();
        }

        public ViewResult Chart()
        {
            string[] ChartLabels = new string[] { "PA", "NY", "CA", "TX", "MA", "AZ", "GA", "KY", "SC", "CO" };
            int[] ChartData = new int[] { 994, 707, 615, 225, 188, 165, 151, 151, 135, 125 };



            ChartModel Model = new ChartModel
            {
                ChartType = "bar",
                Labels = String.Join(",", ChartLabels.Select(d => "'" + d + "'")),
                Data = String.Join(",", ChartData.Select(d => d)),
                Title = "TOP FUNDED STATES IN THE USA (IN MILLIONS)"
            };



            return View(Model);
        }

     public IActionResult ViewCandidates()
        {
            return View(dbContext.Candidate.ToList());
        }

    public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Candidate c)
        {
            if (ModelState.IsValid)
            {
                dbContext.Candidate.Add(c);
                dbContext.SaveChanges();
                return RedirectToAction("ViewCandidates");
            }

            return View(c);
        }

          


        public IActionResult Details(string id)
        {
            List<Location> loc = dbContext.Location.ToList();
            List<Candidate> cand = dbContext.Candidate.ToList();
            List<CommitteeDT> comm = dbContext.CommitteeDT.ToList();
            List<Party> party = dbContext.Party.ToList();
            List<Funding> fund = dbContext.Funding.ToList();

            var SC_List = (from c in cand
                           join l in loc on c.zip equals l.zip
                           join co in comm on c.committee_id equals co.committee_id
                           join p in party on c.party_code equals p.party_code
                           join f in fund on c.candidate_id equals f.candidate_id
                           //group fund by c.name into g
                           where c.candidate_id == id
                           //orderby l.state, l.city
                           select new StateCandidates()
                           {
                               name = c.name,
                               comname = co.name,
                               pname = p.party_name,
                               city = l.city,
                               state = l.state,
                               debts = Math.Round(Convert.ToDecimal(f.debts_owed), 2),
                               funding = Math.Round(Convert.ToDecimal(f.funding_received), 2)
                           });

            return View(SC_List);


        }

        public IActionResult Edit (string id)
        {
            return View(dbContext.Candidate.Where(x => x.candidate_id == id).FirstOrDefault());
        }

        [HttpPost]
        public IActionResult Edit(string id, Candidate cand)
        {
            dbContext.Entry(cand).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            dbContext.SaveChanges();

            return RedirectToAction("ViewCandidates");

        }

        public IActionResult Delete(string id)
        {
            return View(dbContext.Candidate.Where(x => x.candidate_id == id).FirstOrDefault());
        }

        [HttpPost]
        public IActionResult Delete(string id, Candidate candidate)
        {
            Candidate cand = dbContext.Candidate.Where(x => x.candidate_id == id).FirstOrDefault();
            dbContext.Candidate.Remove(cand);
            dbContext.SaveChanges();
            return View("Chart");
        }




        public IActionResult AboutUs()
        {
            return View();
        }
        public ViewResult DemoChart()
        {
            string[] ChartLabels = new string[] { "Africa", "Asia", "Europe", "Latin America", "North America" };
            int[] ChartData = new int[] { 2478, 5267, 734, 784, 433 };

            ChartModel Model = new ChartModel
            {
                ChartType = "bar",
                Labels = String.Join(",", ChartLabels.Select(d => "'" + d + "'")),
                Data = String.Join(",", ChartData.Select(d => d)),
                Title = "Predicted world population (millions) in 2050"
            };

            return View(Model);
        }


    }
}