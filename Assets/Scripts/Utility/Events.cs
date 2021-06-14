using System;
using UnityEngine.Events;

public class Events
{
    [Serializable] public class EventGameState : UnityEvent<GameManager.GameState, GameManager.GameState> { }
    [Serializable] public class EventWheatCollected : UnityEvent<int> { }
    [Serializable] public class EventUnitsLost : UnityEvent<int> { }
    [Serializable] public class EventUnitsKilled : UnityEvent<int> { }
}