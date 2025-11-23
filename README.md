# Football League API

A RESTful API for managing teams, played matches, and generating rankings based on match results.

## Features

* **CRUD operations for Teams** — Create, Read, Update, Delete Teams.
* **CRUD operations for Played Matches** — Create, Read, Update, Delete Matches.
* **Rankings & Results Table** — Auto‑computed from match outcomes.
* **Scoring Rules** — Win: 3 points, Draw: 1 point, Loss: 0 points (Configurable through the Constants file).

---

## Endpoints

### 1. Teams CRUD Endpoints

| Method     | Endpoint                  | Description           |
| ---------- | --------------------------|-----------------------|
| **POST**   | `/api/teams`              | Create a new team     |
| **GET**    | `/api/teams`              | Retrieve all teams    |
| **GET**    | `/api/teams/{name}`       | Retrieve all teams    |
| **PUT**    | `api/teams/{name}`        | Update a team by name |
| **DELETE** | `api/teams/delete/{name}` | Delete a team by name |

---

### 2. Matches CRUD Endpoints (Played Matches Only)

| Method     | Endpoint                   | Description                  |
| ---------- | ---------------------------|------------------------------|
| **POST**   | `api/matches`              | Create a played match result |
| **GET**    | `api/matches`              | Retrieve all played matches  |
| **GET**    | `api/matches/{id}`         | Retrieve match by ID         |
| **PUT**    | `api/matches/{id}`         | Update a played match result |
| **DELETE** | `api/matches/delete/{id}`  | Delete a played match result |

> **Note:** Only played matches should be stored. No scheduling functionality.

---

## 3. Rankings & Results Table Endpoint

| Method  | Endpoint       | Description                                                   |
| ------- | ---------------|-------------------------------------------------------------- |
| **GET** | `/api/ranking` | Returns a computed rankings table based on all played matches |

### Ranking Table Includes:

* Team Name
* Points
* Matches Played
* Wins 
* Draws 
* Losses

---

## 4. Scoring System

The standings are calculated using the following points model:

* **Win:** 3 points
* **Draw:** 1 point
* **Loss:** 0 points

The rankings endpoint aggregates all match results to return an up‑to‑date standings table. The point values can be configured through the constants file. 

---

## Example Ranking Logic

For each match:

* If `teamA_score > teamB_score`: team A gets 3 points
* If `teamA_score < teamB_score`: team B gets 3 points
* If `teamA_score == teamB_score`: both teams get 1 point


