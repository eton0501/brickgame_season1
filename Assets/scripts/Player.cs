using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    Vector3 _originalScale;//抓取目前玩家的大小
    Rigidbody _rigidbody;
    Coroutine _expandCoroutine;//宣告一個倒計時變數
    void Start()
    {
        _originalScale=transform.localScale;//一開始先把大小記下來
        _rigidbody=GetComponent<Rigidbody>();
    }

    
    void FixedUpdate()
    {
        float mouseWorldX = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, 0, 50)).x;//抓取目前滑鼠位置對應世界的X座標
        float clampedX=Mathf.Clamp(mouseWorldX,-32.44f,32.44f);//將玩家的x座標限制在-32.44f到32.44f
        _rigidbody.MovePosition(new Vector3(clampedX,-17f,0f));//移動玩家到指定位置，限制y座標為-17，z座標為0
    }
    public void ActivateExpand(float duration)//玩家變寬的方法
    {
        if (_expandCoroutine != null)//如果現在已經有一個變寬的協程在跑了
        {
            StopCoroutine(_expandCoroutine);//直接把舊的協程取消
        }
        _expandCoroutine=StartCoroutine(ExpandRoutine(duration));//啟動一個新的協程
    }
    IEnumerator ExpandRoutine(float duration)
    {
        transform.localScale=new Vector3(_originalScale.x*2f,_originalScale.y,_originalScale.z);//把玩家的X軸的長度變為兩倍
        yield return new WaitForSeconds(duration);//在背後倒計時
        transform.localScale=_originalScale;//時間到就回復原狀
        _expandCoroutine=null;//清空協程
    }
}
