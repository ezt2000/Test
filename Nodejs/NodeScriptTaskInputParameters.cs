using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taskmatics.TestComponents.Nodejs
{
    public class NodeScriptTaskInputParameters
    {
        [Required]
        public string Script { get; set; }
    }
}
