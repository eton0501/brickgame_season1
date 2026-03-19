using Unity.VisualScripting;
using UnityEngine;

public class Ball : MonoBehaviour
{
    
    public bool isClone=false;//確認這顆球是不是分裂出來的球
    float _baseSpeed=20f;//球的基礎速度
    float _currentSpeed;//球目前的速度
    float _speedIncrease=2f;//球撞到牆壁或是磚塊，增加的速度量
    public float _maxSpeed=35f;//球的最大速度
    Rigidbody _rigidbody;
    Vector3 _velocity;//紀錄球撞擊前的速度方向與大小
    Renderer _renderer;
    AudioSource _audioSource;
    private bool _isLaunched=false;//確認球是否被玩家發射
    public float yOffset = 1.0f;//決定球發射前要在玩家多高的位子
    void Start()
    {
        _rigidbody=GetComponent<Rigidbody>();
        _renderer=GetComponent<Renderer>();
        _audioSource=GetComponent<AudioSource>();
        _currentSpeed=_baseSpeed;//把當前速度設為基礎速度
        if (!isClone)//如果不是分裂的球
        {
            GameObject player=GameObject.FindGameObjectWithTag("Player");//去找尋標籤為Player的物件，並把他賦值名為player的物件
            if (player != null)//如果有找到玩家
            {
                _rigidbody.isKinematic=true;//關閉物理反應
                transform.position=player.transform.position+new Vector3(0f,yOffset,0f);//將球的座標設在玩家的中心點往上yoffset的位置
                transform.SetParent(player.transform);//將球設為玩家的子物件(這樣才會一直黏在玩家上)
            }
            _isLaunched=false;//標記為未發射狀態
        }
        else//如果是分裂的球
        {
            _isLaunched=true;//標記為已發射狀態
            Launch();//直接呼叫發射指令
        }
    }
    void Update()
    {
        if (!_isLaunched && !isClone)//如果球還沒被發射，而且不是分裂球
        {
            if (Input.GetMouseButton(0) || Input.GetKeyDown(KeyCode.Space))//如果玩家按下左鍵或是空白鍵
            {
                _isLaunched=true;//標記為發射狀態
                transform.SetParent(null);//解除父子關係(讓球脫離玩家)
                Launch();//呼叫發射函數
            }
        }
    }
    void Launch()
    {
        _rigidbody.isKinematic=false;//開啟物理反應
        float randomX=Random.Range(-0.5f,0.5f);//定義一個在-0.5f到0.5f的數字，讓每次發射的X都不太一樣
        _rigidbody.linearVelocity=new Vector3(randomX,1f,0f).normalized*_currentSpeed;//將球往右下方向乘上speed並且利用normalized確保速度都是一致的
    }

    
    void FixedUpdate()
    {
        if(!_isLaunched)return;//如果球還沒發射，就直接return，不執行下面的物理運算
        Vector3 currentV=_rigidbody.linearVelocity;//取得現在的直線速度
        if (Mathf.Abs(currentV.y) < 2f)//如果球上下的移動速度的絕對值小於2f
        {
            float signY=currentV.y!=0?Mathf.Sign(currentV.y):1f;//判斷目前y是正還是負
            currentV.y=signY*2f;//強制把y的速度拉到2
        }
        _rigidbody.linearVelocity=currentV.normalized*_currentSpeed;//把修正過的方向規一化，並且乘上現在的速度
        _velocity=_rigidbody.linearVelocity;//把撞擊前的速度計到_velocity
        if (!_renderer.isVisible)//renderer.isVisible(代表說這個物體現在有沒有被攝影機看到，傳回True或是False)
        {
            GameObject[] ballsInScene=GameObject.FindGameObjectsWithTag("Ball");//尋找畫面上還有幾個標籤為"Ball"的物件
            if (ballsInScene.Length > 1)//如果球數量大於1
            {
                Destroy(gameObject);//只要把這個物件摧毀就好
            }
            else//如果這是最後一顆球
            {
                GameManager.Instance.Balls--;//呼叫GameManager去讓Balls減一
                Destroy(gameObject);//摧毀這個物體
            }
            
        }
    }
    private void OnCollisionEnter(Collision collision)//當碰撞的時候
    {
        if(!_isLaunched)return;//如果還沒發射直接return
        if (collision.gameObject.CompareTag("Player"))//如果碰撞到的是玩家
        {
            _currentSpeed=_baseSpeed;//讓球的速度變為基礎速度
            float offset=transform.position.x-collision.transform.position.x;//計算球打在玩家的相對位置(球的X座標減掉玩家的X座標)
            float paddleWidth=collision.collider.bounds.size.x;//取得玩家的總寬度
            float hitFactor=(offset/paddleWidth)*2f;//計算打擊點的比例
            Vector3 newDirection=new Vector3(hitFactor*1.5f,1f,0f).normalized;//讓球根據打到玩家的左邊或右邊給予更斜的角度
            _rigidbody.linearVelocity=newDirection*_currentSpeed;//把算好的新方向直接賦予
        }
        else//如果撞到的不是玩家
        {
            _rigidbody.linearVelocity=Vector3.Reflect(_velocity,collision.contacts[0].normal);//Vector3.Reflect(撞擊的方向以及速度,牆壁的角度),collision(撞擊的時候).contacts[0](撞擊的第一個點).normal(法線)
            _currentSpeed+=_speedIncrease;//每次撞到磚塊或牆壁，球就會加速
            if (_currentSpeed >= _maxSpeed)//如果超過最高速度
            {
                _currentSpeed=_maxSpeed;//直接變成最高速度
            }
        }
        if (_audioSource != null)
        {
            _audioSource.Play();
        }
    }
}
