using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Geekbrains
{
    public sealed class BotController : BaseController, IExecute, IInitialization
    {
        private readonly int _countBotSideOne = 5;
        private readonly int _countBotSideTwo = 5;
        private float _timeBetweenSpawn = 5;
        private readonly HashSet<Bot> _getBotList = new HashSet<Bot>();

        public void Initialization()
        {

            for (int index = 0; index < _countBotSideOne; index++)
            {
                var tempBot = Object.Instantiate(ServiceLocatorMonoBehaviour.GetService<Reference>().BotTypeOne,
                    Patrol.GenericStartingPoint(ServiceLocatorMonoBehaviour.GetService<CharacterController>().transform, Affiliation.SideOne),
                    Quaternion.identity);

                tempBot.Agent.avoidancePriority = 0;
                tempBot.AffiliationSide = Affiliation.SideOne;
                AddBotToList(tempBot);
            }

            for (var index = 0; index < _countBotSideTwo; index++)
            {
                var tempBot = Object.Instantiate(ServiceLocatorMonoBehaviour.GetService<Reference>().BotTypeTwo,
                    Patrol.GenericStartingPoint(ServiceLocatorMonoBehaviour.GetService<CharacterController>().transform, Affiliation.SideTwo),
                    Quaternion.identity);

                tempBot.Agent.avoidancePriority = 0;
                tempBot.AffiliationSide = Affiliation.SideTwo;
                AddBotToList(tempBot);
            }

        }

        private void AddBotToList(Bot bot)
        {
            if (!_getBotList.Contains(bot))
            {
                _getBotList.Add(bot);
                bot.OnDieChange += RemoveBotToList;
            }
        }

        private void RemoveBotToList(Bot bot)
        {
            if (!_getBotList.Contains(bot))
            {
                return;
            }

            bot.OnDieChange -= RemoveBotToList;
            _getBotList.Remove(bot);
        }

        public void Execute()
        {
            if (!IsActive)
            {
                return;
            }

            //TimeRemainingExtensions.Add(new TimeRemaining(Initialization, _timeBetweenSpawn, false));
            //CustomDebug.Log($"_getBotList.Count for execute = {_getBotList.Count}");
            for (var i = 0; i < _getBotList.Count; i++)
            {
                var bot = _getBotList.ElementAt(i);
                bot.Tick();
            }
        }
    }
}
