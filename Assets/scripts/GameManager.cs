using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public GameObject ballPrefab;
    public GameObject playerPrefab;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI ballsText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI highscoreText;
    public GameObject panelMenu;
    public GameObject panelPlay;
    public GameObject panelLevelCompleted;
    public GameObject panelGameOver;
    public GameObject[] brickPrefabs;
    public float spacingX=1.5f;
    public float spacingY=1.0f; 
    public static GameManager Instance{get;private set;}//GameManager:代表這個變數只能裝掛著GameManager腳本的遊戲物件,Instance:為變數名稱(但通常取Instance，這樣就知道是單例模式)
    public enum State{MENU,INIT,PLAY,LEVELCOMPLETED,LOADLEVEL,GAMEOVER}//enum:可以自訂義一個型別而且裡面可以用自己看得懂的東西。State:為變數名稱。
    State _state;//宣告一個State型別的變數
    GameObject _currentBall;
    GameObject _currentLevel;
    bool _isSwitchingState;
    public AudioClip menuBGM;
    public AudioClip playBGM;
    public AudioClip gameOverBGM;
    AudioSource _bgmSource;
    private int _score;//創建一個不可被訪問的_score變數
    public int Score//創建一個Score屬性
    {
        get { return _score; }
        set { _score = value;
            scoreText.text="SCORE: " +_score ;}
    }

    private int _level;
    public int Level
    {
        get { return _level; }
        set { _level = value;
            levelText.text="LEVEL: "+ _level; }
    }
    private int _balls;
    public int Balls
    {
        get { return _balls; }
        set { _balls = value; 
            ballsText.text="BALLS: "+ _balls;}
    }
    
    
    public void PlayClicked()//這個方式放在ButtonPlay的觸發
    {
        SwitchState(State.INIT);//切換到狀態初始化
    }
    void Start()
    {
        _bgmSource=GetComponent<AudioSource>();
        Instance=this;//宣告Instance就是這個GameManager腳本
        SwitchState(State.MENU);//切換到狀態選單
    }
    public void SwitchState(State newState,float delay=0)//定義一個方式，要傳入一個State型別的參數，delay=0代表不一定要傳
    {
       StartCoroutine(SwitchDelay(newState,delay));//等待delay秒才執行SwitchDelay方式
    }
    IEnumerator SwitchDelay(State newState,float delay)
    {
        _isSwitchingState=true;//代表現在正在進行切換狀態
        yield return new WaitForSeconds(delay);//等待delay秒
        EndState();//結束現在這個狀態
        _state=newState;//切換狀態
        BeginState(newState);//調用BeginState方式切換狀態
        _isSwitchingState=false;//代表切換完成
    }
    void BeginState(State newState)//定義一個正在狀態的方式，要傳入一個State型別的參數
    {
        switch (newState)//查看傳入的狀態
        {
            case State.MENU://選單
                PlayMusic(menuBGM);
                Cursor.visible=true;//顯示屬標
                highscoreText.text="HIGHSCORE: "+PlayerPrefs.GetInt("highscore");//利用PlayerPrefs可以存電腦裡的分數不會不見，並且利用GetInt取得名為highscore的數字
                panelMenu.SetActive(true);//選單畫面開啟
                break;
            case State.INIT://載入關卡前的準備
                Cursor.visible=false;//隱藏屬標
                panelPlay.SetActive(true);//開始的畫面開啟
                Score=0;
                Level=0;
                Balls=3;
                if (_currentLevel != null)//如果這個關卡還有東西
                {
                    Destroy(_currentLevel);//摧毀這個關卡的東西
                }
                Instantiate(playerPrefab);//實例化player的Prefab
                SwitchState(State.LOADLEVEL);//轉換到loadlevel狀態
                break;
            case State.PLAY:
                PlayMusic(playBGM);
                break;
            case State.LEVELCOMPLETED://當關卡完成時
                Destroy(_currentBall);
                Destroy(_currentLevel);
                Level++;
                panelLevelCompleted.SetActive(true);
                SwitchState(State.LOADLEVEL,2f);
                break;
            case State.LOADLEVEL://載入關卡
                if (Level >= 2)//如果沒有關卡了
                {
                    SwitchState(State.GAMEOVER);
                }
                else
                {
                    _currentLevel=new GameObject("Level_"+Level);
                    GenerateLevel(Level,_currentLevel.transform);//呼叫關卡生成器
                    SwitchState(State.PLAY);
                }
                break;
            case State.GAMEOVER://遊戲結束
                PlayMusic(gameOverBGM);
                if (Score > PlayerPrefs.GetInt("highscore"))//如果分數大於歷史最高分數
                {
                    PlayerPrefs.SetInt("highscore",Score);//改變歷史最高分數
                }
                panelGameOver.SetActive(true);
                panelGameOver.transform.localScale=Vector3.zero;//先把gameover面板縮小
                panelGameOver.transform.DOScale(Vector3.one,0.6f).SetEase(Ease.OutBack);//然後用0.6秒的特效放大出現
                break;             
        }
    }
    void Update()
    {
        switch (_state)
        {
            case State.MENU:
                break;
            case State.INIT:
                break;
            case State.PLAY:
                if (GameObject.FindGameObjectsWithTag("Ball").Length==0&&!_isSwitchingState)//如果現在沒有球
                {
                    if (Balls > 0)
                    {
                        Instantiate(ballPrefab);//再實例化一個球
                    }
                    else
                    {
                        SwitchState(State.GAMEOVER);
                    }
                }
                if (_currentLevel != null && _currentLevel.transform.childCount == 0 && !_isSwitchingState)//如果現在有關卡然後關卡的子物體都沒了然後不是在切換狀態
                {
                    SwitchState(State.LEVELCOMPLETED);
                }
                break;
            case State.LEVELCOMPLETED:
                break;
            case State.LOADLEVEL:
                break;
            case State.GAMEOVER:
                if (Input.anyKeyDown)//如果按下任何鍵
                {
                    SwitchState(State.MENU);
                }
                break;             
        }
    }
    void EndState()
    {
        switch (_state)
        {
            case State.MENU:
                panelMenu.SetActive(false);
                break;
            case State.INIT:
                break;
            case State.PLAY:
                break;
            case State.LEVELCOMPLETED:
                panelLevelCompleted.SetActive(false);
                break;
            case State.LOADLEVEL:
                break;
            case State.GAMEOVER:
                panelPlay.SetActive(false);
                panelGameOver.SetActive(false);
                break;             
        }
    }
    void PlayMusic(AudioClip newClip)
    {
        if(newClip==null||_bgmSource.clip==newClip)//如果沒傳入音樂或是已經在撥放了就直接return
            return;
        _bgmSource.Stop();
        _bgmSource.clip=newClip;
        _bgmSource.Play();    
    }
    void GenerateLevel(int currentLevel,Transform levelParent)//根據傳入的關卡數字去生成關卡
    {
        int[,] levelMap;//宣告一個二維陣列
        if (currentLevel == 0)//如果是第一關
        {
            levelMap=new int[,]
            {
                { 0, 0, 0, 0, 0, 2, 2, 2, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 2, 1, 1, 1, 2, 0, 0, 0, 0 },
                { 0, 0, 0, 2, 1, 3, 3, 3, 1, 2, 0, 0, 0 },
                { 0, 0, 2, 1, 3, 3, 3, 3, 3, 1, 2, 0, 0 },
                { 0, 2, 1, 3, 3, 1, 1, 1, 3, 3, 1, 2, 0 },
                { 2, 1, 1, 3, 3, 1, 2, 1, 3, 3, 1, 1, 2 },
                { 2, 1, 1, 1, 3, 3, 3, 3, 3, 1, 1, 1, 2 },
                { 0, 2, 2, 1, 1, 1, 1, 1, 1, 1, 2, 2, 0 },
                { 0, 0, 0, 2, 2, 2, 0, 2, 2, 2, 0, 0, 0 },
                { 0, 0, 2, 2, 0, 0, 0, 0, 0, 2, 2, 0, 0 }
            };
        }
        else
        {
            levelMap=new int[,]
            {
                { 0, 0, 0, 2, 2, 0, 0, 0 },
                { 0, 0, 2, 1, 1, 2, 0, 0 },
                { 0, 2, 1, 0, 0, 1, 2, 0 },
                { 2, 1, 0, 3, 3, 0, 1, 2 }, 
                { 2, 1, 0, 3, 3, 0, 1, 2 },
                { 0, 2, 1, 0, 0, 1, 2, 0 },
                { 0, 0, 2, 1, 1, 2, 0, 0 },
                { 0, 0, 0, 2, 2, 0, 0, 0 }
            };
        }
        int rows=levelMap.GetLength(0);//取得這個陣列有幾列跟幾行
        int cols=levelMap.GetLength(1);
        for(int row = 0; row < rows; row++)
        {
            for(int col = 0; col < cols; col++)
            {
                int brickType=levelMap[row,col];//讀取該格子的數字
                if (brickType>0)//如果數字大於0
                {
                    float posX=(col-(cols/2f))*spacingX;//算出磚塊的X座標
                    float posY=5f-(row*spacingY);//算出y座標
                    Vector3 spawnPosition=new Vector3(posX,posY,0);//得到應該生成的座標
                    GameObject prefabToSpawn=brickPrefabs[brickType-1];//根據數字得倒應該生成的磚塊類型
                    GameObject newBrick=Instantiate(prefabToSpawn,spawnPosition,Quaternion.identity);//生成磚塊
                    newBrick.transform.SetParent(levelParent);//把磚塊設為關卡的子物件
                }
            }
        }
    }
}
