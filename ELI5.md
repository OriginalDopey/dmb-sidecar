# Front Office IQ — Explain Like I'm 5

---

## The Problem

There's an online baseball management game ([Diamond Mind Baseball](https://imaginesports.com/bball)) where you draft real historical players, set lineups, and compete against other owners. Think fantasy baseball, but with Willie Mays and Sandy Koufax.

The game's website was built years ago. It has **no API**, no mobile app, and no built-in analytics. You're staring at HTML tables trying to figure out optimal lineups and salary decisions by memory and gut feel.

---

## What We Built

A **browser sidebar that acts like a smart assistant** for the game.

When you're on the game's website looking at your lineup, the sidebar:
1. **Sees what you see** — it reads the page to understand context (which lineup, which pitcher you're facing)
2. **Gives advice** — "Put Henderson in LF, move Carew to DH" with explanations
3. **Answers questions** — "What's the salary penalty for releasing a player mid-season?" → "75%, here's the source" (with citations from the actual rulebook)

---

## How It Works (Plain English)

```
You're on the game website
        ↓
Browser extension reads the page
        ↓
Sends context to a local server on your computer
        ↓
Server either:
  • Answers instantly (for lineup math)
  • Asks Microsoft's AI (for rule questions)
        ↓
AI checks a knowledge base of game rules
        ↓
Answer appears in the sidebar with sources
```

---

## The Pieces

| Piece | What It Does | Analogy |
|-------|-------------|---------|
| Chrome Extension | Watches the game website, shows the sidebar | Your eyes + a notepad |
| ASP.NET API | Brain that coordinates everything | A manager routing calls |
| Foundry IQ Agent | AI that reads the rulebook for you | A really fast intern who read every rule |
| Knowledge Base | 17 documents of game rules + strategy | The rulebook on the intern's desk |
| MCP Bridge | Fetches live league data (standings, rosters) | Today's newspaper with scores |

---

## Why This Is Interesting (For Reviewers)

This project solves a real problem that exists for **any legacy web application**:

> "How do you add AI features to a website you don't control and that has no API?"

The answer: **observe the DOM, extract context, route to intelligence.**

This pattern works for:
- Legacy enterprise apps (SAP, Oracle, internal tools)
- Third-party SaaS with no API
- Any website where you want to augment the user experience

---

## Key Technical Choices

1. **No scraping** — We read the page the user is already on (they're authenticated, it's their data)
2. **No hallucination** — AI answers are grounded in actual game documents with citations
3. **Works offline** — If Azure is down, falls back to local keyword search
4. **Fast for common questions** — Lineup math happens locally in milliseconds, only "why" questions go to the cloud
5. **Secure** — API key auth, no stored credentials, CORS-locked to the extension

---

## One-Sentence Summary

> A Chrome extension that adds an AI-powered front office advisor to a legacy baseball game by observing the DOM, routing questions through a local API, and grounding answers in Microsoft Foundry IQ with cited game rules.
