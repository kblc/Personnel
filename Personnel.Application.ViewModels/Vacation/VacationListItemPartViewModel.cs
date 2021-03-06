﻿using Helpers.WPF;
using Personnel.Application.ViewModels.Additional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Personnel.Application.ViewModels.Vacation
{
    public class VacationListItemPartViewModel : NotifyDisposablePropertyChangedBase
    {
        #region Data

        private System.Timers.Timer timer;

        #endregion
        #region Constructor

        public VacationListItemPartViewModel(VacationListItemViewModel owner, VacationService.Vacation vacation)
        {
            if (owner == null)
                throw new ArgumentNullException(nameof(owner));
            if (vacation == null)
                throw new ArgumentNullException(nameof(vacation));

            Owner = owner;
            Vacation = vacation;

            Owner.Owner.PropertyChanged += (_, e) => 
            {
                if (e.PropertyName == nameof(VacationsViewModel.CanManageVacations))
                    UpdateCommands();
            };

            Owner.Owner.Staffing.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(Staffing.StaffingViewModel.Current))
                    UpdateCommands();
            };

            Vacation.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(Vacation.Agreements))
                    UpdateCommands();
            };

            timer = new System.Timers.Timer(1000);
            timer.Elapsed += (_, __) => Owner.Owner.RunUnderDispatcher(new Action(() => UpdateCommands()));
            timer.Start();
        }

        #endregion
        #region Properties

        public VacationListItemViewModel Owner { get; private set; }
        public VacationService.Vacation Vacation { get; private set; }

        private bool GetCanManage()
            => GetCanManage(Owner.Owner, Vacation) && !IsDeleted && !IsBusy;

        public bool CanManage => GetCanManage();

        private bool isBusy = false;
        public bool IsBusy { get { return isBusy; } set { isBusy = value; RaisePropertyChanged(); UpdateCommands(); } }

        private bool isDeleted = false;
        public bool IsDeleted { get { return isDeleted; } set { isDeleted = value; RaisePropertyChanged(); UpdateCommands(); } }

        private string error = string.Empty;
        public string Error { get { return error; } set { error = value; RaisePropertyChanged(); RaisePropertyChanged(() => HasError); UpdateCommands(); } }

        public bool HasError => !string.IsNullOrWhiteSpace(error);

        public bool IsItGoesOver => GetIsItGoesOver(Vacation);

        private void UpdateCommands()
        {
            RaisePropertyChanged(() => CanManage);
            RaisePropertyChanged(() => IsItGoesOver);
            removeCommand?.RaiseCanExecuteChanged();
        }

        private DelegateCommand removeCommand = null;
        public ICommand RemoveCommand { get { return removeCommand ?? (removeCommand = new DelegateCommand(o => DeleteAsync(Vacation.Id), o => CanManage)); } }

        public async void DeleteAsync(long vacationId)
        {
            IsBusy = true;
            try
            {
                var srvc = new VacationService.VacationServiceClient();
                var resultTask = srvc.VacationRemoveAsync(vacationId).ContinueWith((t) => 
                    {
                        if (t.Exception != null)
                            throw t.Exception;

                        if (!string.IsNullOrEmpty(t.Result.Error))
                            throw new Exception(t.Result.Error);
                    }, 
                    CancellationToken.None, TaskContinuationOptions.AttachedToParent, TaskScheduler.FromCurrentSynchronizationContext());

                await resultTask;

                IsDeleted = true;
            }
            catch(Exception ex)
            {
                Error = ex.ToString();
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion
        #region IDisposable

        protected override void DisposeManaged()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
                timer = null;
            }
        }

        #endregion
        #region Static

        private static bool GetIsItGoesOver(VacationService.Vacation vacation) => DateTime.Now > vacation.Begin;

        internal static bool GetCanManage(VacationsViewModel vm, VacationService.Vacation vacation)
        {
            var isItMyOwnVacation = vm.Staffing.Current.Id == vacation.EmployeeId;
            var isAgrrementExists = (vacation.Agreements?.Any() ?? false);
            var canUserChengeHisOwn = isItMyOwnVacation 
                && !GetIsItGoesOver(vacation)
                && !vacation.NotUsed;
            return !isAgrrementExists && (vm.CanManageVacations || canUserChengeHisOwn);
        }

        #endregion
    }
}
