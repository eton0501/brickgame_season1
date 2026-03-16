using Unity.Mathematics;
using UnityEngine;

public class PropDown : MonoBehaviour
{
    public float _fallSpeed=5f;
    GameObject _originaBall;
    Rigidbody _originalRB;
    Vector3 _currentVelocity;
    GameObject _ball1;
    GameObject _ball2;
    Renderer _renderer;
    public Vector3 TumbleDirection;
    public enum PowerType{SplitBall,ExpandPaddle}
    public PowerType type;
    void Start()
    {
        
    }

    
    void FixedUpdate()
    {
        transform.Translate(Vector3.down*_fallSpeed*Time.deltaTime,Space.World);
        transform.Rotate(TumbleDirection*Time.deltaTime);
        if (!_renderer.isVisible)
        {
            Destroy(gameObject);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (type == PowerType.SplitBall)
            {
                ActiveSplitBall();
            }
            else if (type == PowerType.ExpandPaddle)
            {
                other.GetComponent<Player>().ActivateExpand(5f);
            }
            Destroy(gameObject);
        }
    }
    void ActiveSplitBall()
    {
        _originaBall=GameObject.FindGameObjectWithTag("Ball");
        if (_originaBall != null)
        {
            _originalRB=_originaBall.GetComponent<Rigidbody>();
            _currentVelocity=_originalRB.linearVelocity;
            _ball1=Instantiate(_originaBall,_originaBall.transform.position,quaternion.identity);
            _ball1.GetComponent<Ball>().isClone=true;
            _ball1.GetComponent<Rigidbody>().linearVelocity=Quaternion.Euler(0,0,30)*_currentVelocity;
            _ball2=Instantiate(_originaBall, _originaBall.transform.position, Quaternion.identity);
            _ball2.GetComponent<Ball>().isClone=true;
            _ball2.GetComponent<Rigidbody>().linearVelocity = Quaternion.Euler(0, 0, -30) * _currentVelocity;
        }
    }
}
