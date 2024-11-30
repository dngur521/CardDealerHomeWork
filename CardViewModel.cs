using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CardDealerHomeWork
{
    public class CardViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<CardModel> _cards;
        private string _buttonContent;

        public ObservableCollection<CardModel> Cards
        {
            get => _cards;
            set
            {
                _cards = value;
                OnPropertyChanged();
            }
        }

        public string ButtonContent
        {
            get => _buttonContent;
            set
            {
                _buttonContent = value;
                OnPropertyChanged();
            }
        }

        public ICommand DealCardsCommand { get; }

        public CardViewModel()
        {
            // 초기 상태 설정
            Cards = new ObservableCollection<CardModel>(new List<CardModel>
            {
                new CardModel("ace", "clubs"),
                new CardModel("ace", "clubs"),
                new CardModel("ace", "clubs"),
                new CardModel("ace", "clubs"),
                new CardModel("ace", "clubs")
            });

            ButtonContent    = "클릭해주세요";
            DealCardsCommand = new RelayCommand(DealCards);
        }

        private void DealCards()
        {
            // 임의의 5장의 카드 생성
            var randomCards = GenerateRandomCards();

            // 카드 정렬 (낮은 숫자부터 높은 숫자까지)
            var sortedCards = SortCards(randomCards);

            // 카드 컬렉션 업데이트
            Cards = new ObservableCollection<CardModel>(sortedCards);

            // 족보 계산
            ButtonContent = CalculateHand(sortedCards);
        }

        private List<CardModel> GenerateRandomCards()
        {
            var random = new Random();
            var suits  = new[] { "spades", "diamonds", "hearts", "clubs" };
            var values = new[] { "ace", "2", "3", "4", "5", "6", "7", "8", "9", "10", "jack", "queen", "king" };

            var cards = new List<CardModel>();
            for (int i = 0; i < 5; i++)
            {
                var value = values[random.Next(values.Length)];
                var suit  = suits [random.Next(suits.Length)];
                cards.Add(new CardModel(value, suit));
            }

            return cards;
        }

        private List<CardModel> SortCards(List<CardModel> unsortedCards)
        {
            // 카드 값 순서 (낮은 것부터 높은 것까지)
            var cardOrder = new Dictionary<string, int>
            {
                { "ace",   1 },
                { "2",     2 },
                { "3",     3 },
                { "4",     4 },
                { "5",     5 },
                { "6",     6 },
                { "7",     7 },
                { "8",     8 },
                { "9",     9 },
                { "10",    10 },
                { "jack",  11 },
                { "queen", 12 },
                { "king",  13 }
            };

            return unsortedCards.OrderBy(c => cardOrder[c.Value]).ToList();
        }

        private string CalculateHand(List<CardModel> cardList)
        {
            var valueCounts = cardList.GroupBy(c => c.Value).ToDictionary(g => g.Key, g => g.Count());
            var suitCounts  = cardList.GroupBy(c => c.Suit).ToDictionary(g => g.Key, g => g.Count());

            var pairs   = valueCounts.Values.Count(v => v == 2);
            var triples = valueCounts.Values.Count(v => v == 3);
            var quads   = valueCounts.Values.Count(v => v == 4);

            if (quads == 1) 
                return "포카드";
            if (triples == 1 && pairs == 1) 
                return "풀하우스";
            if (triples == 1) 
                return "트리플";
            if (pairs == 2) 
                return "투페어";
            if (pairs == 1) 
                return "원페어";

            // 스트레이트 체크
            var sortedValues = new List<string> { "2", "3", "4", "5", "6", "7", "8", "9", "10", "jack", "queen", "king", "ace" };
            var handValues   = cardList.Select(c => c.Value).ToList();
            if (IsStraight(handValues, sortedValues)) return "스트레이트";

            // 플러쉬 체크
            if (suitCounts.Values.Any(v => v == 5)) return "플러쉬";

            return "노페어";
        }

        private bool IsStraight(List<string> hand, List<string> sortedValues)
        {
            var indexes = hand.Select(v => sortedValues.IndexOf(v)).OrderBy(i => i).ToList();
            for (int i = 1; i < indexes.Count; i++)
            {
                if (indexes[i] != indexes[i - 1] + 1) return false;
            }
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;

        public RelayCommand(Action execute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => _execute();

        public event EventHandler CanExecuteChanged;
    }
}