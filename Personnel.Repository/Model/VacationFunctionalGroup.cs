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
        /// Хранилище функциональных групп сотрудников
        /// </summary>
        public DbSet<VacationFunctionalGroup> VacationFunctionalGroups { get; set; }
    }

    /// <summary>
    /// Функциональная группа сотрудников
    /// </summary>
    [Table("vacation_functional_group")]
    public class VacationFunctionalGroup : HistoryAbstractBase<long, VacationFunctionalGroup>
    {
        public VacationFunctionalGroup()
        {
            VacationFunctionalGroupEmployees = new HashSet<VacationFunctionalGroupEmployee>();
        }

        [Key, Column("vacation_functional_group_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long VacationFunctionalGroupId { get; set; }

        [Column("name"), Required, MaxLength(400)]
        public string Name { get; set; }

        /// <summary>
        /// Состав группы
        /// </summary>
        public virtual ICollection<VacationFunctionalGroupEmployee> VacationFunctionalGroupEmployees { get; set; }
    }
}
