﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellController : MonoBehaviour
{
    public float deleteTime = 3.0f; // 제거할 시간 지정

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, deleteTime); // 제거 설정
    }   

    void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject); // 무언가에 접촉하면 제거
    }
}
