namespace CardDealerHomeWork;
public class CardModel
{
    public string Value { get; set; }
    public string Suit  { get; set; }
    public string ImagePath => $"Images/{Value}_of_{Suit}.png";  // 이미지 경로 제공

    public CardModel(string value, string suit)
    {
        Value = value;
        Suit  = suit;
    }
}
