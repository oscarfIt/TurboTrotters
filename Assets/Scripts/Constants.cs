struct Movement
{
    public const float DETECTION_THRESHOLD = 0.1f;
    public const float SPEED_NUMERATOR = 100f;
    public const float KICKED_POSITION_OFFSET = 3f;
    public const float KICK_DURATION = 1f;
    public const float KICKED_HEIGHT = 2f;
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
    public const float COLLISION_FORCE = 50f;
}


public struct PigScale
{
    public const float MIN = 1f;
    public const float MAX = 4f;
    public const float SLOP_INCREASE = 0.2f;
    public const float DISTANCE_DECREASE = 0.01f;
}

public struct TerrainFriction
{
    public const float DEFAULT_DRAG = 1f;
    public const float ICE_DRAG = 0.2f;
    public const float MUD_DRAG = 5f;
    public const float DEFAULT_SPEED_MULTIPLIER = 1f;
    public const float ICE_SPEED_MULTIPLIER = 1.2f;
    public const float MUD_SPEED_MULTIPLIER = 0.8f;
}