using Unity.Mathematics;
using UnityEngine;

public class PropDown : MonoBehaviour
{
    public float _fallSpeed=5f;//道具掉落的速度
    GameObject _originaBall;//原始的球
    Rigidbody _originalRB;//原始的剛體
    Vector3 _currentVelocity;//當前的方向和速度
    GameObject _ball1;
    GameObject _ball2;
    Renderer _renderer;
    public Vector3 TumbleDirection;//道具掉落時的翻轉量
    public enum PowerType{SplitBall,ExpandPaddle}//列舉兩種道具的類型
    public PowerType type;
    void Start()
    {
        _renderer=GetComponent<Renderer>();
    }

    
    void FixedUpdate()
    {
        transform.Translate(Vector3.down*_fallSpeed*Time.deltaTime,Space.World);//讓道具往下掉落，並且使用Space.World確保是往螢幕下掉落
        transform.Rotate(TumbleDirection*Time.deltaTime);//讓道具一邊掉落一邊旋轉
        if (!_renderer.isVisible)//如果掉出螢幕外
        {
            Destroy(gameObject);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))//如果碰到的是玩家
        {
            if (type == PowerType.SplitBall)//如果道具類型是分裂球
            {
                ActiveSplitBall();//執行分裂球
            }
            else if (type == PowerType.ExpandPaddle)//如果道具類型是玩家變寬
            {
                other.GetComponent<Player>().ActivateExpand(5f);//去呼叫Player身上的ActivateExpand方式
            }
            Destroy(gameObject);
        }
    }
    void ActiveSplitBall()//分裂球
    {
        _originaBall=GameObject.FindGameObjectWithTag("Ball");//先找到主球
        if (_originaBall != null)//如果有找到主球
        {
            _originalRB=_originaBall.GetComponent<Rigidbody>();//抓取主球的剛體
            _currentVelocity=_originalRB.linearVelocity;//抓取主球的直線速度
            _ball1=Instantiate(_originaBall,_originaBall.transform.position,quaternion.identity);//在主球的位置複製出第一顆球
            _ball1.GetComponent<Ball>().isClone=true;//標記為分裂的球
            _ball1.GetComponent<Rigidbody>().linearVelocity=Quaternion.Euler(0,0,30)*_currentVelocity;//讓第一顆球的飛行方向為主球的z軸偏移+30度
            _ball2=Instantiate(_originaBall, _originaBall.transform.position, Quaternion.identity);//複製第二顆球
            _ball2.GetComponent<Ball>().isClone=true;
            _ball2.GetComponent<Rigidbody>().linearVelocity = Quaternion.Euler(0, 0, -30) * _currentVelocity;//偏移-30度
        }
    }
}
