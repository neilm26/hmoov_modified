using UnityEngine;
using Photon.Bolt;

public class GameController : EntityEventListener<IGameModeState>
{
    #region Singleton
    private static GameController _instance = null;

    public static GameController Current
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<GameController>();

            return _instance;
        }
    }
    #endregion

    GamePhase _currentPhase = GamePhase.WaitForPlayers;
    int _playerCountTarget = 2;
    float _nextEvent = 0;
    GameObject _walls;
    Team _roundWinner = Team.None;
    int _winningRoundAmount = 10;

    SiteController _ASite;
    SiteController _BSite;

    public GamePhase CurrentPhase { get => _currentPhase; }

    public bool IsInSite { get => _ASite.IsPlayerIn || _BSite.IsPlayerIn; }

    private void Start()
    {
        _walls = GameObject.Find("__Walls");
        _ASite = GameObject.Find("__ASite").GetComponent<SiteController>();
        _BSite = GameObject.Find("__BSite").GetComponent<SiteController>();

        Debug.Log(_ASite);
        Debug.Log(_BSite);

    }

    public override void Attached()
    {
        state.AddCallback("AlivePlayers", UpdatePlayersAlive);
        state.AddCallback("TTPoints", UpdatePoints);
        state.AddCallback("ATPoints", UpdatePoints);
        state.AddCallback("Timer", UpdateTime);
        state.AddCallback("Planted", UpdatePlanted);    
    }

    public void UpdatePlanted()
    {
        GUI_Controller.Current.Planted(state.Planted);
    }

    public void UpdatePoints()
    {
        GUI_Controller.Current.UpdatePoints(state.ATPoints, state.TTPoints);

        if (_winningRoundAmount == state.ATPoints || _winningRoundAmount == state.TTPoints)
        {
            _currentPhase = GamePhase.EndGame;
            UpdateGameState();
        }
    }

    public void UpdateTime()
    {
        GUI_Controller.Current.UpdateTimer(state.Timer);
    }

    public void SetWalls(bool b)
    {
        SetWallsEvent evnt = SetWallsEvent.Create(entity);
        evnt.Set = b;
        evnt.Send();
    }

    public override void OnEvent(SetWallsEvent evnt)
    {
        for (int i = 0; i < _walls.transform.childCount; i++)
        {
            _walls.transform.GetChild(i).gameObject.SetActive(evnt.Set);
        }
    }

    public void Planted()
    {
        _nextEvent = BoltNetwork.ServerTime + 60;
        state.Timer = 60;
        _currentPhase = GamePhase.TT_Planted;
        UpdateGameState();
        state.Planted = true;
        Debug.Log("state.Planted = true; Planted() has ran");
    }

    public void Defuse() {
        state.ATPoints++;
        _nextEvent = BoltNetwork.ServerTime + 10f;
        state.Timer = 10f;
        _currentPhase = GamePhase.EndRound;
        _roundWinner = Team.AT;
        UpdateGameState();
    }

    public void UpdatePlayersAlive()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        

        if (entity.IsOwner)
        {
            int ATCount = 0;
            int TTCount = 0;

            foreach (GameObject player in players)
            {
                PlayerToken pt = (PlayerToken)player.GetComponent<PlayerMotor>().entity.AttachToken;
                if (!player.GetComponent<PlayerMotor>().state.IsDead)
                {
                    if (pt.team == Team.AT)
                        ATCount++;
                    else
                        TTCount++;
                }
            }
            _roundWinner = Team.None;

            if (_currentPhase == GamePhase.AT_Defending)
            {
                if (ATCount == 0)
                {
                    state.TTPoints++;
                    _nextEvent = BoltNetwork.ServerTime + 10f;
                    state.Timer = 10f;
                    _currentPhase = GamePhase.EndRound;
                    _roundWinner = Team.TT;
                    UpdateGameState();
                }

            }

            if (GamePhase.WaitForPlayers == _currentPhase)
            {
                foreach (GameObject player in players)
                {
                    player.GetComponent<PlayerCallback>().RoundReset(Team.None);
                }
            }

            if (_currentPhase == GamePhase.TT_Planted)
            {
                if (ATCount == 0)
                {
                    state.TTPoints++;
                    _nextEvent = BoltNetwork.ServerTime + 10f;
                    state.Timer = 10f;
                    _currentPhase = GamePhase.EndRound;
                    _roundWinner = Team.TT;
                    UpdateGameState();
                }

                if (TTCount == 0)
                {
                    state.ATPoints++;
                    _nextEvent = BoltNetwork.ServerTime + 10f;
                    state.Timer = 10f;
                    _currentPhase = GamePhase.EndRound;
                    _roundWinner = Team.AT;
                    UpdateGameState();
                }
            }
        }

        GameObject lp = GameObject.FindGameObjectWithTag("LocalPlayer");
        GUI_Controller.Current.UpdatePlayerPlates(players, lp);
    }

    public void UpdateGameState()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        switch (_currentPhase)
        {
            case GamePhase.WaitForPlayers: 
                if (_playerCountTarget == players.Length)
                {
                    _currentPhase = GamePhase.Starting;
                    _nextEvent = BoltNetwork.ServerTime + 10f;
                    state.Timer = 10f;
                }
                Debug.Log("game phase wait for players");
                break;
            case GamePhase.Starting:
                Debug.Log("game phase starting");
                break;
            case GamePhase.StartRound:
                SetWalls(true);
                _ASite.RoundReset();
                _BSite.RoundReset(); 
                GameObject[] drops = GameObject.FindGameObjectsWithTag("Drop");

                foreach (GameObject drop in drops)
                {
                    BoltNetwork.Destroy(drop.GetComponent<BoltEntity>());
                }

                bool founded = false;

                /*
                for (int i=0;i<players.Length;i++) {
                    players[i].GetComponent<PlayerWeapons>().AddWeaponEvent(WeaponID.Bomb);
                }
                */

                while (!founded) {
                    int r = Random.Range(0, players.Length);

                    PlayerToken pt = (PlayerToken) players[r].GetComponent<PlayerMotor>().entity.AttachToken;

                    if (pt.team == Team.TT) {
                        players[r].GetComponent<PlayerWeapons>().AddWeaponEvent(WeaponID.Bomb);
                        founded = true;
                        Debug.Log("Got Bomb");
                    }
                }

                foreach (GameObject player in players)
                {
                    player.GetComponent<PlayerCallback>().RoundReset(_roundWinner);
                }
                _nextEvent = BoltNetwork.ServerTime + 10f;
                state.Timer = 10f;
                Debug.Log("game phase start round");
                break;
            case GamePhase.AT_Defending:
                SetWalls(false);
                Debug.Log("game phase at defending");
                break;
            case GamePhase.TT_Planted:
                Debug.Log("game phase tt planted");
                break;
            case GamePhase.EndRound:
                state.Planted = false;
                foreach (GameObject p in players) {
                    if (p.GetComponent<PlayerWeapons>().HasBomb) {
                        p.GetComponent<PlayerWeapons>().RemoveBomb();
                    }
                }
                Debug.Log("game phase endround");
                break;
            case GamePhase.EndGame:
                //TODO declare winner
                Debug.Log("game phase endgame");
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        switch (_currentPhase)
        {
            case GamePhase.WaitForPlayers:
                break;
            case GamePhase.Starting:
                if (_nextEvent < BoltNetwork.ServerTime)
                {
                    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                    foreach (GameObject player in players)
                    {
                        player.GetComponent<PlayerCallback>().RoundReset(Team.None); 
                    }

                    _nextEvent = BoltNetwork.ServerTime + 15f;
                    state.Timer = 15f;
                    _currentPhase = GamePhase.StartRound;
                    UpdateGameState();
                    Debug.Log("game phase starting");
                }
                break;
            case GamePhase.StartRound:
                if (_nextEvent < BoltNetwork.ServerTime)
                {
                    _nextEvent = BoltNetwork.ServerTime + 100f;
                    state.Timer = 180f;
                    _currentPhase = GamePhase.AT_Defending;
                    UpdateGameState();
                    Debug.Log("game phase start round");
                }
                break;
            case GamePhase.AT_Defending:
                if (_nextEvent < BoltNetwork.ServerTime)
                {
                    _nextEvent = BoltNetwork.ServerTime + 180f;
                    state.Timer = 180f;
                    _currentPhase = GamePhase.AT_Defending;
                    UpdateGameState();
                    Debug.Log("game phase at defending");
                }
                break;
            case GamePhase.TT_Planted:
                //TODO
                break;
            case GamePhase.EndRound:
                if (_nextEvent < BoltNetwork.ServerTime)
                {
                    _nextEvent = BoltNetwork.ServerTime + 15f;
                    state.Timer = 15f;
                    _currentPhase = GamePhase.StartRound;
                    BombController._IS_DEFUSED = false;
                    UpdateGameState();
                    Debug.Log("game phase tt planted");
                }
                break;
            case GamePhase.EndGame:
                Debug.Log("game phase end game");

                break;
            default:
                break;

        }
    }
}

public enum GamePhase
{
    WaitForPlayers,
    Starting,
    StartRound,
    AT_Defending, 
    TT_Planted,
    EndRound,
    EndGame
}
