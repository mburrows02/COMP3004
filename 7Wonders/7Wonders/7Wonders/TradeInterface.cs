﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace _7Wonders
{
    public class TradeInterface : Interface
    {
        private const int MARGIN = 5;
        private const float CARDRATIO = 1.612f;
        private int CARDHEIGHT;
        private int CARDWIDTH;
        private int SECTIONWIDTH;

        private static int RSIZE;

        private Button buildCard;
        private Button buildStage;
        private Button sellCard;
        private Button close;
        private Button back;
        private Button build;

        private Visual card;
        private Dictionary<string, Visual> visuals1;
        private Dictionary<string, Visual> trade;
        private Dictionary<Visual, Resource> requirements;
        private int cardSpot = 0;
        private bool disableBuild = false;
        private bool buildingCard = true;
        private bool needtrade = false;
        private int cardCost = 0;
        private TradeHelper th = new TradeHelper("", Resource.CLAY, 3, new Vector2(400,400));

        public TradeInterface(Game1 theGame)
            : base(theGame, "bg", new Vector2(Game1.WIDTH/3, Game1.HEIGHT/6), Game1.WIDTH * 2/3, Game1.HEIGHT * 2/3)
        {
            CARDHEIGHT = height - MARGIN * 2;
            CARDWIDTH = (int) (CARDHEIGHT / CARDRATIO) - MARGIN * 2;
            SECTIONWIDTH = (width - MARGIN * 4) / 3;
            RSIZE = (int)(height * 0.054f);

            visuals1 = new Dictionary<string, Visual>();
            trade = new Dictionary<string, Visual>();
            requirements = new Dictionary<Visual, Resource>();

            card = new Visual(new Vector2(pos.X + MARGIN, pos.Y + MARGIN), CARDWIDTH, CARDHEIGHT, null);
            buildCard = new Button(theGame, new Vector2(pos.X + width - 150, pos.Y + MARGIN + height * 0 / 4), 100, 50, "Card", "Font1");
            buildStage = new Button(theGame, new Vector2(pos.X + width - 150, pos.Y + MARGIN * 2 + height * 1 / 4), 100, 50, "Stage", "Font1");
            sellCard = new Button(theGame, new Vector2(pos.X + width - 150, pos.Y + MARGIN * 3 + height * 2 / 4), 100, 50, "Sell", "Font1");
            close = new Button(theGame, new Vector2(pos.X + width - 150, pos.Y + MARGIN * 4 + height* 3/4), 100, 50, "Close", "Font1");

            back = new Button(theGame, new Vector2(pos.X + width/5, pos.Y + height * 9 / 10), 100, 40, "Back", "Font1");
            build = new Button(theGame, new Vector2(pos.X + width * 3/5, pos.Y + height * 9 / 10), 100, 40, "Build", "Font1");

            visuals1.Add("buildcard", buildCard);
            visuals1.Add("buildstage", buildStage);
            visuals1.Add("sellCard", sellCard);
            visuals1.Add("card", card);
            visuals1.Add("close", close);

            trade.Add("discount", new Visual(new Vector2(pos.X + MARGIN * 2, pos.Y + MARGIN), "Requirements:", "Font1", Color.Black));
            trade.Add("costborder", new Visual(new Vector2(pos.X + MARGIN - 1, pos.Y + MARGIN - 1), SECTIONWIDTH + 2, (int)(RSIZE * 2.5f) + 2, "border").setBorder(false));
            trade.Add("label1", new Visual(new Vector2(pos.X + MARGIN * 2, pos.Y + (int)(RSIZE * 2.5) + MARGIN * 2), "West", "Font1", Color.Black));
            trade.Add("border1", new Visual(new Vector2(pos.X + MARGIN - 1, pos.Y + (int)(RSIZE * 2.5) + MARGIN * 2 - 1), SECTIONWIDTH + 2, (int)(RSIZE * 1.5f) + 2, "border").setBorder(false));
            trade.Add("label2", new Visual(new Vector2(pos.X + SECTIONWIDTH + MARGIN * 3, pos.Y + (int)(RSIZE * 2.5) + MARGIN * 2), "Self", "Font1", Color.Black));
            trade.Add("border2", new Visual(new Vector2(pos.X + SECTIONWIDTH + MARGIN * 2 - 1, pos.Y + (int)(RSIZE * 2.5) + MARGIN * 2 - 1), SECTIONWIDTH + 2, (int)(RSIZE * 1.5f) + 2, "border").setBorder(false));
            trade.Add("label3", new Visual(new Vector2(pos.X + (SECTIONWIDTH + MARGIN) * 2 + MARGIN * 2, pos.Y + (int)(RSIZE * 2.5) + MARGIN * 2), "East", "Font1", Color.Black));
            trade.Add("border3", new Visual(new Vector2(pos.X + (SECTIONWIDTH + MARGIN) * 2 + MARGIN - 1, pos.Y + (int)(RSIZE * 2.5) + MARGIN * 2 - 1), SECTIONWIDTH + 2, (int)(RSIZE * 1.5f) + 2, "border").setBorder(false));

            trade.Add("border4", new Visual(new Vector2(pos.X + (SECTIONWIDTH + MARGIN) * 0 + MARGIN - 1, pos.Y + RSIZE * 4 + MARGIN * 3 - 1), SECTIONWIDTH + 2, RSIZE * 6 + 2, "border").setBorder(false));
            trade.Add("border5", new Visual(new Vector2(pos.X + (SECTIONWIDTH + MARGIN) * 1 + MARGIN - 1, pos.Y + RSIZE * 4 + MARGIN * 3 - 1), SECTIONWIDTH + 2, RSIZE * 6 + 2, "border").setBorder(false));
            trade.Add("border6", new Visual(new Vector2(pos.X + (SECTIONWIDTH + MARGIN) * 2 + MARGIN - 1, pos.Y + RSIZE * 4 + MARGIN * 3 - 1), SECTIONWIDTH + 2, RSIZE * 6 + 2, "border").setBorder(false));

            for (int i = 0; i < 6; i++)
            {
                trade.Add("res1" + i, new Visual(new Vector2(pos.X + MARGIN * 2 + RSIZE * i, pos.Y + 04 * RSIZE + MARGIN * 3), RSIZE, RSIZE, "CLAY"));
                trade.Add("res2" + i, new Visual(new Vector2(pos.X + MARGIN * 2 + RSIZE * i, pos.Y + 05 * RSIZE + MARGIN * 3), RSIZE, RSIZE, "CLAY"));
                trade.Add("res3" + i, new Visual(new Vector2(pos.X + MARGIN * 2 + RSIZE * i, pos.Y + 06 * RSIZE + MARGIN * 3), RSIZE, RSIZE, "CLAY"));
                trade.Add("res4" + i, new Visual(new Vector2(pos.X + MARGIN * 2 + RSIZE * i, pos.Y + 07 * RSIZE + MARGIN * 3), RSIZE, RSIZE, "CLAY"));
                trade.Add("res5" + i, new Visual(new Vector2(pos.X + MARGIN * 2 + RSIZE * i, pos.Y + 08 * RSIZE + MARGIN * 3), RSIZE, RSIZE, "CLAY"));
                trade.Add("res6" + i, new Visual(new Vector2(pos.X + MARGIN * 2 + RSIZE * i, pos.Y + 09 * RSIZE + MARGIN * 3), RSIZE, RSIZE, "CLAY"));
                trade.Add("res7" + i, new Visual(new Vector2(pos.X + MARGIN * 2 + RSIZE * i, pos.Y + 10 * RSIZE + MARGIN * 4), RSIZE, RSIZE, "CLAY"));
                trade.Add("res8" + i, new Visual(new Vector2(pos.X + MARGIN * 2 + RSIZE * i, pos.Y + 11 * RSIZE + MARGIN * 4), RSIZE, RSIZE, "CLAY"));
                trade.Add("res9" + i, new Visual(new Vector2(pos.X + MARGIN * 2 + RSIZE * i, pos.Y + 12 * RSIZE + MARGIN * 4), RSIZE, RSIZE, "CLAY"));
            }

            trade.Add("back", back);
            trade.Add("build", build);

            trade.Add("rwest", new Visual(new Vector2(pos.X + width * 1 / 3, pos.Y + MARGIN), 110, 45, "rwest"));
            trade.Add("reast", new Visual(new Vector2(pos.X + width * 1 / 3, pos.Y + MARGIN), 110, 45, "reast"));
            trade.Add("rboth", new Visual(new Vector2(pos.X + width * 1 / 3, pos.Y + MARGIN), 110, 45, "rboth"));
            trade.Add("mboth", new Visual(new Vector2(pos.X + width * 2 / 3, pos.Y + MARGIN), 110, 45, "mboth"));

            for (int i = 0; i < 7; i++)
            {
                requirements.Add(new Visual(new Vector2(pos.X + MARGIN * 2 + RSIZE * i, pos.Y + MARGIN + 30), RSIZE, RSIZE, null).setVisible(false), Resource.CLAY);
            }

            activeVisuals = visuals1;
            hideTrade();
        }

        public void showTrade(string image, int _cardSpot)
        {
            cardSpot = _cardSpot;
            card.setTexture(image);
            foreach (Visual v in activeVisuals.Values)
                v.setVisible(true);

            cardCost = Game1.client.constructCost(image);
            if (CardLibrary.getCard(image).cost.ContainsKey(Resource.COIN))
            {
                needtrade = false;
            }

            if (cardCost < 0) disableBuild = true;
            else if (cardCost == 0) needtrade = false;
            else needtrade = true;
            
        }

        public void hideTrade()
        {
            foreach (Visual v in activeVisuals.Values)
                v.setVisible(false);
        }

        public override void LoadContent()
        {
            base.LoadContent();
            foreach (Visual v in visuals1.Values)
                v.LoadContent();
            foreach (Visual v in trade.Values)
                v.LoadContent();
        }

        public override void Update(GameTime gameTime, MouseState mouseState)
        {
            base.Update(gameTime, mouseState);

            if (buildCard.isClicked())
            {
                buildCard.reset();
                if (!disableBuild)
                {
                    //HACKS
                    //int cost = Game1.client.constructCost(Game1.client.getSelf().getHand()[cardSpot].getImageId());
                    //Game1.client.getSelf().addPlayed(Game1.client.getSelf().getHand()[cardSpot]);
                    //Game1.client.getSelf().getHand().RemoveAt(cardSpot);
                    //Game1.client.getSelf().addResource(Resource.COIN, -cost);
                    //
                    buildCard.reset();
                    if (needtrade)
                    {
                        buildingCard = true;
                        buildTrade();
                        activeVisuals = trade;
                    }
                    else
                    {
                        //HACKS
                        Game1.client.getSelf().addPlayed(CardLibrary.getCard(Game1.client.getSelf().getHand()[cardSpot]));
                        Game1.client.getSelf().getHand().RemoveAt(cardSpot);
                        if (CardLibrary.getCard(card.getTexture()).cost.ContainsKey(Resource.COIN))
                            Game1.client.getSelf().addResource(Resource.COIN, -CardLibrary.getCard(card.getTexture()).cost[Resource.COIN]);
                        else
                            Game1.client.getSelf().addResource(Resource.COIN, -cardCost);
                        //
                        finished = true;
                    }
                }
            }

            if (buildStage.isClicked())
            {
                //HACKS
                Game1.client.getSelf().getHand().RemoveAt(cardSpot);
                //
                buildStage.reset();
                buildingCard = false;
                finished = true;
            }

            if (sellCard.isClicked())
            {
                //HACKS
                Game1.client.getSelf().getHand().RemoveAt(cardSpot);
                Game1.client.getSelf().addResource(Resource.COIN, 3);
                //
                sellCard.reset();
                finished = true;
            }

            if (close.isClicked())
            {
                
                close.reset();
                finished = true;
            }

            if (back.isClicked())
            {
                back.reset();
                buildingCard = false;
                activeVisuals = visuals1;
            }

            if (build.isClicked())
            {
                build.reset();
                //Check if cost is met
                //activeVisuals = visuals1;
                //finished = true;
            }

            if (buildingCard)
            {
                foreach (Visual v in requirements.Keys)
                    v.Update(gameTime, mouseState);
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            if (buildingCard)
            {
                foreach (Visual v in requirements.Keys)
                    v.Draw(gameTime, spriteBatch);
            }
        }

        public override Dictionary<string, string> isFinished()
        {
            if (finished)
                return new Dictionary<string, string>();
            return null;
        }

        public override void reset()
        {
            base.reset();
            disableBuild = false;
        }

        private void buildTrade()
        {
            foreach (Visual v in requirements.Keys)
            {
                v.setVisible(false);
            }
            int i = 0;
            foreach (KeyValuePair<Resource, int> kp in CardLibrary.getCard(card.getTexture()).cost)
            {
                for (int j = 0; j < kp.Value; j++)
                {
                    Visual v = requirements.Keys.ElementAt(i).setTexture(kp.Key.ToString()).setVisible(true);
                    requirements[v] = kp.Key;
                    i++;
                }
            }
        }

        private class TradeHelper
        {
            private string neighbour;
            private int max;
            private int num = 0;
            Resource r;
            private Visual resource;
            private Visual plus;
            private Visual minus;
            private Visual total;

            public TradeHelper(string _neighbour, Resource _r, int _max, Vector2 position)
            {
                neighbour = _neighbour;
                r = _r;
                max = _max;
                resource = new Visual(position, RSIZE, RSIZE, r.ToString());

            }
        }
    }
}