using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taskmatics.TestComponents.MsBuild
{
    public class MsBuildTaskInputParameters
    {
        [Required]
        public string SolutionOrProjectFilePath { get; set; }

        public string Configuration { get; set; }

        public string Platform { get; set; }

        public string MsBuildExecutablePath { get; set; }
    }
}
