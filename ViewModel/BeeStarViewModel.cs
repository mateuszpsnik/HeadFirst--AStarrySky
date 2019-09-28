using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using HeadFirst__AStarrySky.View;
using HeadFirst__AStarrySky.Model;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using DispatcherTimer = Windows.UI.Xaml.DispatcherTimer;
using UIElement = Windows.UI.Xaml.UIElement;
using HeadFirst__AnimatedBees.View;

namespace HeadFirst__AStarrySky.ViewModel
{
    class BeeStarViewModel
    {
        private readonly ObservableCollection<UIElement>
            _sprites = new ObservableCollection<UIElement>();
        public INotifyCollectionChanged Sprites => _sprites;

        private readonly Dictionary<Star, StarControl> _stars = new Dictionary<Star, StarControl>();
        private readonly List<StarControl> _fadedStars = new List<StarControl>();

        private BeeStarModel _model = new BeeStarModel();

        private readonly Dictionary<Bee, AnimatedImage> _bees = new Dictionary<Bee, AnimatedImage>();

        private DispatcherTimer _timer = new DispatcherTimer();
        public Size PlayAreaSize
        {
            get
            {
                return _model.PlayAreaSize;
            }
            set
            {
                _model.PlayAreaSize = value;
            }
        }
        public BeeStarViewModel()
        {
            _model.BeeMoved += _model_BeeMoved;
            _model.StarChanged += _model_StarChanged;
            _timer.Interval = TimeSpan.FromSeconds(2);
            _timer.Tick += _timer_Tick;
            _timer.Start();
        }
       

        private void _timer_Tick(object sender, object e)
        {
            foreach (StarControl fadedStar in _fadedStars)
            {
                _sprites.Remove(fadedStar);
            }

            _model.Update();
        }

        private void _model_BeeMoved(object sender, BeeMovedEventArgs e)
        {
            if (!_bees.ContainsKey(e.BeeThatMoved) || (_bees.ContainsKey(e.BeeThatMoved) && _bees[e.BeeThatMoved] == null))
            {
                AnimatedImage image = 
                    BeeStarHelper.BeeFactory(e.BeeThatMoved.Width, e.BeeThatMoved.Height, 
                    TimeSpan.FromMilliseconds(50));
                BeeStarHelper.SetCanvasLocation(image, e.X, e.Y);
                _bees.Add(e.BeeThatMoved, image);
                _sprites.Add(image);
            }
            else
            {
                AnimatedImage imageToMove = _bees[e.BeeThatMoved];
                BeeStarHelper.MoveElementOnCanvas(imageToMove, e.X, e.Y);
            }
        }

        private void _model_StarChanged(object sender, StarChangedEventArgs e)
        {
            if (e.Removed && _stars.ContainsKey(e.StarThatChanged))
            {
                StarControl starRemoved = _stars[e.StarThatChanged];
                _fadedStars.Add(starRemoved);
                _stars.Remove(e.StarThatChanged);
                starRemoved.FadeOut();
            }
            else
            {
                StarControl starToBeShown;
                if (_stars.ContainsKey(e.StarThatChanged))
                  starToBeShown = _stars[e.StarThatChanged];
                else
                {
                    starToBeShown = new StarControl();
                    starToBeShown.FadeIn();
                    _sprites.Add(starToBeShown);
                    BeeStarHelper.SentToBack(starToBeShown);
                }

                BeeStarHelper.SetCanvasLocation(starToBeShown, e.StarThatChanged.Location.X, e.StarThatChanged.Location.Y);
            }
        }
    }
}
