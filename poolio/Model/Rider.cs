//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace poolio.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Rider
    {
        public int Id { get; set; }
        public int UserId { get; set; }
    
        public virtual Driver Driver { get; set; }
    }
}