﻿namespace JB.Job.DTOs.Job
{
    public class ApplicationRequest
    {
        public int JobId { get; set; }
        public int? CVId { get; set; }
        public string CVPDFUrl { get; set; }
        public string[] Attachments { get; set; }
        public string Introdution { get; set; }
    }
}
