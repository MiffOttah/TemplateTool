using System;
using System.Collections.Generic;
using System.Text;

namespace MiffTheFox.TemplateTool
{
    public enum TTErrorCode
    {
        Success = 0,
        Unknown = 1,
        Arguments = 2,
        OverwriteRequired = 3,
        PathNotFound = 4,
        NoTemplateMatch = 5,
        NoFilesGiven = 6
    }

    public class ErrorCodeException : Exception
    {
        public TTErrorCode ErrorCode { get; }

        public ErrorCodeException(TTErrorCode errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
