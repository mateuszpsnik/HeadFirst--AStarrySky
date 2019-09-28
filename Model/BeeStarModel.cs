using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace HeadFirst__AStarrySky.Model
{
    delegate void BeeMovedHandler(object sender, BeeMovedEventArgs e);
    delegate void StarChangedHandler(object sender, StarChangedEventArgs e);

    class BeeStarModel
    {
        public static readonly Size StarSize = new Size(150, 100);

        private readonly Dictionary<Bee, Point> _bees = new Dictionary<Bee, Point>();
        private readonly Dictionary<Star, Point> _stars = new Dictionary<Star, Point>();
        private Random _random = new Random();
        private Size _playAreaSize;

        public BeeStarModel()
        {
            _playAreaSize = Size.Empty;
        }

        public void Update()
        {
            MoveOneBee();
            AddOrRemoveAStar();
        }

        private static bool RectsOverlap(Rect r1, Rect r2)
        {
            r1.Intersect(r2);
            if (r1.Width > 0 || r1.Height > 0)
                return true;
            return false;
        }

        public Size PlayAreaSize
        {
            get
            {
                return _playAreaSize;
            }
            set
            {
                _playAreaSize = value;
                CreateBees();
                CreateStars();
            }
        }

        private void CreateBees()
        {
            if (_playAreaSize == null)
                return;

            if (_bees.Count != 0)
            {
                foreach (Bee bee in _bees.Keys.ToList())
                {
                    MoveOneBee(bee);
                }
            }
            else
            {
                int howManyBees = _random.Next(5, 16);

                for(int i = 0; i < howManyBees; i++)
                {
                    Size beeSize = new Size(_random.Next(40, 150), _random.Next(40, 150));
                    Point beeLocation = new Point(_random.Next((int)_playAreaSize.Width),
                        _random.Next((int)_playAreaSize.Height));
                    Bee bee = new Bee(beeLocation, beeSize);
                    _bees.Add(bee, beeLocation);
                    OnBeeMoved(bee, beeLocation.X, beeLocation.Y);
                }
            }
        }

        private void CreateStars()
        {
            if (_playAreaSize == null)
                return;

            if (_stars.Count != 0)
            {
                foreach (Star star in _stars.Keys)
                {
                    star.Location = FindNonOverlappingPoint(StarSize);
                    OnStarChanged(star, false);
                }
            }
            else
            {
                int howManyTimes = _random.Next(5, 11);

                for (int i = 0; i < howManyTimes; i++)
                {
                    CreateAStar();
                }
            }
        }

        private void CreateAStar()
        {
            Point point = FindNonOverlappingPoint(StarSize);
            Star starCreated = new Star(point);
            _stars.Add(starCreated, point);
            OnStarChanged(starCreated, false);
        }

        private Point FindNonOverlappingPoint(Size size)
        {
            int i = 0;
            while (i < 1000)
            {
                Point randomPoint = new Point(_random.Next((int)_playAreaSize.Width), _random.Next((int)_playAreaSize.Height));
                Rect rect = new Rect(randomPoint, size);

                var matchingBees = from bee in _bees.Keys
                                   where RectsOverlap(rect, new Rect(bee.Location, bee.Size))
                                   select bee;
                var matchingStars = from star in _stars.Keys
                                    where RectsOverlap(rect, new Rect(star.Location, StarSize))
                                    select star;

                if (matchingBees.Count() == 0 && matchingStars.Count() == 0)
                {
                    Point point = new Point(rect.Left, rect.Top);
                    return point;
                }

                i++;   
            }
            return new Point(_random.Next((int)_playAreaSize.Width), _random.Next((int)_playAreaSize.Height));
        }

        private void MoveOneBee(Bee bee = null)
        {
            if (_bees.Count() == 0)
                return;

            Bee beeToMove;
            if (bee == null)
                beeToMove = _bees.Keys.ToList()[_random.Next(_bees.Count())];
            else
                beeToMove = bee;

            _bees.Remove(beeToMove);
            Point nonOverlappingPoint = FindNonOverlappingPoint(beeToMove.Size);
            beeToMove.Location = nonOverlappingPoint;
            _bees.Add(beeToMove, nonOverlappingPoint);
            OnBeeMoved(beeToMove, nonOverlappingPoint.X, nonOverlappingPoint.Y);
        }

        private void AddOrRemoveAStar()
        {
            int coinTossed = _random.Next(2);

            if (_stars.Count() <= 5)
                CreateAStar();
            else if (_stars.Count >= 25 || coinTossed == 0)
            {
                Star starToDie = _stars.Keys.ToList()[_random.Next(_stars.Count)];
                _stars.Remove(starToDie);
                OnStarChanged(starToDie, true);
            }
            else
                CreateAStar();
        }

        public event BeeMovedHandler BeeMoved;

        protected void OnBeeMoved(Bee beeThatMoved, double x, double y)
        {
            BeeMoved?.Invoke(this, new BeeMovedEventArgs(beeThatMoved, x, y));
        }

        public event StarChangedHandler StarChanged;
        
        protected void OnStarChanged(Star starThatChanged, bool removed)
        {
            StarChanged?.Invoke(this, new StarChangedEventArgs(starThatChanged, removed));
        }

    }
}
