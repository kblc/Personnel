using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers.Linq;
using Personnel.Repository.Additional;
using System.ComponentModel;

namespace Personnel.Repository.Model
{
    public partial class RepositoryContext
    {
        /// <summary>
        /// Хранилище функциональных групп и их сотрудников
        /// </summary>
        public DbSet<VacationFunctionalGroupEmployee> VacationFunctionalGroupEmployees { get; set; }
    }

    /// <summary>
    /// Функциональная группа сотрудников
    /// </summary>
    [Table("vacation_functional_group_employee")]
    public class VacationFunctionalGroupEmployee : HistoryAbstractBase<long, VacationFunctionalGroupEmployee>
    {
        [Key, Column("vacation_functional_group_employee_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long VacationFunctionalGroupEmployeeId { get; set; }

        #region Employee
        /// <summary>
        /// Идентификатор сотрудника
        /// </summary>
        [ForeignKey("Employee"), Column("employee_id"), Required]
        [Index("UIX_VACATION_FUNCTIONAL_GROUP_EMPLOYEE", IsUnique = true, Order = 1)]
        public long EmployeeId { get; set; }
        /// <summary>
        /// Сотрудник
        /// </summary>
        public virtual Employee Employee { get; set; }
        #endregion
        #region VacationFunctionalGroup
        /// <summary>
        /// Идентификатор функциональной группы
        /// </summary>
        [ForeignKey("VacationFunctionalGroup"), Column("vacation_functional_group_id"), Required]
        [Index("UIX_VACATION_FUNCTIONAL_GROUP_EMPLOYEE", IsUnique = true, Order = 2)]
        public long VacationFunctionalGroupId { get; set; }
        /// <summary>
        /// Функциональная группа
        /// </summary>
        public virtual VacationFunctionalGroup VacationFunctionalGroup { get; set; }
        #endregion
    }
}
