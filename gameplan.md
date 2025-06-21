# Rock Paper Scissors - Development Gameplan

## Network Discovery & Matchmaking
- **NetworkDiscovery**: Use Mirror's `NetworkDiscovery` component for local network game detection
- **Server Broadcasting**: Host broadcasts game availability with custom data (game mode, player count)
- **Client Discovery**: Players search (using periodic polling or manual refresh using a button) for available games and join directly
- **Discovery Data Structure**: Include game settings (rounds to win, host name, current players)
- **Direct Connection Flow**: Discover → Select Game → Connect → Start Match

## UI Layout & Player Positioning
- **Dynamic Player Assignment**: Each client shows themselves at bottom, opponent at top
- **Player Identification**: Use `NetworkIdentity.isLocalPlayer` to determine local vs remote player
- **UI Setup**: 
  - Bottom Panel: Local player name, score, choice buttons, "Ready" indicator
  - Top Panel: Opponent name, score, hidden choice (show after reveal), "Ready" indicator
- **Implementation**: In UI Manager, populate panels based on `isLocalPlayer` check


## Core Game Flow
1. **Match Setup**: When 2 players connect, create GameSession with scores (0-0)
2. **Round Logic**: 
   - Display choice UI (Rock/Paper/Scissors)
   - Both players submit choices via `[Command]`
   - Server validates and determines winner using `[ClientRpc]`
   - Update scores and display round result

## Key Components Needed
- **NetworkDiscovery**: Custom discovery component extending Mirror's `NetworkDiscovery`
- **GameManager** (`NetworkBehaviour`): Handles game state, scores, round progression
- **PlayerController**: Manages player input and choice submission
- **UI Manager**: Choice buttons, score display, result animations, available games list
- **GameState Enum**: Waiting, Playing, RoundEnd, GameEnd
- **DiscoveryUI**: Shows available games, refresh button, connection status

## Network Implementation
- Use `[SyncVar]` for player scores and current round
- `[Command] SubmitChoice()` for player moves
- `[ClientRpc] ShowRoundResult()` to display outcomes
- Discovery responses include game metadata (host name, players, settings)
- Sync player names and ready states across clients

## Win Conditions
- First to 3 wins takes the match (should be adjustable in lobby by host)
- Handle disconnections ?? count as forfeit ??
- Return to lobby after game completion (maybe save number of total game wins beside each player?)

## Essential States
Discovery → WaitingForPlayers → Playing → RoundResult → (repeat until 3 wins) → GameEnd → Lobby