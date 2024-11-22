using System.Collections.Generic;
using HexSystem;

namespace Score
{
    public class ScoreCalculator
    {
        private readonly GridData _grid;
        
        public ScoreCalculator(GridData grid)
        {
            _grid = grid;
        }
        
        public Dictionary<ulong, int> CalculatePlayerScores()
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
}