﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace _7Wonders
{
    class Lobby : Interface
    {
        protected const int MARGIN = 5;
        protected const int CHECKBOXDIM = 15;
        protected const int DIVIDERWIDTH = 2;
        protected int SEC1WIDTH = Game1.WIDTH / 3;
        protected int WONDERHEIGHT = (Game1.HEIGHT - 10) / 6;
        protected int WONDERWIDTH = Game1.WIDTH / 3 - 10;
        protected int SEC1HEIGHT = Game1.HEIGHT * 2/3;
        protected int DROPDOWNWIDTH = (Game1.WIDTH / 3) - 100;
        protected int DROPDOWNHEIGHT = (Game1.HEIGHT*2/3 - (Game1.MAXPLAYER + 1) * MARGIN) / Game1.MAXPLAYER;        

        protected Dictionary<String, Visual> visuals1;
        protected List<Checkbox> readyCBs;
        protected Dictionary<String, Visual> wonders;
        protected List<string> playerTypes = new List<string>() { "Open", "AIType1", "AIType2", "AIType3" };
        protected List<Visual> dropDowns;

        protected Button sideButton;
        protected Button backButton;

        protected const int NUMPLAYERS = 7;
        protected bool random = false;
        protected bool onlyA = false;
        protected bool viewSideB = false;
        
        public Lobby(Game1 theGame)
            : base(theGame, "title", 0.4f)
        {
            sideButton = new Button(game, new Vector2(Game1.WIDTH - 140, Game1.HEIGHT - 140), 140, 40, "Toggle Side", "Font1");
            backButton = new Button(game, new Vector2(10, Game1.HEIGHT - 100), 75, 40, "Back", "Font1");

            readyCBs = new List<Checkbox>();
            for (int i = 0; i < NUMPLAYERS; i++)
            {
                readyCBs.Add(new Checkbox(game, new Vector2(50 + DROPDOWNWIDTH, 20 + (MARGIN + DROPDOWNHEIGHT) * i), CHECKBOXDIM, CHECKBOXDIM));
            }

            wonders = new Dictionary<String, Visual>();

            visuals1 = new Dictionary<String, Visual>();
            visuals1.Add("Divider1", new Visual(game, new Vector2(SEC1WIDTH - 1, 0), DIVIDERWIDTH, Game1.HEIGHT, "line", Color.Silver));
            visuals1.Add("Divider2", new Visual(game, new Vector2(0, SEC1HEIGHT - 1), Game1.WIDTH, DIVIDERWIDTH, "line", Color.Silver));

            dropDowns = new List<Visual>();
            dropDowns.Add((new DropDown(game, new Vector2(MARGIN, MARGIN), DROPDOWNWIDTH, DROPDOWNHEIGHT, new List<string>() { "Host" })).setEnabled(false));

            for (int i = 1; i < NUMPLAYERS; i++)
            {
                dropDowns.Add(new DropDown(game, new Vector2(MARGIN, MARGIN + (MARGIN + DROPDOWNHEIGHT) * i), DROPDOWNWIDTH, DROPDOWNHEIGHT, playerTypes).setEnabled(false));
            }

            for (int i = dropDowns.Count; i > 0; i--)
            {
                visuals1.Add("drop" + i, dropDowns[i - 1]);
            }

            for (int i = 0; i < readyCBs.Count; i++)
            {
                visuals1.Add("ready" + i, readyCBs[i]);
            }

            int count = 0;
            int count2 = 1;
            //need to work on this so that it adapts better to number of wonders
            foreach (Wonder w in Game1.wonders.Values)
            {
                w.getVisual().setPosition(new Vector2(5 + SEC1WIDTH * count2, 5 + WONDERHEIGHT * count)).setWidth(WONDERWIDTH).setHeight(WONDERHEIGHT);
                visuals1.Add(w.getName(), w.getVisual());
                count++;

                if (count>3)
                {
                    count=0;
                    count2++;
                }
                
            }

            visuals1.Add("selected", new Visual(game, new Vector2(5 + SEC1WIDTH, 5 + SEC1HEIGHT), WONDERWIDTH * 2 + 10, WONDERHEIGHT * 2, Game1.wonders.Values.First().getVisual().getTexture()));
            visuals1.Add("toggleButton", sideButton);
            visuals1.Add("backButton", backButton);
            activeVisuals = visuals1;
        }

        public override void LoadContent()
        {
            base.LoadContent();
            foreach (Visual v in visuals1.Values)
            {
                v.LoadContent();
            }
        }

        public override void receiveMessage(Dictionary<string, string> message)
        {
            random = Boolean.Parse(message["random"]);
            onlyA = Boolean.Parse(message["onlyA"]);
            if (onlyA) sideButton.setVisible(false);
            else sideButton.setVisible(true);
        }

        public override void Update(GameTime gameTime, MouseState mouseState)
        {
            base.Update(gameTime, mouseState);
            if (sideButton.isClicked())
            {
                viewSideB = !viewSideB;
                foreach (Wonder w in Game1.wonders.Values)
                {
                    if (viewSideB)
                        w.setSideB();
                    else
                        w.setSideA();
                }
                string image = visuals1["selected"].getTexture();
                if (viewSideB)
                    visuals1["selected"].setTexture(image.Substring(0, image.Length - 1) + "B");
                else
                    visuals1["selected"].setTexture(image.Substring(0, image.Length - 1) + "A");

                sideButton.reset();               
            }
            if (backButton.isClicked())
            {
                finished = true;
                backButton.reset();
            }

            foreach (Wonder w in Game1.wonders.Values)
            {
                if (visuals1[w.getName()].isClicked())
                {
                    if (viewSideB)
                        visuals1["selected"].setTexture(w.getName() + "_B");
                    else
                        visuals1["selected"].setTexture(w.getName() + "_A");
                }
            }

            if (Game1.client.isUpdateAvailable()) updatePlayers();

            int count = 0;
            foreach (Player p in Game1.client.getState().getPlayers().Values)
            {
                if (p.getName() == System.Environment.MachineName)
                {
                    if (readyCBs[count].hasChanged())
                        p.setReady(readyCBs[count].isSelected());
                }
                count++;
            }
        }

        public override Dictionary<string, string> isFinished()
        {
            if (finished)
            {
                return MainMenu.createMessage();
            }

            return null;
        }

        public static Dictionary<string, string> createMessage(bool random, bool onlyA)
        {
            return new Dictionary<string, string>()
                {
                    {"nextInterface", "lobby"},
                    {"role" , "join"},
                    {"random", random.ToString()},
                    {"onlyA", onlyA.ToString()}
                };
        }

        public virtual void updatePlayers()
        {
            int count = 0;
            List<Player> players = Game1.client.getState().getPlayers().Values.ToList<Player>();
            foreach (DropDown dd in dropDowns)
            {
                if (count < players.Count)
                {
                    dd.setSelected(players[count].getName());
                    if (Game1.host != null) dd.setEnabled(false);
                    if (players[count].getName() == System.Environment.MachineName)
                        readyCBs[count].setEnabled(true);
                    else readyCBs[count].setEnabled(false);
                    readyCBs[count].setSelected(players[count].getReady());
                }
                else if (!playerTypes.Contains(dd.getSelected()))
                {
                    dd.setSelected("Open");
                    if (Game1.host != null) dd.setEnabled(true);
                    readyCBs[count].setEnabled(false);
                }
                else readyCBs[count].setEnabled(false);
                count++;
            }
        }
    }
}