using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiffTheFox.TemplateTool
{
    public class Arguments
    {
        [Value(0, HelpText = "The paths to the file(s) to create.", Min = 1)]
        public IEnumerable<string> FileNames { get; set; }

        [Option('c', "create", HelpText = "Instead of creating a new file based on a template, creates a new template based on a file.")]
        public bool CreateFromExisting { get; set; }

        [Option('f', "force", HelpText = "Overrides the file if it exists, rather than failing. If combined with -c, overrides an existing template.")]
        public bool Force { get; set; }

        [Option('l', "list", HelpText = "Lists all available templates.")]
        public bool List { get; set; }

        [Option('t', "template", HelpText = "Override the default template selection.")]
        public string Template { get; set; }

        [Option('d', "template-directory", HelpText = "Override the default template directory.")]
        public string TemplateDirectory { get; set; }
    }
}
