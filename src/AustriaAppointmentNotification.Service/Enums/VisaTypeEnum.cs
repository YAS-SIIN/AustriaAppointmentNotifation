
using System.ComponentModel.DataAnnotations;

namespace AustriaAppointmentNotification.Services.Enums;

public enum VisaTypeEnum
{
    [Display(Name = "Jobseeker")]
    JobSeeker = 30782,

    [Display(Name = "NAG, residence permit, nur/only Studenten und Schüler, students and pupils")]
    Student = 23134510,

    [Display(Name = "NAG, residence permits, NO students and pupils, keine Schüler und Studenten")]
    Attachment = 13713913,

    [Display(Name = "Residence Permit 2024")]
    ResidencePermit = 21836307,

    [Display(Name = "Sondertermin, Special appointment")]
    Special = 23369141
}
