//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BasicAPIProject.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Sec_RoleMenuUser
    {
        public int roleMenuId { get; set; }
        public int roleId { get; set; }
        public int menuId { get; set; }
        public Nullable<bool> startupPage_menuId { get; set; }
        public int userId { get; set; }
        public bool canView { get; set; }
        public bool canEdit { get; set; }
        public bool canDelete { get; set; }
        public bool canAdd { get; set; }
    }
}
