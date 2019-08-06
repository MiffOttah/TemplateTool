using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MiffTheFox.TemplateTool
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            var rc = TTErrorCode.Arguments;

            Parser.Default.ParseArguments<Arguments>(args)
                .WithParsed(args2 => rc = Main2(args2));

            return (int)rc;
        }

        private static TTErrorCode Main2(Arguments args)
        {
            try
            {
                // find template directory
                string templateDirectory = args.TemplateDirectory;
                if (string.IsNullOrEmpty(templateDirectory))
                {
                    if (Environment.OSVersion.Platform == PlatformID.Unix)
                    {
                        templateDirectory = Path.Combine(Environment.GetEnvironmentVariable("HOME"), ".config");
                    }
                    else
                    {
                        templateDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    }
                    templateDirectory = Path.Combine(templateDirectory, "MiffTheFox", "TemplateTool", "Templates");
                }
                Directory.CreateDirectory(templateDirectory);

                // find all templates and normalize their names
                var templates = Directory.GetFiles(templateDirectory)
                    .Select(t => Path.GetFileName(t).ToLowerInvariant())
                    .OrderBy(t => t)
                    .ToArray();

                // if we just want a list, we can end here
                if (args.List)
                {
                    foreach (var template in templates)
                    {
                        Console.Out.WriteLine(template);
                    }
                    return TTErrorCode.Success;
                }

                // make sure we have files
                if (!args.FileNames.Any())
                {
                    throw new ErrorCodeException(TTErrorCode.NoFilesGiven, "No filenames provided.");
                }

                // pre-prepare the template copy operations
                var operations = new Queue<TemplateCopyOperation>();
                foreach (string file in args.FileNames)
                {
                    string fullPath = Path.GetFullPath(file);
                    if (!Directory.Exists(Path.GetDirectoryName(fullPath))) throw new ErrorCodeException(TTErrorCode.PathNotFound, "No directory to put the file into.");

                    if (args.CreateFromExisting)
                    {
                        if (!File.Exists(fullPath)) throw new ErrorCodeException(TTErrorCode.PathNotFound, $"File not found: `{file}`");
                        string template = args.Template ?? Path.GetFileName(file);

                        string templateFile = Path.Combine(templateDirectory, template.ToLowerInvariant());
                        if (File.Exists(templateFile) && !args.Force) throw new ErrorCodeException(TTErrorCode.OverwriteRequired, $"Template already exists: `{template}`");

                        operations.Enqueue(new TemplateCopyOperation(fullPath, templateFile));
                    }
                    else
                    {
                        string template = args.Template ?? MatchTemplate(templates, file);
                        if (string.IsNullOrEmpty(template)) throw new ErrorCodeException(TTErrorCode.NoTemplateMatch, $"No matching template found for `{file}`.");

                        string templateFile = Path.Combine(templateDirectory, template.ToLowerInvariant());
                        if (!File.Exists(templateFile)) throw new ErrorCodeException(TTErrorCode.NoTemplateMatch, $"Template not found: `{template}`");
                        if (File.Exists(fullPath) && !args.Force) throw new ErrorCodeException(TTErrorCode.OverwriteRequired, $"File alredy exists: `{file}`");
                        operations.Enqueue(new TemplateCopyOperation(templateFile, fullPath));
                    }
                }

                // perform operations
                while (operations.Count > 0)
                {
                    operations.Dequeue().Copy(args.WhatIf);
                }
                return TTErrorCode.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (ex as ErrorCodeException)?.ErrorCode ?? TTErrorCode.Unknown;
            }
        }

        private static string MatchTemplate(string[] templates, string file)
        {
            string fileName = Path.GetFileName(file).ToLowerInvariant();
            for (int i = 0; i < templates.Length; i++)
            {
                if (templates[i] == fileName) return templates[i];
            }

            string extension = Path.GetExtension(file);
            if (string.IsNullOrEmpty(extension)) return null;
            for (int i = 0; i < templates.Length; i++)
            {
                if (Path.GetExtension(templates[i]) == extension) return templates[i];
            }

            return null;
        }
    }
}
