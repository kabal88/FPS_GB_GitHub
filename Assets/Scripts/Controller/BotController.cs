using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Geekbrains
{
    public sealed class BotController : BaseController, IExecute, IInitialization
    {
        private readonly int _countBot = 1;
        private readonly HashSet<Bot> _getBotList = new HashSet<Bot>();

        public void Initialization()
        {
            var targetBot = Object.Instantiate(ServiceLocatorMonoBehaviour.GetService<Reference>().BerserkerBot,
                Patrol.GenericPoint(ServiceLocatorMonoBehaviour.GetService<CharacterController>().transform),
                Quaternion.identity);

            targetBot.Agent.avoidancePriority = 0;
            targetBot.AffiliationSide = Affiliation.SideOne;
            AddBotToList(targetBot);

            for (var index = 0; index < _countBot; index++)
            {
                var tempBot = Object.Instantiate(ServiceLocatorMonoBehaviour.GetService<Reference>().Bot,
                    Patrol.GenericPoint(ServiceLocatorMonoBehaviour.GetService<CharacterController>().transform),
                    Quaternion.identity);

                tempBot.Agent.avoidancePriority = 0;
                tempBot.Target = ServiceLocatorMonoBehaviour.GetService<CharacterController>().transform;
                tempBot.Target = targetBot.transform;
                tempBot.AffiliationSide = Affiliation.SideTwo;
                //todo разных противников
                AddBotToList(tempBot);
                targetBot.Target = tempBot.transform;
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

            for (var i = 0; i < _getBotList.Count; i++)
            {
                var bot = _getBotList.ElementAt(i);
                bot.Tick();
            }
        }
    }
}
