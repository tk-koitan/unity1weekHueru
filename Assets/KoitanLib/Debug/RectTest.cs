using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RectTest : MonoBehaviour
{
    RectTransform rectTransform;
    Image image;
    Vector2 mpos = new Vector2(0, 0);
    [SerializeField]
    private Vector2 defaultSize = new Vector2(100f, 100f);
    [SerializeField]
    private Vector2 targetSize = new Vector2(500f, 500f);
    private OpenState openState = OpenState.Closed;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    private void Start()
    {
        KoitanLib.Debug.Display(() => "mouse:" + mpos, this);

        //初期化
        rectTransform.sizeDelta = defaultSize;
        openState = OpenState.Closed;
    }

    // Update is called once per frame
    void Update()
    {
        mpos = Input.mousePosition;
        mpos.x /= Screen.width;
        mpos.y /= Screen.height;
        mpos.x *= 1920f;
        mpos.y *= 1080f;
        Vector2 min = rectTransform.anchoredPosition + rectTransform.rect.min;
        Vector2 max = rectTransform.anchoredPosition + rectTransform.rect.max;
        if (min.x < mpos.x && mpos.x < max.x && min.y < mpos.y && mpos.y < max.y)
        {
            if (openState == OpenState.Closed)
            {
                openState = OpenState.Opening;
                rectTransform.DOSizeDelta(targetSize, 0.5f).OnComplete(() => openState = OpenState.Opened);
            }

            image.color = Color.red;
            if (Input.GetMouseButtonDown(0))
            {
                transform.DOPunchScale(Vector3.one, 1f);
            }
        }
        else
        {
            image.color = Color.white;
            if (openState == OpenState.Opened)
            {
                openState = OpenState.Closing;
                rectTransform.DOSizeDelta(defaultSize, 0.5f).OnComplete(() => openState = OpenState.Closed);
            }
        }
    }

    /*
    private void OnDrawGizmos()
    {
        var r = GetComponent<RectTransform>();
        Debug.Log("Min:" + r.rect.min);
        Debug.Log("Max:" + r.rect.max);
        Debug.Log("Anc:" + r.anchoredPosition);
        Gizmos.DrawLine(r.anchoredPosition + r.rect.min, r.anchoredPosition + r.rect.position + r.rect.max);
    }
    */

    enum OpenState
    {
        Closed,
        Opening,
        Opened,
        Closing
    }
}
