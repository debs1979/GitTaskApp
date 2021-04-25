using System.Collections.Generic;

namespace OctokitDemo.Models
{
    public class IndexViewModel
    {
        public IndexViewModel()
        {
            // Repositories = repositories;
        }

        public string Name { get; set; }
        public string DefaultBranch { get; set; }
        public List<Commit> Commits { get; set; }
    }

    public class Commit
    {
        public string Message { get; set; }
        public string Author { get; set; }
        public string ComitDate { get; set; }
    }

    public class RequestInput
    {
        public string UserName { get; set; }
        public string GitUrl { get; set; }
        public string Token { get; set; }
    }
}
