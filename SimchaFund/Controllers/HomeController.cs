using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimchaFund.Data;
using SimchaFund.Models;

namespace SimchaFund.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var manager = new SimchaFundManager(Properties.Settings.Default.ConStr);
            var viewModel = new IndexViewModel();
            viewModel.SimchaInfos = manager.GetSimchas().Select(simcha =>
                new SimchaInfo
                {
                    Simcha = simcha,
                    ContributorCount = manager.GetTotalContributors(),
                    SimchaContributorCount = manager.GetContributorCountForSimcha(simcha.Id),
                    SimchaContributionAmount = manager.GetContributionTotal(simcha.Id)
                });
            viewModel.Message = (string) TempData["message"];
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AddSimcha(Simcha simcha)
        {
            var manager = new SimchaFundManager(Properties.Settings.Default.ConStr);
            manager.AddSimcha(simcha);
            TempData["message"] = "New Simcha added!";
            return Redirect("/home/index");
        }

        public ActionResult Contributions(int simchaId)
        {
            var manager = new SimchaFundManager(Properties.Settings.Default.ConStr);
            ContributionsViewModel viewModel = new ContributionsViewModel();
            IEnumerable<Contributor> contributors = manager.GetContributors();
            IEnumerable<Contribution> contributions = manager.GetContributionsForSimcha(simchaId);
            IEnumerable<SimchaContribution> simchaContributions = contributors.Select(contributor =>
            {
                SimchaContribution simchaContribution = new SimchaContribution();
                Contribution contribution = contributions.FirstOrDefault(c => c.ContributorId == contributor.Id);
                if (contribution != null)
                {
                    simchaContribution.Included = true;
                    simchaContribution.Amount = contribution.Amount;
                }

                simchaContribution.Balance = manager.GetContributorBalance(contributor.Id);
                simchaContribution.Contributor = contributor;
                return simchaContribution;
            });

            viewModel.Contributions = simchaContributions;
            viewModel.Simcha = manager.GetSimcha(simchaId);
            viewModel.Message = (string) TempData["message"];
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult UpdateContributions(List<SimchaContributionPost> contributions, int simchaId)
        {
            var manager = new SimchaFundManager(Properties.Settings.Default.ConStr);
            IEnumerable<Contribution> existingContributions = manager.GetContributionsForSimcha(simchaId);
            foreach (SimchaContributionPost contribution in contributions)
            {
                Contribution existingContribution =
                    existingContributions.FirstOrDefault(c => c.ContributorId == contribution.ContributorId);
                if (existingContribution != null && existingContribution.Amount == contribution.Amount && contribution.Include)
                {
                    //person is contributing, no change
                    continue;
                }

                if (existingContribution == null && !contribution.Include)
                {
                    //person isn't contributing, no change
                    continue;
                }

                if (existingContribution != null && !contribution.Include)
                {
                    //was a contributor, decided not to contribute anymore
                    manager.DeleteContribution(simchaId, contribution.ContributorId);
                }

                else if (existingContribution == null)
                {
                    //new contributor
                    manager.AddContribution(simchaId, contribution.ContributorId, contribution.Amount);
                }

                else
                {
                    manager.UpdateContribution(simchaId, contribution.ContributorId, contribution.Amount);
                }
            }
            TempData["message"] = "Contributions updated successfully!";
            return Redirect("/home/contributions?simchaId=" + simchaId);
        }

        public ActionResult Email(int simchaId)
        {
            var manager = new SimchaFundManager(Properties.Settings.Default.ConStr);
            IEnumerable<Contribution> contributions = manager.GetContributionsForSimcha(simchaId);
            var viewModel = new EmailViewModel();
            viewModel.Contributors = contributions.Select(c =>
            {
                var contributor = manager.GetContributor(c.ContributorId);
                return contributor.FirstName + " " + contributor.LastName;
            });
            return View(viewModel);
        }
    }
}
