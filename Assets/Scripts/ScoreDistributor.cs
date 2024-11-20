using System.Collections.Generic;
using System.Linq;
using HexSystem;
using Networking.Host;

public class ScoreDistributor
{
    private readonly GridData _grid;
        
    public ScoreDistributor(GridData grid)
    {
        _grid = grid;

        GameEvents.DAY_NIGHT_CYCLE.OnSwitchedCycleState += cycleState =>
        {
            if (cycleState == DayNightCycle.CycleState.Night) DistributePlayerScores();
        };
    }

    private void DistributePlayerScores()
    {
        var scoresByPlayerId = CalculatePlayerScores();

        foreach (var (id, score) in scoresByPlayerId)
        {
            HostSingleton.Instance.GameManager.PlayerData.IncrementPlayerScore(id, score);
        } 
    }
        
    private Dictionary<ulong, int> CalculatePlayerScores()
    {
        var scoresByPlayerId = new Dictionary<ulong, int>();

        foreach (var hex in _grid.GetAllHexData())
        {
            var controllerId = hex.ControllerPlayerId;
                
            if (controllerId == null) continue;

            scoresByPlayerId[controllerId.Value] = scoresByPlayerId.TryGetValue(controllerId.Value, out var score) 
                ? score + 1 
                : 1;
        }
            
        return scoresByPlayerId;
    }
}