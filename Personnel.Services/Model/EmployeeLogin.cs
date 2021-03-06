﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Services.Model
{
    /// <summary>
    /// Employee login
    /// </summary>
    [DataContract]
    public class EmployeeLogin
    {
        /// <summary>
        /// Employee login identifier
        /// </summary>
        [DataMember(IsRequired = false)]
        public long Id { get; set; }

        /// <summary>
        /// Employee identifier
        /// </summary>
        [DataMember(IsRequired = false)]
        public long EmployeeId { get; set; }

        /// <summary>
        /// Login
        /// </summary>
        [DataMember]
        public string Login { get; set; }
    }

    [DataContract(Name = "LoginValueResults")]
    public class LoginValueExecutionResults : BaseExecutionResults<EmployeeLogin>
    {
        public LoginValueExecutionResults() { }
        public LoginValueExecutionResults(EmployeeLogin[] e) : base(e) { }
        public LoginValueExecutionResults(Exception ex) : base(ex) { }
    }
}
