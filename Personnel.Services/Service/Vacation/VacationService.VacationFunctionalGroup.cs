using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Helpers;

namespace Personnel.Services.Service.Vacation
{
    public partial class VacationService : IVacationService
    {
        /// <summary>
        /// Get information about vacation functional groups
        /// </summary>
        /// <returns>Result info</returns>
        public Model.VacationFunctionalGroupResults VacationFunctionalGroupsGet()
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login, Repository.Model.RightType.ViewVacation);

                        var dbRes = rep.Get<Repository.Model.VacationFunctionalGroup>(asNoTracking: true);
                        var res = dbRes.ToArray().Select(i => AutoMapper.Mapper.Map<Model.VacationFunctionalGroup>(i)).ToArray();
                        return new Model.VacationFunctionalGroupResults(res);
                    }
                }
                catch (Exception ex)
                {
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new Model.VacationFunctionalGroupResults(ex);
                }
        }

        /// <summary>
        /// Get information about vacation functional group
        /// </summary>
        /// <param name="vacationFunctionalGroupId">Identifier</param>
        /// <returns>Result info</returns>
        public Model.VacationFunctionalGroupResult VacationFunctionalGroupGet(long vacationFunctionalGroupId)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login, Repository.Model.RightType.ViewVacation);

                        var dbRes = rep.Get<Repository.Model.VacationFunctionalGroup>(i => i.VacationFunctionalGroupId == vacationFunctionalGroupId, asNoTracking: true);
                        var res = AutoMapper.Mapper.Map<Model.VacationFunctionalGroup>(dbRes.SingleOrDefault());
                        return new Model.VacationFunctionalGroupResult(res);
                    }
                }
                catch (Exception ex)
                {
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
        public Model.BaseExecutionResult VacationFunctionalGroupRemove(long vacationFunctionalGroupId) => VacationFunctionalGroupRemoveRange(new long[] { vacationFunctionalGroupId });

        /// <summary>
        /// Delete vacation functional groups
        /// </summary>
        /// <param name="vacationFunctionalGroupIds">Identifiers</param>
        /// <returns>Result info</returns>
        public Model.BaseExecutionResult VacationFunctionalGroupRemoveRange(IEnumerable<long> vacationFunctionalGroupIds)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var current = SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login, Repository.Model.RightType.ManageVacationFunctionalGroups);
                        var res = rep.Get<Repository.Model.VacationFunctionalGroup>(e => vacationFunctionalGroupIds.Contains(e.VacationFunctionalGroupId)).ToArray();
                        rep.RemoveRange(res);
                        return new Model.BaseExecutionResult();
                    }
                }
                catch (Exception ex)
                {
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new Model.BaseExecutionResult(ex);
                }
        }

        /// <summary>
        /// Update single vacation functional group
        /// </summary>
        /// <param name="vacationFunctionalGroup">Vacation functional group</param>
        /// <returns>Result info</returns>
        public Model.VacationFunctionalGroupResult VacationFunctionalGroupUpdate(Model.VacationFunctionalGroup vacationFunctionalGroup)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var current = SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login, Repository.Model.RightType.ManageVacation);

                        var res = rep.Get<Repository.Model.Vacation>(e => e.VacationId == vacationFunctionalGroup.Id).SingleOrDefault();
                        if (res == null)
                            throw new KeyNotFoundException();

                        var newDbValue = AutoMapper.Mapper.Map<Repository.Model.VacationFunctionalGroup>(vacationFunctionalGroup);
                        res.CopyObjectFrom(newDbValue);

                        rep.SaveChanges();
                        
                        return new Model.VacationFunctionalGroupResult(AutoMapper.Mapper.Map<Model.VacationFunctionalGroup>(res));
                    }
                }
                catch (Exception ex)
                {
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new Model.VacationFunctionalGroupResult(ex);
                }
        }

        /// <summary>
        /// Insert single vacation
        /// </summary>
        /// <param name="vacation">Vacation</param>
        /// <returns>Result info</returns>
        public Model.VacationFunctionalGroupResult VacationFunctionalGroupInsert(Model.VacationFunctionalGroup vacationFunctionalGroup)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var current = SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login, Repository.Model.RightType.ManageVacation);
                        var newDbValue = AutoMapper.Mapper.Map<Repository.Model.VacationFunctionalGroup>(vacationFunctionalGroup);
                        newDbValue.VacationFunctionalGroupId = 0;
                        rep.Add(newDbValue);
                        return new Model.VacationFunctionalGroupResult(AutoMapper.Mapper.Map<Model.VacationFunctionalGroup>(newDbValue));
                    }
                }
                catch (Exception ex)
                {
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new Model.VacationFunctionalGroupResult(ex);
                }
        }
    }
}
