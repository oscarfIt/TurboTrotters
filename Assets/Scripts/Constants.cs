struct Movement
{
    public const float DETECTION_THRESHOLD = 0.1f;
}



public struct TurboBoost
{
    public const int SPEED_MULTIPLIER = 2;
    public const float DURATION = 1.5f;
}



public struct PigMass
{
    public const int MIN = 5;
    public const int MAX = 20;
    public const float SLOP_INCREASE = 2;
    public const float DISTANCE_DECREASE = 0.1f;
}


public struct PigScale
{
    public const float MIN = 1f;
    public const float MAX = 4f;
    public const float SLOP_INCREASE = 0.2f;
    public const float DISTANCE_DECREASE = 0.01f;
}