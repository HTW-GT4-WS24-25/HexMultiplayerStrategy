using System;
using Networking.Host;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace NightShop
{
    public class PaymentHandler
    {
            public int TryToPurchase(int cost, PlayerDataStorage.PlayerData playerData)
            {
                var playerMoney = playerData.PlayerMoney;
                playerMoney.AttemptToPurchase(cost);
                var remainingMoney = playerData.PlayerMoney.GetMoney();

                return remainingMoney;
            }
    }
}