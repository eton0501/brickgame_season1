using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    Vector3 _originalScale;
    Rigidbody _rigidbody;
    Coroutine _expandCoroutine;
    void Start()
    {
        _originalScale=transform.localScale;
        _rigidbody=GetComponent<Rigidbody>();
    }

    
    void FixedUpdate()
    {
        float mouseWorldX = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, 0, 50)).x;
        float clampedX=Mathf.Clamp(mouseWorldX,-32.44f,32.44f);
        _rigidbody.MovePosition(new Vector3(clampedX,-17f,0f));
    }
    public void ActivateExpand(float duration)
    {
        if (_expandCoroutine != null)
        {
            StopCoroutine(_expandCoroutine);
        }
        _expandCoroutine=StartCoroutine(ExpandRoutine(duration));
    }
    IEnumerator ExpandRoutine(float duration)
    {
        transform.localScale=new Vector3(_originalScale.x*2f,_originalScale.y,_originalScale.z);
        yield return new WaitForSeconds(duration);
        transform.localScale=_originalScale;
        _expandCoroutine=null;
    }
}
