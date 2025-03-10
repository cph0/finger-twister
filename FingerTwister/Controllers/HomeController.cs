using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using FingerTwister.Models;
using Microsoft.AspNetCore.Http;

namespace FingerTwister.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        var buttons = new List<int>();
        var players = new Dictionary<int, List<int>>();
        var scores = new Dictionary<int, int>();

        HttpContext.Session.Set("players", players);
        HttpContext.Session.Set("scores", scores);

        HttpContext.Session.Set("buttons", buttons);
        HttpContext.Session.Set("level", 1);
        return View("Game");
    }

    [HttpPost]
    public IActionResult GetData(int id)
    {
        var players = HttpContext.Session.Get<Dictionary<int, List<int>>>("players");
        var scores = HttpContext.Session.Get<Dictionary<int, int>>("scores");
        var model = new Data();
        var buttons = HttpContext.Session.Get<List<int>>("buttons");
        var level = HttpContext.Session.Get<int>("level");
        var playerHasID = players.Any(w => w.Value.Contains(id));
        int currentPlayer;
        var started = false;

        if (playerHasID)
        {
            currentPlayer = players.Where(w => w.Value.Contains(id)).FirstOrDefault().Key;
            started = true;
        }
        else
        {
            //set up
            int maxPlayerID;
            if (players.Count == 0)
                maxPlayerID = 1;
            else
                maxPlayerID = players.Max(w => w.Key) + 1;

            var player = new List<int>() { id };
            buttons.Add(id);
            players.Add(maxPlayerID, player);
            scores.Add(maxPlayerID, 0);
            currentPlayer = maxPlayerID;

            if (players.Count(c => c.Value.Count > 0) == 3)
                started = true;
        }

        if (buttons.Count == 12)
        {
            model.finish = true;
            if (level < 5)
            {
                level += 1;
            }
            else
            {
                level = 1;
            }

            scores = new Dictionary<int, int>();
            players = new Dictionary<int, List<int>>();
            started = false;

            buttons = new List<int>();
            model.level = level;
        }
        else if (buttons.Count == 11)
        {
            var full = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var nextID = full.Except(buttons).First();

            var index = buttons.Count <= level ? 0 : buttons.Count - level;
            model.lastID = buttons.ElementAt(index);
            model.nextID = nextID;
            buttons.Add(nextID);

            players[currentPlayer].Add(nextID);
            scores[currentPlayer] += 1;
        }
        else
        {
            var gen = new Random();
            var nextID = gen.Next(1, 12);
            while (buttons.Contains(nextID))
            {
                nextID = gen.Next(1, 12);
            }

            var index = buttons.Count <= level ? 0 : buttons.Count - level;
            model.lastID = buttons.ElementAt(index);
            model.nextID = nextID;
            buttons.Add(nextID);

            players[currentPlayer].Add(nextID);
            scores[currentPlayer] += 1;
        }

        var scoreString = "";
        foreach (var player in players)
        {
            if (player.Value.Count != 0)
                scoreString = $"{scoreString} {scores[player.Key]}";
        }

        HttpContext.Session.Set("players", players);
        HttpContext.Session.Set("scores", scores);

        HttpContext.Session.Set("buttons", buttons);
        HttpContext.Session.Set("level", level);

        model.player = currentPlayer;
        model.started = started;
        model.scores = scoreString;
        return Json(model);
    }

    [HttpPost]
    public IActionResult SendData(int id)
    {
        var buttons = HttpContext.Session.Get<List<int>>("buttons");
        var players = HttpContext.Session.Get<Dictionary<int, List<int>>>("players");
        var scores = HttpContext.Session.Get<Dictionary<int, int>>("scores");
        var playerHasID = players.Any(w => w.Value.Contains(id));

        if (playerHasID)
        {
            var currentPlayer = players.Where(w => w.Value.Contains(id)).FirstOrDefault().Key;
            players[currentPlayer].Remove(id);
            scores[currentPlayer] -= 1;
        }

        buttons.Remove(id);

        HttpContext.Session.Set("players", players);
        HttpContext.Session.Set("scores", scores);
        HttpContext.Session.Set("buttons", buttons);
        return Json(id);
    }

    public IActionResult About()
    {
        ViewData["Message"] = "Your application description page.";

        return View();
    }

    public IActionResult Contact()
    {
        ViewData["Message"] = "Your contact page.";

        return View();
    }

    public IActionResult Error()
    {
        return View();
    }
}
