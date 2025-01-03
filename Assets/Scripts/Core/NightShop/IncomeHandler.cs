using Networking.Host;

namespace Core.NightShop
{
    public class IncomeHandler
    {
        public void GrantMoneyToPlayerById(int amount, ulong playerId)
        {
            var playerMoney = HostSingleton.Instance.GameManager.PlayerData.GetPlayerById(playerId)?.Money;
            playerMoney?.Set(amount);
        }

        public void GrantMoneyToAllPlayers(int amount)
        {
            var playerList = HostSingleton.Instance.GameManager.PlayerData.GetPlayerList();
            
            foreach (var player in playerList)
            {
                player.Money.Set(amount);
            }
        }
    }
}