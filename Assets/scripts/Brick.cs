using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class Brick : MonoBehaviour
{
    public int hits=1;//磚塊的血量
    public int points=100;//打破磚塊的分數
    public Vector3 rotator;//磚塊自轉的速度和方向
    public Material hitMaterial;//擊中時的材質
    Material _orgMaterial;//原始材質
    Renderer _renderer;
    public GameObject[] propPrefabs;//各種道具的陣列
    public GameObject explosionEffect;//粒子效果
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
    private void OnCollisionEnter(Collision collision)//撞擊的時候
    {
        hits--;//血量減1
        if (hits <= 0)//如果血量小於等於0
        {
            if (UnityEngine.Random.Range(0f, 1f) <= 0.2f)//會隨機抽0到1的小數，如果小於等於0.2就會掉道具
            {
                int randomIndex=UnityEngine.Random.Range(0,propPrefabs.Length);//從道具的陣列中隨機抽出一個
                Instantiate(propPrefabs[randomIndex],transform.position,Quaternion.Euler(0,0,90));//在磚塊目前的位置生成抽中的道具，並且將z軸旋轉90度
            }
            GameManager.Instance.Score+=points;//加分數
            Instantiate(explosionEffect, transform.position, transform.rotation);//在磚塊死亡時呼叫一個粒子效果
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
