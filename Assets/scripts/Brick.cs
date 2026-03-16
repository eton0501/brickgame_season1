using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class Brick : MonoBehaviour
{
    public int hits=1;
    public int points=100;
    public Vector3 rotator;
    public Material hitMaterial;
    Material _orgMaterial;
    Renderer _renderer;
    public GameObject[] propPrefabs;
    void Start()
    {
        transform.Rotate(rotator*(transform.position.x+transform.position.y)*0.1f);//一開始先讓每個磚塊轉(自身的X跟Y座標加起來去乘上0.1f)度
        _renderer=GetComponent<Renderer>();//去抓取Brick裡面的Renderer物件
        _orgMaterial=_renderer.sharedMaterial;//先讓_orgMaterial等於遊戲一開始磚塊的材質
    }

    
    void Update()
    {
        transform.Rotate(rotator*Time.deltaTime);//磚塊每一偵會轉rotator這個(x,y,z)乘上deltatime的角度
    }
    private void OnCollisionEnter(Collision collision)
    {
        hits--;
        if (hits <= 0)
        {
            if (UnityEngine.Random.Range(0f, 1f) <= 0.8f)
            {
                int randomIndex=UnityEngine.Random.Range(0,propPrefabs.Length);
                Instantiate(propPrefabs[randomIndex],transform.position,Quaternion.Euler(0,0,90));
            }
            GameManager.Instance.Score+=points;
            Destroy(gameObject);
        }
        _renderer.sharedMaterial=hitMaterial;//把材質改為hitMaterial
        Invoke("RestoreMaterial",0.05f);//在0.05f過後呼叫RestoreMaterial方式
    }
    void RestoreMaterial()
    {
        _renderer.sharedMaterial=_orgMaterial;//把材質改為原本的_orgMaterial
    }
}
