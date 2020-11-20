using System.Collections.Generic;

public static class PoolBallsContainer 
{
   public static HashSet<Ball> PoolBallHashSet { get; private set; }
    public static void Register(Ball ball)
    {
        if (PoolBallHashSet == null)
            PoolBallHashSet = new HashSet<Ball>();

        PoolBallHashSet.Add(ball);
    }

    public static void Deregister(Ball ball)
    {
        PoolBallHashSet.Remove(ball);
        
    }

    public static void RemoveAll()
    {
        PoolBallHashSet.Clear();
    }

}
