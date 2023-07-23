using Microsoft.AspNetCore.Mvc;
using SimchaFund.data;
using SimchaFund.web.Models;
using System.Diagnostics;

namespace SimchaFund.web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=SimchaFund;Integrated Security=true;";

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Contributors()
        {
            var manager = new DbManager(_connectionString);
            var vm = new ContributorsViewModel
            {
                Contributors = manager.GetContributors(),
                Message = (string)TempData["message"]
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult AddContributor(Contributor c)
        {
            var manager = new DbManager(_connectionString);
            if (c.Id == 0)
            {
                manager.AddContributor(c);
                manager.Deposit(c.Id, c.Balance, c.Date);

                TempData["message"] = "New Contributor Added!!";
            }
            else
            {
                manager.Update(c);
                TempData["message"] = "Contributor Updated!!";
            }

            return Redirect("/home/contributors");
        }

        [HttpPost]
        public IActionResult Deposit(int id, int amount, DateTime date)
        {
            var manager = new DbManager(_connectionString);
            manager.Deposit(id, amount, date);

            return Redirect("/home/contributors");
        }

        public IActionResult History(int id)
        {
            var manager = new DbManager(_connectionString);
            string name = manager.GetName(id);
            if (name == null)
            {
                return Redirect("/home/contributors");
            }
            List<History> history = manager.GetDepositsForHistory(id);
            history.AddRange(manager.GetWithdrawlsForHistory(id));

            int total = 0;
            foreach (var h in history)
            {
                total += h.Amount;
            }

            var vm = new HistoryViewModel
            {
                History = history.OrderBy(h => h.Date).ToList(),
                Name = name,
                Balance = total
            };

            return View(vm);

        }

    }
}