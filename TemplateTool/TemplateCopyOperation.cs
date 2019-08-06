using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MiffTheFox.TemplateTool
{
    public class TemplateCopyOperation
    {
        public string Source { get; set; }
        public string Destination { get; set; }

        public TemplateCopyOperation(string source, string destination)
        {
            Source = source;
            Destination = destination;
        }

        public void Copy(bool whatIf)
        {
            if (whatIf)
            {
                if (File.Exists(Destination)) Console.Out.WriteLine("Delete existing file `{0}`", Destination);
                Console.Out.WriteLine("Copy `{0}` -> `{1}`", Source, Destination);
            }
            else
            {
                if (File.Exists(Destination)) File.Delete(Destination);
                File.Copy(Source, Destination);
            }
        }
    }
}
