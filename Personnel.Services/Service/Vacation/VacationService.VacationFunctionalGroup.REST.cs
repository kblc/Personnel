using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Helpers;
using Helpers.Linq;

namespace Personnel.Services.Service.Vacation
{
    public partial class VacationService : IVacationServiceREST
    {
        /// <summary>
        /// Get information about vacation functional groups
        /// </summary>
        /// <returns>Result info</returns>
        public Model.VacationFunctionalGroupResults RESTVacationFunctionalGroupsGet() => VacationFunctionalGroupsGet();

        /// <summary>
        /// Get information about vacation functional group
        /// </summary>
        /// <param name="vacationFunctionalGroupId">Identifier</param>
        /// <returns>Result info</returns>
        public Model.VacationFunctionalGroupResult RESTVacationFunctionalGroupGet(string vacationFunctionalGroupId)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    return VacationFunctionalGroupGet(LongFromString(vacationFunctionalGroupId));
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(vacationFunctionalGroupId), vacationFunctionalGroupId);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new Model.VacationFunctionalGroupResult(ex);
                }
        }

        /// <summary>
        /// Delete single vacation functional group
        /// </summary>
        /// <param name="vacationFunctionalGroupId">Identifier</param>
        /// <returns>Result info</returns>
        public Model.BaseExecutionResult RESTVacationFunctionalGroupRemove(string vacationFunctionalGroupId) => RESTVacationFunctionalGroupRemoveRange(new string[] { vacationFunctionalGroupId });

        /// <summary>
        /// Delete vacation functional groups
        /// </summary>
        /// <param name="vacationFunctionalGroupIds">Identifier</param>
        /// <returns>Result info</returns>
        public Model.BaseExecutionResult RESTVacationFunctionalGroupRemoveRange(IEnumerable<string> vacationFunctionalGroupIds)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    return VacationFunctionalGroupRemoveRange(vacationFunctionalGroupIds.Select(i => LongFromString(i)));
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(vacationFunctionalGroupIds), vacationFunctionalGroupIds.Concat(i => $"'{i}'",", "));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new Model.VacationExecutionResult(ex);
                }
        }

        /// <summary>
        /// Update single vacation functional group
        /// </summary>
        /// <param name="vacationFunctionalGroup">Vacation functional group</param>
        /// <returns>Result info</returns>
        public Model.VacationFunctionalGroupResult RESTVacationFunctionalGroupUpdate(Model.VacationFunctionalGroup vacationFunctionalGroup) => VacationFunctionalGroupUpdate(vacationFunctionalGroup);

        /// <summary>
        /// Insert single vacation functional group
        /// </summary>
        /// <param name="vacationFunctionalGroup">Vacation functional group</param>
        /// <returns>Result info</returns>
        public Model.VacationFunctionalGroupResult RESTVacationFunctionalGroupInsert(Model.VacationFunctionalGroup vacationFunctionalGroup) => VacationFunctionalGroupInsert(vacationFunctionalGroup);
    }
}
