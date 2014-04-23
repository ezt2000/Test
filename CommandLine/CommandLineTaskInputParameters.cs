using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taskmatics.TestComponents.CommandLine
{
    public class CommandLineTaskInputParameters
    {
        [Required]
        public string Command { get; set; }
        public string Arguments { get; set; }
    }
}
