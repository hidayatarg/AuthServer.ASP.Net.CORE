using System;
using System.Collections.Generic;

namespace SharedLibrary.Dtos
{
    public class ErrorDto
    {
        public List<String> Errors { get; private set; }
        public bool IsShow { get; private set; }

        public ErrorDto()
        {
            Errors = new List<string>();
        }

        public ErrorDto(string error, bool isShow)
        {
            Errors.Add(error);
            IsShow = true;
        }
        
        
        public ErrorDto(List<string> errors, bool isShow)
        {
            Errors = errors;
            IsShow = isShow;
        }
        
        // * private set can be used to set via methods
        // * here we set these properties (fields) via constructor so we do not need the set
        /* code block
        public void test()
        {
            Errors = new List<string>();
        } */
    }
}