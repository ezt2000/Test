using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taskmatics.TestComponents.Git
{
    public class GitPullTaskInputParameters
    {
        [Required]
        public string WorkingDirectoryPath { get; set; }

        public string RepositoryUrl { get; set; }
    }
}
