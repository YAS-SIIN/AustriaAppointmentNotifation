﻿
using System.ComponentModel.DataAnnotations;

namespace AustriaAppointment.Services.Enums;

public enum ReservationTypeEnum
{
    [Display(Name = "Jobseeker")]
    JobSeeker = 30782,

    [Display(Name = "NAG, residence permit, nur/only Studenten und Schüler, students and pupils")]
    Student = 23134510,

    [Display(Name = "NAG, residence permits, NO students and pupils, keine Schüler und Studenten")]
    Attachment = 13713913,
     
    [Display(Name = "Sondertermin, Special appointment")]
    Test = 23369141
}
