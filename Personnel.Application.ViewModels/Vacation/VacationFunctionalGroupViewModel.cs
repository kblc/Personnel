using Helpers;
using Helpers.WPF;
using Personnel.Application.ViewModels.Additional;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Personnel.Application.ViewModels.Vacation
{
    public class VacationFunctionalGroupEmployeePlacementViewModel : NotifyPropertyChangedBase
    {
        public VacationFunctionalGroupEmployeePlacementViewModel(VacationFunctionalGroupViewModel owner, Staffing.EmployeeViewModel employee)
        {
            Owner = owner;
            Employee = employee;
        }

        public VacationFunctionalGroupViewModel Owner { get; private set; }
        public Staffing.EmployeeViewModel Employee { get; private set; }

        private DelegateCommand dropEmployeeCommand = null;
        public ICommand DropEmployeeCommand { get { return dropEmployeeCommand ?? (dropEmployeeCommand = new DelegateCommand(o => DropEmployee(o as System.Windows.DragEventArgs), o => Owner.Owner.CanManageVacationFunctionalGroups && !IsBusy)); } }
        private void DropEmployee(System.Windows.DragEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (e.Data.GetDataPresent(typeof(Staffing.EmployeeViewModel)))
            {
                var emplVM = e.Data.GetData(typeof(Staffing.EmployeeViewModel)) as Staffing.EmployeeViewModel;
                if (emplVM != null && this.Employee == null)
                {
                    this.Employee = emplVM;
                    IsBusy = true;
                    Owner.Save(() => IsBusy = false, () => { IsBusy = false; this.Employee = null; });
                }
            }
        }

        private DelegateCommand dragOverEmployeeCommand = null;
        public ICommand DragOverEmployeeCommand { get { return dragOverEmployeeCommand ?? (dragOverEmployeeCommand = new DelegateCommand(o => DragOverEmployee(o as System.Windows.DragEventArgs), o => Owner.Owner.CanManageVacationFunctionalGroups && !IsBusy)); } }
        private void DragOverEmployee(System.Windows.DragEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (IsBusy || Employee != null || !e.Data.GetDataPresent(typeof(Staffing.EmployeeViewModel)) || e.Data.GetData(typeof(Staffing.EmployeeViewModel)) == this.Employee)
            {
                e.Effects = System.Windows.DragDropEffects.None;
                e.Handled = true;
            }
        }

        private DelegateCommand deleteCommand = null;
        public ICommand DeleteCommand {
            get { return deleteCommand ?? (deleteCommand = new DelegateCommand(o => Delete(), o => Owner.Owner.CanManageVacationFunctionalGroups && !IsBusy)); }
        }

        private void Delete()
        {
            IsBusy = true;
            Owner.Employees.Remove(this);
            Owner.Save(() => IsBusy = false, () => { IsBusy = false; Owner.Employees.Add(this); });
        }

        private bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            private set { if (isBusy == value) return; isBusy = value; RaisePropertyChanged(); RaiseAllComamnds(); }
        }

        private void RaiseAllComamnds()
        {
            dropEmployeeCommand?.RaiseCanExecuteChanged();
            dragOverEmployeeCommand?.RaiseCanExecuteChanged();
        }
    }

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

            LoadEmployees(group);
            Group.PropertyChanged += (_, e) => 
            {
                if (e.PropertyName == nameof(Group.EmployeIds))
                    LoadEmployees(Group);
            };
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

        public ObservableCollection<VacationFunctionalGroupEmployeePlacementViewModel> Employees { get; } = new ObservableCollection<VacationFunctionalGroupEmployeePlacementViewModel>();

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

        private async void SaveAsync(VacationService.VacationFunctionalGroup groupToSave, Action sucessEndAction = null, Action errorEndAction = null)
        {
            SaveEmployees(groupToSave);
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
                            return updateRes.Value;
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

                this.Group.CopyObjectFrom(task.Result);

                Error = null;
                IsBusy = false;
                IsEditMode = false;
                sucessEndAction?.Invoke();
            }
            catch (Exception ex)
            {
                IsBusy = false;
                Error = GetExceptionText(nameof(SaveAsync), ex);
                errorEndAction?.Invoke();
            }
        }

        internal void Save(Action sucessEndAction = null, Action errorEndAction = null)
        {
            SaveEmployees(this.Group);
            SaveAsync(this.Group, sucessEndAction, errorEndAction);
        }

        private string GetExceptionText(string whereCatched, Exception ex)
        {
            return ex.GetExceptionText($"{GetType().Name}.{whereCatched}()"
#if !DEBUG
                , clearText: true, includeData: false, includeStackTrace: false
#endif

                );
        }

        private void LoadEmployees(VacationService.VacationFunctionalGroup grp)
        {
            Employees.Clear();
            var ids = grp?.EmployeIds ?? new long[] { };

            foreach (var item in Owner.Staffing.Employees.Where(e => ids.Contains(e.Employee.Id)))
                Employees.Add(new VacationFunctionalGroupEmployeePlacementViewModel(this, item));

            Employees.Add(new VacationFunctionalGroupEmployeePlacementViewModel(this, null));
        }

        private void SaveEmployees(VacationService.VacationFunctionalGroup grp)
        {
            grp.EmployeIds = Employees
                .Where(e => e.Employee != null)
                .Select(e => e.Employee.Employee.Id)
                .ToArray();
        }

        internal static VacationFunctionalGroupViewModel CreateEdited(VacationService.VacationFunctionalGroup group, VacationsViewModel owner)
        {
            return new VacationFunctionalGroupViewModel(group, owner) { IsEditMode = true };
        }
    }
}
