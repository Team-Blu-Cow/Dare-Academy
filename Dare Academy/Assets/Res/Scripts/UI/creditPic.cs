using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class creditPic : MonoBehaviour
{
    [SerializeField] Texture2D mainTex;
    [SerializeField] Texture2D altTex;
    [SerializeField] float balance;

    float count = 0;
    [SerializeField] float swapFreaquency = 1f;
    [SerializeField] float swapTime = 0.1f;

    [SerializeField] Material mat;

    private void Start()
    {
        mat = Instantiate(GetComponent<Image>().material);
        mat.SetFloat("_SomeProperty", 1f);
        GetComponent<Image>().material = mat;

        mat.SetTexture("_MainTex", mainTex);
        mat.SetTexture("_AltTex", altTex);

        count = (float)RandWrapper(0, 100) / 100f;
        swapFreaquency = (float)RandWrapper(80, 120) / 100f;
    }

    private int RandWrapper(int min, int max)
    {
        var rand = new System.Random(gameObject.GetInstanceID()*Mathf.RoundToInt( Time.realtimeSinceStartup + Time.time));

        return rand.Next(min, max);
    }

    private void Update()
    {
        count += Time.deltaTime;

        if(count >= ((balance < 0.5f)? swapFreaquency/4 : swapFreaquency))
        {
            count = 0;

            LeanTween.value(balance, (balance > 0.5f) ? 0 : 1, swapTime)
                .setOnUpdate((float value) =>
                {
                    balance = value;
                    mat.SetFloat("_Balance", value);
                });
        }

    }
}
