using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace IT.Tangdao.Framework.DaoAdmin.Navigates
{
    public class SingleRouter : ISingleRouter, INotifyPropertyChanged
    {
        private readonly ObservableCollection<ISingleNavigateView> _views;
        private ISingleNavigateView _currentView;
        private int _currentIndex;
        private readonly DispatcherTimer _autoCarouselTimer;

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler NavigationChanged;

        public ISingleNavigateView CurrentView
        {
            get => _currentView;
            private set
            {
                if (_currentView != value)
                {
                    _currentView = value;
                    OnPropertyChanged();
                    OnNavigationChanged();
                }
            }
        }

        public bool CanPrevious => _currentIndex > 0;
        public bool CanNext => _currentIndex < _views.Count - 1;

        public bool IsAutoRotating
        {
            get => _autoCarouselTimer.IsEnabled;
            set
            {
                if (_autoCarouselTimer.IsEnabled != value)
                {
                    if (value)
                        _autoCarouselTimer.Start();
                    else
                        _autoCarouselTimer.Stop();

                    OnPropertyChanged();
                    OnPropertyChanged(nameof(AutoRotateStatusText));
                }
            }
        }

        public string AutoRotateStatusText => IsAutoRotating ? "自动轮播开启中" : "自动轮播已禁用";

        public IReadOnlyList<ISingleNavigateView> Views => _views;

        public SingleRouter(IEnumerable<ISingleNavigateView> views)
        {
            _views = new ObservableCollection<ISingleNavigateView>(
                views.OrderBy(v => v.DisplayOrder).ToList());

            CurrentView = _views.FirstOrDefault();

            _autoCarouselTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            _autoCarouselTimer.Tick += OnAutoCarouselTick;
        }

        public void Previous()
        {
            if (!CanPrevious) return;

            CurrentView = _views[--_currentIndex];
            RefreshNavigationState();
        }

        public void Next()
        {
            if (!CanNext) return;

            CurrentView = _views[++_currentIndex];
            RefreshNavigationState();
        }

        public void ToggleAutoCarousel()
        {
            IsAutoRotating = !IsAutoRotating;
        }

        public void NavigateTo(ISingleNavigateView view)
        {
            var index = _views.IndexOf(view);
            if (index >= 0)
            {
                NavigateToIndex(index);
            }
        }

        public void NavigateToIndex(int index)
        {
            if (index >= 0 && index < _views.Count)
            {
                _currentIndex = index;
                CurrentView = _views[index];
                RefreshNavigationState();
            }
        }

        public void AddView(ISingleNavigateView view)
        {
            _views.Add(view);
            _views.OrderBy(v => v.DisplayOrder);
            RefreshNavigationState();
        }

        public void RemoveView(ISingleNavigateView view)
        {
            if (_views.Remove(view))
            {
                if (_currentView == view)
                {
                    CurrentView = _views.FirstOrDefault();
                    _currentIndex = 0;
                }
                RefreshNavigationState();
            }
        }

        private void OnAutoCarouselTick(object sender, EventArgs e)
        {
            // 循环导航
            _currentIndex = (_currentIndex + 1) % _views.Count;
            CurrentView = _views[_currentIndex];
            RefreshNavigationState();
        }

        private void RefreshNavigationState()
        {
            OnPropertyChanged(nameof(CanPrevious));
            OnPropertyChanged(nameof(CanNext));
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnNavigationChanged()
        {
            NavigationChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            _autoCarouselTimer.Stop();
            _autoCarouselTimer.Tick -= OnAutoCarouselTick;
        }
    }
}