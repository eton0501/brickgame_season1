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
    private bool _isLaunched=false;
    public float yOffset = 1.0f;
    void Start()
    {
        _rigidbody=GetComponent<Rigidbody>();
        _renderer=GetComponent<Renderer>();
        _audioSource=GetComponent<AudioSource>();
        _currentSpeed=_baseSpeed;
        if (!isClone)
        {
            GameObject player=GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _rigidbody.isKinematic=true;
                transform.position=player.transform.position+new Vector3(0f,yOffset,0f);
                transform.SetParent(player.transform);
            }
            _isLaunched=false;
        }
        else
        {
            _isLaunched=true;
            Launch();
        }
    }
    void Update()
    {
        if (!_isLaunched && !isClone)
        {
            if (Input.GetMouseButton(0) || Input.GetKeyDown(KeyCode.Space))
            {
                _isLaunched=true;
                transform.SetParent(null);
                Launch();
            }
        }
    }
    void Launch()
    {
        _rigidbody.isKinematic=false;
        float randomX=Random.Range(-0.5f,0.5f);
        _rigidbody.linearVelocity=new Vector3(randomX,-1f,0f).normalized*_currentSpeed;//將球往右下方向乘上speed並且利用normalized確保速度都是一致的
    }

    
    void FixedUpdate()
    {
        if(!_isLaunched)return;
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
        if(!_isLaunched)return;
        if (collision.gameObject.CompareTag("Player"))
        {
            _currentSpeed=_baseSpeed;
            float offset=transform.position.x-collision.transform.position.x;
            float paddleWidth=collision.collider.bounds.size.x;
            float hitFactor=(offset/paddleWidth)*2f;
            Vector3 newDirection=new Vector3(hitFactor*1.5f,1f,0f).normalized;
            _rigidbody.linearVelocity=newDirection*_currentSpeed;
        }
        else
        {
            _rigidbody.linearVelocity=Vector3.Reflect(_velocity,collision.contacts[0].normal);//Vector3.Reflect(撞擊的方向以及速度,牆壁的角度),collision(撞擊的時候).contacts[0](撞擊的第一個點).normal(法線)
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
