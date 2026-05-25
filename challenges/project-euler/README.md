---
title: "Project Euler"
summary: "Project Euler problems implemented as a RazorConsole application."
sourceUrl: https://github.com/evilz/projecteuler
topics:
  - challenge
  - project-euler
  - math
---

projecteuler
============

Project Euler problems implemented as a RazorConsole (TUI) app using routing.

## Screenshot

![Project Euler Problem Selector](https://github.com/user-attachments/assets/e51b9f87-29e3-4eae-9506-d99f2e7aca69)

## How it works

- Each problem is a routable Razor component with a route like `/problem/1`.
- A shared base component renders the UI (title, description, solve/clear buttons).
- The main layout shows a problem selector and navigates to the selected route.

## Run locally

From the repository root:

- Build: `dotnet build`
- Run: `dotnet run`

After launch, use the selector to choose a problem and press Enter to navigate.

## Routes

- `/` – Home (instructions)
- `/problem/1` … `/problem/20` – Problem pages

## CI

GitHub Actions runs a simple build on push and pull requests.


