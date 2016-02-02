using Helpers;
using Helpers.WPF;
using Personnel.Application.ViewModels.Additional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Personnel.Application.ViewModels.Vacation
{
    public class VacationFunctionalGroupViewModel : NotifyPropertyChangedBase
    {
        #region Constructor

        public VacationFunctionalGroupViewModel(VacationService.VacationFunctionalGroup group, VacationsViewModel owner)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));
            if (owner == null)
                throw new ArgumentNullException(nameof(owner));

            Group = group;
            Owner = owner;
        }

        #endregion

        public VacationsViewModel Owner { get; private set; }
        public VacationService.VacationFunctionalGroup Group { get; private set; }

        private bool isEmpty = false;
        public bool IsEmpty
        {
            get { return isEmpty; }
            set { if (isEmpty == value) return; isEmpty = value; RaisePropertyChanged(); RaiseAllComamnds(); }
        }

        private bool isDeleted = false;
        public bool IsDeleted
        {
            get { return isDeleted; }
            set { if (isDeleted == value) return; isDeleted = value; RaisePropertyChanged(); RaiseAllComamnds(); }
        }

        private bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            private set { if (isBusy == value) return; isBusy = value; RaisePropertyChanged(); RaiseAllComamnds(); }
        }

        private bool isEditMode = false;
        public bool IsEditMode
        {
            get { return isEditMode; }
            private set
            {
                if (isEditMode == value)
                    return;
                isEditMode = value;

                if (isEditMode)
                    StartEdit();
                else
                    StopEdit();

                RaisePropertyChanged();
                RaiseAllComamnds();
            }
        }

        public bool HasError { get { return !string.IsNullOrWhiteSpace(Error); } }

        private string error = string.Empty;
        public string Error
        {
            get { return error; }
            internal set { if (error == value) return; error = value; RaisePropertyChanged(); RaisePropertyChanged(() => HasError); RaiseAllComamnds(); }
        }

        private void StartEdit()
        {
            var grp = new VacationService.VacationFunctionalGroup();
            grp.CopyObjectFrom(Group);
            GroupForEdit = grp;
        }

        private void StopEdit()
        {
            GroupForEdit = null;
        }

        private VacationService.VacationFunctionalGroup groupForEdit = null;
        public VacationService.VacationFunctionalGroup GroupForEdit
        {
            get { return groupForEdit; }
            private set
            {
                if (groupForEdit == value)
                    return;
                groupForEdit = value;
                RaisePropertyChanged();
            }
        }

        private void RaiseAllComamnds()
        {
            deleteCommand?.RaiseCanExecuteChanged();
            saveCommand?.RaiseCanExecuteChanged();
            editCommand?.RaiseCanExecuteChanged();
            cancelCommand?.RaiseCanExecuteChanged();
        }

        #region Commands

        private DelegateCommand deleteCommand = null;
        public ICommand DeleteCommand { get { return deleteCommand ?? (deleteCommand = new DelegateCommand(o => DeleteAsync(), o => (Owner?.CanManageVacationFunctionalGroups ?? false) && !IsDeleted && !IsBusy && !IsEmpty && !IsEditMode)); } }

        private DelegateCommand saveCommand = null;
        public ICommand SaveCommand { get { return saveCommand ?? (saveCommand = new DelegateCommand(o => SaveAsync(GroupForEdit), o => (Owner?.CanManageVacationFunctionalGroups ?? false) && !IsDeleted && !IsBusy && IsEditMode)); } }

        private DelegateCommand editCommand = null;
        public ICommand EditCommand { get { return editCommand ?? (editCommand = new DelegateCommand(o => IsEditMode = true, o => (Owner?.CanManageVacationFunctionalGroups ?? false) && !IsDeleted && !IsBusy && !IsEditMode)); } }

        private DelegateCommand cancelCommand = null;
        public ICommand CancelCommand { get { return cancelCommand ?? (cancelCommand = new DelegateCommand(o => IsEditMode = false, o => (Owner?.CanManageVacationFunctionalGroups ?? false) && !IsDeleted && !IsBusy && IsEditMode)); } }

        #endregion

        private async void DeleteAsync()
        {
            if (Group.Id == 0)
            {
                IsDeleted = true;
                return;
            }

            IsBusy = true;
            try
            {
                var sc = new VacationService.VacationServiceClient();
                var waittask = sc.VacationFunctionalGroupRemoveAsync(Group.Id);
                var task = waittask.ContinueWith(t =>
                {
                    try
                    {
                        if (t.Exception != null)
                            Error = GetExceptionText(nameof(DeleteAsync), t.Exception);
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(t.Result.Error))
                            {
                                Error = t.Result.Error;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Error = GetExceptionText(nameof(DeleteAsync), ex);
                    }
                    finally
                    {
                        try { sc.Close(); } catch { }
                    }
                },
                    System.Threading.CancellationToken.None,
                    TaskContinuationOptions.AttachedToParent,
                    TaskScheduler.FromCurrentSynchronizationContext());

                await task;

                IsEmpty = Group.Id == 0;

                if (task.Exception != null)
                    throw task.Exception;

                Error = null;
                IsBusy = false;
                IsEditMode = false;
            }
            catch (Exception ex)
            {
                IsBusy = false;
                Error = GetExceptionText(nameof(DeleteAsync), ex);
            }
        }

        private async void SaveAsync(VacationService.VacationFunctionalGroup groupToSave)
        {
            IsBusy = true;
            try
            {
                var task = Task.Factory.StartNew(() => {
                    var sc = new VacationService.VacationServiceClient();
                    try
                    {
                        var updateRes = Group.Id == 0
                            ? sc.VacationFunctionalGroupInsert(groupToSave)
                            : sc.VacationFunctionalGroupUpdate(groupToSave);

                        if (!string.IsNullOrWhiteSpace(updateRes.Error))
                        {
                            throw new Exception(updateRes.Error);
                        }
                        else
                        {
                            this.Group.CopyObjectFrom(updateRes.Value);
                        }
                    }
                    finally
                    {
                        try { sc.Close(); } catch { }
                    }
                });

                await task;

                IsEmpty = Group.Id == 0;

                if (task.Exception != null)
                    throw task.Exception;

                Error = null;
                IsBusy = false;
                IsEditMode = false;
            }
            catch (Exception ex)
            {
                IsBusy = false;
                Error = GetExceptionText(nameof(SaveAsync), ex);
            }
        }

        private string GetExceptionText(string whereCatched, Exception ex)
        {
            return ex.GetExceptionText($"{GetType().Name}.{whereCatched}()"
#if !DEBUG
                , clearText: true, includeData: false, includeStackTrace: false
#endif

                );
        }

        internal static VacationFunctionalGroupViewModel CreateEdited(VacationService.VacationFunctionalGroup group, VacationsViewModel owner)
        {
            return new VacationFunctionalGroupViewModel(group, owner) { IsEditMode = true };
        }
    }
}
