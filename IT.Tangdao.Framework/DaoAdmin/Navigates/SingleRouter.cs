using IT.Tangdao.Framework.DaoAttributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace IT.Tangdao.Framework.DaoAdmin.Navigates
{
    public class SingleRouter : ISingleRouter, INotifyPropertyChanged
    {
        private readonly ObservableCollection<ISingleNavigateView> _views;
        private readonly IReadOnlyDictionary<string, IReadOnlyList<ISingleNavigateView>> _viewGroups;
        private ISingleNavigateView _currentView;
        private int _currentIndex;
        private readonly DispatcherTimer _autoCarouselTimer;

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler NavigationChanged;

        public event EventHandler<string> GroupChanged; // 新增：组切换事件

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

        private string _currentGroupKey;

        public string CurrentGroupKey
        {
            get => _currentGroupKey;
            set
            {
                if (_currentGroupKey != value)
                {
                    _currentGroupKey = value;
                    OnPropertyChanged();
                    GroupChanged?.Invoke(this, value);
                }
            }
        }

        public string AutoRotateStatusText => IsAutoRotating ? "自动轮播开启中" : "自动轮播已禁用";

        public IReadOnlyList<ISingleNavigateView> Views => _views;

        public SingleRouter(IEnumerable<ISingleNavigateView> views)
        {
            // 参数验证
            if (views == null)
                throw new ArgumentNullException(nameof(views));

            var viewList = views.ToList();
            if (viewList.Count == 0)
                throw new ArgumentException("必须提供至少一个视图实现", nameof(views));
            // 1. 先根据特性或默认值补齐分组键
            // 1. 当场用特性或默认值计算分组键
            _viewGroups = viewList
                .GroupBy(v =>
                {
                    var attr = v.GetType().GetCustomAttribute<SingleNavigateScanAttribute>();
                    return attr?.ViewScanName ?? "Default";
                })
                .ToDictionary(
                    g => g.Key,
                    g => (IReadOnlyList<ISingleNavigateView>)g
                            .OrderBy(v => v.DisplayOrder)
                            .ToList());

            // 2. 默认启用第一个分组
            var firstKey = _viewGroups.Keys.First();
            _views = new ObservableCollection<ISingleNavigateView>(_viewGroups[firstKey]);
            CurrentView = _views.FirstOrDefault();
            // 策略优先，否则取第一组
            //  CurrentGroupKey = singleNavigateConfig?.GroupKey ?? _viewGroups.Keys.First();
            //if (!_viewGroups.TryGetValue(CurrentGroupKey, out var list))
            //    CurrentGroupKey = _viewGroups.Keys.First(); // 兜底

            //_views = new ObservableCollection<ISingleNavigateView>(list);
            // 设置默认组（第一个分组）
            // CurrentGroupKey = _viewGroups.Keys.FirstOrDefault();

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
            _ = _views.OrderBy(v => v.DisplayOrder);
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