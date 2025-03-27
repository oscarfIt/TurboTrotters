public class TurboPoints
{
    private const int maxPoints = 3;
    private const int minPoints = 0;
    private int points = 2;

    public void add()
    {
        if (points < maxPoints)
        {
            points++;
        }
    }

    public void remove()
    {
        if (points > minPoints)
        {
            points--;
        }
    }

}