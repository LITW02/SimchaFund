using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimchaFund.Data;
using SimchaFund.Models;

namespace SimchaFund.Controllers
{
    public class ContributorsController : Controller
    {
        public ActionResult Index()
        {
            var manager = new SimchaFundManager(Properties.Settings.Default.ConStr);
            var viewModel = new ContributorIndexViewModel();
            viewModel.Contributors = manager.GetContributors().Select(c =>
                new ContributorInfo
                {
                    Contributor = c,
                    Balance = manager.GetContributorBalance(c.Id)
                });
            viewModel.TotalBalance = manager.GetTotalBalance();
            viewModel.Message = (string) TempData["message"];
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Add(Contributor contributor, decimal initialDeposit)
        {
            var manager = new SimchaFundManager(Properties.Settings.Default.ConStr);
            var newId = manager.AddContributor(contributor);
            manager.AddDeposit(newId, initialDeposit, DateTime.Now);
            TempData["message"] = contributor.FirstName + " " + contributor.LastName + " added successfully";
            return Redirect("/contributors/index");
        }

        [HttpPost]
        public ActionResult Deposit(decimal amount, DateTime date, int contributorId)
        {
            var manager = new SimchaFundManager(Properties.Settings.Default.ConStr);
            manager.AddDeposit(contributorId, amount, date);
            TempData["message"] = "Deposit added successfully";
            return Redirect("/contributors/index");
        }

        [HttpPost]
        public ActionResult Edit(Contributor contributor)
        {
            var manager = new SimchaFundManager(Properties.Settings.Default.ConStr);
            manager.UpdateContributor(contributor);
            TempData["message"] = "Contributor updated successfully";
            return Redirect("/contributors/index");
        }

        public ActionResult History(int contributorId)
        {
            var manager = new SimchaFundManager(Properties.Settings.Default.ConStr);
            IEnumerable<Transaction> deposits =
                manager.GetDeposits(contributorId)
                    .Select(d => new Transaction { Action = "Deposit", Amount = d.Amount, Date = d.Date });
            IEnumerable<Transaction> contributions = manager.GetContributionsForContributor(contributorId).Select(c => new Transaction
            {
                Action = "Contribution to the " + c.SimchaName + " simcha",
                Amount = -c.Amount,
                Date = c.SimchaDate
            });

            var viewModel = new HistoryViewModel();
            viewModel.Transactions = deposits.Concat(contributions).OrderBy(t => t.Date);
            viewModel.Contributor = manager.GetContributor(contributorId);
            viewModel.Balance = manager.GetContributorBalance(contributorId);
            return View(viewModel);
        }

    }
}
