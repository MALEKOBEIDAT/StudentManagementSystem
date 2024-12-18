//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StudentManagementSystem.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Student
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Student()
        {
            this.Attachments = new HashSet<Attachment>();
        }
    
        public int StudentID { get; set; }
        public string StudentNumber { get; set; }
        public string NameArabic { get; set; }
        public string NameEnglish { get; set; }
        public string PicturePath { get; set; }
        public System.DateTime DateOfBirth { get; set; }
        public double TawjehiAverage { get; set; }
        public string SchoolName { get; set; }
        public string NationalNumber { get; set; }
        public int PhysicsGrade { get; set; }
        public int MathGrade { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Attachment> Attachments { get; set; }
    }
}
