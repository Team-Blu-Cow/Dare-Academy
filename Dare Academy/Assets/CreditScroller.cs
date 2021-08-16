using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditScroller : MonoBehaviour
{
    RectTransform rectTransform;

    [SerializeField] float scrollSpeed;
    [SerializeField] float startHeight;
    [SerializeField] float height;
    [SerializeField] float currentHeight;

    [SerializeField] GameObject sceneSwitcher;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        startHeight = rectTransform.anchoredPosition.y;

    }

    // Update is called once per frame
    void Update()
    {
        currentHeight = rectTransform.anchoredPosition.y + (Time.deltaTime * scrollSpeed);

        rectTransform.anchoredPosition = new Vector2(
            rectTransform.anchoredPosition.x,
            currentHeight);

        if(currentHeight >= startHeight+height)
        {
            sceneSwitcher.SetActive(true);
        }
    }
}
