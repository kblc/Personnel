﻿using Helpers;
using Helpers.Linq;
using Helpers.WPF;
using Personnel.Application.ViewModels.AdditionalModels;
using Personnel.Application.ViewModels.ServiceWorkers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Personnel.Application.ViewModels.Vacation
{
    public enum VacationViewForm
    {
        ManageVacations = 0,
        ManageVacationFunctionalGroups
    };

    public class VacationsViewModel : DependencyObject, INotifyPropertyChanged
    {
        private const string MANAGEVACATION = "MANAGEVACATION";
        private const string MANAGEVACATIONFUNCTIONALGROUPS = "MANAGEVACATIONFUNCTIONALGROUPS";
        private const string VACATIONLEVELPLAN = "PLAN";
        private const string VACATIONLEVELFACT = "FACT";
        private const string VACATIONLEVELTRANSFER = "TRANSFER";

        private readonly ServiceWorkers.VacationWorker worker = new ServiceWorkers.VacationWorker();

        private NotifyCollection<VacationService.VacationLevel> levels = new NotifyCollection<VacationService.VacationLevel>();
        public IReadOnlyNotifyCollection<VacationService.VacationLevel> Levels => levels;

        private NotifyCollection<VacationService.Vacation> vacations = new NotifyCollection<VacationService.Vacation>();
        public IReadOnlyNotifyCollection<VacationService.Vacation> Vacations => vacations;

        private NotifyCollection<VacationService.VacationBalance> vacationBalances = new NotifyCollection<VacationService.VacationBalance>();
        public IReadOnlyNotifyCollection<VacationService.VacationBalance> VacationBalances => vacationBalances;

        private NotifyCollection<VacationListItemViewModel> employeeVacations = new NotifyCollection<VacationListItemViewModel>();
        public IReadOnlyNotifyCollection<VacationListItemViewModel> EmployeeVacations => employeeVacations;

        public ICollectionView EmployeeVacationsCollectionView { get; private set; }

        private NotifyCollection<VacationFunctionalGroupViewModel> vacationFunctionalGroups = new NotifyCollection<VacationFunctionalGroupViewModel>();
        public IReadOnlyNotifyCollection<VacationFunctionalGroupViewModel> VacationFunctionalGroups => vacationFunctionalGroups;

        #region Notifications

        public static readonly DependencyProperty NotificationsProperty = DependencyProperty.Register(nameof(Notifications), typeof(Notifications.NotificationsViewModel),
            typeof(VacationsViewModel), new PropertyMetadata(null, (s, e) => { }));

        public Notifications.NotificationsViewModel Notifications
        {
            get { return (Notifications.NotificationsViewModel)GetValue(NotificationsProperty); }
            set { SetValue(NotificationsProperty, value); }
        }

        #endregion
        #region History

        public static readonly DependencyProperty HistoryProperty = DependencyProperty.Register(nameof(History), typeof(History.HistoryViewModel),
            typeof(VacationsViewModel), new PropertyMetadata(null, (s, e) =>
            {
                var model = s as VacationsViewModel;
                var historyNewvalue = e.NewValue as History.HistoryViewModel;
                var historyOldvalue = e.OldValue as History.HistoryViewModel;
                if (model != null)
                {
                    if (historyOldvalue != null)
                        historyOldvalue.AsyncChanged -= model.OnHistoryChanged;
                    if (historyNewvalue != null)
                        historyNewvalue.AsyncChanged += model.OnHistoryChanged;
                }
            }));

        public History.HistoryViewModel History
        {
            get { return (History.HistoryViewModel)GetValue(HistoryProperty); }
            set { SetValue(HistoryProperty, value); }
        }

        #endregion
        #region Staffing

        public static readonly DependencyProperty StaffingProperty = DependencyProperty.Register(nameof(Staffing), typeof(Staffing.StaffingViewModel),
            typeof(VacationsViewModel), new PropertyMetadata(null, (s, e) =>
            {
                var model = s as VacationsViewModel;
                var staffingNewvalue = e.NewValue as Staffing.StaffingViewModel;
                var staffingOldvalue = e.OldValue as Staffing.StaffingViewModel;
                if (model != null)
                {
                    if (staffingOldvalue != null) {
                        staffingOldvalue.OnRightsChanged -= model.OnRightsChanged;
                        staffingOldvalue.OnCurrentChanged -= model.OnCurrentChanged;
                        staffingNewvalue.Employees.CollectionChanged -= model.EmployeesCollectionChanged;
                    }
                    if (staffingNewvalue != null)
                    {
                        staffingNewvalue.OnRightsChanged += model.OnRightsChanged;
                        staffingNewvalue.OnCurrentChanged += model.OnCurrentChanged;
                        staffingNewvalue.Employees.CollectionChanged += model.EmployeesCollectionChanged;
                    }
                }
            }));

        public Staffing.StaffingViewModel Staffing
        {
            get { return (Staffing.StaffingViewModel)GetValue(StaffingProperty); }
            set { SetValue(StaffingProperty, value); }
        }

        #endregion
        #region From

        private DateTime? from;
        public DateTime? From
        {
            get { return from; }
            private set { from = value; RaisePropertyChanged(); }
        }

        #endregion
        #region To

        private DateTime? to;
        public DateTime? To
        {
            get { return to; }
            private set { to = value; RaisePropertyChanged(); }
        }

        #endregion
        #region Year

        public static readonly DependencyProperty YearProperty = DependencyProperty.Register(nameof(Year), typeof(int),
            typeof(VacationsViewModel), new PropertyMetadata(default(int), (s, e) => {
                var model = (VacationsViewModel)s;
                var newYear = (int)e.NewValue;

                var from = new DateTime(newYear, 1, 1, 0, 0, 0, 0);
                var to = new DateTime(newYear + 1, 1, 1, 0, 0, 0, 0);

                model.From = from;
                model.To = to;
                model.worker.setPeriod(from, to);
                model.UpdateCommands();
            }));

        public int Year
        {
            get { return (int)GetValue(YearProperty); }
            set { SetValue(YearProperty, value); }
        }

        #endregion
        #region IsLoaded

        private static readonly DependencyPropertyKey ReadOnlyIsLoadedPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(IsLoaded), typeof(bool), typeof(VacationsViewModel),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) =>
                    {
                        var model = s as VacationsViewModel;
                        if (model != null)
                            model.RaiseOnIsLoadedChanged((bool)e.NewValue);
                    })));
        public static readonly DependencyProperty ReadOnlyIsLoadedProperty = ReadOnlyIsLoadedPropertyKey.DependencyProperty;

        public bool IsLoaded
        {
            get { return (bool)GetValue(ReadOnlyIsLoadedProperty); }
            private set { SetValue(ReadOnlyIsLoadedPropertyKey, value); RaiseOnIsLoadedChanged(value); }
        }

        #endregion
        #region Error

        private static readonly DependencyPropertyKey ReadOnlyErrorPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(Error), typeof(string), typeof(VacationsViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyErrorProperty = ReadOnlyErrorPropertyKey.DependencyProperty;

        public string Error
        {
            get { return (string)GetValue(ReadOnlyErrorProperty); }
            private set { SetValue(ReadOnlyErrorPropertyKey, value); }
        }

        #endregion
        #region State

        private static readonly DependencyPropertyKey ReadOnlyStatePropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(State), typeof(ServiceWorkers.WorkerState), typeof(VacationsViewModel),
                new FrameworkPropertyMetadata(ServiceWorkers.WorkerState.None,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyStateProperty = ReadOnlyStatePropertyKey.DependencyProperty;

        public ServiceWorkers.WorkerState State
        {
            get { return (ServiceWorkers.WorkerState)GetValue(ReadOnlyStateProperty); }
            private set { SetValue(ReadOnlyStatePropertyKey, value); }
        }
        #endregion
        #region ConnectionTimeInterval

        public static readonly DependencyProperty ConnectionTimeIntervalProperty = DependencyProperty.Register(nameof(ConnectionTimeInterval), typeof(TimeSpan),
            typeof(VacationsViewModel), new PropertyMetadata(TimeSpan.FromSeconds(ServiceWorkers.AbstractBaseWorker.DefaultConnectionTimeIntervalIsSeconds), (s, e) =>
            {
                var model = s as VacationsViewModel;
                if (model != null)
                    model.worker.ConnectionTimeInterval = (TimeSpan)e.NewValue;
            }));

        public TimeSpan ConnectionTimeInterval
        {
            get { return (TimeSpan)GetValue(ConnectionTimeIntervalProperty); }
            set { SetValue(ConnectionTimeIntervalProperty, value); }
        }
        #endregion
        #region ServiceCultureInfo

        public static readonly DependencyProperty ServiceCultureInfoProperty = DependencyProperty.Register(nameof(ServiceCultureInfo), typeof(CultureInfo),
            typeof(VacationsViewModel), new PropertyMetadata(System.Threading.Thread.CurrentThread.CurrentUICulture, (s, e) =>
            {
                var model = s as VacationsViewModel;
                if (model != null)
                    model.worker.ServiceCultureInfo = (CultureInfo)e.NewValue;
            }));

        public CultureInfo ServiceCultureInfo
        {
            get { return (CultureInfo)GetValue(ServiceCultureInfoProperty); }
            set { SetValue(ServiceCultureInfoProperty, value); }
        }
        #endregion
        #region IsActive

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(nameof(IsActive), typeof(bool),
            typeof(VacationsViewModel), new PropertyMetadata(false, (s, e) =>
            {
                var model = s as VacationsViewModel;
                if (model != null && (bool)e.NewValue != (bool)e.OldValue)
                {
                    if ((bool)e.NewValue)
                        model.worker.Start();
                    else
                        model.worker.Stop();
                }
            }));

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        #endregion
        #region IsDebugView

        public static readonly DependencyProperty IsDebugViewProperty = DependencyProperty.Register(nameof(IsDebugView), typeof(bool),
            typeof(VacationsViewModel), new PropertyMetadata(true, (s, e) =>
            {
                var model = s as VacationsViewModel;
                if (model != null)
                    model.RaisePropertyChanged(e.Property.Name);
            }));

        public bool IsDebugView
        {
            get { return (bool)GetValue(IsDebugViewProperty); }
            set { SetValue(IsDebugViewProperty, value); RaisePropertyChanged(); }
        }
        #endregion
        #region CanManageVacations

        private static readonly DependencyPropertyKey ReadOnlyCanManageVacationsPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(CanManageVacations), typeof(bool), typeof(VacationsViewModel),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) =>
                    {
                        var model = s as VacationsViewModel;
                        if (model != null)
                        {
                            model.RaisePropertyChanged(e.Property.Name);
                            model.UpdateCommands();
                        }
                    })));
        public static readonly DependencyProperty ReadOnlyCanManageVacationsProperty = ReadOnlyCanManageVacationsPropertyKey.DependencyProperty;

        public bool CanManageVacations
        {
            get { return (bool)GetValue(ReadOnlyCanManageVacationsProperty); }
            protected set { SetValue(ReadOnlyCanManageVacationsPropertyKey, value); RaisePropertyChanged(); UpdateCommands(); }
        }

        private bool GetCanManageVacationsProperty()
        {
            var res = false;
            if (Staffing != null && Staffing.Current != null)
            {
                var canManageRight = Staffing.Rights.FirstOrDefault(r => string.Compare(r.SystemName, MANAGEVACATION, true) == 0);
                if (canManageRight != null)
                    res = Staffing.Current.Rights.Any(r => r.RightId == canManageRight.Id);
            }
            return res;
        }

        #endregion
        #region Commands

        private void UpdateCommands()
        {
            insertVacationCommand?.RaiseCanExecuteChanged();
            editVacationCommand?.RaiseCanExecuteChanged();
            increaseYearCommand?.RaiseCanExecuteChanged();
            decreaseYearCommand?.RaiseCanExecuteChanged();
            manageFunctionalGroupsCommand?.RaiseCanExecuteChanged();
            manageVacationsCommand?.RaiseCanExecuteChanged();
        }

        private DelegateCommand insertVacationFunctionalGroupCommand = null;
        public ICommand InsertVacationFunctionalGroupCommand { get { return insertVacationFunctionalGroupCommand ?? (insertVacationFunctionalGroupCommand = new DelegateCommand(o => InsertVacationFunctionalGroup(), o => CanManageVacationFunctionalGroups)); } }

        private void InsertVacationFunctionalGroup()
        {
            var g = new VacationService.VacationFunctionalGroup()
            {
                Name = "Новая функциональная группа",
            };
            var gvm = VacationFunctionalGroupViewModel.CreateEdited(g, this);
            gvm.PropertyChanged += (_, e) => 
            {
                var checkList = new[] { nameof(gvm.IsDeleted), nameof(gvm.IsEditMode) };
                if (checkList.Contains(e.PropertyName) && gvm.Group.Id == 0)
                {
                    if (vacationFunctionalGroups.Contains(gvm))
                        vacationFunctionalGroups.Remove(gvm);
                }
                if (e.PropertyName == nameof(gvm.IsChecked))
                {
                    EmployeeVacationsCollectionView.Refresh();
                }
            };
            vacationFunctionalGroups.Add(gvm);
        }

        private DelegateCommand insertVacationCommand = null;
        public ICommand InsertVacationCommand { get { return insertVacationCommand ?? (insertVacationCommand = new DelegateCommand(o => InsertVacation((long?)o), o => (Staffing?.Current?.Id ?? 0) == ((long?)o) || CanManageVacations)); } }

        private void InsertVacation(long? employeeId)
        {
            var levelPlan = Levels.FirstOrDefault(l => string.Compare(VACATIONLEVELPLAN, l.SystemName, true) == 0);
            var newVac = new VacationViewModel(this, new VacationService.Vacation()
            {
                EmployeeId = employeeId ?? Staffing.Current.Id,
                VacationLevelId = levelPlan.Id,
                Begin = DateTime.Now,
                DayCount = 1
            }, true);
            SelectedVacationForEdit = newVac;
        }

        private DelegateCommand editVacationCommand = null;
        public ICommand EditVacationCommand { get { return editVacationCommand ?? (editVacationCommand = new DelegateCommand(o => EditVacation((long?)o), o => (Staffing?.Current?.Id ?? 0) == ((long?)o) || CanManageVacations)); } }

        private void EditVacation(long? vacationId)
        {
            var vac = vacations.FirstOrDefault(v => v.Id == vacationId);
            var newVac = new VacationViewModel(this, vac, true);
            SelectedVacationForEdit = newVac;
        }

        private DelegateCommand increaseYearCommand = null;
        public ICommand IncreaseYearCommand { get { return increaseYearCommand ?? (increaseYearCommand = new DelegateCommand(o => Year = Year + 1, o => IsLoaded)); } }

        private DelegateCommand decreaseYearCommand = null;
        public ICommand DecreaseYearCommand { get { return decreaseYearCommand ?? (decreaseYearCommand = new DelegateCommand(o => Year = Year - 1, o => IsLoaded)); } }

        private DelegateCommand viewAllEmployeeVacationsCommand = null;
        public ICommand ViewAllEmployeeVacationsCommand { get { return viewAllEmployeeVacationsCommand ?? (viewAllEmployeeVacationsCommand = new DelegateCommand(o => ViewAllEmployeeVacations((long)o), o => Staffing?.Current != null)); } }

        private DelegateCommand hideAllEmployeeVacationsCommand = null;
        public ICommand HideAllEmployeeVacationsCommand { get { return hideAllEmployeeVacationsCommand ?? (hideAllEmployeeVacationsCommand = new DelegateCommand(o => SelectedEmployeeVacationsForView = null)); } }

        private void ViewAllEmployeeVacations(long employeeId)
        {
            var vm = EmployeeVacations.FirstOrDefault(v => v.Employee.Employee.Id == employeeId);
            SelectedEmployeeVacationsForView = vm;
        }

        private DelegateCommand manageFunctionalGroupsCommand = null;
        public ICommand ManageFunctionalGroupsCommand { get { return manageFunctionalGroupsCommand ?? (manageFunctionalGroupsCommand = new DelegateCommand(o => 
        {
            ViewForm = VacationViewForm.ManageVacationFunctionalGroups;
        }, o => CanManageVacationFunctionalGroups)); } }

        private DelegateCommand manageVacationsCommand = null;
        public ICommand ManageVacationsCommand
        {
            get
            {
                return manageVacationsCommand ?? (manageVacationsCommand = new DelegateCommand(o =>
                {
                    ViewForm = VacationViewForm.ManageVacations;
                }, o => CanManageVacationFunctionalGroups));
            }
        }

        private DelegateCommand filterVacationsCommand = null;
        public ICommand FilterVacationsCommand
        {
            get
            {
                return filterVacationsCommand ?? (filterVacationsCommand = new DelegateCommand(o =>
                {
                    var fea = (System.Windows.Data.FilterEventArgs)o;
                    FilterEvent(fea);
                }));
            }
        }

        private void FilterEvent(System.Windows.Data.FilterEventArgs e)
        {
            if (vacationFunctionalGroups.Any(i => i.IsChecked))
            {
                var validEmployeeIds = vacationFunctionalGroups
                    .Where(i => i.IsChecked)
                    .SelectMany(e2 => e2.Group.EmployeIds)
                    .Distinct()
                    .ToArray();

                var employeeVacation = e.Item as VacationListItemViewModel;
                e.Accepted = validEmployeeIds.Contains(employeeVacation?.Employee?.Employee?.Id ?? 0);
            }
        }

        #endregion

        private bool GetCanManageVacationFunctionalGroupsProperty()
        {
            var res = false;
            if (Staffing != null && Staffing.Current != null)
            {
                var canManageRight = Staffing.Rights.FirstOrDefault(r => string.Compare(r.SystemName, MANAGEVACATIONFUNCTIONALGROUPS, true) == 0);
                if (canManageRight != null)
                    res = Staffing.Current.Rights.Any(r => r.RightId == canManageRight.Id);
            }
            return res;
        }

        private bool canManageVacationFunctionalGroups = false;
        public bool CanManageVacationFunctionalGroups
        {
            get { return canManageVacationFunctionalGroups; }
            private set { canManageVacationFunctionalGroups = value; RaisePropertyChanged(); }
        }

        private VacationViewForm viewForm = VacationViewForm.ManageVacations;
        public VacationViewForm ViewForm
        {
            get { return viewForm; }
            private set { viewForm = value; RaisePropertyChanged(); }
        }

        private Timer updateCurrentDateTimeTimer = new Timer(1000);
        public DateTime CurrentDateTime { get { return DateTime.Now; } }

        private VacationViewModel selectedVacationForEdit;
        public VacationViewModel SelectedVacationForEdit
        {
            get { return selectedVacationForEdit; }
            set { if (selectedVacationForEdit == value) return; selectedVacationForEdit = value; RaisePropertyChanged(); }
        }

        private VacationListItemViewModel selectedEmployeeVacationsForView = null;
        public VacationListItemViewModel SelectedEmployeeVacationsForView
        {
            get { return selectedEmployeeVacationsForView; }
            set { if (selectedEmployeeVacationsForView == value) return; selectedEmployeeVacationsForView = value; RaisePropertyChanged(); }
        }

        internal void RunUnderDispatcher(Delegate a) => Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, a);

        public VacationsViewModel()
        {
            worker.CopyObjectTo(this);
            worker.OnErrorChanged += (s, e) => RunUnderDispatcher(new Action(() => Error = e));
            worker.OnLoadedChanged += (s, e) => RunUnderDispatcher(new Action(() => IsLoaded = e));
            worker.OnStateChanged += (s, e) => RunUnderDispatcher(new Action(() => State = e));
            worker.OnNotification += (s, e) => RunUnderDispatcher(new Action(() => Notifications?.Add(e)));
            worker.OnVacationLevelChanged += (s,e) => RunUnderDispatcher(new Action(() => OnWorkerVacationLevelChanged(s, e)));
            worker.OnVacationBalanceChanged += (s, e) => RunUnderDispatcher(new Action(() => OnWorkerVacationBalanceChanged(s, e)));
            worker.OnVacationChanged += (s, e) => RunUnderDispatcher(new Action(() => OnWorkerVacationChanged(s, e)));
            worker.OnVacationFunctionalGroupChanged += (s, e) => RunUnderDispatcher(new Action(() => OnWorkerVacationFunctionalGroupChanged(s, e)));
            UpdateRights();
            Year = DateTime.Now.Year;
            ReloadEmployeeVacations();
            updateCurrentDateTimeTimer.Elapsed += (s,e) => RaisePropertyChanged(nameof(CurrentDateTime));
            EmployeeVacationsCollectionView = CollectionViewSource.GetDefaultView(employeeVacations);
            EmployeeVacationsCollectionView.SortDescriptions.Add(new SortDescription("Employee.Department.Name", ListSortDirection.Ascending));
            EmployeeVacationsCollectionView.SortDescriptions.Add(new SortDescription("Employee.Employee.Stuffing.Position", ListSortDirection.Ascending));
            EmployeeVacationsCollectionView.Filter = new Predicate<object>(o =>
            {
                if (vacationFunctionalGroups.Any(i => i.IsChecked))
                {
                    var validEmployeeIds = vacationFunctionalGroups
                        .Where(i => i.IsChecked)
                        .SelectMany(e2 => e2.Group.EmployeIds)
                        .Distinct()
                        .ToArray();

                    var employeeVacation = o as VacationListItemViewModel;
                    return validEmployeeIds.Contains(employeeVacation?.Employee?.Employee?.Id ?? 0);
                }
                return true;
            });
        }

        private void UpdateRights()
        {
            CanManageVacations = GetCanManageVacationsProperty();
            CanManageVacationFunctionalGroups = GetCanManageVacationFunctionalGroupsProperty();
        }

        private void OnHistoryChanged(object sender, HistoryService.History e) => RunUnderDispatcher(new Action(() => worker.ApplyHistoryChanges(e)));
        private void OnRightsChanged(object semder, ListItemsEventArgs<StaffingService.Right> e) => RunUnderDispatcher(new Action(() => UpdateRights()));
        private void OnCurrentChanged(object semder, StaffingService.Employee e) => RunUnderDispatcher(new Action(() => UpdateRights()));

        private void OnWorkerVacationLevelChanged(object sender, ListItemsEventArgs<VacationService.VacationLevel> e)
        {
            if (new[] { ChangeAction.Add, ChangeAction.Change }.Contains(e.Action))
            {
                foreach (var d in e.Items)
                {
                    if (d.Id != 0)
                    {
                        var existedLevel = levels.FirstOrDefault(l => l.Id == d.Id);
                        if (existedLevel != null)
                        {
                            existedLevel.CopyObjectFrom(d);
                        }
                        else
                        {
                            levels.Add(d);
                        }
                    }
                }
            } else
            {
                foreach (var d in e.Items)
                {
                    if (d.Id != 0)
                    {
                        var existedLevel = levels.FirstOrDefault(l => l.Id == d.Id);
                        if (existedLevel != null)
                            levels.Remove(existedLevel);
                    }
                }
            }

            OnVacationLevelChanged?.Invoke(this, e);
        }
        private void OnWorkerVacationBalanceChanged(object sender, ListItemsEventArgs<VacationService.VacationBalance> e)
        {
            if (new[] { ChangeAction.Add, ChangeAction.Change }.Contains(e.Action))
            {
                foreach (var d in e.Items)
                {
                    if (d.Id != 0)
                    {
                        var existedBalance = vacationBalances.FirstOrDefault(l => l.Id == d.Id);
                        if (existedBalance != null)
                        {
                            existedBalance.CopyObjectFrom(d);
                        }
                        else
                        {
                            vacationBalances.Add(d);
                        }
                    }
                }
            }
            else
            {
                foreach (var d in e.Items)
                {
                    if (d.Id != 0)
                    {
                        var existedBalance = vacationBalances.FirstOrDefault(l => l.Id == d.Id);
                        if (existedBalance != null)
                            vacationBalances.Remove(existedBalance);
                    }
                }
            }

            OnVacationBalanceChanged?.Invoke(this, e);
        }
        private void OnWorkerVacationChanged(object sender, ListItemsEventArgs<VacationService.Vacation> e)
        {
            if (new[] { ChangeAction.Add, ChangeAction.Change }.Contains(e.Action))
            {
                foreach (var d in e.Items)
                {
                    if (d.Id != 0)
                    {
                        var existedVacation = vacations.FirstOrDefault(l => l.Id == d.Id);
                        if (existedVacation != null)
                        {
                            existedVacation.CopyObjectFrom(d);
                        }
                        else
                        {
                            vacations.Add(d);
                        }
                    }
                }
            }
            else
            {
                foreach (var d in e.Items)
                {
                    if (d.Id != 0)
                    {
                        var existedVacation = vacations.FirstOrDefault(l => l.Id == d.Id);
                        if (existedVacation != null)
                            vacations.Remove(existedVacation);
                    }
                }
            }

            OnVacationChanged?.Invoke(this, e);
        }
        private void OnWorkerVacationFunctionalGroupChanged(object sender, ListItemsEventArgs<VacationService.VacationFunctionalGroup> e)
        {
            if (new[] { ChangeAction.Add, ChangeAction.Change }.Contains(e.Action))
            {
                foreach (var d in e.Items)
                {
                    if (d.Id != 0)
                    {
                        var existedVacationFunctionalGroup = vacationFunctionalGroups.FirstOrDefault(l => l.Group.Id == d.Id);
                        if (existedVacationFunctionalGroup != null)
                        {
                            existedVacationFunctionalGroup.Group.CopyObjectFrom(d);
                        }
                        else
                        {
                            var vm = new VacationFunctionalGroupViewModel(d, this);
                            vm.PropertyChanged += (_, e2) => 
                            {
                                if (e2.PropertyName == nameof(vm.IsChecked))
                                {
                                    EmployeeVacationsCollectionView.Refresh();
                                }
                            };
                            vacationFunctionalGroups.Add(vm);
                        }
                    }
                }
            }
            else
            {
                foreach (var d in e.Items)
                {
                    if (d.Id != 0)
                    {
                        var existedVacationFunctionalGroup = vacationFunctionalGroups.FirstOrDefault(l => l.Group.Id == d.Id);
                        if (existedVacationFunctionalGroup != null)
                            vacationFunctionalGroups.Remove(existedVacationFunctionalGroup);
                    }
                }
            }

            OnVacationFunctionalGroupChanged?.Invoke(this, e);
        }

        private void RaiseOnIsLoadedChanged(bool value)
        {
            RunUnderDispatcher(new Action(() => OnIsLoadedChanged?.Invoke(this, value)));
        }

        private void ReloadEmployeeVacations()
        {
            employeeVacations.Clear();
            if (Staffing != null)
                foreach(var i in Staffing.Employees.Select(e => new VacationListItemViewModel(e, this)))
                    employeeVacations.Add(i);
        }

        private void EmployeesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (var i in e.OldItems.Cast<Staffing.EmployeeViewModel>().Join(employeeVacations, evm => evm, evvm => evvm.Employee, (evm, evvm) => evvm).ToArray())
                    employeeVacations.Remove(i);

            if (e.NewItems != null)
                foreach (var i in e.NewItems.Cast<Staffing.EmployeeViewModel>().Select(vm => new VacationListItemViewModel(vm, this)))
                    employeeVacations.Add(i);
        }

        public event EventHandler<bool> OnIsLoadedChanged;
        public event EventHandler<ListItemsEventArgs<VacationService.VacationLevel>> OnVacationLevelChanged;
        public event EventHandler<ListItemsEventArgs<VacationService.VacationBalance>> OnVacationBalanceChanged;
        public event EventHandler<ListItemsEventArgs<VacationService.Vacation>> OnVacationChanged;
        public event EventHandler<ListItemsEventArgs<VacationService.VacationFunctionalGroup>> OnVacationFunctionalGroupChanged;

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged([ParenthesizePropertyName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
