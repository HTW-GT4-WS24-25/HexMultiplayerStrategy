using Networking.Host;

namespace NightShop
{
    public class IncomeHandler
    {
        public void GrantMoneyToPlayerById(int amount, ulong playerId)
        {
            var playerMoney = HostSingleton.Instance.GameManager.PlayerData.GetPlayerById(playerId)?.PlayerMoney;
            playerMoney?.SetMoney(amount);
        }

        public void GrantMoneyToAllPlayers(int amount)
        {
            var playerList = HostSingleton.Instance.GameManager.PlayerData.GetPlayerList();
            
            foreach (var player in playerList)
            {
                player.PlayerMoney.SetMoney(amount);
            }
        }
    }
}