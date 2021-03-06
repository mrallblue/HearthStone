﻿using Engine.Action;
using Engine.Card;
using Engine.Utility;
using System;
using System.Collections.Generic;

namespace Engine.Effect
{
    /// <summary>
    /// 治疗效果
    /// </summary>
    public class HealthEffect : IAtomicEffect
    {
        /// <summary>
        /// 生命值回复表达式
        /// </summary>
        public string 生命值回复表达式 = string.Empty;
        /// <summary>
        /// 护甲回复表达式
        /// </summary>
        public string 护甲回复表达式 = string.Empty;
        /// <summary>
        /// 对英雄动作
        /// </summary>
        /// <param name="game"></param>
        /// <param name="PlayInfo"></param>
        /// <returns></returns>
        string IAtomicEffect.DealHero(ActionStatus game, Client.PublicInfo PlayInfo)
        {
            int ShieldPoint = ExpressHandler.GetEffectPoint(game, 护甲回复表达式);
            int HealthPoint = ExpressHandler.GetEffectPoint(game, 生命值回复表达式);
            PlayInfo.Hero.AfterBeShield(ShieldPoint);
            if (PlayInfo.Hero.AfterBeHealth(HealthPoint))
            {
                game.battleEvenetHandler.事件池.Add(new EventCard.全局事件()
                {
                    触发事件类型 = EventCard.事件类型枚举.治疗,
                    触发位置 = PlayInfo.Hero.战场位置
                });
            }
            return Server.ActionCode.strHealth + CardUtility.strSplitMark + PlayInfo.Hero.战场位置.ToString() + CardUtility.strSplitMark +
                        HealthPoint.ToString() + CardUtility.strSplitMark + ShieldPoint.ToString();
        }
        /// <summary>
        /// 对随从动作
        /// </summary>
        /// <param name="game"></param>
        /// <param name="Minion"></param>
        /// <returns></returns>
        string IAtomicEffect.DealMinion(ActionStatus game, MinionCard Minion)
        {
            int HealthPoint = ExpressHandler.GetEffectPoint(game, 生命值回复表达式);
            if (Minion.设置被治疗后状态(HealthPoint))
            {
                game.battleEvenetHandler.事件池.Add(new EventCard.全局事件()
                {
                    触发事件类型 = EventCard.事件类型枚举.治疗,
                    触发位置 = Minion.战场位置
                });
            }
            return Server.ActionCode.strHealth + CardUtility.strSplitMark + Minion.战场位置.ToString() +
                                                 CardUtility.strSplitMark + HealthPoint.ToString();
        }
        /// <summary>
        /// 对方复原操作
        /// </summary>
        /// <param name="game"></param>
        /// <param name="actField"></param>
        void IAtomicEffect.ReRunEffect(ActionStatus game, string[] actField)
        {
            int HealthPoint = int.Parse(actField[3]);
            if (actField[1] == CardUtility.strYou)
            {
                //MyInfo
                if (actField[2] == Client.BattleFieldInfo.HeroPos.ToString("D1"))
                {
                    game.AllRole.MyPublicInfo.Hero.AfterBeHealth(HealthPoint);
                    if (actField.Length == 5)
                    {
                        game.AllRole.MyPublicInfo.Hero.AfterBeShield(int.Parse(actField[4]));
                    }
                }
                else
                {
                    game.AllRole.MyPublicInfo.BattleField.BattleMinions[int.Parse(actField[2]) - 1].设置被治疗后状态(HealthPoint);
                }
            }
            else
            {
                //YourInfo
                if (actField[2] == Client.BattleFieldInfo.HeroPos.ToString("D1"))
                {
                    game.AllRole.YourPublicInfo.Hero.AfterBeHealth(HealthPoint);
                    if (actField.Length == 5)
                    {
                        game.AllRole.YourPublicInfo.Hero.AfterBeShield(int.Parse(actField[4]));
                    }
                }
                else
                {
                    game.AllRole.YourPublicInfo.BattleField.BattleMinions[int.Parse(actField[2]) - 1].设置被治疗后状态(HealthPoint);
                }
            }
        }
        /// <summary>
        /// 获得效果信息
        /// </summary>
        /// <param name="InfoArray"></param>
        void IAtomicEffect.GetField(List<string> InfoArray)
        {
            生命值回复表达式 = InfoArray[0];
            护甲回复表达式 = InfoArray[1];
        }
    }
}
