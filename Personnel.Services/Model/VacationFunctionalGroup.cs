using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Services.Model
{
    /// <summary>
    /// Vacation functional group
    /// </summary>
    [DataContract]
    [Serializable]
    public class VacationFunctionalGroup
    {
        /// <summary>
        /// Group identifier
        /// </summary>
        [DataMember(IsRequired = false)]
        public long Id { get; set; }

        /// <summary>
        /// Group name
        /// </summary>
        [DataMember(IsRequired = false)]
        public string Name { get; set; }

        /// <summary>
        /// Employee identifiers
        /// </summary>
        [DataMember(IsRequired = false)]
        public long[] EmployeIds { get; set; }
    }

    [DataContract(Name = "VacationFunctionalGroupResult")]
    public class VacationFunctionalGroupResult : BaseExecutionResult<VacationFunctionalGroup>
    {
        public VacationFunctionalGroupResult() { }
        public VacationFunctionalGroupResult(VacationFunctionalGroup e) : base(e) { }
        public VacationFunctionalGroupResult(Exception ex) : base(ex) { }
    }

    [DataContract(Name = "VacationFunctionalGroupResults")]
    public class VacationFunctionalGroupResults : BaseExecutionResults<VacationFunctionalGroup>
    {
        public VacationFunctionalGroupResults() { }
        public VacationFunctionalGroupResults(VacationFunctionalGroup[] e) : base(e) { }
        public VacationFunctionalGroupResults(Exception ex) : base(ex) { }
    }
}
