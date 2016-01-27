using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Personnel.Services.Service.Vacation
{
    public partial interface IVacationService
    {
        /// <summary>
        /// Get information about vacation functional groups
        /// </summary>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.VacationFunctionalGroupResults VacationFunctionalGroupsGet();

        /// <summary>
        /// Get information about vacation functional group
        /// </summary>
        /// <param name="vacationFunctionalGroupId">Identifier</param>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.VacationFunctionalGroupResult VacationFunctionalGroupGet(long vacationFunctionalGroupId);

        /// <summary>
        /// Delete single vacation functional group
        /// </summary>
        /// <param name="vacationFunctionalGroupId">Identifier</param>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.BaseExecutionResult VacationFunctionalGroupRemove(long vacationFunctionalGroupId);

        /// <summary>
        /// Delete vacation functional groups
        /// </summary>
        /// <param name="vacationFunctionalGroupIds">Identifiers</param>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.BaseExecutionResult VacationFunctionalGroupRemoveRange(IEnumerable<long> vacationFunctionalGroupIds);

        /// <summary>
        /// Update single vacation functional group
        /// </summary>
        /// <param name="vacationFunctionalGroup">Vacation functional group</param>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.VacationFunctionalGroupResult VacationFunctionalGroupUpdate(Model.VacationFunctionalGroup vacationFunctionalGroup);

        /// <summary>
        /// Insert single vacation functional group
        /// </summary>
        /// <param name="vacationFunctionalGroup">Vacation functional group</param>
        /// <returns>Result info</returns>
        [OperationContract]
        Model.VacationFunctionalGroupResult VacationFunctionalGroupInsert(Model.VacationFunctionalGroup vacationFunctionalGroup);
    }

    public partial interface IVacationServiceREST
    {
        /// <summary>
        /// Get information about vacation functional groups
        /// </summary>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "GET", UriTemplate = "/vacationFunctionalGroups",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.VacationFunctionalGroupResults RESTVacationFunctionalGroupsGet();

        /// <summary>
        /// Get information about vacation functional group
        /// </summary>
        /// <param name="vacationFunctionalGroupId">Identifier</param>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "GET", UriTemplate = "/vacationFunctionalGroup/{vacationFunctionalGroupId}",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.VacationFunctionalGroupResult RESTVacationFunctionalGroupGet(string vacationFunctionalGroupId);

        /// <summary>
        /// Delete single vacation functional group
        /// </summary>
        /// <param name="vacationFunctionalGroupId">Identifier</param>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "DELETE", UriTemplate = "/vacationFunctionalGroup/{vacationFunctionalGroupId}",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.BaseExecutionResult RESTVacationFunctionalGroupRemove(string vacationFunctionalGroupId);

        /// <summary>
        /// Delete vacation functional groups
        /// </summary>
        /// <param name="vacationFunctionalGroupIds">Identifier</param>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "POST", UriTemplate = "/vacationFunctionalGroup/remove",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.BaseExecutionResult RESTVacationFunctionalGroupRemoveRange(IEnumerable<string> vacationFunctionalGroupIds);

        /// <summary>
        /// Update single vacation functional group
        /// </summary>
        /// <param name="vacationFunctionalGroup">Vacation functional group</param>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "POST", UriTemplate = "/vacationFunctionalGroup",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.VacationFunctionalGroupResult RESTVacationFunctionalGroupUpdate(Model.VacationFunctionalGroup vacationFunctionalGroup);

        /// <summary>
        /// Insert single vacation functional group
        /// </summary>
        /// <param name="vacationFunctionalGroup">Vacation functional group</param>
        /// <returns>Result info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "PUT", UriTemplate = "/vacationFunctionalGroup",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Model.VacationFunctionalGroupResult RESTVacationFunctionalGroupInsert(Model.VacationFunctionalGroup vacationFunctionalGroup);
    }
}
