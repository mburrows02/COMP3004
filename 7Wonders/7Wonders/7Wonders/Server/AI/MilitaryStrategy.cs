﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using _7Wonders.Game_Cards;

namespace _7Wonders.Server.AI
{
    class MilitaryStrategy : AIStrategy
    {
        Player self;
        Player east;
        Player west;
        List<Player> opponents;
        Dictionary<string, int> buildPriorities;
        Dictionary<string, int> hidePriorities;
        CardColour pref = CardColour.RED;
        List<CardColour> secondPrefs = new List<CardColour> { CardColour.BROWN, CardColour.GRAY };

        public void initPriorities(int players, Player self, Player east, Player west, List<Player> opponents)
        {
            this.self = self;
            this.east = east;
            this.west = west;
            this.opponents = opponents;
            buildPriorities = new Dictionary<string, int>();
            hidePriorities = new Dictionary<string, int>();
            List<Card> cards = CardLibrary.getCardList(players);
            foreach (Card c in cards)
                calcPriorities(c);
            //initialize priorities
        }

        private int calcPriorities(Card c)
        {
            if (c.effects == null) return 0;
            int p = 0;
            if (!buildPriorities.ContainsKey(c.name))
            {
                p = 0;
                if (c.colour == pref)
                    p += 10;
                else if (secondPrefs.Contains(c.colour))
                    p += 1;
                else if (c.effects[0].type == Effect.TypeType.RCOSTEAST || c.effects[0].type == Effect.TypeType.RCOSTWEST || c.effects[0].type == Effect.TypeType.MCOST)
                    p += 2;
                else
                    foreach (Effect e in c.effects)
                        p += e.amount;

                buildPriorities.Add(c.name, p);
                recurseChain(c, p);
            }
            if (!hidePriorities.ContainsKey(c.name))
            {
                p = 0;
                hidePriorities.Add(c.name, p);
            }

            return p;
        }

        private void recurseChain(Card c, int p)
        {
            if (c.chains == null) return;
            foreach (string chain in c.chains)
            {
                buildPriorities[chain] += p;
                recurseChain(CardLibrary.getCardWithName(chain), p);
            }
        }

        public void chooseActions(Dictionary<string, ActionType> outActions, List<int> outTrades)
        {
            recalculateHidePriorities();
            Dictionary<string, ActionType> actions = new Dictionary<string, ActionType>();
            Dictionary<string, int> handBuildPriorities = new Dictionary<string, int>();
            Dictionary<string, int> handHidePriorities = new Dictionary<string, int>();
            //wonder build priority (will be standard calculation + highest hide priority)
            foreach (string c in self.getHand())
            {
                Card card = CardLibrary.getCard(c);
                int p = buildPriorities[card.name];
                int cost = ConstructionUtils.canChainBuild(self, card) ? 0 : ConstructionUtils.constructCost(self, west, east, card.cost);
                //using 0 here only for testing purposes. should be changed to > -1 once AIs can analyze trade situations
                if (cost == 0) handBuildPriorities.Add(c, p - cost);
                handHidePriorities.Add(c, 0);
            }
            string maxBuild = null;
            string maxHide = null;
            foreach (string c in handBuildPriorities.Keys)
            {
                if (maxBuild == null) maxBuild = c;
                else if (handBuildPriorities[c] > handBuildPriorities[maxBuild]) maxBuild = c;
            }
            foreach (string c in handHidePriorities.Keys)
            {
                if (maxHide == null) maxHide = c;
                else if (handHidePriorities[c] > handHidePriorities[maxHide]) maxHide = c;
            }

            outTrades.Clear();
            if (maxBuild != null)
            {
                actions.Add(maxBuild, ActionType.BUILD_CARD);
                outTrades.AddRange(chooseTrades(CardLibrary.getCard(maxBuild).cost));
            }
            else
            {
                actions.Add(maxHide, ActionType.SELL_CARD);
                outTrades.Add(0);
                outTrades.Add(0);
            }

            outActions.Clear();
            foreach (string k in actions.Keys) outActions.Add(k, actions[k]);
        }

        private void recalculateHidePriorities()
        {
            //calculate hide priorities
        }

        private List<int> chooseTrades(Dictionary<Resource, int> cost)
        {
            int eastTrade = 0;
            int westTrade = 0;

            //calculate best strategy for trading



            return new List<int> { eastTrade, westTrade };
        }
    }
}
