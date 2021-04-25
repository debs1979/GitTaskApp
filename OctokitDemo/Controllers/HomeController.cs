using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using Octokit;
using OctokitDemo.Models;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace OctokitDemo.Controllers
{
    public class HomeController : Controller
    {

        // This URL uses the GitHub API to get a list of the current user's
        // repositories which include public and private repositories.
        public async Task<ActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> GetRepoDetails(string input)
        {
            var serializer = new JavaScriptSerializer();
            dynamic jsondata = serializer.Deserialize(input, typeof(object));

            //Get your variables here from AJAX call
            var GitUrl = jsondata["GitUrl"];
            var Token = jsondata["Token"];
            var UserName = jsondata["UserName"];

            try
            {
                // The following requests retrieves all of the user's repositories and
                // requires that the user be logged in to work.
                var ghe = new Uri(GitUrl);
                var client = new GitHubClient(new ProductHeaderValue("git-repo-app"))
                {
                    Credentials = new Credentials(Token)
                };
                List<IndexViewModel> list = new List<IndexViewModel>();
                var user = client.User.Get(UserName);

                var repositories = await client.Repository.GetAllForCurrent();
                IReadOnlyList<GitHubCommit> commits = null;
                foreach (var item in repositories)
                {
                    var ivm = new IndexViewModel();
                    ivm.Name = item.FullName;
                    ivm.DefaultBranch = item.DefaultBranch;
                    ivm.Commits = new List<Models.Commit>();

                    try
                    {
                        commits = await client.Repository.Commit.GetAll(item.Id);
                    }
                    catch
                    {   }
                    
                    int commitCount = commits?.Count < 10 ? commits == null ? 0 : commits.Count : 10;

                    for (int i = 0; i < commitCount; i++)
                    {
                        Models.Commit commit = new Models.Commit();
                        commit.Author = commits[i].Commit.Committer.Name;
                        commit.Message = commits[i].Commit.Message;
                        commit.ComitDate = commits[i].Commit.Committer.Date.ToString("dd-MMM-yyyy hh:mm tt");

                        ivm.Commits.Add(commit);
                    }
                    list.Add(ivm);
                }


                return Json(list);
            }
            catch (Exception ex)
            {
                // Either the accessToken is null or it's invalid. This redirects
                // to the GitHub OAuth login page. That page will redirect back to the
                // Authorize action.
                Response.StatusCode = 400;
                return Json("Error in fetching Repos");
            }
        }

        
    }
}
