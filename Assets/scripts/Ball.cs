using Unity.VisualScripting;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public bool isClone=false;
    float _baseSpeed=20f;
    float _currentSpeed;
    float _speedIncrease=2f;
    public float _maxSpeed=35f;
    Rigidbody _rigidbody;
    Vector3 _velocity;
    Renderer _renderer;
    AudioSource _audioSource;
    void Start()
    {
        _rigidbody=GetComponent<Rigidbody>();
        _renderer=GetComponent<Renderer>();
        _audioSource=GetComponent<AudioSource>();
        _currentSpeed=_baseSpeed;
        if (!isClone)
        {
            Invoke("Launch",0.5f);//等待0.5f之後才執行Launch方法
        }
    }
    void Launch()
    {
        _rigidbody.linearVelocity=new Vector3(1f,-1f,0f).normalized*_currentSpeed;//將球往右下方向乘上speed並且利用normalized確保速度都是一致的
    }

    
    void FixedUpdate()
    {
        Vector3 currentV=_rigidbody.linearVelocity;
        if (Mathf.Abs(currentV.y) < 2f)
        {
            float signY=currentV.y!=0?Mathf.Sign(currentV.y):1f;
            currentV.y=signY*2f;
        }
        _rigidbody.linearVelocity=currentV.normalized*_currentSpeed;
        _velocity=_rigidbody.linearVelocity;//把撞擊前的速度計到_velocity
        if (!_renderer.isVisible)//renderer.isVisible(代表說這個物體現在有沒有被攝影機看到，傳回True或是False)
        {
            GameObject[] ballsInScene=GameObject.FindGameObjectsWithTag("Ball");
            if (ballsInScene.Length > 1)
            {
                Destroy(gameObject);
            }
            else
            {
                GameManager.Instance.Balls--;//呼叫GameManager去讓Balls減一
                Destroy(gameObject);//摧毀這個物體
            }
            
        }
    }
    private void OnCollisionEnter(Collision collision)//當碰撞的時候
    {
        _rigidbody.linearVelocity=Vector3.Reflect(_velocity,collision.contacts[0].normal);//Vector3.Reflect(撞擊的方向以及速度,牆壁的角度),collision(撞擊的時候).contacts[0](撞擊的第一個點).normal(法線)
        if (collision.gameObject.CompareTag("Player"))
        {
            _currentSpeed=_baseSpeed;
        }
        else
        {
            _currentSpeed+=_speedIncrease;
            if (_currentSpeed >= _maxSpeed)
            {
                _currentSpeed=_maxSpeed;
            }
        }
        if (_audioSource != null)
        {
            _audioSource.Play();
        }
    }
}
